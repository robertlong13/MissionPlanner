// DROPME: TCP debug listener for exercising MessageRateManager against SITL.
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Comms;
using static MAVLink;

namespace MissionPlanner.Utilities
{
    public class RateManagerDebugListener : IDisposable
    {
        private const int DefaultPort = 7400;

        private readonly int _port;
        private TcpListener _listener;
        private CancellationTokenSource _cts;
        private Task _acceptLoop;

        private readonly Dictionary<int, MessageRateLease> _leases = new Dictionary<int, MessageRateLease>();
        private int _nextId = 1;

        private LossyCommsSerial _lossy;
        private Comms.ICommsSerial _originalBaseStream;

        public RateManagerDebugListener(int port = DefaultPort)
        {
            _port = port;
        }

        public void Start()
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Loopback, _port);
            _listener.Start();
            _acceptLoop = Task.Run(() => AcceptLoopAsync(_cts.Token));
            Console.WriteLine($"RateManagerDebugListener on port {_port}");
        }

        private async Task AcceptLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                    _ = Task.Run(() => HandleClient(client, ct));
                }
                catch (ObjectDisposedException) { break; }
                catch (SocketException) { break; }
            }
        }

        private void HandleClient(TcpClient client, CancellationToken ct)
        {
            using (client)
            using (var stream = client.GetStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream) { AutoFlush = true })
            {
                writer.WriteLine("RateManager debug listener. Type 'help' for commands.");

                while (!ct.IsCancellationRequested)
                {
                    string line;
                    try
                    {
                        line = reader.ReadLine();
                    }
                    catch { break; }

                    if (line == null)
                        break;

                    line = line.Trim();
                    if (line.Length == 0)
                        continue;

                    try
                    {
                        var response = Dispatch(line);
                        writer.WriteLine(response);

                        if (line.Equals("quit", StringComparison.OrdinalIgnoreCase))
                            break;
                    }
                    catch (Exception ex)
                    {
                        writer.WriteLine($"ERROR: {ex.Message}");
                    }
                }
            }
        }

        private string Dispatch(string line)
        {
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "help":
                    return Help();
                case "targets":
                    return CmdTargets();
                case "sub":
                    return CmdSubscribe(parts);
                case "release":
                    return CmdRelease(parts);
                case "release-all":
                    return CmdReleaseAll();
                case "state":
                    return CmdState();
                case "rates":
                    return CmdRates(parts);
                case "leases":
                    return CmdLeases();
                case "lossy":
                    return CmdLossy(parts);
                case "tick":
                    return CmdTick();
                case "tick-interval":
                    return CmdTickInterval(parts);
                case "sleep":
                    return CmdSleep(parts);
                case "quit":
                    return "bye";
                default:
                    return $"Unknown command: {cmd}. Type 'help'.";
            }
        }

        private static string Help()
        {
            return string.Join("\n",
                "targets                          - list all sysid.compid pairs",
                "sub <msgId> <hz> [owner] [s.c]   - subscribe (s.c = sysid.compid)",
                "release <id>                     - dispose lease by ID",
                "release-all                      - dispose all debug leases",
                "state                            - dump RateManager internal state",
                "rates [msgId] [s.c]              - show observed Hz (s.c = sysid.compid)",
                "leases                           - list debug leases held by this listener",
                "lossy wrap                - wrap BaseStream in LossyCommsSerial",
                "lossy read <pct>          - set whole-read drop %",
                "lossy read-byte <pct>     - set byte corruption %",
                "lossy write <pct>         - set write drop %",
                "lossy off                 - set all rates to 0",
                "lossy unwrap              - restore original BaseStream",
                "tick                      - force an immediate tick + retry",
                "tick-interval <ms>        - change tick loop interval",
                "sleep <ms>                - pause",
                "quit                      - close connection");
        }

        // --- Commands ---

        private string CmdTargets()
        {
            var port = MainV2.comPort;
            if (port == null)
                return "ERROR: not connected";

            var lines = new List<string>();
            foreach (var mav in port.MAVlist)
            {
                var current = (mav.sysid == port.MAV.sysid && mav.compid == port.MAV.compid)
                    ? " (current)" : "";
                lines.Add($"  {mav.sysid}.{mav.compid}{current}");
            }
            return lines.Count > 0
                ? $"Targets ({lines.Count}):\n" + string.Join("\n", lines)
                : "No targets";
        }

        private bool TryParseTarget(string sc, out byte sysid, out byte compid)
        {
            sysid = 0; compid = 0;
            var dot = sc.Split('.');
            if (dot.Length != 2) return false;
            if (!byte.TryParse(dot[0], out sysid)) return false;
            if (!byte.TryParse(dot[1], out compid)) return false;
            return true;
        }

        private string CmdSubscribe(string[] parts)
        {
            if (parts.Length < 3)
                return "Usage: sub <msgId> <hz> [owner] [sysid.compid]";

            if (!uint.TryParse(parts[1], out var msgId))
                return $"Bad msgId: {parts[1]}";
            if (!double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var hz))
                return $"Bad hz: {parts[2]}";

            var owner = parts.Length > 3 ? parts[3] : $"debug-{_nextId}";

            var mgr = MainV2.comPort?.RateManager;
            if (mgr == null)
                return "ERROR: no RateManager (not connected?)";

            var mav = MainV2.comPort.MAV;
            byte sysid = mav.sysid, compid = mav.compid;

            // Check if last arg is sysid.compid
            if (parts.Length > 3 && TryParseTarget(parts[parts.Length - 1], out var s, out var c))
            {
                sysid = s; compid = c;
                // If owner was the target arg, use default owner
                if (parts.Length == 4)
                    owner = $"debug-{_nextId}";
            }

            var lease = mgr.Subscribe(sysid, compid,
                (MAVLINK_MSG_ID)msgId, hz, owner);

            var id = _nextId++;
            _leases[id] = lease;
            return $"OK lease={id} msg={msgId} hz={hz:F1} owner={owner} target={sysid}.{compid}";
        }

        private string CmdRelease(string[] parts)
        {
            if (parts.Length < 2)
                return "Usage: release <id>";
            if (!int.TryParse(parts[1], out var id))
                return $"Bad id: {parts[1]}";
            if (!_leases.TryGetValue(id, out var lease))
                return $"No lease with id={id}";

            if (lease.Released != 0)
            {
                _leases.Remove(id);
                return $"Lease {id} already released";
            }

            lease.Dispose();
            _leases.Remove(id);
            return $"OK released {id}";
        }

        private string CmdReleaseAll()
        {
            int count = 0;
            foreach (var kv in _leases)
            {
                if (kv.Value.Released == 0)
                {
                    kv.Value.Dispose();
                    count++;
                }
            }
            _leases.Clear();
            return $"OK released {count} lease(s)";
        }

        private string CmdState()
        {
            var mgr = MainV2.comPort?.RateManager;
            return mgr == null ? "ERROR: no RateManager" : mgr.DumpState();
        }

        private string CmdRates(string[] parts)
        {
            var port = MainV2.comPort;
            if (port == null)
                return "ERROR: not connected";

            var mgr = port.RateManager;

            // Resolve target: last arg may be sysid.compid
            byte sysid = port.MAV.sysid, compid = port.MAV.compid;
            if (parts.Length >= 2 && TryParseTarget(parts[parts.Length - 1], out var s, out var c))
            {
                sysid = s; compid = c;
            }

            var mav = port.MAVlist[sysid, compid];

            if (parts.Length >= 2 && uint.TryParse(parts[1], out var msgId))
            {
                mav.packetspersecond.TryGetValue(msgId, out var hz);
                mav.packetspersecondbuild.TryGetValue(msgId, out var lastTime);
                var age = lastTime == default ? "never" : $"{(DateTime.UtcNow - lastTime).TotalSeconds:F1}s ago";
                var line = $"msg {msgId} ({sysid}.{compid}): ema={hz:F2} Hz, last={age}";

                if (mgr != null)
                {
                    var (obsHz, estHz, lq) = mgr.GetMeasuredRate(msgId, sysid, compid);
                    line += $", counted={obsHz:F2} Hz, est_send={estHz:F2} Hz, lq={lq:P0}";
                }
                return line;
            }

            // Show all messages with nonzero rates
            var lines = new List<string>();
            foreach (var kv in mav.packetspersecond.OrderBy(kv => kv.Key))
            {
                if (kv.Value <= 0) continue;
                mav.packetspersecondbuild.TryGetValue(kv.Key, out var lt);
                var age = lt == default ? "?" : $"{(DateTime.UtcNow - lt).TotalSeconds:F1}s";
                lines.Add($"  msg {kv.Key}: {kv.Value:F2} Hz (last {age})");
            }
            return lines.Count > 0
                ? $"Active rates for {sysid}.{compid} ({lines.Count}):\n" + string.Join("\n", lines)
                : $"No observed rates for {sysid}.{compid}";
        }

        private string CmdLeases()
        {
            if (_leases.Count == 0)
                return "No debug leases";
            var lines = new List<string>();
            foreach (var kv in _leases.OrderBy(kv => kv.Key))
            {
                var l = kv.Value;
                lines.Add($"  {kv.Key}: msg {l.MsgId} ({l.SysId},{l.CompId}) {l.Hz:F1} Hz " +
                    $"owner={l.Owner} released={l.Released}");
            }
            return $"Debug leases ({_leases.Count}):\n" + string.Join("\n", lines);
        }

        private string CmdLossy(string[] parts)
        {
            if (parts.Length < 2)
                return "Usage: lossy <wrap|read|read-byte|write|off|unwrap> [pct]";

            var sub = parts[1].ToLowerInvariant();

            switch (sub)
            {
                case "wrap":
                    return LossyWrap();
                case "unwrap":
                    return LossyUnwrap();
                case "off":
                    return LossySetAll(0);
                default:
                    if (parts.Length < 3 || !double.TryParse(parts[2],
                        NumberStyles.Float, CultureInfo.InvariantCulture, out var pct))
                        return "Usage: lossy <read|read-byte|write> <pct>";
                    return LossySet(sub, pct / 100.0);
            }
        }

        private static readonly FieldInfo BaseStreamField =
            typeof(MAVLinkInterface).GetField("_baseStream",
                BindingFlags.NonPublic | BindingFlags.Instance);

        private string LossyWrap()
        {
            if (_lossy != null)
                return "Already wrapped";
            var port = MainV2.comPort;
            if (port == null)
                return "ERROR: not connected";

            _originalBaseStream = port.BaseStream;
            _lossy = new LossyCommsSerial(_originalBaseStream);
            BaseStreamField.SetValue(port, _lossy);
            return "OK: BaseStream wrapped in LossyCommsSerial";
        }

        private string LossyUnwrap()
        {
            if (_lossy == null)
                return "Not wrapped";
            var port = MainV2.comPort;
            if (port == null || _originalBaseStream == null)
                return "ERROR: can't unwrap";

            BaseStreamField.SetValue(port, _originalBaseStream);
            _lossy = null;
            _originalBaseStream = null;
            return "OK: original BaseStream restored";
        }

        private string LossySet(string which, double rate)
        {
            if (_lossy == null)
                return "Not wrapped. Run 'lossy wrap' first.";

            switch (which)
            {
                case "read":
                    _lossy.ReadDropRate = rate;
                    return $"OK: ReadDropRate={rate:P0}";
                case "read-byte":
                    _lossy.ReadByteCorruptRate = rate;
                    return $"OK: ReadByteCorruptRate={rate:P0}";
                case "write":
                    _lossy.WriteDropRate = rate;
                    return $"OK: WriteDropRate={rate:P0}";
                default:
                    return $"Unknown lossy mode: {which}";
            }
        }

        private string LossySetAll(double rate)
        {
            if (_lossy == null)
                return "Not wrapped";
            _lossy.ReadDropRate = rate;
            _lossy.ReadByteCorruptRate = rate;
            _lossy.WriteDropRate = rate;
            return $"OK: all rates={rate:P0}";
        }

        private string CmdTick()
        {
            var mgr = MainV2.comPort?.RateManager;
            if (mgr == null)
                return "ERROR: no RateManager";
            mgr.ForceTick().Wait();
            return "OK: tick complete";
        }

        private string CmdTickInterval(string[] parts)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out var ms))
                return "Usage: tick-interval <ms>";
            var mgr = MainV2.comPort?.RateManager;
            if (mgr == null)
                return "ERROR: no RateManager";
            mgr.SetTickInterval(ms);
            return $"OK: tick interval={ms} ms";
        }

        private static string CmdSleep(string[] parts)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out var ms))
                return "Usage: sleep <ms>";
            Thread.Sleep(ms);
            return $"OK: slept {ms} ms";
        }

        public void Dispose()
        {
            _cts?.Cancel();
            _listener?.Stop();
            _cts?.Dispose();

            // Release any leases we still hold
            foreach (var kv in _leases)
            {
                if (kv.Value.Released == 0)
                    kv.Value.Dispose();
            }
            _leases.Clear();

            LossyUnwrap();
        }
    }
}

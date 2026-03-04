// DROPME: Debug-only ICommsSerial decorator for simulating link degradation.
using System;
using System.IO;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Wraps an <see cref="ICommsSerial"/> and probabilistically drops or corrupts data.
    /// </summary>
    public class LossyCommsSerial : ICommsSerial
    {
        private readonly ICommsSerial _inner;
        private readonly Random _rng = new Random();

        /// <summary>Probability (0–1) of dropping an entire Read() call (return 0 bytes).</summary>
        public double ReadDropRate { get; set; }

        /// <summary>Probability (0–1) of corrupting a random byte in a Read() result.</summary>
        public double ReadByteCorruptRate { get; set; }

        /// <summary>Probability (0–1) of silently discarding a Write() call.</summary>
        public double WriteDropRate { get; set; }

        public ICommsSerial Inner => _inner;

        public LossyCommsSerial(ICommsSerial inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        // --- Drop/corrupt logic ---

        public int Read(byte[] buffer, int offset, int count)
        {
            if (ReadDropRate > 0 && _rng.NextDouble() < ReadDropRate)
                return 0;

            int n = _inner.Read(buffer, offset, count);

            if (n > 0 && ReadByteCorruptRate > 0 && _rng.NextDouble() < ReadByteCorruptRate)
                buffer[offset + _rng.Next(n)] ^= 0xFF;

            return n;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (WriteDropRate > 0 && _rng.NextDouble() < WriteDropRate)
                return;
            _inner.Write(buffer, offset, count);
        }

        public void Write(string text)
        {
            if (WriteDropRate > 0 && _rng.NextDouble() < WriteDropRate)
                return;
            _inner.Write(text);
        }

        public void WriteLine(string text)
        {
            if (WriteDropRate > 0 && _rng.NextDouble() < WriteDropRate)
                return;
            _inner.WriteLine(text);
        }

        // --- Pure delegation ---

        public Stream BaseStream => _inner.BaseStream;
        public int BaudRate { get => _inner.BaudRate; set => _inner.BaudRate = value; }
        public int BytesToRead => _inner.BytesToRead;
        public int BytesToWrite => _inner.BytesToWrite;
        public int DataBits { get => _inner.DataBits; set => _inner.DataBits = value; }
        public bool DtrEnable { get => _inner.DtrEnable; set => _inner.DtrEnable = value; }
        public bool IsOpen => _inner.IsOpen;
        public string PortName { get => _inner.PortName; set => _inner.PortName = value; }
        public int ReadBufferSize { get => _inner.ReadBufferSize; set => _inner.ReadBufferSize = value; }
        public int ReadTimeout { get => _inner.ReadTimeout; set => _inner.ReadTimeout = value; }
        public bool RtsEnable { get => _inner.RtsEnable; set => _inner.RtsEnable = value; }
        public int WriteBufferSize { get => _inner.WriteBufferSize; set => _inner.WriteBufferSize = value; }
        public int WriteTimeout { get => _inner.WriteTimeout; set => _inner.WriteTimeout = value; }

        public void Open() => _inner.Open();
        public void Close() => _inner.Close();
        public int ReadByte() => _inner.ReadByte();
        public int ReadChar() => _inner.ReadChar();
        public string ReadExisting() => _inner.ReadExisting();
        public string ReadLine() => _inner.ReadLine();
        public void DiscardInBuffer() => _inner.DiscardInBuffer();
        public void toggleDTR() => _inner.toggleDTR();

        public void Dispose()
        {
            _inner.Dispose();
        }
    }
}

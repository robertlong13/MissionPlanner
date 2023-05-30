using System;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Controls;
using MissionPlanner.Comms;
using System.Net;
using System.Net.Sockets;

/*
 * 
 * This plugin automatically creates a MAVLink mirror connection over UDP to a specified
 * IP address and port. This mirror stream is created independently of the built-in
 * functionality, and can coexist with it. 
 * 
 * Pay close attention to the comments in all-caps to adjust this plugin to suit your needs.
 * 
 */

namespace AutoForward
{
    public class AutoForward : Plugin
    {
        // SET THE ADDRESS AND PORT TO WHERE YOU WANT TO FORWARD THE MAVLINK STREAM
        string address = "127.0.0.1";
        int port = 14550;

        // SET THIS TO true IF YOU WANT TO GIVE THE DESITNATION WRITE ACCESS
        // (this gives the destination the possibility for full control of the aircraft;
        // this could be dangerous; use with caution)
        bool allowWrites = false;

        private string _Name = "Auto Forward MAVLink";
        private string _Version = "0.1";
        private string _Author = "Bob Long";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        // CHANGE THIS TO true TO USE THIS PLUGIN
        public override bool Init() { return false; }

        ICommsSerial mirrorstream;

        public override bool Loaded()
        {
            // Try to open the connection and display a window with an error message on failure
            try
            {
                var udp = new UdpSerialConnect() { ConfigRef = "SerialOutputPassUDPCL" };
                udp.hostEndPoint = new IPEndPoint(IPAddress.Parse(address), port);
                udp.client = new UdpClient();
                udp.IsOpen = true;
                mirrorstream = udp;
                Host.comPort.OnPacketReceived += mirror_OnPacketReceived;
            }
            catch (Exception e)
            {
                CustomMessageBox.Show("Error opening UDP connection: " + e.Message, Name + " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }


            return true;
        }

        public override bool Exit() { return true; }

        private void mirror_OnPacketReceived(object sender, MAVLink.MAVLinkMessage e)
        {
            try
            {
                var buffer = e.buffer;

                // full rw from mirror stream
                if (mirrorstream != null && mirrorstream.IsOpen)
                {
                    mirrorstream.Write(buffer, 0, buffer.Length);

                    while (mirrorstream.BytesToRead > 0)
                    {
                        var len = mirrorstream.BytesToRead;

                        byte[] buf = new byte[len];

                        len = mirrorstream.Read(buf, 0, len);

                        if (allowWrites)
                        {
                            lock (Host.comPort.writelock)
                            {
                                Host.comPort.BaseStream.Write(buf, 0, len);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }
}
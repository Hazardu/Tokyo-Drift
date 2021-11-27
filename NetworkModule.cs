using Bolt;
using System;
using System.IO;

namespace TokyoDrift
{
    public class NetworkConfig
    {
        public const ulong ModNetworkPacked = 999999422;
        public readonly static NetworkId ModNetworkID = new NetworkId(ModNetworkPacked);
        internal const string ModName = "TurtleShellMod Network Data";
    }





    //copy paste all of this below

    public class NetworkCallbacksServerMod : CoopServerCallbacks
    {
        public override void OnEvent(ChatEvent evnt)
        {
            if (evnt.Sender == NetworkConfig.ModNetworkID)
            {
                NetworkManager.RecieveCommand(NetworkManager.StringToBytes(evnt.Message));
                return;
            }
            base.OnEvent(evnt);
        }
    }
    public class NetworkCallbacksClientMod : CoopPlayerCallbacks
    {
        public override void OnEvent(ChatEvent evnt)
        {
            if (evnt.Sender == NetworkConfig.ModNetworkID)
            {
                NetworkManager.RecieveCommand(NetworkManager.StringToBytes(evnt.Message));
                return;
            }
            base.OnEvent(evnt);
        }
    }


    public static class NetworkManager
    {
        public static byte[] StringToBytes(string cmd)
        {
            var a = cmd.ToCharArray();
            var b = new byte[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                b[i] = (byte)a[i];
            }
            return b;
        }
        public static string BytesToString(byte[] b)
        {
            string s = string.Empty;
            for (int i = 0; i < b.Length; i++)
            {
                s += (char)b[i];
            }
            return s;
        }


        /// <summary>
        /// Sends bytes to targets, calls RecieveCommand on them
        /// </summary>
        /// <param name="bytearray">Bytes to send</param>
        /// <param name="target">Choose between possible recievers</param>
        public static void SendCommand(byte[] bytearray, GlobalTargets target)
        {
            if (BoltNetwork.isRunning)
            {
                ChatEvent chatEvent = ChatEvent.Create(target);
                chatEvent.Message = BytesToString(bytearray);
                chatEvent.Sender = NetworkConfig.ModNetworkID;
                chatEvent.Send();
            }
        }


        public static void RecieveCommand(byte[] b)
        {
            using (MemoryStream stream = new MemoryStream(b))
            {
                using (BinaryReader r = new BinaryReader(stream))
                {
                    int cmdIndex = r.ReadInt32();
                    try
                    {
                        switch (cmdIndex)
                        {
                            case 1:
                                ShellModOptionsGUI.Reset();
                                ModSettings.bounciness = r.ReadSingle();
                                ModSettings.acceleration = r.ReadSingle();
                                ModSettings.bouncetimer = r.ReadSingle();
                                ModSettings.friction= r.ReadSingle();
                                ModSettings.turnRate = r.ReadSingle();
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        ModAPI.Log.Write("Network error: " + cmdIndex + "\n\n" + e.ToString());
                    }
                    r.Close();
                }
                stream.Close();
            }
        }

    }
}


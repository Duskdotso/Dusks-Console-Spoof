using System.Linq;
using BepInEx;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace DusksConsoleSpoof
{
    [BepInPlugin("com.dusk.consolespoof", "Dusks Console Spoof", Version)]
    internal class Plugin : BaseUnityPlugin
    {
        public const string Version = "1.0.0";
        
        private void OnEnable() => PhotonNetwork.NetworkingClient.EventReceived += EventReceived;
        void OnDisable() => PhotonNetwork.NetworkingClient.EventReceived -= EventReceived;

        #region ConsoleSpoof

        public static void EventReceived(EventData data) // this is where the actual spoof thing happens!
        {
            try
            {
                if (data.Code != 68) return;
                Player sender = PhotonNetwork.NetworkingClient.CurrentRoom.GetPlayer(data.Sender);

                object[] args = data.CustomData == null ? new object[] { } : (object[])data.CustomData;
                string command = args.Length > 0 ? (string)args[0] : "";

                if (command == "confirmusing")
                {
                    ExecuteCommand("confirmusing", sender.ActorNumber, "1.0.0",
                        GetConsoleReply());
                }
            }
            catch
            {
            }
        }

        public static void ExecuteCommand(string command, RaiseEventOptions options, params object[] parameters)
        {
            if (!PhotonNetwork.InRoom)
                return;

            PhotonNetwork.RaiseEvent(68,
                new object[] { command }
                    .Concat(parameters)
                    .ToArray(),
                options, SendOptions.SendReliable);
        }

        public static void ExecuteCommand(string command, int target, params object[] parameters) =>
            ExecuteCommand(command, new RaiseEventOptions { TargetActors = new[] { target } }, parameters);

        #endregion
        
        Rect windowRect = new Rect(20, 20, 250, 220);

        void OnGUI()
        {
            windowRect.height = customSize ? 230 : 160;
            windowRect = GUI.Window(1, windowRect, Window, $"Dusk's Console Spoof V{Version}");
        }

        private static string text = "";
        private static string Size = "";
        private static string ActualTextForConsole = "Super Homo Pimp Menu V69"; // hehe
        static int ActualSize = 0;
        private static bool customSize = true;

        void Window(int id)
        {
            GUILayout.BeginVertical();

            GUILayout.Label($"Current Text: {ActualTextForConsole}");
            if (customSize)
                GUILayout.Label($"Current Size: {ActualSize}");
            
            GUILayout.Space(10);
            
            text = GUILayout.TextField(text);
            if (GUILayout.Button("Set Text"))
                ActualTextForConsole = text;
            customSize = GUILayout.Toggle(customSize, "Custom Size");
            if (customSize) // this is probably really badly coded, but who cares
            {
                GUILayout.Space(10);
                Size = GUILayout.TextField(Size);
                if (GUILayout.Button("Set Size"))
                    int.TryParse(Size, out ActualSize);
            }

            GUILayout.EndVertical();

            GUI.DragWindow();
        }

        static string GetConsoleReply() // likely could be simplified... nah, too much work
        {
            string reply;
            if (customSize)
                reply = $"<size={ActualSize}>{ActualTextForConsole}</size>";
            else
                reply = ActualTextForConsole;

            return reply;
        }
    }
}

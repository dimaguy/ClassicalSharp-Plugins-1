using System;
using ClassicalSharp;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using System.IO;
using System.Collections.Generic;

namespace GlobalChatPlugin
{

    public class Core : Plugin
    {

        public string ClientVersion { get { return "0.99.9.96"; } }

        public static WebSocket websocket;
        
        Chat gameChat;
        List<string> lastMessages = new List<string>();
        bool firstConnection = true;
        public static string filePath = "./plugins/globalChat.txt";
        string version = "1.0.3";
        string MyUsername = "";
        bool crashed = false;

        public void Dispose()
        {
            websocket.Send("logout");
        }

        public void Init(Game game)
        {
            MyUsername = game.Username;
            game.Server.AppName += " + GlobalChat V" + version;

            game.AddScheduledTask(1.0 / 60, Scheduled);
            game.CommandList.Register(new GlobalChatCommand());
            game.CommandList.Register(new GlobalLoginChatCommand());
            game.CommandList.Register(new GlobalLogoutChatCommand());
            game.CommandList.Register(new GlobalRegisterChatCommand());
            game.CommandList.Register(new GlobalHelpChatCommand());
            game.CommandList.Register(new GlobalUpdateCommand());


            websocket = new WebSocket("ws://nameless-tor-48663.herokuapp.com/api/notifications/ws"); //Websocket Server
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs>(websocket_Error);
            websocket.Closed += new EventHandler(websocket_Closed);
            websocket.Open();

            gameChat = game.Chat;

        }
        
        void Scheduled(ScheduledTask task)
        {
            try {
                foreach (string i in lastMessages)
                {
                    gameChat.Add(i);
                    lastMessages.Remove(i);
                }
            }
            catch{ }

        }

        public void Ready(Game game)
        {
        }

        public void Reset(Game game) { }

        public void OnNewMap(Game game)
        {
        }

        public void OnNewMapLoaded(Game game)
        {
        }

        public void websocket_Opened(object sender, EventArgs e)
        {


            string info = "";
            if (File.Exists(filePath))
            {
                info = File.ReadAllText(filePath);
            }
            if (firstConnection)
            {
                websocket.Send("version_" + version);
                lastMessages.Add("/client ghelp <- Global Chat Help.");
                firstConnection = false;
                if (info != "")
                {
                    if (info.Split('|')[1] != MyUsername) return;
                    websocket.Send("login_" + info);
                }
            }
            else
            {
                if (info != "")
                {
                    websocket.Send("reconnect_" + info);
                }
            }

        }

        public void websocket_Closed(object sender, EventArgs e)
        {
            if (!crashed) { 
                websocket.Open();
            }
        }

        public void websocket_Error(object sender, EventArgs e)
        {
            lastMessages.Add("&eError: Disconnected from the Global Chat.");
            crashed = true;
        }

        public void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            lastMessages.Add("&e[GLOBAL]&f " + e.Message);
        }
    }

    public class GlobalChatCommand : ClassicalSharp.Commands.Command //Send Messages
    {

        public GlobalChatCommand()
        {
            Name = "gb";
            Help = new string[] {
                "&a/client gb [message]",
                "&eSend a global message.",

            };
        }

        public override void Execute(string[] args)
        {
            args[0] = "";
            string message = "";
            string name = game.Username;

            for (int i = 0; i < args.Length; i++)
            {
                message += args[i] + " ";
            }

            Core.websocket.Send("message_" + message);

        }
    }

    public class GlobalLoginChatCommand : ClassicalSharp.Commands.Command //Chat Login
    {

        public GlobalLoginChatCommand()
        {
            Name = "glogin";
            Help = new string[] {
                "&a/client glogin [password]",
                "&eLogin to global chat.",
            };
        }

        public override void Execute(string[] args)
        {
            if (args.Length < 2) return;
            string name = game.Username;
            Core.websocket.Send("login_" + args[1] + "|" + name);
            File.WriteAllText(Core.filePath, args[1] + "|" + name);
        }
    }
    public class GlobalLogoutChatCommand : ClassicalSharp.Commands.Command //Chat Logout
    {

        public GlobalLogoutChatCommand()
        {
            Name = "glogout";
            Help = new string[] {
                "&a/client glogout [password]",
                "&eLogout from global chat.",
            };
        }

        public override void Execute(string[] args)
        {
            Core.websocket.Send("disconnect");
        }
    }

    public class GlobalRegisterChatCommand : ClassicalSharp.Commands.Command //Chat Register
    {

        public GlobalRegisterChatCommand()
        {
            Name = "gregister";
            Help = new string[] {
                "&a/client gregister [password]",
                "&e Register to global chat.",
            };
        }

        public override void Execute(string[] args)
        {
            if (args.Length < 2) return;
            string name = game.Username;
            Core.websocket.Send("register_" + args[1] + "|" + name);
        }
    }
    public class GlobalUpdateCommand : ClassicalSharp.Commands.Command //Updated Plugin Location
    {

        public GlobalUpdateCommand()
        {
            Name = "gupdate";
            Help = new string[] {
                "&a/client gupdate",
                "&e Update Global Chat.",
            };
        }

        public override void Execute(string[] args)
        {
            string LatestVersionUrl = "https://github.com/Sirvoid/ClassicalSharp-Plugins/blob/master/GlobalChat/GlobalChat.dll?raw=true";
            game.Chat.Add("The latest version of the plugin can be downloaded on:");
            game.Chat.Add(LatestVersionUrl);
        }
    }
    public class GlobalHelpChatCommand : ClassicalSharp.Commands.Command //Plugin commands Help
    {

        public GlobalHelpChatCommand()
        {
            Name = "ghelp";
            Help = new string[] {
                "&a/client ghelp",
                "&e Global Chat help.",
            };
        }

        public override void Execute(string[] args)
        {
            game.Chat.Add("/client gLogin <password> <- Login to your account.");
            game.Chat.Add("/client gLogout <- Logout from GlobalChat.");
            game.Chat.Add("/client gRegister <password> <- Register your account.");
            game.Chat.Add("/client gb <message> <- Say something in the chat.");
            game.Chat.Add("/client gUpdate <-- Tells you where you can download the latest version of the plugin");
        }
    }
}
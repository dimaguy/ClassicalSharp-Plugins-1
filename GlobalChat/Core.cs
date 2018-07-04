using System;
using ClassicalSharp;
using WebSocket4Net;
using SuperSocket.ClientEngine;

namespace GlobalChatPlugin {

	public class Core : Plugin {
		
		public string ClientVersion { get { return "0.99.9.96"; } }

        public static WebSocket websocket;

        Chat gameChat;
        string lastMessage = "";
        bool firstConnection = true;
        string myUsername = "Anonymous";

        public void Dispose() {
            websocket.Send("&e" + myUsername + " Left the Global Chat.");
        }
		
		public void Init(Game game) {
            game.AddScheduledTask(1.0/60, Scheduled);
            game.CommandList.Register(new GlobalChatCommand());
            myUsername = game.Username;

            websocket = new WebSocket("ws://nameless-tor-48663.herokuapp.com/api/notifications/ws");
            websocket.Opened += new EventHandler(websocket_Opened);
            websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
            websocket.Error += new EventHandler<ErrorEventArgs>(websocket_Error);
            websocket.Closed += new EventHandler(websocket_Closed);
            websocket.Open();
            
            gameChat = game.Chat;
            
        }

        void Scheduled(ScheduledTask task)
        {
            if(lastMessage != "") { 
                gameChat.Add(lastMessage);
                lastMessage = "";
            }

        }

        public void Ready(Game game) {
        }
		
		public void Reset(Game game) { }
		
		public void OnNewMap(Game game) {
        }
		
		public void OnNewMapLoaded(Game game) {       
        }

        public void websocket_Opened(object sender, EventArgs e)
        {
            if (firstConnection) {
                lastMessage = "Connected to the Global Chat!";
                websocket.Send("&e" + myUsername + " Joined the Global Chat!");
                firstConnection = false;
            }
        }

        public void websocket_Closed(object sender, EventArgs e)
        {
            websocket.Open();
        }

        public void websocket_Error(object sender, EventArgs e)
        {
            lastMessage = "&eError: Disconnected from the Global Chat.";
        }

        public void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
        {            
            lastMessage = "&e[GLOBAL]&f " + e.Message;
        }
    }

    public class GlobalChatCommand : ClassicalSharp.Commands.Command
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

            for (int i = 0; i < args.Length; i++) {
                message += args[i] + " ";
            }

            string name = "anonymous";
            if(game.Username != "" ) name = game.Username;
            Core.websocket.Send(name + ": " + message);
        }
    }
}

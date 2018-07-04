using System;
using ClassicalSharp;
using OpenTK;
using OpenTK.Input;
using ClassicalSharp.Entities;
using ClassicalSharp.Events;

namespace MountingPlugin {

	public class Core : Plugin {

        Game Ggame;
        public static bool MountingEnabled = false;
        bool haveModelPerm = true;

        public string ClientVersion { get { return "0.99.9.96"; } }

        public void Dispose() {
         
        }
		
		public void Init(Game game) {
            Ggame = game;
            game.CommandList.Register(new MountCommand());
            game.AddScheduledTask(1.0/60, Scheduled);
            game.Mouse.ButtonUp += MouseUp;       
        }
       
        public static int mountingId = -1;
        void MouseUp(object sender, MouseButtonEventArgs e) {
            if (!MountingEnabled) return;
            if (e.Button != MouseButton.Right) return;
            float curScale = Ggame.LocalPlayer.ModelScale.Y;
            if (mountingId == -1)
            {
                mountingId = Ggame.Entities.GetClosetPlayer(Ggame.LocalPlayer);
                if (mountingId != 255) Ggame.Server.SendChat("/model " + Ggame.Username + " sit:"+curScale);
            }
            else {
                if(mountingId != 255) Ggame.Server.SendChat("/model " + Ggame.Username + " humanoid:"+curScale);
                Ggame.LocalPlayer.Hacks.Floating = false;
                mountingId = -1;
            }
        }

        void Scheduled(ScheduledTask task)
        {
            if (!MountingEnabled) return;
            float curScale = Ggame.LocalPlayer.ModelScale.Y;
            if (mountingId != -1 && mountingId != 255) {
                if (Ggame.Entities.List[mountingId] == null) { Ggame.Server.SendChat("/model " + Ggame.Username + " humanoid");  Ggame.LocalPlayer.Hacks.Floating = false; mountingId = -1; return; }
                Ggame.LocalPlayer.Hacks.Floating = true;
                Ggame.LocalPlayer.Velocity.Y = 0;
               float offSetY = 1;
                if (Ggame.LocalPlayer.ModelScale.Y <= 0.5)
                {
                    switch (Ggame.Entities.List[mountingId].ModelName)
                    {
                        case "sit": offSetY = 1; break;
                        case "pony": offSetY = 1.5f; break;
                        case "ponysit": offSetY = 1.4f; break;
                        default: offSetY = 2; break;
                    }
                }
                else {
                    switch (Ggame.Entities.List[mountingId].ModelName)
                    {
                        case "sit":
                        case "spider":
                        case "ponysit":
                        case "pony": offSetY = 1; break;

                        default: offSetY = 1.5f; break;
                    }
                }

                Vector3 v = new Vector3(Ggame.Entities.List[mountingId].Position.X, Ggame.Entities.List[mountingId].Position.Y+offSetY, Ggame.Entities.List[mountingId].Position.Z);
                LocationUpdate update = LocationUpdate.MakePos(v, false);
                Ggame.LocalPlayer.SetLocation(update, false);
            }

        }

        public void Ready(Game game) {
        }
		
		public void Reset(Game game) { }
		
		public void OnNewMap(Game game) {
        }
		
		public void OnNewMapLoaded(Game game) {       
        }

      
    }

    public class MountCommand : ClassicalSharp.Commands.Command
    {

        public MountCommand()
        {
            Name = "mount";
            Help = new string[] {
                "&a/client mount",
                "&eEnable or Disable mounting.",
            };
        }

        public override void Execute(string[] args)
        {
            if (Core.MountingEnabled)
            {
                Core.MountingEnabled = false;
                game.Chat.Add("Mounting disabled.");
                Core.mountingId = -1;
                game.Server.SendChat("/model " + game.Username + " humanoid");
                game.LocalPlayer.Hacks.Floating = false;

            }
            else {
                Core.MountingEnabled = true;
                game.Chat.Add("Mounting enabled.");

            }

        }
    }
}

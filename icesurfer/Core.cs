using System;
using ClassicalSharp;
using OpenTK;
using OpenTK.Input;
using ClassicalSharp.Entities;
using ClassicalSharp.Events;
using ClassicalSharp.Gui;
using ClassicalSharp.Gui.Screens;
using System.IO.Compression;

    namespace IceSurferPlugin
    {

        public class Core : Plugin
        {

            Game Ggame;
            public static bool IceSurferEnabled = false;
            public static int rayon = 1;
            public static ushort block = 60;
            public static bool rainbowMode = false;

            public string ClientVersion { get { return "0.99.9.96"; } }

            public void Dispose()
            {

            }

            public void Init(Game game)
            {
                Ggame = game;
                game.CommandList.Register(new IceSurferCommand());
                game.AddScheduledTask(1.0 / 20, Scheduled);
                game.Mouse.ButtonUp += MouseUp;
            }

            void MouseUp(object sender, MouseButtonEventArgs e)
            {
           
            }

            void Scheduled(ScheduledTask task)
            {
                if (IceSurferEnabled && Ggame.IsKeyDown(Key.ShiftLeft))
                {
                    Vector3I pos;
                    for (int x = 1; x <= rayon; x++) {
                        if (rainbowMode)
                        {
                            block++;
                            if (block > 33) block = 21;
                        }
                        for (int z = 1; z <= rayon; z++)
                        {
                            
                            int halfRay = rayon == 1 ? 1 : (int)(rayon / 2);
                            

                            pos.X = (int)Ggame.LocalPlayer.Position.X - halfRay + x;
                            pos.Y = (int)Ggame.LocalPlayer.Position.Y - 1;
                            pos.Z = (int)Ggame.LocalPlayer.Position.Z - halfRay + z;

                            if (pos.Y >= Ggame.World.Height) return;
                            if (pos.X > Ggame.World.MaxX || pos.X < 0) break;
                            if (pos.Z > Ggame.World.MaxZ || pos.Z < 0) break;
                            if (pos.Y < 0) return;
                            if (Ggame.World.GetBlock(pos) != block)
                            {
                                Ggame.UserEvents.RaiseBlockChanged(pos, Ggame.World.GetBlock(pos), block);
                                Ggame.UpdateBlock(pos.X, pos.Y, pos.Z, block);
                            }
                        }
                    }
                }
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


        }

        public class IceSurferCommand : ClassicalSharp.Commands.Command
        {

            public IceSurferCommand()
            {
                Name = "icesurfer";
                Help = new string[] {
                "&a/client icesurfer",
                "&eEnable or Disable icesurfer.",
            };
            }

            public override void Execute(string[] args)
            {
                if (Core.IceSurferEnabled)
                {
                    Core.IceSurferEnabled = false;
                    game.Chat.Add("Ice Surfer disabled.");

                }
                else
                {
                    ushort newB;
                    int newRay;
                    Core.rainbowMode = false;
                    if (args.Length > 1){
                        int.TryParse(args[1],out newRay);
                        Core.rayon = newRay;
                        if (args.Length > 2)
                        {
                            ushort.TryParse(args[2],out newB);
                            Core.block = newB;
                            
                            if (args[2] == "rainbow") { Core.block = 21; Core.rainbowMode = true; } 
                        }
                        else {
                            Core.block = 60;
                        }
                    }
                    else{
                        Core.rayon = 1;
                    }

                    Core.IceSurferEnabled = true;
                    game.Chat.Add("Ice Surfer enabled.");

                }

            }
        }
    }

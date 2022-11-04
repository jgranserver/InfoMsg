using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;
using OTAPI;

namespace FeatureInfo
{
    [ApiVersion(2, 1)]
    public class FeatureInfo : TerrariaPlugin
    {
        public Infos Infos = new Infos();

        public override string Author => "Jgran";
        public override string Name => "InfoMsg";
        public override string Description => "Sends custom server information message for players";
        public override Version Version => new(1, 0, 0);

        public FeatureInfo(Main game) : base(game)
        {
            Order = 1;
        }

        public override void Initialize()
        {
            LoadInfos();
            ServerApi.Hooks.GameInitialize.Register(this, OnInit);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInit);
            }
            base.Dispose(disposing);
        }

        public void OnInit(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("info.msg", getInfoCommand, Infos.Command)
            {
                HelpText = "Command Format: /getinfo <keyword>"
            });
            Commands.ChatCommands.Add(new Command("info.reload", reloadInfo, Infos.Reload)
            {
                HelpText = "Command Format: /inforeload"
            });
        }

        private void LoadInfos()
        {
            string path = Path.Combine(TShock.SavePath, "infomsg.json");
            Infos = Infos.Read(path);
        }

        public void getInfoCommand(CommandArgs args)
        {
            if (args.Parameters.Count > 0)
            {
                string key = args.Parameters[0];
                var player = args.Player;
                
                if (Infos.GetInfoMsgs.Any(x => Regex.Match(key, x.info).Success == true))
                {
                    var msg = Infos.GetInfoMsgs.Find(x => Regex.Match(key, x.info).Success == true).msg;
                    player.SendInfoMessage(string.Format("{0}", msg));
                }
                else
                {
                    var keyword = Infos.GetInfoMsgs.Select(x => x.info).ToList();
                    player.SendErrorMessage("Invalid keyword. Try lower-case only.");
                    args.Player.SendInfoMessage("List of keywords:");
                    args.Player.SendInfoMessage(string.Join(",", keyword));
                }
            }
            else
            {

                var keyword = Infos.GetInfoMsgs.Select(x => x.info).ToList();
                args.Player.SendErrorMessage("No valid keyword. Usage /getinfo <keyword>");
                args.Player.SendInfoMessage("List of keywords:");
                args.Player.SendInfoMessage(string.Join(", ", keyword));
            }
        }

        public void reloadInfo(CommandArgs args)
        {
            LoadInfos();
            args.Player.SendSuccessMessage("Informations reloaded");
        }
    }
}
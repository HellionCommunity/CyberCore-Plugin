using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Commands;
using ZeroGravity;
using ZeroGravity.Objects;

namespace CyberCore
{
    [Permission(PermissionName = "CyberTech.CyberCore.Msg")]
    [Command(CommandName = "msg", Description = "Message Other Players", Permission = "CyberTech.CyberCore.Msg", Plugin = "CyberCore", Usage = "/msg <player> <message>")]
    public class MsgCommand : Command
    {
        public MsgCommand(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            if (args.Length < 2)
            {
                GetPluginHelper.SendMessageToClient(sender,"Error! Format /msg <player> <message>");
                return;
            }
            Player target = GetPluginHelper.GetPlayer(args[0]);
            if (target == null)
            {
                GetPluginHelper.SendMessageToClient(sender, "Error! Target not found");
                return;
            }
            GetPluginHelper.SendMessageToClient(target, sender.Name + " > You : " + string.Join(" ", args.Skip(1)));
        }
    }
}

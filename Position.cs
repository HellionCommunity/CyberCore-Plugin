using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Commands;
using ZeroGravity;
using ZeroGravity.Objects;

namespace CyberCore
{

    [Permission(PermissionName = "CyberTech.CyberCore.pos")]
    [Command(CommandName = "pos", Description = "Message Other Players", Permission = "CyberTech.CyberCore.pos", Plugin = "CyberCore", Usage = "/pos")]
    public class PositionCmd : Command
    {
        public PositionCmd(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            GetPluginHelper.SendMessageToClient(sender,"Your current Postion to string "+sender.LocalPosition.ToString());
        }
    }
}

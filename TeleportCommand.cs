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
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;

namespace CyberCore
{
    //WORKS! But you must Leave and Reconnect :(
    [Permission(PermissionName = "CyberTech.CyberCore.tp", Default = "OP")]
    [Command(CommandName = "tp", Description = "Teleport To Other Players", Permission = "CyberTech.CyberCore.tp",
        Plugin = "CyberCore", Usage = "/tp <player>")]
    public class TeleportCommand2 : Command
    {
        public TeleportCommand2(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
                GetPluginHelper.SendMessageToClient(sender, "Comming Soon!");
                return;
         
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Event;
using HellionExtendedServer.Managers.Event.Player;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace CyberCore
{
    [Plugin(API = "1.0.0", Author = "Yungtechboy1", Description = "Core Plugin", Name = "CyberCore", Version = "1.0.0")]
    public class CyberCoreMain : PluginBase
    {
        public static Double GetStartingMoney
        {
            get { return 5000.0; }
        }
        protected readonly string SaveFile = "money.json";

        public override void OnEnable()
        {
            GetLogger.Info("Loading CyberCore Plugin!");

        }
        


        [HESEvent(EventType = EventID.PlayerSpawnRequest)]
        public void TestSpawnEvent(GenericEvent evnt)
        {
            PlayerSpawnRequest hesse = evnt.Data as PlayerSpawnRequest;
            Player p = GetPluginHelper.getPlayerFromGuid(hesse.Sender);
            if (p == null)
            {
                GetLogger.Error("Error! Player Not Found E:55");
                return;
            }
            GetPluginHelper.SendMessageToServer(p.Name+" has joined the server!");
            GetPluginHelper.SendMessageToClient(p, "Welcome to the server "+p.Name+"!");
            GetPluginHelper.SendMessageToClient(p, "Use /help for all commands");
        }
    }
}

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
    [Command(CommandName = "tp2", Description = "Teleport To Other Players", Permission = "CyberTech.CyberCore.tp",
        Plugin = "CyberCore", Usage = "/tp <player>")]
    public class TeleportCommand2 : Command
    {
        public TeleportCommand2(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            if (args.Length == 1)
            {
                GetPluginHelper.SendMessageToClient(sender, "Error! Format /tp <player>");
                return;
            }

            sender.UnsubscribeFromAll();

            PlayerSpawnResponse playerSpawnResponse = new PlayerSpawnResponse();
            playerSpawnResponse.CharacterTransform = new CharacterTransformData();
            playerSpawnResponse.CharacterTransform.LocalPosition = new float[] { 500, 500, 500 };
            if (args.Length == 3)
                playerSpawnResponse.CharacterTransform.LocalPosition = new float[]
                    {float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2])};
            playerSpawnResponse.CharacterTransform.LocalRotation = new float[] { 0, 0, 0 };
            playerSpawnResponse.Health = 100;
            ArtificialBody parent = sender.Parent as ArtificialBody;
            playerSpawnResponse.ParentID = parent.GUID;
            playerSpawnResponse.ParentType = parent.ObjectType;
            playerSpawnResponse.MainVesselID = parent.GUID;
            //TODO Can not TP in same ship XD
            Console.WriteLine("PARENT " + parent.ObjectType);

            SpaceObjectVessel spaceObjectVessel = parent as SpaceObjectVessel;
            if (spaceObjectVessel != null && spaceObjectVessel.IsDocked)
            {
                spaceObjectVessel = spaceObjectVessel.DockedToMainVessel;
                playerSpawnResponse.MainVesselID = spaceObjectVessel.GUID;
            }
            ArtificialBody artificialBody = spaceObjectVessel != null ? (ArtificialBody)spaceObjectVessel : parent;
            playerSpawnResponse.ParentTransform = new ObjectTransform()
            {
                GUID = artificialBody.GUID,
                Type = artificialBody.ObjectType,
                Forward = artificialBody.Forward.ToFloatArray(),
                Up = artificialBody.Up.ToFloatArray()
            };

            playerSpawnResponse.ParentTransform.Realtime = new RealtimeData()
            {
                ParentGUID = artificialBody.Orbit.Parent.CelestialBody.GUID,
                Position = artificialBody.Orbit.RelativePosition.ToArray(),
                Velocity = artificialBody.Orbit.Velocity.ToArray()
            };

            if (artificialBody.Orbit.IsOrbitValid)
            {
                playerSpawnResponse.ParentTransform.Orbit = new OrbitData()
                {
                    ParentGUID = artificialBody.Orbit.Parent.CelestialBody.GUID
                };
                artificialBody.Orbit.FillOrbitData(ref playerSpawnResponse.ParentTransform.Orbit,
                    (SpaceObjectVessel)null);
            }
            else
                playerSpawnResponse.ParentTransform.Realtime = new RealtimeData()
                {
                    ParentGUID = artificialBody.Orbit.Parent.CelestialBody.GUID,
                    Position = artificialBody.Orbit.RelativePosition.ToArray(),
                    Velocity = artificialBody.Orbit.Velocity.ToArray()
                };

            List<DynamicObjectDetails> dynamicObjectDetailsList = new List<DynamicObjectDetails>();
            foreach (DynamicObject dynamicObject in sender.DynamicObjects)
                dynamicObjectDetailsList.Add(dynamicObject.GetDetails());
            playerSpawnResponse.DynamicObjects = dynamicObjectDetailsList;

            //playerSpawnResponse.HomeGUID = 11;

            //playerSpawnResponse.SpawnPointID = sender.CurrentSpawnPoint.SpawnPointID;

            GetServer.NetworkController.SendToGameClient(sender.GUID, (NetworkData)playerSpawnResponse);




            SpawnObjectsResponse res = new SpawnObjectsResponse();
            Server.Instance.NetworkController.AddCharacterSpawnsToResponse(sender, ref res);
            res.Data.Add(sender.GetSpawnResponseData(sender));
            if (sender.Parent is SpaceObjectVessel)
            {
                SpaceObjectVessel spaceObjectVessel2 = sender.Parent as SpaceObjectVessel;
                if (spaceObjectVessel2.IsDocked)
                    spaceObjectVessel2 = spaceObjectVessel2.DockedToMainVessel;
                foreach (DynamicObject dynamicObject in spaceObjectVessel2.DynamicObjects)
                    res.Data.Add(dynamicObject.GetSpawnResponseData(sender));
                foreach (Corpse corpse in spaceObjectVessel2.Corpses)
                    res.Data.Add(corpse.GetSpawnResponseData(sender));
                if (spaceObjectVessel2.AllDockedVessels.Count > 0)
                {
                    foreach (SpaceObjectVessel allDockedVessel in spaceObjectVessel2.AllDockedVessels)
                    {
                        foreach (DynamicObject dynamicObject in allDockedVessel.DynamicObjects)
                            res.Data.Add(dynamicObject.GetSpawnResponseData(sender));
                        foreach (Corpse corpse in allDockedVessel.Corpses)
                            res.Data.Add(corpse.GetSpawnResponseData(sender));
                    }
                }
            }
            Server.Instance.NetworkController.SendToGameClient(sender.GUID, (NetworkData)res);
            Server.Instance.NetworkController.SendCharacterSpawnToOtherPlayers(sender);
            if (sender.MessagesReceivedWhileLoading != null && sender.MessagesReceivedWhileLoading.Count > 0)
            {
                foreach (NetworkData data1 in sender.MessagesReceivedWhileLoading)
                    Server.Instance.NetworkController.SendToGameClient(sender.GUID, data1);
            }

            Console.WriteLine("TRYYTYYYYYYY");
        }
    }

    [Permission(Default = "op",PermissionName = "CyberTech.CyberCore.fill")]
    [Command(CommandName = "fill", Description = "Teleport To Other Players", Permission = "CyberTech.CyberCore.fill",
        Plugin = "CyberCore", Usage = "/tp <player>")]
    public class Fill : Command
    {
        public Fill(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            if (args.Length == 1)
            {
                GetPluginHelper.SendMessageToClient(sender, "Error! Format /tp <player>");
                return;
            }
            if (sender.Parent is Ship)
            {
                Ship ship = sender.Parent as Ship;
                float f1 = ship.Resource1;
                float f2 = ship.Resource2;
                float f3 = ship.Resource3;
                List<ResourceContainerDetails> a = ship.DistributionManager.GetResourceContainersDetails(false);
                List<ResourceContainer> aa = ship.DistributionManager.GetResourceContainers();
                List<RefinedResourcesData> aaaa = ship.DistributionManager.Refinery.Resources;
                List<VesselComponent> aaa = ship.DistributionManager.GetGenerators();
                foreach (ResourceContainerDetails b in a)
                {

                    GetPluginHelper.GetLogger.Info(">>>>> " + b);
                    foreach (CargoResourceData c in b.Resources)
                    {
                        GetPluginHelper.GetLogger.Info("lll  " + c.Quantity + " TYPE " + c.ResourceType);
                    }
                }
                GetPluginHelper.GetLogger.Info("-------------------------------");
                foreach (RefinedResourcesData b in aaaa)
                {
                    GetPluginHelper.GetLogger.Info("TYPPPEEEE  " + b.RawResource);
                    foreach (CargoResourceData d in b.RefinedResources)
                    {
                        GetPluginHelper.GetLogger.Info("lll  " + d.Quantity + " TYPE " + d.ResourceType);
                    }
                }
                GetPluginHelper.GetLogger.Info("-------------------------------");
                foreach (ResourceContainer b in aa)
                {

                    GetPluginHelper.GetLogger.Info("-> " + b.OutputType);

                    foreach (CargoCompartmentData c in b.Compartments)
                    {
                        GetPluginHelper.GetLogger.Info("qqq  " + c.Name + " TYPE " + c.Capacity);
                        foreach (CargoResourceData d in c.Resources)
                        {
                            GetPluginHelper.GetLogger.Info("lll  " + d.Quantity + " TYPE " + d.ResourceType);
                        }
                    }
                }

                ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
                shipStatsMessage.GUID = ship.GUID;
                shipStatsMessage.VesselObjects = new VesselObjects();
                shipStatsMessage.VesselObjects.SubSystems = ship.DistributionManager.GetSubSystemsDetails(true, ship.GUID);
                shipStatsMessage.VesselObjects.Generators = ship.DistributionManager.GetGeneratorsDetails(true, ship.GUID);
                shipStatsMessage.VesselObjects.ServicePoints = ship.DistributionManager.GetServicePointsDetails(true, ship.GUID);
                shipStatsMessage.VesselObjects.RoomTriggers = ship.DistributionManager.GetRoomsDetails(true, ship.GUID);
                shipStatsMessage.VesselObjects.ResourceContainers = ship.DistributionManager.GetResourceContainersDetails(true, ship.GUID);


                Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData)shipStatsMessage, -1L, (SpaceObject)ship);

                GetPluginHelper.GetLogger.Info("FUEL > " + f1 + " | " + f2 + " | " + f3);
            }

        }
    }


    [Permission(PermissionName = "CyberTech.Econ.npc")]
    [Command(CommandName = "npcc", Description = "Testing NPC", Permission = "CyberTech.Econ.npc", Plugin = "CyberEcon", Usage = "/npc")]
    public class NPCCMD : Command
    {
        public NPCCMD(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            Ship s = Ship.CreateNewShip("YOU SHOULD SEE THIS NAME", GameScenes.SceneID.AltCorp_Ship_Tamara, -1, new List<long>(), (List<long>)null, sender.Position + new Vector3D(10, 10, 10), new Vector3D?(), new QuaternionD?(QuaternionD.Identity), "", false, 0.03, 0.3);

            GetPluginHelper.SendMessageToClient(sender, "AHHHHHH Spawn?");
            //Activate Beacon
            s.IsDistresActive = true;
            s.DistressActivatedTime = Server.Instance.SolarSystem.CurrentTime;
            ObjectTransform objectTransform = new ObjectTransform()
            {
                GUID = s.GUID,
                Type = SpaceObjectType.Ship,
                Forward = s.Forward.ToFloatArray(),
                Up = s.Up.ToFloatArray()
            };
            if (s.Orbit.IsOrbitValid)
            {
                objectTransform.Orbit = new OrbitData()
                {
                    ParentGUID = s.Orbit.Parent.CelestialBody.GUID
                };
                s.Orbit.FillOrbitData(ref objectTransform.Orbit, (SpaceObjectVessel)null);
            }
            else
                objectTransform.Realtime = new RealtimeData()
                {
                    ParentGUID = s.Orbit.Parent.CelestialBody.GUID,
                    Position = (s.Position - s.Orbit.Parent.Position).ToArray(),
                    Velocity = s.Velocity.ToArray()
                };
            if (s.CurrentCourse != null)
                objectTransform.Maneuver = s.CurrentCourse.CurrentData();
            objectTransform.IsDistressActive = true;
            NetworkController networkController = Server.Instance.NetworkController;
            DistressCallResponse distressCallResponse = new DistressCallResponse();
            distressCallResponse.GUID = s.GUID;
            distressCallResponse.Trans = objectTransform;
            long skipPlayerGUID = -1;
            networkController.SendToAllClients((NetworkData)distressCallResponse, skipPlayerGUID);

            GetPluginHelper.SendMessageToClient(sender, "AHHHHHH Spawn?");

        }
    }
}

namespace Pluton.Rust.Objects
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using System.Runtime.Serialization;
	using Network;
	using Facepunch;
	using Logger = Core.Logger;
	using Server = Rust.Server;

    [Serializable]
    public class Player : Entity
    {
        [NonSerialized]
        private BasePlayer _basePlayer;

        public readonly ulong GameID;

        public readonly string SteamID;

        public Player(BasePlayer player) : base(player)
        {
            GameID = player.userID;
            SteamID = player.userID.ToString();
            _basePlayer = player;
            try {
                Stats = new PlayerStats(SteamID);
            } catch (Exception ex) {
                Logger.LogDebug("[Player] Couldn't load stats!");
                Logger.LogException(ex);
            }
        }

        [OnDeserialized]
        public void OnPlayerDeserialized(StreamingContext context)
        {
            Logger.LogWarning("Deserializing player with id: " + SteamID);
            _basePlayer = BasePlayer.FindByID(GameID);
            if (_basePlayer == null)
                Logger.LogWarning("_basePlayer is <null>, is the player offline?");
            else
                Logger.LogWarning("basePlayer found: " + _basePlayer.displayName);
        }

        public static Player Find(string nameOrSteamidOrIP)
        {
            BasePlayer player = BasePlayer.Find(nameOrSteamidOrIP);
            if (player != null)
                return Server.GetPlayer(player);
            Logger.LogDebug("[Player] Couldn't find player!");
            return null;
        }

        public static Player FindByGameID(ulong steamID)
        {
            BasePlayer player = BasePlayer.FindByID(steamID);
            if (player != null)
                return Server.GetPlayer(player);
            Logger.LogDebug("[Player] Couldn't find player!");
            return null;
        }

        public static Player FindBySteamID(string steamID)
        {
            return FindByGameID(UInt64.Parse(steamID));
        }

        public void Ban(string reason = "no reason")
        {
            ServerUsers.Set(GameID, ServerUsers.UserGroup.Banned, Name, reason);
            ServerUsers.Save();
            Kick("[BAN] " + reason);
        }

        public void Kick(string reason = "no reason")
        {
			global::Network.Net.sv.Kick(basePlayer.net.connection, reason);
        }

        public void Reject(string reason = "no reason")
        {
            ConnectionAuth.Reject(basePlayer.net.connection, reason);
        }

        public Vector3 GetLookPoint(float maxDist = 500f)
        {
            return GetLookHit(maxDist).point;
        }

        public RaycastHit GetLookHit(float maxDist = 500f, int layers = Physics.AllLayers)
        {
            RaycastHit hit;
            Ray orig = basePlayer.eyes.HeadRay();
            Physics.Raycast(orig, out hit, maxDist, layers);
            return hit;
        }

        public Player GetLookPlayer(float maxDist = 500f)
        {
            RaycastHit hit = GetLookHit(maxDist, LayerMask.GetMask("Player (Server)"));
            if (hit.collider != null) {
                BasePlayer basePlayer = hit.collider.GetComponentInParent<BasePlayer>();
                if (basePlayer != null) {
                    return Server.GetPlayer(basePlayer);
                }
            }
            return null;
        }

        public BuildingPart GetLookBuildingPart(float maxDist = 500f)
        {
            RaycastHit hit = GetLookHit(maxDist, LayerMask.GetMask("Construction", "Deployed"));
            if (hit.collider != null) {
                BuildingBlock buildingBlock = hit.collider.GetComponentInParent<BuildingBlock>();
                if (buildingBlock != null) {
                    return new BuildingPart(buildingBlock);
                }
            }
            return null;
        }

        public override void Kill()
        {
            var info = new HitInfo();
            info.damageTypes.Add(global::Rust.DamageType.Suicide, Single.MaxValue);
            info.Initiator = baseEntity;
            basePlayer.Die(info);
        }

        public void MakeNone(string reason = "no reason")
        {
            ServerUsers.Set(GameID, ServerUsers.UserGroup.None, Name, reason);
            basePlayer.net.connection.authLevel = 0;
            ServerUsers.Save();
        }

        public void MakeModerator(string reason = "no reason")
        {
            ServerUsers.Set(GameID, ServerUsers.UserGroup.Moderator, Name, reason);
            basePlayer.net.connection.authLevel = 1;
            ServerUsers.Save();
        }

        public void MakeOwner(string reason = "no reason")
        {
            ServerUsers.Set(GameID, ServerUsers.UserGroup.Owner, Name, reason);
            basePlayer.net.connection.authLevel = 2;
            ServerUsers.Save();
        }

        public void Message(string msg)
        {
            MessageFrom(Server.server_message_name, msg);
        }

        public void MessageFrom(string from, string msg)
        {
            basePlayer.SendConsoleCommand("chat.add", 0, from.ColorText("fa5") + ": " + msg);
        }

        public void ConsoleMessage(string msg)
        {
            basePlayer.SendConsoleCommand("echo", msg);
        }

        public override bool IsPlayer()
        {
            return true;
        }

        public void SendConsoleCommand(string cmd)
        {
            basePlayer.SendConsoleCommand(cmd.QuoteSafe());
        }

        public bool GroundTeleport(float x, float y, float z)
        {
            return Teleport(x, World.GetInstance().GetGround(x, z), z);
        }

        public bool GroundTeleport(Vector3 v3)
        {
            return Teleport(v3.x, World.GetInstance().GetGround(v3.x, v3.z), v3.z);
        }

        public bool Teleport(Vector3 v3)
        {
            return Teleport(v3.x, v3.y, v3.z);
        }

        public static float worldSizeHalf = (float)global::World.Size / 2;
        public static Vector3[] firstLocations = new Vector3[] {
            new Vector3(worldSizeHalf, 0, worldSizeHalf),
            new Vector3(-worldSizeHalf, 0, worldSizeHalf),
            new Vector3(worldSizeHalf, 0, -worldSizeHalf),
            new Vector3(-worldSizeHalf, 0, -worldSizeHalf)
        };

        public bool Teleport(float x, float y, float z)
        {
            if (teleporting || basePlayer.IsDead())
                return false;

            teleporting = true;

            Vector3 newPos = new Vector3(x, y + 0.05f, z);
            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.Sleeping, true);
            if (!BasePlayer.sleepingPlayerList.Contains(basePlayer))
            {
                BasePlayer.sleepingPlayerList.Add(basePlayer);
            }
            basePlayer.CancelInvoke("InventoryUpdate");
            basePlayer.inventory.crafting.CancelAll(true);
            basePlayer.MovePosition(newPos);
            basePlayer.ClientRPCPlayer(null, basePlayer, "ForcePositionTo", newPos);
            basePlayer.TransformChanged();
            basePlayer.SetPlayerFlag(BasePlayer.PlayerFlags.ReceivingSnapshot, true);
            basePlayer.UpdateNetworkGroup();
            basePlayer.SendNetworkUpdateImmediate(false);
            basePlayer.ClientRPCPlayer(null, basePlayer, "StartLoading");
            basePlayer.SendFullSnapshot();

            teleporting = false;

            return true;
        }

        public bool Admin {
            get {
                return Moderator || Owner;
            }
        }

        public string AuthStatus {
            get {
                return basePlayer.net.connection.authStatus;
            }
        }

        public BasePlayer basePlayer {
            get {
                if (_basePlayer == null)
                    return BasePlayer.FindByID(GameID);
                return _basePlayer;
            }
            private set {
                _basePlayer = value;
            }
        }

        public float Health {
            get {
                return basePlayer.health;
            }
            set {
                basePlayer.health = value;
            }
        }

        public Inv Inventory {
            get {
                return new Inv(basePlayer.inventory);
            }
        }

        public string IP {
            get {
                return basePlayer.net.connection.ipaddress;
            }
        }

        public bool IsWounded {
            get {
                return basePlayer.HasPlayerFlag(BasePlayer.PlayerFlags.Wounded);
            }
        }

        public override Vector3 Location {
            get {
                return basePlayer.transform.position;
            }
            set {
                Teleport(value.x, value.y, value.z);
            }
        }

        public bool Moderator {
            get {
                return ServerUsers.Is(GameID, ServerUsers.UserGroup.Moderator);
            }
        }

        public override string Name {
            get {
                return basePlayer.displayName;
            }
        }

        public bool Offline {
            get {
                return _basePlayer == null;
            }
        }

        public bool Owner {
            get {
                return ServerUsers.Is(GameID, ServerUsers.UserGroup.Owner);
            }
        }

        public string OS {
            get {
                return basePlayer.net.connection.os;
            }
        }

        public int Ping {
            get {
                return Net.sv.GetAveragePing(basePlayer.net.connection);
            }
        }

        public PlayerStats Stats {
            get {
                return Server.GetInstance().serverData.Get("PlayerStats", SteamID) as PlayerStats;
            }
            set {
                Server.GetInstance().serverData.Add("PlayerStats", SteamID, value);
            }
        }

        public float TimeOnline {
            get {
                return basePlayer.net.connection.connectionTime;
            }
        }

        private bool teleporting;

        public bool Teleporting {
            get {
                return teleporting;
            }
        }
    }
}


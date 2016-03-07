namespace Pluton.Rust.Objects
{
	using Core;
	using System;

    [Serializable]
    public class OfflinePlayer : CountedInstance
    {
        public string Name;
        public string SteamID;
        public string IP;
        public string OS;
        public float X;
        public float Y;
        public float Z;
        public ulong totalTimeOnline;
        public bool Admin;

        public OfflinePlayer(Player player)
        {
            Name = player.Name;
            SteamID = player.SteamID;
            IP = player.IP;
            OS = player.OS;
            X = player.X;
            Y = player.Y;
            Z = player.Z;
            totalTimeOnline = (ulong)player.TimeOnline;
            Admin = player.Admin;
		}

		public static OfflinePlayer Get(string steamID) => Get(UInt64.Parse(steamID));

        public static OfflinePlayer Get(ulong steamID)
        {
            OfflinePlayer op = Server.GetInstance().OfflinePlayers[steamID];
            if (op == null) {
                Logger.LogDebug("[OfflinePlayer] Couldn't find OfflinePlayer: " + steamID.ToString());
                return null;
            }
            return op;
        }

        public void Update(Player player)
        {
            if (Name != player.Name) {
                Logger.LogDebug("[OfflinePlayer] " + Name + " changed name to: " + player.Name);
                Name = player.Name;
            }
            IP = player.IP;
            OS = player.OS;
            X = player.X;
            Y = player.Y;
            Z = player.Z;
            Admin = player.Admin;
            totalTimeOnline += (ulong)player.TimeOnline;
        }
    }
}


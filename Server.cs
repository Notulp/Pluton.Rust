namespace Pluton.Rust {
	using Core;
	using Objects;
	using System;
	using System.IO;
	using System.Linq;
	using System.Collections;
	using System.Collections.Generic;

	public class Server : Singleton<Server>, ISingleton {
		public bool Loaded = false;
		public Dictionary<ulong, Player> Players;
		public Dictionary<ulong, OfflinePlayer> OfflinePlayers;
		public Dictionary<string, LoadOut> LoadOuts;
		public DataStore serverData;
		public static string server_message_name = "Pluton";

        private float craftTimeScale = 1f;

		public void Broadcast(string arg) => BroadcastFrom(server_message_name, arg);

		public void BroadcastFrom(string name, string arg) => ConsoleNetwork.BroadcastToAllClients("chat.add", 0, String.Format("{0}: {1}", name.ColorText("fa5"), arg));

		public void BroadcastFrom(ulong playerid, string arg) => ConsoleNetwork.BroadcastToAllClients("chat.add", playerid, arg, 1);

		public Player FindPlayer(string s) {
			BasePlayer player = BasePlayer.Find(s);

			if (player != null)
				return new Player(player);

			return null;
		}

		public Player FindPlayer(ulong steamid) {
			if (Players.ContainsKey(steamid))
				return Players[steamid];

			return FindPlayer(steamid.ToString());
		}

		public static Player GetPlayer(BasePlayer basePlayer) {
			try {
				Player player = GetInstance().FindPlayer(basePlayer.userID);

				if (player != null)
					return player;

				return new Player(basePlayer);
			} catch (Exception ex) {
				Logger.LogDebug("[Server] GetPlayer: " + ex.Message);
				Logger.LogException(ex);
				return null;
			}
		}

		public void Initialize() {
			Instance.LoadOuts = new Dictionary<string, LoadOut>();
			//Instance.Structures = new Dictionary<string, StructureRecorder.Structure>();
			Instance.Players = new Dictionary<ulong, Player>();
			Instance.OfflinePlayers = new Dictionary<ulong, OfflinePlayer>();
			Instance.serverData = new DataStore("ServerData.ds");
			Instance.serverData.Load();
			Instance.LoadLoadouts();
			//Instance.LoadStructures();
			//Instance.ReloadBlueprints();
			Instance.LoadOfflinePlayers();
			Instance.CheckPluginsFolder();
		}

		public void CheckPluginsFolder() {
			string path = Singleton<Util>.Instance.GetPluginsFolder();

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);
		}

		public float CraftingTimeScale {
			get {
				return craftTimeScale;
			}
			set {
				craftTimeScale = value;
			}
		}

		public void LoadLoadouts() {
			string path = Singleton<Util>.GetInstance().GetLoadoutFolder();

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

            DirectoryInfo loadoutPath = new DirectoryInfo(path);

			foreach (FileInfo file in loadoutPath.GetFiles()) {
				if (file.Extension == ".ini") {
					new LoadOut(file.Name.Replace(".ini", ""));
				}
			}

			Logger.Log("[Server] " + LoadOuts.Count + " loadout loaded!");
		}

		public void LoadOfflinePlayers() {
			Hashtable ht = serverData.GetTable("OfflinePlayers");

			if (ht != null) {
				foreach (DictionaryEntry entry in ht) {
					Instance.OfflinePlayers.Add(UInt64.Parse(entry.Key as string), entry.Value as OfflinePlayer);
				}
			} else {
				Logger.LogWarning("[Server] No OfflinePlayers found!");
			}

			Logger.Log("[Server] " + Instance.OfflinePlayers.Count + " offlineplayer loaded!");
		}

		/*
        public void LoadStructures()
        {
            string path = Util.GetStructuresFolder();

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            DirectoryInfo structuresPath = new DirectoryInfo(path);
            Structures.Clear();

            foreach (FileInfo file in structuresPath.GetFiles()) {
                if (file.Extension.ToLower() == ".sps") {
                    using (FileStream stream = new FileStream(file.FullName, FileMode.Open)) {
                        BinaryFormatter formatter = new BinaryFormatter();

                        StructureRecorder.Structure structure = (StructureRecorder.Structure)formatter.Deserialize(stream);
                        Structures.Add(file.Name.Substring(0, file.Name.Length - 5), structure);
                    }
                }
            }

            Logger.Log("[Server] " + Structures.Count.ToString() + " structure loaded!");
        }
        */

		public void Save() {
			OnShutdown();

			foreach (Player p in Players.Values) {
				OfflinePlayers.Remove(p.GameID);
			}
		}

		public string SendCommand(string command, params object[] args) => ConsoleSystem.Run.Server.Normal(command, args);

		public void OnShutdown() {
			foreach (Player player in Players.Values) {
				if (serverData.ContainsKey("OfflinePlayers", player.SteamID)) {
					OfflinePlayer op = serverData.Get("OfflinePlayers", player.SteamID) as OfflinePlayer;

					op.Update(player);
					OfflinePlayers[player.GameID] = op;
				} else {
                    OfflinePlayer op = new OfflinePlayer(player);

					OfflinePlayers.Add(player.GameID, op);
				}
			}

			foreach (OfflinePlayer op in OfflinePlayers.Values) {
				serverData.Add("OfflinePlayers", op.SteamID, op);
			}

			serverData.Save();
			Singleton<Util>.Instance.SaveZones();
			Singleton<Util>.Instance.ZoneStore.Save();
		}

		public List<Player> ActivePlayers => (from player in BasePlayer.activePlayerList
				        					  select GetPlayer(player)).ToList();

		public List<Player> SleepingPlayers => (from player in BasePlayer.sleepingPlayerList
				        						select GetPlayer(player)).ToList();

		public int MaxPlayers => ConVar.Server.maxplayers;
	}
}

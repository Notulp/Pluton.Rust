namespace Pluton.Rust
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UnityEngine;
	using Pluton.Core;
	using Objects;
	using Serialize;
	using Logger = Core.Logger;
	using System.Collections;

	public class Util : Core.Util
	{
		public Dictionary<string, Zone2D> zones = new Dictionary<string, Zone2D>();
		public DataStore ZoneStore;

		public DirectoryInfo UtilPath;

		public Zone2D GetZone(string name)
		{
			if (zones.ContainsKey(name))
				return zones[name];
			return null;
		}

		public void SetZone(Zone2D zone)
		{
			if (zone == null)
				throw new NullReferenceException("SetZone( zone )");
			zones[zone.Name] = zone;
		}

		public Zone2D CreateZone(string name)
		{
			try {
				var obj = new GameObject(name);
				var gobj = UnityEngine.Object.Instantiate(obj, Vector3.zero, Quaternion.identity) as GameObject;
				var zone = gobj.AddComponent<Zone2D>();
				zone.Name = name;
				zones[name] = zone;
				return zone;
			} catch (Exception ex) {
				Logger.LogException(ex);
				return null;
			}
		}

		public void LoadZones()
		{
			try {
				Logger.LogWarning("Loading zones.");
				zones = new Dictionary<string, Zone2D>();
				Hashtable zht = ZoneStore.GetTable("Zones");
				if (zht == null)
					return;

				foreach (object zone in zht.Values) {
					var z = zone as SerializedZone2D;
					if (z == null)
						continue;
					Logger.LogWarning("Zone found with name: " + z.Name);
					z.ToZone2D();
				}
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		}

		public void SaveZones()
		{
			try {
				Logger.LogWarning("Saving " + zones.Count.ToString() + " zone.");
				foreach (var zone in zones.Values) {
					ZoneStore.Add("Zones", zone.Name, zone.Serialize());
				}
			} catch (Exception ex) {
				Debug.LogException(ex);
			}
		}

		public void ChangeTriggerRadius(TriggerBase trigger, float newRadius)
		{
			if (newRadius < 0f) {
				throw new InvalidOperationException(String.Format("Radius can't be less then zero. ChangeTriggerRadius({0}, {1})", trigger, newRadius));
			}

			trigger.GetComponent<SphereCollider>().radius = newRadius;
			trigger.SendMessage("OnValidate", SendMessageOptions.DontRequireReceiver);
		}

		public void ConsoleLog(string str, bool adminOnly = false)
		{
			try {
				foreach (Player player in Server.GetInstance().Players.Values) {
					if (!adminOnly || (adminOnly && player.Admin))
						player.ConsoleMessage(str);
				}
			} catch (Exception ex) {
				Logger.LogDebug("ConsoleLog ex");
				Logger.LogException(ex);
			}
		}

		public LoadOut CreateLoadOut(string name) => new LoadOut(name);

		public string GetLoadoutFolder() => Path.Combine(GetPublicFolder(), "LoadOuts");

		public string GetStructuresFolder() => Path.Combine(GetPublicFolder(), "Structures");

		public override string GetPublicFolder() => Path.Combine(GetIdentityFolder(), "Pluton");

		public string GetIdentityFolder() => Path.Combine(GetRootFolder(), Path.Combine("server", ConVar.Server.identity));

		public void DestroyEntity(BaseEntity ent) => ent.GetComponent<BaseNetworkable>().KillMessage();

		public void DestroyEntityGib(BaseEntity ent) => ent.GetComponent<BaseNetworkable>().Kill(BaseNetworkable.DestroyMode.Gib);

		public Vector3 Infront(Player p, float length) => p.Location + ((Vector3.forward * length));

		new public void Initialize()
		{
			UtilPath = new DirectoryInfo(Path.Combine(GetPublicFolder(), "Util"));
			ZoneStore = new DataStore("Zones.ds");
			ZoneStore.Load();
			LoadZones();
		}

		public IEnumerable<string> Prefabs()
		{
			if (Server.GetInstance().Loaded)
			{
				foreach (string entity in FileSystem.FindAll("assets", ".prefab"))
				{
					yield return entity;
				}
			}
			else
			{
				Logger.LogError("Util.Prefabs() should be only called after the server is loaded.");
			}
		}

		public void Items()
		{
			var path = Path.Combine(GetPublicFolder(), "Items.ini");
			if (!File.Exists(path))
				File.AppendAllText(path, "");
			var ini = new IniParser(path);
			foreach (ItemDefinition item in ItemManager.itemList) {
				ini.AddSetting(item.displayName.english, "itemid", item.itemid.ToString());
				ini.AddSetting(item.displayName.english, "category", item.category.ToString());
				ini.AddSetting(item.displayName.english, "shortname", item.shortname);
				ini.AddSetting(item.displayName.english, "description", item.displayDescription.english);
			}
			ini.Save();
		}
	}
}

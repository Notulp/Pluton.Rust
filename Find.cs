namespace Pluton.Rust
{
	using System.Collections.Generic;
	using System.Linq;
	using Pluton.Core;
	using Objects;
	
	public class Find : Singleton<Find>, ISingleton
	{
		public void Initialize() { }

		public List<ItemBlueprint> BluePrints()
		{
			return ItemManager.bpList;
		}

		public ItemBlueprint BluePrint(string name)
		{
			return ItemManager.bpList.Find(item => {
				if (item.targetItem.shortname == name ||
				    item.targetItem.displayName.english == name)
					return true;
				return false;
			});
		}

		public List<ItemBlueprint> BluePrintsByCategory(string cat)
		{
			return ItemManager.bpList.FindAll(item => {
				if (item.targetItem.category.ToString() == cat)
					return true;
				return false;
			});
		}

		public List<ItemBlueprint> BluePrintsByCategory(ItemCategory cat)
		{
			return ItemManager.bpList.FindAll(item => {
				if (item.targetItem.category == cat)
					return true;
				return false;
			});
		}

		public List<BuildingPart> BuildingParts()
		{
			return (from block in UnityEngine.Object.FindObjectsOfType<BuildingBlock>()
			        select new BuildingPart(block)).ToList();
		}

		public List<BuildingPart> BuildingPartsByName(string name)
		{
			return (from block in UnityEngine.Object.FindObjectsOfType<BuildingBlock>()
			        where block.LookupPrefabName() == name ||
			        block.LookupShortPrefabName() == name ||
			        block.blockDefinition.fullName == name
			        select new BuildingPart(block)).ToList();
		}

		public List<BuildingPart> BuildingPartsByGrade(BuildingGrade.Enum grade)
		{
			return (from block in UnityEngine.Object.FindObjectsOfType<BuildingBlock>()
			        where block.grade == grade
			        select new BuildingPart(block)).ToList();
		}

		public List<BuildingPart> BuildingPartsByGrade(int grade)
		{
			return (from block in UnityEngine.Object.FindObjectsOfType<BuildingBlock>()
			        where (int)block.grade == grade
			        select new BuildingPart(block)).ToList();
		}

		public List<BuildingPart> BuildingPartsByGrade(string grade)
		{
			return (from block in UnityEngine.Object.FindObjectsOfType<BuildingBlock>()
			        where block.grade.ToString() == grade
			        select new BuildingPart(block)).ToList();
		}

		public List<TriggerComfort> ComfortZones()
		{
			return UnityEngine.Object.FindObjectsOfType<TriggerComfort>().ToList();
		}

		public List<Entity> CupBoards()
		{
			return (from board in UnityEngine.Object.FindObjectsOfType<BuildingPrivlidge>()
			        select new Entity(board)).ToList();
		}

		public List<Entity> CupBoards(ulong steamid)
        {
            return (from board in UnityEngine.Object.FindObjectsOfType<BuildingPrivlidge>()
			        where board.authorizedPlayers.Count(x => x.userid == steamid) != 0
                select new Entity(board)).ToList();
        }

		public List<Entity> Doors()
		{
			return (from door in UnityEngine.Object.FindObjectsOfType<Door>()
			        select new Entity(door as BaseEntity)).ToList();
		}

		public Entity Entity(UnityEngine.Vector3 location, float dist = 1f)
		{
			foreach (var x in BaseNetworkable.serverEntities.All())
			{
				if (UnityEngine.Vector3.Distance(x.transform.position, location) <= dist)
				{
					return new Entity(x as BaseEntity);
				}
			}
			return null;
		}

		public List<Entity> Entities()
		{
			return (from ent in BaseNetworkable.serverEntities.All()
			        select new Entity(ent as BaseEntity)).ToList();
		}

		public List<Entity> Entities(string name)
		{
			return (from ent in BaseNetworkable.serverEntities.All()
			        where ent.LookupPrefabName().Contains(name) ||
			        ent.LookupShortPrefabName().Contains(name) ||
			        ent.name.Contains(name)
			        select new Entity(ent as BaseEntity)).ToList();
		}

		public List<ItemDefinition> ItemDefinitions()
		{
			return ItemManager.itemList;
		}

		public List<ItemDefinition> ItemDefinitionsByCategory(string cat)
		{
			return (from item in ItemManager.itemList
			        where item.category.ToString() == cat
			        select item).ToList();
		}

		public List<ItemDefinition> ItemDefinitionsByCategory(ItemCategory cat)
		{
			return (from item in ItemManager.itemList
			        where item.category == cat
			        select item).ToList();
		}

		public ItemDefinition ItemDefinition(string name)
		{
			return (from item in ItemManager.itemList
			        where item.shortname == name ||
			        item.displayName.english == name
			        select item).FirstOrDefault();
		}

		public List<Entity> Locks()
		{
			return (from _lock in UnityEngine.Object.FindObjectsOfType<BaseLock>()
			        select new Entity(_lock as BaseEntity)).ToList();
		}

		public List<Entity> LocksByAuthorizedUserID(ulong steamid)
		{
			return (from _lock in UnityEngine.Object.FindObjectsOfType<CodeLock>()
			        where ((List<ulong>)_lock.GetFieldValue("whitelistPlayers")).Contains(steamid)
			        select new Entity(_lock as BaseEntity)).ToList();
		}

		public List<Entity> Loot()
		{
			return (from loot in UnityEngine.Object.FindObjectsOfType<LootContainer>()
			        select new Entity(loot as BaseEntity)).ToList();
		}

		public List<NPC> NPCs()
		{
			return (from npc in UnityEngine.Object.FindObjectsOfType<BaseNPC>()
			        select new NPC(npc)).ToList();
		}

		public List<NPC> NPCs(string name)
		{
			return (from npc in UnityEngine.Object.FindObjectsOfType<BaseNPC>()
			        where npc.name.Contains(name)
			        select new NPC(npc)).ToList();
		}

		public Player Player(string nameorIPorID)
		{
			return (from player in Server.GetInstance().Players.Values
			        where player.Name == nameorIPorID ||
			        player.IP == nameorIPorID ||
			        player.SteamID == nameorIPorID
			        select player).FirstOrDefault();
		}

		public List<Player> Players(string nameorIP)
		{
			return (from player in Server.GetInstance().Players.Values
			        where player.Name.Contains(nameorIP) ||
			        player.IP.Contains(nameorIP)
			        select player).ToList();
		}

		public List<TriggerRadiation> RadZones()
		{
			return UnityEngine.Object.FindObjectsOfType<TriggerRadiation>().ToList();
		}

		public List<Entity> Storage()
		{
			return (from storage in UnityEngine.Object.FindObjectsOfType<StorageContainer>()
			        select new Entity(storage as BaseEntity)).ToList();
		}

		public List<TriggerTemperature> TemperatureZones()
		{
			return UnityEngine.Object.FindObjectsOfType<TriggerTemperature>().ToList();
		}

		public List<TriggerBase> Triggers()
		{
			return UnityEngine.Object.FindObjectsOfType<TriggerBase>().ToList();
		}

		public List<T> Triggers<T>() where T : TriggerBase
		{
			return UnityEngine.Object.FindObjectsOfType<T>().ToList();
		}
	}
}


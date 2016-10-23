namespace Pluton.Rust.Objects
{
	using System;
	using UnityEngine;
	using Core;

	[Serializable]
	public class Entity : CountedInstance
	{
		[NonSerialized]
		public readonly BaseEntity baseEntity;
		public readonly string Prefab;
		public readonly uint PrefabID;

		public Entity(BaseEntity ent)
		{
			baseEntity = ent;
			Prefab = baseEntity.PrefabName;
			PrefabID = baseEntity.prefabID;
		}

		public virtual void Kill() => baseEntity.Kill(BaseNetworkable.DestroyMode.Gib);

		public virtual bool IsBuildingPart() => false;

		public virtual bool IsNPC() => false;

		public virtual bool IsPlayer() => false;

		public BuildingPart ToBuildingPart()
		{
			BuildingBlock block = baseEntity.GetComponent<BuildingBlock>();

			if (block == null)
				return null;

			return new BuildingPart(block);
		}

		public NPC ToNPC()
		{
			BaseNPC baseNPC = baseEntity.GetComponent<BaseNPC>();

			if (baseNPC == null)
				return null;

			return new NPC(baseNPC);
		}

		public Player ToPlayer()
		{
			BasePlayer basePlayer = baseEntity.ToPlayer();

			if (basePlayer == null)
				return null;

			return Server.GetPlayer(basePlayer);
		}

		public virtual Vector3 Location {
			get {
				return baseEntity.transform.position;
			}
			set {
				bool oldsync = baseEntity.syncPosition;
				baseEntity.transform.position = value;
				baseEntity.syncPosition = true;
				baseEntity.TransformChanged();
				baseEntity.syncPosition = oldsync;
			}
		}

		public virtual string Name => baseEntity.name == "player/player" ? (baseEntity as BasePlayer).displayName : baseEntity.name;

		public float X => baseEntity.transform.position.x;

		public float Y => baseEntity.transform.position.y;

		public float Z => baseEntity.transform.position.z;
	}
}

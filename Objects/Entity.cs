﻿namespace Pluton.Rust.Objects
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
            BuildingBlock b = baseEntity.GetComponent<BuildingBlock>();
            if (b == null)
                return null;
            return new BuildingPart(b);
        }

        public NPC ToNPC()
        {
            BaseNPC a = baseEntity.GetComponent<BaseNPC>();
            if (a == null)
                return null;
            return new NPC(a);
        }

        public Player ToPlayer()
        {
            BasePlayer p = baseEntity.ToPlayer();
            if (p == null)
                return null;
            return Server.GetPlayer(p);
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


namespace Pluton.Rust.Events {
	using System;
	using Core;
	using Rust;
	using Rust.Objects;

    public class BuildingPartDestroyedEvent : CountedInstance {
        public readonly BuildingPart BuildingPart;
        public readonly HitInfo Info;
        public readonly string HitBone;

        public BuildingPartDestroyedEvent(BuildingBlock buildingBlock, HitInfo info) {
            BuildingPart = new BuildingPart(buildingBlock);
            Info = info;
            string bonename = StringPool.Get(info.HitBone);
            HitBone = bonename == "" ? "unknown" : bonename;
        }

        public float[] DamageAmounts {
            get {
                return Info.damageTypes.types;
            }
            set {
                Info.damageTypes.types = value;
            }
        }

        public global::Rust.DamageType DamageType => Info.damageTypes.GetMajorityDamageType();

        public Entity Attacker {
            get {
                try {
                    if (Info.Initiator != null) {
                        BaseEntity baseEntity = Info.Initiator;
                        BasePlayer basePlayer = baseEntity.GetComponent<BasePlayer>();

                        if (basePlayer != null)
                            return Server.GetPlayer(basePlayer);

                        BaseNPC baseNPC = baseEntity.GetComponent<BaseNPC>();

                        if (baseNPC != null)
                            return new NPC(baseNPC);

                        return new Entity(baseEntity);
                    }

                    return null;
                } catch (Exception ex) {
                    Logger.LogWarning("[BPDestroyedEvent] Got an exception instead of the attacker.");
                    Logger.LogException(ex);

                    return null;
                }
            }
        }

        public InvItem Weapon {
            get {
                try {
                    if (Info.Weapon == null)
                        return null;

                    uint itemUID = (uint) Info.Weapon.GetFieldValue("ownerItemUID");

					BasePlayer ownerPlayer = Info.Weapon.GetOwnerPlayer();

                    if (ownerPlayer == null)
                        return null;

                    return new InvItem(ownerPlayer.inventory.FindItemUID(itemUID));

                } catch (Exception ex) {
                    Logger.LogWarning("[BPDestroyedEvent] Got an exception instead of the weapon.");
                    Logger.LogException(ex);

                    return null;
                }
            }
        }
    }
}

namespace Pluton.Rust.Events
{
	using System;
	using Core;
	using Rust;
	using Rust.Objects;

    public class BuildingPartDestroyedEvent : CountedInstance
    {
        public readonly BuildingPart BuildingPart;

        public readonly HitInfo Info;

        public readonly string HitBone;

        public BuildingPartDestroyedEvent(BuildingBlock bb, HitInfo info)
        {
            Info = info;
            BuildingPart = new BuildingPart(bb);
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
                        BaseEntity ent = Info.Initiator;
                        BasePlayer p = ent.GetComponent<BasePlayer>();
                        if (p != null)
                            return Server.GetPlayer(p);

                        BaseNPC n = ent.GetComponent<BaseNPC>();
                        if (n != null)
                            return new NPC(n);

                        return new Entity(ent);
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
                    uint itemUID = (uint)Info.Weapon.GetFieldValue("ownerItemUID");

                    BasePlayer ownerPlayer = Info.Weapon.ownerPlayer;
                    if (ownerPlayer == null) {
                        return null;
                    }

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
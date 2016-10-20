namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;
	using System;

    public class DeathEvent : CountedInstance
    {
        public readonly HitInfo _info;
        public readonly string HitBone;

        public bool Die = true;
        public bool dropLoot = true;

        public DeathEvent(HitInfo info)
        {
            _info = info;
            string bonename = StringPool.Get(info.HitBone);
            HitBone = bonename == "" ? "unknown" : bonename;
        }

        /******************
        *                 *
        * Generic      0  *
        * Hunger       1  *
        * Thirst       2  *
        * Cold         3  *
        * Drowned      4  *
        * Heat         5  *
        * Bleeding     6  *
        * Poison       7  *
        * Suicide      8  *
        * Bullet       9  *
        * Slash        10 *
        * Blunt        11 *
        * Fall         12 *
        * Radiation    13 *
        * Bite         14 *
        * Stab         15 *
        *                 *
        ******************/

        public float[] DamageAmounts {
            get {
                return _info.damageTypes.types;
            }
            set {
                _info.damageTypes.types = value;
            }
        }

		public global::Rust.DamageType DamageType => _info.damageTypes.GetMajorityDamageType();

        public Entity Attacker {
            get {
                try {
                    if (_info.Initiator != null)
                    {
                        BaseEntity baseEntity = _info.Initiator;
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
                    Logger.LogWarning("[HurtEvent] Got an exception instead of the attacker.");
                    Logger.LogException(ex);
                    return null;
                }
            }
        }

        public InvItem Weapon {
            get {
                try {
                    if (_info.Weapon == null)
                        return null;
                    var itemUID = (uint) _info.Weapon.GetFieldValue("ownerItemUID");

					BasePlayer ownerPlayer = _info.Weapon.GetOwnerPlayer();
                    if (ownerPlayer == null) {
                        return null;
                    }

                    return new InvItem(ownerPlayer.inventory.FindItemUID(itemUID));
                } catch (Exception ex) {
                    Logger.LogWarning("[DeathEvent] Got an exception instead of the weapon.");
                    Logger.LogException(ex);
                    return null;
                }
            }
        }
    }
}

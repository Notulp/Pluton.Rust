﻿namespace Pluton.Rust.Events {
	using Core;
	using Rust;
	using Rust.Objects;
	using System;

    public class HurtEvent : CountedInstance {
        public readonly HitInfo _info;
        public readonly string HitBone;

        public HurtEvent(HitInfo info) {
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
                    if (_info.Initiator != null) {
                        BaseEntity ent = _info.Initiator;
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

                    return new InvItem(_info.Weapon.GetItem());
                } catch (Exception ex) {
                    Logger.LogWarning("[HurtEvent] Got an exception instead of the weapon.");
                    Logger.LogException(ex);
                    return null;
                }
            }
        }
    }
}

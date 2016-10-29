using System.Collections.Generic;

namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class ThrowEvent : Event
    {
        public static Dictionary<string, string> ExplosiveNames = new Dictionary<string, string>()
        {
            {"grenade.f1.entity", "F1 Grenade"},
            {"grenade.beancan.entity", "Beancan Grenade"},
            {"explosive.timed.entity", "Timed Explosive Charge"},
            {"survey_charge", "Survey Charge"},
            {"explosive.satchel.entity", "Satchel Charge"},
            {"smoke_grenade.weapon", "Supply Signal"},
        };

        public readonly ThrownWeapon Projectile;
        public readonly Player Player;
        public readonly string ProjectileName;

        public ThrowEvent(ThrownWeapon thrownWeapon, BaseEntity.RPCMessage msg)
        {
            Projectile = thrownWeapon;
            Player = Server.GetPlayer(msg.player);
            ProjectileName = ExplosiveNames[thrownWeapon.ShortPrefabName];
        }
    }
}

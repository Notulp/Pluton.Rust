using ProtoBuf;

namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class WeaponThrowEvent : Event
    {
        public readonly BaseMelee Weapon;
        public readonly Player Player;
        public readonly ProjectileShoot ProjectileShoot;
        public readonly ProjectileShoot.Projectile Projectile;

        /// <summary>
        /// Modifies the projectile speed.
        /// Must be modified in the Pre_PlayerThrowWeapon hook for the changes to have effect on the projectile.
        /// </summary>
        public float Magnitude;

        public WeaponThrowEvent(BaseMelee weapon, BasePlayer basePlayer, ProjectileShoot projectileShoot, ProjectileShoot.Projectile projectile)
        {
            Weapon = weapon;
            Player = Server.GetPlayer(basePlayer);
            ProjectileShoot = projectileShoot;
            Projectile = projectile;

            Magnitude = projectile.startVel.magnitude;
        }
    }
}

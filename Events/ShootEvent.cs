namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class ShootEvent : Event
    {
        public readonly BaseProjectile BaseProjectile;
        public readonly Player Player;

        public ShootEvent(BaseProjectile baseProjectile, BaseEntity.RPCMessage msg)
        {
            BaseProjectile = baseProjectile;
            Player = Server.GetPlayer(msg.player);
        }
    }
}

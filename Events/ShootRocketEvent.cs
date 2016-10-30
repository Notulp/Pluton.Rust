namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class ShootRocketEvent : Event
    {
        public readonly BaseLauncher BaseLauncher;
        public readonly Entity Entity;
        public readonly Player Player;

        public ShootRocketEvent(BaseLauncher baseLauncher, BaseEntity.RPCMessage msg, BaseEntity baseEntity)
        {
            BaseLauncher = baseLauncher;
            Entity = new Entity(baseEntity);
            Player = Server.GetPlayer(msg.player);
        }
    }
}

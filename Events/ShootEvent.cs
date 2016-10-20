namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class ShootEvent : CountedInstance
    {
        public readonly BaseProjectile BaseProjectile;
        public readonly BaseEntity.RPCMessage RPCMessage;
        public readonly Player Player;

        public ShootEvent(BaseProjectile baseProjectile, BaseEntity.RPCMessage msg)
        {
            RPCMessage = msg;
            Player = Server.GetPlayer(msg.player);
			BaseProjectile = baseProjectile;
        }
	}
}

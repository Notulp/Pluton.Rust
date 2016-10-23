namespace Pluton.Rust.Events {
	using Core;
	using Rust;
	using Rust.Objects;

	public class RocketShootEvent : CountedInstance {
		public readonly BaseLauncher BaseLauncher;
		public readonly BaseEntity.RPCMessage RPCMessage;
		public readonly Entity Entity;
		public readonly Player Player;

		public RocketShootEvent(BaseLauncher baseLauncher, BaseEntity.RPCMessage msg, BaseEntity baseEntity) {
			BaseLauncher = baseLauncher;
			RPCMessage = msg;
			Entity = new Entity(baseEntity);
			Player = Server.GetPlayer(msg.player);
		}
	}
}

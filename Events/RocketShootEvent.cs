namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class RocketShootEvent : CountedInstance
	{
		BaseEntity.RPCMessage _msg;
		BaseLauncher _launch;
		Entity _entity;
		Player _player;

		public RocketShootEvent(BaseLauncher baseLauncher, BaseEntity.RPCMessage msg, BaseEntity baseEntity)
		{
			_entity = new Entity(baseEntity);
			_player = Server.GetPlayer(msg.player);
			_msg = msg;
			_launch = baseLauncher;
		}

		public BaseLauncher BaseLauncher => _launch;

		public Player Player => _player;

		public Entity Entity => _entity;

		public BaseEntity.RPCMessage RPCMessage => _msg;
	}
}

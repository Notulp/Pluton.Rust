namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class ShootEvent : CountedInstance
	{
		private BaseEntity.RPCMessage _rpcMessage;
		private BaseProjectile _projectile;
		private Player _player;

		public ShootEvent(BaseProjectile baseProjectile, BaseEntity.RPCMessage msg)
		{
			_player = Server.GetPlayer(msg.player);
			_rpcMessage = msg;
			_projectile = baseProjectile;
		}

		public BaseProjectile BaseProjectile => _projectile;

		public Player Player => _player;

		public BaseEntity.RPCMessage RPCMessage => _rpcMessage;
	}
}

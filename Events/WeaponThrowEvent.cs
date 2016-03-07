namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class WeaponThrowEvent : CountedInstance
	{
		ThrownWeapon _thrownWeapon;
		BaseEntity.RPCMessage _msg;
		Player _player;

		public WeaponThrowEvent(ThrownWeapon thrownWeapon, BaseEntity.RPCMessage msg)
		{
			_msg = msg;
			_thrownWeapon = thrownWeapon;
			_player = Server.GetPlayer(msg.player);
		}

		public BaseEntity.RPCMessage RPCMessage => _msg;

		public ThrownWeapon Weapon => _thrownWeapon;

		public Player Player => _player;
	}
}

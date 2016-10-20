namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class WeaponThrowEvent : CountedInstance
	{
		public readonly ThrownWeapon Weapon;
		public readonly BaseEntity.RPCMessage RPCMessage;
		public readonly Player Player;

		public WeaponThrowEvent(ThrownWeapon thrownWeapon, BaseEntity.RPCMessage msg)
		{
			Weapon = thrownWeapon;
		    RPCMessage = msg;
			Player = Server.GetPlayer(msg.player);
		}
	}
}

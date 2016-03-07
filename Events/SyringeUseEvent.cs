namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class SyringeUseEvent : CountedInstance
	{
		private Player _user;
		private Player _receiver;
		private SyringeWeapon _syringeWeapon;
		private bool _self;

		public SyringeUseEvent(SyringeWeapon sw, BaseEntity.RPCMessage msg, bool isSelf)
		{
			_syringeWeapon = sw;
			_user = Server.GetPlayer(sw.ownerPlayer);
			_self = isSelf;

			if (isSelf)
				_receiver = _user;
			else
				_receiver = new Player(BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer);
		}

		public Player User => _user;

		public Player Receiver => _receiver;

		public SyringeWeapon Syringe =>  _syringeWeapon;

		public bool IsSelfUsage() => _self;
	}
}

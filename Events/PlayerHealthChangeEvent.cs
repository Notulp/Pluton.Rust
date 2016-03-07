namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class PlayerHealthChangeEvent : CountedInstance
	{
		float _oldh, _newh;
		Player _pl;

		public PlayerHealthChangeEvent(BasePlayer p, float oldh, float newh)
		{
			_oldh = oldh;
			_newh = newh;
			_pl = Server.GetPlayer(p);
		}

		public Player Player
		{
			get { return _pl; }
		}

		public float OldHealth
		{
			get { return _oldh; }
		}

		public float NewHealth
		{
			get { return _newh; }
		}
	}
}

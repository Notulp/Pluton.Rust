namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class PlayerHealthChangeEvent : CountedInstance
	{
		public readonly Player Player;
		public readonly float OldHealth;
		public readonly float NewHealth;

		public PlayerHealthChangeEvent(BasePlayer basePlayer, float oldHealth, float newHealth)
		{
			Player = Server.GetPlayer(basePlayer);
			OldHealth = oldHealth;
			NewHealth = newHealth;
		}
	}
}

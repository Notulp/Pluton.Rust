namespace Pluton.Rust.Events
{
	using Rust.Objects;

	public class PlayerHurtEvent : HurtEvent
	{
		public readonly Player Victim;

		public PlayerHurtEvent(Player player, HitInfo info)
			: base(info)
		{
			Victim = player;
		}
	}
}


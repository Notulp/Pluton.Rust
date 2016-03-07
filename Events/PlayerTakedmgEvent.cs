namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;

	public class PlayerTakedmgEvent : CountedInstance
	{
		public readonly Player Victim;
		public float Amount;
		public global::Rust.DamageType Type;

		public PlayerTakedmgEvent(Player p, float amount, global::Rust.DamageType type)
		{
			Type = type;
			Amount = amount;
			Victim = p;
		}
	}
}


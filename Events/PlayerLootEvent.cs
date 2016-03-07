namespace Pluton.Rust.Events
{
	using Rust.Objects;

	public class PlayerLootEvent : LootEvent
	{
		public readonly Player Target;

		public PlayerLootEvent(PlayerLoot pl, Player looter, Player looted)
			: base(pl, looter)
		{
			Target = looted;
		}
	}
}


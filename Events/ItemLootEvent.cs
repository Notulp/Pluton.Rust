namespace Pluton.Rust.Events
{
	using Rust.Objects;

	public class ItemLootEvent : LootEvent
	{
		public readonly Item Target;

		public ItemLootEvent(PlayerLoot pl, Player looter, Item looted)
			: base(pl, looter)
		{
			Target = looted;
		}
	}
}


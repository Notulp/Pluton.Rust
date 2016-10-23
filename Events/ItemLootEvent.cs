namespace Pluton.Rust.Events
{
	using Rust.Objects;

	public class ItemLootEvent : LootEvent
	{
		public readonly Item Target;

		public ItemLootEvent(PlayerLoot playerLoot, Player looter, Item looted)
			: base(playerLoot, looter)
		{
			Target = looted;
		}
	}
}


namespace Pluton.Rust.Events {
	using Rust.Objects;

	public class EntityLootEvent : LootEvent {
		public readonly Entity Target;

		public EntityLootEvent(PlayerLoot playerLoot, Player looter, Entity looted)
			: base(playerLoot, looter) {
			Target = looted;
		}
	}
}

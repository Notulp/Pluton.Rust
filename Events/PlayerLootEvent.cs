namespace Pluton.Rust.Events
{
    using Rust.Objects;

    public class PlayerLootEvent : LootEvent
    {
        public readonly Player Target;

        public PlayerLootEvent(PlayerLoot playerLoot, Player looter, Player looted)
            : base(playerLoot, looter)
        {
            Target = looted;
        }
    }
}

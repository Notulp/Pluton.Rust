namespace Pluton.Rust.Events
{
	using Rust;
	using Rust.Objects;

    public class EntityLootEvent : LootEvent
    {
        public readonly Entity Target;

        public EntityLootEvent(PlayerLoot pl, Player looter, Entity looted)
            : base(pl, looter)
        {
            Target = looted;
        }
    }
}


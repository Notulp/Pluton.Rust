namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class GatherEvent : CountedInstance
    {
        public readonly ResourceDispenser ResourceDispenser;
        public readonly Entity Resource;
        public readonly Player Gatherer;
        public readonly ItemAmount ItemAmount;

        public int Amount;

        public GatherEvent(ResourceDispenser resourceDispenser, BaseEntity from, BaseEntity to, ItemAmount itemAmount, int amount)
        {
            if (to is BasePlayer) {
                ResourceDispenser = resourceDispenser;
                Resource = new Entity(from);
                Gatherer = Server.GetPlayer(to as BasePlayer);
                ItemAmount = itemAmount;
                Amount = (int) (amount * World.GetInstance().ResourceGatherMultiplier);
            }
        }
    }
}

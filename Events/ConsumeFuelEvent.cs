namespace Pluton.Rust.Events
{
    using Core;
    using Rust.Objects;

    public class ConsumeFuelEvent : Event
    {
        public readonly BaseOven BaseOven;
        public readonly InvItem Item;
        public readonly ItemModBurnable Burnable;

        public ConsumeFuelEvent(BaseOven baseOven, Item fuel, ItemModBurnable burnable)
        {
            BaseOven = baseOven;
            Item = new InvItem(fuel);
            Burnable = burnable;
        }
    }
}

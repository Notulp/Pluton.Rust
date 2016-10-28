namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class InventoryModEvent : Event
    {
        public readonly InvItem Item;
        public readonly ItemContainer ItemContainer;
        public readonly Player Player;
        public readonly Entity Entity;

        public InventoryModEvent(ItemContainer itemContainer, Item item)
        {
            ItemContainer = itemContainer;
            Item = new InvItem(item);

            if (itemContainer.playerOwner != null)
                Player = Server.GetPlayer(itemContainer.playerOwner);

            if (itemContainer.entityOwner != null)
                Entity = new Entity(itemContainer.entityOwner);
        }
    }
}

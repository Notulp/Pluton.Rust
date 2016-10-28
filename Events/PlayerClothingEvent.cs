namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class PlayerClothingEvent : Event
    {
        public readonly Player Player;
        public readonly InvItem Item;

        public PlayerClothingEvent(PlayerInventory playerInventory, Item item)
        {
            Player = Server.GetPlayer(playerInventory.containerMain.playerOwner);
            Item = new InvItem(item);
        }
    }
}

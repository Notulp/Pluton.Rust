namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class PlayerClothingEvent : CountedInstance
	{
        public readonly Player Player;
        public readonly InvItem Item;
        
        public PlayerClothingEvent(PlayerInventory playerInventory, Item item)
        {
            Player = Server.GetPlayer(playerInventory.GetComponent<BasePlayer>());
            Item = new InvItem(item);
		}
	}
}

namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class PlayerClothingEvent : CountedInstance
	{
		private InvItem _item;
		private Player _player;

		public PlayerClothingEvent(PlayerInventory playerInventory, Item item)
		{
			_item = new InvItem(item);
			_player = Server.GetPlayer(playerInventory.GetComponent<BasePlayer>());
		}

		public Player Player
		{
			get { return _player; }
		}

		public InvItem Item
		{
			get { return _item; }
		}
	}
}

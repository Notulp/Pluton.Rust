namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class ItemPickupEvent : CountedInstance
	{
		public readonly InvItem Item;
		public readonly Player Player;
		public readonly BaseEntity.RPCMessage RPCMessage;
		public readonly CollectibleEntity Entity;

		public ItemPickupEvent(CollectibleEntity collectibleEntity, BaseEntity.RPCMessage msg, Item item)
		{
			Entity = collectibleEntity;
			RPCMessage = msg;
			Player = Server.GetPlayer(msg.player);
			Item = new InvItem(item);
		}
	}
}

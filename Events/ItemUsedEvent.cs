namespace Pluton.Rust.Events {
	using Core;
	using Rust.Objects;

	public class ItemUsedEvent : CountedInstance {
		public readonly InvItem Item;
		public readonly int Amount;

		public ItemUsedEvent(Item item, int amount) {
			Item = new InvItem(item);
			Amount = amount;
		}
	}
}

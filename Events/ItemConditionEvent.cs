namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class ItemConditionEvent : Event
	{
		public readonly InvItem Item;
		public readonly float Amount;
		public readonly Player Player;

		public ItemConditionEvent(Item item, float amount)
		{
			Item = new InvItem(item);
			Amount = amount;

			BasePlayer ownerPlayer = item.GetOwnerPlayer();

			if (ownerPlayer != null) {
				Player = Server.GetPlayer(ownerPlayer);
			}
		}
	}
}

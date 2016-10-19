namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class ItemConditionEvent : CountedInstance
    {
        private InvItem _item;
        private float _amount;

        public Player Player;

        public ItemConditionEvent(Item item, float amount)
        {
            _item = new InvItem(item);
            _amount = amount;

            BasePlayer ownerPlayer = item.GetOwnerPlayer();

            if (ownerPlayer != null)
            {
                Player = Server.GetPlayer(ownerPlayer);
            }
        }

        public InvItem Item {
            get { return _item; }
        }

        public float Amount {
            get { return _amount; }
        }
    }
}
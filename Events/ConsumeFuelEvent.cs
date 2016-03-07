namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;

    public class ConsumeFuelEvent : CountedInstance
    {
        BaseOven _baseOven;
        InvItem _item;
        ItemModBurnable _burn;

        public ConsumeFuelEvent(BaseOven bo, Item fuel, ItemModBurnable burn)
        {
            _baseOven = bo;
            _item = new InvItem(fuel);
            _burn = burn;
        }

        public BaseOven BaseOven {
            get { return _baseOven; }
        }

        public InvItem Item {
            get { return _item; }
        }

        public ItemModBurnable Burnable {
            get { return _burn; }
        }

    }
}

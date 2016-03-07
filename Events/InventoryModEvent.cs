namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class InventoryModEvent : CountedInstance
    {
        private InvItem _item;
        private ItemContainer _itemContainer;
        private Player _owner;
        private Entity _entity;

        public InventoryModEvent(ItemContainer ic, Item i)
        {
            _item = new InvItem(i);
            _itemContainer = ic;

            if (ic.entityOwner != null)
                _entity = new Entity(ic.entityOwner);
            if (ic.playerOwner != null)
                _owner = Server.GetPlayer(ic.playerOwner);
        }

        public InvItem Item => _item;

        public ItemContainer ItemContainer => _itemContainer;

        public Player Player => _owner;

        public Entity Entity => _entity;
    }
}
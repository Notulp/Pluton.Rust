namespace Pluton.Rust.Objects
{
	using Core;
	using System.Linq;
	using System.Collections.Generic;

	public class Inv : CountedInstance
	{
		public readonly PlayerInventory _inv;
		public readonly Player owner;

		public Inv(PlayerInventory inv)
		{
			_inv = inv;
			owner = Server.GetPlayer(inv.GetComponent<BasePlayer>());
		}

		public bool Add(InvItem item)
		{
			ItemContainer container;

			if (item.containerPref == InvItem.ContainerPreference.Belt)
				container = InnerBelt;
			else if (item.containerPref == InvItem.ContainerPreference.Wear)
				container = InnerWear;
			else
				container = InnerMain;

			bool flag = _inv.GiveItem(item._item, container);

			if (!flag) {
				flag = _inv.GiveItem(item._item);
			}

			return flag;
		}

		public bool Add(InvItem item, ItemContainer con) => _inv.GiveItem(item._item, con);

		public bool Add(int itemID) => Add(itemID, 1);

		public bool Add(int itemID, int amount) => Add(new InvItem(itemID, amount));

		public bool Add(string longNameOrShortNameOrPrefab, int amount) => Add(new InvItem(longNameOrShortNameOrPrefab, amount));

		public void Notice(LoadOutItem loItem) => Notice($"{InvItem.GetItemID(loItem.Name)} {loItem.Amount}");

		public void Notice(InvItem item) => Notice($"{item.ItemID} {item.Quantity}");

		public void Notice(string msg) => owner.basePlayer.Command("note.inv " + msg);

		public ItemContainer InnerBelt => _inv.containerBelt;

		public ItemContainer InnerMain => _inv.containerMain;

		public ItemContainer InnerWear => _inv.containerWear;

		public List<InvItem> AllItems() => (from item in _inv.AllItems()
											select new InvItem(item)).ToList();

		public List<InvItem> BeltItems() => (from item in _inv.containerBelt.itemList
											 select new InvItem(item)).ToList();

		public List<InvItem> MainItems() => (from item in _inv.containerMain.itemList
											 select new InvItem(item)).ToList();

		public List<InvItem> WearItems() => (from item in _inv.containerWear.itemList
											 select new InvItem(item)).ToList();
	}
}

namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class CraftEvent : CountedInstance
    {
        public readonly Player Crafter;

        public ItemCrafter itemCrafter;
        public ItemBlueprint bluePrint;
        public ItemDefinition Target;
        public int Amount;
        public bool Cancel = false;
        public string cancelReason = string.Empty;
        public int SkinID;

        public CraftEvent(ItemCrafter itemCrafter, ItemBlueprint itemBlueprint, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData, int amount, int skinid)
        {
            this.itemCrafter = itemCrafter;
            bluePrint = itemBlueprint;
            Crafter = Server.GetPlayer(owner);
            Target = itemBlueprint.targetItem;
            Amount = amount;
            SkinID = skinid;
        }

        public void Stop(string reason = "A plugin stops you from crafting that!")
        {
            cancelReason = reason;
            Cancel = true;
        }

        public float CraftTime {
            get {
                return bluePrint.time;
            }
            set {
                bluePrint.time = value;
            }
        }
    }
}

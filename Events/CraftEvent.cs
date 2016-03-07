namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class CraftEvent : CountedInstance
    {
        public Player Crafter;

        public ItemDefinition Target;

        public ItemCrafter itemCrafter;

        public ItemBlueprint bluePrint;

        public int Amount;

        public bool Cancel = false;

        public string cancelReason = System.String.Empty;

        public int SkinID;

        public CraftEvent(ItemCrafter self, ItemBlueprint bp, BasePlayer owner, ProtoBuf.Item.InstanceData instanceData, int amount, int skinid)
        {
            Crafter = Server.GetPlayer(owner);
            Target = bp.targetItem;
            itemCrafter = self;
            Amount = amount;
            bluePrint = bp;
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


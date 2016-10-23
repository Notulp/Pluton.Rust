namespace Pluton.Rust.Events {
	using Core;
	using Rust;
	using Rust.Objects;

	public class ItemRepairEvent : CountedInstance {
		public readonly RepairBench RepairBench;
		public readonly Player Player;
		public readonly InvItem Item;
		public readonly BaseEntity.RPCMessage RPCMessage;

		public ItemRepairEvent(RepairBench repairBench, BaseEntity.RPCMessage msg) {
			RepairBench = repairBench;
			RPCMessage = msg;
			Player = Server.GetPlayer(msg.player);
			Item = new InvItem(repairBench.inventory.GetSlot(0));
		}
	}
}

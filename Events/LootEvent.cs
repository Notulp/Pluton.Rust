namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;

	public class LootEvent : CountedInstance
	{
		public readonly Player Looter;
		public readonly PlayerLoot pLoot;

		public bool Cancel = false;
		public string cancelReason = "A plugin stops you from looting that!";

		public LootEvent(PlayerLoot playerLoot, Player looter)
		{
			Looter = looter;
			pLoot = playerLoot;
		}

		public void Stop(string reason = "A plugin stops you from looting that!")
		{
			cancelReason = reason;
			Cancel = true;
		}
	}
}

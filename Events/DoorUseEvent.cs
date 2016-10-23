namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;

	public class DoorUseEvent : CountedInstance
	{
		public readonly Player Player;
		public readonly Entity Door;

		public bool Allow = true;
		public bool Open;
		public bool IgnoreLock = false;

		public string DenyReason = "";

		public DoorUseEvent(Entity door, Player player, bool open)
		{
			Door = door;
			Player = player;
			Open = open;
		}

		public void Deny(string reason = "")
		{
			Allow = false;
			DenyReason = reason;
		}
	}
}

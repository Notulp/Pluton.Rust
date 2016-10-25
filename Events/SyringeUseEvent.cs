namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class SyringeUseEvent : Event
	{
		public readonly MedicalTool Syringe;
		public readonly Player User;
		public readonly Player Receiver;

		public SyringeUseEvent(MedicalTool syringe, BasePlayer owner, BasePlayer target)
		{
			Syringe = syringe;
			User = Server.GetPlayer(owner);
		    Receiver = Server.GetPlayer(target);
		}
    }
}

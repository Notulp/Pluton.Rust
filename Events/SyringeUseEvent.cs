namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class SyringeUseEvent : CountedInstance
	{
		public readonly MedicalTool Syringe;
		public readonly BaseEntity.RPCMessage RPCMessage;
		public readonly Player User;
		public readonly Player Receiver;

		public SyringeUseEvent(MedicalTool syringe, BaseEntity.RPCMessage msg, bool isSelf)
		{
			Syringe = syringe;
			RPCMessage = msg;
			User = Server.GetPlayer(syringe.GetOwnerPlayer());

			if (isSelf)
				Receiver = User;
			else
				Receiver = new Player(BaseNetworkable.serverEntities.Find(msg.read.UInt32()) as BasePlayer);
		}
	}
}

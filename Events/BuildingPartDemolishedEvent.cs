namespace Pluton.Rust.Events {
	using Core;
	using Rust.Objects;
	using Rust;

	public class BuildingPartDemolishedEvent : CountedInstance {
		public readonly BuildingPart BuildingPart;
		public readonly Player Player;

		public BuildingPartDemolishedEvent(BuildingBlock buildingBlock, BasePlayer basePlayer) {
			BuildingPart = new BuildingPart(buildingBlock);
			Player = Server.GetPlayer(basePlayer);
		}
	}
}

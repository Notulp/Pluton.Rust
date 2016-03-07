namespace Pluton.Rust.Events
{
	using Core;
	using Rust.Objects;
	using Rust;

    public class BuildingPartDemolishedEvent : CountedInstance
    {
        public readonly Player Player;
        public readonly BuildingPart BuildingPart;

        public BuildingPartDemolishedEvent(BuildingBlock bb, BasePlayer basePlayer)
        {
            BuildingPart = new BuildingPart(bb);
            Player = Server.GetPlayer(basePlayer);
        }
    }
}
namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class BuildingUpgradeEvent : Event
    {
        public readonly BuildingPart BuildingPart;
        public readonly BuildingGrade.Enum Grade;
        public readonly Player Player;

        public BuildingUpgradeEvent(BuildingBlock buildingBlock, BuildingGrade.Enum buildingGrade, BasePlayer basePlayer)
        {
            BuildingPart = new BuildingPart(buildingBlock);
            Grade = buildingGrade;
            Player = Server.GetPlayer(basePlayer);
        }
    }
}

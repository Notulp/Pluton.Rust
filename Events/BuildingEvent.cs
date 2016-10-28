namespace Pluton.Rust.Events
{
    using System;
    using Core;
    using Rust;
    using Rust.Objects;

    public class BuildingEvent : Event
    {
        public readonly Construction Construction;
        public readonly Construction.Target Target;
        public readonly BuildingPart BuildingPart;
        public readonly Player Builder;
        public readonly bool NeedsValidPlacement;

        public string DestroyReason = string.Empty;
        public bool DoDestroy = false;

        public BuildingEvent(Construction construction,
                             Construction.Target target,
                             BuildingBlock buildingBlock,
                             bool needsValidPlacement)
        {
            Construction = construction;
            Target = target;
            BuildingPart = new BuildingPart(buildingBlock);
            Builder = Server.GetPlayer(target.player);
            NeedsValidPlacement = needsValidPlacement;
        }

        public void Destroy(string reason = "Plugin blocks building!")
        {
            DoDestroy = true;
            DestroyReason = reason;
        }
    }
}

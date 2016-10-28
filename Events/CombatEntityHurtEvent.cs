namespace Pluton.Rust.Events
{
    using Rust.Objects;

    public class CombatEntityHurtEvent : HurtEvent
    {
        public readonly Entity Victim;

        public CombatEntityHurtEvent(BaseCombatEntity combatEntity, HitInfo info)
            : base(info)
        {
            BuildingBlock buildingBlock = combatEntity.GetComponent<BuildingBlock>();

            if (buildingBlock != null)
                Victim = new BuildingPart(buildingBlock);
            else
                Victim = new Entity(combatEntity);
        }
    }
}

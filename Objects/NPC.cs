namespace Pluton.Rust.Objects
{
    public class NPC : Entity
    {
        public readonly BaseNPC baseNPC;

        public NPC(BaseNPC npc) : base(npc)
        {
            baseNPC = npc;
        }

        public override void Kill()
        {
            HitInfo info = new HitInfo();

            info.damageTypes.Add(global::Rust.DamageType.Suicide, 100f);
            info.Initiator = baseNPC as BaseEntity;

            baseNPC.Die(info);
        }

		public override bool IsNPC() => true;

		public uint ID => baseNPC.net.ID;

        public float Health {
            get {
                return baseNPC.health;
            }
            set {
                baseNPC.health = value;
            }
        }
    }
}

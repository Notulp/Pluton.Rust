namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class HammerEvent : CountedInstance
    {
        public readonly HitInfo _info;
        public readonly string HitBone;
        public readonly BasePlayer basePlayer;

        public HammerEvent(HitInfo info, BasePlayer player)
        {
            _info = info;
            basePlayer = player;
            string bonename = StringPool.Get(info.HitBone);
            HitBone = bonename == "" ? "unknown" : bonename;
        }

        public Player Player {
            get { return Server.GetPlayer(basePlayer); }
        }

        public Entity Victim {
            get {
                BaseEntity baseEntity = _info.HitEntity;
                BasePlayer basePlayer = baseEntity.GetComponent<BasePlayer>();

                if (basePlayer != null)
                    return Server.GetPlayer(basePlayer);

                BaseNPC baseNPC = baseEntity.GetComponent<BaseNPC>();

                if (baseNPC != null)
                    return new NPC(baseNPC);

                return new Entity(baseEntity);
            }
        }
    }
}

namespace Pluton.Rust.Events
{
    using Core;
    using Rust.Objects;

    public class CorpseInitEvent : Event
    {
        public readonly BaseCorpse Corpse;
        public readonly Entity Parent;

        public CorpseInitEvent(BaseCorpse baseCorpse, BaseEntity baseEntity)
        {
            Corpse = baseCorpse;
            Parent = new Entity(baseEntity);
        }
    }
}

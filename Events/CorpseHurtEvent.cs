namespace Pluton.Rust.Events {
	public class CorpseHurtEvent : HurtEvent {
		public readonly BaseCorpse corpse;

		public CorpseHurtEvent(BaseCorpse baseCorpse, HitInfo info)
			: base(info) {
			corpse = baseCorpse;
		}
	}
}

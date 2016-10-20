namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class LandmineTriggerEvent : CountedInstance
    {
        public readonly Landmine Landmine;
        public readonly Player Player;

        public bool Explode = true;

		public LandmineTriggerEvent(Landmine landmine, BasePlayer player)
		{
			Landmine = landmine;
			Player = Server.GetPlayer(player);
		}

		public void CancelExplosion()
		{
			Explode = false;
		}
	}
}

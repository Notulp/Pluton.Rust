namespace Pluton.Rust.Events
{
    using Core;
    using Rust;
    using Rust.Objects;

    public class LandmineTriggerEvent : Event
    {
        public readonly Landmine Landmine;
        public readonly Player Player;
        
        public LandmineTriggerEvent(Landmine landmine, BasePlayer player)
        {
            Landmine = landmine;
            Player = Server.GetPlayer(player);
        }
    }
}

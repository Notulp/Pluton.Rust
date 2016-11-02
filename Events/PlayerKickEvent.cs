namespace Pluton.Rust.Events
{
    using Core;
    using Network;

    public class PlayerKickEvent : Event
    {
        public bool Kick = false;
        public string Reason;

        public Connection Connection;

        public PlayerKickEvent(Connection connection, string reason)
        {
            Connection = connection;
            Reason = reason;
        }
    }
}

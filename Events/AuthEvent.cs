namespace Pluton.Rust.Events
{
	using Network;
	using Core;

    public class AuthEvent : CountedInstance
    {
        public bool Approved;

        public readonly Connection Connection;

        public ulong GameID => Connection.userid;

        public string IP => Connection.ipaddress;

        public string Name => Connection.username;

        public string OS => Connection.os;

        public string Reason;

        public AuthEvent(Connection connection)
        {
            Connection = connection;
            Approved = true;
        }

        public void Reject(string reason = "no reason")
        {
            Approved = false;
            Reason = reason;
        }
    }
}


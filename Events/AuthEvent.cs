namespace Pluton.Rust.Events
{
	using Network;
	using Core;

    public class AuthEvent : CountedInstance
    {
        public readonly Connection Connection;

        public bool Approved;
        public string Reason;
        
        public AuthEvent(Connection connection)
        {
            Connection = connection;
            Approved = true;
        }

        public void Reject(string reason = "No reason.")
        {
            Approved = false;
            Reason = reason;
        }

        public ulong GameID => Connection.userid;

        public string IP => Connection.ipaddress;

        public string Name => Connection.username;

        public string OS => Connection.os;
    }
}

namespace Pluton.Rust.Events
{
    using Core;
    using Rust.Objects;

    public class DoorUseEvent : Event
    {
        public readonly Player Player;
        public readonly Entity Door;
        
        public bool Open;
        public bool IgnoreLock = false;

        public string DenyReason = "";

        public DoorUseEvent(Door door, BaseEntity.RPCMessage msg, bool open)
        {
            Door = new Entity(door);
            Player = Server.GetPlayer(msg.player);
            Open = open;
        }
    }
}

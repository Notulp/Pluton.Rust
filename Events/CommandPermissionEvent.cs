namespace Pluton.Rust.Events
{
    using Rust.Objects;

    public class CommandPermissionEvent : CommandEvent
    {
        public readonly ChatCommand ChatCommand;

        public bool Blocked = false;

        public CommandPermissionEvent(Player player, string[] command, ChatCommand chatCmd)
            : base(player, command)
        {
            ChatCommand = chatCmd;
        }

        public void BlockCommand(string reason)
        {
            Reply = reason;
            Blocked = true;
        }

        public string PluginName => ChatCommand.plugin.Name;
    }
}

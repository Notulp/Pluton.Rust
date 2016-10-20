namespace Pluton.Rust.Events
{
	using System;
	using Core;
	using Rust.Objects;

    public class CommandEvent : CountedInstance
    {
        public readonly Player User;
        public readonly string[] Args;
        public readonly string Cmd;

        public string[] QuotedArgs => Util.GetInstance().GetQuotedArgs(Args);
        public string Reply;

        public CommandEvent(Player player, string[] command)
        {
            User = player;
            Args = new string[command.Length - 1];
            Cmd = command[0];
            Reply = string.Format("/{0} executed!", string.Join(" ", command));
            Array.Copy(command, 1, Args, 0, command.Length - 1);
        }

        public void ReplyWith(string msg) => Reply = msg;
    }
}

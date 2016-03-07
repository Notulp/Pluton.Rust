namespace Pluton.Rust.Events
{
	using System;
	using Core;
	using Rust.Objects;

    public class CommandEvent : CountedInstance
    {

        public readonly string[] Args;

        public readonly string Cmd;

        public string[] QuotedArgs => Core.Util.GetInstance().GetQuotedArgs(Args);

        public string Reply;

        public readonly Player User;

        public CommandEvent(Player player, string[] command)
        {
            User = player;
            Reply = String.Format("/{0} executed!", String.Join(" ", command));
            Cmd = command[0];
            Args = new string[command.Length - 1];
            Array.Copy(command, 1, Args, 0, command.Length - 1);
        }

        public void ReplyWith(string msg) => Reply = msg;
    }
}


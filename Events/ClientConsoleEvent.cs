namespace Pluton.Rust.Events
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Rust;
	using Rust.Objects;

    public class ClientConsoleEvent : CountedInstance
    {
        public readonly ConsoleSystem.Arg _args;
        public readonly Player User;

        public readonly string Cmd;

        public readonly List<string> Args;
        public string Reply;

        public ClientConsoleEvent(ConsoleSystem.Arg arg, string rconCmd)
        {
			_args = arg;
            User = Server.GetPlayer((BasePlayer)arg.connection.player);
            Args = new List<string>();

            Reply = "Command not found!";

            if (String.IsNullOrEmpty(rconCmd))
                return;

            foreach (string str in rconCmd.Split(' '))
                Args.Add(str);

            Cmd = Args[0];
            Args.RemoveAt(0);
        }

        public void ReplyWith(string msg) => Reply = msg;
    }
}

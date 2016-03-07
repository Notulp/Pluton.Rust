namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

    public class ChatEvent : CountedInstance
    {
        public readonly ConsoleSystem.Arg Arg;

        public readonly string OriginalText;
        public readonly Player User;
        public string BroadcastName;
        public string FinalText;
        public string Reply;

        public ChatEvent(Player player, ConsoleSystem.Arg args)
        {
            User = player;
            Arg = args;
            if (args.connection != null)
                BroadcastName = args.connection.username;
            else
                BroadcastName = Server.server_message_name;
            OriginalText = args.ArgsStr.Substring(1, args.ArgsStr.Length - 2).Replace("\\", "");
            FinalText = OriginalText.Replace('<', '[').Replace('>', ']');
            Reply = "chat.say was executed";
        }

        public void Cancel(string reply = "Your message was not sent")
        {
            FinalText = "";
            Reply = reply;
        }

        public void ReplyWith(string msg) => Reply = msg;
    }
}


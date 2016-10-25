namespace Pluton.Rust.Events
{
	using Core;
	using Rust;
	using Rust.Objects;

	public class ChatEvent : Event
	{
		public readonly ConsoleSystem.Arg Arg;
		public readonly string OriginalText;
		public readonly Player User;

		public string BroadcastName;
		public string FinalText;
		public string Reply;
		public bool AllowFormatting = false;

		public ChatEvent(Player player, ConsoleSystem.Arg args)
		{
			Arg = args;
			OriginalText = args.ArgsStr.Substring(1, args.ArgsStr.Length - 2).Replace("\\", "");
			User = player;

			if (args.connection != null)
				BroadcastName = args.connection.username.Replace('<', '[').Replace('>', ']');
			else
				BroadcastName = Server.server_message_name;

			FinalText = OriginalText;
			Reply = "";
		}

		public void Cancel(string reply = "Your message was not sent")
		{
			FinalText = "";
			Reply = reply;
		}

		public void ReplyWith(string msg) => Reply = msg;
	}
}

namespace Pluton.Rust.Objects
{
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using Core.PluginLoaders;
	using Core;

	public class PluginCommand
	{
		public string _command;
		public string _description;
		public string _usage;

		public PluginCommand(string command = "", string description "_not specified_", string usage = "_not specified")
		{
			_command = command;
			_usage = usage;
			_description = description;
		}
	}

	// TODO: should be singleton
	public class PluginCommands
	{
		private static PluginCommands instance;

		public static Dictionary<int, PluginCommand> Commands = new Dictionary<int, PluginCommand>();

		public static PluginCommands GetInstance()
		{
			if (instance == null)
				new PluginCommands();

			return instance;
		}

		public void Init() {
			Commands.Clear();

			if (Config.GetInstance().GetBoolValue("Commands", "enabled", true)) {
				RegisterCommand(GetPlutonCommand("ShowMyStats", "mystats"),
				                GetPlutonCommand("ShowMyStats", "mystats"),
				                "Shows your stat.");
				
				RegisterCommand(GetPlutonCommand("ShowStatsOther", "statsof"),
				                GetPlutonCommand("ShowStatsOther", "statsof") + " \"<playername>\"",
				                "Shows another player's stat.");
				
				RegisterCommand(GetPlutonCommand("ShowLocation", "whereami"),
				                GetPlutonCommand("ShowLocation", "whereami"),
				                "Shows where you are.");
				
				RegisterCommand(GetPlutonCommand("ShowOnlinePlayers", "players"),
				                GetPlutonCommand("ShowOnlinePlayers", "players"),
				                "Shows how many ppl are online.");
				
				RegisterCommand(GetPlutonCommand("Help", "help"),
				                GetPlutonCommand("Help", "help"),
				                "Shows the basic help message(s).");
				
				RegisterCommand(GetPlutonCommand("Commands", "commands"),
				                GetPlutonCommand("Commands", "commands"),
				                "Shows the list of commands.");
			}

			PluginLoader.LoadCommands();
		}

		public string GetPlutonCommand(string c, string defaultValue)
		{
			if (String.IsNullOrEmpty(c))
				return "";

			string c2 = Config.GetInstance().GetValue("Commands", c, defaultValue);

			if (c2 == null)
				return "";

			return c2;
		}

		public void RegisterCommand(string command, string usage, string description)
		{
			if (String.IsNullOrEmpty(command))
				return;

			PluginCommand cmd = new PluginCommand();
			cmd._description = description;
			cmd._command = command;
			cmd._usage = usage;

			RegisterCommand(cmd);
		}

		public void RegisterCommand(PluginCommand command) => Commands.Add(Commands.Count, command);

		public PluginCommands() {
			if (instance == null)
				instance = this;
		}

		public List<string> getCommands()
		{
			return (from c in Commands.Values
			                 select c._command).ToList<string>();
		}

		public string[] getDescriptions(string command)
		{
			return (from c in Commands.Values
			                 where c._command == command
			                 select c._description).ToArray<string>();
		}

		public string[] getUsages(string command)
		{
			return (from c in Commands.Values
			                 where c._command == command
			                 select c._usage).ToArray<string>();
		}
	}
}

using System;
using Pluton.Rust.Objects;
namespace Pluton.Rust.PluginLoaders
{
	public class LUAPlugin : Core.PluginLoaders.LUAPlugin
	{
		public LUAPlugin(string name) : base(name) { }

		public override void AssignVariables()
		{
			script.Globals["Find"] = Find.GetInstance();
			script.Globals["Server"] = Server.GetInstance();
			script.Globals["ServerConsoleCommands"] = new ConsoleCommands(this);
			script.Globals["Commands"] = new ChatCommands(this);
			script.Globals["World"] = World.GetInstance();
		}
	}
}


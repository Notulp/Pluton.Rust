using System;
using Pluton.Rust.Objects;
namespace Pluton.Rust.PluginLoaders
{
	public class JSPlugin : Core.PluginLoaders.JSPlugin
	{
		public JSPlugin(string name) : base (name) { }

		public override void AssignVariables()
		{
			Engine.SetParameter("Commands", new ChatCommands(this))
			      .SetParameter("ServerConsoleCommands", new ConsoleCommands(this))
			      .SetParameter("Find", Find.GetInstance())
			      .SetParameter("Server", Server.GetInstance())
			      .SetParameter("World", World.GetInstance());
		}
	}
}


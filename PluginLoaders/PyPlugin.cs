using Pluton.Rust.Objects;

namespace Pluton.Rust.PluginLoaders
{
	public class PYPlugin : Core.PluginLoaders.PYPlugin
	{
		public PYPlugin(string name)
			: base(name)
		{
		}

		public override void AssignVariables()
		{
			Scope.SetVariable("Commands", new ChatCommands(this));
			Scope.SetVariable("ServerConsoleCommands", new ConsoleCommands(this));
			Scope.SetVariable("Find", Find.GetInstance());
			Scope.SetVariable("Server", Server.GetInstance());
			Scope.SetVariable("World", World.GetInstance());
		}
	}
}


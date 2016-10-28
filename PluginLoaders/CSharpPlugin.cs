namespace Pluton.Rust.PluginLoaders
{
    using Objects;

    public class CSharpPlugin : Core.PluginLoaders.CSharpPlugin
    {
        ConsoleCommands _ServerConsoleCommands;

        ChatCommands _Commands;

        public ConsoleCommands ServerConsoleCommands {
            get {
                if (_ServerConsoleCommands == null)
                    _ServerConsoleCommands = new ConsoleCommands(Plugin);
                return _ServerConsoleCommands;
            }
        }

        public ChatCommands Commands {
            get {
                if (_Commands == null)
                    _Commands = new ChatCommands(Plugin);
                return _Commands;
            }
        }

        public Server Server => Server.GetInstance();

        public World World => World.GetInstance();

        public Find Find => Find.GetInstance();
    }
}


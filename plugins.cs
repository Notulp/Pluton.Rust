namespace Pluton.Rust {
	using System;
	using Core.PluginLoaders;
	using System.Collections.Generic;
	using System.Linq;

	[ConsoleSystem.Factory("plugins")]
	public class plugins : ConsoleSystem {
		public static string Loaded() {
			int count = PluginLoader.GetInstance().Plugins.Count;
			string result = String.Format("Loaded plugins({0}):" + Environment.NewLine, count);

			foreach (BasePlugin plugin in PluginLoader.GetInstance().Plugins.Values) {
				result += String.Format("    {0, -22} [{1, -12} timers: {2, 8}, parallel: {3, 8}\r\n",
				                        plugin.Name,
				                        plugin.GetType().Name + "],",
				                        plugin.Timers.Count + plugin.ParallelTimers.Count,
				                        plugin.ParallelTimers.Count);
				result += String.Join(", ", new [] {
					String.IsNullOrEmpty(plugin.Author) ? String.Empty : "Author: " + plugin.Author,
					String.IsNullOrEmpty(plugin.About) ? String.Empty : "About: " + plugin.About,
					String.IsNullOrEmpty(plugin.Version) ? String.Empty : "Version: " + plugin.Version
				}.Where(res => !String.IsNullOrEmpty(res)).ToArray()) + "\r\n";
			}

			return result;
		}

		[ConsoleSystem.Admin, ConsoleSystem.Help("Prints out plugin statistics!")]
		public static void Loaded(ConsoleSystem.Arg args) {
			args.ReplyWith(Loaded());
		}

		public static string Hooks() {
			var hooks = new Dictionary<string, List<string>>();

			PluginLoader.GetInstance().Plugins.Values.ToList().ForEach(
				p => p.Globals.ToList().ForEach(
					g => {
						if (g.StartsWith("On_"))
							AddPluginToHookListInDict(hooks, g, p.Name);
					}));

			string result = "The registered hooks are:" + Environment.NewLine;

			hooks.Keys.ToList().ForEach(k => {
				result += k + ": " + String.Join(", ", hooks[k].ToArray()) + Environment.NewLine;
			});

			return result;
		}

		[ConsoleSystem.Admin, ConsoleSystem.Help("Prints out hooks statistics!")]
		public static void Hooks(ConsoleSystem.Arg args) {
			args.ReplyWith(Hooks());
		}

		static void AddPluginToHookListInDict(Dictionary<string, List<string>> hooks, string key, string value) {
			if (hooks.ContainsKey(key))
				hooks[key].Add(value);
			else
				hooks.Add(key, new List<string>() { value });
		}
	}
}

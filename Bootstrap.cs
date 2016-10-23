namespace Pluton.Rust {
	using System;
	using System.IO;
	using System.Collections.Generic;
	using Core;
	using Core.PluginLoaders;

	public class Bootstrap : Singleton<Bootstrap>, ISingleton {
		public static string Version => typeof(Bootstrap).Assembly.GetName().Version.ToString();

		public void Initialize() {
			Console.WriteLine($"Loading Pluton.Rust, v.{Version}");

			System.Reflection.Assembly assembly = GetType().Assembly;
			Type[] types = assembly.GetTypes();

			for (int i = 0; i < types.Length; i++) {
				object[] customAttributes = types[i].GetCustomAttributes(typeof(ConsoleSystem.Factory), false);
				if (customAttributes != null && customAttributes.Length != 0) {
                    ConsoleSystem.Factory factory = customAttributes[0] as ConsoleSystem.Factory;
				    Type indexType = typeof(ConsoleSystem.Index);

                    indexType.CallStaticMethod("BuildFields", types[i], factory);
                    indexType.CallStaticMethod("BuildProperties", types[i], factory);
                    indexType.CallStaticMethod("BuildFunctions", types[i], factory);
				}
			}

			Singleton<Core.Util>.SetInstance<Util>();

			Singleton<PluginLoader>.GetInstance();

			Singleton<Hooks>.GetInstance().Initialize();

			PluginLoaderHelper.RegisterPluginType<CSSPlugin>(".cs", "csscript");
			PluginLoaderHelper.RegisterPluginType<CSPlugin>(".dll", "csharp");
			PluginLoaderHelper.RegisterPluginType<PluginLoaders.JSPlugin>(".js", "javascript");
			PluginLoaderHelper.RegisterPluginType<PluginLoaders.LUAPlugin>(".lua", "lua");
			PluginLoaderHelper.RegisterPluginType<PluginLoaders.PYPlugin>(".py", "python");

			PluginLoader<CSSPlugin>.dependencies = new List<Func<bool>>() {
				() => CoreConfig.GetInstance().GetBoolValue("csscript", "enabled"),
				() => File.Exists(Path.Combine(Core.Util.GetInstance().GetManagedFolder(), "mcs.exe"))
			};

			PluginLoader<CSPlugin>.dependencies = new List<Func<bool>>() {
				() => CoreConfig.GetInstance().GetBoolValue("csharp", "enabled")
			};

			PluginLoader<PluginLoaders.JSPlugin>.dependencies = new List<Func<bool>>() {
				() => CoreConfig.GetInstance().GetBoolValue("javascript", "enabled"),
				() => File.Exists(Path.Combine(Core.Util.GetInstance().GetManagedFolder(), "Jint.dll"))
			};

			PluginLoader<PluginLoaders.LUAPlugin>.dependencies = new List<Func<bool>>() {
				() => CoreConfig.GetInstance().GetBoolValue("lua", "enabled"),
				() => File.Exists(Path.Combine(Core.Util.GetInstance().GetManagedFolder(), "MoonSharp.Interpreter.dll"))
			};

			PluginLoader<PluginLoaders.PYPlugin>.dependencies = new List<Func<bool>>() {
				() => CoreConfig.GetInstance().GetBoolValue("python", "enabled"),
				() => File.Exists(Path.Combine(Core.Util.GetInstance().GetManagedFolder(), "IronPython.Deps.dll"))
			};

			Console.WriteLine($"[v.{Version}] Pluton.Rust loaded!");
		}
	}
}

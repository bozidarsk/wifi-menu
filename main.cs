using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;

public static class Program 
{
	public static bool Connect(string ssid, string password) 
	{
		int code;
		if (password != null) 
		{
			Console.WriteLine("Connect to '" + ssid + "' with '" + password + "'.");
			StartProccess(
				Environment.GetEnvironmentVariable("SHELL"),
				"-c \"" + Config.ConnectSecureCommand.Replace("%ssid%", ssid).Replace("%password%", password) + "\"",
				out code
			);
		}
		else 
		{
			Console.WriteLine("Connect to '" + ssid + "'.");
			StartProccess(
				Environment.GetEnvironmentVariable("SHELL"),
				"-c \"" + Config.ConnectOpenCommand.Replace("%ssid%", ssid) + "\"",
				out code
			);
		}

		return code == 0;
	}

	public static void Disconnect() 
	{
		Console.WriteLine("Disconnect.");
		StartProccess(
			Environment.GetEnvironmentVariable("SHELL"),
			"-c \"" + Config.DisconnectCommand + "\"",
			out int code
		);
	}

	public static void Forget(string ssid) 
	{
		Console.WriteLine("Forget '" + ssid + "'.");
		StartProccess(
			Environment.GetEnvironmentVariable("SHELL"),
			"-c \"" + Config.ForgetCommand.Replace("%ssid%", ssid) + "\"",
			out int code
		);
	}

	private static bool IsWireless(string dev) 
	{
		StartProccess("ls", "/sys/class/net/" + dev + "/wireless", out int code);
		return code == 0;
	}

	private static string GetCurrentInterface() 
	{
		string dev = StartProccess(
			Environment.GetEnvironmentVariable("SHELL"),
			"-c \"ip route get 8.8.8.8 2> /dev/null | grep dev | sed -E 's/[^ ]+ via [^ ]+ dev ([^ ]+).+/\\1/' | tr -d '\\n'\"",
			out int code
		);
		return !string.IsNullOrEmpty(dev) ? dev : null;
	}

	private static bool UsingEthernet() 
	{
		string dev = GetCurrentInterface();
		return dev != null ? !IsWireless(dev) : false;
	}

	private static NetworkInfo[] GetAllNetworks() 
	{
		NetworkInfo[] networks = StartProccess(
			Environment.GetEnvironmentVariable("SHELL"),
			"-c \"" + Config.AllNetworksCommand + "\"",
			out int code
		)
		.Split('\n')
		.Where(x => !string.IsNullOrEmpty(x))
		.Select(x => new NetworkInfo(x.Split('\x1d')))
		.ToArray()
		;

		string[] knowns = GetKnownNetworks();
		for (int i = 0; i < networks.Length; i++) { if (Array.IndexOf(knowns, networks[i].SSID) >= 0) { networks[i].IsKnown = true; } }

		return networks;
	}

	private static string[] GetKnownNetworks() => 
		StartProccess(
			Environment.GetEnvironmentVariable("SHELL"),
			"-c \"" + Config.KnownNetworksCommand + "\"",
			out int code
		)
		.Split('\n')
		.Where(x => !string.IsNullOrEmpty(x))
		.ToArray()
	;

	private static string StartProccess(string name, string args, out int code) 
	{
		ProcessStartInfo info = new ProcessStartInfo();
		info.FileName = name;
		info.Arguments = args;
		info.UseShellExecute = false;
		info.RedirectStandardOutput = true;
		info.RedirectStandardError = false;

		Process proc = new Process();
		proc.StartInfo = info;
		proc.Start();
		proc.WaitForExit();
		code = proc.ExitCode;

		return proc.StandardOutput.ReadToEnd();
	}

	private static int Main(string[] args) 
	{
		if (args.Length == 0 || (args.Length == 1 && (args[0] == "-h" || args[0] == "--help" || args[0] == "help")))
		{
			Console.WriteLine("Usage:\n\twifi-menu <command> [arguments...] [options]");
			Console.WriteLine("\nCommands:");
			Console.WriteLine("\twindow                     Opens a gtk3 gui window.");
			Console.WriteLine("\tconnect <ssid> <password>  Connects to a network with a password. (if the network is open pass \"\" as password)");
			Console.WriteLine("\tdisconnect                 Disconnects from any network.");
			Console.WriteLine("\tforget <ssid>              Forgets a network.");
			Console.WriteLine("\tdefaults                   Creates default configuration and style files.");
			Console.WriteLine("\nOptions:");
			Console.WriteLine("\t--config-dir <dir>         Directory which contains configuration and style files.");
			return 0;
		}

		Config.Initialize(ref args);

		Func<int> useEthernet = () => { Console.WriteLine("You are using a wired connection."); return 1; };
		Func<int> invalidArguments = () => { Console.WriteLine("Invalid arguments."); return 1; };
		Func<int> wrongPassword = () => { Console.WriteLine("Wrong password."); return 2; };

		if (args.Length == 0) { Console.WriteLine("No commands provided."); return 1; }
		switch (args[0]) 
		{
			case "window":
				Window.Run(!UsingEthernet() ? GetAllNetworks() : null);
				break;
			case "connect":
				if (args.Length != 3) { return invalidArguments(); }
				if (UsingEthernet()) { return useEthernet(); }
				if (!Connect(args[1], !string.IsNullOrEmpty(args[2]) ? args[2] : null)) { return wrongPassword(); }
				break;
			case "disconnect":
				if (UsingEthernet()) { return useEthernet(); }
				Disconnect();
				break;
			case "forget":
				if (args.Length != 2) { return invalidArguments(); }
				if (UsingEthernet()) { return useEthernet(); }
				Forget(args[1]);
				break;
			case "defaults":
				Config.CreateDefaults();
				break;
			default:
				Console.WriteLine("Unrecognized command.");
				return 1;
		}

		return 0;
	}
}
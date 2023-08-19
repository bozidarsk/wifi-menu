using System;

public static partial class Config 
{
	public static int WindowWidth { private set; get; } = 400;
	public static int WindowHeight { private set; get; } = 450;
	public static int WindowMarginTop { private set; get; } = 10;
	public static int WindowMarginRight { private set; get; } = 10;
	public static int WindowMarginBottom { private set; get; } = 0;
	public static int WindowMarginLeft { private set; get; } = 0;
	public static Gtk.LayerShell.Edge[] WindowAnchor { private set; get; } = { Gtk.LayerShell.Edge.Top, Gtk.LayerShell.Edge.Right };
	public static int EntryHeight { private set; get; } = 48;
	public static int EntrySpacing { private set; get; } = 5;
	public static string[] StrengthOpen { private set; get; } = { "󰤯","󰤟","󰤢","󰤥","󰤨" };
	public static string[] StrengthSecure { private set; get; } = { "󰤬","󱛋","󱛌","󱛍","󰤪" };
	public static int MaxStrength { private set; get; } = 4;
	public static int StrengthSpacing { private set; get; } = 43;
	public static string ConnectedString { private set; get; } = ">";
	/* output of AllNetworksCommand must be 'SSID\x1dSECURITY\x1dSTRENGTH\x1dCONNECTEDSTRING\n...' (if CONNECTEDSTRING == Config.ConnectedString you are currently connected to this network, otherwise CONNECTEDSTRING == '') */
	public static string AllNetworksCommand { private set; get; } = @"iwctl station wlan0 get-networks | grep \* | tr -d '\33' | sed -E 's/(\[0m)?\s*(\[1;90m(>) \[0m)?\s*(.+)/\4 \3/' | rev | sed -E 's/(>)?\s+(m0\[\*+m09;1\[)?(\*+)\s+([^ ]+)\s+(.+)/\1\x1d\3\x1d\4\x1d\5/' | rev";
	/* output of KnownNetworksCommand must be 'SSID\n...' */
	public static string KnownNetworksCommand { private set; get; } = @"iwctl known-networks list | grep , | tr -d '\33' | sed -E 's/(\[0m)?\s*(.+)/\2/' | rev | sed -E 's/\s*MA?P? [0-9]+:[0-9]+\s+,[0-9]+ [a-zA-Z]+\s+[^ ]+\s+(.+)/\1/' | rev";
	public static string ConnectSecureCommand { private set; get; } = @"iwctl --passphrase '%password%' station wlan0 connect '%ssid%'";
	public static string ConnectOpenCommand { private set; get; } = @"iwctl station wlan0 connect '%ssid%'";
	public static string DisconnectCommand { private set; get; } = @"iwctl station wlan0 disconnect";
	public static string ForgetCommand { private set; get; } = @"iwctl known-networks '%ssid%' forget";

	public static string Css { private set; get; } = "* {\n    font-family: \"Source Code Pro Bold\";\n    font-weight: bold;\n    transition: 0.2s;\n}\n\nwindow {\n    border: 2px solid @theme_selected_bg_color;\n    border-radius: 8px;\n}\n\n#box {\n	margin: 10px;\n}\n\n#entry, #entry-selected {\n	border-radius: 8px;\n}\n\n#entry:focus, #entry-selected {\n    background-color: #38414e;\n}\n\n#entry-connected, #entry-selected-connected {\n	border: 2px solid @theme_selected_bg_color;\n	border-radius: 8px;\n}\n\n#entry-connected:focus, #entry-selected-connected {\n	border: 2px solid @theme_selected_bg_color;\n    background-color: #38414e;\n}\n\n#info {\n	margin-top: 15px;\n}\n\n#info #ssid {\n	margin-left: 10px;\n}\n\n#info #strength {\n}\n\n#menu {\n}\n\n#menu #input {\n	margin: 15px 2px -10px 2px;\n	font-weight: normal;\n}\n\n#menu #connect {\n	border-radius: 8px;\n	margin: 15px 2px 2px 2px;\n}\n\n#menu #connect:focus {\n	box-shadow: none;\n}\n\n#menu #forget {\n	border-radius: 8px;\n	margin: 3px 2px 2px 2px;\n}\n\n#menu #forget:focus {\n	box-shadow: none;\n}\n\n#menu #output {\n	margin: 7px 0px 5px 7px;\n	color: yellow;\n	font-size: 12px;\n}\n\n#wired-icon {\n	margin-top: 160px;\n	font-family: \"Font Awesome 6 Sharp\";\n	font-size: 35px;\n}\n\n#wired-text {\n	margin-top: 15px;\n	font-size: 20px;\n}";
	public static string ConfigDir { private set; get; } = Environment.GetEnvironmentVariable("HOME") + "/.config/wifi-menu";

	private static readonly Option[] OptionsDefinition =
	{
		new Option("--config-dir", 'c', true, null),
	};
}
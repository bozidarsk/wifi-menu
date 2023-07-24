using System;

public static partial class Config 
{
	public static int WindowWidth { private set; get; }
	public static int WindowHeight { private set; get; }
	public static int WindowMarginTop { private set; get; }
	public static int WindowMarginRight { private set; get; }
	public static int WindowMarginBottom { private set; get; }
	public static int WindowMarginLeft { private set; get; }
	public static Gtk.LayerShell.Edge[] WindowAnchor { private set; get; }
	public static int EntryHeight { private set; get; }
	public static int EntrySpacing { private set; get; }
	public static string[] StrengthOpen { private set; get; }
	public static string[] StrengthSecure { private set; get; }
	public static int MaxStrength { private set; get; }
	public static int StrengthSpacing { private set; get; }
	public static string ConnectedString { private set; get; }
	public static string AllNetworksCommand { private set; get; }
	public static string KnownNetworksCommand { private set; get; }
	public static string ConnectSecureCommand { private set; get; }
	public static string ConnectOpenCommand { private set; get; }
	public static string DisconnectCommand { private set; get; }
	public static string ForgetCommand { private set; get; }
	public static string Css { private set; get; }

	private static readonly string DefaultConfig = "# format is 'NAME=VALUE'\n# each space is treated as part of NAME or VALUE depending on its position\n# lines that start with '#' are comments and are skipped\n# if a line starts with '!' environment variable expansion will not be used\n# each environment variable (starting with '$') will be expanded to its corresponding value\n# for directories VALUE must end with '/'\n# arrays are defined as:\n# 'NAME[]=ITEM0,ITEM1,ITEM2' (supports ony 1d arrays; again be carefull of spaces)\n\n# output of AllNetworksCommand must be 'SSID\\x1dSECURITY\\x1dSTRENGTH\\x1dCONNECTEDSTRING\\n...' (if CONNECTEDSTRING == Config.ConnectedString you are currently connected to this network, otherwise CONNECTEDSTRING == '')\n# output of KnownNetworksCommand must be 'SSID\\n...'\n\nWindowWidth=400\nWindowHeight=450\nWindowMarginTop=10\nWindowMarginRight=10\nWindowMarginBottom=0\nWindowMarginLeft=0\nWindowAnchor[]=Top,Right\nEntryHeight=48\nEntrySpacing=5\nStrengthOpen[]=󰤯,󰤟,󰤢,󰤥,󰤨\nStrengthSecure[]=󰤬,󱛋,󱛌,󱛍,󰤪\nMaxStrength=4\nStrengthSpacing=43\nConnectedString=>\nAllNetworksCommand=iwctl station wlan0 get-networks | grep \\* | tr -d '\\33' | sed -E 's/(\\[0m)?\\s*(\\[1;90m(>) \\[0m)?\\s*(.+)/\\4 \\3/' | rev | sed -E 's/(>)?\\s+(m0\\[\\*+m09;1\\[)?(\\*+)\\s+([^ ]+)\\s+(.+)/\\1\\x1d\\3\\x1d\\4\\x1d\\5/' | rev\nKnownNetworksCommand=iwctl known-networks list | grep , | tr -d '\\33' | sed -E 's/(\\[0m)?\\s*(.+)/\\2/' | rev | sed -E 's/\\s*MA?P? [0-9]+:[0-9]+\\s+,[0-9]+ [a-zA-Z]+\\s+[^ ]+\\s+(.+)/\\1/' | rev\nConnectSecureCommand=iwctl --passphrase '%password%' station wlan0 connect '%ssid%'\nConnectOpenCommand=iwctl station wlan0 connect '%ssid%'\nDisconnectCommand=iwctl station wlan0 disconnect\nForgetCommand=iwctl known-networks '%ssid%' forget";
	private static readonly string DefaultCss = "* {\n    font-family: \"Source Code Pro Bold\";\n    font-weight: bold;\n    transition: 0.2s;\n}\n\nwindow {\n    border: 2px solid @theme_selected_bg_color;\n    border-radius: 8px;\n}\n\n#box {\n	margin: 10px;\n}\n\n#entry, #entry-selected {\n	border-radius: 8px;\n}\n\n#entry:focus, #entry-selected {\n    background-color: #38414e;\n}\n\n#entry-connected, #entry-selected-connected {\n	border: 2px solid @theme_selected_bg_color;\n	border-radius: 8px;\n}\n\n#entry-connected:focus, #entry-selected-connected {\n	border: 2px solid @theme_selected_bg_color;\n    background-color: #38414e;\n}\n\n#info {\n	margin-top: 15px;\n}\n\n#info #ssid {\n	margin-left: 10px;\n}\n\n#info #strength {\n}\n\n#menu {\n}\n\n#menu #input {\n	margin: 15px 2px -10px 2px;\n	font-weight: normal;\n}\n\n#menu #connect {\n	border-radius: 8px;\n	margin: 15px 2px 2px 2px;\n}\n\n#menu #connect:focus {\n	box-shadow: none;\n}\n\n#menu #forget {\n	border-radius: 8px;\n	margin: 3px 2px 2px 2px;\n}\n\n#menu #forget:focus {\n	box-shadow: none;\n}\n\n#menu #output {\n	margin: 7px 0px 5px 7px;\n	color: yellow;\n	font-size: 12px;\n}\n\n#wired-icon {\n	margin-top: 160px;\n	font-family: \"Font Awesome 6 Sharp\";\n	font-size: 35px;\n}\n\n#wired-text {\n	margin-top: 15px;\n	font-size: 20px;\n}";
	private static readonly string DefaultConfigDir = Environment.GetEnvironmentVariable("HOME") + "/.config/wifi-menu/";
}
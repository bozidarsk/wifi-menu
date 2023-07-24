using System;
using System.Linq;
using Gtk;

public static class Window 
{
	public static event System.Action OnLoad;

	public static void Run(NetworkInfo[] networks) 
	{
		Application.Init();

		CssProvider provider = new CssProvider();
		provider.LoadFromData(Config.Css);
		StyleContext.AddProviderForScreen(Gdk.Screen.Default, provider, 800);

		VBox box = new VBox();
		box.Name = "box";

		ScrolledWindow scroll = new ScrolledWindow();
		scroll.Add(box);

		Gtk.Window window = new Gtk.Window("wifi-menu");
		LayerShell.InitWindow(window);
		LayerShell.SetLayer(window, LayerShell.Layer.Overlay);
		LayerShell.SetKeyboardInteractivity(window, true);
		LayerShell.SetKeyboardMode(window, LayerShell.KeyboardMode.OnDemand);
		LayerShell.SetMargin(window, LayerShell.Edge.Top, Config.WindowMarginTop);
		LayerShell.SetMargin(window, LayerShell.Edge.Right, Config.WindowMarginRight);
		LayerShell.SetMargin(window, LayerShell.Edge.Bottom, Config.WindowMarginBottom);
		LayerShell.SetMargin(window, LayerShell.Edge.Left, Config.WindowMarginLeft);
		for (int i = 0; Config.WindowAnchor != null && i < Config.WindowAnchor.Length; i++) { LayerShell.SetAnchor(window, Enum.Parse<LayerShell.Edge>(Config.WindowAnchor[i]), true); }

		if (networks != null) { for (int i = 0; i < networks.Length; i++) { box.PackStart(new Entry(networks[i]).Widget, false, false, 0); } }
		else 
		{
			Label wiredIcon = new Label("");
			wiredIcon.Name = "wired-icon";

			Label wiredText = new Label("You are using a wired connection.");
			wiredText.Name = "wired-text";

			box.PackStart(wiredIcon, false, false, 0);
			box.PackStart(wiredText, false, false, 0);

			window.KeyPressEvent += (object sender, KeyPressEventArgs e) => Application.Quit();
			window.ButtonPressEvent += (object sender, ButtonPressEventArgs e) => Application.Quit();
		}

		window.Resizable = false;
		window.KeepAbove = true;
		window.FocusOutEvent += (object sender, FocusOutEventArgs e) => Application.Quit();
		window.SetDefaultSize(Config.WindowWidth, Config.WindowHeight);
		window.Add(scroll);
		window.ShowAll();

		if (Window.OnLoad != null) { Window.OnLoad(); }

		Application.Run();
	}
}
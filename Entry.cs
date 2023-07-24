using System;
using System.Linq;
using Gtk;

/*

**************** EventBox (entry) ********************************
*                                                                *
*  *********** VBox (entry-box) *******************************  *
*  *                                                          *  *
*  *  *********** HBox (info) ******************************  *  *
*  *  *                                                    *  *  *
*  *  * Label (ssid)               Label (strength) [icon] *  *  *
*  *  *                                                    *  *  *
*  *  ******************************************************  *  *
*  *                                                          *  *
*  *  *********** VBox (menu) [hidden] *********************  *  *
*  *  *                                                    *  *  *
*  *  * Entry (input) [hidden]                             *  *  *
*  *  * Button (connect)                                   *  *  *
*  *  * Button (forget)                                    *  *  *
*  *  * Label (output) [hidden]                            *  *  *
*  *  *                                                    *  *  *
*  *  ******************************************************  *  *
*  *                                                          *  *
*  ************************************************************  *
*                                                                *
******************************************************************

*/


public sealed class Entry 
{
	public NetworkInfo Network { private set; get; }
	public EventBox Widget { private set; get; }

	private bool isMenuOpen = false;
	private VBox menu;

	private static event System.Action HideAllMenus;
	private static event System.Action DisconnectAll;

	private void HideMenu() 
	{
		isMenuOpen = false;
		menu.Children[0].Hide();
		menu.Children[3].Hide();
		if (!this.Network.IsKnown) { menu.Children[2].Hide(); }
		this.Widget.Name = "entry" + (this.Widget.Name.Contains("-connected") ? "-connected" : "");
		menu.Hide();
	}

	private void ShowMenu() 
	{
		isMenuOpen = true;
		this.Widget.Name = "entry-selected" + (this.Widget.Name.Contains("-connected") ? "-connected" : "");
		menu.Show();
	}

	private void Disconnect() 
	{
		((Button)menu.Children[1]).Label = "Connect";
		this.Network.IsConnected = false;
		this.Widget.Name = this.Widget.Name.Replace("-connected", "");
		this.Widget.GrabFocus();
		ShowMenu();
	}

	private void HandleConnection() 
	{
		if (this.Network.IsConnected) 
		{
			Disconnect();
			Program.Disconnect();
			return;
		}

		if (string.IsNullOrEmpty(((Gtk.Entry)menu.Children[0]).Text) && !this.Network.IsKnown) { return; }
		if (Program.Connect(this.Network.SSID, this.Network.Security != "open" && !this.Network.IsKnown ? ((Gtk.Entry)menu.Children[0]).Text : null)) 
		{
			DisconnectAll();

			((Button)menu.Children[1]).Label = "Disconnect";
			menu.Children[0].Hide();
			menu.Children[2].Show();
			menu.Children[3].Hide();
			this.Widget.Name += "-connected";
			this.Network.IsConnected = true;
			this.Network.IsKnown = true;

			this.Widget.GrabFocus();
			ShowMenu();
			((Gtk.Entry)menu.Children[0]).Text = "";

			return;
		}
		else 
		{
			menu.Children[3].Show();
			((Gtk.Entry)menu.Children[0]).Text = "";
		}
	}

	public Entry(NetworkInfo network) 
	{
		this.Network = network;
		this.Widget = new EventBox();

		this.Widget.CanFocus = true;
		this.Widget.HeightRequest = Config.EntryHeight;
		this.Widget.Name = "entry" + (this.Network.IsConnected ? "-connected" : "");
		this.Widget.MarginBottom = Config.EntrySpacing;

		// info
		HBox info = new HBox(false, 0);

		Label ssidLabel = new Label(this.Network.SSID);
		ssidLabel.SetAlignment(0f, 2f);
		ssidLabel.WidthChars = Config.StrengthSpacing;
		ssidLabel.Name = "ssid";

		string[] icons = this.Network.Security == "open" ? Config.StrengthOpen : Config.StrengthSecure;
		Label strengthLabel = new Label(icons[(int)((float)(icons.Length - 1) * this.Network.Strength)]);
		strengthLabel.Name = "strength";

		info.PackStart(ssidLabel, false, false, 0);
		info.PackStart(strengthLabel, false, false, 0);
		info.Name = "info";
		// info

		// menu
		menu = new VBox(false, 0);
		menu.Name = "menu";

		Gtk.Entry input = new Gtk.Entry();
		input.Name = "input";
		input.Visibility = false;
		input.SetIconActivatable(EntryIconPosition.Secondary, true);
		input.SetIconFromIconName(EntryIconPosition.Secondary, "layer-visible");
		input.IconPress += (object sender, IconPressArgs e) => 
		{
			bool visible = input.GetIconName(EntryIconPosition.Secondary) == "layer-not-visible";
			input.SetIconFromIconName(EntryIconPosition.Secondary, visible ? "layer-visible" : "layer-not-visible");
			input.Visibility = !visible;
		};
		input.Activated += (object sender, EventArgs e) => 
		{
			if (string.IsNullOrEmpty(input.Text)) { return; }
			this.HandleConnection();
		};

		Button connect = new Button(this.Network.IsConnected ? "Disconnect" : "Connect");
		connect.Name = "connect";
		connect.Clicked += (object sender, EventArgs e) => 
		{
			if (input.Visible || this.Network.IsConnected || this.Network.IsKnown) 
			{
				this.HandleConnection();
				return;
			}

			input.Show();
			input.GrabFocus();
		};

		Button forget = new Button("Forget");
		forget.Name = "forget";
		forget.Clicked += (object sender, EventArgs e) => 
		{
			this.Network.IsKnown = false;
			Disconnect();
			forget.Hide();
			Program.Forget(this.Network.SSID);
		};

		Label output = new Label("Wrong password.");
		output.SetAlignment(0f, 2f);
		output.Name = "output";

		menu.PackStart(input, true, false, 0);
		menu.PackStart(connect, true, false, 0);
		menu.PackStart(forget, true, false, 0);
		menu.PackStart(output, true, false, 0);
		// menu

		// box
		VBox box = new VBox(false, 0);
		box.PackStart(info, false, false, 0);
		box.PackStart(menu, false, false, 0);
		box.Name = "entry-box";
		// box

		this.Widget.Add(box);

		Window.OnLoad += () => HideMenu();
		HideAllMenus += () => HideMenu();
		DisconnectAll += () => Disconnect();

		this.Widget.KeyPressEvent += (object sender, KeyPressEventArgs e) => 
		{
			switch (e.Event.Key) 
			{
				case Gdk.Key.Escape:
					if (isMenuOpen) { HideMenu(); this.Widget.GrabFocus(); }
					else { Application.Quit(); }
					break;
				case Gdk.Key.Menu:
				case Gdk.Key.Return:
				case Gdk.Key.KP_Enter:
					this.Widget.GrabFocus();
					if (this.Network.IsConnected && isMenuOpen) { Disconnect(); Program.Disconnect(); }
					else if (!isMenuOpen) { ShowMenu(); }
					else if (this.Network.IsKnown) { HandleConnection(); }
					else { input.Show(); input.GrabFocus(); }
					break;
			}
		};

		this.Widget.ButtonPressEvent += (object sender, ButtonPressEventArgs e) => 
		{
			switch (e.Event.Button) 
			{
				case 1:
					this.Widget.GrabFocus();
					bool isMenuOpenNew = !isMenuOpen;
					HideAllMenus();
					if (isMenuOpenNew) { ShowMenu(); }
					else { HideMenu(); }
					break;
			}
		};

		this.Widget.FocusInEvent += (object sender, FocusInEventArgs e) => HideAllMenus();
	}
}
using System;

public sealed class NetworkInfo 
{
	public string SSID;
	public string Security;
	public float Strength;
	public bool IsConnected;
	public bool IsKnown;

	public NetworkInfo(string[] groups) 
	{
		this.SSID = groups[0];
		this.Security = groups[1];
		this.Strength = (float)groups[2].Length / (float)Config.MaxStrength;
		this.IsConnected = groups[3] == Config.ConnectedString;
	}
}
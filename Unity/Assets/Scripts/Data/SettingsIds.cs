using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SettingsIds
{
	static public readonly string showDebugObjectsSettingId = "showDebugObjects";
	static public readonly string versionNumber = "versionNumber";

	public static readonly Dictionary<string, string> defaults = new Dictionary<string, string>( )
	{
		{  SettingsIds.showDebugObjectsSettingId, "0" }
	};

	public static readonly List<string> encrypted = new List<string>( )
	{
	};
}

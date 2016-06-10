using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class SettingsIds
{
	static public readonly string showControlForceMarkerSettingId = "showControlForceMarker";
	static public readonly string showDebugObjectsSettingId = "showDebugObjects";
	static public readonly string showFPSId = "showFPS";
	static public readonly string versionNumber = "versionNumber";
	static public readonly string controlForceMultiplierSettingId = "controlForceMultiplier";
	static public readonly string playerSpeedMultiplierSettingId = "playerSpeedMultiplier";

	public static readonly Dictionary<string, string> defaults = new Dictionary<string, string>( )
	{
		{  showDebugObjectsSettingId, "0" },
        {  showFPSId, "0" },
        {  showControlForceMarkerSettingId, "1"  },
		{ controlForceMultiplierSettingId, "2" },
        { playerSpeedMultiplierSettingId, "4" }
	};

	public static readonly List<string> encrypted = new List<string>( )
	{
	};
}

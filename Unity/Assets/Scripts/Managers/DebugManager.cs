using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	static public class DebugManager
	{
		static private int debugObjectsLayer_ = 0;
		static public int debugObjectsLayer
		{
			get
			{
				if (debugObjectsLayer_ == 0)
				{
					debugObjectsLayer_ = 1 << LayerMask.NameToLayer( "DebugObjects" );
				}
				return debugObjectsLayer_;
			}
		}

		static public void ToggleDebugObjects( UnityEngine.Camera cam )
		{
			bool isShowing = (cam.cullingMask & debugObjectsLayer) != 0;
			SetShowDebugObjects(cam, !isShowing );
		}

		static public void SetShowDebugObjects(UnityEngine.Camera cam, bool show)
		{
			if (show)
			{
				cam.cullingMask = cam.cullingMask | debugObjectsLayer;
			}
			else
			{
				cam.cullingMask = cam.cullingMask & ~debugObjectsLayer;
			}

		}
	}

}

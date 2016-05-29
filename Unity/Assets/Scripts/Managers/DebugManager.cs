using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class DebugManager : RJWard.Core.Singleton.SingletonApplicationLifetimeLazy< DebugManager >
	{
		private int debugObjectsLayer_ = 0;

		protected override void PostAwake( )
		{
			debugObjectsLayer_ = 1 << LayerMask.NameToLayer( "DebugObjects" );
		}

		public void ToggleDebugObjects( UnityEngine.Camera cam )
		{
			bool isShowing = (cam.cullingMask & debugObjectsLayer_) != 0;
			SetShowDebugObjects(cam, !isShowing );
		}

		public void SetShowDebugObjects(UnityEngine.Camera cam, bool show)
		{
			if (show)
			{
				cam.cullingMask = cam.cullingMask | debugObjectsLayer_;
			}
			else
			{
				cam.cullingMask = cam.cullingMask & ~debugObjectsLayer_;
			}

		}
	}

}

using UnityEngine;
using System.Collections;

public class TouchManager : RJWard.Core.Singleton.SingletonSceneLifetime<TouchManager>
{
	public static readonly bool DEBUG_TOUCHES = false;

	private int numTouches_ = 0;

	public System.Action< Touch[] > onTouches;

	private void Update()
	{
		int newNumTouches = Input.touchCount;
		if (newNumTouches != numTouches_)
		{
			if (DEBUG_TOUCHES)
			{
				Debug.Log( "Num touches changed from " + numTouches_ + " to " + newNumTouches );
			}
			numTouches_ = newNumTouches;
		}
		else
		{
			if (DEBUG_TOUCHES)
			{
				//			Debug.Log( "Still "+numTouches_+" touches");
			}
		}

		if (onTouches != null)
		{
			onTouches( Input.touches );
		}
	}
}

using UnityEngine;
using System.Collections;

public class TouchManager : RJWard.Core.Singleton.SingletonSceneLifetime<TouchManager>
{
	public static readonly bool DEBUG_TOUCHES = true;

	private int numTouches_ = 0;

	private void Update()
	{
		int newNumTouches = Input.touches.Length;
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
//			Debug.Log( "Still "+numTouches_+" touches");
		}
	}
}

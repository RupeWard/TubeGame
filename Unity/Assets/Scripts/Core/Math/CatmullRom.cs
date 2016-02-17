using UnityEngine;
using System.Collections;

namespace RJWard.Core
{
	public static class CatmullRom
	{
		static public Vector3 Interpolate(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			if (t < 0f || t > 1f)
			{
				Debug.LogError( "CatMullRom interpolator out of range at " + t );
				t = Mathf.Clamp01( t );
			}
			Vector3 a = 0.5f * (2f * p1);
			Vector3 b = 0.5f * (p2 - p0);
			Vector3 c = 0.5f * ( 2f * p0 - 5f * p1 + 4f * p2 - p3);
			Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

			return  a + (b * t) + (c * t * t) + (d * t * t * t);
		}
	}

}

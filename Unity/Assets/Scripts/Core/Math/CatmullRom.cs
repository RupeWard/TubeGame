using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Core
{
	// See http://stackoverflow.com/questions/9489736/catmull-rom-curve-with-no-cusps-and-no-self-intersections/19283471#19283471
	class CubicPoly
	{
		private float c0_, c1_, c2_, c3_;

		public CubicPoly(float c0, float c1, float c2, float c3)
		{
			c0_ = c0; c1_ = c1; c2_ = c2; c3_ = c3;
		}

		protected CubicPoly( ) { }

		public float eval(float t)
		{
			float t2 = t * t;
			float t3 = t * t2;
			return c0_ + c1_ * t + c2_ * t2 + c3_ * t3;
		}
	}

	class NonUniformCatMullRomPoly : CubicPoly
	{
		private NonUniformCatMullRomPoly( float x0, float x1, float t0, float t1 )
			: base( x0, t0, -3f * x0 + 3f*x1 - 2f*t0 - t1, 2f*x0 -2f*x1 + t0 + t1 )
		{ }

		static public NonUniformCatMullRomPoly Create(float x0, float x1, float x2, float x3, float dt0, float dt1, float dt2 )
		{
			float t1 = (x1 - x0) / dt0 - (x2 - x0) / (dt0 + dt1) + (x2 - x1) / dt1;
			float t2 = (x2 - x1) / dt1 - (x3 - x1) / (dt1 + dt2) + (x3 - x2) / dt2;

			t1 *= dt1;
			t2 *= dt1;

			return new NonUniformCatMullRomPoly( x1, x2, t1, t2 ); 
		}
	}

	class CatMullRom3D
	{
		NonUniformCatMullRomPoly xPoly_ = null;
		NonUniformCatMullRomPoly yPoly_ = null;
		NonUniformCatMullRomPoly zPoly_ = null;

		float catmullRomPower_ = 0f;

		static readonly float s_epsilon = 0.00001f;

		static public CatMullRom3D Create( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float catmullRomPower )
		{
			return new CatMullRom3D( p0, p1, p2, p3, catmullRomPower );
		}

		static public CatMullRom3D CreateCentripetal( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
		{
			return new CatMullRom3D( p0, p1, p2, p3, 0.5f);
		}

		private CatMullRom3D( Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float catmullRomPower)
		{	
			catmullRomPower_ = catmullRomPower;

			float powerToUse = catmullRomPower_ / 2f;
			float dt0 = Mathf.Pow( (p1 - p0).sqrMagnitude, powerToUse );
			float dt1 = Mathf.Pow( (p2 - p1).sqrMagnitude, powerToUse );
			float dt2 = Mathf.Pow( (p3 - p2).sqrMagnitude, powerToUse );

			if (dt1 < s_epsilon)
			{
				dt1 = 1f;
				Debug.LogWarning( "dt1 vanishing" );
			}
			if ( dt0 < s_epsilon)
			{
				dt0 = dt1;
				Debug.LogWarning( "dt0 vanishing" );
			}
			if (dt2 < s_epsilon)
			{
				dt2 = dt1;
				Debug.LogWarning( "dt2 vanishing" );
			}

			xPoly_ = NonUniformCatMullRomPoly.Create( p0.x, p1.x, p2.x, p3.x, dt0, dt1, dt2 );
			yPoly_ = NonUniformCatMullRomPoly.Create( p0.y, p1.y, p2.y, p3.y, dt0, dt1, dt2 );
			zPoly_ = NonUniformCatMullRomPoly.Create( p0.z, p1.z, p2.z, p3.z, dt0, dt1, dt2 );

		}

		public Vector3 Interpolate(float t)
		{
			if ( t < 0f || t > 1f)
			{
				throw new System.ArgumentOutOfRangeException( "CatmullRom interpolator must be on unit interval" );
			}
			return new Vector3( xPoly_.eval( t ), yPoly_.eval( t ), zPoly_.eval( t ) );
		}
	}

/*
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
}*/


}

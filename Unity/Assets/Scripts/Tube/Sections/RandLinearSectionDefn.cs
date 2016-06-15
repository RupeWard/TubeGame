using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	[System.Serializable]
	public class RandLinearSectionDefn : RJWard.Core.IDebugDescribable
	{
		public int numSections = 10;
		public float sectionSeparation = 3f;
		public int numHoopsPerSection = 4;

		public Vector2 xAngleChangeRange = new Vector2( -10f, 10f );
		public Vector2 yAngleChangeRange = new Vector2( -10f, 10f );
		public float initialRad = 1f;
		public Vector2 radRange = new Vector2( 0.5f, 3f );
		public float maxRadD = 0.5f;
//		public int numHoopPoints = 10;

		public static RandLinearSectionDefn Lerp(RandLinearSectionDefn a, RandLinearSectionDefn b, float fraction)
		{
			RandLinearSectionDefn result = new RandLinearSectionDefn( );
			result.numSections = Mathf.CeilToInt( Mathf.Lerp( (float)a.numSections, (float)b.numSections, fraction ) );
			result.sectionSeparation = Mathf.Lerp( a.sectionSeparation, b.sectionSeparation, fraction );
			result.numHoopsPerSection = Mathf.CeilToInt( Mathf.Lerp( (float)a.numHoopsPerSection, (float)b.numHoopsPerSection, fraction ) );
			result.xAngleChangeRange = new Vector2( 
				Mathf.Lerp(a.xAngleChangeRange.x, b.xAngleChangeRange.x, fraction),
				Mathf.Lerp( a.xAngleChangeRange.y, b.xAngleChangeRange.y, fraction )
                );
			result.yAngleChangeRange = new Vector2(
				Mathf.Lerp( a.yAngleChangeRange.x, b.yAngleChangeRange.x, fraction ),
				Mathf.Lerp( a.yAngleChangeRange.y, b.yAngleChangeRange.y, fraction )
				);
			result.radRange = new Vector2(
				Mathf.Lerp( a.radRange.x, b.radRange.x, fraction ),
				Mathf.Lerp( a.radRange.y, b.radRange.y, fraction )
				);
			result.maxRadD = Mathf.Lerp( a.maxRadD, b.maxRadD, fraction );
			return result;
		}

		public HoopDefinition_Explicit firstHoop = null;

		public RandLinearSectionDefn( )
		{

		}

        public RandLinearSectionDefn ( RandLinearSectionDefn other )
		{
			this.CopyValuesFrom( other );
		}

		public void CopyValuesFrom(RandLinearSectionDefn other)
		{
			numSections = other.numSections;
			sectionSeparation = other.sectionSeparation;
			numHoopsPerSection = other.numHoopsPerSection;
			xAngleChangeRange = other.xAngleChangeRange;
			yAngleChangeRange = other.yAngleChangeRange;
			initialRad = other.initialRad;
			radRange = other.radRange;
			maxRadD = other.maxRadD;

		}

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "[RLSD: n=" ).Append( numSections ).Append( "x" );
			sb.Append( " s=" ).Append( sectionSeparation );
			sb.Append( "]" );
		}
	}
}

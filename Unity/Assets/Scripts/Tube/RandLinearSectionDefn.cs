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
		public int numHoopPoints = 10;
		public HoopDefinition_Explicit firstHoop = null;

		public void DebugDescribe( System.Text.StringBuilder sb )
		{
			sb.Append( "[RLSD: n=" ).Append( numSections ).Append( "x" ).Append( numHoopPoints );
			sb.Append( " s=" ).Append( sectionSeparation );
			sb.Append( "]" );
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class HoopDefinition_Explicit : HoopDefinition_Base
	{
		private List<Vector3> positions_ = null;
		public HoopDefinition_Explicit( Vector3 p, Vector3? ro, int nh )
			: base( p, ro, nh )
		{
			positions_ = new List<Vector3>( nh );
		}

		public HoopDefinition_Explicit( Hoop hoop )
			: base ( hoop.transform.localPosition, hoop.transform.localRotation.eulerAngles, hoop.numPoints())
		{
			positions_ = new List<Vector3>( numHoopPoints );
			for (int i = 0; i < numHoopPoints; i++)
			{
				positions_[i] = hoop.GetHoopPoint( i ).transform.localPosition;
			}
		}

		private HoopDefinition_Explicit( )  { }

		protected override void DebugDescribeDetails(System.Text.StringBuilder sb)
		{
			sb.Append( "EXPL ");
			for (int i = 0; i < positions_.Count; i++)
			{
				sb.Append( positions_[i] );
			}
		}

		override public void AddToSpine( Spine sp, bool fixedRotation )
		{
			Debug.LogError( "Not implemented" );
			throw new System.InvalidOperationException( "not implememted" );
		}

	}
}


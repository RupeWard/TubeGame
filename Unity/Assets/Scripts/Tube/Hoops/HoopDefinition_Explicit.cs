using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class HoopDefinition_Explicit : HoopDefinition_Base
	{
		private Vector3[] positions_ = null;

		public float radius()
		{
			float sum = 0f;
			foreach( Vector3 v in positions_)
			{
				sum += v.magnitude;
			}
			return sum / positions_.Length;
		}

		private void InitialisePositions(int n)
		{
			positions_ = new Vector3[n];
		}

		public Vector3 GetHoopPointPosition(int n)
		{
			return positions_[ n ];
		}

		public HoopDefinition_Explicit( Vector3 p, Vector3? ro, int nh )
			: base( p, ro, nh )
		{
			InitialisePositions( nh );
		}

		public HoopDefinition_Explicit( Hoop hoop )
			: base ( hoop.transform.localPosition, hoop.transform.localRotation.eulerAngles, hoop.numPoints())
		{
			if (numHoopPoints == 0)
			{
				Debug.LogError( "HP = 0" );
			}
			else
			{
				InitialisePositions( numHoopPoints );
				for (int i = 0; i < numHoopPoints; i++)
				{
					Vector3 pos = hoop.GetHoopPoint( i ).transform.localPosition;
					if (i<0 || i >= positions_.Length)
					{
						Debug.LogError( "i=" + i + " pc=" + positions_.Length);
					}
					else
					{
						positions_[i] = pos;
					}
				}
			}
		}

		private HoopDefinition_Explicit( )  { } // hiding

		protected override void DebugDescribeDetails(System.Text.StringBuilder sb)
		{
			sb.Append( "EXPL ");
			for (int i = 0; i < positions_.Length; i++)
			{
				sb.Append( positions_[i] );
			}
		}

		override public void AddToSpine( Spine_Linear sp, bool fixedRotation )
		{
			sp.AddExplicitSpinePoint( this, fixedRotation );
		}
	}
}


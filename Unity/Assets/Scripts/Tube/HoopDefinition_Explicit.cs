using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class HoopDefinition_Explicit : HoopDefinition_Base
	{
		private List<Vector3> positions_ = new List<Vector3>();

		private void InitialisePositions(int n)
		{
			positions_.Clear();
			if (n==0)
			{
				Debug.LogError( "N=0" );
			}
			else
			{
				for (int i = 0; i < n; i++)
				{
						positions_.Add(new Vector3());
				}
			}
		}

		public Vector3 GetHoopPointPosition(int n)
		{
			return positions_[ n ];
		}
		public HoopDefinition_Explicit( Vector3 p, Vector3? ro, int nh )
			: base( p, ro, nh )
		{
			InitialisePositions( nh );
//			positions_ = new List<Vector3>( nh );
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
//				positions_ = new List<Vector3>( numHoopPoints );
				for (int i = 0; i < numHoopPoints; i++)
				{
					Vector3 pos = hoop.GetHoopPoint( i ).transform.localPosition;
					Debug.Log( "i=" + i + " pos = " + pos );
					if (i<0 || i >= positions_.Count)
					{
						Debug.LogError( "i=" + i + " pc=" + positions_.Count );
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
			for (int i = 0; i < positions_.Count; i++)
			{
				sb.Append( positions_[i] );
			}
		}

		override public void AddToSpine( Spine sp, bool fixedRotation )
		{
			sp.AddExplicitSpinePoint( this, fixedRotation );
		}
/*
		override public void AddToSpine( Spine sp, HoopDefinition_Explicit hde )
		{
			sp.AddExplicitSpinePoint( hde, false);

			Debug.LogError( "Not implemented" );
			throw new System.InvalidOperationException( "not implememted" );
		}
*/
	}
}


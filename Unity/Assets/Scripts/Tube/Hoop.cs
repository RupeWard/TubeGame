using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Hoop : MonoBehaviour
	{
		private static readonly bool DEBUG_HOOP = true;
		private System.Text.StringBuilder debugSb_ = null;
		private System.Text.StringBuilder debugSb
		{
			get
			{
				if (debugSb_ == null)
				{
					debugSb_ = new System.Text.StringBuilder( );
				}
				return debugSb_;
			}
		}
		private Hoop nextHoop_ = null;
		public Hoop nextHoop
		{
			get { return nextHoop_;  }
		}

		private Hoop previousHoop_ = null;
		public Hoop previousHoop
		{
			get { return previousHoop_;  }
		}

		private List< GameObject > hoopPoints_ = new List< GameObject>( );

		private SpinePoint spinePoint_ = null;
		public SpinePoint spinePoint
		{
			get { return spinePoint_;  }
		}

		private int numPoints = 0;

		private float radius_ = 0f;

		public void Init( SpinePoint sp, int np, float r )
		{
			transform.parent = sp.transform;
			transform.localPosition = Vector3.zero;

			spinePoint_ = sp;
			numPoints = np;
			radius_ = r;

			CreateHoopPoints( );
		}

		private void CreateHoopPoints( )
		{
			if (DEBUG_HOOP)
			{
				debugSb.Append( "Creating Hoop Points" );
			}
			if (hoopPoints_.Count > 0)
			{
				if (DEBUG_HOOP)
				{
					debugSb.Append( "\n - deleting " + hoopPoints_.Count );
				}
				for (int i = 0; i< hoopPoints_.Count; i++)
				{
					GameObject.Destroy( hoopPoints_[i] );
				}
				hoopPoints_.Clear( );
			}
			GameObject firstHoopPoint = new GameObject( "HP_0" );
			firstHoopPoint.AddComponent<HoopPoint>( );
			firstHoopPoint.transform.parent = this.transform;
			firstHoopPoint.transform.localPosition = new Vector3( radius_, 0f, 0f );
			if (DEBUG_HOOP)
			{
				debugSb.Append( "\n - added first " );
			}

			hoopPoints_.Add( firstHoopPoint );

			for (int i = 1; i < numPoints; i++)
			{
				GameObject nextHoopPoint = new GameObject( "HP_"+i.ToString() );
				nextHoopPoint.AddComponent<HoopPoint>( );
				nextHoopPoint.transform.parent = this.transform;
				nextHoopPoint.transform.localPosition = new Vector3( radius_, 0f, 0f );
				nextHoopPoint.transform.RotateAround( Vector3.zero, Vector3.forward, i * (360f / numPoints) );
				hoopPoints_.Add( nextHoopPoint );

				if (DEBUG_HOOP)
				{
					debugSb.Append( "\n - added " +i );
				}

			}
			if (DEBUG_HOOP)
			{
				Debug.Log( debugSb.ToString( ) );
			}
			foreach (GameObject hp in hoopPoints_)
			{
				DebugBlob.AddToObject( hp, 0.1f, Color.yellow );
			}
		}
	}

}

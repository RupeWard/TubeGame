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
			get { return nextHoop_; }
		}

		private Hoop previousHoop_ = null;
		public Hoop previousHoop
		{
			get { return previousHoop_; }
		}

		private List<HoopPoint> hoopPoints_ = new List<HoopPoint>( );

		private SpinePoint spinePoint_ = null;
		public SpinePoint spinePoint
		{
			get { return spinePoint_; }
		}

		private int numPoints = 0;

		private float radius_ = 0f;

		public void Init( SpinePoint sp, int np, float r )
		{
			transform.parent = sp.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

			spinePoint_ = sp;
			numPoints = np;
			radius_ = r;

			CreateHoopPoints( );
		}

		private void CreateHoopPoints( )
		{
			if (DEBUG_HOOP)
			{
				debugSb.Length = 0;
				if (hoopPoints_.Count > 0)
				{
					debugSb.Append( "Re-" );
				}
				debugSb.Append( "Creating Hoop Points " );
			}
			if (hoopPoints_.Count > 0)
			{
				if (DEBUG_HOOP)
				{
					debugSb.Append( "\n - deleting " + hoopPoints_.Count );
				}
				for (int i = 0; i < hoopPoints_.Count; i++)
				{
					GameObject.Destroy( hoopPoints_[i].gameObject );
				}
				hoopPoints_.Clear( );
			}
			GameObject firstHoopPointGO = new GameObject( "HP_0" );
			HoopPoint firstHoopPoint = firstHoopPointGO.AddComponent<HoopPoint>( );
			firstHoopPointGO.transform.parent = this.transform;
			firstHoopPointGO.transform.localPosition = new Vector3( radius_, 0f, 0f );
			firstHoopPointGO.transform.localRotation = Quaternion.identity;
			if (DEBUG_HOOP)
			{
				debugSb.Append( "\n - added first " );
			}

			hoopPoints_.Add( firstHoopPoint );

			for (int i = 1; i < numPoints; i++)
			{
				GameObject nextHoopPointGO = new GameObject( "HP_" + i.ToString( ) );
				HoopPoint nextHoopPoint = nextHoopPointGO.AddComponent<HoopPoint>( );
				nextHoopPointGO.transform.parent = this.transform;
				nextHoopPointGO.transform.localPosition = new Vector3( radius_, 0f, 0f );
				nextHoopPointGO.transform.localRotation = Quaternion.identity;

				Vector3 forwardAxis = nextHoopPointGO.transform.TransformDirection( Vector3.forward );
				nextHoopPointGO.transform.RotateAround( spinePoint.transform.position, forwardAxis, i * (360f / numPoints) );
				hoopPoints_.Add( nextHoopPoint );

				if (DEBUG_HOOP)
				{
					debugSb.Append( "\n - added " + i );
				}

			}
			if (DEBUG_HOOP)
			{
				Debug.Log( debugSb.ToString( ) );
			}
			foreach (HoopPoint hp in hoopPoints_)
			{
				RJWard.Core.Test.DebugBlob.AddToObject( hp.gameObject, 0.5f, Color.yellow );
			}
		}

		public void AddAllVertices( List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, float v )
		{
			Vector3 dirn;

            for (int i = 0; i < hoopPoints_.Count; i++)
			{
				hoopPoints_[i].vertexNumber = verts.Count;
				verts.Add( hoopPoints_[i].transform.position );
				uvs.Add( new Vector2( (float)i/(hoopPoints_.Count),v  ) );
				
				dirn = spinePoint.transform.position - hoopPoints_[i].transform.position;
				Vector3 normedDirn = dirn.normalized;

				normals.Add( normedDirn);
				hoopPoints_[i].LookAt(spinePoint.transform);
			}
			hoopPoints_[0].altVertexNumber = verts.Count;
			verts.Add( hoopPoints_[0].transform.position );
			uvs.Add( new Vector2( 1f, v ) );

			dirn = spinePoint.transform.position - hoopPoints_[0].transform.position;
			normals.Add( dirn.normalized );
		}

		public static void AddConnectingTriVerts( Hoop A, Hoop B, List<int> triVerts)
		{
			if (A.hoopPoints_.Count != B.hoopPoints_.Count)
			{
				Debug.LogError( "Hoop point number mismatch" );
			}
			else
			{
				int numPoints = A.hoopPoints_.Count;
				for (int i = 0; i < numPoints; i++)
				{
					int nextIndex = i + 1;
					if (nextIndex >= numPoints)
					{
						nextIndex = 0;
					}
					/*
					triVerts.Add( A.hoopPoints_[i].vertexNumber );
					triVerts.Add( B.hoopPoints_[i].vertexNumber );
					triVerts.Add( A.hoopPoints_[nextIndex].vertexNumber );

					triVerts.Add( A.hoopPoints_[nextIndex].vertexNumber );
					triVerts.Add( B.hoopPoints_[i].vertexNumber );
					triVerts.Add( B.hoopPoints_[nextIndex].vertexNumber );
					*/

					triVerts.Add( A.hoopPoints_[i].vertexNumber );
					triVerts.Add( A.hoopPoints_[nextIndex].getVertexNumber(nextIndex == 0) );
					triVerts.Add( B.hoopPoints_[i].vertexNumber );

					triVerts.Add( A.hoopPoints_[nextIndex].getVertexNumber( nextIndex == 0 ) );
					triVerts.Add( B.hoopPoints_[nextIndex].getVertexNumber( nextIndex == 0 ) );
					triVerts.Add( B.hoopPoints_[i].vertexNumber );
				}
			}
		}

	}
}
using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Hoop : MonoBehaviour
	{
		private Color firstPointColour_ = Color.green;
		private Color endPointColor_ = Color.yellow;

		private static readonly bool DEBUG_LOCAL = false;
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

		private List<HoopPoint> hoopPoints_ = new List<HoopPoint>( );

		private SpinePoint_Base spinePoint_ = null;
		public SpinePoint_Base spinePoint
		{
			get { return spinePoint_; }
		}

		public int numPoints( )
		{
			return hoopPoints_.Count;
		}

		public HoopPoint GetHoopPoint( int index )
		{
			return hoopPoints_[index];
		}

		public void Init( SpinePoint_Base sp )
		{
			gameObject.name = "H" + sp.gameObject.name;

			transform.parent = sp.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;

			spinePoint_ = sp;
		}

		public HoopPoint AddHoopPoint( Vector3 pos )
		{
			int n = hoopPoints_.Count;
			GameObject hoopPointGO = new GameObject( this.gameObject.name + "_HP" + n.ToString( ) );
			HoopPoint hoopPoint = hoopPointGO.AddComponent<HoopPoint>( );
			hoopPointGO.transform.parent = this.transform;
			hoopPointGO.transform.position = pos;
			hoopPointGO.transform.localRotation = Quaternion.identity;
			hoopPoints_.Add( hoopPoint );
			hoopPoint.hoopIndex = n;

			return hoopPoint;
		}

		public Color GetColourForPoint( HoopPoint hp )
		{
			float f = (float)hp.hoopIndex / (hoopPoints_.Count - 1);
			return Color.Lerp( firstPointColour_, endPointColor_, f );
		}

		public void CheckCentreing()
		{
			Vector3 sum = Vector3.zero;
			for (int i =0; i<hoopPoints_.Count; i++)
			{
				sum += hoopPoints_[i].transform.localPosition;
			}
			Vector3 centre = sum / hoopPoints_.Count;
			if (centre.sqrMagnitude > 0.001f)
			{
				Debug.Log( "centre " + centre + " (mag " + centre.magnitude + ")" );
			}
			for (int i =0; i< hoopPoints_.Count; i++)
			{
//				hoopPoints_[i].transform.localPosition = hoopPoints_[i].transform.localPosition + centre;
			}
		}

		public void CreateHoopPointsExplicit( HoopDefinition_Explicit hde)
		{
			if (DEBUG_LOCAL)
			{
				debugSb.Length = 0;
				if (hoopPoints_.Count > 0)
				{
					debugSb.Append( "Re-" );
				}
				debugSb.Append( "Creating Explicit Hoop Points " );
			}
			if (hoopPoints_.Count > 0)
			{
				if (DEBUG_LOCAL)
				{
					debugSb.Append( "\n - deleting " + hoopPoints_.Count );
				}
				for (int i = 0; i < hoopPoints_.Count; i++)
				{
					GameObject.Destroy( hoopPoints_[i].gameObject );
				}
				hoopPoints_.Clear( );
			}
			GameObject firstHoopPointGO = new GameObject( this.gameObject.name + "_HP0" );
			HoopPoint firstHoopPoint = firstHoopPointGO.AddComponent<HoopPoint>( );
			firstHoopPointGO.transform.parent = this.transform;
			firstHoopPointGO.transform.localPosition = hde.GetHoopPointPosition( 0 );
			firstHoopPointGO.transform.localRotation = Quaternion.identity;
			if (DEBUG_LOCAL)
			{
				debugSb.Append( "\n - added first " );
			}
			firstHoopPoint.hoopIndex = 0;
			hoopPoints_.Add( firstHoopPoint );

			for (int i = 1; i < hde.numHoopPoints; i++)
			{
				GameObject nextHoopPointGO = new GameObject( this.gameObject.name + "_HP" + i.ToString( ) );
				HoopPoint nextHoopPoint = nextHoopPointGO.AddComponent<HoopPoint>( );
				nextHoopPointGO.transform.parent = this.transform;
				nextHoopPointGO.transform.localPosition = hde.GetHoopPointPosition( i );// new Vector3( radius_, 0f, 0f );
				nextHoopPointGO.transform.localRotation = Quaternion.identity;
				nextHoopPoint.hoopIndex = i;

				hoopPoints_.Add( nextHoopPoint );

				if (DEBUG_LOCAL)
				{
					debugSb.Append( "\n - added " + i );
				}

			}
			foreach (HoopPoint hp in hoopPoints_)
			{
				RJWard.Core.Test.DebugBlob.AddToObject( hp.gameObject, 0.1f, GetColourForPoint( hp ) );
			}
			spinePoint_.SetDirty( );

			if (DEBUG_LOCAL && debugSb.Length > 0)
			{
				Debug.Log( debugSb.ToString( ) );
			}

		}

		public void CreateHoopPointsCircular( int num, float radius )
		{
			if (DEBUG_LOCAL)
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
				if (DEBUG_LOCAL)
				{
					debugSb.Append( "\n - deleting " + hoopPoints_.Count );
				}
				for (int i = 0; i < hoopPoints_.Count; i++)
				{
					GameObject.Destroy( hoopPoints_[i].gameObject );
				}
				hoopPoints_.Clear( );
			}
			GameObject firstHoopPointGO = new GameObject( this.gameObject.name + "_HP0" );
			HoopPoint firstHoopPoint = firstHoopPointGO.AddComponent<HoopPoint>( );
			firstHoopPointGO.transform.parent = this.transform;
			firstHoopPointGO.transform.localPosition = new Vector3( radius, 0f, 0f );
			firstHoopPointGO.transform.localRotation = Quaternion.identity;
			if (DEBUG_LOCAL)
			{
				debugSb.Append( "\n - added first " );
			}
			firstHoopPoint.hoopIndex = 0;
			hoopPoints_.Add( firstHoopPoint );

			for (int i = 1; i < num; i++)
			{
				GameObject nextHoopPointGO = new GameObject( this.gameObject.name + "_HP" + i.ToString( ) );
				HoopPoint nextHoopPoint = nextHoopPointGO.AddComponent<HoopPoint>( );
				nextHoopPointGO.transform.parent = this.transform;
				nextHoopPointGO.transform.localPosition = new Vector3( radius, 0f, 0f );
				nextHoopPointGO.transform.localRotation = Quaternion.identity;
				nextHoopPoint.hoopIndex = i;

				Vector3 forwardAxis = nextHoopPointGO.transform.TransformDirection( Vector3.forward );
				nextHoopPointGO.transform.RotateAround( spinePoint.transform.position, forwardAxis, i * (360f / num) );
				hoopPoints_.Add( nextHoopPoint );

				if (DEBUG_LOCAL)
				{
					debugSb.Append( "\n - added " + i );
				}

			}
			foreach (HoopPoint hp in hoopPoints_)
			{
				RJWard.Core.Test.DebugBlob.AddToObject( hp.gameObject, 0.1f, GetColourForPoint( hp ) );
			}
			spinePoint_.SetDirty( );
			if (DEBUG_LOCAL)
			{
				Debug.Log( debugSb.ToString( ) );
			}
		}

		public void ExtractAllVertexInfo( List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, float v )
		{
			Vector3 dirn;

			for (int i = 0; i < hoopPoints_.Count; i++)
			{
				hoopPoints_[i].vertexNumber = verts.Count;
				verts.Add( hoopPoints_[i].transform.position );
				if (uvs != null)
				{
					uvs.Add( new Vector2( (float)i / (hoopPoints_.Count), v ) );
				}

				dirn = spinePoint.transform.position - hoopPoints_[i].transform.position;

				if (normals != null)
				{
					Vector3 normedDirn = dirn.normalized;
					normals.Add( normedDirn );
				}

				hoopPoints_[i].LookAt( spinePoint.transform );
			}
			hoopPoints_[0].altVertexNumber = verts.Count;
			verts.Add( hoopPoints_[0].transform.position );
			if (uvs != null)
			{
				uvs.Add( new Vector2( 1f, v ) );
			}
			if (normals != null)
			{
				dirn = spinePoint.transform.position - hoopPoints_[0].transform.position;
				normals.Add( dirn.normalized );
			}

		}

		public void ExtractDiscTriVerts( List<int> triVerts, int spineIndex, bool reverse )
		{
			List<int> newVerts = new List<int>( );
			for (int i = 0; i < numPoints(); i++)
			{
				int j = i + 1;
				if (j == numPoints( ))
				{
					j = 0;
				}
				newVerts.Add( spineIndex );
				newVerts.Add( GetHoopPoint( i ).vertexNumber);
				newVerts.Add( GetHoopPoint( j ).getVertexNumber(j==0));
			}
			if (reverse)
			{
				newVerts.Reverse( );
			}
			triVerts.AddRange( newVerts );
		}

		public static void ExtractConnectingTriVerts( Hoop A, Hoop B, List<int> triVerts, bool reverse )
		{
			if (A.hoopPoints_.Count != B.hoopPoints_.Count)
			{
				Debug.LogError( "Hoop point number mismatch" );
			}
			else
			{
				List<int> newVerts = new List<int>( );

				int numPoints = A.hoopPoints_.Count;
				for (int i = 0; i < numPoints; i++)
				{
					int nextIndex = i + 1;
					if (nextIndex >= numPoints)
					{
						nextIndex = 0;
					}
					newVerts.Add( A.hoopPoints_[i].vertexNumber );
					newVerts.Add( A.hoopPoints_[nextIndex].getVertexNumber( nextIndex == 0 ) );
					newVerts.Add( B.hoopPoints_[i].vertexNumber );

					newVerts.Add( A.hoopPoints_[nextIndex].getVertexNumber( nextIndex == 0 ) );
					newVerts.Add( B.hoopPoints_[nextIndex].getVertexNumber( nextIndex == 0 ) );
					newVerts.Add( B.hoopPoints_[i].vertexNumber );
				}
				if (reverse)
				{
					newVerts.Reverse( );
				}
				triVerts.AddRange( newVerts );
			}
		}

		public HoopDefinition_Explicit ExplicitDefinition( )
		{
			HoopDefinition_Explicit result = new HoopDefinition_Explicit(this );
			return result;
		}
	}
}
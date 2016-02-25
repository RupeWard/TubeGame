using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Spine : MonoBehaviour
	{
		private TubeSection tubeSection_ = null;

		private List< SpinePoint > spinePoints_ = new List< SpinePoint >( );

		public int NumSpinePoints
		{
			get { return spinePoints_.Count;  }
		}

		public void Init(TubeSection ts)
		{
			tubeSection_ = ts;
			transform.parent = tubeSection_.transform;
		}

		public void SetDirty()
		{
			tubeSection_.SetMeshDirty( );
		}

		public SpinePoint GetSpinePoint(int index)
		{
			SpinePoint result = null;
			if (index >= 0  && index < NumSpinePoints)
			{
				result = spinePoints_[index];
			}
			return result;
		}

		/*
		public void MakeLastLookBack()
		{
			if (NumSpinePoints > 1)
			{
				spinePoints_[NumSpinePoints - 1].HandlePreviousPointAdded( spinePoints_[NumSpinePoints - 2] );
			}
		}
		*/

		public void AddSpinePoint(SpinePointDefinition spd)
		{
			AddSpinePoint( spd.position, spd.rotation, spd.radius);
		}

		public void AddSpinePoint( Vector3 pos, Vector3? rot, float radius )
		{
			GameObject spGo = new GameObject( "SP_"+spinePoints_.Count.ToString() );
			SpinePoint spinePoint = spGo.AddComponent<SpinePoint>( );
			spGo.transform.parent = this.transform;
			spinePoint.InitCircular( this, pos, rot, radius );
			if (spinePoints_.Count > 0)
			{
				spinePoints_[spinePoints_.Count - 1].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 1];
            }
		
			spinePoints_.Add( spinePoint );

			RJWard.Core.Test.DebugBlob.AddToObject( spGo, 0.15f, Color.green );
		}

		public void AddAllVertexInfoToLists( List< Vector3 > verts, List< Vector3 > normals, List <Vector2> uvs)
		{
			for (int i = 0; i< spinePoints_.Count; i++)
			{
				float v = (float)i / (spinePoints_.Count - 1);
				spinePoints_[i].AddAllVertices( verts, normals, uvs, v );
			}
		}

		public void AddAllTriInfoToList( List<int> triVerts)
		{
			if (spinePoints_.Count > 1)
			{
				for (int i = 0; i < (spinePoints_.Count - 1); i++)
				{
					Hoop hoopA = spinePoints_[i].hoop;
					Hoop hoopB = spinePoints_[i + 1].hoop;

					Hoop.AddConnectingTriVerts( hoopA, hoopB, triVerts );
				}
			}
		}
	}


}

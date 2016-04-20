using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Spine : MonoBehaviour
	{
		private TubeSection_Linear tubeSection_ = null;

		private List< SpinePoint_Simple > spinePoints_ = new List< SpinePoint_Simple >( );

		public int NumSpinePoints
		{
			get { return spinePoints_.Count;  }
		}

		public void Init(TubeSection_Linear ts)
		{
			tubeSection_ = ts;
			transform.parent = tubeSection_.transform;
		}

		public void SetDirty()
		{
			tubeSection_.SetMeshDirty( false );
		}

		public SpinePoint_Simple GetSpinePoint(int index)
		{
			SpinePoint_Simple result = null;
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

		public SpinePoint_Simple AddHoopLess(Vector3 pos, bool fixedRotation)
		{
			GameObject spGo = new GameObject( "SP_" + spinePoints_.Count.ToString( ) );
			SpinePoint_Simple spinePoint = spGo.AddComponent<SpinePoint_Simple>( );
			if (fixedRotation)
			{
				spinePoint.fixRotation( );
			}
			spinePoint.Init( this, pos, null );
			spGo.transform.parent = this.transform;
			spinePoints_.Add( spinePoint );
			if (spinePoints_.Count > 1)
			{
				spinePoints_[spinePoints_.Count - 2].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 2];
			}

			RJWard.Core.Test.DebugBlob.AddToObject( spGo, 0.05f, Color.green );

			return spinePoint;
		}

		public void AddSpinePoint( HoopDefinition_Base hdb, bool fixedRotation)
		{
			hdb.AddToSpine( this, fixedRotation );
		}

		public void AddCircularSpinePoint( HoopDefinition_Circular hdc, bool fixedRotation )
		{
			AddCircularSpinePoint( hdc.position, hdc.rotation, hdc.numHoopPoints, hdc.radius, fixedRotation );
		}

		private void AddCircularSpinePoint( Vector3 pos, Vector3? rot, int num,  float radius, bool fixedRotation )
		{
			SpinePoint_Simple spinePoint = AddHoopLess( pos, fixedRotation );
			spinePoint.InitCircular( this, pos, rot, num, radius );
			if (spinePoints_.Count > 1)
			{
				spinePoints_[spinePoints_.Count - 2].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 2];
            }
		}

		public void AddExplicitSpinePoint(HoopDefinition_Explicit hde, bool fixedRotation)
		{
			SpinePoint_Simple spinePoint = AddHoopLess( hde.position, fixedRotation );
			spinePoint.InitExplicit( this, hde );
			if (spinePoints_.Count > 1)
			{
				spinePoints_[spinePoints_.Count - 2].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 2];
			}
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

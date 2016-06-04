using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Spine_Linear : MonoBehaviour
	{
		private TubeSection_Linear tubeSection_ = null;
		public TubeSection_Linear tubeSection
		{
			get { return tubeSection_; }
		}

		private List< SpinePoint_Linear > spinePoints_ = new List< SpinePoint_Linear >( );

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

		public SpinePoint_Linear GetSpinePoint(int index)
		{
			SpinePoint_Linear result = null;
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

		public SpinePoint_Linear AddHoopLess(Vector3 pos, bool fixedRotation)
		{
			GameObject spGo = new GameObject( "SP_" + spinePoints_.Count.ToString( ) );
			spGo.layer = TubeFactory.Instance.buildLayerMask;

			SpinePoint_Linear spinePoint = spGo.AddComponent<SpinePoint_Linear>( );
			if (fixedRotation)
			{
				spinePoint.fixRotation(true );
			}
			spinePoint.Init( this, pos, null );
			spGo.transform.parent = this.transform;
			spinePoints_.Add( spinePoint );
			if (spinePoints_.Count > 1)
			{
				spinePoints_[spinePoints_.Count - 2].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 2];
			}

			DebugBlob debugBlob = DebugBlob.AddToObject( spGo, 0.05f, Color.green );
			if (debugBlob != null)
			{
				debugBlob.ActivateDirectionPointer( true );
			}

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
			SpinePoint_Linear spinePoint = AddHoopLess( pos, fixedRotation );
			spinePoint.InitCircular( this, pos, rot, num, radius );
			if (spinePoints_.Count > 1)
			{
				spinePoints_[spinePoints_.Count - 2].nextSpinePoint = spinePoint;
				spinePoint.previousSpinePoint = spinePoints_[spinePoints_.Count - 2];
            }
		}

		public void AddExplicitSpinePoint(HoopDefinition_Explicit hde, bool fixedRotation)
		{
			SpinePoint_Linear spinePoint = AddHoopLess( hde.position, fixedRotation );
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
//				float v = (float)i / (spinePoints_.Count - 1);
				float v = (i % 2 == 0) ? (0f) : (1f);
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

					Hoop.ExtractConnectingTriVerts( hoopA, hoopB, triVerts, false );
				}
			}
		}
	}


}

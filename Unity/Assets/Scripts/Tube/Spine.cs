using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class Spine : MonoBehaviour
	{
		private List< SpinePoint > spinePoints_ = new List< SpinePoint >( );

		public void AddSpinePoint( Vector3 pos, Vector3? rot, float radius )
		{
			GameObject spGo = new GameObject( "SP_"+spinePoints_.Count.ToString() );
			SpinePoint spinePoint = spGo.AddComponent<SpinePoint>( );
			spGo.transform.parent = this.transform;
			spinePoint.Init( pos, rot, radius );

			if (spinePoints_.Count > 0)
			{
				spinePoints_[spinePoints_.Count - 1].HandleNextPointAdded(spinePoint);
			}
		
			spinePoints_.Add( spinePoint );

			RJWard.Core.Test.DebugBlob.AddToObject( spGo, 0.15f, Color.green );
			
			
		}

		public void AddAllVertices( List< Vector3 > verts, List <Vector2> uvs)
		{
			for (int i = 0; i< spinePoints_.Count; i++)
			{
				float v = (float)i / (spinePoints_.Count - 1);
				spinePoints_[i].AddAllVertices( verts, uvs, v );
			}
		}

		public void AddAllTriVerts( List<int> triVerts)
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

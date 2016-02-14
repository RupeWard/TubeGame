using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSection : MonoBehaviour
	{
		public Spine spine = null;

		public static TubeSection CreateLinear( Vector3 start, Vector3? startRotation, Vector3 end, Vector3? endRotation, int num, float radius )
		{
			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );

			GameObject spineGO = new GameObject( "Spine" );
			spineGO.transform.parent = tsGo.transform;

			result.spine = spineGO.AddComponent<Spine>( );

			for (int i = 0; i < num; i++)
			{
				Vector3? rot = null;
				if (i == 0)
				{
					rot = startRotation;
				}
				else if (i == num-1)
				{
					rot = endRotation;
				}
				result.spine.AddSpinePoint( Vector3.Lerp(start, end, (float)i/(num-1) ) , rot, radius );
			}

			result.MakeMesh( );

			return result;
		}

		private void MakeMesh()
		{
			MeshRenderer meshRenderer = GetComponent<MeshRenderer>( );
			if (meshRenderer == null)
			{
				meshRenderer = gameObject.AddComponent<MeshRenderer>( );
			}
			MeshFilter meshFilter = GetComponent<MeshFilter>( );
			if (meshFilter == null)
			{
				meshFilter = gameObject.AddComponent<MeshFilter>( );
			}
			Mesh mesh = meshFilter.sharedMesh;
			if (mesh == null)
			{
				meshFilter.sharedMesh = new Mesh( );
				mesh = meshFilter.sharedMesh;
			}
			mesh.Clear( );

			List<Vector3> verts = new List<Vector3>( );
			
			if (spine != null)
			{
				spine.AddAllVertices( verts );
				Debug.Log( "Verts count = " + verts.Count );

				List<int> triVerts = new List<int>( );
				spine.AddAllTriVerts( triVerts );

				mesh.vertices = verts.ToArray( );
				mesh.triangles = triVerts.ToArray( );
				mesh.uv = new Vector2[verts.Count];

				mesh.RecalculateNormals( );
				mesh.RecalculateBounds( );
				mesh.Optimize( );



			} 
		}


	}

}

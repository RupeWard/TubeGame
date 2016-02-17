using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class TubeSection : MonoBehaviour
	{
		public Spine spine = null;
		private Material tubeWallMaterial_;

		private static System.Text.StringBuilder debugSb = new System.Text.StringBuilder( );

		public static TubeSection Create( TubeSectionDefinition tsd, Material mat )
		{
			debugSb.Length = 0;
			debugSb.Append( "Creating TubeSection" );

			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );
			result.tubeWallMaterial_ = mat;

			GameObject spineGO = new GameObject( "Spine" );
			spineGO.transform.parent = tsGo.transform;

			result.spine = spineGO.AddComponent<Spine>( );

			debugSb.Append( "\n Created spine, adding " ).Append( tsd.NumSpinePoints ).Append( " spinepoints" );

			for (int i=0; i< tsd.NumSpinePoints; i++)
			{
				SpinePointDefinition spd = tsd.GetSpinePointDefn( i );
				if (spd != null)
				{
					result.spine.AddSpinePoint( spd );
					debugSb.Append( "\n  " ).Append( i ).Append( ": " ).DebugDescribe( spd );
				}
				else
				{
					Debug.LogError( "NULL SPD" );
					debugSb.Append( "\n  " ).Append( i ).Append( ": NULL" );
				}
			}

			Debug.Log( debugSb.ToString());

			result.MakeMesh( );

			return result;
		}

		public static TubeSection CreateLinear( Vector3 start, Vector3? startRotation, Vector3 end, Vector3? endRotation, int num, float startRadius, float endRadius, Material mat )
		{			
			GameObject tsGo = new GameObject( "TubeSection" );
			TubeSection result = tsGo.AddComponent<TubeSection>( );
			result.tubeWallMaterial_ = mat;

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
				float interpolator = (float)i / (num - 1);
                result.spine.AddSpinePoint( Vector3.Lerp(start, end,  interpolator) , rot, Mathf.Lerp(startRadius, endRadius,  interpolator));
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
			meshRenderer.sharedMaterial = tubeWallMaterial_;

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

			
			if (spine != null)
			{
				List<Vector3> verts = new List<Vector3>( );
				List<Vector2> uvs = new List<Vector2>( );
				List<Vector3> normals = new List<Vector3>( );

				spine.AddAllVertices( verts, normals, uvs );
				Debug.Log( "Verts count = " + verts.Count );

				List<int> triVerts = new List<int>( );
				spine.AddAllTriVerts( triVerts );

				mesh.vertices = verts.ToArray( );
				mesh.triangles = triVerts.ToArray( );
				mesh.uv = uvs.ToArray();
				mesh.normals = normals.ToArray();

//				mesh.RecalculateNormals( );
				mesh.RecalculateBounds( );
				mesh.Optimize( );

				RJWard.Core.ReverseNormals reverseNormals = GetComponent<RJWard.Core.ReverseNormals>( );
				if (reverseNormals == null)
				{
					reverseNormals = gameObject.AddComponent<RJWard.Core.ReverseNormals>( );
                }
				reverseNormals.Init( Core.ReverseNormals.EState.Inside );
			} 
		}


	}

}

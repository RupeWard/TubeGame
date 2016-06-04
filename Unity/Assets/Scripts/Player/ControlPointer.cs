using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube.Player
{
	[RequireComponent( typeof( MeshFilter ) )]
	[RequireComponent( typeof( MeshRenderer ) )]
	public class ControlPointer : MonoBehaviour
	{
		#region inspector data

		public float radius = 0.3f;
		public float length = 0.3f;
		public int numSegments = 10;

		public Material material;
		
		#endregion inspector data

		#region private hooks

		private Transform cachedTransform_ = null;
		private MeshFilter meshFilter_ = null;
		private MeshRenderer meshRenderer_ = null;
		private Player player_;

		#endregion private hooks

		private void Awake( )
		{
			cachedTransform_ = transform;
			meshFilter_ = GetComponent<MeshFilter>( );
			meshRenderer_ = GetComponent<MeshRenderer>( );

			CreateMesh( );
			gameObject.SetActive( false );
		}

		public void Init( Player p)
		{
			player_ = p;
		}

	private void CreateMesh( )
		{
			cachedTransform_.position = Vector3.zero;

			meshRenderer_.sharedMaterial = material;

			Mesh mesh = meshFilter_.sharedMesh;
			if (mesh == null)
			{
				meshFilter_.sharedMesh = new Mesh( );
				mesh = meshFilter_.sharedMesh;
			}

			List<Vector3> verts = new List<Vector3>( );
			List<Vector2> uvs = new List<Vector2>( );
			List<Vector3> normals = new List<Vector3>( );


			Vector3 apex = new Vector3( 0f, 0f, 0.5f * length ); // apex at 0
			verts.Add( apex );
			uvs.Add( new Vector2(0.5f, 0.5f) );
			normals.Add( new Vector3( 0f, 0f, 1f ) );

			Vector3 centreBack = new Vector3( 0f, 0f, -0.5f * length ); // centreback at 1
			verts.Add( centreBack );
			uvs.Add( new Vector2( 0.5f, 0.5f ) );
			normals.Add( new Vector3( 0f, 0f, -1f ) );

			float angle = (2f * Mathf.PI) / numSegments;

			float v = 0f;
			float vd = 0.5f;

			// back-facing points at 2 to 1 + numSegments
			for (int i = 0; i < numSegments; i++)
			{
				Vector3 newPoint = centreBack;
				newPoint.x = radius * Mathf.Sin( angle * i );
				newPoint.y = radius * Mathf.Cos( angle * i );

				verts.Add( newPoint );
				uvs.Add( new Vector2( 0.5f, v ) );
				v += vd;
				if ( v >= 1f || v <= 0f)
				{
					vd *= -1f;
				}
				normals.Add( new Vector3( 0f, 0f, -1f ) );
			}

			List<int> triVerts = new List<int>( );

			for (int i = 0; i < numSegments; i++)
			{
				triVerts.Add( 0 ); // apex
				int nextIndex = i + 1;
				if (nextIndex >= numSegments)
				{
					nextIndex = 0;
				}
				triVerts.Add( 2 + nextIndex );
				triVerts.Add( 2 + i );

				triVerts.Add( 1 );
				triVerts.Add( 2 + i );
				triVerts.Add( 2 + nextIndex );
			}

			mesh.vertices = verts.ToArray( );
			mesh.triangles = triVerts.ToArray( );
			mesh.uv = uvs.ToArray( );
			mesh.normals = normals.ToArray( );

			mesh.RecalculateBounds( );
			mesh.Optimize( );


		}

		public void UpdatePosition(Vector3 pos)
		{
			cachedTransform_.position = pos;
			cachedTransform_.LookAt( player_.cachedTransform.position );
		}
	}
}

using UnityEngine;
using System.Collections;

namespace RJWard.Core
{
	[RequireComponent( typeof( MeshFilter ) )]
	public class ReverseNormals : MonoBehaviour
	{
		public enum EState
		{
			Inside,
			Outside
		}

		public EState state = EState.Outside;
		private EState cachedState = EState.Outside;

		public void Init( EState s )
		{
			state = s;
		}

		void Start( )
		{
			cachedState = state;
			if (state == EState.Inside)
			{
				Reverse( );
			}
		}

		public bool SetState( EState newState )
		{
			if (cachedState != newState)
			{
				//			Debug.LogWarning ( "State change from " + cachedState + " to " + newState );
				Reverse( );
				state = newState;
				cachedState = newState;
				return true;
			}
			else
			{
				//			Debug.LogError("No change: "+newState);
			}
			return false;
		}

		void Update( )
		{
#if UNITY_EDITOR
			if (cachedState != state)
			{
				Debug.LogWarning( "ReverseNormals Detected state change on "+gameObject.name );
				SetState( state );
			}
#endif
		}

		void Reverse( )
		{
			MeshFilter filter = GetComponent( typeof( MeshFilter ) ) as MeshFilter;
			if (filter != null)
			{
				Mesh mesh = filter.mesh;

				Vector3[] normals = mesh.normals;
				for (int i = 0; i < normals.Length; i++)
					normals[i] = -normals[i];
				mesh.normals = normals;

				for (int m = 0; m < mesh.subMeshCount; m++)
				{
					int[] triangles = mesh.GetTriangles( m );
					for (int i = 0; i < triangles.Length; i += 3)
					{
						int temp = triangles[i + 0];
						triangles[i + 0] = triangles[i + 1];
						triangles[i + 1] = temp;
					}
					mesh.SetTriangles( triangles, m );
				}
			}

			MeshCollider mc = GetComponent<MeshCollider>( );
			if (mc != null)
			{
				mc.sharedMesh = filter.mesh;
			}
		}

	}

}

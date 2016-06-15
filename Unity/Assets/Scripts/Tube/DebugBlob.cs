using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class DebugBlob : MonoBehaviour
	{
#if UNITY_EDITOR
		static public readonly bool disable = true;
#else
		static public readonly bool disable = true; // Don't change
#endif

		static private List<GameObject> s_debugBlob_ = new List<GameObject>( );
		static private bool s_areDebugObjectsShowing_ = false;

		static public void RegisterDebugBlob( GameObject go )
		{
			if (s_debugBlob_.Contains( go ) == false)
			{
				s_debugBlob_.Add( go );
			}
		}

		static public void DeregisterDebugBlob( GameObject go )
		{
			if (s_debugBlob_.Contains( go ) == true)
			{
				s_debugBlob_.Remove( go );
			}
		}

		static public void ActivateAllDebugBlobs( bool b )
		{
			s_areDebugObjectsShowing_ = b;
			foreach (GameObject go in s_debugBlob_)
			{
				go.SetActive( s_areDebugObjectsShowing_ );
			}
		}

		static private GameObject s_cachedPrefab_ = null;
		static private GameObject s_prefab
		{
			get
			{
				if (s_cachedPrefab_ == null)
				{
					s_cachedPrefab_ = Resources.Load( "Prefabs/DebugBlob" ) as GameObject;
				}
				return s_cachedPrefab_;
			}
		}
		public static DebugBlob AddToObject( GameObject go, float s, Color c )
		{
			if (!disable)
			{
				GameObject dbGo = Instantiate( s_prefab ) as GameObject;
				DebugBlob result = dbGo.GetComponent<DebugBlob>( );
				result.transform.parent = go.transform;
				result.transform.localPosition = Vector3.zero;
				result.transform.localRotation = Quaternion.identity;
				result.Init( s, c );

				return result;
			}
			else
			{
				return null;
			}
		}

		private Transform cachedTransform_ = null;

		private Material mat_ = null;
		public MeshRenderer pointerMesh;

		private void Awake( )
		{
			cachedTransform_ = transform;
			mat_ = GetComponent<MeshRenderer>( ).sharedMaterial;
			pointerMesh.gameObject.SetActive( false );

			RegisterDebugBlob( gameObject );

			if (s_areDebugObjectsShowing_ == false)
			{
				gameObject.SetActive( false );
			}
		}

		private void OnDestroy()
		{
			DeregisterDebugBlob( gameObject );
		}

		private void Init( float s, Color c )
		{
			cachedTransform_.localScale = s * Vector3.one;
			if (mat_.color != c)
			{
				mat_ = GetMaterialForColor( c );
				GetComponent<MeshRenderer>( ).sharedMaterial = mat_;
				pointerMesh.sharedMaterial = mat_;
			}
			pointerMesh.gameObject.SetActive( false );
		}

		static Dictionary<Color, Material> s_materials_ = new Dictionary<Color, Material>( );
		Material GetMaterialForColor( Color c)
		{
			Material result = null;
			if (s_materials_.ContainsKey( c ))
			{
				result = s_materials_[c];
			}
			else
			{
				result = new Material( mat_ );
				result.color = c;
				result.SetColor( "_EmissionColor", c );
				s_materials_.Add( c, result );
			}
			return result;
		}

		public void ActivateDirectionPointer( bool active )
		{
			pointerMesh.gameObject.SetActive( active );
		}
	}

}


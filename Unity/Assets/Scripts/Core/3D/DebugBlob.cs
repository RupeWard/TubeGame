using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RJWard.Core.Test
{
	public class DebugBlob : MonoBehaviour
	{
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
			GameObject dbGo = Instantiate( s_prefab ) as GameObject;
			DebugBlob result = dbGo.GetComponent<DebugBlob>( );
			result.transform.parent = go.transform;
			result.transform.localPosition = Vector3.zero;
			result.transform.localRotation = Quaternion.identity;
			result.Init( s, c );

			return result;
		}

		private Transform cachedTransform_ = null;

		private Material mat_ = null;
		public MeshRenderer pointerMesh;

		private void Awake( )
		{
			cachedTransform_ = transform;
			mat_ = GetComponent<MeshRenderer>( ).sharedMaterial;
			pointerMesh.gameObject.SetActive( false );
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


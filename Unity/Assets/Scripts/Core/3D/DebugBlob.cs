using UnityEngine;
using System.Collections;

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
	public static DebugBlob AddToObject( GameObject go, float s, Color c)
	{
		GameObject dbGo = Instantiate( s_prefab ) as GameObject;
		DebugBlob result = dbGo.GetComponent<DebugBlob>( );
		result.transform.parent = go.transform;
		result.transform.localPosition = Vector3.zero;
		result.Init( s, c );
		return result;
	}

	private Material mat_ = null;
	private void Awake()
	{
		mat_ = GetComponent<MeshRenderer>( ).material;
	}

	private void Init( float s, Color c)
	{
		transform.localScale = s * Vector3.one;
		mat_.color = c;
		mat_.SetColor( "_EmissionColor", c );
	}

}

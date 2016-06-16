using UnityEngine;
using System.Collections;
using RJWard.Core.UI.Extensions;

public class UIControlPointer : MonoBehaviour
{
	private UnityEngine.UI.Image image_ =  null;
	private RectTransform cachedRT_ = null;
	private RectTransform viewPort_ = null;
	private Vector2 halfViewPortDims_;

	public float distFromCentre = 0.6f;

	public Color lowColour = Color.white;
	public Color highColour = Color.white;

	private void Awake()
	{
		cachedRT_ = GetComponent<RectTransform>( );
		viewPort_ = cachedRT_.parent.GetComponent<RectTransform>( );
		image_ = GetComponent<UnityEngine.UI.Image>( );
		image_.enabled = false;
	}

	private void Start()
	{
		halfViewPortDims_ = new Vector2 ( 0.5f * viewPort_.GetWidth( ), 0.5f * viewPort_.GetHeight( ) );
	}

	private void SetControlVector(Vector2 v)
	{
		if (v.sqrMagnitude > 0f)
		{
			float angle = Mathf.Atan2( v.y, v.x );

			float y = Mathf.Sin( angle ) * distFromCentre;
			float x = Mathf.Cos( angle ) * distFromCentre;

			Vector2 relpos = new Vector2( x, y );
			Vector2 actualPos = new Vector2( relpos.x * halfViewPortDims_.x, relpos.y * halfViewPortDims_.y );

			cachedRT_.anchoredPosition = actualPos;
			cachedRT_.rotation = Quaternion.Euler( new Vector3( 0f, 0f, Mathf.Rad2Deg * angle ) );
			//Debug.Log( "Setting anchors to " + relpos + " = "+actualPos);

			image_.color = Color.Lerp( lowColour, highColour, v.magnitude );
			image_.enabled = true;
		}
		else
		{
			image_.enabled = false;
		}
	}

	private void Update()
	{
		SetControlVector( GameManager.Instance.currentControlForce );
	}
	
}

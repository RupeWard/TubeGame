using UnityEngine;
using System.Collections;
using RJWard.Core.UI.Extensions;

public class PlayerControlPanel : MonoBehaviour
{
	private enum Emode
	{
		Idle,
		SimpleForce,
	};

	private Emode mode_ = Emode.Idle;

	static private readonly bool DEBUG_CONTROLS = false;
	static private readonly bool DEBUG_TOUCH = false;

	#region inspector hooks

	public RectTransform centreMarker;
	public RectTransform centreButton;

	#endregion inspector hooks

	#region inspector data

	public float centreButtonReturnSpeed = 1f;

	public Color centreButtonActiveColour = Color.green;
	public Color centreButtonInactiveColour = Color.white;
	public Color centreButtonReturningColour = Color.red;

	#endregion inspector data

	#region private hooks

	RectTransform cachedRT_ = null;
	RectTransform centreButtonRT_ = null;
	UnityEngine.UI.Image centreButtonImage_ = null;

	#endregion private hooks

	#region private data

	private bool isActive_ = false;
	private Vector2 dims_ = Vector2.zero;
	private Vector2 halfDims_ = Vector2.zero;
	private Vector2 halfDimsSqr_ = Vector2.zero;
	private float centreButtonRadius_ = 0f;
	private float centreButtonRadiusSqr_ = 0f;
	private bool centreButtonReturning_ = false;

	#endregion private data

	private class LocalTouch
	{
		private Vector2 position_ = Vector2.zero;
		public Vector2 position
		{
			get { return position_; }
		}
	}

	private void SwitchToIdle()
	{
		mode_ = Emode.Idle;
		SetCentreButtonActive( false );
		GameManager.Instance.SetControlForce( Vector2.zero );
	}

	public void HandleSimpleForce(Touch[] touches)
	{
		if (touches.Length != 1)
		{
			if (DEBUG_CONTROLS)
			{
				Debug.Log( "SimpleForce: Switch to idle (" + touches.Length+") touches" );
			}
			SwitchToIdle( );
		}
		else
		{
			Touch touch = touches[0];
			Vector2 v2;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRT_, touch.position, null, out v2 ))
			{
				v2.y += halfDims_.y;
				if (DEBUG_TOUCH)
				{
					Debug.Log( "SimpleForce: Single Touch  at " + touch.position + " ==>" + v2 + ", phase = " + touch.phase );
				}

				switch (touch.phase)
				{					
					case TouchPhase.Began:
						{
							/*
							if (!isCentreButtonActive( ) && !centreButtonReturning_)
							{
								if (v2.sqrMagnitude < halfDimsSqr_.x)
								{
									mode_ = Emode.SimpleForce;
									SetCentreButtonActive( true );
									centreButtonRT_.anchoredPosition = v2;
									Vector2 force = centreButtonRT_.anchoredPosition / halfDims_.x;
									GameManager.Instance.SetControlForce( force );
									if (DEBUG_CONTROLS)
									{
										Debug.Log( "Force started at " + force );
									}
								}
							}*/
							Debug.LogWarning( "TP = began but mode = " + mode_ );
							break;
						}
					case TouchPhase.Moved:
						{
							if (isCentreButtonActive( ))
							{
								if (v2.sqrMagnitude > halfDimsSqr_.x)
								{
									v2 = v2.normalized * halfDims_.x;
								}
								{
									centreButtonRT_.anchoredPosition = v2;
									Vector2 force = centreButtonRT_.anchoredPosition / halfDims_.x;
									GameManager.Instance.SetControlForce( force );
									if (DEBUG_CONTROLS)
									{
										Debug.Log( "SimpleForce: Force set to " + force );
									}
								}
							}
							break;
						}
					case TouchPhase.Ended:
						{
							if (DEBUG_CONTROLS)
							{
								Debug.Log( "SimpleForce: Switch to idle (" + touches.Length + ") touches" );
							}
							SwitchToIdle( );
							break;
						}


				}

			}

		}
	}

	private void SwitchToSimpleForce( Vector2 v2 )
	{
		if (v2.sqrMagnitude < halfDimsSqr_.x)
		{
			mode_ = Emode.SimpleForce;
			SetCentreButtonActive( true );
			centreButtonRT_.anchoredPosition = v2;
			Vector2 force = centreButtonRT_.anchoredPosition / halfDims_.x;
			GameManager.Instance.SetControlForce( force );
			if (DEBUG_CONTROLS)
			{
				Debug.Log( "Force started at " + force );
			}
		}
	}

	public void HandleIdle( Touch[] touches )
	{
		if (touches.Length == 1)
		{
			Touch touch = touches[0];
			Vector2 v2;
			if (RectTransformUtility.ScreenPointToLocalPointInRectangle( cachedRT_, touch.position, null, out v2 ))
			{
				v2.y += halfDims_.y;
				if (DEBUG_TOUCH)
				{
					Debug.Log( "Single Touch  at " + touch.position + " ==>" + v2 + ", phase = " + touch.phase );
				}

				//			bool bTouchOutsideArea = (v2.sqrMagnitude > halfDimsSqr_.x);

				switch (touch.phase)
				{
					case TouchPhase.Began:
						{
							if (!isCentreButtonActive( ) && !centreButtonReturning_)
							{
								if (v2.sqrMagnitude < halfDimsSqr_.x)
								{
									SwitchToSimpleForce( v2 );
									if (DEBUG_CONTROLS)
									{
										Debug.Log( "Idle: Force started at " + v2 );
									}
								}
							}
							break;
						}
				}

			}

		}

	}

	public void HandleTouches( Touch[] touches)
	{
		switch (mode_)
		{
			case Emode.Idle:
				{
					HandleIdle( touches );
					break;
				}
			case Emode.SimpleForce:
				{
					HandleSimpleForce( touches );
					break;
				}
		}
	}

	#region centre button

	private bool isCentreButtonActive()
	{
		return centreButton.gameObject.activeSelf;
	}

	private void SetCentreButtonActive( bool b)
	{
		if (b != isCentreButtonActive())
		{
			if (b)
			{
				SetCentreButtonColour( centreButtonActiveColour );
				centreButton.gameObject.SetActive( true );
			}
			else
			{
				GameManager.Instance.SetControlForce( Vector2.zero );
				if (DEBUG_CONTROLS)
				{
					Debug.Log( "Force set to zero");
				}
				StartCoroutine( ReturnCentreButtonToOriginAndDeactivate( ) );
			}
		}
	}

	private IEnumerator ReturnCentreButtonToOriginAndDeactivate( )
	{
		yield return StartCoroutine( ReturnCentreButtonToOrigin( ) );
		centreButton.gameObject.SetActive( false );
	}

	private IEnumerator ReturnCentreButtonToOrigin()
	{
		centreButtonReturning_ = true;
		SetCentreButtonColour( centreButtonReturningColour );

		Vector2 startPos = centreButtonRT_.anchoredPosition;
		Vector2 endPos = Vector2.zero;
		Vector2 pos = startPos;
		while( (pos - endPos).sqrMagnitude > Mathf.Epsilon)
		{
			pos = Vector2.MoveTowards( pos, endPos, centreButtonReturnSpeed * Time.deltaTime );
			centreButtonRT_.anchoredPosition = pos;
			yield return null;
		}
		centreButtonRT_.anchoredPosition = endPos;
		centreButtonReturning_ = false;
		SetCentreButtonColour( centreButtonInactiveColour);
	}

	private bool IsTouchInCentreButton(Vector2 v)
	{
		bool result = false;
		if (( v - centreButtonRT_.anchoredPosition).sqrMagnitude < centreButtonRadiusSqr_)
		{
			result = true;
		}
		return result;
	}

	private void SetCentreButtonColour(Color c)
	{
		centreButtonImage_.color = c;
	}

	#endregion centre button

	#region flow

	private void Activate(bool b)
	{
		if (b != isActive_)
		{
			isActive_ = b;
			if (isActive_)
			{
				if (DEBUG_CONTROLS)
				{
					Debug.Log( "PlayerControls activated" );
				}
				TouchManager.Instance.onTouches += HandleTouches;

				dims_.x = cachedRT_.GetWidth( );
				dims_.y = cachedRT_.GetHeight( );

				halfDims_ = 0.5f * dims_;
				halfDimsSqr_ = new Vector2( halfDims_.x * halfDims_.x, halfDims_.y * halfDims_.y );
			}
			else
			{
				if (DEBUG_CONTROLS)
				{
					Debug.Log( "PlayerControls deactivated" );
				}
				TouchManager.Instance.onTouches -= HandleTouches;
			}

		}
	}

	private void Awake()
	{
		cachedRT_ = GetComponent<RectTransform>();
		centreButtonRT_ = centreButton.GetComponent<RectTransform>( );
		centreButtonRadius_ = 0.5f * centreButtonRT_.GetWidth( );
		centreButtonRadiusSqr_ = centreButtonRadius_ * centreButtonRadius_;
		centreButton.gameObject.SetActive( false );
		centreButtonImage_ = centreButton.GetComponent<UnityEngine.UI.Image>( );
		SetCentreButtonColour( centreButtonInactiveColour );
	}

	private void Start()
	{
		Activate( true );
	}
	#endregion flow

}

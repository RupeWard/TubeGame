using UnityEngine;
using System.Collections;
using RJWard.Tube;
using RJWard.Tube.Camera;

public class TestSceneManager : RJWard.Core.Singleton.SingletonApplicationLifetime<TestSceneManager>
{
	static private readonly bool DEBUG_LOCAL = false;

	public SpineCamera mainCamera;
	//	public TubeTester tubeTester;

	public Transform pos1 = null;
	public Transform pos2 = null;
	public int num = 5;
	public float startRadius = 5f;
	public float endRadius = 8f;

	public int delaySecs = 10;

	public int numHoopPoints = 10;

	public Material tubeWallMaterial;

	public Transform testTubeContainer;

	public TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

	private Tube tube_ = null;

	public RJWard.Tube.Player.Player player;

	// Use this for initialization
	void Start( )
	{
		TubeFactory.Instance.tubeWallMaterial = tubeWallMaterial;
		GameObject tubeGO = new GameObject( "Tube" );
		tubeGO.transform.position = Vector3.zero;
		tubeGO.transform.localScale = Vector3.one;
		tube_ = tubeGO.AddComponent<Tube>( );

		RJWard.Tube.UI.UIManager.Instance.SetCameraToViewport( mainCamera.GetComponent<Camera>( ) );

	}

	public void NewFromSources( )
	{
		TubeFactory.Instance.CreateFromSourcesInContainer( testTubeContainer, numHoopPoints, tubeWallMaterial, HandleTubeSectionMade );
	}

	public void ClearTube( )
	{
		tube_.DeleteAllSections( );
	}

	private void HandleTubeSectionMade( TubeSection_Linear ts )
	{
		tube_.AddToEnd( ts );
	}

	public SpinePoint_Base GetFirstSpinePoint( )
	{
		SpinePoint_Base result = null;
		if (tube_ != null)
		{
			result = tube_.FirstSpinePoint( );
		}
		return result;
	}

	public void CreateRandomSection( )
	{
		//			Debug.Log( "Randomising" );
		randLinearSectionDefn.firstHoop = null;
		if (tube_ != null)
		{
			Hoop lastHoop = tube_.LastHoop( );
			if (lastHoop != null)
			{
				randLinearSectionDefn.firstHoop = lastHoop.ExplicitDefinition( );
			}
		}
		TubeFactory.Instance.CreateRandomLinearSection( randLinearSectionDefn, HandleTubeSectionMade );
	}

	public Transform cameraHook;

	private bool cameraOnHook_ = true;

	private Vector3 originalPosition_ = Vector3.zero;
	private Quaternion originalRotation_ = Quaternion.identity;

	public void SetCameraOffHook()
	{
		cameraOnHook_ = false;
	}

	protected override void PostAwake( )
	{
		originalPosition_ = mainCamera.transform.position;
		originalRotation_ = mainCamera.transform.rotation;
		cameraOnHook_ = true;
	}

	public void EndPlayMode()
	{
		player.gameObject.SetActive( false );
		mainCamera.gameObject.SetActive( true );
	}

	public void StartPlayMode()
	{
		player.gameObject.SetActive( true );
		mainCamera.gameObject.SetActive( false );
	}

	public void StartPlayer()
	{
		SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
		if (firstSpinePoint != null)
		{
			player.InitialiseAt( firstSpinePoint.transform );
		}
		else
		{
			Debug.LogWarning( "Can't start player with no tube" );
		}
	}

	public float forwardPowerMult = 1f;
	private float forwardPower_ = 0f;

	private void FixedUpdate()
	{
		if (forwardPower_ != 0f)
		{
			player.body.AddForce( player.cachedTransform.forward * forwardPower_ * Time.deltaTime, ForceMode.Impulse );
		}
	}

	public void PlayerForwardDown()
	{
		if (player. currentFlowZone == null)
		{
			SetCameraToFirstSpinePoint( );
		}
		forwardPower_ = forwardPowerMult;
	}

	public void PlayerForwardUp( )
	{
		forwardPower_ = 0f;
	}

	public void ToggleExtCamOnHook()
	{
		if (cameraOnHook_)
		{
			SetCameraToFirstSpinePoint( );
		}
		else
		{
			SetCameraOnHook( );
		}
	}

	public void SetCameraOnHook()
	{
		mainCamera.enabled = false;
		mainCamera.transform.position = originalPosition_;
		mainCamera.transform.rotation = originalRotation_;
		cameraOnHook_ = true;
	}

	public void SetCameraToFirstSpinePoint()
	{
		SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
		if (firstSpinePoint != null )
		{
			mainCamera.enabled = true;
			mainCamera.InitStationary( firstSpinePoint, 0f );
			cameraOnHook_ = false;
			if (DEBUG_LOCAL)
			{
				Debug.Log( "Set camera to first spine point = " + firstSpinePoint.gameObject.name );
			}
		}
		else
		{
			Debug.LogWarning( "Didn't find first spine point" );
		}
	}

	public void HandleCameraForwardDown()
	{
//		Debug.Log( "Forward" );
		mainCamera.accelerate( );
	}

	public void HandleCameraBackDown( )
	{
//		Debug.Log( "Back" );
		mainCamera.decelerate( );
	}

	public void HandleCameraMotionButtonUp( )
	{
//		Debug.Log( "Stop" );
		mainCamera.killPower( );
	}

	public void HandleCameraStopPressed()
	{
		mainCamera.stop( );
	}

	public void HandleDebugButtonPressed()
	{
		mainCamera.toggleDebugObjects( );
	}

	public void HandleRandomTubeSectionButtonPressed( )
	{
//		Debug.Log( "Randomising" );
		CreateRandomSection( );
	}

	public void HandleSourcesButtonPressed()
	{
		NewFromSources( );
	}

	public void HandleClearButtonPressed()
	{
		ClearTube( );
	}
}

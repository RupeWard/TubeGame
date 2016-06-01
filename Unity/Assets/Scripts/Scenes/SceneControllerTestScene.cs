using UnityEngine;
using System.Collections;
using RJWard.Tube;
using RJWard.Tube.Camera;

public class SceneControllerTestScene : SceneController_Base
{
	override public SceneManager.EScene Scene( )
	{
		return SceneManager.EScene.TestScene;
	}

	static private readonly bool DEBUG_LOCAL = false;

	public SpineCamera spineCamera;
	public Camera playerCamera;

	//	public TubeTester tubeTester;

	public Transform pos1 = null;
	public Transform pos2 = null;
	public int num = 5;
	public float startRadius = 5f;
	public float endRadius = 8f;

	public float FlowZone_defaultWeight = 1f;
	public float FlowZone_defaultSpeed = 1f;

	public float flowZoneConvexAdjust = 0.2f;
	public int delaySecs = 10;

	public int numHoopPoints = 10;

	public Material tubeWallMaterial;

	public Transform testTubeContainer;

	public TubeFactory.RandLinearSectionDefn randLinearSectionDefn;

	private Tube tube_ = null;

	public RJWard.Tube.Player.Player player;

	static private SceneControllerTestScene instance_ = null;
	static public SceneControllerTestScene Instance
	{
		get { return instance_;  }
	}

	public void OnDestroy()
	{
		instance_ = null;
	}

	// Use this for initialization
	void Start( )
	{
		TubeFactory.Instance.tubeWallMaterial = tubeWallMaterial;
		GameObject tubeGO = new GameObject( "Tube" );
		tubeGO.transform.position = Vector3.zero;
		tubeGO.transform.localScale = Vector3.one;
		tube_ = tubeGO.AddComponent<Tube>( );

		RJWard.Tube.UI.TestSceneUIManager.Instance.SetCameraToViewport( spineCamera.GetComponent<Camera>( ) );

	}

	public void NewFromSources( )
	{
		TubeFactory.Instance.CreateFromSourcesInContainer( tube_ , testTubeContainer, numHoopPoints, tubeWallMaterial, HandleTubeSectionMade );
	}

	public void ClearTube( )
	{
		tube_.DeleteAllSections( );
	}

	private void HandleTubeSectionMade( TubeSection_Linear ts )
	{
		tube_.AddToEnd( ts, null );
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
//		Debug.Log( "Randomising" );
		randLinearSectionDefn.firstHoop = null;
		if (tube_ != null)
		{
			Hoop lastHoop = tube_.LastHoop( );
			if (lastHoop != null)
			{
				randLinearSectionDefn.firstHoop = lastHoop.ExplicitDefinition( );
			}
		}
		TubeFactory.Instance.CreateRandomLinearSection( tube_, randLinearSectionDefn, HandleTubeSectionMade );
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
		if (instance_ != null)
		{
			Debug.LogError( "Second TestSCeneController" );
		}
		else
		{
			instance_ = this;
		}
		originalPosition_ = spineCamera.transform.position;
		originalRotation_ = spineCamera.transform.rotation;
		cameraOnHook_ = true;
	}

	public void EndPlayMode()
	{
		player.gameObject.SetActive( false );
		spineCamera.gameObject.SetActive( true );
	}

	public void StartPlayMode()
	{
		if (tube_.FirstSection() == null)
		{
			CreateRandomSection( );
		}
		player.gameObject.SetActive( true );
		spineCamera.gameObject.SetActive( false );
	}

	public void StartPlayer()
	{
		SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
		if (firstSpinePoint != null)
		{
			player.InitialiseAt( firstSpinePoint.transform );
			MessageBus.instance.dispatchPlayerRestarted( );
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
		spineCamera.enabled = false;
		spineCamera.transform.position = originalPosition_;
		spineCamera.transform.rotation = originalRotation_;
		cameraOnHook_ = true;
	}

	public void SetCameraToFirstSpinePoint()
	{
		SpinePoint_Base firstSpinePoint = GetFirstSpinePoint( );
		if (firstSpinePoint != null )
		{
			spineCamera.enabled = true;
			spineCamera.InitStationary( firstSpinePoint, 0f );
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
		spineCamera.accelerate( );
	}

	public void HandleCameraBackDown( )
	{
//		Debug.Log( "Back" );
		spineCamera.decelerate( );
	}

	public void HandleCameraMotionButtonUp( )
	{
//		Debug.Log( "Stop" );
		spineCamera.killPower( );
	}

	public void HandleCameraStopPressed()
	{
		spineCamera.stop( );
	}

	public void HandleDebugButtonPressed()
	{
		if (spineCamera != null)
		{
			DebugManager.ToggleDebugObjects( spineCamera.myCamera );
		}

		if (playerCamera != null)
		{
			DebugManager.ToggleDebugObjects( playerCamera );
		}
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

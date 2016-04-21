using UnityEngine;
using System.Collections;
using RJWard.Tube;
using RJWard.Tube.Camera;

public class TestSceneManager : RJWard.Core.Singleton.SingletonApplicationLifetime<TestSceneManager>
{
	static private readonly bool DEBUG_LOCAL = false;

	public SpineCamera mainCamera;
	public TubeTester tubeTester;

	public Transform cameraHook;

	private bool cameraOnHook_ = true;

	private Vector3 originalPosition_ = Vector3.zero;
	private Quaternion originalRotation_ = Quaternion.identity;

	public TubeFactory.RandLinearSectionDefn randLinearSectionDefn = new TubeFactory.RandLinearSectionDefn( );

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

	public void ToggleCamera()
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
		SpinePoint_Base firstSpinePoint = tubeTester.GetFirstSpinePoint( );
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

	public void HandleDebugButtonPressed()
	{
		mainCamera.toggleDebugObjects( );
	}

	public void HandleRandomTubeSectionButtonPressed( )
	{
//		Debug.Log( "Randomising" );
		tubeTester.CreateRandomSection( );
	}

	public void HandleSourcesButtonPressed()
	{
		tubeTester.NewFromSources( );
	}

	public void HandleClearButtonPressed()
	{
		tubeTester.ClearTube( );
	}
}

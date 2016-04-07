using UnityEngine;
using System.Collections;
using RJWard.Tube;
using RJWard.Tube.Camera;

public class TestSceneManager : RJWard.Core.Singleton.SingletonApplicationLifetime<TestSceneManager>
{
	public SpineCamera mainCamera;
	public TubeTester tubeTester;

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
		SpinePoint[] spinePoints = Transform.FindObjectsOfType<SpinePoint>( );
		if (spinePoints != null && spinePoints.Length > 0)
		{
			SpinePoint spFound = null;
			for (int i = 0; i < spinePoints.Length; i++)
			{
				spFound = spinePoints[i];
				if (spinePoints[i].previousSpinePoint == null)
				{
					break;
				}
			}
			mainCamera.enabled = true;
			mainCamera.InitStationary( spFound, 0f );
			cameraOnHook_ = false;
			Debug.Log( "Set to first spine point = " + spFound.gameObject.name );
		}
		else
		{
			Debug.LogWarning( "Didn't find any spine points" );
		}

	}

	public void HandleCameraForwardDown()
	{
		Debug.Log( "Forward" );
		mainCamera.accelerate( );
	}

	public void HandleCameraBackDown( )
	{
		Debug.Log( "Back" );
		mainCamera.decelerate( );
	}

	public void HandleCameraMotionButtonUp( )
	{
		Debug.Log( "Stop" );
		mainCamera.killPower( );
	}

	public void HandleDebugButtonPressed()
	{
		mainCamera.toggleDebugObjects( );
	}

	public void HandleAppendTubeSectionButtonPressed()
	{
		tubeTester.DuplicateSection( );
	}
}

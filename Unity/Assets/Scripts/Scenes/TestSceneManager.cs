using UnityEngine;
using System.Collections;
using RJWard.Tube;
using RJWard.Tube.Camera;

public class TestSceneManager : RJWard.Core.Singleton.SingletonApplicationLifetime<TestSceneManager>
{

	public SpineCamera mainCamera;

	public Transform cameraHook;

	public bool cameraOnHook = true;

	private Vector3 originalPosition_ = Vector3.zero;
	private Quaternion originalRotation_ = Quaternion.identity;

	protected override void PostAwake( )
	{
		originalPosition_ = mainCamera.transform.position;
		originalRotation_ = mainCamera.transform.rotation;
		cameraOnHook = true;
	}

	public void ToggleCamera()
	{
		if (cameraOnHook)
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
		cameraOnHook = true;
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
			mainCamera.Init( spFound, 0f );
			cameraOnHook = false;
			Debug.Log( "Set to first spine point = " + spFound.gameObject.name );
		}
		else
		{
			Debug.LogWarning( "Didn't find any spine points" );
		}

	}
}

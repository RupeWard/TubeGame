using UnityEngine;
using System.Collections;

namespace RJWard.Tube.UI
{
	public class UIManager : RJWard.Core.Singleton.SingletonApplicationLifetime<UIManager>
	{
		public RectTransform permanentPanel;
		public UnityEngine.UI.Text cameraButtonText;
		public UnityEngine.UI.Text versionText;

		protected override void PostAwake( )
		{
			versionText.text = RJWard.Core.Version.versionNumber.DebugDescribe( );
		}

		public void HandleCameraButton()
		{
			TestSceneManager.Instance.ToggleCamera( );
			
		}

		public void HandleDebugButton()
		{
			TestSceneManager.Instance.HandleDebugButtonPressed( );
		}

		public void HandleForwardButtonDown()
		{
			TestSceneManager.Instance.HandleCameraForwardDown( );
		}

		public void HandleForwardButtonUp( )
		{
			TestSceneManager.Instance.HandleCameraMotionButtonUp( );
		}

		public void HandleBackButtonDown( )
		{
			TestSceneManager.Instance.HandleCameraBackDown( );
		}

		public void HandleBackButtonUp( )
		{
			TestSceneManager.Instance.HandleCameraMotionButtonUp( );
		}

		public void HandleStopButton()
		{
			TestSceneManager.Instance.HandleCameraStopPressed( );
		}
	}

}

using UnityEngine;
using System.Collections;

namespace RJWard.Tube.UI
{
	public class UIManager : RJWard.Core.Singleton.SingletonApplicationLifetime<UIManager>
	{
		public RectTransform permanentPanel;
		public UnityEngine.UI.Text cameraButtonText;

		public void HandleCameraButton()
		{
			TestSceneManager.Instance.ToggleCamera( );
			
		}

		public void HandleForwardButtonDown()
		{
			TestSceneManager.Instance.HandleCameraForwardDown( );
		}

		public void HandleForwardButtonUp( )
		{

		}

		public void HandleBackButtonDown( )
		{
			TestSceneManager.Instance.HandleCameraBackDown( );
		}

		public void HandleBackButtonUp( )
		{

		}
	}

}

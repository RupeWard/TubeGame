using UnityEngine;
using System.Collections;

public class SceneControllerDevSetup: SceneController_Base 
{
	#region inspector hooks

	public UnityEngine.UI.Text versionText;

	#endregion inspector hooks

	#region event handlers

	public void HandleConstGameButtonPressed( )
	{
		HandleGameButtonPressed( GameManager.GameType.Constant );
	}

	public void HandleSequenceGameButtonPressed( )
	{
		HandleGameButtonPressed( GameManager.GameType.Sequence );
	}

	private void HandleGameButtonPressed(GameManager.GameType gt)
	{
		GameManager.staticGameType = gt;
		SceneManager.Instance.SwitchScene( SceneManager.EScene.Game );
	}

	public void HandleTestButtonPressed( )
	{
		SceneManager.Instance.SwitchScene( SceneManager.EScene.TestScene);
	}

	public void HandleQuitButtonPressed( )
	{
		Application.Quit();
	}

	#endregion event handlers

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.DevSetup;
	}

	override protected void PostStart()
	{
		versionText.text = RJWard.Core.Version.versionNumber.DebugDescribe( );
	}

	override protected void PostAwake()
	{
		GameManager.staticGameType = GameManager.GameType.Unknown;
	}

#endregion SceneController_Base

}

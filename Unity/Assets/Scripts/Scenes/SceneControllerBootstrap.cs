using UnityEngine;
using System.Collections;

public class SceneControllerBootstrap : SceneController_Base 
{
#region inspector hooks

	public UnityEngine.UI.Text versionText;
	public float delay = 5f;

#endregion inspector hooks

	private void MoveOn()
	{
		SceneManager.Instance.SwitchScene( SceneManager.EScene.TestScene );
	}

#region event handlers


#endregion event handlers

#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Bootstrap;
	}

	override protected void PostStart()
	{
		Application.targetFrameRate = 60;

		versionText.text = RJWard.Core.Version.versionNumber.DebugDescribe ();

		Invoke( "MoveOn", delay );
	}

#endregion SceneController_Base

}

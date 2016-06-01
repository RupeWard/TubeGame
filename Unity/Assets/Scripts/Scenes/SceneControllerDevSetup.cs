using UnityEngine;
using System.Collections;

public class SceneControllerDevSetup: SceneController_Base 
{
#region inspector hooks

#endregion inspector hooks

#region event handlers
	
#endregion event handlers

#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.DevSetup;
	}

	override protected void PostStart()
	{

	}

	override protected void PostAwake()
	{
	}

#endregion SceneController_Base

}

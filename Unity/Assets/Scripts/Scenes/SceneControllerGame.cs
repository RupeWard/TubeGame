using UnityEngine;
using System.Collections;

public class SceneControllerGame : SceneController_Base 
{
//	static private readonly bool DEBUG_LOCAL = false;

	#region inspector hooks

	public Camera playerCamera;
	public float FlowZone_defaultWeight = 1f;
	public float FlowZone_defaultSpeed = 1f;

	public int numHoopPoints = 10;

	#endregion inspector hooks

	#region event handlers

	#endregion event handlers

	#region SceneController_Base

	override public SceneManager.EScene Scene ()
	{
		return SceneManager.EScene.Game;
	}

	override protected void PostStart()
	{
	}

	override protected void PostAwake()
	{
	}

#endregion SceneController_Base

}

public partial class MessageBus : MonoBehaviour
{
	public System.Action toggleDebugObjectsAction;
	public void dispatchToggleDebugObjects( )
	{
		if (toggleDebugObjectsAction != null)
		{
			toggleDebugObjectsAction( );
		}
	}
}


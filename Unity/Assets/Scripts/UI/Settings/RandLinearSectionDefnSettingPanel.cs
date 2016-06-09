using UnityEngine;
using System.Collections;

public class RandLinearSectionDefnSettingPanel : MonoBehaviour
{
	#region inspector hooks

	public UnityEngine.UI.Text titleText;
	public UnityEngine.UI.Text messageText;

	public FloatSettingPanel floatSettingPanel;
	public Vector2SettingPanel vector2SettingPanel;
	public IntSettingPanel intSettingPanel;

	public UnityEngine.UI.Text numSectionsButtonText;

	#endregion inspector hooks

	#region inspector data

	#endregion inspector data

	#region actions

	public System.Action< RJWard.Tube.RandLinearSectionDefn > onValueChangedAction;
	
	#endregion actions

	#region private data

	private RJWard.Tube.RandLinearSectionDefn oldSetting_;
	private RJWard.Tube.RandLinearSectionDefn currentSetting_;

	#endregion private data

	private void Awake()
	{
		gameObject.SetActive( false );
	}

	public void Init( string title, RJWard.Tube.RandLinearSectionDefn current, System.Action<RJWard.Tube.RandLinearSectionDefn> changeAction)
	{
		titleText.text = title;
		oldSetting_ = new RJWard.Tube.RandLinearSectionDefn( current);
		currentSetting_ = new RJWard.Tube.RandLinearSectionDefn( current );

		onValueChangedAction = changeAction;
	
		messageText.text = string.Empty;
		messageText.gameObject.SetActive( false );

		SetValue( currentSetting_ );

		gameObject.SetActive( true );
	}

	private void SetValue( RJWard.Tube.RandLinearSectionDefn defn)
	{
		numSectionsButtonText.text = "Num Sections: " + defn.numSections;
	}

	#region handlers

	public void OnNumSectionsButtonClicked()
	{
		intSettingPanel.Init( "Num Sections", currentSetting_.numSections, new int[] { 0, int.MaxValue }, OnNumSectionsChanged );
	}

	public void OnNumSectionsChanged(int i)
	{
		currentSetting_.numSections = i;
		SetValue( currentSetting_ );
		onValueChangedAction( currentSetting_);
	}

	private void DoChangeAction()
	{
		if (onValueChangedAction != null)
		{
			onValueChangedAction( currentSetting_ );
		}
	}

	public void onDoneButtonClicked()
	{
		gameObject.SetActive( false );
	}

	public void onResetButtonClicked( )
	{
		currentSetting_ = oldSetting_;
		SetValue( oldSetting_ );
		if (onValueChangedAction != null)
		{
			onValueChangedAction( oldSetting_ );
		}
	}

	#endregion handlers

	#region factory

	static public RandLinearSectionDefnSettingPanel CreateFromRefab()
	{
		return Resources.Load<GameObject>( "Prefabs/UI/RandLinearSectionDefnSettingPanel" ).GetComponent<RandLinearSectionDefnSettingPanel>( );
	}
	#endregion factory

}

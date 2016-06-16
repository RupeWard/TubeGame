using UnityEngine;
using System.Collections;

public class GameSettingsPanel : MonoBehaviour
{
	public UnityEngine.UI.Text gameMultiplierButtonText;
	public UnityEngine.UI.Text titleText;
	public UnityEngine.UI.Text messageText;

	public IntSettingPanel intSettingPanel;

	private int originalGameMultiplierValue_;
	 
	public void Init()
	{
		originalGameMultiplierValue_ = GameManager.Instance.gameMultiplier;
		SetGameMultiplierText( );
		titleText.text = "Game Options";
		gameObject.SetActive( true );
	}

	private void SetGameMultiplierText()
	{
		gameMultiplierButtonText.text = "Game Mult: " + GameManager.Instance.gameMultiplier.ToString();
	}

	public void HandleDoneButton()
	{
		gameObject.SetActive( false );
	}

	public void HandleResetButton()
	{
		GameManager.Instance.gameMultiplier = originalGameMultiplierValue_;
		SetGameMultiplierText( );
	}

	public void HandleGameMultiplierButton()
	{
		intSettingPanel.Init( "Game Mutliplier", GameManager.Instance.gameMultiplier, new int[] { 1, 10 }, OnGameMultiplierChanged);
	}

	public void OnGameMultiplierChanged( int i )
	{
		/*
		if (GameManager.Instance.isPlaying)
		{
			Debug.LogError( "Can't change while playing" );
		}
		else
		*/
		{
			SettingsStore.storeSetting( SettingsIds.gameMultiplierSettingId, i );
			GameManager.Instance.gameMultiplier = i;
			SetGameMultiplierText( );
		}
	}

}

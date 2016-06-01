﻿using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	[RequireComponent (typeof(UnityEngine.Camera))]
	[RequireComponent( typeof( Rigidbody ) )]
	[RequireComponent( typeof( ConfigurableJoint) )]
	public class TetheredCamera : MonoBehaviour
	{
		private Transform cachedTransform_ = null;
		public Transform cachedTransform
		{
			get { return cachedTransform_;  }
		}

		private Rigidbody cachedRigidbody_ = null;
		public Rigidbody cachedRigidbody
		{
			get { return cachedRigidbody_; }
		}

		private UnityEngine.Camera cachedCamera_ = null;
		public UnityEngine.Camera cachedCamera
		{
			get { return cachedCamera_; }
		}


		private void Awake()
		{
			cachedTransform_ = transform;
			cachedRigidbody_ = GetComponent<Rigidbody>( );
			cachedCamera_ = GetComponent<UnityEngine.Camera>( );

			int debugObjectsSetting = PlayerPrefs.GetInt( showDebugObjectsSettingId );
			RJWard.Tube.DebugManager.SetShowDebugObjects( cachedCamera_, debugObjectsSetting == 1 );

			MessageBus.instance.toggleDebugObjectsAction += ToggleDebugObjects;
		}

		static readonly string showDebugObjectsSettingId = "showDebugObjects";

		private void ToggleDebugObjects()
		{
			RJWard.Tube.DebugManager.ToggleDebugObjects( cachedCamera_);
			bool isShowing = (cachedCamera_.cullingMask & DebugManager.debugObjectsLayer) != 0;
			PlayerPrefs.SetInt( showDebugObjectsSettingId, (isShowing)?(1):(0) );
			PlayerPrefs.Save( );
		}
	}

}

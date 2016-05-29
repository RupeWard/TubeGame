using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class CamTether : MonoBehaviour
	{
		public Player player;
		public float targetDistance = 5f;

		private Transform camTransform_;
		public UnityEngine.Camera tetheredCamera;

        private Transform cachedTransform_;

		private Vector3 directionVector_ = Vector3.zero;

		private void Awake()
		{
			cachedTransform_ = transform;
			camTransform_ = tetheredCamera.transform;

		}

		private void Start()
		{
			RJWard.Tube.UI.UIManager.Instance.SetCameraToViewport( tetheredCamera );
		}

		void Update( )
		{
			if (player != null && player.isActiveAndEnabled)
			{
				player.UpdateDirection( ref directionVector_ );
				cachedTransform_.position = player.cachedTransform.position + directionVector_ * targetDistance;

				if (camTransform_ != null )
				{
					camTransform_.LookAt( player.cachedTransform.position );
				}
			}
		}
	}

}

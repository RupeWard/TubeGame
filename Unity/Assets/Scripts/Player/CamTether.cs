using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class CamTether : MonoBehaviour
	{
		public Player player;
		public float targetDistance = 5f;

		private Transform camTransform_;
		private Rigidbody camRB_;

		public UnityEngine.Camera tetheredCamera;

        private Transform cachedTransform_;

		private Vector3 directionVector_ = Vector3.zero;

		private void Awake()
		{
			cachedTransform_ = transform;
			camTransform_ = tetheredCamera.transform;
			camRB_ = tetheredCamera.GetComponent<Rigidbody>( );
			MessageBus.instance.onPlayerRestarted += HandlePlayerRestart;
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

		public float repulseForce = 1f;
		public float minDistanceForForce = 3f;

		void FixedUpdate()
		{
			if (player != null && player.isActiveAndEnabled)
			{
				float dist = Vector3.Distance( player.cachedTransform.position, camTransform_.position );
				if (dist < (targetDistance - minDistanceForForce))
				{
					float forceFraction = dist / (targetDistance - minDistanceForForce);
					camRB_.AddForce( forceFraction * repulseForce * Time.fixedDeltaTime * (camTransform_.position - player.cachedTransform.position).normalized, ForceMode.Impulse);
					Debug.Log( "Adding force" );
				}
			}
		}

		public void HandlePlayerRestart()
		{
			if (player != null)
			{
				player.UpdateDirection( ref directionVector_ );
				cachedTransform_.position = player.cachedTransform.position + directionVector_ * targetDistance;
				if (camTransform_ != null)
				{
					camTransform_.position = cachedTransform_.position;
					camTransform_.LookAt( player.cachedTransform.position );
				}
			}
		}
	}

}

using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	[RequireComponent (typeof (Rigidbody))]
	public class CamTether : MonoBehaviour
	{
		private Player player_;
		public float targetDistance = 5f;

		private TetheredCamera tetheredCamera_ = null;

		private Transform cachedTransform_;

		private Vector3 directionVector_ = Vector3.zero;

		private void Awake()
		{
//			Debug.Log( "CamTether Awake" );
			cachedTransform_ = transform;
			MessageBus.instance.onPlayerRestarted += HandlePlayerRestart;
		}

		public void Init(Player p, TetheredCamera tc)
		{
			player_ = p;
			tetheredCamera_ = tc;
			cachedTransform_.position = player_.cachedTransform.position;
			cachedTransform_.rotation = player_.cachedTransform.rotation;

			tetheredCamera_.GetComponent<ConfigurableJoint>( ).connectedBody = GetComponent<Rigidbody>();

			GameManager.Instance.SetCameraToViewport( tetheredCamera_.cachedCamera );
		}

		void Update( )
		{
			if (player_ != null && player_.isActiveAndEnabled)
			{
				player_.UpdateDirection( ref directionVector_ );
				cachedTransform_.position = player_.cachedTransform.position + directionVector_ * targetDistance;

				if (tetheredCamera_.cachedTransform != null )
				{
					tetheredCamera_.cachedTransform.LookAt( player_.cachedTransform.position );
				}
			}
		}

		public float repulseForce = 1f;
		public float maxShortfallForForce = 3f;

		void FixedUpdate()
		{
			if (player_ != null && player_.isActiveAndEnabled)
			{
				float dist = Vector3.Distance( player_.cachedTransform.position, tetheredCamera_.cachedTransform.position );
				float maxDistance = targetDistance - maxShortfallForForce; // OPT
                if (dist < maxDistance)
				{
					Vector3 v = tetheredCamera_.cachedRigidbody.velocity;
					float newPosD = Vector3.Distance( player_.cachedTransform.position, tetheredCamera_.cachedTransform.position + v * Time.fixedDeltaTime);
					if (newPosD < dist)
					{
						float forceFraction = dist / maxDistance;
						tetheredCamera_.cachedRigidbody.AddForce( forceFraction * repulseForce * Time.fixedDeltaTime * (tetheredCamera_.cachedTransform.position - player_.cachedTransform.position).normalized, ForceMode.Impulse );
						//		Debug.Log( "Adding force" );
					}
				}
			}
		}

		public void HandlePlayerRestart()
		{
			if (player_ != null)
			{
				player_.UpdateDirection( ref directionVector_ );
				cachedTransform_.position = player_.cachedTransform.position + directionVector_ * targetDistance;
				if (tetheredCamera_.cachedTransform != null)
				{
					tetheredCamera_.cachedTransform.position = cachedTransform_.position;
					tetheredCamera_.cachedTransform.LookAt( player_.cachedTransform.position );
				}
			}
		}
	}

}

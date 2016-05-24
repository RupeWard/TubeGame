using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class PlayerCam	 : MonoBehaviour
	{
		private Vector3 direction_ = Vector3.back;
		private UnityEngine.Camera camera_ = null;

		public Transform cachedTransform;
		private Player player_;

		public Transform camTarget;

		public float distanceFromPlayer = 5f;

//		private bool isDirty_ = false;

		private void Awake( )
		{
			cachedTransform = transform;
			camera_ = GetComponent<UnityEngine.Camera>( );
			RJWard.Tube.UI.UIManager.Instance.SetCameraToViewport( camera_);

		}

		public void Init(Player p)
		{
			player_ = p;
		}

		public void toggleDebugObjects( )
		{
			int debugObjectsLayer = 1 << LayerMask.NameToLayer( "DebugObjects" );
			bool isShowing = (camera_.cullingMask & debugObjectsLayer) != 0;
			showDebugObjects( !isShowing );
		}

		public void showDebugObjects( bool show )
		{
			int debugObjectsLayer = 1 << LayerMask.NameToLayer( "DebugObjects" );
			if (show)
			{
				camera_.cullingMask = camera_.cullingMask | debugObjectsLayer;
			}
			else
			{
				camera_.cullingMask = camera_.cullingMask & ~debugObjectsLayer;
			}
		}


		/*
		public void SetCamTarget(Vector3 pos)
		{
			camTarget.position = pos;
			isDirty_ = true;
		}*/

		public float camSpeed = 1f;

//		public float camDistFromObjMultipler = 10f;
		public float minDistFromObj = 1f;

//		private Vector3 currentForceDirection_ = Vector3.zero;
//		private float currentForce_ = 0f;
//		public float wallForce = 2f;
//		public float wallForceReduction = 1f;

//		private static readonly bool DEBUG_FORCE = true;

		public bool tmpCheckUsingRaycast = true;

		private Vector3 lastRelativePosition_ = Vector3.zero;

		private void FixedUpdate()
		{
			player_.UpdateDirection( ref direction_ );
			Vector3 desiredPosition = player_.cachedTransform.position + direction_ * distanceFromPlayer;

			Ray playerLookRay = new Ray( player_.cachedTransform.position, desiredPosition - player_.cachedTransform.position );
			RaycastHit playerLookHitInfo;

			bool playerLookHit = Physics.Raycast( playerLookRay, out playerLookHitInfo, Vector3.Distance(player_.cachedTransform.position, desiredPosition) );
			if (playerLookHit && playerLookHitInfo.collider.gameObject.layer != FlowZone_Linear.FLOWZONELAYER)
			{
				Debug.Log( "Hit " + playerLookHitInfo.collider.gameObject.name );
			}
			else
			{
				cachedTransform.position = Vector3.MoveTowards( cachedTransform.position, desiredPosition, camSpeed * Time.fixedDeltaTime );
			}

			/*
			float moveDist = camSpeed * Time.deltaTime;

			Vector3 forceAdjustment = Vector3.zero;
			if (currentForce_ > 0f)
			{
				forceAdjustment = currentForce_ * currentForceDirection_;
                cachedTransform.position = cachedTransform.position + forceAdjustment * Time.fixedDeltaTime;

				currentForce_ -= wallForceReduction * Time.fixedDeltaTime;
				if (currentForce_ < 0f)
				{
					currentForce_ = 0f;
					if (DEBUG_FORCE)
					{
						Debug.Log( "Force depleted" );
					}
				}
			}
			if (tmpCheckUsingRaycast)
			{
				Vector3 moveDirection = (player_.cachedTransform.position + direction_ * distanceFromPlayer) - cachedTransform.position;
                Ray lookRay = new Ray( cachedTransform.position, moveDirection );

				RaycastHit hitInfo;

				float lookDist = Mathf.Max( camDistFromObjMultipler * moveDist, minDistFromObj );
				bool rayCastHit = Physics.Raycast( lookRay, out hitInfo, lookDist );

				if (rayCastHit && hitInfo.collider.gameObject.layer != FlowZone_Linear.FLOWZONELAYER)
				{
					currentForceDirection_ = Vector3.Cross( direction_, moveDirection);
					if (currentForce_ == 0f)
					{						
						currentForce_ = wallForce;
						if (DEBUG_FORCE)
						{
							Debug.Log( "Applying force" );
						}
					}
					else
					{
						if (DEBUG_FORCE)
						{
							Debug.Log( "reapplying force" );
						}
					}
				
				{
						float dist = Vector3.Distance( cachedTransform.position, player_.cachedTransform.position );
						if (dist > distanceFromPlayer)
						{
							float distToTest = Mathf.Min( camSpeed * Time.deltaTime, dist );
							distToTest = Mathf.Max( distToTest, camera_.nearClipPlane );

							cachedTransform.position = Vector3.MoveTowards( cachedTransform.position,
								player_.cachedTransform.position,
								distToTest
								);
							Debug.LogWarning( "against T" + hitInfo.collider.gameObject.name );
						}
						else if (dist < distanceFromPlayer)
						{
							cachedTransform.position = Vector3.MoveTowards( cachedTransform.position,
								player_.cachedTransform.position,
								-1f * Mathf.Min( camSpeed * Time.deltaTime, dist ) );
							Debug.LogWarning( "against A " + hitInfo.collider.gameObject.name );
						}
						else
						{
							Debug.LogWarning( "against 0 " + hitInfo.collider.gameObject.name );
						}
					}
				}
				else
				{
					cachedTransform.position = Vector3.MoveTowards( cachedTransform.position,
						player_.cachedTransform.position + direction_ * distanceFromPlayer,
						camSpeed * Time.fixedDeltaTime );
				}
			}
			else
			{
				cachedTransform.position = Vector3.MoveTowards( cachedTransform.position,
					player_.cachedTransform.position + direction_ * distanceFromPlayer,
					camSpeed * Time.fixedDeltaTime );
			}
			*/
			cachedTransform.LookAt( player_.cachedTransform );

			/*
			//			if (isDirty_)
			{
				Vector3 direction = (camTarget.position - player_.cachedTransform.position).normalized;
				cachedTransform.position = player_.cachedTransform.position - direction * distanceFromPlayer;
				cachedTransform.LookAt( camTarget );
				isDirty_ = false;
			}
			*/
		}
		static readonly bool DEBUG_COLLISIONS = true;

		private void OnCollisionEnter( Collision collision )
		{
			if (DEBUG_COLLISIONS)
			{
				Debug.Log( "COLLISION ENTER: " + gameObject.name + " " + collision.gameObject.name );
			}
		}

		private void OnCollisionExit( Collision collision )
		{
			if (DEBUG_COLLISIONS)
			{
				Debug.Log( "COLLISION EXIT: " + gameObject.name + " " + collision.gameObject.name );
			}
		}

	}
}

using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class Player : MonoBehaviour
	{
		static readonly bool DEBUG_COLLISIONS = false;
		static readonly bool DEBUG_WALLCOLLISIONS = false;

		private Transform cachedTransform_;
		private Rigidbody body_;
		private AudioSource audioSource_ = null;

		private FlowZone_Linear currentFlowZone_ = null;
		public FlowZone_Linear currentFlowZone
		{
			get { return currentFlowZone_; }
		}

		public float speed;

		private bool shouldLogNoSpeedExcess_ = true;

		public static readonly bool DEBUG_FORCE = false;

		bool isBallRolling_ = false;

		public void HandleBallRolling( bool on )
		{
			if (on != isBallRolling_)
			{
				if (on)
				{
					audioSource_.loop = true;
					audioSource_.clip = AudioManager.Instance.rollingBallClip;
					audioSource_.Play( );
					isBallRolling_ = true;
				}
				else
				{
					audioSource_.Stop( );
					isBallRolling_ = false;
				}
			}
		}

		private void FixedUpdate()
		{
			if (currentFlowZone_ != null)
			{
				float speedExcess = currentFlowZone_.speed - body_.velocity.magnitude;
				if (speedExcess > 0)
				{
					shouldLogNoSpeedExcess_ = true;
                    body.AddForce( speed * speedExcess * currentFlowZone_.flowAtPosition( cachedTransform.position ) * Time.deltaTime, ForceMode.Impulse );
				}
				else
				{
					if (shouldLogNoSpeedExcess_)
					{
						if (DEBUG_FORCE)
						{
							Debug.Log( "Stopped adding force" );
						}
					}
					shouldLogNoSpeedExcess_ = false;
				}
			}
		}

		public bool UpdateDirection(ref Vector3 dir)
		{
			bool success = false;
			if (currentFlowZone_ != null)
			{
				dir = -1f * currentFlowZone_.directionAtPosition( cachedTransform.position );
			}
			return success;
		}

		public Rigidbody body
		{
			get { return body_; }
		}

		public Transform cachedTransform
		{
			get { return cachedTransform_; }
		}

		private void Awake()
		{
			cachedTransform_ = transform;
			body_ = GetComponent<Rigidbody>( );
			audioSource_ = GetComponent<AudioSource>( );
		}

		public float camTargetDistance = 5f;

		public void Update()
		{
		}

		public void InitialiseAt(Transform t)
		{
			cachedTransform_.position = t.position;
			cachedTransform_.rotation = t.rotation;

			body_.velocity = Vector3.zero;
		}

		public int maxSpinePointsToGap = 10;

		private bool shouldExtend = false;

		private void OnTriggerEnter( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
//				Debug.Log( "TRIGGER ENTER: " + gameObject.name + " " + other.gameObject.name );
			}
			
			FlowZone_Linear newFz = other.gameObject.GetComponent<FlowZone_Linear>( );
			if (newFz != null)
			{
				if (newFz != currentFlowZone_)
				{
					currentFlowZone_ = newFz;

					SpinePoint_Linear spinePoint = currentFlowZone_.firstSpinePoint;
					int minToGap = spinePoint.MinSpinePointsToEnd( );
					if (DEBUG_COLLISIONS)
					{
						Debug.Log( "TRIGGER ENTER " + gameObject.name + " in " + other.gameObject.name + " with " + minToGap + " to end " + " from spine point " + spinePoint.DebugDescribe( ) + "' from FZ " + currentFlowZone_ + " with dirn = " + newFz.directionVector );
					}	
                    if (minToGap > maxSpinePointsToGap)
					{
						shouldExtend = true;
					}
					else
					{
						if (shouldExtend)
						{
							spinePoint.spine.tubeSection.HandlePlayerEnterSection( );
							shouldExtend = false;
						}
					}

				}
				else
				{
					Debug.LogWarning( "TRIGGER ENTER PLAYER: '" + gameObject.name + "' to SAME flow zone '" + other.gameObject.name + "' from " + currentFlowZone_.gameObject.name );
				}
			}
		}

		private void OnTriggerExit( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
				FlowZone_Linear fz = other.gameObject.GetComponent<FlowZone_Linear>( );
				if (fz != null)
				{
					if (fz == currentFlowZone_)
					{
						currentFlowZone_ = null;
					}
				}
				else
				{
					Debug.LogWarning( "TRIGGER EXIT: " + gameObject.name + " flowzone '" + other.gameObject.name+"' when currently in fz '"+currentFlowZone_.gameObject.name+"'" );
				}
			}
		}

		private void OnCollisionEnter( Collision collision )
		{
			if (collision.gameObject.layer == TubeFactory.Instance.tubeWallLayerMask)
			{
				if (DEBUG_COLLISIONS || DEBUG_WALLCOLLISIONS)
				{
					Debug.Log( "HIT WALL: " + gameObject.name + " " + collision.gameObject.name );
				}
				HandleBallRolling( true );
			}
			else
			{
			//	if (DEBUG_COLLISIONS)
				{
					Debug.Log( "HIT : " + gameObject.name + " " + collision.gameObject.name );
				}
			}
		}

		private void OnCollisionExit( Collision collision )
		{
			if (collision.gameObject.layer == TubeFactory.Instance.tubeWallLayerMask)
			{
				if (DEBUG_COLLISIONS || DEBUG_WALLCOLLISIONS)
				{
					Debug.Log( "LEFT WALL: " + gameObject.name + " " + collision.gameObject.name );
				}
				HandleBallRolling( false );
			}
			else
			{
		//		if (DEBUG_COLLISIONS)
				{
					Debug.Log( "LEFT : " + gameObject.name + " " + collision.gameObject.name );
				}
			}
		}

	}


}

public partial class MessageBus : MonoBehaviour
{
	public System.Action onPlayerRestarted;
	public void dispatchPlayerRestarted()
	{
		if (onPlayerRestarted != null)
		{
			onPlayerRestarted( );
		}
	}
}


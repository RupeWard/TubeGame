using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class Player : MonoBehaviour
	{
		static readonly bool DEBUG_COLLISIONS = false;
		static readonly bool DEBUG_WALLCOLLISIONS = false;
		public static readonly bool DEBUG_FORCE = false;
		private static readonly bool DEBUG_SPARKS = false;

		#region inspector hooks

		public ControlPointer3D controlPointer;

		#endregion inspector hooks

		#region prefabs

		public GameObject camTetherPrefab;
		public GameObject tetheredCameraPrefab;
		public GameObject sparksPrefab;

		#endregion prefabs

		#region inspector data

		public float speed;

		public float sparkColourShiftDuration = 2f;

		public float rollingSoundStopDelay = 0.2f;

		// Sparks - TODO move sparks stuff to own class?

		public Color sparkColourLow = Color.yellow;
		public Color sparkColourHigh = Color.red;

		public float camTargetDistance = 5f;

		public float sparkStartSize = 0.2f;
		public float sparkEndSize = 0.7f;
		public float sparksStopTime = 0.5f;

		public Vector2 controlMarkerRange = new Vector2( 0.25f, 1f );

		public int maxSpinePointsToGap = 10;

		#endregion inspector data

		#region private hooks

		private float sparkStartTime = -1f;
		private float tillSparksStop = -1f;

		private Transform cachedTransform_;
		public Transform cachedTransform
		{
			get { return cachedTransform_; }
		}

		private Rigidbody body_;
		public Rigidbody body
		{
			get { return body_; }
		}

		private AudioSource audioSource_ = null;

		private ParticleSystem sparks_ = null;
		private CamTether camTether_ = null;
		private TetheredCamera tetheredCamera_ = null;

		#endregion private hooks

		#region private data 

		private FlowZone_LinearBase currentFlowZone_ = null;
		public FlowZone_LinearBase currentFlowZone
		{
			get { return currentFlowZone_; }
		}

		private bool shouldLogNoSpeedExcess_ = true;

		private bool isBallRolling_ = false;

		private float tillRollingSoundStop = -1f;

		private bool showControlMovementMarker_ = true;

		private Vector3 currentForce_ = Vector3.zero;
		private Vector3 currentControlForce_ = Vector3.zero;
		private Vector3 currentFlowForce_ = Vector3.zero;


		#endregion private data 

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
					tillRollingSoundStop = rollingSoundStopDelay;
					isBallRolling_ = false;
					tillSparksStop = sparksStopTime;
				}
			}
		}

		public void StartGame(Tube t)
		{
			InitialiseAt( t.FirstSpinePoint( ).transform );
			camTether_.Init(this, tetheredCamera_ );
			running = true;
		}

		public void GetReadyToStart()
		{
			InitialiseAtStart( );
			camTether_.Init( this, tetheredCamera_ );
		}

		public void ToggleShowControlMarker()
		{
			showControlMovementMarker_ = !showControlMovementMarker_;
			SettingsStore.storeSetting( SettingsIds.showControlForceMarkerSettingId, showControlMovementMarker_ );
		}

		private void FixedUpdate()
		{
			if (running)
			{
				currentForce_ = Vector3.zero;
				currentFlowForce_ = Vector3.zero;
				currentControlForce_ = Vector3.zero;

				if (currentFlowZone_ != null)
				{
					float speedExcess = currentFlowZone_.speed - body_.velocity.magnitude;
					if (speedExcess > 0)
					{
						shouldLogNoSpeedExcess_ = true;
						currentFlowForce_ = speed * speedExcess * currentFlowZone_.flowAtPosition( cachedTransform.position );
						currentForce_ += currentFlowForce_;
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
				Vector2 controlVector = GameManager.Instance.currentControlForce;
				if (controlVector.magnitude > 0f)
				{
					Vector3 controlVector3D = new Vector3( controlVector.x, controlVector.y, 0f );
					currentControlForce_ = tetheredCamera_.cachedTransform.TransformDirection( controlVector3D );
					currentForce_ += currentControlForce_;
				}
				if (currentForce_.sqrMagnitude > 0f)
				{
					body.AddForce( currentForce_ * Time.deltaTime, ForceMode.Impulse );
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

		private bool running = false;

		private void Awake()
		{
			Debug.Log( "Player.Awake()" );
			cachedTransform_ = transform;
			body_ = GetComponent<Rigidbody>( );
			audioSource_ = GetComponent<AudioSource>( );

			if (camTether_ == null)
			{
				camTether_ = GameObject.Instantiate<GameObject>( camTetherPrefab ).GetComponent<CamTether>( );
				camTether_.gameObject.SetActive( true );
			}
			if (tetheredCamera_ == null)
			{
				tetheredCamera_ = GameObject.Instantiate<GameObject>( tetheredCameraPrefab ).GetComponent<TetheredCamera>( );
				tetheredCamera_.gameObject.SetActive( true );
			}
			if (sparks_ == null)
			{
				sparks_ = GameObject.Instantiate<GameObject>( sparksPrefab ).GetComponent<ParticleSystem>( );
				sparks_.gameObject.SetActive( true );
			}
//			showDebugObjects_ = SettingsStore.retrieveSetting<bool>( SettingsIds.showDebugObjectsSettingId );
			showControlMovementMarker_ = SettingsStore.retrieveSetting<bool>( SettingsIds.showControlForceMarkerSettingId);

			if (controlPointer != null)
			{
				controlPointer.Init( this );
			}

			MessageBus.instance.toggleControlMarkersAction += ToggleShowControlMarker;
		}

		public void Update()
		{
			if (running)
			{
				if (sparks_.isPlaying)
				{
					float elapsed = Time.time - sparkStartTime;
					float fraction = (elapsed > sparkColourShiftDuration) ? (1f) : (elapsed / sparkColourShiftDuration);
					sparks_.startColor = Color.Lerp( sparkColourLow, sparkColourHigh, fraction );
					sparks_.startSize = Mathf.Lerp( sparkStartSize, sparkEndSize, fraction );
				}
				if (tillSparksStop > 0f)
				{
					tillSparksStop -= Time.deltaTime;
					if (tillSparksStop <= 0f)
					{
						if (DEBUG_SPARKS)
						{
							Debug.Log( "Stopped sparks" );
						}
						sparks_.Stop( );
					}
				}
				if (tillRollingSoundStop > 0f)
				{
					tillRollingSoundStop -= Time.deltaTime;
					if (tillRollingSoundStop <= 0f)
					{
						audioSource_.Stop( );
					}
				}
				if (controlPointer != null)
				{
					if (showControlMovementMarker_)
					{
						if (currentControlForce_.sqrMagnitude > 0f)
						{
							Vector3 direction = currentControlForce_.normalized;
							float distance = controlMarkerRange.x + Mathf.Lerp( 0, controlMarkerRange.y - controlMarkerRange.x, currentControlForce_.magnitude );

							Vector3 pos = cachedTransform_.position - direction * distance;

							controlPointer.UpdatePosition( pos );
							controlPointer.gameObject.SetActive( true );
						}
						else
						{
							controlPointer.gameObject.SetActive( false );
						}
					}
					else
					{
						controlPointer.gameObject.SetActive( false );
					}
				}

			}
		}

		public void InitialiseAtStart( )
		{
			cachedTransform_.position = Vector3.zero;
			cachedTransform_.rotation = Quaternion.identity;

			body_.velocity = Vector3.zero;
		}

		public void InitialiseAt(Transform t)
		{
			cachedTransform_.position = t.position;
			cachedTransform_.rotation = t.rotation;

			body_.velocity = Vector3.zero;
		}

		private void OnTriggerEnter( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
//				Debug.Log( "TRIGGER ENTER: " + gameObject.name + " " + other.gameObject.name );
			}

			FlowZone_LinearBase newFz = other.gameObject.GetComponent<FlowZone_LinearBase>( );
			if (newFz != null)
			{
				if (newFz != currentFlowZone_)
				{
					currentFlowZone_ = newFz;
					if (running)
					{
						SpinePoint_Linear spinePoint = currentFlowZone_.firstSpinePoint;
						SpinePoint_Base endSpinePoint = null;
						int minToGap = spinePoint.MinSpinePointsToEnd( ref endSpinePoint );
						if (DEBUG_COLLISIONS)
						{
							Debug.Log( "TRIGGER ENTER " + gameObject.name + " in " + other.gameObject.name + " with " + minToGap + " to end " + " from spine point " + spinePoint.DebugDescribe( ) + "' from FZ " + currentFlowZone_ + " with dirn = " + newFz.directionVector );
						}
						if (minToGap < maxSpinePointsToGap && !endSpinePoint.spine.tubeSection.isExtending)
						{
							endSpinePoint.spine.tubeSection.HandlePlayerEnterSection( );
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
				FlowZone_LinearBase fz = other.gameObject.GetComponent<FlowZone_LinearBase>( );
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

		private void OnCollisionStay( Collision collision)
		{
			if (!running)
			{
				return;
			}
			if (collision.gameObject.layer == TubeFactory.Instance.tubeWallLayerMask)
			{
				if (DEBUG_COLLISIONS || DEBUG_WALLCOLLISIONS)
				{
					Debug.Log( "STAY WALL: " + gameObject.name + " " + collision.gameObject.name );
				}
				if (collision.contacts.Length > 0)
				{
					sparks_.gameObject.transform.position = collision.contacts[0].point;
					sparks_.gameObject.transform.LookAt( cachedTransform_.position );
					if (sparks_.isPlaying)
					{
						float elapsed = Time.time - sparkStartTime;
						tillSparksStop = -1f;

						if (DEBUG_SPARKS)
						{
							Debug.Log( "Stay wall when sparks already playing" );
						}
						float fraction = (elapsed > sparkColourShiftDuration) ? (1f) : (elapsed / sparkColourShiftDuration);
						sparks_.startColor = Color.Lerp( sparkColourLow, sparkColourHigh, fraction );
						sparks_.startSize = Mathf.Lerp( sparkStartSize, sparkEndSize, fraction );
						sparks_.Play( );
					}
					else
					{
						Debug.LogWarning( "Why aren't sparks playing already?" );
						//						sparks.startColor = sparkColourLow;
						sparks_.Play( );
						sparkStartTime = Time.time;
						tillSparksStop = -1f;
						sparks_.startColor = sparkColourLow;
						sparks_.startSize = sparkStartSize;
						if (DEBUG_SPARKS)
						{
							Debug.Log( "Stay wall, starting sparks" );
						}
					}
				}
				else
				{
					Debug.LogWarning( "No contacts!" );
				}
				HandleBallRolling( true );
			}
			else
			{
				if (DEBUG_COLLISIONS)
				{
					Debug.Log( "STAY : " + gameObject.name + " " + collision.gameObject.name );
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
				if (collision.contacts.Length > 0)
				{
					sparks_.gameObject.transform.position = collision.contacts[0].point;
					sparks_.gameObject.transform.LookAt( cachedTransform_.position );
					if (sparks_.isPlaying)
					{
						sparkStartTime = Time.time;
						float elapsed = Time.time - sparkStartTime;
						tillSparksStop = -1f;

						if (DEBUG_SPARKS)
						{
							Debug.Log( "Hit wall when sparks already playing" );
						}

						float fraction = (elapsed > sparkColourShiftDuration) ? (1f) : (elapsed / sparkColourShiftDuration);
						sparks_.startColor = Color.Lerp( sparkColourLow, sparkColourHigh, fraction );
						sparks_.startSize = Mathf.Lerp( sparkStartSize, sparkEndSize, fraction );
						sparks_.Play( );
					}
					else
					{
						//						sparks.startColor = sparkColourLow;
						sparks_.Play( );                      
						sparkStartTime = Time.time;
						tillSparksStop = -1f;
						sparks_.startColor = sparkColourLow;
						sparks_.startSize = sparkStartSize;
						if (DEBUG_SPARKS)
						{
							Debug.Log( "Hit wall, starting sparks" );
						}
					}

				}
				else
				{
					Debug.LogWarning( "No contacts!" );
				}
				HandleBallRolling( true );
			}
			else
			{
				if (DEBUG_COLLISIONS)
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
				if (sparks_.isPlaying)
				{
					if (DEBUG_SPARKS)
					{
						Debug.Log( "Leave wall when sparks playing" );
					}
				}
				HandleBallRolling( false );
			}
			else
			{
				if (DEBUG_COLLISIONS)
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


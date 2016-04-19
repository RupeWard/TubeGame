using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Camera
{
	[RequireComponent (typeof(UnityEngine.Camera))]
	public class SpineCamera : MonoBehaviour
	{
		private Transform cachedTransform_ = null;

		public float camSpeedMult = 1f;
		public float camAcc = 1f;
		public float camMaxSpeed = 1f;
		public float camDrag = 1f;

		private SpinePoint_Simple spinePoint_ = null;

#if UNITY_EDITOR
		public SpinePoint_Simple modSpinePoint = null;
#endif
		private float currentSpeed = 0f;
		private float currentAcc = 0f;

		private UnityEngine.Camera camera_;

		public void stop()
		{
			currentSpeed = 0f;
			currentAcc = 0f;
		}

		public void accelerate()
		{
			currentAcc = camAcc; ;
		}

		public void decelerate( )
		{
			currentAcc = -1f * camAcc;
		}

		public void killPower()
		{
			currentAcc = 0f;
		}

		public void showDebugObjects(bool show)
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

		public void toggleDebugObjects(  )
		{
			int debugObjectsLayer = 1 << LayerMask.NameToLayer( "DebugObjects" );
			bool isShowing = (camera_.cullingMask & debugObjectsLayer) != 0;
			showDebugObjects( !isShowing );
		}

		void Update()
		{
			if (modSpinePoint != spinePoint_)
			{
				SpinePoint_Simple pointToMod = modSpinePoint;
				modSpinePoint = null;
				Debug.Log( "Spine point changed" );
				InitStationary( pointToMod, 0f );
				TestSceneManager.Instance.SetCameraOffHook();
			}

			if (Mathf.Abs(currentAcc) > Mathf.Epsilon)
			{
				currentSpeed += currentAcc * Time.deltaTime;
				if (currentSpeed > camMaxSpeed)
				{
					currentSpeed = camMaxSpeed;
//					Debug.Log( "Camera speed maxed at " + currentSpeed );
					currentAcc = 0f;
				}
				else
				{
//					Debug.Log( "Camera speed changed to " + currentSpeed );
				}
			}
			else if (Mathf.Abs(currentSpeed) > Mathf.Epsilon)
			{
				float currentSpeedSign = Mathf.Sign( currentSpeed );
				currentSpeed = currentSpeedSign * (Mathf.Abs( currentSpeed ) - camDrag);
				if (currentSpeedSign != Mathf.Sign(currentSpeed))
				{
					currentSpeed = 0f;
//					Debug.Log( "Cam slowed to halt" );
				}
			}

			if (Mathf.Abs(currentSpeed) > Mathf.Epsilon)
			{
//				Debug.Log( "Moving at speed " + currentSpeed );
				float newT = t_ + currentSpeed * Time.deltaTime;
				if (newT < 0f)
				{
					if (lastSpinePoint_.previousSpinePoint != null)
					{
						Init( lastSpinePoint_.previousSpinePoint, newT + 1 );
					}
					else
					{
						stop( );
//						Debug.Log( "Can't move, at start" );
					}
				}
				else if (newT < 1f)
				{
					Init( lastSpinePoint_, newT );
				}
				else
				{
					int numJumps = 0;
					SpinePoint_Simple foundSpinePoint = lastSpinePoint_;
					while (newT >= 1f)
					{
						newT -= 1f;
						if (foundSpinePoint.nextSpinePoint == null)
						{
							stop( );
							newT = 1f;
//							Debug.Log( "Can't move, at end" );
						}
						else
						{
							foundSpinePoint = foundSpinePoint.nextSpinePoint;
							numJumps++;
						}
					}
					if (numJumps > 0)
					{
						Debug.LogWarning( "NumJumps = " + numJumps + " fsp= "+foundSpinePoint.gameObject.name );
					//	Init( foundSpinePoint, newT );
					}

					if (newT < 1f)
					{
						Init( foundSpinePoint, newT );
					}
				}
			}
		}

		void Awake()
		{
			cachedTransform_ = transform;
			camera_ = GetComponent<UnityEngine.Camera>( );
		}

		private RJWard.Tube.SpinePoint_Simple lastSpinePoint_ = null;
		private float t_ = 0f;

		public void InitStationary( SpinePoint_Base sp, float t )
		{
			stop( );
			Init( sp, t );
		}

		public void Init(SpinePoint_Base spb, float t)
		{
			// TODO this all needs refactoring for more generic spinepoints

			SpinePoint_Simple sp = spb as SpinePoint_Simple;
			if (sp == null)
			{
				throw new System.InvalidOperationException( "Non spinepointsimple not yet implemented" );
			}

			lastSpinePoint_ = sp;
			t_ = t;

			if (lastSpinePoint_.nextSpinePoint == null)
			{
				if (lastSpinePoint_.previousSpinePoint != null)
				{
					Debug.LogWarning( "Initing on preultimate point" );
					InitStationary( lastSpinePoint_.previousSpinePoint, 0.9f );
				}
				else
				{
					Debug.LogError( "Can't init on only one point" );
				}
			}
			else
			{
				Vector3 pos = Vector3.zero;
				if (lastSpinePoint_.InterpolateForwardWorld(t, ref pos))
				{
					transform.position = pos;

					Vector3 lookAtPos = Vector3.zero;
					float tdiff = 0.01f;
					float newT = t + tdiff;
					if (newT <= 1f)
					{
						if (lastSpinePoint_.InterpolateForwardWorld( (newT), ref lookAtPos ))
						{
							if (Vector3.Distance( pos, lookAtPos ) > 0.001f)
							{
								transform.LookAt( lookAtPos );
							}
							else
							{
								Debug.LogWarning( "Failed to set camera position" );
							}
						}
						else
						{
							Debug.LogError( "Stopped" );
							stop( );
						}
					}
				}
				else
				{
					Debug.LogError( "Can't init camera" );
				}

			}
		}
	}

}

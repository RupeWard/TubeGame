using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Camera
{
	public class SpineCamera : MonoBehaviour
	{
		private Transform cachedTransform_ = null;

		public float camSpeedMult = 1f;
		public float camAcc = 1f;
		public float camMaxSpeed = 1f;
		public float camDrag = 1f;

		private SpinePoint spinePoint_ = null;

#if UNITY_EDITOR
		public SpinePoint modSpinePoint = null;
#endif
		private float currentSpeed = 0f;
		private float currentAcc = 0f;

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

		void Update()
		{
			if (modSpinePoint != spinePoint_)
			{
				SpinePoint pointToMod = modSpinePoint;
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
					Debug.Log( "Camera speed maxed at " + currentSpeed );
					currentAcc = 0f;
				}
				else
				{
					Debug.Log( "Camera speed changed to " + currentSpeed );
				}
			}
			else if (Mathf.Abs(currentSpeed) > Mathf.Epsilon)
			{
				float currentSpeedSign = Mathf.Sign( currentSpeed );
				currentSpeed = currentSpeedSign * (Mathf.Abs( currentSpeed ) - camDrag);
				if (currentSpeedSign != Mathf.Sign(currentSpeed))
				{
					currentSpeed = 0f;
					Debug.Log( "Cam slowed to halt" );
				}
			}

			if (Mathf.Abs(currentSpeed) > Mathf.Epsilon)
			{
				Debug.Log( "Moving at speed " + currentSpeed );
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
						Debug.Log( "Can't move, at start" );
					}
				}
				else if (newT < 1f)
				{
					Init( lastSpinePoint_, newT );
				}
				else
				{
					if (lastSpinePoint_.nextSpinePoint != null)
					{
						Init( lastSpinePoint_.nextSpinePoint, newT - 1f );
					}
					else
					{
						stop( );
						Debug.Log( "Can't move, at end" );
					}
				}
			}
		}

		void Awake()
		{
			cachedTransform_ = transform;
		}

		private RJWard.Tube.SpinePoint lastSpinePoint_ = null;
		private float t_ = 0f;

		public void InitStationary( SpinePoint sp, float t )
		{
			stop( );
			Init( sp, t );
		}

		public void Init(SpinePoint sp, float t)
		{
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
					if (lastSpinePoint_.InterpolateForwardWorld((t+0.1f), ref lookAtPos))
					{
						if (Vector3.Distance(pos, lookAtPos) > 0.001f)
						{
							transform.LookAt( lookAtPos );
						}
						else
						{
							Debug.LogWarning( "Failed to set camera position" );
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

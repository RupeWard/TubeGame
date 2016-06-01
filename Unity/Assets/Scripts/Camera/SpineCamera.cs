using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Camera
{
	[RequireComponent (typeof(UnityEngine.Camera))]
	public class SpineCamera : MonoBehaviour
	{
		static private readonly bool DEBUG_LOCAL = false;

		private Transform cachedTransform_ = null;

		public float camSpeedMult = 1f;
		public float camAcc = 1f;
		public float camMaxSpeed = 1f;
		public float camDrag = 1f;

		private SpinePoint_Linear spinePoint_ = null;

#if UNITY_EDITOR
		public SpinePoint_Linear modSpinePoint = null;
#endif
		private float currentSpeed = 0f;
		private float currentAcc = 0f;

		private UnityEngine.Camera camera_;
		public UnityEngine.Camera myCamera
		{
			get { return camera_; }
		}

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

		/*
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
		*/

		void Update()
		{
#if UNITY_EDITOR
			if (modSpinePoint != spinePoint_)
			{
				SpinePoint_Linear pointToMod = modSpinePoint;
				modSpinePoint = null;
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Spine point changed in editor" );
				}
				InitStationary( pointToMod, 0f );
				SceneControllerTestScene.Instance.SetCameraOffHook();
			}
#endif
			if (Mathf.Abs(currentAcc) > Mathf.Epsilon)
			{
				currentSpeed += currentAcc * Time.deltaTime;
				if (currentSpeed > camMaxSpeed)
				{
					currentSpeed = camMaxSpeed;
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Camera speed maxed at " + currentSpeed );
					}
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
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Cam slowed to halt" );
					}
				}
			}

			if (Mathf.Abs(currentSpeed) > Mathf.Epsilon)
			{
//				Debug.Log( "Moving at speed " + currentSpeed );
				float newT = t_ + currentSpeed *camSpeedMult * Time.deltaTime;
				if (newT < 0f)
				{
					if (lastSpinePoint_.previousSpinePoint != null)
					{
						if (DEBUG_LOCAL)
						{
//							Debug.Log( "Reverse, NewT=" + newT );
						}
						int numJumps = 0;
						SpinePoint_Linear foundSpinePoint = lastSpinePoint_;
						while (newT < 0f)
						{
							newT += 1f;
							if (foundSpinePoint.previousSpinePoint == null)
							{
								stop( );
								newT = 0f;
								if (DEBUG_LOCAL)
								{
									Debug.Log( "Can't move back, at start" );
								}
							}
							else
							{
								foundSpinePoint = foundSpinePoint.previousSpinePoint;
								numJumps++;
								if (DEBUG_LOCAL)
								{
									Debug.Log( "Cam Switching to previous spinepoint " + foundSpinePoint.gameObject.name + " (" + numJumps + " jumps" );
								}
							}
						}
						if (DEBUG_LOCAL && numJumps > 0)
						{
							Debug.LogWarning( "Rev NumJumps = " + numJumps + " fsp= " + foundSpinePoint.gameObject.name );
						}

						if (newT < 1f)
						{
							Init( foundSpinePoint, newT );
						}
						else
						{
							Debug.LogWarning( "Reverse NewT=" + newT );
						}
					}
					else
					{
						stop( );
						if (DEBUG_LOCAL)
						{
							Debug.Log( "Can't move back, at start" );
						}
					}
				}
				else if (newT < 1f)
				{
					Init( lastSpinePoint_, newT );
				}
				else
				{
					if (DEBUG_LOCAL)
					{
						Debug.Log( "Forward, NewT=" + newT );
					}
					int numJumps = 0;
					SpinePoint_Linear foundSpinePoint = lastSpinePoint_;
					while (newT >= 1f)
					{
						newT -= 1f;
						if (foundSpinePoint.nextSpinePoint == null)
						{
							stop( );
							newT = 0f;
							if (DEBUG_LOCAL)
							{
								Debug.Log( "Can't move fwd, at end" );
							}
						}
						else
						{
							foundSpinePoint = foundSpinePoint.nextSpinePoint;
							numJumps++;
							if (DEBUG_LOCAL)
							{
								Debug.Log( "Switching to next spinepoint " + foundSpinePoint.gameObject.name + " (" + numJumps + " jumps" );
							}
						}
					}
					if (DEBUG_LOCAL && numJumps > 0)
					{
						Debug.LogWarning( "FWD NumJumps = " + numJumps + " fsp= "+foundSpinePoint.gameObject.name );
					}

					if (newT < 1f)
					{
						Init( foundSpinePoint, newT );
					}
					else
					{
						Debug.LogWarning( "FWD NewT=" + newT );
					}
				}
			}
		}

		void Awake()
		{
			cachedTransform_ = transform;
			camera_ = GetComponent<UnityEngine.Camera>( );
		}

		private RJWard.Tube.SpinePoint_Linear lastSpinePoint_ = null;
		private float t_ = 0f;

		public void InitStationary( SpinePoint_Base sp, float t )
		{
			stop( );
			Init( sp, t );
		}

		public void Init(SpinePoint_Base spb, float t)
		{
			// TODO this all needs refactoring for more generic spinepoints

			SpinePoint_Linear sp = spb as SpinePoint_Linear;
			if (sp == null)
			{
				throw new System.InvalidOperationException( "Non spinepointsimple not yet implemented" );
			}

			if (lastSpinePoint_ != sp)
			{
				if (DEBUG_LOCAL)
				{
					Debug.Log( "Initing to new " + sp.gameObject.name );
				}
			}
			lastSpinePoint_ = sp;
			t_ = t;

			if (t>0f && lastSpinePoint_.nextSpinePoint == null)
			{
				if (DEBUG_LOCAL)
				{
					Debug.LogWarning( "No next point with t=" + t_ + ", clamping to zero" );
				}
				stop( );
				t_ = 0f;
			}

			{
				Vector3 pos = Vector3.zero;
				if (t < 0f || t > 1f)
				{
					Debug.LogError( "t=" + t );
				}
				if (lastSpinePoint_.InterpolateForwardWorld(null, t, ref pos))
				{
					cachedTransform_.position = pos;

					Vector3 lookAtPos = Vector3.zero;
					float tdiff = 0.01f;
					float newT = t + tdiff;
					if (newT > 1f)
					{
						newT = 1f;
					}
					if (newT <= 1f)
					{
						if (lastSpinePoint_.InterpolateForwardWorld(null, (newT), ref lookAtPos ))
						{
							if (Vector3.Distance( pos, lookAtPos ) > 0.001f)
							{
								cachedTransform_.LookAt( lookAtPos );
							}
							else
							{
								Debug.LogWarning( "Failed to set camera position" );
							}
						}
						else
						{
							Debug.LogWarning( "Stopped because failed to interpolate" );
							stop( );
						}
					}
					else
					{
						Debug.LogWarning( "Camera failed to lookAt" );
					}
				}
				else
				{
					stop( );
					t = 0f;
					Debug.LogWarning( "Stopping because Can't init camera" );
				}

			}
		}
	}

}

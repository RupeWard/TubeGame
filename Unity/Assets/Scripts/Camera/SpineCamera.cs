using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Camera
{
	public class SpineCamera : MonoBehaviour
	{
		private Transform cachedTransform_ = null;

		private SpinePoint spinePoint_ = null;

#if UNITY_EDITOR
		public SpinePoint modSpinePoint = null;
#endif

		void Update()
		{
			if (modSpinePoint != spinePoint_)
			{
				SpinePoint pointToMod = modSpinePoint;
				modSpinePoint = null;
				Debug.Log( "Spine point changed" );
				Init( pointToMod, 0f );
				TestSceneManager.Instance.cameraOnHook = false;
			}
		}

		void Awake()
		{
			cachedTransform_ = transform;
		}

		private RJWard.Tube.SpinePoint lastSpinePoint_ = null;
		private float t_ = 0f;

		public void Init(SpinePoint sp, float t)
		{
			lastSpinePoint_ = sp;
			t_ = t;

			if (lastSpinePoint_.nextSpinePoint == null)
			{
				if (lastSpinePoint_.previousSpinePoint != null)
				{
					Debug.LogWarning( "Initing on preultimate point" );
					Init( lastSpinePoint_.previousSpinePoint, 0.9f );
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

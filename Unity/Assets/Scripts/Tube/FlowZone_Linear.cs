using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class FlowZone_Linear : MonoBehaviour
	{
		static readonly bool DEBUG_COLLISIONS = false;

		private SpinePoint_Linear firstSpinePoint_;
		public SpinePoint_Linear firstSpinePoint
		{
			get { return firstSpinePoint_; }
		}

		private Vector3 directionVector_ = Vector3.zero;
		public Vector3 directionVector
		{
			get { return directionVector_; }
		}

		public Vector3 directionAtPosition(Vector3 pos)
		{
			Vector3 prevDirection = directionVector_;
			if (firstSpinePoint_.previousSpinePoint != null)
			{
				if (firstSpinePoint_.previousSpinePoint.flowZone != null)
				{
					prevDirection = firstSpinePoint_.previousSpinePoint.flowZone.directionVector_;
				}
				else
				{
					Debug.LogWarning( "No flow zone in " + firstSpinePoint_.previousSpinePoint.DebugDescribe( ) +
						"which is before " + firstSpinePoint_.DebugDescribe( ) );
				}
			}
			Vector3 nextDirection = directionVector_;
			if (firstSpinePoint_.nextSpinePoint != null)
			{
				if (firstSpinePoint_.nextSpinePoint.flowZone != null)
				{
					nextDirection = firstSpinePoint_.nextSpinePoint.flowZone.directionVector_;
				}
				else
				{
					Debug.LogWarning( "No flow zone in " + firstSpinePoint_.nextSpinePoint.DebugDescribe( ) +
						"which is next after " + firstSpinePoint_.DebugDescribe( ) );
				}
			}

			float d0 = Vector3.Distance( pos, firstSpinePoint_.cachedTransform.position );
			float d1 = Vector3.Distance( pos, firstSpinePoint_.nextSpinePoint.cachedTransform.position );
			float dtotal = d0 + d1;

			float dFraction = d0 / d1;

			float factor0 = 0f;
			float factor1 = 0f;
			float factor2 = 0f;

			if (dFraction < 0.5f)
			{
				factor0 = (0.5f - dFraction);
				factor1 = 0.5f + dFraction;
			}
			else if (dFraction > 0.5f)
			{
				factor1 = 0.5f + (1f - dFraction);
				factor2 = 0.5f - (1f - dFraction);
			}
			else
			{
				factor1 = 1f;
			}

			Vector3 result = factor0 * prevDirection
				+ factor1 * directionVector_
				+ factor2 * nextDirection;
			result = result.normalized;
			return result;
		}

		public void Init( SpinePoint_Linear sp)
		{
			firstSpinePoint_ = sp;
			directionVector_ = (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized;
		}

		private void OnTriggerEnter( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
				Debug.Log( "TRIGGER ENTER: " + gameObject.name + " " + other.gameObject.name );
			}
		}

		private void OnTriggerExit( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
				Debug.Log( "TRIGGER EXIT: " + gameObject.name + " " + other.gameObject.name );
			}
		}

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

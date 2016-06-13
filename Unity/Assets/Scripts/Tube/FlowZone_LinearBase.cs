using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public abstract class FlowZone_LinearBase : MonoBehaviour
	{
		static public readonly int FLOWZONELAYER = 12;

		static public readonly bool DEBUG_COLLISIONS = false;

		abstract public void Init( SpinePoint_Linear sp );

		private Transform cachedTransform_ = null;
		public Transform cachedTransform
		{
			get { return cachedTransform_; }
		}

		private void Awake()
		{
			cachedTransform_ = transform;
		}

		protected SpinePoint_Linear firstSpinePoint_;
		public SpinePoint_Linear firstSpinePoint
		{
			get { return firstSpinePoint_; }
		}

		//		private Vector3 directionVector_ = Vector3.zero;
		public Vector3 directionVector
		{
			get { return (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized; }
		}

		public float weight = 1f;
		public float speed = 1f;

		//		public void HandleDirectionChange()
		//		{
		//			directionVector_ = (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized;
		//		}

		public Vector3 flowAtPosition( Vector3 pos )
		{
			return weight * directionAtPosition( pos );
		}

		public Vector3 directionAtPosition( Vector3 pos )
		{
			Vector3 result = directionVector;

			float d0 = Vector3.Distance( pos, firstSpinePoint_.cachedTransform.position );
			float d1 = Vector3.Distance( pos, firstSpinePoint_.nextSpinePoint.cachedTransform.position );
			float dtotal = d0 + d1;

			float dFraction = d0 / dtotal;

			if (dFraction < 0.5f)
			{
				Vector3 prevDirection = directionVector;
				if (firstSpinePoint.previousSpinePoint != null)
				{
					if (firstSpinePoint_.previousSpinePoint.flowZone != null)
					{
						prevDirection = firstSpinePoint_.previousSpinePoint.flowZone.directionVector;
					}
					else
					{
						Debug.LogWarning( "No Prev flow zone in " + firstSpinePoint_.previousSpinePoint.DebugDescribe( ) +
							"which is before " + firstSpinePoint_.DebugDescribe( ) + "when working out flow zone direction" );
					}
				}
				else
				{
					//					Debug.LogWarning( "No previous spine point when working out flow zone direction" );
				}
				Vector3 firstDirection = Vector3.Slerp( directionVector, prevDirection, 0.5f );
				result = Vector3.Slerp( firstDirection, directionVector, dFraction / 0.5f );
			}
			else
			{
				Vector3 nextDirection = directionVector;
				if (firstSpinePoint_.nextSpinePoint != null)
				{
					if (firstSpinePoint_.nextSpinePoint.flowZone != null)
					{
						nextDirection = firstSpinePoint_.nextSpinePoint.flowZone.directionVector;
					}
					else
					{
						Debug.LogWarning( "No Next flow zone in " + firstSpinePoint_.nextSpinePoint.DebugDescribe( ) +
							"which is next after " + firstSpinePoint_.DebugDescribe( ) + "when working out flow zone direction" );
					}
				}
				else
				{
					Debug.LogWarning( "No next spine point when working out flow zone direction" );
				}
				Vector3 endDirection = Vector3.Slerp( directionVector, nextDirection, 0.5f );
				result = Vector3.Slerp( directionVector, endDirection, dFraction / 0.5f );

			}

			return result;

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


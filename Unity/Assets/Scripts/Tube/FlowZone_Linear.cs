using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class FlowZone_Linear : MonoBehaviour
	{
		static readonly bool DEBUG_COLLISIONS = true;

		private SpinePoint_Linear firstSpinePoint_;
		public SpinePoint_Linear firstSpinePoint
		{
			get { return firstSpinePoint_; }
		}

		public Vector3 directionVector = Vector3.zero;

		public void Init( SpinePoint_Linear sp)
		{
			firstSpinePoint_ = sp;
			directionVector = (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized;
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

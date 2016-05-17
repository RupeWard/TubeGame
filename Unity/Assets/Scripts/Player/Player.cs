using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class Player : MonoBehaviour
	{
		static readonly bool DEBUG_COLLISIONS = true;

		private Transform cachedTransform_;
		private Rigidbody body_;
		public PlayerCam cam;

		private FlowZone_Linear currentFlowZone = null;

		private void FixedUpdate()
		{
			if (currentFlowZone != null)
			{
				body.AddForce( currentFlowZone.directionVector * Time.deltaTime, ForceMode.Impulse );
			}
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
		}

		public void InitialiseAt(Transform t)
		{
			cachedTransform_.position = t.position;
			cachedTransform_.rotation = t.rotation;

			body_.velocity = Vector3.zero;

			cam.gameObject.SetActive( true );
		}

		private void OnTriggerEnter( Collider other )
		{
			if (DEBUG_COLLISIONS)
			{
				Debug.Log( "TRIGGER ENTER: " + gameObject.name + " " + other.gameObject.name );
			}
			FlowZone_Linear newFz = other.gameObject.GetComponent<FlowZone_Linear>( );
			if (newFz != null)
			{
				if (newFz != currentFlowZone)
				{
					if (DEBUG_COLLISIONS)
					{
						Debug.Log( "TRIGGER ENTER PLAYER: '" + gameObject.name + "' to new flow zone '" + other.gameObject.name +"' from "+currentFlowZone);
					}
					currentFlowZone = newFz;
				}
				else
				{
					Debug.LogWarning( "TRIGGER ENTER PLAYER: '" + gameObject.name + "' to SAME flow zone '" + other.gameObject.name + "' from " + currentFlowZone.gameObject.name );
				}
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

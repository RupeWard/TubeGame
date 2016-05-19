using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class PlayerCam : MonoBehaviour
	{
		private Vector3 direction_ = Vector3.back;

		public Transform cachedTransform;
		private Player player_;

		public Transform camTarget;

		public float distanceFromPlayer = 5f;

		private bool isDirty_ = false;

		private void Awake( )
		{
			cachedTransform = transform;	
		}

		public void Init(Player p)
		{
			player_ = p;
		}

		public void SetCamTarget(Vector3 pos)
		{
			camTarget.position = pos;
			isDirty_ = true;
		}

		public float camSpeed = 1f;

		private void Update()
		{
			player_.UpdateDirection( ref direction_ );
			cachedTransform.position = Vector3.MoveTowards( cachedTransform.position, 
				player_.cachedTransform.position + direction_ * distanceFromPlayer,
				camSpeed * Time.deltaTime);
			cachedTransform.LookAt( player_.cachedTransform );

			/*
			//			if (isDirty_)
			{
				Vector3 direction = (camTarget.position - player_.cachedTransform.position).normalized;
				cachedTransform.position = player_.cachedTransform.position - direction * distanceFromPlayer;
				cachedTransform.LookAt( camTarget );
				isDirty_ = false;
			}
			*/
		}
	}
}

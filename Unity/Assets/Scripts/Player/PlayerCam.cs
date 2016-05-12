using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class PlayerCam : MonoBehaviour
	{
		public Transform cachedTransform;

		private void Awake( )
		{
			cachedTransform = transform;
		}
	}
}

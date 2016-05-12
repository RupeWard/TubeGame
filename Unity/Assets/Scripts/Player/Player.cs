using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Player
{
	public class Player : MonoBehaviour
	{
		public Transform cachedTransform;

		private void Awake()
		{
			cachedTransform = transform;
		}
	}
}

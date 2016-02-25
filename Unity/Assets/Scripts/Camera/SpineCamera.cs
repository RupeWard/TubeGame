using UnityEngine;
using System.Collections;

namespace RJWard.Tube.Camera
{
	public class SpineCamera : MonoBehaviour
	{
		private Transform cachedTransform_ = null;

		void Awake()
		{
			cachedTransform_ = transform;
		}

	}

}

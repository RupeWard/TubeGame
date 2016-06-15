using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeEnd : MonoBehaviour
	{
		private Transform cachedTransform_ = null;

		private void Awake( )
		{
			cachedTransform_ = transform;
		}

		public void SetToHoop(Hoop h)
		{
			cachedTransform_.position = h.spinePoint.cachedTransform.position;
			cachedTransform_.localRotation = h.spinePoint.cachedTransform.rotation;
			float scale = h.GetMaxDistFromCentre( ) * 2.1f;
			cachedTransform_.localScale = new Vector3( scale, scale, 1f );
		}

		public void Init(Transform parent)
		{
			cachedTransform_.parent = transform;
			cachedTransform_.localScale = Vector3.zero;
		}
	}
}


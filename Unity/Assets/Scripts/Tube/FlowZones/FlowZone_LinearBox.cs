using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class FlowZone_LinearBox : FlowZone_LinearBase
	{
		static private GameObject s_prefab_ = null;
		static public FlowZone_LinearBox CreateFromPrefab()
		{
			if (s_prefab_ == null)
			{
				s_prefab_ = Resources.Load<GameObject>( "Prefabs/FlowZone_LinearBox" ) as GameObject;
			}
			GameObject go = Instantiate<GameObject>( s_prefab_ );
			go.layer = FlowZone_LinearBase.FLOWZONELAYER;
			return go.GetComponent<FlowZone_LinearBox>( );

		}

		public override void Init( SpinePoint_Linear sp)
		{
			gameObject.name = "FlowZone" + sp.gameObject.name;

			sp.flowZone = this;
			weight = GameManager.Instance.FlowZone_defaultWeight;
			speed = GameManager.Instance.FlowZone_defaultSpeed;

			firstSpinePoint_ = sp;
			//directionVector_ = (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized;

			float height = 1.5f * Vector3.Distance( firstSpinePoint_.cachedTransform.position, firstSpinePoint_.nextSpinePoint.cachedTransform.position );
			float radius = firstSpinePoint_.hoop.GetMaxDistFromCentre( );
			float scale = 2.1f * radius;
			cachedTransform.parent = firstSpinePoint_.spine.flowZonesContainer;
			cachedTransform.position = firstSpinePoint_.cachedTransform.position;
			cachedTransform.rotation = firstSpinePoint_.cachedTransform.rotation;
			cachedTransform.localScale = new Vector3( scale, scale, height );

		}
	}

}

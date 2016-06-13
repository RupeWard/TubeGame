using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class FlowZone_LinearMesh : FlowZone_LinearBase
	{
		public override void Init( SpinePoint_Linear sp)
		{
			sp.flowZone = this;
			weight = GameManager.Instance.FlowZone_defaultWeight;
			speed = GameManager.Instance.FlowZone_defaultSpeed;

			firstSpinePoint_ = sp;
			//directionVector_ = (firstSpinePoint_.nextSpinePoint.transform.position - firstSpinePoint_.transform.position).normalized;
		}

	}

}

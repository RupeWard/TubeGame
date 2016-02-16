using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class SpinePointSource : MonoBehaviour, RJWard.Core.IDebugDescribable
	{
		public bool fixRotation = false;

		public float radius
		{
			get { return transform.localScale.magnitude;  }
		}

		public void DebugDescribe( System.Text.StringBuilder sb)
		{
			sb.Append( "SPS ").Append(gameObject.name).Append(" @" ).Append( transform.position ).Append( " RO=" );
			if (fixRotation)
			{
				sb.Append( transform.rotation.eulerAngles );
			}
			else
			{
				sb.Append( "free" );
			}
			sb.Append( " RA=" ).Append( radius );
		}
	}
}

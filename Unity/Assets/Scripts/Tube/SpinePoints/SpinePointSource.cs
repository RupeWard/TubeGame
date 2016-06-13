using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class SpinePointSource : MonoBehaviour, RJWard.Core.IDebugDescribable
	{
		public bool fixRotation = false;

		private void Awake()
		{
			if (	transform.localScale.x != transform.localScale.y 
				||	transform.localScale.y != transform.localScale.z 
				||	transform.localScale.z != transform.localScale.z)
			{
				Debug.LogWarning( "SpinePointSource " + gameObject.name + " distorted with scale = " + transform.localScale );
			}
		}

		public float radius
		{
			get { return 0.5f * (transform.localScale.x + transform.localScale.y + transform.localScale.z) / 3f;  }
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

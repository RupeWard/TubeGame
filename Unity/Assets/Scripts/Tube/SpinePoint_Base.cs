using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class SpinePoint_Base : MonoBehaviour
	{
		protected Spine spine_ = null;

		public void SetDirty( )
		{
			spine_.SetDirty( );
		}

		abstract public bool isFirst( );
		abstract public bool isLast( );

	}



}

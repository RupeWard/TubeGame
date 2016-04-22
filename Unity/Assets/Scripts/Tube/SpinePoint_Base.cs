using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class SpinePoint_Base : MonoBehaviour, RJWard.Core.IDebugDescribable
	{
		protected Transform cachedTransform_ = null;
		public Transform cachedTransform {  get { return cachedTransform_; } }

		protected Spine spine_ = null;

		public void SetDirty( )
		{
			spine_.SetDirty( );
		}

		abstract public bool isFirst( );
		abstract public bool isLast( );

		public void Awake()
		{
			cachedTransform_ = transform;
		}

		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[SP" );
			DebugDescribeDetails( sb );
			sb.Append( "]" );
		}

		abstract protected void DebugDescribeDetails( System.Text.StringBuilder sb );
    }



}

using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class SpinePoint_Base : MonoBehaviour, RJWard.Core.IDebugDescribable
	{
		protected Transform cachedTransform_ = null;
		public Transform cachedTransform {  get { return cachedTransform_; } }

		protected Spine_Linear spine_ = null;
		public Spine_Linear spine
		{
			get { return spine_;  }
		}

		protected bool fixedRotation_ = false;
		public void fixRotation( bool b)
		{
			fixedRotation_ = true;
		}

		protected bool rotationIsDirty_ = false;
		public void SetRotationDirty( )
		{
			//			if (!fixedRotation_)
			{
				rotationIsDirty_ = true;
			}
			//			else
			//			{
			//				Debug.LogWarning( "Setting rotation dirty when fixed" );
			//			}
		}


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

		abstract public void DisconnnectFront( );

		abstract public SpinePointConnection GetConnectionOut( SpinePointPathChooser chooser );
		virtual public SpinePoint_Base GetSpinePointOut(SpinePointPathChooser chooser)
		{
			SpinePoint_Base result = null;
			SpinePointConnection connection = GetConnectionOut( chooser );
			if (connection != null)
			{
#if UNITY_EDITOR
				if (connection.startPoint != this)
				{
					Debug.LogError( "Dodgy start point on "+this.DebugDescribe() );
				}
#endif
				result = connection.endPoint;
			}
			return result;
		}

		abstract public SpinePointConnection GetConnectionIn( SpinePointPathChooser chooser );
		virtual public SpinePoint_Base GetSpinePointIn( SpinePointPathChooser chooser )
		{
			SpinePoint_Base result = null;
			SpinePointConnection connection = GetConnectionIn( chooser );
			if (connection != null)
			{
#if UNITY_EDITOR
				if (connection.endPoint != this)
				{
					Debug.LogError( "Dodgy end point on " + this.DebugDescribe( ) );
				}
#endif
				result = connection.startPoint;
			}
			return result;
		}


		public void DebugDescribe(System.Text.StringBuilder sb)
		{
			sb.Append( "[SP" );
			DebugDescribeDetails( sb );
			sb.Append( "]" );
		}

		abstract protected void DebugDescribeDetails( System.Text.StringBuilder sb );

		//		abstract public bool InterpolateForwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result );

		//		abstract public bool InterpolateBackwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result );


		virtual public bool InterpolateForwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result )
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
				//				return false;
			}
			t = Mathf.Clamp01( t );

			if (t == 0f)
			{
				result = cachedTransform_.position;
				success = true;
			}
			else
			{
				SpinePointConnection connection = GetConnectionOut( chooser );
				if (connection != null)
				{
					result = connection.InterpolatePosition( t );
					success = true;
				}
				else
				{
					Debug.LogWarning( "Spine_Linear pt has no out connection " + this.DebugDescribe( ) );
				}
			}
			return success;
		}

		virtual public bool InterpolateBackwardWorld( SpinePointPathChooser chooser, float t, ref Vector3 result )
		{
			bool success = false;
			result = Vector3.zero;

			if (t < 0f || t > 1f)
			{
				Debug.LogWarning( "t = " + t );
			}
			t = Mathf.Clamp01( t );

			if (t == 0)
			{
				result = cachedTransform_.position;
				success = true;
			}
			else
			{
				SpinePointConnection connection = GetConnectionIn( chooser );
				if (connection != null)
				{
					result = connection.InterpolatePosition( 1 - t );
					success = true;
				}
				else
				{
					Debug.LogWarning( "Spine_Linear pt has no in connection " + this.DebugDescribe( ) );
				}
			}
			return success;
		}

	}



}

using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class HoopPoint : MonoBehaviour
	{
		private int vertexNumber_ = int.MaxValue;
		private int altVertexNumber_ = int.MaxValue; // when being considered as last instead of first. Only differs from default for that one point in hoop
		
		public int vertexNumber
		{
			get { return vertexNumber_; }
			set
			{
#if UNITY_EDITOR
				if (vertexNumber_ != int.MaxValue)
				{
					if (vertexNumber != value)
					{
						Debug.LogWarning( "VertexNumber being changed from " + vertexNumber_ + " to " + value );
					}
				}
#endif
				vertexNumber_ = value;
			}
		}

		public int altVertexNumber
		{
			get { return altVertexNumber_; }
			set
			{
#if UNITY_EDITOR
				if (altVertexNumber_ != int.MaxValue)
				{
					if (altVertexNumber_ != value)
					{
						Debug.LogWarning( "AltVertexNumber being changed from " + altVertexNumber_ + " to " + value );
					}
				}
#endif
				altVertexNumber_ = value;
			}
		}

		public int getVertexNumber(bool useAlt)
		{
			return (useAlt) ? (altVertexNumber) : (vertexNumber);
		}

		public void LookAt(Transform t)
		{
			if (t != null)
			{
				transform.LookAt( t );
			}
			RJWard.Core.Test.DebugBlob debugBlob = transform.GetComponentInChildren<RJWard.Core.Test.DebugBlob>( );
			if (debugBlob != null)
			{
				debugBlob.ActivateDirectionPointer( t != null );
			}
		}
	}
}

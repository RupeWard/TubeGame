using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class HoopPoint : MonoBehaviour
	{
		private int vertexNumber_ = int.MaxValue;
		public int vertexNumber
		{
			get { return vertexNumber_; }
			set
			{
#if UNITY_EDITOR
				if (vertexNumber_ != int.MaxValue)
				{
					Debug.LogWarning( "VertexNumber already " + vertexNumber_ + " on being set to " + value );
				}
#endif
				vertexNumber_ = value;
			}
		}

		private int altVertexNumber_ = int.MaxValue;
		public int altVertexNumber
		{
			get { return altVertexNumber_; }
			set
			{
#if UNITY_EDITOR
				if (altVertexNumber_ != int.MaxValue)
				{
					Debug.LogWarning( "AltVertexNumber already " + altVertexNumber_ + " on being set to " + value );
				}
#endif
				altVertexNumber_ = value;
			}
		}

		public int getVertexNumber(bool useAlt)
		{
			return (useAlt) ? (altVertexNumber) : (vertexNumber);
		}

		private Vector3 normal_ = Vector3.zero;

		public Vector3 normal
		{
			get { return normal_; }
			set
			{
				normal_ = value;
				transform.rotation = Quaternion.LookRotation( normal_, Vector3.forward );
				
				RJWard.Core.Test.DebugBlob debugBlob = transform.GetComponentInChildren<RJWard.Core.Test.DebugBlob>( );
				if (debugBlob != null)
				{
					debugBlob.ActivateDirectionPointer( true );
				}
				
			}
		}
	}
}

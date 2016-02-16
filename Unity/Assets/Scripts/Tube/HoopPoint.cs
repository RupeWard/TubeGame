﻿using UnityEngine;
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

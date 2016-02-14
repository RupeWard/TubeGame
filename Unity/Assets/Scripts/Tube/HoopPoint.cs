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
		
	}
}

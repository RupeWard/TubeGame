using UnityEngine;
using System.Collections.Generic;

namespace RJWard.Tube
{
	public class SpinePoint : MonoBehaviour
	{
		private Hoop hoop_ = null;
		public Hoop hoop
		{
			get { return hoop_; }
		}

		private float radius_ = 0f;

		private bool fixedRotation_ = false;

		public void Init( Vector3 pos, Vector3? rot, float rad )
		{
			radius_ = rad;
			transform.localPosition = pos;
			if (rot != null)
			{
				transform.localRotation = Quaternion.Euler( (Vector3)rot );
			}
			else
			{
				transform.localRotation = Quaternion.identity;
			}
			MakeHoop( );
		}

		public void HandleNextPointAdded( SpinePoint spinePoint )
		{
			if (!fixedRotation_)
			{
				transform.LookAt( spinePoint.transform );
				MakeHoop( );
			}
		}

		private void MakeHoop( )
		{
			if (hoop_ != null)
			{
				GameObject.Destroy( hoop_.gameObject );
				hoop_ = null;
			}
			GameObject hoopGo = new GameObject( "Hoop" );
			hoop_ = hoopGo.AddComponent<Hoop>( );
			hoop_.Init( this, 10, radius_ );
		}

		public void AddAllVertices( List<Vector3> verts )
		{
			if (hoop_ != null)
			{
				hoop_.AddAllVertices( verts );
			}
		}

	}
}

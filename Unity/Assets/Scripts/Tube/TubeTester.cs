using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	public class TubeTester: MonoBehaviour
	{
		public Transform pos1 = null;
		public Transform pos2 = null;
		public int num = 5;
		public float startRadius = 5f;
		public float endRadius = 8f;

		public Material tubeWallMaterial;

		// Use this for initialization
		void Start( )
		{
			StartCoroutine( TestCR( ) );

		}

		private IEnumerator TestCR()
		{
			yield return null;
			TubeSection TS = TubeSection.CreateLinear( pos1.position, null, pos2.position, null, num, startRadius, endRadius, tubeWallMaterial ); 
		}

		// Update is called once per frame
		void Update( )
		{

		}
	}

}

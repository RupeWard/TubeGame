using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class SpinePointPathChooser : MonoBehaviour
	{
		abstract public SpinePointConnection ChoosePath( SpinePoint_Base spb );
	}

}

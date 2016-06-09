﻿using UnityEngine;
using System.Collections;

namespace RJWard.Tube
{
	abstract public class Game_Base
	{
		abstract public RandLinearSectionDefn GetNextTubeSectionDefn(  );
		abstract public void Reset( );
	}
}

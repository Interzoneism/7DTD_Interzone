using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014B2 RID: 5298
	[Preserve]
	public class ConsiderationData
	{
		// Token: 0x0600A374 RID: 41844 RVA: 0x00410E3B File Offset: 0x0040F03B
		public ConsiderationData()
		{
			this.EntityTargets = new List<Entity>();
			this.WaypointTargets = new List<Vector3>();
		}

		// Token: 0x04007E8C RID: 32396
		public List<Entity> EntityTargets;

		// Token: 0x04007E8D RID: 32397
		public List<Vector3> WaypointTargets;
	}
}

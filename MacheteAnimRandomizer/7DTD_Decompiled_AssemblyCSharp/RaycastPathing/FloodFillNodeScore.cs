using System;
using UnityEngine.Scripting;

namespace RaycastPathing
{
	// Token: 0x020015C3 RID: 5571
	[Preserve]
	public class FloodFillNodeScore
	{
		// Token: 0x1700132C RID: 4908
		// (get) Token: 0x0600AB0A RID: 43786 RVA: 0x00434B68 File Offset: 0x00432D68
		public float F
		{
			get
			{
				return this.G + this.H;
			}
		}

		// Token: 0x0400857C RID: 34172
		public float G;

		// Token: 0x0400857D RID: 34173
		public float H;
	}
}

using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014B1 RID: 5297
	[Preserve]
	public class Context
	{
		// Token: 0x170011D2 RID: 4562
		// (get) Token: 0x0600A372 RID: 41842 RVA: 0x00410DF8 File Offset: 0x0040EFF8
		public List<string> AIPackages
		{
			get
			{
				return this.Self.AIPackages;
			}
		}

		// Token: 0x0600A373 RID: 41843 RVA: 0x00410E05 File Offset: 0x0040F005
		public Context(EntityAlive _self)
		{
			this.Self = _self;
			this.World = GameManager.Instance.World;
			this.ConsiderationData = new ConsiderationData();
			this.ActionData = default(ActionData);
		}

		// Token: 0x04007E87 RID: 32391
		public EntityAlive Self;

		// Token: 0x04007E88 RID: 32392
		public World World;

		// Token: 0x04007E89 RID: 32393
		public ConsiderationData ConsiderationData;

		// Token: 0x04007E8A RID: 32394
		public ActionData ActionData;

		// Token: 0x04007E8B RID: 32395
		public float updateTimer;
	}
}

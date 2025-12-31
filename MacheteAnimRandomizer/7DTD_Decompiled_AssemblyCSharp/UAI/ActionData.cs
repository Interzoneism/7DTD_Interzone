using System;
using UnityEngine.Scripting;

namespace UAI
{
	// Token: 0x020014B3 RID: 5299
	[Preserve]
	public struct ActionData
	{
		// Token: 0x170011D3 RID: 4563
		// (get) Token: 0x0600A375 RID: 41845 RVA: 0x00410E5C File Offset: 0x0040F05C
		public UAITaskBase CurrentTask
		{
			get
			{
				if (this.Action == null || this.Action.GetTasks() == null || this.TaskIndex < 0 || this.TaskIndex >= this.Action.GetTasks().Count)
				{
					return null;
				}
				return this.Action.GetTasks()[this.TaskIndex];
			}
		}

		// Token: 0x0600A376 RID: 41846 RVA: 0x00410EB7 File Offset: 0x0040F0B7
		public void ClearData()
		{
			this.Data = null;
			this.TaskStartTimeStamp = 0UL;
			this.Initialized = false;
			this.Started = false;
			this.Executing = false;
			this.Failed = false;
			this.Finished = false;
		}

		// Token: 0x04007E8E RID: 32398
		public UAIAction Action;

		// Token: 0x04007E8F RID: 32399
		public object Target;

		// Token: 0x04007E90 RID: 32400
		public object Data;

		// Token: 0x04007E91 RID: 32401
		public int TaskIndex;

		// Token: 0x04007E92 RID: 32402
		public ulong TaskStartTimeStamp;

		// Token: 0x04007E93 RID: 32403
		public bool Initialized;

		// Token: 0x04007E94 RID: 32404
		public bool Started;

		// Token: 0x04007E95 RID: 32405
		public bool Executing;

		// Token: 0x04007E96 RID: 32406
		public bool Failed;

		// Token: 0x04007E97 RID: 32407
		public bool Finished;
	}
}

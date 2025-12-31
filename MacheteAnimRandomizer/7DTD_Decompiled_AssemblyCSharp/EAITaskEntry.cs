using System;
using UnityEngine.Scripting;

// Token: 0x02000400 RID: 1024
[Preserve]
public class EAITaskEntry
{
	// Token: 0x06001ED6 RID: 7894 RVA: 0x000C01FC File Offset: 0x000BE3FC
	public EAITaskEntry(int _priority, EAIBase _action)
	{
		this.priority = _priority;
		this.action = _action;
	}

	// Token: 0x0400154F RID: 5455
	public EAIBase action;

	// Token: 0x04001550 RID: 5456
	public int priority;

	// Token: 0x04001551 RID: 5457
	public bool isExecuting;

	// Token: 0x04001552 RID: 5458
	public float executeTime;
}

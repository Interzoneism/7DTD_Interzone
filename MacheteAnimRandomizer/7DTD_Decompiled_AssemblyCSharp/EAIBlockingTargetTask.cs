using System;
using UnityEngine.Scripting;

// Token: 0x020003E0 RID: 992
[Preserve]
public class EAIBlockingTargetTask : EAIBase
{
	// Token: 0x06001E04 RID: 7684 RVA: 0x000BAB17 File Offset: 0x000B8D17
	public override void Init(EntityAlive _theEntity)
	{
		base.Init(_theEntity);
		this.MutexBits = 1;
	}

	// Token: 0x06001E05 RID: 7685 RVA: 0x000BACBA File Offset: 0x000B8EBA
	public override bool CanExecute()
	{
		return this.canExecute;
	}

	// Token: 0x06001E06 RID: 7686 RVA: 0x000BACBA File Offset: 0x000B8EBA
	public override bool Continue()
	{
		return this.canExecute;
	}

	// Token: 0x040014A4 RID: 5284
	public bool canExecute;
}

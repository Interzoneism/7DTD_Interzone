using System;
using UnityEngine.Scripting;

// Token: 0x0200062B RID: 1579
[Preserve]
public class MinEventActionRemoveBuff : MinEventActionBuffModifierBase
{
	// Token: 0x060030A0 RID: 12448 RVA: 0x0014C107 File Offset: 0x0014A307
	public override void Execute(MinEventParams _params)
	{
		base.Remove(_params);
	}
}

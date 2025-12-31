using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x0200066B RID: 1643
[Preserve]
public class MinEventActionFadeOutSound : MinEventActionSoundBase
{
	// Token: 0x06003180 RID: 12672 RVA: 0x0015195C File Offset: 0x0014FB5C
	public override void Execute(MinEventParams _params)
	{
		if ((this.localPlayerOnly && this.targets[0] as EntityPlayerLocal != null) || (!this.localPlayerOnly && this.targets[0] != null))
		{
			string soundGroupForTarget = base.GetSoundGroupForTarget();
			Manager.FadeOut(this.targets[0].entityId, soundGroupForTarget);
		}
	}
}

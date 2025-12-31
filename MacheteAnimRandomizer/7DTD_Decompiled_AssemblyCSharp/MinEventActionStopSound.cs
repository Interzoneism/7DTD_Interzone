using System;
using Audio;
using DynamicMusic;
using UnityEngine.Scripting;

// Token: 0x0200066A RID: 1642
[Preserve]
public class MinEventActionStopSound : MinEventActionSoundBase
{
	// Token: 0x0600317E RID: 12670 RVA: 0x001518AC File Offset: 0x0014FAAC
	public override void Execute(MinEventParams _params)
	{
		string soundGroupForTarget = base.GetSoundGroupForTarget();
		if (this.localPlayerOnly && this.targets[0] as EntityPlayerLocal != null)
		{
			if (!this.loop)
			{
				Manager.Stop(this.targets[0].entityId, soundGroupForTarget);
				return;
			}
			Manager.StopLoopInsidePlayerHead(soundGroupForTarget, this.targets[0].entityId);
			if (this.toggleDMS)
			{
				SectionSelector.IsDMSTempDisabled = false;
				return;
			}
		}
		else if (!this.localPlayerOnly && this.targets[0] != null)
		{
			Manager.BroadcastStop(this.targets[0].entityId, soundGroupForTarget);
		}
	}
}

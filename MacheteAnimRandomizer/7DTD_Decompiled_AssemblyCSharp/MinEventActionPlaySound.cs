using System;
using Audio;
using DynamicMusic;
using UnityEngine.Scripting;

// Token: 0x02000669 RID: 1641
[Preserve]
public class MinEventActionPlaySound : MinEventActionSoundBase
{
	// Token: 0x0600317C RID: 12668 RVA: 0x00151784 File Offset: 0x0014F984
	public override void Execute(MinEventParams _params)
	{
		if (!this.silentOnEquip || !(_params.Self != null & _params.Self.IsEquipping))
		{
			string soundGroupForTarget = base.GetSoundGroupForTarget();
			if (this.localPlayerOnly && this.targets[0] as EntityPlayerLocal != null)
			{
				if (!this.loop)
				{
					Manager.Play(this.playAtSelf ? _params.Self : this.targets[0], soundGroupForTarget, 1f, false);
					return;
				}
				Manager.PlayInsidePlayerHead(soundGroupForTarget, this.targets[0].entityId, 0f, true, false);
				if (this.toggleDMS)
				{
					SectionSelector.IsDMSTempDisabled = true;
					return;
				}
			}
			else
			{
				if (!this.localPlayerOnly && !this.playAtSelf && this.targets[0] != null)
				{
					Manager.BroadcastPlay(this.targets[0], soundGroupForTarget, false);
					return;
				}
				if (!this.localPlayerOnly && this.playAtSelf && _params.Self != null)
				{
					Manager.BroadcastPlay(_params.Self, soundGroupForTarget, false);
				}
			}
		}
	}
}

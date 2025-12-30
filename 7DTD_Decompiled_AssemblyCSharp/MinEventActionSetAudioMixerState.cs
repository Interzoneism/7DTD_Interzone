using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000648 RID: 1608
[Preserve]
public class MinEventActionSetAudioMixerState : MinEventActionTargetedBase
{
	// Token: 0x170004B4 RID: 1204
	// (get) Token: 0x0600310F RID: 12559 RVA: 0x0014EC68 File Offset: 0x0014CE68
	// (set) Token: 0x06003110 RID: 12560 RVA: 0x0014EC70 File Offset: 0x0014CE70
	public MinEventActionSetAudioMixerState.AudioMixerStates State { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004B5 RID: 1205
	// (get) Token: 0x06003111 RID: 12561 RVA: 0x0014EC79 File Offset: 0x0014CE79
	// (set) Token: 0x06003112 RID: 12562 RVA: 0x0014EC81 File Offset: 0x0014CE81
	public bool Value { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06003113 RID: 12563 RVA: 0x0014EC8C File Offset: 0x0014CE8C
	public override void Execute(MinEventParams _params)
	{
		if (this.targets == null)
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityPlayerLocal entityPlayerLocal = this.targets[i] as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				MinEventActionSetAudioMixerState.AudioMixerStates state = this.State;
				if (state != MinEventActionSetAudioMixerState.AudioMixerStates.Stunned)
				{
					if (state == MinEventActionSetAudioMixerState.AudioMixerStates.Deafened)
					{
						entityPlayerLocal.isDeafened = this.Value;
					}
				}
				else
				{
					entityPlayerLocal.isStunned = this.Value;
				}
			}
		}
	}

	// Token: 0x06003114 RID: 12564 RVA: 0x0014ECFC File Offset: 0x0014CEFC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (!(localName == "state"))
			{
				if (localName == "enabled")
				{
					this.Value = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				}
			}
			else
			{
				this.State = (MinEventActionSetAudioMixerState.AudioMixerStates)Enum.Parse(typeof(MinEventActionSetAudioMixerState.AudioMixerStates), _attribute.Value);
			}
		}
		return flag;
	}

	// Token: 0x02000649 RID: 1609
	public enum AudioMixerStates
	{
		// Token: 0x04002775 RID: 10101
		Stunned,
		// Token: 0x04002776 RID: 10102
		Deafened
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000629 RID: 1577
[Preserve]
public class MinEventActionAddBuff : MinEventActionBuffModifierBase
{
	// Token: 0x0600309A RID: 12442 RVA: 0x0014BE28 File Offset: 0x0014A028
	public override void Execute(MinEventParams _params)
	{
		bool netSync = !_params.Self.isEntityRemote | _params.IsLocal;
		int num = -1;
		if (_params.Buff != null)
		{
			num = _params.Buff.InstigatorId;
		}
		if (num == -1)
		{
			num = _params.Self.entityId;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			string[] array = this.buffNames;
			if (this.buffOneOnly && this.buffWeights != null)
			{
				float randomFloat = this.targets[i].rand.RandomFloat;
				float num2 = 0f;
				for (int j = 0; j < this.buffWeights.Length; j++)
				{
					num2 += this.buffWeights[j];
					if (num2 >= randomFloat)
					{
						array = new string[]
						{
							this.buffNames[j]
						};
						break;
					}
				}
			}
			else if (this.buffWeights != null)
			{
				List<string> list = new List<string>();
				for (int k = 0; k < this.buffWeights.Length; k++)
				{
					float randomFloat2 = this.targets[i].rand.RandomFloat;
					if (this.buffWeights[k] >= randomFloat2)
					{
						list.Add(this.buffNames[k]);
					}
				}
				array = list.ToArray();
			}
			foreach (string name in array)
			{
				BuffClass buff = BuffManager.GetBuff(name);
				if (buff != null)
				{
					if (this.durationAltered && this.cvarRef)
					{
						if (this.targets[i].Buffs.HasCustomVar(this.refCvarName))
						{
							this.duration = this.targets[i].Buffs.GetCustomVar(this.refCvarName);
						}
						else
						{
							this.duration = buff.InitialDurationMax;
						}
					}
					if (this.durationAltered)
					{
						this.targets[i].Buffs.AddBuff(name, num, netSync, false, this.duration);
					}
					else
					{
						this.targets[i].Buffs.AddBuff(name, num, netSync, false, -1f);
					}
				}
			}
		}
	}

	// Token: 0x0600309B RID: 12443 RVA: 0x0014C044 File Offset: 0x0014A244
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "duration")
		{
			if (_attribute.Value.StartsWith("@"))
			{
				this.cvarRef = true;
				this.refCvarName = _attribute.Value.Substring(1);
			}
			else
			{
				this.duration = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
			}
			this.durationAltered = true;
			return true;
		}
		return flag;
	}

	// Token: 0x04002712 RID: 10002
	[PublicizedFrom(EAccessModifier.Private)]
	public float duration;

	// Token: 0x04002713 RID: 10003
	[PublicizedFrom(EAccessModifier.Private)]
	public bool durationAltered;

	// Token: 0x04002714 RID: 10004
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x04002715 RID: 10005
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName = string.Empty;
}

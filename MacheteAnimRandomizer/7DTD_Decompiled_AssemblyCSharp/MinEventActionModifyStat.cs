using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200066C RID: 1644
[Preserve]
public class MinEventActionModifyStat : MinEventActionTargetedBase
{
	// Token: 0x06003182 RID: 12674 RVA: 0x001519C4 File Offset: 0x0014FBC4
	public override void Execute(MinEventParams _params)
	{
		for (int i = 0; i < this.targets.Count; i++)
		{
			if (this.cvarRef)
			{
				this.value = this.targets[i].Buffs.GetCustomVar(this.refCvarName);
			}
			Stat stat = null;
			string a = this.statName;
			if (!(a == "health"))
			{
				if (!(a == "stamina"))
				{
					if (!(a == "water"))
					{
						if (a == "coretemp")
						{
							stat = ((EntityPlayer)this.targets[i]).PlayerStats.CoreTemp;
						}
					}
					else
					{
						stat = this.targets[i].Stats.Water;
					}
				}
				else
				{
					stat = this.targets[i].Stats.Stamina;
				}
			}
			else
			{
				stat = this.targets[i].Stats.Health;
			}
			if (stat != null)
			{
				switch (this.operation)
				{
				case MinEventActionModifyStat.OperationTypes.set:
				case MinEventActionModifyStat.OperationTypes.setvalue:
					stat.Value = this.value;
					break;
				case MinEventActionModifyStat.OperationTypes.add:
					stat.Value += this.value;
					break;
				case MinEventActionModifyStat.OperationTypes.subtract:
					stat.Value -= this.value;
					break;
				case MinEventActionModifyStat.OperationTypes.multiply:
					stat.Value *= this.value;
					break;
				case MinEventActionModifyStat.OperationTypes.divide:
					stat.Value /= ((this.value == 0f) ? 0.0001f : this.value);
					break;
				}
			}
		}
	}

	// Token: 0x06003183 RID: 12675 RVA: 0x00151B60 File Offset: 0x0014FD60
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "stat")
			{
				this.statName = _attribute.Value.ToLower();
				return true;
			}
			if (localName == "operation")
			{
				this.operation = EnumUtils.Parse<MinEventActionModifyStat.OperationTypes>(_attribute.Value, true);
				return true;
			}
			if (localName == "value")
			{
				if (_attribute.Value.StartsWith("@"))
				{
					this.cvarRef = true;
					this.refCvarName = _attribute.Value.Substring(1);
				}
				else
				{
					this.value = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				}
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027E4 RID: 10212
	[PublicizedFrom(EAccessModifier.Private)]
	public string statName;

	// Token: 0x040027E5 RID: 10213
	[PublicizedFrom(EAccessModifier.Private)]
	public MinEventActionModifyStat.OperationTypes operation;

	// Token: 0x040027E6 RID: 10214
	[PublicizedFrom(EAccessModifier.Private)]
	public float value;

	// Token: 0x040027E7 RID: 10215
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x040027E8 RID: 10216
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName;

	// Token: 0x0200066D RID: 1645
	[PublicizedFrom(EAccessModifier.Private)]
	public enum OperationTypes
	{
		// Token: 0x040027EA RID: 10218
		set,
		// Token: 0x040027EB RID: 10219
		setvalue,
		// Token: 0x040027EC RID: 10220
		add,
		// Token: 0x040027ED RID: 10221
		subtract,
		// Token: 0x040027EE RID: 10222
		multiply,
		// Token: 0x040027EF RID: 10223
		divide,
		// Token: 0x040027F0 RID: 10224
		randomfloat,
		// Token: 0x040027F1 RID: 10225
		randomint
	}
}

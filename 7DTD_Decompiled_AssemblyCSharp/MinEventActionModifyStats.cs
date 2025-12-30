using System;
using System.Collections;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200066E RID: 1646
[Preserve]
public class MinEventActionModifyStats : MinEventActionTargetedBase
{
	// Token: 0x06003185 RID: 12677 RVA: 0x00151C1F File Offset: 0x0014FE1F
	public override void Execute(MinEventParams _params)
	{
		if (this.Delay > 0f)
		{
			GameManager.Instance.StartCoroutine(this.executeDelayed(this.Delay, _params));
			return;
		}
		this.execute(_params);
	}

	// Token: 0x06003186 RID: 12678 RVA: 0x00151C4E File Offset: 0x0014FE4E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator executeDelayed(float delaySeconds, MinEventParams _params)
	{
		yield return new WaitForSeconds(delaySeconds);
		this.execute(_params);
		yield break;
	}

	// Token: 0x06003187 RID: 12679 RVA: 0x00151C6C File Offset: 0x0014FE6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void execute(MinEventParams _params)
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
					if (!(a == "food"))
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
						stat = this.targets[i].Stats.Food;
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
				case MinEventActionModifyStats.OperationTypes.set:
				case MinEventActionModifyStats.OperationTypes.setvalue:
					if (this.valueType == "max")
					{
						stat.BaseMax = this.value;
					}
					else if (this.valueType == "modifiedmax")
					{
						stat.MaxModifier = this.value - stat.BaseMax;
					}
					else
					{
						stat.Value = this.value;
					}
					break;
				case MinEventActionModifyStats.OperationTypes.add:
					if (this.valueType == "max")
					{
						stat.BaseMax += this.value;
					}
					else if (this.valueType == "modifiedmax")
					{
						stat.MaxModifier += this.value;
					}
					else
					{
						stat.Value += this.value;
					}
					break;
				case MinEventActionModifyStats.OperationTypes.subtract:
					if (this.valueType == "max")
					{
						stat.BaseMax -= this.value;
					}
					else if (this.valueType == "modifiedmax")
					{
						stat.MaxModifier -= this.value;
					}
					else
					{
						stat.Value -= this.value;
					}
					break;
				case MinEventActionModifyStats.OperationTypes.multiply:
					if (this.valueType == "max")
					{
						stat.BaseMax *= this.value;
					}
					else if (this.valueType == "modifiedmax")
					{
						stat.MaxModifier *= this.value;
					}
					else
					{
						stat.Value *= this.value;
					}
					break;
				case MinEventActionModifyStats.OperationTypes.divide:
					if (this.valueType == "max")
					{
						stat.BaseMax = stat.Value / ((this.value == 0f) ? 0.0001f : this.value);
					}
					else if (this.valueType == "modifiedmax")
					{
						stat.MaxModifier = stat.Value / ((this.value == 0f) ? 0.0001f : this.value);
					}
					else
					{
						stat.Value /= ((this.value == 0f) ? 0.0001f : this.value);
					}
					break;
				}
			}
		}
	}

	// Token: 0x06003188 RID: 12680 RVA: 0x00152000 File Offset: 0x00150200
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
				this.operation = EnumUtils.Parse<MinEventActionModifyStats.OperationTypes>(_attribute.Value, true);
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
			if (localName == "value_type")
			{
				this.valueType = _attribute.Value.ToLower();
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027F2 RID: 10226
	[PublicizedFrom(EAccessModifier.Private)]
	public string statName;

	// Token: 0x040027F3 RID: 10227
	[PublicizedFrom(EAccessModifier.Private)]
	public MinEventActionModifyStats.OperationTypes operation;

	// Token: 0x040027F4 RID: 10228
	[PublicizedFrom(EAccessModifier.Private)]
	public float value;

	// Token: 0x040027F5 RID: 10229
	[PublicizedFrom(EAccessModifier.Private)]
	public string valueType;

	// Token: 0x040027F6 RID: 10230
	[PublicizedFrom(EAccessModifier.Private)]
	public bool cvarRef;

	// Token: 0x040027F7 RID: 10231
	[PublicizedFrom(EAccessModifier.Private)]
	public string refCvarName;

	// Token: 0x0200066F RID: 1647
	[PublicizedFrom(EAccessModifier.Private)]
	public enum OperationTypes
	{
		// Token: 0x040027F9 RID: 10233
		set,
		// Token: 0x040027FA RID: 10234
		setvalue,
		// Token: 0x040027FB RID: 10235
		add,
		// Token: 0x040027FC RID: 10236
		subtract,
		// Token: 0x040027FD RID: 10237
		multiply,
		// Token: 0x040027FE RID: 10238
		divide,
		// Token: 0x040027FF RID: 10239
		randomfloat,
		// Token: 0x04002800 RID: 10240
		randomint
	}
}

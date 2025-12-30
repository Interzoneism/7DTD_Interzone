using System;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

// Token: 0x02000625 RID: 1573
public class MinEventActionBuffModifierBase : MinEventActionTargetedBase
{
	// Token: 0x0600308F RID: 12431 RVA: 0x0014BAE4 File Offset: 0x00149CE4
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "buffs" || localName == "buff")
			{
				if (_attribute.Value.Contains(","))
				{
					this.buffNames = _attribute.Value.Replace(" ", "").Split(',', StringSplitOptions.None);
				}
				else
				{
					this.buffNames = new string[]
					{
						_attribute.Value
					};
				}
				return true;
			}
			if (localName == "weights" || localName == "weight")
			{
				if (_attribute.Value.Contains(","))
				{
					string[] array = _attribute.Value.Replace(" ", "").Split(',', StringSplitOptions.None);
					this.buffWeights = new float[array.Length];
					for (int i = 0; i < array.Length; i++)
					{
						this.buffWeights[i] = StringParsers.ParseFloat(array[i], 0, -1, NumberStyles.Any);
					}
				}
				else
				{
					this.buffNames = new string[]
					{
						_attribute.Value
					};
					this.buffWeights = new float[]
					{
						1f
					};
				}
				return true;
			}
			if (localName == "fireOneBuff")
			{
				this.buffOneOnly = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x06003090 RID: 12432 RVA: 0x0014BC4C File Offset: 0x00149E4C
	public override void ParseXMLPostProcess()
	{
		base.ParseXMLPostProcess();
		if (this.buffOneOnly && this.buffWeights != null)
		{
			float weightSum = this.buffWeights.Sum();
			this.buffWeights = (from w in this.buffWeights
			select w / weightSum).ToArray<float>();
		}
		if (!this.buffOneOnly && this.buffWeights != null)
		{
			if (this.buffWeights.Any((float w) => w > 1f || w < 0f))
			{
				Log.Warning("Warning: Invalid \"Buffs.xml\" configuration. User has specified weights outside of range 0-1 and fireOneBuff=\"false\" or missing. When fireOneBuff=\"false\", the weights represent probabilities between 0-1 for the buffs to be added.");
			}
		}
	}

	// Token: 0x06003091 RID: 12433 RVA: 0x0014BCF0 File Offset: 0x00149EF0
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Remove(MinEventParams _params)
	{
		bool netSync = !_params.Self.isEntityRemote | _params.IsLocal;
		for (int i = 0; i < this.buffNames.Length; i++)
		{
			string name = this.buffNames[i];
			if (BuffManager.GetBuff(name) != null)
			{
				for (int j = 0; j < this.targets.Count; j++)
				{
					this.targets[j].Buffs.RemoveBuff(name, netSync);
				}
			}
		}
	}

	// Token: 0x0400270C RID: 9996
	[PublicizedFrom(EAccessModifier.Protected)]
	public string[] buffNames;

	// Token: 0x0400270D RID: 9997
	[PublicizedFrom(EAccessModifier.Protected)]
	public float[] buffWeights;

	// Token: 0x0400270E RID: 9998
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool buffOneOnly;
}

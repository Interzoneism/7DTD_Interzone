using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

// Token: 0x020004BF RID: 1215
public class DamageMultiplier
{
	// Token: 0x060027CE RID: 10190 RVA: 0x00101E8D File Offset: 0x0010008D
	public DamageMultiplier()
	{
	}

	// Token: 0x060027CF RID: 10191 RVA: 0x00101EA0 File Offset: 0x001000A0
	public DamageMultiplier(DynamicProperties _properties, string _prefix)
	{
		if (_prefix == null)
		{
			_prefix = "";
		}
		if (_prefix.Length > 0 && !_prefix.EndsWith("."))
		{
			_prefix += ".";
		}
		_prefix += "DamageBonus.";
		foreach (KeyValuePair<string, string> keyValuePair in _properties.Values.Dict)
		{
			if (keyValuePair.Key.StartsWith(_prefix))
			{
				string name = keyValuePair.Key.Substring(_prefix.Length);
				float value = StringParsers.ParseFloat(_properties.Values[keyValuePair.Key], 0, -1, NumberStyles.Any);
				this.addMultiplier(name, value);
			}
		}
	}

	// Token: 0x060027D0 RID: 10192 RVA: 0x00101F88 File Offset: 0x00100188
	[PublicizedFrom(EAccessModifier.Private)]
	public void addMultiplier(string _name, float _value)
	{
		this.damageMultiplier[_name] = _value;
	}

	// Token: 0x060027D1 RID: 10193 RVA: 0x00101F97 File Offset: 0x00100197
	public float Get(string _group)
	{
		if (_group == null || this.damageMultiplier == null || !this.damageMultiplier.ContainsKey(_group))
		{
			return 1f;
		}
		return this.damageMultiplier[_group];
	}

	// Token: 0x060027D2 RID: 10194 RVA: 0x00101FC4 File Offset: 0x001001C4
	public void Read(BinaryReader _br)
	{
		this.damageMultiplier.Clear();
		int num = (int)_br.ReadInt16();
		for (int i = 0; i < num; i++)
		{
			string key = _br.ReadString();
			float value = _br.ReadSingle();
			this.damageMultiplier.Add(key, value);
		}
	}

	// Token: 0x060027D3 RID: 10195 RVA: 0x0010200C File Offset: 0x0010020C
	public void Write(BinaryWriter _bw)
	{
		_bw.Write((short)this.damageMultiplier.Count);
		foreach (KeyValuePair<string, float> keyValuePair in this.damageMultiplier)
		{
			_bw.Write(keyValuePair.Key);
			_bw.Write(keyValuePair.Value);
		}
	}

	// Token: 0x04001E73 RID: 7795
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PREFIX = "DamageBonus.";

	// Token: 0x04001E74 RID: 7796
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, float> damageMultiplier = new Dictionary<string, float>();
}

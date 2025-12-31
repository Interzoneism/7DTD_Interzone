using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

// Token: 0x020005C6 RID: 1478
public class RequirementBase : IRequirement
{
	// Token: 0x06002F59 RID: 12121 RVA: 0x00144BF3 File Offset: 0x00142DF3
	public virtual bool IsValid(MinEventParams _params)
	{
		if (this.cvarName != null)
		{
			this.value = _params.Self.Buffs.GetCustomVar(this.cvarName);
		}
		return true;
	}

	// Token: 0x06002F5A RID: 12122 RVA: 0x00144C1A File Offset: 0x00142E1A
	public void SetDescription(string desc)
	{
		this.Description = desc;
	}

	// Token: 0x06002F5B RID: 12123 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void GetInfoStrings(ref List<string> list)
	{
	}

	// Token: 0x06002F5C RID: 12124 RVA: 0x00144C24 File Offset: 0x00142E24
	public virtual bool ParseXAttribute(XAttribute _attribute)
	{
		string localName = _attribute.Name.LocalName;
		if (localName == "operation")
		{
			this.operation = EnumUtils.Parse<RequirementBase.OperationTypes>(_attribute.Value, true);
			return true;
		}
		if (localName == "value")
		{
			string text = _attribute.Value;
			if (text.Length > 0)
			{
				if (text[0] == '@')
				{
					this.cvarName = text.Substring(1);
				}
				else
				{
					this.value = StringParsers.ParseFloat(text, 0, -1, NumberStyles.Any);
				}
			}
			return true;
		}
		if (!(localName == "invert"))
		{
			return false;
		}
		this.invert = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
		return true;
	}

	// Token: 0x06002F5D RID: 12125 RVA: 0x00144CD0 File Offset: 0x00142ED0
	[PublicizedFrom(EAccessModifier.Protected)]
	public static bool compareValues(float _valueA, RequirementBase.OperationTypes _operation, float _valueB)
	{
		switch (_operation)
		{
		case RequirementBase.OperationTypes.Equals:
		case RequirementBase.OperationTypes.EQ:
		case RequirementBase.OperationTypes.E:
			return _valueA == _valueB;
		case RequirementBase.OperationTypes.NotEquals:
		case RequirementBase.OperationTypes.NEQ:
		case RequirementBase.OperationTypes.NE:
			return _valueA != _valueB;
		case RequirementBase.OperationTypes.Less:
		case RequirementBase.OperationTypes.LessThan:
		case RequirementBase.OperationTypes.LT:
			return _valueA < _valueB;
		case RequirementBase.OperationTypes.Greater:
		case RequirementBase.OperationTypes.GreaterThan:
		case RequirementBase.OperationTypes.GT:
			return _valueA > _valueB;
		case RequirementBase.OperationTypes.LessOrEqual:
		case RequirementBase.OperationTypes.LessThanOrEqualTo:
		case RequirementBase.OperationTypes.LTE:
			return _valueA <= _valueB;
		case RequirementBase.OperationTypes.GreaterOrEqual:
		case RequirementBase.OperationTypes.GreaterThanOrEqualTo:
		case RequirementBase.OperationTypes.GTE:
			return _valueA >= _valueB;
		default:
			return false;
		}
	}

	// Token: 0x06002F5E RID: 12126 RVA: 0x00144D58 File Offset: 0x00142F58
	public static IRequirement ParseRequirement(XElement _element)
	{
		string text = _element.GetAttribute("name");
		if (text.Length == 0)
		{
			return null;
		}
		bool flag = text[0] == '!';
		if (flag)
		{
			text = text.Substring(1);
		}
		Type type = Type.GetType(text);
		if (type == null)
		{
			return null;
		}
		RequirementBase requirementBase = (RequirementBase)Activator.CreateInstance(type);
		requirementBase.invert = flag;
		string attribute = _element.GetAttribute("desc_key");
		if (attribute.Length > 0)
		{
			requirementBase.SetDescription(Localization.Get(attribute, false));
		}
		foreach (XAttribute attribute2 in _element.Attributes())
		{
			requirementBase.ParseXAttribute(attribute2);
		}
		return requirementBase;
	}

	// Token: 0x06002F5F RID: 12127 RVA: 0x00144E30 File Offset: 0x00143030
	public static List<IRequirement> ParseRequirements(XElement _element)
	{
		List<IRequirement> list = new List<IRequirement>();
		foreach (XElement element in _element.Elements("requirement"))
		{
			list.Add(RequirementBase.ParseRequirement(element));
		}
		return list;
	}

	// Token: 0x06002F60 RID: 12128 RVA: 0x00144E94 File Offset: 0x00143094
	public string GetInfoString()
	{
		if (this.Description != null)
		{
			return this.Description;
		}
		List<string> list = new List<string>();
		this.GetInfoStrings(ref list);
		if (list.Count > 0)
		{
			return list[0];
		}
		return null;
	}

	// Token: 0x04002664 RID: 9828
	[PublicizedFrom(EAccessModifier.Protected)]
	public RequirementBase.OperationTypes operation;

	// Token: 0x04002665 RID: 9829
	[PublicizedFrom(EAccessModifier.Protected)]
	public string cvarName;

	// Token: 0x04002666 RID: 9830
	public bool invert;

	// Token: 0x04002667 RID: 9831
	[PublicizedFrom(EAccessModifier.Protected)]
	public float value;

	// Token: 0x04002668 RID: 9832
	public string Description;

	// Token: 0x020005C7 RID: 1479
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum OperationTypes
	{
		// Token: 0x0400266A RID: 9834
		None,
		// Token: 0x0400266B RID: 9835
		Equals,
		// Token: 0x0400266C RID: 9836
		EQ,
		// Token: 0x0400266D RID: 9837
		E,
		// Token: 0x0400266E RID: 9838
		NotEquals,
		// Token: 0x0400266F RID: 9839
		NEQ,
		// Token: 0x04002670 RID: 9840
		NE,
		// Token: 0x04002671 RID: 9841
		Less,
		// Token: 0x04002672 RID: 9842
		LessThan,
		// Token: 0x04002673 RID: 9843
		LT,
		// Token: 0x04002674 RID: 9844
		Greater,
		// Token: 0x04002675 RID: 9845
		GreaterThan,
		// Token: 0x04002676 RID: 9846
		GT,
		// Token: 0x04002677 RID: 9847
		LessOrEqual,
		// Token: 0x04002678 RID: 9848
		LessThanOrEqualTo,
		// Token: 0x04002679 RID: 9849
		LTE,
		// Token: 0x0400267A RID: 9850
		GreaterOrEqual,
		// Token: 0x0400267B RID: 9851
		GreaterThanOrEqualTo,
		// Token: 0x0400267C RID: 9852
		GTE
	}
}

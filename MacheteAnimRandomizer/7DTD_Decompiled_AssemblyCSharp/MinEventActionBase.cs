using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x02000662 RID: 1634
[Preserve]
public class MinEventActionBase : IMinEventAction
{
	// Token: 0x06003163 RID: 12643 RVA: 0x00150735 File Offset: 0x0014E935
	public MinEventActionBase()
	{
		this.Requirements = new List<IRequirement>();
	}

	// Token: 0x06003164 RID: 12644 RVA: 0x00150748 File Offset: 0x0014E948
	public virtual void GetInfoStrings(ref List<string> list)
	{
		list.Add(this.EventType.ToStringCached<MinEventTypes>() + ": " + this.ToString());
		if (this.Requirements != null)
		{
			for (int i = 0; i < this.Requirements.Count; i++)
			{
				this.Requirements[i].GetInfoStrings(ref list);
			}
		}
	}

	// Token: 0x06003165 RID: 12645 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void Execute(MinEventParams _params)
	{
	}

	// Token: 0x06003166 RID: 12646 RVA: 0x001507A8 File Offset: 0x0014E9A8
	public virtual bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.Requirements.Count > 0)
		{
			bool flag = true;
			if (!this.OrCompare)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					flag &= this.Requirements[i].IsValid(_params);
					if (!flag)
					{
						return false;
					}
				}
			}
			else
			{
				for (int j = 0; j < this.Requirements.Count; j++)
				{
					flag = this.Requirements[j].IsValid(_params);
					if (flag)
					{
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return flag;
		}
		return true;
	}

	// Token: 0x06003167 RID: 12647 RVA: 0x00150834 File Offset: 0x0014EA34
	public virtual bool ParseXmlAttribute(XAttribute _attribute)
	{
		string localName = _attribute.Name.LocalName;
		if (localName == "trigger")
		{
			this.EventType = EnumUtils.Parse<MinEventTypes>(_attribute.Value, false);
			return true;
		}
		if (localName == "anytrue")
		{
			this.OrCompare = true;
			return true;
		}
		if (localName == "compare_type")
		{
			this.OrCompare = _attribute.Value.EqualsCaseInsensitive("or");
			return true;
		}
		if (!(localName == "delay"))
		{
			return false;
		}
		this.Delay = float.Parse(_attribute.Value);
		return true;
	}

	// Token: 0x06003168 RID: 12648 RVA: 0x001508CC File Offset: 0x0014EACC
	public static MinEventActionBase ParseAction(XElement _element)
	{
		if (!_element.HasAttribute("action"))
		{
			return null;
		}
		Type typeWithPrefix = ReflectionHelpers.GetTypeWithPrefix("MinEventAction", _element.GetAttribute("action"));
		if (typeWithPrefix == null)
		{
			Log.Out("Unable to find class: MinEventAction{0}", new object[]
			{
				_element.GetAttribute("action")
			});
			return null;
		}
		MinEventActionBase minEventActionBase = (MinEventActionBase)Activator.CreateInstance(typeWithPrefix);
		foreach (XAttribute attribute in _element.Attributes())
		{
			minEventActionBase.ParseXmlAttribute(attribute);
		}
		foreach (XElement element in _element.Elements("requirement"))
		{
			IRequirement requirement = RequirementBase.ParseRequirement(element);
			if (requirement != null)
			{
				minEventActionBase.Requirements.Add(requirement);
			}
		}
		minEventActionBase.ParseXMLPostProcess();
		return minEventActionBase;
	}

	// Token: 0x06003169 RID: 12649 RVA: 0x00002914 File Offset: 0x00000B14
	public virtual void ParseXMLPostProcess()
	{
	}

	// Token: 0x040027C4 RID: 10180
	public MinEventTypes EventType;

	// Token: 0x040027C5 RID: 10181
	public bool OrCompare;

	// Token: 0x040027C6 RID: 10182
	public float Delay;

	// Token: 0x040027C7 RID: 10183
	public List<IRequirement> Requirements;
}

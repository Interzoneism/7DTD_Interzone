using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200064A RID: 1610
[Preserve]
public class MinEventActionSetNavObject : MinEventActionTargetedBase
{
	// Token: 0x06003116 RID: 12566 RVA: 0x0014ED74 File Offset: 0x0014CF74
	public override void Execute(MinEventParams _params)
	{
		if (this.navObjectName == "")
		{
			return;
		}
		for (int i = 0; i < this.targets.Count; i++)
		{
			EntityAlive entityAlive = this.targets[i];
			if (this.isAdd)
			{
				entityAlive.AddNavObject(this.navObjectName, this.overrideSprite, (this.cvarToText != "") ? entityAlive.GetCVar(this.cvarToText).ToString() : this.overrideText);
			}
			else
			{
				entityAlive.RemoveNavObject(this.navObjectName);
			}
		}
	}

	// Token: 0x06003117 RID: 12567 RVA: 0x0014EE10 File Offset: 0x0014D010
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "nav_object")
			{
				this.navObjectName = _attribute.Value;
				return true;
			}
			if (localName == "sprite")
			{
				this.overrideSprite = _attribute.Value;
				return true;
			}
			if (localName == "text")
			{
				this.overrideText = _attribute.Value;
				return true;
			}
			if (localName == "cvar_to_text")
			{
				this.cvarToText = _attribute.Value;
				return true;
			}
			if (localName == "add")
			{
				this.isAdd = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
		}
		return flag;
	}

	// Token: 0x04002777 RID: 10103
	[PublicizedFrom(EAccessModifier.Private)]
	public string navObjectName = "";

	// Token: 0x04002778 RID: 10104
	[PublicizedFrom(EAccessModifier.Private)]
	public string overrideSprite = "";

	// Token: 0x04002779 RID: 10105
	[PublicizedFrom(EAccessModifier.Private)]
	public string overrideText = "";

	// Token: 0x0400277A RID: 10106
	[PublicizedFrom(EAccessModifier.Private)]
	public string cvarToText = "";

	// Token: 0x0400277B RID: 10107
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isAdd = true;
}

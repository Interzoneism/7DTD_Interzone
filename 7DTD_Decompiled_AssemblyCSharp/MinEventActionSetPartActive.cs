using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200065B RID: 1627
[Preserve]
public class MinEventActionSetPartActive : MinEventActionBase
{
	// Token: 0x06003149 RID: 12617 RVA: 0x0014FF7C File Offset: 0x0014E17C
	public override void Execute(MinEventParams _params)
	{
		bool flag = this.isActive;
		if (this.cVarName != null)
		{
			flag = (_params.Self.GetCVar(this.cVarName) != 0f);
			if (this.isInvert)
			{
				flag = !flag;
			}
		}
		_params.Self.SetPartActive(this.partName, flag);
	}

	// Token: 0x0600314A RID: 12618 RVA: 0x0014FFD3 File Offset: 0x0014E1D3
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self != null && _params.ItemValue != null && this.partName != null;
	}

	// Token: 0x0600314B RID: 12619 RVA: 0x00150000 File Offset: 0x0014E200
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "active")
			{
				if (_attribute.Value.Length >= 2 && _attribute.Value[0] == '@')
				{
					int num = 1;
					if (_attribute.Value[1] == '!')
					{
						num++;
						this.isInvert = true;
					}
					this.cVarName = _attribute.Value.Substring(num);
				}
				else
				{
					this.isActive = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				}
				return true;
			}
			if (localName == "part")
			{
				this.partName = _attribute.Value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0400279D RID: 10141
	[PublicizedFrom(EAccessModifier.Private)]
	public string partName;

	// Token: 0x0400279E RID: 10142
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isActive;

	// Token: 0x0400279F RID: 10143
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isInvert;

	// Token: 0x040027A0 RID: 10144
	[PublicizedFrom(EAccessModifier.Private)]
	public string cVarName;
}

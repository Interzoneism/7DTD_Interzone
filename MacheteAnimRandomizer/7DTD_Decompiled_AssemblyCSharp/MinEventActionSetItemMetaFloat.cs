using System;
using System.Globalization;
using System.Xml.Linq;
using UnityEngine.Scripting;

// Token: 0x0200065E RID: 1630
[Preserve]
public class MinEventActionSetItemMetaFloat : MinEventActionBase
{
	// Token: 0x06003155 RID: 12629 RVA: 0x001503C4 File Offset: 0x0014E5C4
	public override void Execute(MinEventParams _params)
	{
		ItemValue itemValue = _params.ItemValue;
		if (!itemValue.HasMetadata(this.metaKey, TypedMetadataValue.TypeTag.None))
		{
			itemValue.SetMetadata(this.metaKey, 0f, "float");
		}
		object metadata = itemValue.GetMetadata(this.metaKey);
		if (!(metadata is float))
		{
			return;
		}
		if (this.relative)
		{
			itemValue.SetMetadata(this.metaKey, (float)metadata + this.change, "float");
		}
		else
		{
			itemValue.SetMetadata(this.metaKey, this.change, "float");
		}
		if ((float)metadata < 0f)
		{
			itemValue.SetMetadata(this.metaKey, 0, "float");
		}
	}

	// Token: 0x06003156 RID: 12630 RVA: 0x00150485 File Offset: 0x0014E685
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && !string.IsNullOrEmpty(this.metaKey);
	}

	// Token: 0x06003157 RID: 12631 RVA: 0x001504A4 File Offset: 0x0014E6A4
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "change")
			{
				this.change = StringParsers.ParseFloat(_attribute.Value, 0, -1, NumberStyles.Any);
				return true;
			}
			if (localName == "relative")
			{
				this.relative = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
			if (localName == "key")
			{
				this.metaKey = _attribute.Value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027A7 RID: 10151
	[PublicizedFrom(EAccessModifier.Private)]
	public float change;

	// Token: 0x040027A8 RID: 10152
	[PublicizedFrom(EAccessModifier.Private)]
	public bool relative = true;

	// Token: 0x040027A9 RID: 10153
	[PublicizedFrom(EAccessModifier.Private)]
	public string metaKey;
}

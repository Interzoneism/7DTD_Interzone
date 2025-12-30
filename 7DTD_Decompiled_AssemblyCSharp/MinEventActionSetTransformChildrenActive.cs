using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200065D RID: 1629
[Preserve]
public class MinEventActionSetTransformChildrenActive : MinEventActionBase
{
	// Token: 0x06003151 RID: 12625 RVA: 0x0015023C File Offset: 0x0014E43C
	public override void Execute(MinEventParams _params)
	{
		Transform transform;
		if (this.parent_transform.EqualsCaseInsensitive("#HeldItemRoot") && _params.Transform != null)
		{
			transform = _params.Transform;
		}
		else if (this.parent_transform != "")
		{
			transform = GameUtils.FindDeepChildActive(_params.Self.RootTransform, this.parent_transform);
		}
		else
		{
			transform = _params.Self.RootTransform;
		}
		if (transform == null)
		{
			return;
		}
		Transform transform2 = GameUtils.FindDeepChildActive(transform, this.transformPath);
		if (transform2 == null)
		{
			return;
		}
		for (int i = 0; i < transform2.childCount; i++)
		{
			transform2.GetChild(i).gameObject.SetActive(this.isActive);
		}
	}

	// Token: 0x06003152 RID: 12626 RVA: 0x001502F4 File Offset: 0x0014E4F4
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self != null && _params.ItemValue != null && this.transformPath != null && this.transformPath != "";
	}

	// Token: 0x06003153 RID: 12627 RVA: 0x00150330 File Offset: 0x0014E530
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "active")
			{
				this.isActive = StringParsers.ParseBool(_attribute.Value, 0, -1, true);
				return true;
			}
			if (localName == "parent_transform")
			{
				this.parent_transform = _attribute.Value;
				return true;
			}
			if (localName == "transform_path")
			{
				this.transformPath = _attribute.Value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x040027A4 RID: 10148
	[PublicizedFrom(EAccessModifier.Private)]
	public string transformPath;

	// Token: 0x040027A5 RID: 10149
	[PublicizedFrom(EAccessModifier.Private)]
	public string parent_transform = "";

	// Token: 0x040027A6 RID: 10150
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isActive;
}

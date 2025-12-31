using System;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200063C RID: 1596
[Preserve]
public class MinEventActionRemovePrefabFromEntity : MinEventActionTargetedBase
{
	// Token: 0x060030D3 RID: 12499 RVA: 0x0014DA20 File Offset: 0x0014BC20
	public override void Execute(MinEventParams _params)
	{
		if (_params.Self == null)
		{
			return;
		}
		Transform transform = null;
		if (this.parent_transform_path != null)
		{
			transform = GameUtils.FindDeepChildActive(_params.Self.RootTransform, this.parent_transform_path);
		}
		Transform transform2;
		if (transform == null)
		{
			transform2 = GameUtils.FindDeepChildActive(_params.Self.RootTransform, "tempPrefab_" + this.prefabName);
		}
		else
		{
			transform2 = GameUtils.FindDeepChildActive(transform, "tempPrefab_" + this.prefabName);
		}
		if (transform2 == null)
		{
			return;
		}
		UnityEngine.Object.Destroy(transform2.gameObject);
	}

	// Token: 0x060030D4 RID: 12500 RVA: 0x0014DAB7 File Offset: 0x0014BCB7
	public override bool CanExecute(MinEventTypes _eventType, MinEventParams _params)
	{
		return base.CanExecute(_eventType, _params) && _params.Self != null && this.prefabName != null;
	}

	// Token: 0x060030D5 RID: 12501 RVA: 0x0014DADC File Offset: 0x0014BCDC
	public override bool ParseXmlAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXmlAttribute(_attribute);
		if (!flag)
		{
			string localName = _attribute.Name.LocalName;
			if (localName == "prefab" || localName == "prefab_name")
			{
				this.prefabName = _attribute.Value;
				if (this.prefabName.Contains("/"))
				{
					this.prefabName = this.prefabName.Substring(this.prefabName.LastIndexOf("/") + 1);
				}
				return true;
			}
			if (localName == "parent_transform")
			{
				this.parent_transform_path = _attribute.Value;
				return true;
			}
		}
		return flag;
	}

	// Token: 0x0400274D RID: 10061
	[PublicizedFrom(EAccessModifier.Private)]
	public string prefabName;

	// Token: 0x0400274E RID: 10062
	[PublicizedFrom(EAccessModifier.Private)]
	public string parent_transform_path;
}

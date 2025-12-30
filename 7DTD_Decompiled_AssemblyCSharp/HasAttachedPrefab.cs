using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000610 RID: 1552
[Preserve]
public class HasAttachedPrefab : TargetedCompareRequirementBase
{
	// Token: 0x0600303A RID: 12346 RVA: 0x00148338 File Offset: 0x00146538
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		Transform transform = null;
		if (this.parent_transform_path != null)
		{
			transform = GameUtils.FindDeepChildActive(_params.Self.RootTransform, this.parent_transform_path);
		}
		Transform x;
		if (transform == null)
		{
			x = GameUtils.FindDeepChildActive(_params.Self.RootTransform, "tempPrefab_" + this.prefabName);
		}
		else
		{
			x = GameUtils.FindDeepChildActive(transform, "tempPrefab_" + this.prefabName);
		}
		if (x != null)
		{
			return !this.invert;
		}
		return this.invert;
	}

	// Token: 0x0600303B RID: 12347 RVA: 0x001483CF File Offset: 0x001465CF
	public override void GetInfoStrings(ref List<string> list)
	{
		list.Add(string.Format("Does {0}Have Attached Prefab", this.invert ? "NOT " : ""));
	}

	// Token: 0x0600303C RID: 12348 RVA: 0x001483F8 File Offset: 0x001465F8
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
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

	// Token: 0x040026C8 RID: 9928
	[PublicizedFrom(EAccessModifier.Private)]
	public string prefabName;

	// Token: 0x040026C9 RID: 9929
	[PublicizedFrom(EAccessModifier.Private)]
	public string parent_transform_path;
}

using System;
using System.Xml.Linq;

// Token: 0x020005CA RID: 1482
public class TargetedCompareRequirementBase : RequirementBase
{
	// Token: 0x06002F66 RID: 12134 RVA: 0x001450B8 File Offset: 0x001432B8
	public override bool IsValid(MinEventParams _params)
	{
		if (!base.IsValid(_params))
		{
			return false;
		}
		this.target = null;
		if (this.targetType == TargetedCompareRequirementBase.TargetTypes.other)
		{
			if (_params.Other != null)
			{
				this.target = _params.Other;
			}
		}
		else if (this.targetType == TargetedCompareRequirementBase.TargetTypes.instigator)
		{
			if (_params.Instigator != null)
			{
				this.target = _params.Instigator;
			}
		}
		else if (_params.Self != null)
		{
			this.target = _params.Self;
		}
		return this.target != null;
	}

	// Token: 0x06002F67 RID: 12135 RVA: 0x00145148 File Offset: 0x00143348
	public override bool ParseXAttribute(XAttribute _attribute)
	{
		bool flag = base.ParseXAttribute(_attribute);
		if (!flag && _attribute.Name.LocalName == "target")
		{
			this.targetType = EnumUtils.Parse<TargetedCompareRequirementBase.TargetTypes>(_attribute.Value, true);
			return true;
		}
		return flag;
	}

	// Token: 0x04002685 RID: 9861
	[PublicizedFrom(EAccessModifier.Protected)]
	public TargetedCompareRequirementBase.TargetTypes targetType;

	// Token: 0x04002686 RID: 9862
	[PublicizedFrom(EAccessModifier.Protected)]
	public EntityAlive target;

	// Token: 0x020005CB RID: 1483
	[PublicizedFrom(EAccessModifier.Protected)]
	public enum TargetTypes
	{
		// Token: 0x04002688 RID: 9864
		self,
		// Token: 0x04002689 RID: 9865
		other,
		// Token: 0x0400268A RID: 9866
		instigator
	}
}

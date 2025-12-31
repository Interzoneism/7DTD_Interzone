using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001618 RID: 5656
	[Preserve]
	public class RequirementGroupLiveCount : BaseOperationRequirement
	{
		// Token: 0x0600ADE4 RID: 44516 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADE5 RID: 44517 RVA: 0x00440247 File Offset: 0x0043E447
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			return (float)this.Owner.GetEntityGroupLiveCount(this.targetGroup);
		}

		// Token: 0x0600ADE6 RID: 44518 RVA: 0x0044025B File Offset: 0x0043E45B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return (float)GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600ADE7 RID: 44519 RVA: 0x00440270 File Offset: 0x0043E470
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementGroupLiveCount.PropTargetGroup, ref this.targetGroup);
			properties.ParseString(RequirementGroupLiveCount.PropCount, ref this.valueText);
		}

		// Token: 0x0600ADE8 RID: 44520 RVA: 0x0044029B File Offset: 0x0043E49B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGroupLiveCount
			{
				Invert = this.Invert,
				operation = this.operation,
				targetGroup = this.targetGroup,
				valueText = this.valueText
			};
		}

		// Token: 0x04008706 RID: 34566
		[PublicizedFrom(EAccessModifier.Protected)]
		public string targetGroup = "";

		// Token: 0x04008707 RID: 34567
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008708 RID: 34568
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropTargetGroup = "target_group";

		// Token: 0x04008709 RID: 34569
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropCount = "count";
	}
}

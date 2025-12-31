using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x0200162E RID: 5678
	[Preserve]
	public class RequirementProgression : BaseOperationRequirement
	{
		// Token: 0x0600AE43 RID: 44611 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600AE44 RID: 44612 RVA: 0x00440FBC File Offset: 0x0043F1BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null && entityAlive.Progression != null)
			{
				this.pv = entityAlive.Progression.GetProgressionValue(this.progressionName);
				if (this.pv != null)
				{
					return this.pv.GetCalculatedLevel(entityAlive);
				}
			}
			return 0f;
		}

		// Token: 0x0600AE45 RID: 44613 RVA: 0x0044100C File Offset: 0x0043F20C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return (float)GameEventManager.GetIntValue(target as EntityAlive, this.valueText, 0);
		}

		// Token: 0x0600AE46 RID: 44614 RVA: 0x00441021 File Offset: 0x0043F221
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementProgression.PropProgressionName, ref this.progressionName);
			properties.ParseString(RequirementProgression.PropValue, ref this.valueText);
		}

		// Token: 0x0600AE47 RID: 44615 RVA: 0x0044104C File Offset: 0x0043F24C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementProgression
			{
				Invert = this.Invert,
				operation = this.operation,
				progressionName = this.progressionName,
				valueText = this.valueText
			};
		}

		// Token: 0x0400872D RID: 34605
		[PublicizedFrom(EAccessModifier.Protected)]
		public string progressionName = "";

		// Token: 0x0400872E RID: 34606
		[PublicizedFrom(EAccessModifier.Protected)]
		public ProgressionValue pv;

		// Token: 0x0400872F RID: 34607
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008730 RID: 34608
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropProgressionName = "name";

		// Token: 0x04008731 RID: 34609
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";
	}
}

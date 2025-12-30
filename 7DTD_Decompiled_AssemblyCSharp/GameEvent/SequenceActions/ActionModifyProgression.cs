using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200167A RID: 5754
	[Preserve]
	public class ActionModifyProgression : ActionBaseClientAction
	{
		// Token: 0x0600AF8D RID: 44941 RVA: 0x0044A3F8 File Offset: 0x004485F8
		public override void OnClientPerform(Entity target)
		{
			EntityPlayerLocal entityPlayerLocal = target as EntityPlayerLocal;
			if (entityPlayerLocal != null)
			{
				for (int i = 0; i < this.ProgressionNames.Length; i++)
				{
					string value = (this.Values != null && this.Values.Length > i) ? this.Values[i] : "1";
					ProgressionValue progressionValue = entityPlayerLocal.Progression.GetProgressionValue(this.ProgressionNames[i]);
					int intValue = GameEventManager.GetIntValue(entityPlayerLocal, value, 1);
					ActionModifyProgression.ModifyTypes modifyType = this.ModifyType;
					if (modifyType != ActionModifyProgression.ModifyTypes.Set)
					{
						if (modifyType == ActionModifyProgression.ModifyTypes.Add)
						{
							progressionValue.Level += intValue;
						}
					}
					else
					{
						progressionValue.Level = intValue;
					}
				}
			}
		}

		// Token: 0x0600AF8E RID: 44942 RVA: 0x0044A494 File Offset: 0x00448694
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionModifyProgression.PropProgressionNames))
			{
				this.ProgressionNames = properties.Values[ActionModifyProgression.PropProgressionNames].Replace(" ", "").Split(',', StringSplitOptions.None);
				if (properties.Values.ContainsKey(ActionModifyProgression.PropValues))
				{
					this.Values = properties.Values[ActionModifyProgression.PropValues].Replace(" ", "").Split(',', StringSplitOptions.None);
				}
				else
				{
					this.Values = null;
				}
			}
			else
			{
				this.ProgressionNames = null;
				this.Values = null;
			}
			properties.ParseEnum<ActionModifyProgression.ModifyTypes>(ActionModifyProgression.PropModifyType, ref this.ModifyType);
		}

		// Token: 0x0600AF8F RID: 44943 RVA: 0x0044A550 File Offset: 0x00448750
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionModifyProgression
			{
				ModifyType = this.ModifyType,
				ProgressionNames = this.ProgressionNames,
				Values = this.Values,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008915 RID: 35093
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionModifyProgression.ModifyTypes ModifyType;

		// Token: 0x04008916 RID: 35094
		public string[] ProgressionNames;

		// Token: 0x04008917 RID: 35095
		public string[] Values;

		// Token: 0x04008918 RID: 35096
		public static string PropModifyType = "modify_type";

		// Token: 0x04008919 RID: 35097
		public static string PropProgressionNames = "progression_names";

		// Token: 0x0400891A RID: 35098
		public static string PropValues = "values";

		// Token: 0x0200167B RID: 5755
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum ModifyTypes
		{
			// Token: 0x0400891C RID: 35100
			Set,
			// Token: 0x0400891D RID: 35101
			Add,
			// Token: 0x0400891E RID: 35102
			Remove
		}
	}
}

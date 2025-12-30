using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001677 RID: 5751
	[Preserve]
	public class ActionModifyEntityStat : ActionBaseClientAction
	{
		// Token: 0x0600AF87 RID: 44935 RVA: 0x0044A18C File Offset: 0x0044838C
		public override void OnClientPerform(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				float floatValue = GameEventManager.GetFloatValue(entityAlive, this.valueText, 0f);
				switch (this.Stat)
				{
				case ActionModifyEntityStat.StatTypes.Health:
					entityAlive.Health = (int)this.GetValue(floatValue, (float)entityAlive.Health, (float)entityAlive.GetMaxHealth());
					return;
				case ActionModifyEntityStat.StatTypes.Stamina:
					entityAlive.Stamina = (float)((int)this.GetValue(floatValue, entityAlive.Stamina, (float)entityAlive.GetMaxStamina()));
					return;
				case ActionModifyEntityStat.StatTypes.Food:
					entityAlive.Stats.Food.Value = (float)((int)this.GetValue(floatValue, (float)((int)entityAlive.Stats.Food.Value), (float)((int)entityAlive.Stats.Food.Max)));
					return;
				case ActionModifyEntityStat.StatTypes.Water:
					entityAlive.Stats.Water.Value = (float)((int)this.GetValue(floatValue, (float)((int)entityAlive.Stats.Water.Value), (float)((int)entityAlive.Stats.Water.Max)));
					return;
				case ActionModifyEntityStat.StatTypes.SightRange:
					entityAlive.sightRangeBase = this.GetValue(floatValue, entityAlive.sightRangeBase, entityAlive.sightRangeBase);
					break;
				default:
					return;
				}
			}
		}

		// Token: 0x0600AF88 RID: 44936 RVA: 0x0044A2AC File Offset: 0x004484AC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual float GetValue(float value, float original, float max)
		{
			if (this.isPercent)
			{
				switch (this.operationType)
				{
				case ActionModifyEntityStat.OperationTypes.Set:
					return value * max;
				case ActionModifyEntityStat.OperationTypes.SetMax:
					return max;
				case ActionModifyEntityStat.OperationTypes.Add:
					return original / max + value * max;
				case ActionModifyEntityStat.OperationTypes.Subtract:
					return original / max - value * max;
				case ActionModifyEntityStat.OperationTypes.Multiply:
					return original / max * (value * max);
				}
			}
			else
			{
				switch (this.operationType)
				{
				case ActionModifyEntityStat.OperationTypes.Set:
					return value;
				case ActionModifyEntityStat.OperationTypes.SetMax:
					return max;
				case ActionModifyEntityStat.OperationTypes.Add:
					return original + value;
				case ActionModifyEntityStat.OperationTypes.Subtract:
					return original - value;
				case ActionModifyEntityStat.OperationTypes.Multiply:
					return original * value;
				}
			}
			return 0f;
		}

		// Token: 0x0600AF89 RID: 44937 RVA: 0x0044A33C File Offset: 0x0044853C
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionModifyEntityStat.PropValue, ref this.valueText);
			properties.ParseEnum<ActionModifyEntityStat.StatTypes>(ActionModifyEntityStat.PropStat, ref this.Stat);
			properties.ParseEnum<ActionModifyEntityStat.OperationTypes>(ActionModifyEntityStat.PropOperation, ref this.operationType);
			properties.ParseBool(ActionModifyEntityStat.PropIsPercent, ref this.isPercent);
		}

		// Token: 0x0600AF8A RID: 44938 RVA: 0x0044A394 File Offset: 0x00448594
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionModifyEntityStat
			{
				Stat = this.Stat,
				valueText = this.valueText,
				operationType = this.operationType,
				isPercent = this.isPercent
			};
		}

		// Token: 0x04008901 RID: 35073
		[PublicizedFrom(EAccessModifier.Protected)]
		public string valueText;

		// Token: 0x04008902 RID: 35074
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionModifyEntityStat.StatTypes Stat;

		// Token: 0x04008903 RID: 35075
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionModifyEntityStat.OperationTypes operationType;

		// Token: 0x04008904 RID: 35076
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isPercent;

		// Token: 0x04008905 RID: 35077
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropValue = "value";

		// Token: 0x04008906 RID: 35078
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropStat = "stat";

		// Token: 0x04008907 RID: 35079
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropOperation = "operation";

		// Token: 0x04008908 RID: 35080
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIsPercent = "is_percent";

		// Token: 0x02001678 RID: 5752
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum StatTypes
		{
			// Token: 0x0400890A RID: 35082
			Health,
			// Token: 0x0400890B RID: 35083
			Stamina,
			// Token: 0x0400890C RID: 35084
			Food,
			// Token: 0x0400890D RID: 35085
			Water,
			// Token: 0x0400890E RID: 35086
			SightRange
		}

		// Token: 0x02001679 RID: 5753
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum OperationTypes
		{
			// Token: 0x04008910 RID: 35088
			Set,
			// Token: 0x04008911 RID: 35089
			SetMax,
			// Token: 0x04008912 RID: 35090
			Add,
			// Token: 0x04008913 RID: 35091
			Subtract,
			// Token: 0x04008914 RID: 35092
			Multiply
		}
	}
}

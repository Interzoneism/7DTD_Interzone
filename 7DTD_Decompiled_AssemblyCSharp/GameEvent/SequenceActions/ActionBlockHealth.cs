using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016CB RID: 5835
	[Preserve]
	public class ActionBlockHealth : ActionBaseBlockAction
	{
		// Token: 0x0600B11F RID: 45343 RVA: 0x00451613 File Offset: 0x0044F813
		[PublicizedFrom(EAccessModifier.Protected)]
		public override bool NeedsDamage()
		{
			return this.healthState == ActionBlockHealth.HealthStates.Remove || this.healthState == ActionBlockHealth.HealthStates.RemoveNoBreak;
		}

		// Token: 0x0600B120 RID: 45344 RVA: 0x0045162C File Offset: 0x0044F82C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BlockChangeInfo UpdateBlock(World world, Vector3i currentPos, BlockValue blockValue)
		{
			if (!blockValue.isair)
			{
				switch (this.healthState)
				{
				case ActionBlockHealth.HealthStates.OneHealth:
				{
					int num = blockValue.Block.MaxDamage - 1;
					if (blockValue.damage != num)
					{
						blockValue.damage = num;
						return new BlockChangeInfo(0, currentPos, blockValue);
					}
					break;
				}
				case ActionBlockHealth.HealthStates.Half:
				{
					int num2 = blockValue.Block.MaxDamage / 2;
					if (blockValue.damage != num2)
					{
						blockValue.damage = num2;
						return new BlockChangeInfo(0, currentPos, blockValue);
					}
					break;
				}
				case ActionBlockHealth.HealthStates.Full:
					if (blockValue.damage != 0)
					{
						blockValue.damage = 0;
						return new BlockChangeInfo(0, currentPos, blockValue);
					}
					break;
				case ActionBlockHealth.HealthStates.Remove:
				{
					int num3 = blockValue.damage + this.amount;
					if (blockValue.damage != num3)
					{
						blockValue.damage = num3;
						if (blockValue.damage >= blockValue.Block.MaxDamage)
						{
							blockValue = blockValue.Block.DowngradeBlock;
						}
						return new BlockChangeInfo(0, currentPos, blockValue);
					}
					break;
				}
				case ActionBlockHealth.HealthStates.RemoveNoBreak:
				{
					int num4 = Mathf.Min(blockValue.Block.MaxDamage - 1, blockValue.damage + this.amount);
					if (blockValue.damage != num4)
					{
						blockValue.damage = num4;
						return new BlockChangeInfo(0, currentPos, blockValue);
					}
					break;
				}
				}
			}
			return null;
		}

		// Token: 0x0600B121 RID: 45345 RVA: 0x00451768 File Offset: 0x0044F968
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			this.Properties.ParseEnum<ActionBlockHealth.HealthStates>(ActionBlockHealth.PropHealthState, ref this.healthState);
			this.Properties.ParseInt(ActionBlockHealth.PropHealthAmount, ref this.amount);
		}

		// Token: 0x0600B122 RID: 45346 RVA: 0x0045179D File Offset: 0x0044F99D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionBlockHealth
			{
				healthState = this.healthState,
				amount = this.amount
			};
		}

		// Token: 0x04008A9F RID: 35487
		[PublicizedFrom(EAccessModifier.Protected)]
		public ActionBlockHealth.HealthStates healthState = ActionBlockHealth.HealthStates.Full;

		// Token: 0x04008AA0 RID: 35488
		[PublicizedFrom(EAccessModifier.Protected)]
		public int amount;

		// Token: 0x04008AA1 RID: 35489
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropHealthState = "health_state";

		// Token: 0x04008AA2 RID: 35490
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropHealthAmount = "health_amount";

		// Token: 0x020016CC RID: 5836
		[PublicizedFrom(EAccessModifier.Protected)]
		public enum HealthStates
		{
			// Token: 0x04008AA4 RID: 35492
			OneHealth,
			// Token: 0x04008AA5 RID: 35493
			Half,
			// Token: 0x04008AA6 RID: 35494
			Full,
			// Token: 0x04008AA7 RID: 35495
			Remove,
			// Token: 0x04008AA8 RID: 35496
			RemoveNoBreak
		}
	}
}

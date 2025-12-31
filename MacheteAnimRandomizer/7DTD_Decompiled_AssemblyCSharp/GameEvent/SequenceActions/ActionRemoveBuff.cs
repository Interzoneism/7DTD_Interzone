using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001688 RID: 5768
	[Preserve]
	public class ActionRemoveBuff : ActionBaseTargetAction
	{
		// Token: 0x0600AFD0 RID: 45008 RVA: 0x0044B744 File Offset: 0x00449944
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.BuffList = this.buffName.Split(',', StringSplitOptions.None);
		}

		// Token: 0x0600AFD1 RID: 45009 RVA: 0x0044B75C File Offset: 0x0044995C
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				for (int i = 0; i < this.BuffList.Length; i++)
				{
					if (entityAlive.Buffs.HasBuff(this.BuffList[i]))
					{
						entityAlive.Buffs.RemoveBuff(this.BuffList[i], true);
					}
				}
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFD2 RID: 45010 RVA: 0x0044B7B6 File Offset: 0x004499B6
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionRemoveBuff.PropBuffName))
			{
				this.buffName = properties.Values[ActionRemoveBuff.PropBuffName];
			}
		}

		// Token: 0x0600AFD3 RID: 45011 RVA: 0x0044B7E7 File Offset: 0x004499E7
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveBuff
			{
				buffName = this.buffName,
				BuffList = this.BuffList,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x0400895D RID: 35165
		[PublicizedFrom(EAccessModifier.Protected)]
		public string buffName = "";

		// Token: 0x0400895E RID: 35166
		[PublicizedFrom(EAccessModifier.Protected)]
		public string[] BuffList;

		// Token: 0x0400895F RID: 35167
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBuffName = "buff_name";
	}
}

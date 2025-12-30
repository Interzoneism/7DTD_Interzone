using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x02001689 RID: 5769
	[Preserve]
	public class ActionRemoveDeathBuffs : ActionBaseTargetAction
	{
		// Token: 0x0600AFD6 RID: 45014 RVA: 0x0044B831 File Offset: 0x00449A31
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.tags = FastTags<TagGroup.Global>.Parse(this.excludeTags);
		}

		// Token: 0x0600AFD7 RID: 45015 RVA: 0x0044B844 File Offset: 0x00449A44
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.Buffs.RemoveDeathBuffs(this.tags);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFD8 RID: 45016 RVA: 0x0044B873 File Offset: 0x00449A73
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionRemoveDeathBuffs.PropExcludeTags))
			{
				this.excludeTags = properties.Values[ActionRemoveDeathBuffs.PropExcludeTags];
			}
		}

		// Token: 0x0600AFD9 RID: 45017 RVA: 0x0044B8A4 File Offset: 0x00449AA4
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveDeathBuffs
			{
				excludeTags = this.excludeTags,
				tags = this.tags,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008960 RID: 35168
		[PublicizedFrom(EAccessModifier.Protected)]
		public string excludeTags = "";

		// Token: 0x04008961 RID: 35169
		[PublicizedFrom(EAccessModifier.Protected)]
		public FastTags<TagGroup.Global> tags;

		// Token: 0x04008962 RID: 35170
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropExcludeTags = "exclude_tags";
	}
}

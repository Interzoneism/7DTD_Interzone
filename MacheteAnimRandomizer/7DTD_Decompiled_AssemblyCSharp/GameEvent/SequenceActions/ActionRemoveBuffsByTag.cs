using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x0200168E RID: 5774
	[Preserve]
	public class ActionRemoveBuffsByTag : ActionBaseTargetAction
	{
		// Token: 0x0600AFF1 RID: 45041 RVA: 0x0044BC8C File Offset: 0x00449E8C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
			this.tags = FastTags<TagGroup.Global>.Parse(this.buffTag);
		}

		// Token: 0x0600AFF2 RID: 45042 RVA: 0x0044BCA0 File Offset: 0x00449EA0
		public override BaseAction.ActionCompleteStates PerformTargetAction(Entity target)
		{
			EntityAlive entityAlive = target as EntityAlive;
			if (entityAlive != null)
			{
				entityAlive.Buffs.RemoveBuffsByTag(this.tags);
			}
			return BaseAction.ActionCompleteStates.Complete;
		}

		// Token: 0x0600AFF3 RID: 45043 RVA: 0x0044BCCF File Offset: 0x00449ECF
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			if (properties.Values.ContainsKey(ActionRemoveBuffsByTag.PropBuffTags))
			{
				this.buffTag = properties.Values[ActionRemoveBuffsByTag.PropBuffTags];
			}
		}

		// Token: 0x0600AFF4 RID: 45044 RVA: 0x0044BD00 File Offset: 0x00449F00
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionRemoveBuffsByTag
			{
				buffTag = this.buffTag,
				tags = this.tags,
				targetGroup = this.targetGroup
			};
		}

		// Token: 0x04008968 RID: 35176
		[PublicizedFrom(EAccessModifier.Protected)]
		public string buffTag = "";

		// Token: 0x04008969 RID: 35177
		[PublicizedFrom(EAccessModifier.Protected)]
		public FastTags<TagGroup.Global> tags;

		// Token: 0x0400896A RID: 35178
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropBuffTags = "buff_tag";
	}
}

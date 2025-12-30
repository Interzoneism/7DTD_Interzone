using System;
using UnityEngine;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001622 RID: 5666
	[Preserve]
	public class RequirementInQuestZone : BaseRequirement
	{
		// Token: 0x0600AE18 RID: 44568 RVA: 0x00440938 File Offset: 0x0043EB38
		public override bool CanPerform(Entity target)
		{
			World world = GameManager.Instance.World;
			Vector3 position = target.position;
			position.y = position.z;
			if (QuestEventManager.Current.QuestBounds.Contains(position))
			{
				return !this.Invert;
			}
			return this.Invert;
		}

		// Token: 0x0600AE19 RID: 44569 RVA: 0x00440986 File Offset: 0x0043EB86
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementInQuestZone();
		}
	}
}

using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016A5 RID: 5797
	[Preserve]
	public class ActionAddPlayerLevel : ActionBaseClientAction
	{
		// Token: 0x0600B05F RID: 45151 RVA: 0x0044E5E8 File Offset: 0x0044C7E8
		public override void OnClientPerform(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			if (entityPlayer != null)
			{
				int intValue = GameEventManager.GetIntValue(entityPlayer, this.addedLevelsText, 1);
				for (int i = 0; i < intValue; i++)
				{
					entityPlayer.Progression.AddLevelExp(entityPlayer.Progression.ExpToNextLevel, "_xpOther", Progression.XPTypes.Other, false, i == intValue - 1);
				}
			}
		}

		// Token: 0x0600B060 RID: 45152 RVA: 0x0044E63D File Offset: 0x0044C83D
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(ActionAddPlayerLevel.PropNewLevel, ref this.addedLevelsText);
		}

		// Token: 0x0600B061 RID: 45153 RVA: 0x0044E657 File Offset: 0x0044C857
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseAction CloneChildSettings()
		{
			return new ActionAddPlayerLevel
			{
				addedLevelsText = this.addedLevelsText
			};
		}

		// Token: 0x040089E9 RID: 35305
		[PublicizedFrom(EAccessModifier.Protected)]
		public string addedLevelsText;

		// Token: 0x040089EA RID: 35306
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropNewLevel = "levels";
	}
}

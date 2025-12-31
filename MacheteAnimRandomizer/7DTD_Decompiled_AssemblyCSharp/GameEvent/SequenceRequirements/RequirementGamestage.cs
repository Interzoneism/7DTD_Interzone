using System;
using UnityEngine.Scripting;

namespace GameEvent.SequenceRequirements
{
	// Token: 0x02001614 RID: 5652
	[Preserve]
	public class RequirementGamestage : BaseOperationRequirement
	{
		// Token: 0x0600ADCA RID: 44490 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void OnInit()
		{
		}

		// Token: 0x0600ADCB RID: 44491 RVA: 0x0043FFEC File Offset: 0x0043E1EC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float LeftSide(Entity target)
		{
			EntityPlayer entityPlayer = target as EntityPlayer;
			return (float)((entityPlayer != null) ? entityPlayer.gameStage : 0);
		}

		// Token: 0x0600ADCC RID: 44492 RVA: 0x0044000D File Offset: 0x0043E20D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override float RightSide(Entity target)
		{
			return (float)GameEventManager.GetIntValue(target as EntityAlive, this.gamestageText, 0);
		}

		// Token: 0x0600ADCD RID: 44493 RVA: 0x00440022 File Offset: 0x0043E222
		public override void ParseProperties(DynamicProperties properties)
		{
			base.ParseProperties(properties);
			properties.ParseString(RequirementGamestage.PropGamestage, ref this.gamestageText);
		}

		// Token: 0x0600ADCE RID: 44494 RVA: 0x0044003C File Offset: 0x0043E23C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override BaseRequirement CloneChildSettings()
		{
			return new RequirementGamestage
			{
				Invert = this.Invert,
				operation = this.operation,
				gamestageText = this.gamestageText
			};
		}

		// Token: 0x040086FA RID: 34554
		[PublicizedFrom(EAccessModifier.Protected)]
		public string gamestageText;

		// Token: 0x040086FB RID: 34555
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropGamestage = "game_stage";
	}
}

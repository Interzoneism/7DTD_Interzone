using System;
using UnityEngine.Scripting;

// Token: 0x0200029C RID: 668
[Preserve]
public class DialogActionAddBuff : BaseDialogAction
{
	// Token: 0x17000202 RID: 514
	// (get) Token: 0x06001301 RID: 4865 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override BaseDialogAction.ActionTypes ActionType
	{
		get
		{
			return BaseDialogAction.ActionTypes.AddBuff;
		}
	}

	// Token: 0x06001302 RID: 4866 RVA: 0x000758CC File Offset: 0x00073ACC
	public override void PerformAction(EntityPlayer player)
	{
		EntityPlayer primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		if (primaryPlayer != null)
		{
			EntityBuffs.BuffStatus buffStatus = primaryPlayer.Buffs.AddBuff(base.ID, -1, true, false, -1f);
			if (buffStatus != EntityBuffs.BuffStatus.Added)
			{
				switch (buffStatus)
				{
				case EntityBuffs.BuffStatus.FailedInvalidName:
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: buff \"" + base.ID + "\" unknown");
					return;
				case EntityBuffs.BuffStatus.FailedImmune:
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: entity is immune to \"" + base.ID);
					return;
				case EntityBuffs.BuffStatus.FailedFriendlyFire:
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: entity is friendly");
					return;
				case EntityBuffs.BuffStatus.FailedEditor:
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: buff " + base.ID + " not allowed in editor.");
					return;
				case EntityBuffs.BuffStatus.FailedGameStat:
					SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Buff failed: missing required game stat.");
					break;
				default:
					return;
				}
			}
		}
	}

	// Token: 0x04000C8A RID: 3210
	[PublicizedFrom(EAccessModifier.Private)]
	public string name = "";
}

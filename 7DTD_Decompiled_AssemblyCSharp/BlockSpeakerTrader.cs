using System;
using Audio;
using UnityEngine.Scripting;

// Token: 0x0200013D RID: 317
[Preserve]
public class BlockSpeakerTrader : Block
{
	// Token: 0x060008DE RID: 2270 RVA: 0x0003E044 File Offset: 0x0003C244
	public override void Init()
	{
		base.Init();
		if (base.Properties.Values.ContainsKey("OpenSound"))
		{
			this.openSound = base.Properties.Values["OpenSound"];
		}
		if (base.Properties.Values.ContainsKey("CloseSound"))
		{
			this.closeSound = base.Properties.Values["CloseSound"];
		}
		if (base.Properties.Values.ContainsKey("WarningSound"))
		{
			this.warningSound = base.Properties.Values["WarningSound"];
		}
	}

	// Token: 0x060008DF RID: 2271 RVA: 0x0003E0F0 File Offset: 0x0003C2F0
	public void PlayOpen(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.openSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_open") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x060008E0 RID: 2272 RVA: 0x0003E154 File Offset: 0x0003C354
	public void PlayClose(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.closeSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_closed") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x060008E1 RID: 2273 RVA: 0x0003E1B8 File Offset: 0x0003C3B8
	public void PlayWarning(Vector3i _blockPos, EntityTrader _trader)
	{
		string text = this.warningSound;
		if (string.IsNullOrEmpty(text))
		{
			text = ((_trader != null) ? (_trader.NPCInfo.VoiceSet + "_announce_closing") : "");
		}
		if (text != "")
		{
			Manager.BroadcastPlay(_blockPos.ToVector3(), text, 0f);
		}
	}

	// Token: 0x040008AE RID: 2222
	[PublicizedFrom(EAccessModifier.Private)]
	public string openSound;

	// Token: 0x040008AF RID: 2223
	[PublicizedFrom(EAccessModifier.Private)]
	public string closeSound;

	// Token: 0x040008B0 RID: 2224
	[PublicizedFrom(EAccessModifier.Private)]
	public string warningSound;
}

using System;
using System.Text;
using LiteNetLib;

// Token: 0x020007CB RID: 1995
public static class NetworkCommonLiteNetLib
{
	// Token: 0x06003990 RID: 14736 RVA: 0x00173BD0 File Offset: 0x00171DD0
	public static bool InitConfig(NetManager _manager)
	{
		_manager.UnsyncedEvents = true;
		_manager.UnsyncedDeliveryEvent = true;
		_manager.UnsyncedReceiveEvent = true;
		_manager.AutoRecycle = true;
		_manager.DisconnectOnUnreachable = true;
		bool @bool = GamePrefs.GetBool(EnumGamePrefs.OptionsLiteNetLibMtuOverride);
		if (@bool)
		{
			Log.Out(string.Format("NET: LiteNetLib: MTU Override enabled ({0})", 1024));
		}
		_manager.MtuOverride = (@bool ? 1024 : 0);
		return true;
	}

	// Token: 0x06003991 RID: 14737 RVA: 0x00173C3C File Offset: 0x00171E3C
	public static byte[] CreateRejectMessage(string _customText)
	{
		int byteCount = Encoding.UTF8.GetByteCount(_customText);
		byte[] array = new byte[2 + byteCount];
		array[0] = byte.MaxValue;
		array[1] = (byte)Encoding.UTF8.GetBytes(_customText, 0, _customText.Length, array, 2);
		return array;
	}

	// Token: 0x04002E7C RID: 11900
	public const int PORT_OFFSET = 2;

	// Token: 0x04002E7D RID: 11901
	public const int MTU_OVERRIDE = 1024;

	// Token: 0x04002E7E RID: 11902
	public const byte ChallengePackageChannelId = 202;

	// Token: 0x020007CC RID: 1996
	public enum EAdditionalDisconnectCause : byte
	{
		// Token: 0x04002E80 RID: 11904
		InvalidPassword,
		// Token: 0x04002E81 RID: 11905
		RateLimit,
		// Token: 0x04002E82 RID: 11906
		PendingConnection,
		// Token: 0x04002E83 RID: 11907
		ServerShutdown,
		// Token: 0x04002E84 RID: 11908
		ClientSideDisconnect,
		// Token: 0x04002E85 RID: 11909
		Other = 255
	}
}

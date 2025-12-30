using System;
using Audio;
using Twitch;
using UnityEngine.Scripting;

// Token: 0x020007A2 RID: 1954
[Preserve]
public class NetPackageTwitchAccess : NetPackage
{
	// Token: 0x06003894 RID: 14484 RVA: 0x00112051 File Offset: 0x00110251
	public NetPackageTwitchAccess Setup()
	{
		return this;
	}

	// Token: 0x06003895 RID: 14485 RVA: 0x001704A9 File Offset: 0x0016E6A9
	public NetPackageTwitchAccess Setup(bool _hasAccess)
	{
		this.hasAccess = _hasAccess;
		return this;
	}

	// Token: 0x06003896 RID: 14486 RVA: 0x001704B3 File Offset: 0x0016E6B3
	public override void read(PooledBinaryReader _br)
	{
		this.hasAccess = _br.ReadBoolean();
	}

	// Token: 0x06003897 RID: 14487 RVA: 0x001704C1 File Offset: 0x0016E6C1
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.hasAccess);
	}

	// Token: 0x06003898 RID: 14488 RVA: 0x001704D8 File Offset: 0x0016E6D8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			AdminTools adminTools = GameManager.Instance.adminTools;
			bool flag = ((adminTools != null) ? adminTools.Users.GetUserPermissionLevel(base.Sender) : 1000) <= GamePrefs.GetInt(EnumGamePrefs.TwitchServerPermission);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageTwitchAccess>().Setup(flag), false, base.Sender.entityId, -1, -1, null, 192, false);
			return;
		}
		if (this.hasAccess)
		{
			GameEventManager.Current.HandleGameEventAccessApproved();
			return;
		}
		Manager.PlayInsidePlayerHead("Misc/password_fail", -1, 0f, false, false);
		TwitchManager.Current.DeniedPermission();
	}

	// Token: 0x06003899 RID: 14489 RVA: 0x000F298B File Offset: 0x000F0B8B
	public override int GetLength()
	{
		return 30;
	}

	// Token: 0x04002DD7 RID: 11735
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasAccess;
}

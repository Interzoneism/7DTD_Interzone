using System;
using UnityEngine.Scripting;

// Token: 0x02000710 RID: 1808
[Preserve]
public class NetPackageCloseAllWindows : NetPackage
{
	// Token: 0x17000562 RID: 1378
	// (get) Token: 0x0600352C RID: 13612 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x0600352D RID: 13613 RVA: 0x00162C89 File Offset: 0x00160E89
	public NetPackageCloseAllWindows Setup(int entityToClose)
	{
		this._playerIdToClose = entityToClose;
		return this;
	}

	// Token: 0x0600352E RID: 13614 RVA: 0x00162C93 File Offset: 0x00160E93
	public override void read(PooledBinaryReader _reader)
	{
		this._playerIdToClose = _reader.ReadInt32();
	}

	// Token: 0x0600352F RID: 13615 RVA: 0x00162CA1 File Offset: 0x00160EA1
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this._playerIdToClose);
	}

	// Token: 0x06003530 RID: 13616 RVA: 0x00162CB8 File Offset: 0x00160EB8
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			EntityPlayerLocal localPlayerFromID = GameManager.Instance.World.GetLocalPlayerFromID(this._playerIdToClose);
			if (localPlayerFromID != null)
			{
				localPlayerFromID.PlayerUI.windowManager.CloseAllOpenWindows(null, false);
			}
		}
	}

	// Token: 0x06003531 RID: 13617 RVA: 0x00075CC0 File Offset: 0x00073EC0
	public override int GetLength()
	{
		return 4;
	}

	// Token: 0x04002B56 RID: 11094
	[PublicizedFrom(EAccessModifier.Private)]
	public int _playerIdToClose = -1;
}

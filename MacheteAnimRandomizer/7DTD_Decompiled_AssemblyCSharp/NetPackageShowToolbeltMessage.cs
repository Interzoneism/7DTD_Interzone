using System;
using UnityEngine.Scripting;

// Token: 0x02000793 RID: 1939
[Preserve]
public class NetPackageShowToolbeltMessage : NetPackage
{
	// Token: 0x0600383F RID: 14399 RVA: 0x0016F1D5 File Offset: 0x0016D3D5
	public NetPackageShowToolbeltMessage Setup(string _toolbeltMessage, string _sound)
	{
		this.toolbeltMessage = _toolbeltMessage;
		this.sound = _sound;
		return this;
	}

	// Token: 0x06003840 RID: 14400 RVA: 0x0016F1E6 File Offset: 0x0016D3E6
	public override void read(PooledBinaryReader _br)
	{
		this.toolbeltMessage = _br.ReadString();
		this.sound = _br.ReadString();
	}

	// Token: 0x06003841 RID: 14401 RVA: 0x0016F200 File Offset: 0x0016D400
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.toolbeltMessage);
		_bw.Write(this.sound);
	}

	// Token: 0x06003842 RID: 14402 RVA: 0x0016F221 File Offset: 0x0016D421
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		GameManager.ShowTooltip(_world.GetLocalPlayers()[0], this.toolbeltMessage, this.sound, null, null, false, false, 0f);
	}

	// Token: 0x170005B2 RID: 1458
	// (get) Token: 0x06003843 RID: 14403 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003844 RID: 14404 RVA: 0x0015DE68 File Offset: 0x0015C068
	public override int GetLength()
	{
		return 80;
	}

	// Token: 0x04002DA3 RID: 11683
	[PublicizedFrom(EAccessModifier.Private)]
	public string toolbeltMessage;

	// Token: 0x04002DA4 RID: 11684
	[PublicizedFrom(EAccessModifier.Private)]
	public string sound = "";
}

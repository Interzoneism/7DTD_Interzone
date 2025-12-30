using System;
using UnityEngine.Scripting;

// Token: 0x02000790 RID: 1936
[Preserve]
public class NetPackageSharedPartyKill : NetPackage
{
	// Token: 0x06003830 RID: 14384 RVA: 0x0016E96C File Offset: 0x0016CB6C
	public NetPackageSharedPartyKill Setup(int _entityID, int _killerID)
	{
		this.entityID = _entityID;
		this.killerID = _killerID;
		return this;
	}

	// Token: 0x06003831 RID: 14385 RVA: 0x0016E97D File Offset: 0x0016CB7D
	public NetPackageSharedPartyKill Setup(int _entityTypeID, int _xp, int _killerID, int _killedEntityID)
	{
		this.entityTypeID = _entityTypeID;
		this.xp = _xp;
		this.killerID = _killerID;
		this.entityID = _killedEntityID;
		return this;
	}

	// Token: 0x06003832 RID: 14386 RVA: 0x0016E99D File Offset: 0x0016CB9D
	public override void read(PooledBinaryReader _br)
	{
		this.entityTypeID = _br.ReadInt32();
		this.xp = _br.ReadInt32();
		this.entityID = _br.ReadInt32();
		this.killerID = _br.ReadInt32();
	}

	// Token: 0x06003833 RID: 14387 RVA: 0x0016E9CF File Offset: 0x0016CBCF
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityTypeID);
		_bw.Write(this.xp);
		_bw.Write(this.entityID);
		_bw.Write(this.killerID);
	}

	// Token: 0x06003834 RID: 14388 RVA: 0x0016EA08 File Offset: 0x0016CC08
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GameManager.Instance.SharedKillServer(this.entityID, this.killerID, 1f);
			return;
		}
		GameManager.Instance.SharedKillClient(this.entityTypeID, this.xp, null, this.entityID);
	}

	// Token: 0x06003835 RID: 14389 RVA: 0x000445B9 File Offset: 0x000427B9
	public override int GetLength()
	{
		return 25;
	}

	// Token: 0x04002D90 RID: 11664
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityTypeID;

	// Token: 0x04002D91 RID: 11665
	[PublicizedFrom(EAccessModifier.Private)]
	public int xp;

	// Token: 0x04002D92 RID: 11666
	[PublicizedFrom(EAccessModifier.Private)]
	public int killerID;

	// Token: 0x04002D93 RID: 11667
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityID;
}

using System;
using UnityEngine.Scripting;

// Token: 0x0200079C RID: 1948
[Preserve]
public class NetPackageTELock : NetPackage
{
	// Token: 0x06003873 RID: 14451 RVA: 0x0016F958 File Offset: 0x0016DB58
	public NetPackageTELock Setup(NetPackageTELock.TELockType _type, int _clrIdx, Vector3i _pos, int _lootEntityId, int _entityIdThatOpenedIt, string _customUi = null, bool _allowEmptyDestroy = true)
	{
		this.type = _type;
		this.clrIdx = _clrIdx;
		this.posX = _pos.x;
		this.posY = _pos.y;
		this.posZ = _pos.z;
		this.lootEntityId = _lootEntityId;
		this.entityIdThatOpenedIt = _entityIdThatOpenedIt;
		this.customUi = (_customUi ?? "");
		this.allowEmptyDestroy = _allowEmptyDestroy;
		return this;
	}

	// Token: 0x06003874 RID: 14452 RVA: 0x0016F9C4 File Offset: 0x0016DBC4
	public override void read(PooledBinaryReader _br)
	{
		this.type = (NetPackageTELock.TELockType)_br.ReadByte();
		this.clrIdx = (int)_br.ReadInt16();
		this.posX = _br.ReadInt32();
		this.posY = _br.ReadInt32();
		this.posZ = _br.ReadInt32();
		this.lootEntityId = _br.ReadInt32();
		this.entityIdThatOpenedIt = _br.ReadInt32();
		this.customUi = _br.ReadString();
		this.allowEmptyDestroy = _br.ReadBoolean();
	}

	// Token: 0x06003875 RID: 14453 RVA: 0x0016FA40 File Offset: 0x0016DC40
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write((byte)this.type);
		_bw.Write((short)this.clrIdx);
		_bw.Write(this.posX);
		_bw.Write(this.posY);
		_bw.Write(this.posZ);
		_bw.Write(this.lootEntityId);
		_bw.Write(this.entityIdThatOpenedIt);
		_bw.Write(this.customUi);
		_bw.Write(this.allowEmptyDestroy);
	}

	// Token: 0x06003876 RID: 14454 RVA: 0x0016FAC4 File Offset: 0x0016DCC4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (this.type != NetPackageTELock.TELockType.UnlockServer && !base.ValidEntityIdForSender(this.entityIdThatOpenedIt, false))
		{
			return;
		}
		switch (this.type)
		{
		case NetPackageTELock.TELockType.LockServer:
			_world.GetGameManager().TELockServer(this.clrIdx, new Vector3i(this.posX, this.posY, this.posZ), this.lootEntityId, this.entityIdThatOpenedIt, this.customUi);
			return;
		case NetPackageTELock.TELockType.UnlockServer:
			_world.GetGameManager().TEUnlockServer(this.clrIdx, new Vector3i(this.posX, this.posY, this.posZ), this.lootEntityId, this.allowEmptyDestroy);
			return;
		case NetPackageTELock.TELockType.AccessClient:
			_world.GetGameManager().TEAccessClient(this.clrIdx, new Vector3i(this.posX, this.posY, this.posZ), this.lootEntityId, this.entityIdThatOpenedIt, this.customUi);
			return;
		case NetPackageTELock.TELockType.DeniedAccess:
			_world.GetGameManager().TEDeniedAccessClient(this.clrIdx, new Vector3i(this.posX, this.posY, this.posZ), this.lootEntityId, this.entityIdThatOpenedIt);
			return;
		default:
			return;
		}
	}

	// Token: 0x06003877 RID: 14455 RVA: 0x0016FBE7 File Offset: 0x0016DDE7
	public override int GetLength()
	{
		return 27 + this.customUi.Length * 2 + 1;
	}

	// Token: 0x04002DB8 RID: 11704
	[PublicizedFrom(EAccessModifier.Private)]
	public NetPackageTELock.TELockType type;

	// Token: 0x04002DB9 RID: 11705
	[PublicizedFrom(EAccessModifier.Private)]
	public int clrIdx;

	// Token: 0x04002DBA RID: 11706
	[PublicizedFrom(EAccessModifier.Private)]
	public int posX;

	// Token: 0x04002DBB RID: 11707
	[PublicizedFrom(EAccessModifier.Private)]
	public int posY;

	// Token: 0x04002DBC RID: 11708
	[PublicizedFrom(EAccessModifier.Private)]
	public int posZ;

	// Token: 0x04002DBD RID: 11709
	[PublicizedFrom(EAccessModifier.Private)]
	public int lootEntityId;

	// Token: 0x04002DBE RID: 11710
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityIdThatOpenedIt;

	// Token: 0x04002DBF RID: 11711
	[PublicizedFrom(EAccessModifier.Private)]
	public string customUi;

	// Token: 0x04002DC0 RID: 11712
	[PublicizedFrom(EAccessModifier.Private)]
	public bool allowEmptyDestroy;

	// Token: 0x0200079D RID: 1949
	public enum TELockType : byte
	{
		// Token: 0x04002DC2 RID: 11714
		LockServer,
		// Token: 0x04002DC3 RID: 11715
		UnlockServer,
		// Token: 0x04002DC4 RID: 11716
		AccessClient,
		// Token: 0x04002DC5 RID: 11717
		DeniedAccess
	}
}

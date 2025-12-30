using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000755 RID: 1877
[Preserve]
public class NetPackageItemDrop : NetPackage
{
	// Token: 0x1700058C RID: 1420
	// (get) Token: 0x060036C3 RID: 14019 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060036C4 RID: 14020 RVA: 0x001685E0 File Offset: 0x001667E0
	public NetPackageItemDrop Setup(ItemStack _itemStack, Vector3 _dropPos, Vector3 _initialMotion, Vector3 _randomPosAdd, float _lifetime, int _entityId, bool _bDropPosIsRelativeToHead, int _clientInstanceId)
	{
		this.itemStack = _itemStack.Clone();
		this.dropPos = _dropPos;
		this.initialMotion = _initialMotion;
		this.randomPosAdd = _randomPosAdd;
		this.lifetime = _lifetime;
		this.entityId = _entityId;
		this.clientInstanceId = _clientInstanceId;
		this.bDropPosIsRelativeToHead = _bDropPosIsRelativeToHead;
		return this;
	}

	// Token: 0x060036C5 RID: 14021 RVA: 0x00168630 File Offset: 0x00166830
	public override void read(PooledBinaryReader _br)
	{
		this.itemStack = new ItemStack();
		this.itemStack.Read(_br);
		this.dropPos = StreamUtils.ReadVector3(_br);
		this.initialMotion = StreamUtils.ReadVector3(_br);
		this.randomPosAdd = StreamUtils.ReadVector3(_br);
		this.lifetime = _br.ReadSingle();
		this.entityId = _br.ReadInt32();
		this.clientInstanceId = _br.ReadInt32();
		this.bDropPosIsRelativeToHead = _br.ReadBoolean();
	}

	// Token: 0x060036C6 RID: 14022 RVA: 0x001686AC File Offset: 0x001668AC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		this.itemStack.Write(_bw);
		StreamUtils.Write(_bw, this.dropPos);
		StreamUtils.Write(_bw, this.initialMotion);
		StreamUtils.Write(_bw, this.randomPosAdd);
		_bw.Write(this.lifetime);
		_bw.Write(this.entityId);
		_bw.Write(this.clientInstanceId);
		_bw.Write(this.bDropPosIsRelativeToHead);
	}

	// Token: 0x060036C7 RID: 14023 RVA: 0x00168724 File Offset: 0x00166924
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		_world.GetGameManager().ItemDropServer(this.itemStack, this.dropPos, this.randomPosAdd, this.initialMotion, this.entityId, this.lifetime, this.bDropPosIsRelativeToHead, this.clientInstanceId);
	}

	// Token: 0x060036C8 RID: 14024 RVA: 0x00168770 File Offset: 0x00166970
	public override int GetLength()
	{
		return 52;
	}

	// Token: 0x04002C73 RID: 11379
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemStack itemStack;

	// Token: 0x04002C74 RID: 11380
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 dropPos;

	// Token: 0x04002C75 RID: 11381
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 initialMotion;

	// Token: 0x04002C76 RID: 11382
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 randomPosAdd;

	// Token: 0x04002C77 RID: 11383
	[PublicizedFrom(EAccessModifier.Private)]
	public float lifetime;

	// Token: 0x04002C78 RID: 11384
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C79 RID: 11385
	[PublicizedFrom(EAccessModifier.Private)]
	public int clientInstanceId;

	// Token: 0x04002C7A RID: 11386
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDropPosIsRelativeToHead;
}

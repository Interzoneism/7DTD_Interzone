using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000754 RID: 1876
[Preserve]
public class NetPackageItemActionEffects : NetPackage
{
	// Token: 0x060036BD RID: 14013 RVA: 0x00168400 File Offset: 0x00166600
	public NetPackageItemActionEffects Setup(int _entityId, int _slotIdx, int _actionIdx, ItemActionFiringState _firingState, Vector3 _startPos, Vector3 _direction, int _userData)
	{
		this.entityId = _entityId;
		this.slotIdx = (byte)_slotIdx;
		this.actionIdx = (byte)_actionIdx;
		this.firingState = _firingState;
		this.startPos = _startPos;
		this.direction = _direction;
		this.userData = _userData;
		return this;
	}

	// Token: 0x060036BE RID: 14014 RVA: 0x0016843C File Offset: 0x0016663C
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.slotIdx = _reader.ReadByte();
		this.actionIdx = _reader.ReadByte();
		this.firingState = (ItemActionFiringState)_reader.ReadByte();
		if (_reader.ReadBoolean())
		{
			this.startPos = StreamUtils.ReadVector3(_reader);
			this.direction = StreamUtils.ReadVector3(_reader);
		}
		else
		{
			this.startPos = Vector3.zero;
			this.direction = Vector3.zero;
		}
		this.userData = _reader.ReadInt32();
	}

	// Token: 0x060036BF RID: 14015 RVA: 0x001684C0 File Offset: 0x001666C0
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.slotIdx);
		_writer.Write(this.actionIdx);
		_writer.Write((byte)this.firingState);
		bool flag = !this.startPos.Equals(Vector3.zero) || !this.direction.Equals(Vector3.zero);
		_writer.Write(flag);
		if (flag)
		{
			StreamUtils.Write(_writer, this.startPos);
			StreamUtils.Write(_writer, this.direction);
		}
		_writer.Write(this.userData);
	}

	// Token: 0x060036C0 RID: 14016 RVA: 0x0016855C File Offset: 0x0016675C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.GetGameManager().ItemActionEffectsServer(this.entityId, (int)this.slotIdx, (int)this.actionIdx, (int)this.firingState, this.startPos, this.direction, this.userData);
			return;
		}
		_world.GetGameManager().ItemActionEffectsClient(this.entityId, (int)this.slotIdx, (int)this.actionIdx, (int)this.firingState, this.startPos, this.direction, this.userData);
	}

	// Token: 0x060036C1 RID: 14017 RVA: 0x0015DCEC File Offset: 0x0015BEEC
	public override int GetLength()
	{
		return 50;
	}

	// Token: 0x04002C6C RID: 11372
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C6D RID: 11373
	[PublicizedFrom(EAccessModifier.Private)]
	public byte slotIdx;

	// Token: 0x04002C6E RID: 11374
	[PublicizedFrom(EAccessModifier.Private)]
	public byte actionIdx;

	// Token: 0x04002C6F RID: 11375
	[PublicizedFrom(EAccessModifier.Private)]
	public ItemActionFiringState firingState;

	// Token: 0x04002C70 RID: 11376
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startPos;

	// Token: 0x04002C71 RID: 11377
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 direction;

	// Token: 0x04002C72 RID: 11378
	[PublicizedFrom(EAccessModifier.Private)]
	public int userData;
}

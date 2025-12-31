using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200073C RID: 1852
[Preserve]
public class NetPackageEntityRagdoll : NetPackage
{
	// Token: 0x06003626 RID: 13862 RVA: 0x001660C5 File Offset: 0x001642C5
	public NetPackageEntityRagdoll Setup(Entity _entity, sbyte _state)
	{
		this.entityId = _entity.entityId;
		this.state = _state;
		return this;
	}

	// Token: 0x06003627 RID: 13863 RVA: 0x001660DC File Offset: 0x001642DC
	public NetPackageEntityRagdoll Setup(Entity _entity, float _duration, EnumBodyPartHit _bodyPart, Vector3 _forceVec, Vector3 _forceWorldPos)
	{
		this.entityId = _entity.entityId;
		this.state = -1;
		this.duration = _duration;
		this.bodyPart = _bodyPart;
		this.forceVec = _forceVec;
		this.forceWorldPos = _forceWorldPos;
		this.hipPos = _entity.emodel.GetHipPosition();
		return this;
	}

	// Token: 0x06003628 RID: 13864 RVA: 0x0016612C File Offset: 0x0016432C
	public override void read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.state = _br.ReadSByte();
		if (this.state < 0)
		{
			this.duration = _br.ReadSingle();
			this.bodyPart = (EnumBodyPartHit)_br.ReadInt16();
			this.forceVec = StreamUtils.ReadVector3(_br);
			this.forceWorldPos = StreamUtils.ReadVector3(_br);
			this.hipPos = StreamUtils.ReadVector3(_br);
		}
	}

	// Token: 0x06003629 RID: 13865 RVA: 0x00166198 File Offset: 0x00164398
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.entityId);
		_bw.Write(this.state);
		if (this.state < 0)
		{
			_bw.Write(this.duration);
			_bw.Write((short)this.bodyPart);
			StreamUtils.Write(_bw, this.forceVec);
			StreamUtils.Write(_bw, this.forceWorldPos);
			StreamUtils.Write(_bw, this.hipPos);
		}
	}

	// Token: 0x0600362A RID: 13866 RVA: 0x0016620C File Offset: 0x0016440C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = _world.GetEntity(this.entityId) as EntityAlive;
		if (entityAlive == null)
		{
			Log.Out("Discarding " + base.GetType().Name + " for entity Id=" + this.entityId.ToString());
			return;
		}
		if (this.state < 0)
		{
			entityAlive.emodel.DoRagdoll(this.duration, this.bodyPart, this.forceVec, this.forceWorldPos, true);
			return;
		}
		entityAlive.emodel.SetRagdollState((int)this.state);
	}

	// Token: 0x0600362B RID: 13867 RVA: 0x001662A2 File Offset: 0x001644A2
	public override int GetLength()
	{
		return 48;
	}

	// Token: 0x04002C02 RID: 11266
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C03 RID: 11267
	[PublicizedFrom(EAccessModifier.Private)]
	public sbyte state;

	// Token: 0x04002C04 RID: 11268
	[PublicizedFrom(EAccessModifier.Private)]
	public float duration;

	// Token: 0x04002C05 RID: 11269
	[PublicizedFrom(EAccessModifier.Private)]
	public EnumBodyPartHit bodyPart;

	// Token: 0x04002C06 RID: 11270
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 forceVec;

	// Token: 0x04002C07 RID: 11271
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 forceWorldPos;

	// Token: 0x04002C08 RID: 11272
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 hipPos;
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000746 RID: 1862
[Preserve]
public class NetPackageEntityVelocity : NetPackage
{
	// Token: 0x17000582 RID: 1410
	// (get) Token: 0x06003668 RID: 13928 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003669 RID: 13929 RVA: 0x00166DEC File Offset: 0x00164FEC
	public NetPackageEntityVelocity Setup(int _entityId, Vector3 _motion, bool _bAdd)
	{
		this.entityId = _entityId;
		this.bAdd = _bAdd;
		if (_motion.x < -8f)
		{
			_motion.x = -8f;
		}
		if (_motion.y < -8f)
		{
			_motion.y = -8f;
		}
		if (_motion.z < -8f)
		{
			_motion.z = -8f;
		}
		if (_motion.x > 8f)
		{
			_motion.x = 8f;
		}
		if (_motion.y > 8f)
		{
			_motion.y = 8f;
		}
		if (_motion.z > 8f)
		{
			_motion.z = 8f;
		}
		this.motion = _motion;
		return this;
	}

	// Token: 0x0600366A RID: 13930 RVA: 0x00166EA5 File Offset: 0x001650A5
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.bAdd = _reader.ReadBoolean();
		this.motion = new Vector3(_reader.ReadSingle(), _reader.ReadSingle(), _reader.ReadSingle());
	}

	// Token: 0x0600366B RID: 13931 RVA: 0x00166EDC File Offset: 0x001650DC
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.bAdd);
		_writer.Write(this.motion.x);
		_writer.Write(this.motion.y);
		_writer.Write(this.motion.z);
	}

	// Token: 0x0600366C RID: 13932 RVA: 0x00166F3C File Offset: 0x0016513C
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		Entity entity = _world.GetEntity(this.entityId);
		if (!(entity != null))
		{
			Log.Out("Discarding " + base.GetType().Name + " for entity Id=" + this.entityId.ToString());
			return;
		}
		if (!this.bAdd)
		{
			entity.SetVelocity(this.motion);
			return;
		}
		entity.AddVelocity(this.motion);
	}

	// Token: 0x0600366D RID: 13933 RVA: 0x00163F5F File Offset: 0x0016215F
	public override int GetLength()
	{
		return 16;
	}

	// Token: 0x04002C20 RID: 11296
	[PublicizedFrom(EAccessModifier.Private)]
	public const float max = 8f;

	// Token: 0x04002C21 RID: 11297
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002C22 RID: 11298
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 motion;

	// Token: 0x04002C23 RID: 11299
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bAdd;
}

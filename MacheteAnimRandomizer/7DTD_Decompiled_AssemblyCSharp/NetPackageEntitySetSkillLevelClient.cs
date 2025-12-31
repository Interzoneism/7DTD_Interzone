using System;
using UnityEngine.Scripting;

// Token: 0x0200072C RID: 1836
[Preserve]
public class NetPackageEntitySetSkillLevelClient : NetPackage
{
	// Token: 0x17000571 RID: 1393
	// (get) Token: 0x060035C5 RID: 13765 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060035C6 RID: 13766 RVA: 0x00164F6F File Offset: 0x0016316F
	public NetPackageEntitySetSkillLevelClient Setup(int _entityId, string _skill, int _level)
	{
		this.entityId = _entityId;
		this.skill = _skill;
		this.level = _level;
		return this;
	}

	// Token: 0x060035C7 RID: 13767 RVA: 0x00164F87 File Offset: 0x00163187
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.skill = _reader.ReadString();
		this.level = _reader.ReadInt32();
	}

	// Token: 0x060035C8 RID: 13768 RVA: 0x00164FAD File Offset: 0x001631AD
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.skill);
		_writer.Write(this.level);
	}

	// Token: 0x060035C9 RID: 13769 RVA: 0x00164FDC File Offset: 0x001631DC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityPlayer entityPlayer = (EntityPlayer)_world.GetEntity(this.entityId);
		if (entityPlayer != null)
		{
			entityPlayer.Progression.GetProgressionValue(this.skill).Level = this.level;
		}
	}

	// Token: 0x060035CA RID: 13770 RVA: 0x000768E0 File Offset: 0x00074AE0
	public override int GetLength()
	{
		return 8;
	}

	// Token: 0x04002BCC RID: 11212
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityId;

	// Token: 0x04002BCD RID: 11213
	[PublicizedFrom(EAccessModifier.Protected)]
	public string skill;

	// Token: 0x04002BCE RID: 11214
	[PublicizedFrom(EAccessModifier.Protected)]
	public int level;
}

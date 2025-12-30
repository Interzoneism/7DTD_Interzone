using System;
using UnityEngine.Scripting;

// Token: 0x0200072F RID: 1839
[Preserve]
public class NetPackageEntityAddScoreClient : NetPackage
{
	// Token: 0x17000574 RID: 1396
	// (get) Token: 0x060035D4 RID: 13780 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x060035D5 RID: 13781 RVA: 0x001650F1 File Offset: 0x001632F1
	public NetPackageEntityAddScoreClient Setup(int _entityId, int _zombieKills, int _playerKills, int _otherTeamNumber, int _conditions)
	{
		this.entityId = _entityId;
		this.zombieKills = _zombieKills;
		this.playerKills = _playerKills;
		this.otherTeamNumber = _otherTeamNumber;
		this.conditions = _conditions;
		return this;
	}

	// Token: 0x060035D6 RID: 13782 RVA: 0x00165119 File Offset: 0x00163319
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.zombieKills = (int)_reader.ReadInt16();
		this.playerKills = (int)_reader.ReadInt16();
		this.otherTeamNumber = (int)_reader.ReadInt16();
		this.conditions = _reader.ReadInt32();
	}

	// Token: 0x060035D7 RID: 13783 RVA: 0x00165158 File Offset: 0x00163358
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write((short)this.zombieKills);
		_writer.Write((short)this.playerKills);
		_writer.Write((short)this.otherTeamNumber);
		_writer.Write(this.conditions);
	}

	// Token: 0x060035D8 RID: 13784 RVA: 0x001651AC File Offset: 0x001633AC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		EntityAlive entityAlive = (EntityAlive)_world.GetEntity(this.entityId);
		if (entityAlive != null)
		{
			entityAlive.AddScore(0, this.zombieKills, this.playerKills, this.otherTeamNumber, this.conditions);
		}
	}

	// Token: 0x060035D9 RID: 13785 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002BCF RID: 11215
	[PublicizedFrom(EAccessModifier.Protected)]
	public int entityId;

	// Token: 0x04002BD0 RID: 11216
	[PublicizedFrom(EAccessModifier.Protected)]
	public int zombieKills;

	// Token: 0x04002BD1 RID: 11217
	[PublicizedFrom(EAccessModifier.Protected)]
	public int playerKills;

	// Token: 0x04002BD2 RID: 11218
	[PublicizedFrom(EAccessModifier.Protected)]
	public int otherTeamNumber;

	// Token: 0x04002BD3 RID: 11219
	[PublicizedFrom(EAccessModifier.Protected)]
	public int conditions;
}

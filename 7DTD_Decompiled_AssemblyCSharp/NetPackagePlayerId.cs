using System;
using UnityEngine.Scripting;

// Token: 0x02000771 RID: 1905
[Preserve]
public class NetPackagePlayerId : NetPackage
{
	// Token: 0x170005A1 RID: 1441
	// (get) Token: 0x06003768 RID: 14184 RVA: 0x000282C0 File Offset: 0x000264C0
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToClient;
		}
	}

	// Token: 0x06003769 RID: 14185 RVA: 0x0016ABF0 File Offset: 0x00168DF0
	public NetPackagePlayerId Setup(int _id, int _teamNumber, PlayerDataFile _playerDataFile, int _chunkViewDim)
	{
		this.id = _id;
		this.teamNumber = _teamNumber;
		this.playerDataFile = _playerDataFile;
		this.chunkViewDim = _chunkViewDim;
		return this;
	}

	// Token: 0x0600376A RID: 14186 RVA: 0x0016AC10 File Offset: 0x00168E10
	public override void read(PooledBinaryReader _reader)
	{
		this.id = _reader.ReadInt32();
		this.teamNumber = (int)_reader.ReadInt16();
		this.playerDataFile = new PlayerDataFile();
		this.playerDataFile.Read(_reader, uint.MaxValue);
		this.chunkViewDim = _reader.ReadInt32();
	}

	// Token: 0x0600376B RID: 14187 RVA: 0x0016AC4E File Offset: 0x00168E4E
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.id);
		_writer.Write((short)this.teamNumber);
		this.playerDataFile.Write(_writer);
		_writer.Write(this.chunkViewDim);
	}

	// Token: 0x0600376C RID: 14188 RVA: 0x0016AC88 File Offset: 0x00168E88
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		_callbacks.PlayerId(this.id, this.teamNumber, this.playerDataFile, this.chunkViewDim);
	}

	// Token: 0x0600376D RID: 14189 RVA: 0x00162999 File Offset: 0x00160B99
	public override int GetLength()
	{
		return 40;
	}

	// Token: 0x04002CEB RID: 11499
	[PublicizedFrom(EAccessModifier.Private)]
	public int id;

	// Token: 0x04002CEC RID: 11500
	[PublicizedFrom(EAccessModifier.Private)]
	public int teamNumber;

	// Token: 0x04002CED RID: 11501
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerDataFile playerDataFile;

	// Token: 0x04002CEE RID: 11502
	[PublicizedFrom(EAccessModifier.Private)]
	public int chunkViewDim;
}

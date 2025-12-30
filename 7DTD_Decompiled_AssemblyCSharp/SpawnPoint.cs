using System;
using UnityEngine;

// Token: 0x02000A47 RID: 2631
public class SpawnPoint
{
	// Token: 0x0600506C RID: 20588 RVA: 0x001FF48C File Offset: 0x001FD68C
	public SpawnPoint()
	{
		this.spawnPosition = SpawnPosition.Undef;
		this.team = 0;
		this.activeInGameMode = 0;
	}

	// Token: 0x0600506D RID: 20589 RVA: 0x001FF4AD File Offset: 0x001FD6AD
	public SpawnPoint(Vector3i _blockPos)
	{
		this.spawnPosition = new SpawnPosition(_blockPos, 0f);
		this.team = 0;
		this.activeInGameMode = -1;
	}

	// Token: 0x0600506E RID: 20590 RVA: 0x001FF4D4 File Offset: 0x001FD6D4
	public SpawnPoint(Vector3 _position, float _heading)
	{
		this.spawnPosition = new SpawnPosition(_position, _heading);
		this.team = 0;
		this.activeInGameMode = -1;
	}

	// Token: 0x0600506F RID: 20591 RVA: 0x001FF4F7 File Offset: 0x001FD6F7
	public void Read(IBinaryReaderOrWriter _readerOrWriter, uint _version)
	{
		this.spawnPosition.Read(_readerOrWriter, _version);
		this.team = _readerOrWriter.ReadWrite(0);
		this.activeInGameMode = _readerOrWriter.ReadWrite(0);
	}

	// Token: 0x06005070 RID: 20592 RVA: 0x001FF520 File Offset: 0x001FD720
	public void Read(PooledBinaryReader _br, uint _version)
	{
		this.spawnPosition.Read(_br, _version);
		this.team = _br.ReadInt32();
		this.activeInGameMode = _br.ReadInt32();
	}

	// Token: 0x06005071 RID: 20593 RVA: 0x001FF547 File Offset: 0x001FD747
	public void Write(PooledBinaryWriter _bw)
	{
		this.spawnPosition.Write(_bw);
		_bw.Write(this.team);
		_bw.Write(this.activeInGameMode);
	}

	// Token: 0x06005072 RID: 20594 RVA: 0x001FF570 File Offset: 0x001FD770
	public override int GetHashCode()
	{
		return this.spawnPosition.ToBlockPos().GetHashCode();
	}

	// Token: 0x04003D9A RID: 15770
	public SpawnPosition spawnPosition;

	// Token: 0x04003D9B RID: 15771
	public int team;

	// Token: 0x04003D9C RID: 15772
	public int activeInGameMode;
}

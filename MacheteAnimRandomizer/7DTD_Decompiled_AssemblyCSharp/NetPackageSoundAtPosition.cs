using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200079A RID: 1946
[Preserve]
public class NetPackageSoundAtPosition : NetPackage
{
	// Token: 0x06003867 RID: 14439 RVA: 0x0016F693 File Offset: 0x0016D893
	public NetPackageSoundAtPosition Setup(Vector3 _pos, string _audioClipName, AudioRolloffMode _mode, int _distance, int _entityId)
	{
		this.pos = _pos;
		this.audioClipName = _audioClipName;
		this.mode = _mode;
		this.distance = _distance;
		this.entityId = _entityId;
		return this;
	}

	// Token: 0x06003868 RID: 14440 RVA: 0x0016F6BB File Offset: 0x0016D8BB
	public override void read(PooledBinaryReader _br)
	{
		this.pos = StreamUtils.ReadVector3(_br);
		this.audioClipName = _br.ReadString();
		this.mode = (AudioRolloffMode)_br.ReadByte();
		this.distance = _br.ReadInt32();
		this.entityId = _br.ReadInt32();
	}

	// Token: 0x06003869 RID: 14441 RVA: 0x0016F6FC File Offset: 0x0016D8FC
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		StreamUtils.Write(_bw, this.pos);
		_bw.Write(this.audioClipName);
		_bw.Write((byte)this.mode);
		_bw.Write(this.distance);
		_bw.Write(this.entityId);
	}

	// Token: 0x0600386A RID: 14442 RVA: 0x0016F750 File Offset: 0x0016D950
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			_world.gameManager.PlaySoundAtPositionServer(this.pos, this.audioClipName, this.mode, this.distance, this.entityId);
			return;
		}
		_world.gameManager.PlaySoundAtPositionClient(this.pos, this.audioClipName, this.mode, this.distance);
	}

	// Token: 0x0600386B RID: 14443 RVA: 0x00162999 File Offset: 0x00160B99
	public override int GetLength()
	{
		return 40;
	}

	// Token: 0x04002DB0 RID: 11696
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 pos;

	// Token: 0x04002DB1 RID: 11697
	[PublicizedFrom(EAccessModifier.Private)]
	public string audioClipName;

	// Token: 0x04002DB2 RID: 11698
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioRolloffMode mode;

	// Token: 0x04002DB3 RID: 11699
	[PublicizedFrom(EAccessModifier.Private)]
	public int distance;

	// Token: 0x04002DB4 RID: 11700
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;
}

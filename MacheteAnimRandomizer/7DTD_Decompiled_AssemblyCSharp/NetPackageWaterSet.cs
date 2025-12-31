using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

// Token: 0x020007AB RID: 1963
[Preserve]
public class NetPackageWaterSet : NetPackage
{
	// Token: 0x170005B7 RID: 1463
	// (get) Token: 0x060038C7 RID: 14535 RVA: 0x00170E0C File Offset: 0x0016F00C
	public bool HasChanges
	{
		get
		{
			return this.changes.Count > 0;
		}
	}

	// Token: 0x060038C8 RID: 14536 RVA: 0x00170E1C File Offset: 0x0016F01C
	public void SetSenderId(int _entityId)
	{
		this.senderEntityId = _entityId;
	}

	// Token: 0x060038C9 RID: 14537 RVA: 0x00170E25 File Offset: 0x0016F025
	public void Reset()
	{
		this.senderEntityId = -1;
		this.changes.Clear();
	}

	// Token: 0x060038CA RID: 14538 RVA: 0x00170E39 File Offset: 0x0016F039
	public void AddChange(int _worldX, int _worldY, int _worldZ, WaterValue _data)
	{
		this.AddChange(new Vector3i(_worldX, _worldY, _worldZ), _data);
	}

	// Token: 0x060038CB RID: 14539 RVA: 0x00170E4C File Offset: 0x0016F04C
	public void AddChange(Vector3i _worldPos, WaterValue _data)
	{
		NetPackageWaterSet.WaterSetInfo item = new NetPackageWaterSet.WaterSetInfo
		{
			worldPos = _worldPos,
			waterData = _data
		};
		this.changes.Add(item);
	}

	// Token: 0x060038CC RID: 14540 RVA: 0x00170E80 File Offset: 0x0016F080
	public override void read(PooledBinaryReader _br)
	{
		this.senderEntityId = _br.ReadInt32();
		this.changes.Clear();
		int num = (int)_br.ReadUInt16();
		for (int i = 0; i < num; i++)
		{
			NetPackageWaterSet.WaterSetInfo item = default(NetPackageWaterSet.WaterSetInfo);
			item.Read(_br);
			this.changes.Add(item);
		}
	}

	// Token: 0x060038CD RID: 14541 RVA: 0x00170ED4 File Offset: 0x0016F0D4
	public override void write(PooledBinaryWriter _bw)
	{
		base.write(_bw);
		_bw.Write(this.senderEntityId);
		int count = this.changes.Count;
		_bw.Write((ushort)count);
		for (int i = 0; i < this.changes.Count; i++)
		{
			this.changes[i].Write(_bw);
		}
	}

	// Token: 0x060038CE RID: 14542 RVA: 0x00170F34 File Offset: 0x0016F134
	public void ApplyChanges(ChunkCluster _cc)
	{
		_cc.ChunkPosNeedsRegeneration_DelayedStart();
		for (int i = 0; i < this.changes.Count; i++)
		{
			NetPackageWaterSet.WaterSetInfo waterSetInfo = this.changes[i];
			_cc.SetWater(waterSetInfo.worldPos, waterSetInfo.waterData);
			GameManager.Instance.World.HandleWaterLevelChanged(waterSetInfo.worldPos, waterSetInfo.waterData.GetMassPercent());
		}
		_cc.ChunkPosNeedsRegeneration_DelayedStop();
	}

	// Token: 0x060038CF RID: 14543 RVA: 0x00170FA4 File Offset: 0x0016F1A4
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(this, false, -1, this.senderEntityId, -1, null, 192, false);
		}
		if (_world == null)
		{
			return;
		}
		ChunkCluster chunkCluster = _world.ChunkClusters[0];
		if (chunkCluster == null)
		{
			return;
		}
		this.ApplyChanges(chunkCluster);
	}

	// Token: 0x060038D0 RID: 14544 RVA: 0x00170FFC File Offset: 0x0016F1FC
	public override int GetLength()
	{
		return 2 + this.changes.Count * NetPackageWaterSet.WaterSetInfo.GetLength();
	}

	// Token: 0x04002DEF RID: 11759
	[PublicizedFrom(EAccessModifier.Private)]
	public int senderEntityId = -1;

	// Token: 0x04002DF0 RID: 11760
	[PublicizedFrom(EAccessModifier.Private)]
	public List<NetPackageWaterSet.WaterSetInfo> changes = new List<NetPackageWaterSet.WaterSetInfo>();

	// Token: 0x020007AC RID: 1964
	[PublicizedFrom(EAccessModifier.Private)]
	public struct WaterSetInfo
	{
		// Token: 0x060038D2 RID: 14546 RVA: 0x0017102B File Offset: 0x0016F22B
		public void Read(BinaryReader _br)
		{
			this.worldPos = StreamUtils.ReadVector3i(_br);
			this.waterData.Read(_br);
		}

		// Token: 0x060038D3 RID: 14547 RVA: 0x00171045 File Offset: 0x0016F245
		public void Write(BinaryWriter _bw)
		{
			StreamUtils.Write(_bw, this.worldPos);
			this.waterData.Write(_bw);
		}

		// Token: 0x060038D4 RID: 14548 RVA: 0x0017105F File Offset: 0x0016F25F
		public static int GetLength()
		{
			return 12 + WaterValue.SerializedLength();
		}

		// Token: 0x04002DF1 RID: 11761
		public Vector3i worldPos;

		// Token: 0x04002DF2 RID: 11762
		public WaterValue waterData;
	}
}

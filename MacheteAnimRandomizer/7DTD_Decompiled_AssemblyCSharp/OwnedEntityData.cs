using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200040F RID: 1039
[Preserve]
[Serializable]
public class OwnedEntityData
{
	// Token: 0x1700036E RID: 878
	// (get) Token: 0x06001F02 RID: 7938 RVA: 0x000C0C3F File Offset: 0x000BEE3F
	// (set) Token: 0x06001F03 RID: 7939 RVA: 0x000C0C47 File Offset: 0x000BEE47
	public EntityCreationData EntityCreationData { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x06001F04 RID: 7940 RVA: 0x000C0C50 File Offset: 0x000BEE50
	public OwnedEntityData()
	{
	}

	// Token: 0x06001F05 RID: 7941 RVA: 0x000C0C66 File Offset: 0x000BEE66
	public OwnedEntityData(Entity _entity)
	{
		this.entityId = _entity.entityId;
		this.classId = _entity.entityClass;
		this.EntityCreationData = new EntityCreationData(_entity, true);
	}

	// Token: 0x06001F06 RID: 7942 RVA: 0x000C0CA1 File Offset: 0x000BEEA1
	public OwnedEntityData(int _entityId, int _classId)
	{
		this.entityId = _entityId;
		this.classId = _classId;
	}

	// Token: 0x1700036F RID: 879
	// (get) Token: 0x06001F07 RID: 7943 RVA: 0x000C0CC5 File Offset: 0x000BEEC5
	public int Id
	{
		get
		{
			return this.entityId;
		}
	}

	// Token: 0x17000370 RID: 880
	// (get) Token: 0x06001F08 RID: 7944 RVA: 0x000C0CCD File Offset: 0x000BEECD
	public int ClassId
	{
		get
		{
			return this.classId;
		}
	}

	// Token: 0x17000371 RID: 881
	// (get) Token: 0x06001F09 RID: 7945 RVA: 0x000C0CD5 File Offset: 0x000BEED5
	public Vector3 LastKnownPosition
	{
		get
		{
			return this.lastKnownPosition;
		}
	}

	// Token: 0x06001F0A RID: 7946 RVA: 0x000C0CDD File Offset: 0x000BEEDD
	public void SetLastKnownPosition(Vector3 pos)
	{
		this.lastKnownPosition = pos;
		this.saveFlags |= 1;
	}

	// Token: 0x06001F0B RID: 7947 RVA: 0x000C0CF5 File Offset: 0x000BEEF5
	public void ClearLastKnownPostition()
	{
		this.lastKnownPosition = Vector3.zero;
		this.saveFlags = (ushort)((int)this.saveFlags & -2);
	}

	// Token: 0x17000372 RID: 882
	// (get) Token: 0x06001F0C RID: 7948 RVA: 0x000C0D12 File Offset: 0x000BEF12
	public bool hasLastKnownPosition
	{
		get
		{
			return (this.saveFlags & 1) > 0;
		}
	}

	// Token: 0x06001F0D RID: 7949 RVA: 0x000C0D20 File Offset: 0x000BEF20
	public void Read(PooledBinaryReader _br)
	{
		this.entityId = _br.ReadInt32();
		this.classId = _br.ReadInt32();
		this.saveFlags = _br.ReadUInt16();
		if ((this.saveFlags & 1) > 0)
		{
			this.lastKnownPosition.x = (float)_br.ReadInt32();
			this.lastKnownPosition.y = (float)_br.ReadInt32();
			this.lastKnownPosition.z = (float)_br.ReadInt32();
		}
	}

	// Token: 0x06001F0E RID: 7950 RVA: 0x000C0D94 File Offset: 0x000BEF94
	public void Write(PooledBinaryWriter _bw)
	{
		_bw.Write(this.Id);
		_bw.Write(this.ClassId);
		_bw.Write(this.saveFlags);
		if ((this.saveFlags & 1) > 0)
		{
			_bw.Write((int)this.lastKnownPosition.x);
			_bw.Write((int)this.lastKnownPosition.y);
			_bw.Write((int)this.lastKnownPosition.z);
		}
	}

	// Token: 0x040015B9 RID: 5561
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId = -1;

	// Token: 0x040015BA RID: 5562
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public int classId = -1;

	// Token: 0x040015BB RID: 5563
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 lastKnownPosition;

	// Token: 0x040015BC RID: 5564
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public ushort saveFlags;

	// Token: 0x040015BD RID: 5565
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public const ushort cHasLastKnownPosition = 1;
}

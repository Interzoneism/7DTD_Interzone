using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000777 RID: 1911
[Preserve]
public class NetPackagePlayerQuestPositions : NetPackage
{
	// Token: 0x170005A8 RID: 1448
	// (get) Token: 0x06003796 RID: 14230 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x06003797 RID: 14231 RVA: 0x0016B621 File Offset: 0x00169821
	public NetPackagePlayerQuestPositions Setup(int entityId, PersistentPlayerData ppd)
	{
		this.entityId = entityId;
		this.questPositions = new List<QuestPositionData>(ppd.QuestPositions);
		return this;
	}

	// Token: 0x06003798 RID: 14232 RVA: 0x0016B63C File Offset: 0x0016983C
	public override void read(PooledBinaryReader _reader)
	{
		this.entityId = _reader.ReadInt32();
		this.questPositions = new List<QuestPositionData>();
		int num = _reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			this.questPositions.Add(QuestPositionData.Read(_reader));
		}
	}

	// Token: 0x06003799 RID: 14233 RVA: 0x0016B684 File Offset: 0x00169884
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityId);
		_writer.Write(this.questPositions.Count);
		foreach (QuestPositionData questPositionData in this.questPositions)
		{
			questPositionData.Write(_writer);
		}
	}

	// Token: 0x0600379A RID: 14234 RVA: 0x0016B6FC File Offset: 0x001698FC
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (!base.ValidEntityIdForSender(this.entityId, false))
		{
			return;
		}
		PersistentPlayerData playerDataFromEntityID = GameManager.Instance.persistentPlayers.GetPlayerDataFromEntityID(this.entityId);
		if (playerDataFromEntityID != null)
		{
			playerDataFromEntityID.QuestPositions.Clear();
			playerDataFromEntityID.QuestPositions.AddRange(this.questPositions);
		}
	}

	// Token: 0x0600379B RID: 14235 RVA: 0x0000FB42 File Offset: 0x0000DD42
	public override int GetLength()
	{
		return 0;
	}

	// Token: 0x04002D04 RID: 11524
	[PublicizedFrom(EAccessModifier.Private)]
	public int entityId;

	// Token: 0x04002D05 RID: 11525
	[PublicizedFrom(EAccessModifier.Private)]
	public List<QuestPositionData> questPositions;
}

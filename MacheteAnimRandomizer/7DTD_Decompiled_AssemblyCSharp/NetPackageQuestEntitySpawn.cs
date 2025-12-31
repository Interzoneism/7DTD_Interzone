using System;
using UnityEngine.Scripting;

// Token: 0x0200077E RID: 1918
[Preserve]
public class NetPackageQuestEntitySpawn : NetPackage
{
	// Token: 0x060037C7 RID: 14279 RVA: 0x0016C257 File Offset: 0x0016A457
	public NetPackageQuestEntitySpawn Setup(int _entityType, int _entityThatPlaced = -1)
	{
		this.entityType = _entityType;
		this.gamestageGroup = "";
		this.entityIDQuestHolder = _entityThatPlaced;
		return this;
	}

	// Token: 0x060037C8 RID: 14280 RVA: 0x0016C273 File Offset: 0x0016A473
	public NetPackageQuestEntitySpawn Setup(string _gamestageGroup, int _entityThatPlaced = -1)
	{
		this.entityType = -1;
		this.gamestageGroup = _gamestageGroup;
		this.entityIDQuestHolder = _entityThatPlaced;
		return this;
	}

	// Token: 0x060037C9 RID: 14281 RVA: 0x0016C28B File Offset: 0x0016A48B
	public override void read(PooledBinaryReader _reader)
	{
		this.entityType = _reader.ReadInt32();
		this.gamestageGroup = _reader.ReadString();
		this.entityIDQuestHolder = _reader.ReadInt32();
	}

	// Token: 0x060037CA RID: 14282 RVA: 0x0016C2B1 File Offset: 0x0016A4B1
	public override void write(PooledBinaryWriter _writer)
	{
		base.write(_writer);
		_writer.Write(this.entityType);
		_writer.Write(this.gamestageGroup);
		_writer.Write(this.entityIDQuestHolder);
	}

	// Token: 0x170005AD RID: 1453
	// (get) Token: 0x060037CB RID: 14283 RVA: 0x000197A5 File Offset: 0x000179A5
	public override NetPackageDirection PackageDirection
	{
		get
		{
			return NetPackageDirection.ToServer;
		}
	}

	// Token: 0x060037CC RID: 14284 RVA: 0x0016C2E0 File Offset: 0x0016A4E0
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (this.entityType == -1)
		{
			EntityPlayer entityPlayer = GameManager.Instance.World.GetEntity(this.entityIDQuestHolder) as EntityPlayer;
			GameStageDefinition gameStage = GameStageDefinition.GetGameStage(this.gamestageGroup);
			this.entityType = EntityGroups.GetRandomFromGroup(gameStage.GetStage(entityPlayer.PartyGameStage).GetSpawnGroup(0).groupName, ref NetPackageQuestEntitySpawn.lastClassId, null);
		}
		QuestActionSpawnEnemy.SpawnQuestEntity(this.entityType, this.entityIDQuestHolder, null);
	}

	// Token: 0x060037CD RID: 14285 RVA: 0x000ADB75 File Offset: 0x000ABD75
	public override int GetLength()
	{
		return 20;
	}

	// Token: 0x04002D17 RID: 11543
	public int entityType = -1;

	// Token: 0x04002D18 RID: 11544
	public string gamestageGroup;

	// Token: 0x04002D19 RID: 11545
	public ItemValue itemValue;

	// Token: 0x04002D1A RID: 11546
	public int entityIDQuestHolder;

	// Token: 0x04002D1B RID: 11547
	[PublicizedFrom(EAccessModifier.Private)]
	public static int lastClassId = -1;
}

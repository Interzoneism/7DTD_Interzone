using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Scripting;

// Token: 0x020003BF RID: 959
[Preserve]
public class AIDirectorGameStagePartySpawner
{
	// Token: 0x06001D3A RID: 7482 RVA: 0x000B6784 File Offset: 0x000B4984
	public AIDirectorGameStagePartySpawner(World _world, string _gameStageName)
	{
		this.world = _world;
		this.def = GameStageDefinition.GetGameStage(_gameStageName);
		this.partyMembers = new ReadOnlyCollection<EntityPlayer>(this.members);
		this.partyLevel = -1;
		this.gsScaling = 1f;
	}

	// Token: 0x06001D3B RID: 7483 RVA: 0x000B67E3 File Offset: 0x000B49E3
	public void SetScaling(float _scaling)
	{
		this.gsScaling = Utils.FastLerp(1f, 2.5f, (_scaling - 1f) / 3f);
	}

	// Token: 0x06001D3C RID: 7484 RVA: 0x000B6808 File Offset: 0x000B4A08
	public void ResetPartyLevel(int mod = 0)
	{
		int num = this.CalcPartyLevel();
		if (mod != 0)
		{
			num %= mod;
		}
		this.SetPartyLevel(num);
	}

	// Token: 0x06001D3D RID: 7485 RVA: 0x000B682C File Offset: 0x000B4A2C
	public int CalcPartyLevel()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.members.Count; i++)
		{
			EntityPlayer entityPlayer = this.members[i];
			list.Add(entityPlayer.gameStage);
		}
		return GameStageDefinition.CalcPartyLevel(list);
	}

	// Token: 0x06001D3E RID: 7486 RVA: 0x000B6874 File Offset: 0x000B4A74
	public void SetPartyLevel(int _partyLevel)
	{
		this.partyLevel = _partyLevel;
		this.partyLevel = (int)((float)this.partyLevel * this.gsScaling);
		this.stageSpawnMax = 0;
		this.groupIndex = 0;
		this.spawnCount = 0;
		if (this.def != null)
		{
			this.stage = this.def.GetStage(_partyLevel);
			if (this.stage != null)
			{
				this.stageSpawnMax = this.CalcStageSpawnMax();
				this.SetupGroup();
			}
		}
		this.bonusLootEvery = Utils.FastMax(this.stageSpawnMax / GameStageDefinition.LootBonusMaxCount, GameStageDefinition.LootBonusEvery);
		Log.Out("Party of {0}, GS {1} ({2}), scaling {3}, enemy max {4}, bonus every {5}", new object[]
		{
			this.members.Count,
			this.partyLevel,
			_partyLevel,
			this.gsScaling,
			this.stageSpawnMax,
			this.bonusLootEvery
		});
		Log.Out("Party members: ");
		for (int i = 0; i < this.members.Count; i++)
		{
			EntityPlayer entityPlayer = this.members[i];
			Log.Out("Player id {0}, gameStage {1}", new object[]
			{
				entityPlayer.entityId,
				entityPlayer.gameStage
			});
		}
	}

	// Token: 0x06001D3F RID: 7487 RVA: 0x000B69C0 File Offset: 0x000B4BC0
	public bool Tick(double _deltaTime)
	{
		if (this.spawnGroup != null)
		{
			bool flag = false;
			if (this.nextStageTime > 0UL && this.world.worldTime >= this.nextStageTime)
			{
				flag = true;
			}
			else if (this.spawnCount >= this.numToSpawn)
			{
				this.interval -= _deltaTime;
				flag = (this.interval <= 0.0);
			}
			if (flag)
			{
				this.groupIndex++;
				this.SetupGroup();
			}
		}
		return this.spawnGroup != null;
	}

	// Token: 0x06001D40 RID: 7488 RVA: 0x000B6A4C File Offset: 0x000B4C4C
	public void AddMember(EntityPlayer _player)
	{
		if (!this.memberIDs.Contains(_player.entityId))
		{
			this.memberIDs.Add(_player.entityId);
		}
		if (!this.members.Contains(_player))
		{
			this.members.Add(_player);
		}
	}

	// Token: 0x06001D41 RID: 7489 RVA: 0x000B6A98 File Offset: 0x000B4C98
	public bool IsMemberOfParty(int _entityID)
	{
		return this.memberIDs.Contains(_entityID);
	}

	// Token: 0x06001D42 RID: 7490 RVA: 0x000B6AA6 File Offset: 0x000B4CA6
	public void RemoveMember(EntityPlayer _player, bool removeID)
	{
		this.members.Remove(_player);
		if (removeID)
		{
			this.memberIDs.Remove(_player.entityId);
		}
	}

	// Token: 0x06001D43 RID: 7491 RVA: 0x000B6ACC File Offset: 0x000B4CCC
	[PublicizedFrom(EAccessModifier.Private)]
	public int CalcStageSpawnMax()
	{
		int num = 0;
		int count = this.stage.Count;
		for (int i = 0; i < count; i++)
		{
			this.spawnGroup = this.stage.GetSpawnGroup(i);
			num += (int)this.spawnGroup.spawnCount;
		}
		return num;
	}

	// Token: 0x06001D44 RID: 7492 RVA: 0x000B6B14 File Offset: 0x000B4D14
	public void ClearMembers()
	{
		this.members.Clear();
		this.memberIDs.Clear();
	}

	// Token: 0x06001D45 RID: 7493 RVA: 0x000B6B2C File Offset: 0x000B4D2C
	public void IncSpawnCount()
	{
		this.spawnCount++;
	}

	// Token: 0x06001D46 RID: 7494 RVA: 0x000B6B3C File Offset: 0x000B4D3C
	public void DecSpawnCount(int dec)
	{
		if (dec > this.spawnCount)
		{
			this.spawnCount = 0;
			return;
		}
		this.spawnCount -= dec;
	}

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x06001D47 RID: 7495 RVA: 0x000B6B5D File Offset: 0x000B4D5D
	public bool IsDone
	{
		get
		{
			return this.groupIndex > 0 && this.spawnGroup == null;
		}
	}

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06001D48 RID: 7496 RVA: 0x000B6B73 File Offset: 0x000B4D73
	public bool canSpawn
	{
		get
		{
			return this.spawnGroup != null && this.spawnCount < this.numToSpawn;
		}
	}

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06001D49 RID: 7497 RVA: 0x000B6B8D File Offset: 0x000B4D8D
	public int maxAlive
	{
		get
		{
			if (this.spawnGroup == null)
			{
				return 0;
			}
			return (int)this.spawnGroup.maxAlive;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06001D4A RID: 7498 RVA: 0x000B6BA4 File Offset: 0x000B4DA4
	public string spawnGroupName
	{
		get
		{
			if (this.spawnGroup == null)
			{
				return null;
			}
			return this.spawnGroup.groupName;
		}
	}

	// Token: 0x06001D4B RID: 7499 RVA: 0x000B6BBC File Offset: 0x000B4DBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupGroup()
	{
		this.spawnGroup = this.stage.GetSpawnGroup(this.groupIndex);
		if (this.spawnGroup != null)
		{
			this.interval = (double)this.spawnGroup.interval;
			this.nextStageTime = ((this.spawnGroup.duration > 0) ? (this.world.worldTime + (ulong)(this.spawnGroup.duration * 1000)) : 0UL);
			this.numToSpawn = EntitySpawner.ModifySpawnCountByGameDifficulty((int)this.spawnGroup.spawnCount);
			this.spawnCount = 0;
			return;
		}
		Log.Out("AIDirectorGameStagePartySpawner: groups done ({0})", new object[]
		{
			this.groupIndex
		});
	}

	// Token: 0x06001D4C RID: 7500 RVA: 0x000B6C6C File Offset: 0x000B4E6C
	public override string ToString()
	{
		return string.Format("{0} {1} (count {2}, numToSpawn {3}, maxAlive {4})", new object[]
		{
			this.groupIndex,
			this.spawnGroupName,
			this.spawnCount,
			this.numToSpawn,
			this.maxAlive
		});
	}

	// Token: 0x04001403 RID: 5123
	public ReadOnlyCollection<EntityPlayer> partyMembers;

	// Token: 0x04001404 RID: 5124
	public float gsScaling;

	// Token: 0x04001405 RID: 5125
	public int groupIndex;

	// Token: 0x04001406 RID: 5126
	public int partyLevel;

	// Token: 0x04001407 RID: 5127
	public int stageSpawnMax;

	// Token: 0x04001408 RID: 5128
	public int bonusLootEvery;

	// Token: 0x04001409 RID: 5129
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition def;

	// Token: 0x0400140A RID: 5130
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition.Stage stage;

	// Token: 0x0400140B RID: 5131
	[PublicizedFrom(EAccessModifier.Private)]
	public GameStageDefinition.SpawnGroup spawnGroup;

	// Token: 0x0400140C RID: 5132
	[PublicizedFrom(EAccessModifier.Private)]
	public int spawnCount;

	// Token: 0x0400140D RID: 5133
	[PublicizedFrom(EAccessModifier.Private)]
	public int numToSpawn;

	// Token: 0x0400140E RID: 5134
	[PublicizedFrom(EAccessModifier.Private)]
	public double interval;

	// Token: 0x0400140F RID: 5135
	[PublicizedFrom(EAccessModifier.Private)]
	public ulong nextStageTime;

	// Token: 0x04001410 RID: 5136
	[PublicizedFrom(EAccessModifier.Private)]
	public World world;

	// Token: 0x04001411 RID: 5137
	[PublicizedFrom(EAccessModifier.Private)]
	public HashSet<int> memberIDs = new HashSet<int>();

	// Token: 0x04001412 RID: 5138
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityPlayer> members = new List<EntityPlayer>();
}

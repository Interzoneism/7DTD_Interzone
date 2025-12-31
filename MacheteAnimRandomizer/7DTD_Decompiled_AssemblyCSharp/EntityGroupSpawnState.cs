using System;
using System.Collections.Generic;

// Token: 0x02000979 RID: 2425
public class EntityGroupSpawnState
{
	// Token: 0x06004942 RID: 18754 RVA: 0x001CF6BC File Offset: 0x001CD8BC
	public EntityGroupSpawnState(string _sEntityGroupName)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		for (int i = 0; i < list.Count; i++)
		{
			this.state.Add(new EntityGroupSpawnState.State(list[i]));
		}
	}

	// Token: 0x06004943 RID: 18755 RVA: 0x001CF710 File Offset: 0x001CD910
	public int GetRandomFromGroup()
	{
		float randomFloat = GameManager.Instance.World.GetGameRandom().RandomFloat;
		float num = 0f;
		for (int i = 0; i < this.state.Count; i++)
		{
			EntityGroupSpawnState.State state = this.state[i];
			num += state.prob;
			if (randomFloat <= num && state.prob > 0f)
			{
				return state.entityClassId;
			}
		}
		return -1;
	}

	// Token: 0x06004944 RID: 18756 RVA: 0x001CF780 File Offset: 0x001CD980
	public void DidSpawn(int _classId)
	{
		for (int i = 0; i < this.state.Count; i++)
		{
			EntityGroupSpawnState.State state = this.state[i];
			if (state.entityClassId == _classId)
			{
				state.numSpawned++;
			}
			this.state[i] = state;
		}
	}

	// Token: 0x04003868 RID: 14440
	[PublicizedFrom(EAccessModifier.Private)]
	public List<EntityGroupSpawnState.State> state = new List<EntityGroupSpawnState.State>();

	// Token: 0x0200097A RID: 2426
	[PublicizedFrom(EAccessModifier.Private)]
	public struct State
	{
		// Token: 0x06004945 RID: 18757 RVA: 0x001CF7D2 File Offset: 0x001CD9D2
		public State(SEntityClassAndProb _src)
		{
			this.entityClassId = _src.entityClassId;
			this.prob = _src.prob;
			this.numSpawned = 0;
		}

		// Token: 0x04003869 RID: 14441
		public readonly int entityClassId;

		// Token: 0x0400386A RID: 14442
		public readonly float prob;

		// Token: 0x0400386B RID: 14443
		public int numSpawned;
	}
}

using System;
using System.Collections.Generic;

// Token: 0x02000978 RID: 2424
public class EntityGroups
{
	// Token: 0x0600493B RID: 18747 RVA: 0x001CF568 File Offset: 0x001CD768
	public static int GetRandomFromGroup(string _sEntityGroupName, ref int lastClassId, GameRandom random = null)
	{
		List<SEntityClassAndProb> grpList = EntityGroups.list[_sEntityGroupName];
		if (random == null)
		{
			random = GameManager.Instance.World.GetGameRandom();
		}
		int num = 0;
		for (int i = 0; i < 3; i++)
		{
			num = EntityGroups.GetRandomFromGroupList(grpList, random);
			if (num != lastClassId)
			{
				lastClassId = num;
				break;
			}
		}
		return num;
	}

	// Token: 0x0600493C RID: 18748 RVA: 0x001CF5B8 File Offset: 0x001CD7B8
	[PublicizedFrom(EAccessModifier.Private)]
	public static int GetRandomFromGroupList(List<SEntityClassAndProb> grpList, GameRandom random)
	{
		float randomFloat = random.RandomFloat;
		float num = 0f;
		for (int i = 0; i < grpList.Count; i++)
		{
			SEntityClassAndProb sentityClassAndProb = grpList[i];
			num += sentityClassAndProb.prob;
			if (randomFloat <= num && sentityClassAndProb.prob > 0f)
			{
				return sentityClassAndProb.entityClassId;
			}
		}
		return -1;
	}

	// Token: 0x0600493D RID: 18749 RVA: 0x001CF610 File Offset: 0x001CD810
	public static bool IsEnemyGroup(string _sEntityGroupName)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		return list != null && list.Count >= 1 && EntityClass.list[list[0].entityClassId].bIsEnemyEntity;
	}

	// Token: 0x0600493E RID: 18750 RVA: 0x001CF654 File Offset: 0x001CD854
	public static void Normalize(string _sEntityGroupName, float totalp)
	{
		List<SEntityClassAndProb> list = EntityGroups.list[_sEntityGroupName];
		for (int i = 0; i < list.Count; i++)
		{
			SEntityClassAndProb value = list[i];
			value.prob /= totalp;
			list[i] = value;
		}
	}

	// Token: 0x0600493F RID: 18751 RVA: 0x001CF69A File Offset: 0x001CD89A
	public static void Cleanup()
	{
		if (EntityGroups.list != null)
		{
			EntityGroups.list.Clear();
		}
	}

	// Token: 0x04003866 RID: 14438
	public static string DefaultGroupName;

	// Token: 0x04003867 RID: 14439
	public static DictionarySave<string, List<SEntityClassAndProb>> list = new DictionarySave<string, List<SEntityClassAndProb>>();
}

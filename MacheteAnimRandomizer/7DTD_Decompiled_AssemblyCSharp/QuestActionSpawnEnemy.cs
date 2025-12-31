using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000897 RID: 2199
[Preserve]
public class QuestActionSpawnEnemy : BaseQuestAction
{
	// Token: 0x06004036 RID: 16438 RVA: 0x001A3E34 File Offset: 0x001A2034
	public override void SetupAction()
	{
		string[] array = this.ID.Split(',', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			foreach (KeyValuePair<int, EntityClass> keyValuePair in EntityClass.list.Dict)
			{
				if (keyValuePair.Value.entityClassName == array[i])
				{
					this.entityIDs.Add(keyValuePair.Key);
					if (this.entityIDs.Count == array.Length)
					{
						break;
					}
				}
			}
		}
	}

	// Token: 0x06004037 RID: 16439 RVA: 0x001A3EDC File Offset: 0x001A20DC
	public override void PerformAction(Quest ownerQuest)
	{
		if (!GameStats.GetBool(EnumGameStats.EnemySpawnMode))
		{
			return;
		}
		this.HandleSpawnEnemies(ownerQuest);
	}

	// Token: 0x06004038 RID: 16440 RVA: 0x001A3EF0 File Offset: 0x001A20F0
	public void HandleSpawnEnemies(Quest ownerQuest)
	{
		if (this.Value != null && this.Value != "" && !int.TryParse(this.Value, out this.count) && this.Value.Contains("-"))
		{
			string[] array = this.Value.Split('-', StringSplitOptions.None);
			int min = Convert.ToInt32(array[0]);
			int maxExclusive = Convert.ToInt32(array[1]);
			World world = GameManager.Instance.World;
			this.count = world.GetGameRandom().RandomRange(min, maxExclusive);
		}
		GameManager.Instance.StartCoroutine(this.SpawnEnemies(ownerQuest));
	}

	// Token: 0x06004039 RID: 16441 RVA: 0x001A3F8B File Offset: 0x001A218B
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator SpawnEnemies(Quest ownerQuest)
	{
		EntityPlayerLocal player = ownerQuest.OwnerJournal.OwnerPlayer;
		int num2;
		for (int i = 0; i < this.count; i = num2 + 1)
		{
			yield return new WaitForSeconds(0.5f);
			World world = GameManager.Instance.World;
			int num = this.entityIDs[world.GetGameRandom().RandomRange(this.entityIDs.Count)];
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				QuestActionSpawnEnemy.SpawnQuestEntity(num, -1, player);
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEntitySpawn>().Setup(num, player.entityId), false);
			}
			num2 = i;
		}
		yield break;
	}

	// Token: 0x0600403A RID: 16442 RVA: 0x001A3FA4 File Offset: 0x001A21A4
	public static void SpawnQuestEntity(int spawnedEntityID, int entityIDQuestHolder, EntityPlayer player = null)
	{
		World world = GameManager.Instance.World;
		if (player == null)
		{
			player = (world.GetEntity(entityIDQuestHolder) as EntityPlayer);
		}
		Vector3 a = new Vector3(world.GetGameRandom().RandomFloat * 2f + -1f, 0f, world.GetGameRandom().RandomFloat * 2f + -1f);
		a.Normalize();
		float d = world.GetGameRandom().RandomFloat * 12f + 12f;
		Vector3 vector = player.position + a * d;
		Vector3 rotation = new Vector3(0f, player.transform.eulerAngles.y + 180f, 0f);
		float num = (float)GameManager.Instance.World.GetHeight((int)vector.x, (int)vector.z);
		float num2 = (float)GameManager.Instance.World.GetTerrainHeight((int)vector.x, (int)vector.z);
		vector.y = (num + num2) / 2f + 1.5f;
		Entity entity = EntityFactory.CreateEntity(spawnedEntityID, vector, rotation);
		entity.SetSpawnerSource(EnumSpawnerSource.Dynamic);
		GameManager.Instance.World.SpawnEntityInWorld(entity);
		(entity as EntityAlive).SetAttackTarget(player, 200);
	}

	// Token: 0x0600403B RID: 16443 RVA: 0x001A40F8 File Offset: 0x001A22F8
	public override BaseQuestAction Clone()
	{
		QuestActionSpawnEnemy questActionSpawnEnemy = new QuestActionSpawnEnemy();
		base.CopyValues(questActionSpawnEnemy);
		questActionSpawnEnemy.entityIDs.AddRange(this.entityIDs);
		return questActionSpawnEnemy;
	}

	// Token: 0x04003384 RID: 13188
	[PublicizedFrom(EAccessModifier.Private)]
	public List<int> entityIDs = new List<int>();

	// Token: 0x04003385 RID: 13189
	[PublicizedFrom(EAccessModifier.Private)]
	public int count = 1;
}

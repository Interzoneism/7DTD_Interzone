using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000199 RID: 409
public class TriggerManager
{
	// Token: 0x170000D6 RID: 214
	// (get) Token: 0x06000C6A RID: 3178 RVA: 0x00054B12 File Offset: 0x00052D12
	// (set) Token: 0x06000C6B RID: 3179 RVA: 0x00054B1A File Offset: 0x00052D1A
	public bool ShowNavObjects
	{
		get
		{
			return this.showNavObjects;
		}
		set
		{
			this.showNavObjects = value;
			this.HandleNavObjects(this.showNavObjects);
		}
	}

	// Token: 0x06000C6C RID: 3180 RVA: 0x00054B30 File Offset: 0x00052D30
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleNavObjects(bool enabled)
	{
		foreach (PrefabTriggerData prefabTriggerData in this.PrefabDataDict.Values)
		{
			if (enabled)
			{
				prefabTriggerData.SetupTriggerTestNavObjects();
			}
			else
			{
				prefabTriggerData.RemoveTriggerTestNavObjects();
			}
		}
	}

	// Token: 0x06000C6D RID: 3181 RVA: 0x00054B94 File Offset: 0x00052D94
	public void AddPrefabData(PrefabInstance instance, int entityID)
	{
		PrefabTriggerData prefabTriggerData;
		if (!this.PrefabDataDict.ContainsKey(instance))
		{
			prefabTriggerData = new PrefabTriggerData(instance)
			{
				Owner = this
			};
			if (this.ShowNavObjects)
			{
				prefabTriggerData.SetupTriggerTestNavObjects();
			}
			this.PrefabDataDict.Add(instance, prefabTriggerData);
		}
		prefabTriggerData = this.PrefabDataDict[instance];
		prefabTriggerData.RefreshTriggers();
		prefabTriggerData.AddPlayerInArea(entityID);
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00054BF4 File Offset: 0x00052DF4
	public void RefreshTriggers(PrefabInstance instance, FastTags<TagGroup.Global> questTags)
	{
		PrefabTriggerData prefabTriggerData;
		if (!this.PrefabDataDict.ContainsKey(instance))
		{
			prefabTriggerData = new PrefabTriggerData(instance)
			{
				Owner = this
			};
			this.PrefabDataDict.Add(instance, prefabTriggerData);
		}
		else
		{
			prefabTriggerData = this.PrefabDataDict[instance];
			prefabTriggerData.RemoveTriggerTestNavObjects();
			prefabTriggerData.ResetData();
		}
		prefabTriggerData.ResetTriggers();
		prefabTriggerData.RefreshTriggersForQuest(questTags);
		prefabTriggerData.HandleNeedTriggers();
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x00054C5C File Offset: 0x00052E5C
	public void Trigger(EntityPlayer player, PrefabInstance instance, byte trigger)
	{
		PrefabTriggerData prefabTriggerData;
		if (this.PrefabDataDict.TryGetValue(instance, out prefabTriggerData))
		{
			prefabTriggerData.Trigger(player, trigger);
		}
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00054C81 File Offset: 0x00052E81
	public void TriggerBlocks(EntityPlayer player, PrefabInstance instance, BlockTrigger trigger)
	{
		if (!trigger.HasAnyTriggers())
		{
			return;
		}
		if (this.PrefabDataDict.ContainsKey(instance))
		{
			this.PrefabDataDict[instance].Trigger(player, trigger);
		}
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x00054CAD File Offset: 0x00052EAD
	public void TriggerBlocks(EntityPlayer player, PrefabInstance instance, TriggerVolume trigger)
	{
		if (!trigger.HasAnyTriggers())
		{
			return;
		}
		if (this.PrefabDataDict.ContainsKey(instance))
		{
			this.PrefabDataDict[instance].Trigger(player, trigger);
		}
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x00054CD9 File Offset: 0x00052ED9
	public void RemovePlayer(PrefabInstance instance, int entityID)
	{
		if (this.PrefabDataDict.ContainsKey(instance))
		{
			this.PrefabDataDict[instance].RemovePlayerInArea(entityID);
		}
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00054CFB File Offset: 0x00052EFB
	public void RemovePrefabData(PrefabInstance instance)
	{
		if (this.PrefabDataDict.ContainsKey(instance))
		{
			this.PrefabDataDict[instance].RemoveTriggerTestNavObjects();
			this.PrefabDataDict.Remove(instance);
		}
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00054D2C File Offset: 0x00052F2C
	public void Update()
	{
		float deltaTime = Time.deltaTime;
		for (int i = this.UpdateList.Count - 1; i >= 0; i--)
		{
			this.UpdateList[i].Update(deltaTime);
		}
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00054D6C File Offset: 0x00052F6C
	public List<byte> GetTriggerLayers()
	{
		List<byte> list = new List<byte>();
		foreach (PrefabTriggerData prefabTriggerData in this.PrefabDataDict.Values)
		{
			for (int i = 0; i < prefabTriggerData.TriggeredLayers.Count; i++)
			{
				if (!list.Contains(prefabTriggerData.TriggeredLayers[i]))
				{
					list.Add(prefabTriggerData.TriggeredLayers[i]);
				}
			}
			for (int j = 0; j < prefabTriggerData.TriggeredByLayers.Count; j++)
			{
				if (!list.Contains(prefabTriggerData.TriggeredByLayers[j]))
				{
					list.Add(prefabTriggerData.TriggeredByLayers[j]);
				}
			}
		}
		return list;
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00054E48 File Offset: 0x00053048
	public void AddToUpdateList(PrefabTriggerData prefabTriggerData)
	{
		if (!this.UpdateList.Contains(prefabTriggerData))
		{
			this.UpdateList.Add(prefabTriggerData);
		}
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00054E64 File Offset: 0x00053064
	public void RemoveFromUpdateList(PrefabTriggerData prefabTriggerData)
	{
		if (this.UpdateList.Contains(prefabTriggerData))
		{
			this.UpdateList.Remove(prefabTriggerData);
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00054E84 File Offset: 0x00053084
	public void RemoveFromUpdateList(PrefabInstance instance)
	{
		for (int i = this.UpdateList.Count - 1; i >= 0; i--)
		{
			if (this.UpdateList[i].PrefabInstance == instance)
			{
				this.UpdateList.RemoveAt(i);
			}
		}
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x00054ECC File Offset: 0x000530CC
	public bool CheckPowerState(BlockTrigger _triggered, FastTags<TagGroup.Global> questTag)
	{
		bool result = false;
		if (_triggered.HasAnyTriggeredBy() && questTag.Test_AnySet(QuestEventManager.restorePowerTag))
		{
			World world = GameManager.Instance.World;
			DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
			Vector3i v3i = _triggered.ToWorldPos();
			PrefabInstance prefabAtPosition = dynamicPrefabDecorator.GetPrefabAtPosition(v3i, true);
			PrefabTriggerData prefabTriggerData;
			if (prefabAtPosition == null || !this.PrefabDataDict.TryGetValue(prefabAtPosition, out prefabTriggerData))
			{
				return result;
			}
			using (List<byte>.Enumerator enumerator = _triggered.TriggeredByIndices.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					byte item = enumerator.Current;
					foreach (BlockTrigger blockTrigger in prefabTriggerData.Triggers)
					{
						if (blockTrigger.TriggersIndices.Contains(item))
						{
							Vector3i vector3i = blockTrigger.ToWorldPos();
							BlockEntityData blockEntity = ((Chunk)world.GetChunkFromWorldPos(vector3i)).GetBlockEntity(vector3i);
							if (blockEntity != null && blockEntity.transform != null)
							{
								QuestGeneratorController component = blockEntity.transform.GetComponent<QuestGeneratorController>();
								if (component != null && component.OnState.activeSelf)
								{
									return true;
								}
							}
						}
					}
				}
				return result;
			}
		}
		result = true;
		return result;
	}

	// Token: 0x04000A62 RID: 2658
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<PrefabInstance, PrefabTriggerData> PrefabDataDict = new Dictionary<PrefabInstance, PrefabTriggerData>();

	// Token: 0x04000A63 RID: 2659
	public List<PrefabTriggerData> UpdateList = new List<PrefabTriggerData>();

	// Token: 0x04000A64 RID: 2660
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showNavObjects;
}

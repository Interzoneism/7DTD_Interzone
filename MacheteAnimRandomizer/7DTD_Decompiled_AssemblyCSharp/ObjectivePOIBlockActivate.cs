using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008C9 RID: 2249
[Preserve]
public class ObjectivePOIBlockActivate : BaseObjective
{
	// Token: 0x170006D8 RID: 1752
	// (get) Token: 0x060041FE RID: 16894 RVA: 0x000197A5 File Offset: 0x000179A5
	public override BaseObjective.ObjectiveValueTypes ObjectiveValueType
	{
		get
		{
			return BaseObjective.ObjectiveValueTypes.Number;
		}
	}

	// Token: 0x170006D9 RID: 1753
	// (get) Token: 0x060041FF RID: 16895 RVA: 0x001A6B33 File Offset: 0x001A4D33
	public override bool UpdateUI
	{
		get
		{
			return base.ObjectiveState != BaseObjective.ObjectiveStates.Failed;
		}
	}

	// Token: 0x06004200 RID: 16896 RVA: 0x001AA5AB File Offset: 0x001A87AB
	public override void SetupQuestTag()
	{
		base.OwnerQuest.AddQuestTag(QuestEventManager.restorePowerTag);
	}

	// Token: 0x06004201 RID: 16897 RVA: 0x001AA5BD File Offset: 0x001A87BD
	public override void SetupObjective()
	{
		this.keyword = Localization.Get("ObjectiveRestorePower_keyword", false);
		base.OwnerQuest.AddQuestTag(QuestEventManager.restorePowerTag);
	}

	// Token: 0x06004202 RID: 16898 RVA: 0x001AA5E0 File Offset: 0x001A87E0
	public override void SetupDisplay()
	{
		base.Description = this.keyword;
		if (this.neededCount == -1)
		{
			this.StatusText = "";
			return;
		}
		this.StatusText = string.Format("{0}/{1}", base.CurrentValue, this.neededCount);
	}

	// Token: 0x06004203 RID: 16899 RVA: 0x001AA634 File Offset: 0x001A8834
	public override void AddHooks()
	{
		QuestEventManager.Current.AddObjectiveToBeUpdated(this);
		QuestEventManager.Current.BlockDestroy -= this.Current_BlockDestroy;
		QuestEventManager.Current.BlockDestroy += this.Current_BlockDestroy;
		QuestEventManager.Current.BlockActivate -= this.Current_BlockActivate;
		QuestEventManager.Current.BlockActivate += this.Current_BlockActivate;
	}

	// Token: 0x06004204 RID: 16900 RVA: 0x001AA6A4 File Offset: 0x001A88A4
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockDestroy(Block block, Vector3i blockPos)
	{
		if (this.activateList == null)
		{
			return;
		}
		if (!this.activateList.Contains(blockPos))
		{
			return;
		}
		base.OwnerQuest.CloseQuest(Quest.QuestState.Failed, null);
	}

	// Token: 0x06004205 RID: 16901 RVA: 0x001AA6CC File Offset: 0x001A88CC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Current_BlockActivate(string blockname, Vector3i blockPos)
	{
		if (this.activateList == null)
		{
			return;
		}
		if (base.Complete)
		{
			return;
		}
		NavObjectManager.Instance.UnRegisterNavObjectByPosition(blockPos.ToVector3() + Vector3.one * 0.5f, this.NavObjectName);
		if (!this.activateList.Contains(blockPos))
		{
			return;
		}
		Block blockByName = Block.GetBlockByName(blockname, false);
		if ((this.ID == null || this.ID == "" || this.ID.EqualsCaseInsensitive(blockByName.IndexName)) && base.OwnerQuest.CheckRequirements())
		{
			byte currentValue = base.CurrentValue;
			base.CurrentValue = currentValue + 1;
			this.activateList.Remove(blockPos);
			this.Refresh();
		}
	}

	// Token: 0x06004206 RID: 16902 RVA: 0x001AA78C File Offset: 0x001A898C
	public void AddActivatedBlock(Vector3i blockPos)
	{
		NavObjectManager.Instance.UnRegisterNavObjectByPosition(blockPos.ToVector3() + Vector3.one * 0.5f, this.NavObjectName);
		byte currentValue = base.CurrentValue;
		base.CurrentValue = currentValue + 1;
		this.activateList.Remove(blockPos);
	}

	// Token: 0x06004207 RID: 16903 RVA: 0x001AA7E4 File Offset: 0x001A89E4
	public override void RemoveHooks()
	{
		QuestEventManager questEventManager = QuestEventManager.Current;
		questEventManager.RemoveObjectiveToBeUpdated(this);
		questEventManager.BlockActivate -= this.Current_BlockActivate;
		questEventManager.BlockDestroy -= this.Current_BlockDestroy;
		this.ClearNavObjects();
		if (base.OwnerQuest != null && base.OwnerQuest.RallyMarkerActivated)
		{
			questEventManager.ActiveQuestBlocks.Clear();
		}
	}

	// Token: 0x06004208 RID: 16904 RVA: 0x001AA848 File Offset: 0x001A8A48
	[PublicizedFrom(EAccessModifier.Private)]
	public void ClearNavObjects()
	{
		if (this.activateList == null)
		{
			return;
		}
		for (int i = 0; i < this.activateList.Count; i++)
		{
			NavObjectManager.Instance.UnRegisterNavObjectByPosition(this.activateList[i].ToVector3() + Vector3.one * 0.5f, this.NavObjectName);
		}
		this.activateList.Clear();
	}

	// Token: 0x06004209 RID: 16905 RVA: 0x001AA8B8 File Offset: 0x001A8AB8
	public override void Refresh()
	{
		if (this.neededCount == -1)
		{
			return;
		}
		if ((int)base.CurrentValue > this.neededCount)
		{
			base.CurrentValue = (byte)this.neededCount;
		}
		if (base.Complete)
		{
			return;
		}
		base.Complete = ((int)base.CurrentValue >= this.neededCount);
		if (base.Complete)
		{
			base.OwnerQuest.RefreshQuestCompletion(QuestClass.CompletionTypes.AutoComplete, null, true, null);
			base.HandleRemoveHooks();
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				QuestEventManager.Current.FinishManagedQuest(base.OwnerQuest.QuestCode, base.OwnerQuest.OwnerJournal.OwnerPlayer);
				return;
			}
			Vector3 zero = Vector3.zero;
			base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.FinishManagedQuest, base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId, zero, base.OwnerQuest.QuestCode), false);
		}
	}

	// Token: 0x0600420A RID: 16906 RVA: 0x001AA9AC File Offset: 0x001A8BAC
	public override void Update(float deltaTime)
	{
		if (Time.time > this.updateTime)
		{
			this.updateTime = Time.time + 1f;
			if (this.neededCount == -1)
			{
				Vector3 zero = Vector3.zero;
				base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
				if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
				{
					if (base.OwnerQuest.SharedOwnerID == -1)
					{
						this.activateList = new List<Vector3i>();
						QuestEventManager.Current.SetupActivateForMP(base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId, base.OwnerQuest.QuestCode, this.completeEvent, this.activateList, GameManager.Instance.World, zero, this.ID, base.OwnerQuest.GetSharedWithIDList());
						this.SetupActivationList(zero, this.activateList);
						return;
					}
				}
				else
				{
					if (base.OwnerQuest.SharedOwnerID == -1)
					{
						SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(NetPackageManager.GetPackage<NetPackageQuestEvent>().Setup(NetPackageQuestEvent.QuestEventTypes.SetupRestorePower, base.OwnerQuest.OwnerJournal.OwnerPlayer.entityId, base.OwnerQuest.QuestCode, this.completeEvent, zero, this.ID, base.OwnerQuest.GetSharedWithIDList()), false);
						return;
					}
					if (QuestEventManager.Current.ActiveQuestBlocks != null && QuestEventManager.Current.ActiveQuestBlocks.Count > 0)
					{
						this.SetupActivationList(zero, QuestEventManager.Current.ActiveQuestBlocks);
					}
				}
			}
		}
	}

	// Token: 0x0600420B RID: 16907 RVA: 0x001AAB14 File Offset: 0x001A8D14
	public override bool SetupActivationList(Vector3 prefabPos, List<Vector3i> newActivateList)
	{
		Vector3 zero = Vector3.zero;
		base.OwnerQuest.GetPositionData(out zero, Quest.PositionDataTypes.POIPosition);
		if (zero.x != prefabPos.x && zero.z != prefabPos.z)
		{
			return false;
		}
		base.CurrentValue = 0;
		this.neededCount = newActivateList.Count;
		byte currentValue = 0;
		for (int i = 0; i < newActivateList.Count; i++)
		{
			NavObjectManager.Instance.RegisterNavObject(this.NavObjectName, newActivateList[i].ToVector3() + Vector3.one * 0.5f, "", false, -1, null);
		}
		base.CurrentValue = currentValue;
		QuestEventManager.Current.ActiveQuestBlocks = newActivateList;
		this.activateList = newActivateList;
		this.Refresh();
		this.SetupDisplay();
		return true;
	}

	// Token: 0x0600420C RID: 16908 RVA: 0x001AABDC File Offset: 0x001A8DDC
	public override BaseObjective Clone()
	{
		ObjectivePOIBlockActivate objectivePOIBlockActivate = new ObjectivePOIBlockActivate();
		this.CopyValues(objectivePOIBlockActivate);
		objectivePOIBlockActivate.completeEvent = this.completeEvent;
		objectivePOIBlockActivate.neededCount = this.neededCount;
		objectivePOIBlockActivate.activateList = this.activateList;
		return objectivePOIBlockActivate;
	}

	// Token: 0x0600420D RID: 16909 RVA: 0x001AAC1C File Offset: 0x001A8E1C
	public override void ParseProperties(DynamicProperties properties)
	{
		base.ParseProperties(properties);
		if (properties.Values.ContainsKey(ObjectivePOIBlockActivate.PropBlockName))
		{
			this.ID = properties.Values[ObjectivePOIBlockActivate.PropBlockName];
		}
		properties.ParseString(ObjectivePOIBlockActivate.PropEventComplete, ref this.completeEvent);
	}

	// Token: 0x0400346E RID: 13422
	[PublicizedFrom(EAccessModifier.Private)]
	public string localizedName = "";

	// Token: 0x0400346F RID: 13423
	[PublicizedFrom(EAccessModifier.Private)]
	public int neededCount = -1;

	// Token: 0x04003470 RID: 13424
	[PublicizedFrom(EAccessModifier.Private)]
	public string completeEvent = "";

	// Token: 0x04003471 RID: 13425
	[PublicizedFrom(EAccessModifier.Private)]
	public List<Vector3i> activateList;

	// Token: 0x04003472 RID: 13426
	public static string PropBlockName = "block_index";

	// Token: 0x04003473 RID: 13427
	public static string PropEventComplete = "complete_event";

	// Token: 0x04003474 RID: 13428
	[PublicizedFrom(EAccessModifier.Private)]
	public new float updateTime;
}

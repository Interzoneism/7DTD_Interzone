using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x020008A5 RID: 2213
[Preserve]
public class TreasureQuestData : BaseQuestData
{
	// Token: 0x170006A9 RID: 1705
	// (get) Token: 0x0600407B RID: 16507 RVA: 0x001A4AE8 File Offset: 0x001A2CE8
	// (set) Token: 0x0600407C RID: 16508 RVA: 0x001A4AF0 File Offset: 0x001A2CF0
	public int BlocksPerReduction { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170006AA RID: 1706
	// (get) Token: 0x0600407D RID: 16509 RVA: 0x001A4AF9 File Offset: 0x001A2CF9
	// (set) Token: 0x0600407E RID: 16510 RVA: 0x001A4B01 File Offset: 0x001A2D01
	public Vector3i Position { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170006AB RID: 1707
	// (get) Token: 0x0600407F RID: 16511 RVA: 0x001A4B0A File Offset: 0x001A2D0A
	// (set) Token: 0x06004080 RID: 16512 RVA: 0x001A4B12 File Offset: 0x001A2D12
	public Vector3 TreasureOffset { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x06004081 RID: 16513 RVA: 0x001A4B1B File Offset: 0x001A2D1B
	public TreasureQuestData(int _questCode, int _entityID, int _blocksPerReduction, Vector3i _position, Vector3 _treasureOffset)
	{
		this.questCode = _questCode;
		this.entityList.Add(_entityID);
		this.Position = _position;
		this.TreasureOffset = _treasureOffset;
		this.BlocksPerReduction = _blocksPerReduction;
	}

	// Token: 0x06004082 RID: 16514 RVA: 0x001A4B4D File Offset: 0x001A2D4D
	public void AddSharedQuester(int _entityID, int _blocksPerReduction)
	{
		if (_blocksPerReduction < this.BlocksPerReduction)
		{
			this.SendBlocksPerReductionUpdate(this.BlocksPerReduction);
		}
		base.AddSharedQuester(_entityID);
	}

	// Token: 0x06004083 RID: 16515 RVA: 0x001A4B6C File Offset: 0x001A2D6C
	public void SendBlocksPerReductionUpdate(int _newBlocksPerReduction)
	{
		this.BlocksPerReduction = _newBlocksPerReduction;
		World world = GameManager.Instance.World;
		for (int i = 0; i < this.entityList.Count; i++)
		{
			EntityPlayer entityPlayer = world.GetEntity(this.entityList[i]) as EntityPlayer;
			if (entityPlayer is EntityPlayerLocal)
			{
				ObjectiveTreasureChest objectiveForQuest = entityPlayer.QuestJournal.GetObjectiveForQuest<ObjectiveTreasureChest>(this.questCode);
				if (objectiveForQuest != null)
				{
					objectiveForQuest.CurrentBlocksPerReduction = this.BlocksPerReduction;
				}
			}
			else
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageQuestTreasurePoint>().Setup(this.questCode, this.BlocksPerReduction), false, this.entityList[i], -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06004084 RID: 16516 RVA: 0x001A4C29 File Offset: 0x001A2E29
	public void UpdatePosition(Vector3i _pos)
	{
		this.Position = _pos;
	}

	// Token: 0x06004085 RID: 16517 RVA: 0x001A4C32 File Offset: 0x001A2E32
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void RemoveFromDictionary()
	{
		QuestEventManager.Current.TreasureQuestDictionary.Remove(this.questCode);
	}

	// Token: 0x06004086 RID: 16518 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnRemove(EntityPlayer player)
	{
	}
}

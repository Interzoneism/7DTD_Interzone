using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200089D RID: 2205
[Preserve]
public class QuestActionTreasureChest : BaseQuestAction
{
	// Token: 0x0600405B RID: 16475 RVA: 0x00002914 File Offset: 0x00000B14
	public override void SetupAction()
	{
	}

	// Token: 0x0600405C RID: 16476 RVA: 0x001A47A4 File Offset: 0x001A29A4
	public override void PerformAction(Quest ownerQuest)
	{
		World world = GameManager.Instance.World;
		EntityPlayer ownerPlayer = ownerQuest.OwnerJournal.OwnerPlayer;
		float d = (this.Value == "" || this.Value == null) ? 50f : StringParsers.ParseFloat(this.Value, 0, -1, NumberStyles.Any);
		GameRandom gameRandom = world.GetGameRandom();
		Vector3 a = new Vector3(-1f + 2f * gameRandom.RandomFloat, 0f, -1f + 2f * gameRandom.RandomFloat);
		a.Normalize();
		Vector3 vector = ownerPlayer.position + a * d;
		int num = (int)vector.x;
		int num2 = (int)vector.z;
		int num3 = (int)(world.GetHeight(num, num2) - 3);
		BlockValue blockValue = new BlockValue
		{
			type = 372
		};
		Vector3i blockPos = new Vector3i(num, num3, num2);
		world.SetBlockRPC(blockPos, blockValue, sbyte.MaxValue);
		ownerQuest.DataVariables.Add("treasurecontainer", string.Format("{0},{1},{2}", num, num3, num2));
	}

	// Token: 0x0600405D RID: 16477 RVA: 0x001A48CC File Offset: 0x001A2ACC
	public override BaseQuestAction Clone()
	{
		QuestActionTreasureChest questActionTreasureChest = new QuestActionTreasureChest();
		base.CopyValues(questActionTreasureChest);
		return questActionTreasureChest;
	}
}

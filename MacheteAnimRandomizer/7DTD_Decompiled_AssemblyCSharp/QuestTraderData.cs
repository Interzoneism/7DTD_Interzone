using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000915 RID: 2325
public class QuestTraderData
{
	// Token: 0x06004544 RID: 17732 RVA: 0x001BBD9C File Offset: 0x001B9F9C
	public QuestTraderData()
	{
	}

	// Token: 0x06004545 RID: 17733 RVA: 0x001BBDC1 File Offset: 0x001B9FC1
	public QuestTraderData(Vector2 traderPOI)
	{
		this.TraderPOI = traderPOI;
	}

	// Token: 0x06004546 RID: 17734 RVA: 0x001BBDF0 File Offset: 0x001B9FF0
	public void AddPOI(int tier, Vector2 poiPosition)
	{
		if (!this.CompletedPOIByTier.ContainsKey(tier))
		{
			this.CompletedPOIByTier.Add(tier, new List<Vector2>());
		}
		if (!this.CompletedPOIByTier[tier].Contains(poiPosition))
		{
			this.CompletedPOIByTier[tier].Add(poiPosition);
		}
		if (this.resetDay == -1)
		{
			this.resetDay = GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime);
		}
	}

	// Token: 0x06004547 RID: 17735 RVA: 0x001BBE68 File Offset: 0x001BA068
	public void ClearTier(int tier)
	{
		if (tier == -1)
		{
			this.resetDay = -1;
			for (int i = QuestTraderData.resetStartTier; i <= QuestTraderData.fullTierCount; i++)
			{
				if (this.CompletedPOIByTier.ContainsKey(i))
				{
					this.CompletedPOIByTier.Remove(i);
				}
			}
			return;
		}
		if (this.CompletedPOIByTier.ContainsKey(tier))
		{
			this.CompletedPOIByTier.Remove(tier);
		}
	}

	// Token: 0x06004548 RID: 17736 RVA: 0x001BBECC File Offset: 0x001BA0CC
	public void CheckReset(EntityPlayer player)
	{
		if (this.resetDay != -1 && GameUtils.WorldTimeToDays(GameManager.Instance.World.worldTime) - this.resetDay >= 7)
		{
			this.resetDay = -1;
			for (int i = QuestTraderData.resetStartTier; i <= QuestTraderData.fullTierCount; i++)
			{
				if (this.CompletedPOIByTier.ContainsKey(i))
				{
					this.CompletedPOIByTier.Remove(i);
				}
			}
			if (!(player is EntityPlayerLocal))
			{
				SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageNPCQuestList>().SetupClear(player.entityId, this.TraderPOI, -1), false, player.entityId, -1, -1, null, 192, false);
			}
		}
	}

	// Token: 0x06004549 RID: 17737 RVA: 0x001BBF7A File Offset: 0x001BA17A
	public List<Vector2> GetTierPOIs(int tier)
	{
		if (this.CompletedPOIByTier.ContainsKey(tier))
		{
			return this.CompletedPOIByTier[tier];
		}
		return null;
	}

	// Token: 0x0600454A RID: 17738 RVA: 0x001BBF98 File Offset: 0x001BA198
	public void Read(BinaryReader _br, byte version)
	{
		this.TraderPOI = StreamUtils.ReadVector2(_br);
		int num = (int)_br.ReadByte();
		this.CompletedPOIByTier.Clear();
		for (int i = 0; i < num; i++)
		{
			int key = (int)_br.ReadByte();
			int num2 = _br.ReadInt32();
			if (num2 > 0)
			{
				List<Vector2> list = new List<Vector2>();
				for (int j = 0; j < num2; j++)
				{
					list.Add(StreamUtils.ReadVector2(_br));
				}
				this.CompletedPOIByTier.Add(key, list);
			}
		}
		int num3 = (int)_br.ReadByte();
		this.TradersSentTo.Clear();
		for (int k = 0; k < num3; k++)
		{
			this.TradersSentTo.Add(StreamUtils.ReadVector2(_br));
		}
		this.resetDay = _br.ReadInt32();
	}

	// Token: 0x0600454B RID: 17739 RVA: 0x001BC058 File Offset: 0x001BA258
	public void Write(BinaryWriter _bw)
	{
		StreamUtils.Write(_bw, this.TraderPOI);
		_bw.Write((byte)this.CompletedPOIByTier.Count);
		foreach (int num in this.CompletedPOIByTier.Keys)
		{
			_bw.Write((byte)num);
			List<Vector2> list = this.CompletedPOIByTier[num];
			_bw.Write(list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				StreamUtils.Write(_bw, list[i]);
			}
		}
		_bw.Write((byte)this.TradersSentTo.Count);
		for (int j = 0; j < this.TradersSentTo.Count; j++)
		{
			StreamUtils.Write(_bw, this.TradersSentTo[j]);
		}
		_bw.Write(this.resetDay);
	}

	// Token: 0x04003639 RID: 13881
	public QuestJournal Owner;

	// Token: 0x0400363A RID: 13882
	public Vector2 TraderPOI;

	// Token: 0x0400363B RID: 13883
	public List<Vector2> TradersSentTo = new List<Vector2>();

	// Token: 0x0400363C RID: 13884
	public Dictionary<int, List<Vector2>> CompletedPOIByTier = new Dictionary<int, List<Vector2>>();

	// Token: 0x0400363D RID: 13885
	[PublicizedFrom(EAccessModifier.Private)]
	public int resetDay = -1;

	// Token: 0x0400363E RID: 13886
	[PublicizedFrom(EAccessModifier.Private)]
	public static int resetStartTier = 4;

	// Token: 0x0400363F RID: 13887
	[PublicizedFrom(EAccessModifier.Private)]
	public static int fullTierCount = 6;
}

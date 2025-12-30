using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Challenges;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001580 RID: 5504
	public class TwitchViewerData
	{
		// Token: 0x170012CE RID: 4814
		// (get) Token: 0x0600A967 RID: 43367 RVA: 0x0042E167 File Offset: 0x0042C367
		// (set) Token: 0x0600A968 RID: 43368 RVA: 0x0042E16F File Offset: 0x0042C36F
		public float PointRate
		{
			get
			{
				return this.pointRate;
			}
			set
			{
				this.pointRate = value;
				this.PointRateSubs = value * 2f;
			}
		}

		// Token: 0x0600A969 RID: 43369 RVA: 0x0042E188 File Offset: 0x0042C388
		public TwitchViewerData(TwitchManager owner)
		{
			this.Owner = owner;
		}

		// Token: 0x0600A96A RID: 43370 RVA: 0x0042E244 File Offset: 0x0042C444
		public int GetSubTierPoints(TwitchSubEventEntry.SubTierTypes tier)
		{
			if (tier == TwitchSubEventEntry.SubTierTypes.Tier2)
			{
				return this.SubPointAddTier2;
			}
			if (tier != TwitchSubEventEntry.SubTierTypes.Tier3)
			{
				return this.SubPointAddTier1;
			}
			return this.SubPointAddTier3;
		}

		// Token: 0x0600A96B RID: 43371 RVA: 0x0042E264 File Offset: 0x0042C464
		public string GetRandomActiveViewer()
		{
			string userName = this.Owner.Authentication.userName;
			List<string> list = new List<string>();
			foreach (string text in this.ViewerEntries.Keys)
			{
				if (this.ViewerEntries[text].IsActive && text != userName)
				{
					list.Add(text);
				}
			}
			if (list.Count > 0)
			{
				return list[GameEventManager.Current.Random.RandomRange(list.Count)];
			}
			return "";
		}

		// Token: 0x0600A96C RID: 43372 RVA: 0x0042E31C File Offset: 0x0042C51C
		public int GetGiftSubTierPoints(TwitchSubEventEntry.SubTierTypes tier)
		{
			if (tier == TwitchSubEventEntry.SubTierTypes.Tier2)
			{
				return this.GiftSubPointAddTier2;
			}
			if (tier != TwitchSubEventEntry.SubTierTypes.Tier3)
			{
				return this.GiftSubPointAddTier1;
			}
			return this.GiftSubPointAddTier3;
		}

		// Token: 0x0600A96D RID: 43373 RVA: 0x0042E33C File Offset: 0x0042C53C
		public void SetupLocalization()
		{
			this.chatOutput_AddPPAll = Localization.Get("TwitchChat_AddPPAll", false);
			this.chatOutput_AddSPAll = Localization.Get("TwitchChat_AddSPAll", false);
			this.chatOutput_ErrorAddingBitCredits = Localization.Get("TwitchChat_ErrorAddingBitCredit", false);
			this.chatOutput_ErrorAddingPoints = Localization.Get("TwitchChat_ErrorAddingPoints", false);
			this.chatOutput_GiftedSubs = Localization.Get("TwitchChat_GiftedSubs", false);
			this.ingameOutput_GiftedSubs = Localization.Get("TwitchInGame_GiftedSubs", false);
		}

		// Token: 0x0600A96E RID: 43374 RVA: 0x0042E3B0 File Offset: 0x0042C5B0
		public void Update(float deltaTime)
		{
			this.NextActionTime -= deltaTime;
			if (this.NextActionTime <= 0f)
			{
				this.IncrementViewerEntries();
				this.NextActionTime = 10f;
			}
			for (int i = this.SubEntries.Count - 1; i >= 0; i--)
			{
				if (this.SubEntries[i].Update(deltaTime))
				{
					GiftSubEntry giftSubEntry = this.SubEntries[i];
					ViewerEntry viewerEntry = this.GetViewerEntry(giftSubEntry.UserName);
					viewerEntry.UserID = giftSubEntry.UserID;
					int num = this.GetGiftSubTierPoints(giftSubEntry.Tier) * giftSubEntry.SubCount * this.Owner.GiftSubPointModifier;
					if (num > 0)
					{
						viewerEntry.SpecialPoints += (float)num;
						this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_GiftedSubs, new object[]
						{
							giftSubEntry.UserName,
							viewerEntry.CombinedPoints,
							giftSubEntry.SubCount,
							this.Owner.GetTierName(giftSubEntry.Tier),
							num
						}), true);
						this.SubEntries.RemoveAt(i);
						string message = string.Format(this.ingameOutput_GiftedSubs, new object[]
						{
							giftSubEntry.UserName,
							giftSubEntry.SubCount,
							this.Owner.GetTierName(giftSubEntry.Tier),
							num
						});
						XUiC_ChatOutput.AddMessage(this.Owner.LocalPlayerXUi, EnumGameMessages.PlainTextLocal, message, EChatType.Global, EChatDirection.Inbound, -1, null, null, EMessageSender.Server, GeneratedTextManager.TextFilteringMode.None, GeneratedTextManager.BbCodeSupportMode.Supported);
					}
					this.Owner.HandleGiftSubEvent(giftSubEntry.UserName, giftSubEntry.SubCount, giftSubEntry.Tier);
				}
			}
		}

		// Token: 0x0600A96F RID: 43375 RVA: 0x0042E56C File Offset: 0x0042C76C
		public void AddGiftSubEntry(string userName, int userID, TwitchSubEventEntry.SubTierTypes tier)
		{
			for (int i = 0; i < this.SubEntries.Count; i++)
			{
				if (this.SubEntries[i].UserName == userName)
				{
					this.SubEntries[i].AddSub();
					return;
				}
			}
			this.SubEntries.Add(new GiftSubEntry(userName, userID, tier));
		}

		// Token: 0x0600A970 RID: 43376 RVA: 0x0042E5D0 File Offset: 0x0042C7D0
		public ViewerEntry AddCredit(string name, int credit, bool displayNewTotal)
		{
			ViewerEntry viewerEntry = this.AddToViewerEntry(name, credit, TwitchAction.PointTypes.Bits);
			if (viewerEntry == null)
			{
				this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_ErrorAddingBitCredits, name), true);
			}
			else if (displayNewTotal)
			{
				this.Owner.SendChannelCreditOutputMessage(name, viewerEntry);
			}
			return viewerEntry;
		}

		// Token: 0x0600A971 RID: 43377 RVA: 0x0042E61C File Offset: 0x0042C81C
		public void AddPoints(string name, int points, bool isSpecial, bool displayNewTotal)
		{
			if (name == "")
			{
				foreach (string key in this.ViewerEntries.Keys)
				{
					if (this.ViewerEntries[key].IsActive)
					{
						if (isSpecial)
						{
							this.ViewerEntries[key].SpecialPoints += (float)points;
							if (this.ViewerEntries[key].SpecialPoints < 0f)
							{
								this.ViewerEntries[key].SpecialPoints = 0f;
							}
						}
						else
						{
							this.ViewerEntries[key].StandardPoints += (float)points;
							if (this.ViewerEntries[key].StandardPoints < 0f)
							{
								this.ViewerEntries[key].StandardPoints = 0f;
							}
						}
					}
				}
				if (isSpecial)
				{
					this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_AddSPAll, points), true);
					return;
				}
				this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_AddPPAll, points), true);
				return;
			}
			else
			{
				ViewerEntry viewerEntry = this.AddToViewerEntry(name, points, isSpecial ? TwitchAction.PointTypes.SP : TwitchAction.PointTypes.PP);
				if (viewerEntry == null)
				{
					this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_ErrorAddingPoints, name), true);
					return;
				}
				if (displayNewTotal)
				{
					this.Owner.SendChannelPointOutputMessage(name, viewerEntry);
				}
				return;
			}
		}

		// Token: 0x0600A972 RID: 43378 RVA: 0x0042E7BC File Offset: 0x0042C9BC
		public void AddPointsAll(int standardPoints, int specialPoints, bool announceToChat = true)
		{
			foreach (string key in this.ViewerEntries.Keys)
			{
				if (this.ViewerEntries[key].IsActive)
				{
					if (standardPoints != 0)
					{
						this.ViewerEntries[key].StandardPoints += (float)standardPoints;
						if (this.ViewerEntries[key].StandardPoints < 0f)
						{
							this.ViewerEntries[key].StandardPoints = 0f;
						}
					}
					if (specialPoints != 0)
					{
						this.ViewerEntries[key].SpecialPoints += (float)specialPoints;
						if (this.ViewerEntries[key].SpecialPoints < 0f)
						{
							this.ViewerEntries[key].SpecialPoints = 0f;
						}
					}
				}
			}
			if (announceToChat)
			{
				if (standardPoints != 0)
				{
					this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_AddPPAll, standardPoints), true);
				}
				if (specialPoints != 0)
				{
					this.Owner.ircClient.SendChannelMessage(string.Format(this.chatOutput_AddSPAll, specialPoints), true);
				}
			}
		}

		// Token: 0x0600A973 RID: 43379 RVA: 0x0042E90C File Offset: 0x0042CB0C
		public void Write(BinaryWriter bw)
		{
			int num = 0;
			foreach (string text in this.ViewerEntries.Keys)
			{
				if (text.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1 && (this.ViewerEntries[text].StandardPoints > 0f || this.ViewerEntries[text].SpecialPoints > 0f))
				{
					num++;
				}
			}
			bw.Write(num);
			foreach (string text2 in this.ViewerEntries.Keys)
			{
				if (text2.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1)
				{
					ViewerEntry viewerEntry = this.ViewerEntries[text2];
					if (viewerEntry.StandardPoints > 0f || viewerEntry.SpecialPoints > 0f)
					{
						bw.Write(text2);
						bw.Write(viewerEntry.UserID);
						bw.Write(viewerEntry.StandardPoints);
					}
				}
			}
		}

		// Token: 0x0600A974 RID: 43380 RVA: 0x0042EA44 File Offset: 0x0042CC44
		public void WriteSpecial(BinaryWriter bw)
		{
			int num = 0;
			foreach (string text in this.ViewerEntries.Keys)
			{
				ViewerEntry viewerEntry = this.ViewerEntries[text];
				if (text.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1 && (viewerEntry.SpecialPoints > 0f || viewerEntry.BitCredits > 0))
				{
					num++;
				}
			}
			bw.Write(num);
			foreach (string text2 in this.ViewerEntries.Keys)
			{
				if (text2.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1)
				{
					ViewerEntry viewerEntry2 = this.ViewerEntries[text2];
					if (viewerEntry2.SpecialPoints > 0f || viewerEntry2.BitCredits > 0)
					{
						bw.Write(text2);
						bw.Write(viewerEntry2.UserID);
						bw.Write(viewerEntry2.SpecialPoints);
						bw.Write(viewerEntry2.BitCredits);
					}
				}
			}
		}

		// Token: 0x0600A975 RID: 43381 RVA: 0x0042EB7C File Offset: 0x0042CD7C
		public void WriteExport(string savePath)
		{
			using (StreamWriter streamWriter = SdFile.CreateText(savePath))
			{
				this.WriteExport(streamWriter);
			}
		}

		// Token: 0x0600A976 RID: 43382 RVA: 0x0042EBB4 File Offset: 0x0042CDB4
		[PublicizedFrom(EAccessModifier.Private)]
		public void WriteExport(TextWriter tw)
		{
			tw.WriteLine("Name|UserID|PP|SP|Bit Credit");
			foreach (string text in this.ViewerEntries.Keys)
			{
				if (text.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1)
				{
					ViewerEntry viewerEntry = this.ViewerEntries[text];
					tw.WriteLine(string.Format("{0}|{1}|{2}|{3}|{4}", new object[]
					{
						text,
						viewerEntry.UserID,
						viewerEntry.StandardPoints,
						viewerEntry.SpecialPoints,
						viewerEntry.BitCredits
					}));
				}
			}
		}

		// Token: 0x0600A977 RID: 43383 RVA: 0x0042EC80 File Offset: 0x0042CE80
		public void LoadExport(TextReader tr)
		{
			tr.ReadLine();
			Dictionary<string, ViewerEntry> dictionary = new Dictionary<string, ViewerEntry>();
			while (tr.Peek() >= 0)
			{
				string[] array = tr.ReadLine().Split('|', StringSplitOptions.None);
				if (array.Length == 5)
				{
					ViewerEntry viewerEntry;
					if (this.ViewerEntries.ContainsKey(array[0]))
					{
						viewerEntry = this.ViewerEntries[array[0]];
					}
					else
					{
						viewerEntry = new ViewerEntry();
					}
					viewerEntry.StandardPoints = (float)StringParsers.ParseSInt32(array[2], 0, -1, NumberStyles.Integer);
					viewerEntry.SpecialPoints = (float)StringParsers.ParseSInt32(array[3], 0, -1, NumberStyles.Integer);
					viewerEntry.BitCredits = StringParsers.ParseSInt32(array[4], 0, -1, NumberStyles.Integer);
					dictionary.Add(array[0], viewerEntry);
				}
			}
			this.ViewerEntries.Clear();
			dictionary.CopyTo(this.ViewerEntries, true);
		}

		// Token: 0x0600A978 RID: 43384 RVA: 0x0042ED40 File Offset: 0x0042CF40
		public void Read(BinaryReader br, byte currentVersion)
		{
			this.ViewerEntries.Clear();
			int num = br.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = br.ReadString();
				int num2 = -1;
				if (currentVersion > 14)
				{
					num2 = br.ReadInt32();
				}
				float standardPoints = br.ReadSingle();
				if (text.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1)
				{
					if (num2 != -1)
					{
						this.AddToIDLookup(num2, text, false);
					}
					this.ViewerEntries.Add(text, new ViewerEntry
					{
						UserID = num2,
						StandardPoints = standardPoints
					});
				}
			}
		}

		// Token: 0x0600A979 RID: 43385 RVA: 0x0042EDC4 File Offset: 0x0042CFC4
		public void ReadSpecial(BinaryReader br, byte currentVersion)
		{
			int num = br.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				string text = br.ReadString();
				int num2 = -1;
				if (currentVersion > 1)
				{
					num2 = br.ReadInt32();
				}
				float specialPoints = br.ReadSingle();
				int bitCredits = 0;
				if (currentVersion > 2)
				{
					bitCredits = br.ReadInt32();
				}
				if (text.IndexOfAny(TwitchViewerData.UsernameExcludeCharacters) == -1)
				{
					if (num2 != -1)
					{
						this.AddToIDLookup(num2, text, false);
					}
					ViewerEntry viewerEntry = this.GetViewerEntry(text);
					viewerEntry.UserID = num2;
					viewerEntry.SpecialPoints = specialPoints;
					if (currentVersion > 2)
					{
						viewerEntry.BitCredits = bitCredits;
					}
				}
			}
		}

		// Token: 0x0600A97A RID: 43386 RVA: 0x0042EE54 File Offset: 0x0042D054
		[PublicizedFrom(EAccessModifier.Private)]
		public void MoveStandardToSpecialPoints()
		{
			foreach (string key in this.ViewerEntries.Keys)
			{
				this.ViewerEntries[key].SpecialPoints += this.ViewerEntries[key].StandardPoints;
				this.ViewerEntries[key].StandardPoints = 0f;
			}
		}

		// Token: 0x0600A97B RID: 43387 RVA: 0x0042EEE4 File Offset: 0x0042D0E4
		public void ResetAllStandardPoints()
		{
			foreach (string key in this.ViewerEntries.Keys)
			{
				this.ViewerEntries[key].StandardPoints = 0f;
			}
		}

		// Token: 0x0600A97C RID: 43388 RVA: 0x0042EF4C File Offset: 0x0042D14C
		public void ResetAllSpecialPoints()
		{
			foreach (string key in this.ViewerEntries.Keys)
			{
				this.ViewerEntries[key].StandardPoints = 0f;
			}
		}

		// Token: 0x0600A97D RID: 43389 RVA: 0x0042EFB4 File Offset: 0x0042D1B4
		public void Cleanup()
		{
			List<string> list = new List<string>();
			foreach (string text in this.ViewerEntries.Keys)
			{
				string text2 = text.ToLower();
				if (text2 != text)
				{
					ViewerEntry viewerEntry = this.ViewerEntries[text];
					if (this.ViewerEntries.ContainsKey(text2))
					{
						ViewerEntry viewerEntry2 = this.ViewerEntries[text2];
						viewerEntry2.StandardPoints += viewerEntry.StandardPoints;
						viewerEntry2.SpecialPoints += viewerEntry.SpecialPoints;
						viewerEntry2.BitCredits += viewerEntry.BitCredits;
					}
					else
					{
						this.ViewerEntries.Add(text2, viewerEntry);
					}
					list.Add(text);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.ViewerEntries.Remove(list[i]);
			}
		}

		// Token: 0x0600A97E RID: 43390 RVA: 0x0042F0C4 File Offset: 0x0042D2C4
		public void ResetAllPoints()
		{
			foreach (string key in this.ViewerEntries.Keys)
			{
				this.ViewerEntries[key].SpecialPoints = 0f;
				this.ViewerEntries[key].StandardPoints = 0f;
			}
		}

		// Token: 0x0600A97F RID: 43391 RVA: 0x0042F144 File Offset: 0x0042D344
		public string GetPointTotals()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (string key in this.ViewerEntries.Keys)
			{
				ViewerEntry viewerEntry = this.ViewerEntries[key];
				num2 += (int)viewerEntry.SpecialPoints;
				num += (int)viewerEntry.StandardPoints;
				num3 += viewerEntry.BitCredits;
			}
			return string.Format("PP: {0} SP: {1} BC: {2}", num, num2, num3);
		}

		// Token: 0x0600A980 RID: 43392 RVA: 0x0042F1E8 File Offset: 0x0042D3E8
		[PublicizedFrom(EAccessModifier.Private)]
		public void ClearDisplayViewers()
		{
			this.ViewerEntries.Clear();
		}

		// Token: 0x0600A981 RID: 43393 RVA: 0x0042F1F8 File Offset: 0x0042D3F8
		[PublicizedFrom(EAccessModifier.Private)]
		public void IncrementViewerEntries()
		{
			float value = EffectManager.GetValue(PassiveEffects.TwitchViewerPointRate, null, this.PointRate, TwitchManager.Current.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			float num = value * 2f;
			float value2 = EffectManager.GetValue(PassiveEffects.TwitchViewerPointRate, null, this.NonSubPointCap, TwitchManager.Current.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			float value3 = EffectManager.GetValue(PassiveEffects.TwitchViewerPointRate, null, this.SubPointCap, TwitchManager.Current.LocalPlayer, null, default(FastTags<TagGroup.Global>), true, true, true, true, true, 1, true, false);
			bool allowPointGeneration = this.Owner.CurrentActionPreset.AllowPointGeneration;
			foreach (string key in this.ViewerEntries.Keys)
			{
				ViewerEntry viewerEntry = this.ViewerEntries[key];
				if (viewerEntry.IsActive)
				{
					this.Owner.HasDataChanges = true;
					if (allowPointGeneration)
					{
						if (viewerEntry.IsSub)
						{
							if (viewerEntry.StandardPoints < value3)
							{
								viewerEntry.StandardPoints += num;
								if (viewerEntry.StandardPoints > value3)
								{
									viewerEntry.StandardPoints = value3;
								}
							}
						}
						else if (viewerEntry.StandardPoints < value2)
						{
							viewerEntry.StandardPoints += value;
							if (viewerEntry.StandardPoints > value2)
							{
								viewerEntry.StandardPoints = value2;
							}
						}
					}
					if (viewerEntry.addPointsUntil < Time.time)
					{
						viewerEntry.IsActive = false;
					}
				}
			}
		}

		// Token: 0x0600A982 RID: 43394 RVA: 0x0042F39C File Offset: 0x0042D59C
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddToIDLookup(int viewerID, string viewerName, bool sendNewInChat = false)
		{
			if (this.IdToUsername.ContainsKey(viewerID))
			{
				this.IdToUsername[viewerID] = viewerName;
				return;
			}
			this.IdToUsername.Add(viewerID, viewerName);
			if (sendNewInChat && TwitchManager.Current.extensionManager != null)
			{
				TwitchManager.Current.extensionManager.PushViewerChatState(viewerID.ToString(), true);
			}
		}

		// Token: 0x0600A983 RID: 43395 RVA: 0x0042F3F8 File Offset: 0x0042D5F8
		public ViewerEntry UpdateViewerEntry(int viewerID, string name, string color, bool isSub)
		{
			this.AddToIDLookup(viewerID, name, true);
			if (this.ViewerEntries.ContainsKey(name))
			{
				ViewerEntry viewerEntry = this.ViewerEntries[name];
				viewerEntry.UserColor = color;
				viewerEntry.UserID = viewerID;
				viewerEntry.addPointsUntil = Time.time + TwitchViewerData.ChattingAddedTimeAmount;
				if (!viewerEntry.IsActive)
				{
					this.Owner.PushBalanceToExtensionQueue(viewerID.ToString(), viewerEntry.BitCredits);
				}
				viewerEntry.IsActive = true;
				viewerEntry.IsSub = isSub;
				return viewerEntry;
			}
			ViewerEntry viewerEntry2 = new ViewerEntry
			{
				UserColor = color,
				UserID = viewerID,
				StandardPoints = (float)this.StartingPoints,
				addPointsUntil = Time.time + TwitchViewerData.ChattingAddedTimeAmount,
				IsActive = true,
				IsSub = isSub
			};
			this.ViewerEntries.Add(name, viewerEntry2);
			return viewerEntry2;
		}

		// Token: 0x0600A984 RID: 43396 RVA: 0x0042F4C8 File Offset: 0x0042D6C8
		public bool HasViewerEntry(string name)
		{
			return this.ViewerEntries.ContainsKey(name);
		}

		// Token: 0x0600A985 RID: 43397 RVA: 0x0042F4D8 File Offset: 0x0042D6D8
		public ViewerEntry GetViewerEntry(string name)
		{
			if (this.ViewerEntries.ContainsKey(name))
			{
				return this.ViewerEntries[name];
			}
			ViewerEntry viewerEntry = new ViewerEntry
			{
				StandardPoints = 0f,
				addPointsUntil = 0f,
				IsActive = false,
				IsSub = false
			};
			this.ViewerEntries.Add(name, viewerEntry);
			return viewerEntry;
		}

		// Token: 0x0600A986 RID: 43398 RVA: 0x0042F538 File Offset: 0x0042D738
		public bool RemoveViewerEntry(string name)
		{
			if (this.ViewerEntries.ContainsKey(name))
			{
				this.ViewerEntries.Remove(name);
				return true;
			}
			return false;
		}

		// Token: 0x0600A987 RID: 43399 RVA: 0x0042F558 File Offset: 0x0042D758
		public ViewerEntry GetViewerEntry(string name, bool isSub)
		{
			if (this.ViewerEntries.ContainsKey(name))
			{
				return this.ViewerEntries[name];
			}
			ViewerEntry viewerEntry = new ViewerEntry
			{
				StandardPoints = (float)this.StartingPoints,
				addPointsUntil = 0f,
				IsActive = true,
				IsSub = isSub
			};
			this.ViewerEntries.Add(name, viewerEntry);
			return viewerEntry;
		}

		// Token: 0x0600A988 RID: 43400 RVA: 0x0042F5BC File Offset: 0x0042D7BC
		[PublicizedFrom(EAccessModifier.Private)]
		public ViewerEntry AddToViewerEntry(string name, int points, TwitchAction.PointTypes pointType)
		{
			if (name.StartsWith("@"))
			{
				name = name.Substring(1).ToLower();
			}
			else
			{
				name = name.ToLower();
			}
			if (this.ViewerEntries.ContainsKey(name))
			{
				ViewerEntry viewerEntry = this.ViewerEntries[name];
				switch (pointType)
				{
				case TwitchAction.PointTypes.PP:
					viewerEntry.StandardPoints += (float)points;
					if (viewerEntry.StandardPoints < 0f)
					{
						viewerEntry.StandardPoints = 0f;
					}
					break;
				case TwitchAction.PointTypes.SP:
					viewerEntry.SpecialPoints += (float)points;
					if (viewerEntry.SpecialPoints < 0f)
					{
						viewerEntry.SpecialPoints = 0f;
					}
					break;
				case TwitchAction.PointTypes.Bits:
					viewerEntry.BitCredits += points;
					if (viewerEntry.BitCredits < 0)
					{
						viewerEntry.BitCredits = 0;
					}
					this.Owner.PushBalanceToExtensionQueue(viewerEntry.UserID.ToString(), viewerEntry.BitCredits);
					break;
				}
				return this.ViewerEntries[name];
			}
			return null;
		}

		// Token: 0x0600A989 RID: 43401 RVA: 0x0042F6C0 File Offset: 0x0042D8C0
		public bool HasPointsForAction(string username, TwitchAction action)
		{
			ViewerEntry viewerEntry = this.ViewerEntries[username];
			return (action.SpecialOnly && viewerEntry.SpecialPoints >= (float)action.CurrentCost) || (!action.SpecialOnly && viewerEntry.CombinedPoints >= (float)action.CurrentCost);
		}

		// Token: 0x0600A98A RID: 43402 RVA: 0x0042F70C File Offset: 0x0042D90C
		public bool HandleInitialActionEntrySetup(string username, TwitchAction action, bool isRerun, bool isBitAction, out TwitchActionEntry actionEntry)
		{
			ViewerEntry viewerEntry = this.ViewerEntries[username];
			bool flag = isRerun || isBitAction;
			if ((flag || viewerEntry.LastAction == -1f || this.ActionSpamDelay == 0f || Time.time - viewerEntry.LastAction > this.ActionSpamDelay) && (flag || (action.SpecialOnly && viewerEntry.SpecialPoints >= (float)action.CurrentCost) || (!action.SpecialOnly && viewerEntry.CombinedPoints >= (float)action.CurrentCost)))
			{
				actionEntry = action.SetupActionEntry();
				actionEntry.UserName = username;
				if (!isRerun)
				{
					viewerEntry.RemovePoints((float)action.CurrentCost, action.PointType, actionEntry);
					if (username != this.Owner.Authentication.userName)
					{
						TwitchLeaderboardStats leaderboardStats = TwitchManager.LeaderboardStats;
						int num = (action.PointType == TwitchAction.PointTypes.Bits) ? 2 : 1;
						if (action.IsPositive)
						{
							leaderboardStats.TotalGood += num;
							leaderboardStats.CheckTopGood(leaderboardStats.AddGoodActionUsed(username, viewerEntry.UserColor, action.PointType == TwitchAction.PointTypes.Bits));
							QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.GoodAction, action.DisplayCategory.Name);
						}
						else
						{
							leaderboardStats.TotalBad += num;
							leaderboardStats.CheckTopBad(leaderboardStats.AddBadActionUsed(username, viewerEntry.UserColor, action.PointType == TwitchAction.PointTypes.Bits));
							QuestEventManager.Current.TwitchEventReceived(TwitchObjectiveTypes.BadAction, action.DisplayCategory.Name);
						}
						leaderboardStats.TotalActions += num;
					}
				}
				viewerEntry.LastAction = Time.time;
				return true;
			}
			actionEntry = null;
			return false;
		}

		// Token: 0x0600A98B RID: 43403 RVA: 0x0042F8A0 File Offset: 0x0042DAA0
		[PublicizedFrom(EAccessModifier.Internal)]
		public void ReimburseAction(TwitchActionEntry twitchActionEntry)
		{
			ViewerEntry viewerEntry = this.ViewerEntries[twitchActionEntry.UserName];
			viewerEntry.StandardPoints += (float)twitchActionEntry.StandardPointsUsed;
			viewerEntry.SpecialPoints += (float)twitchActionEntry.SpecialPointsUsed;
			viewerEntry.BitCredits += twitchActionEntry.BitsUsed;
			this.Owner.PushBalanceToExtensionQueue(viewerEntry.UserID.ToString(), viewerEntry.BitCredits);
		}

		// Token: 0x0600A98C RID: 43404 RVA: 0x0042F918 File Offset: 0x0042DB18
		public void ReimburseAction(string userName, int pointsSpent, TwitchAction action)
		{
			ViewerEntry viewerEntry = this.ViewerEntries[userName];
			TwitchAction.PointTypes pointType = action.PointType;
			if (pointType <= TwitchAction.PointTypes.SP)
			{
				viewerEntry.SpecialPoints += (float)pointsSpent;
				return;
			}
			if (pointType != TwitchAction.PointTypes.Bits)
			{
				return;
			}
			viewerEntry.BitCredits += pointsSpent;
		}

		// Token: 0x040083D8 RID: 33752
		public TwitchManager Owner;

		// Token: 0x040083D9 RID: 33753
		public static float ChattingAddedTimeAmount = 300f;

		// Token: 0x040083DA RID: 33754
		[PublicizedFrom(EAccessModifier.Private)]
		public float pointRate = 1f;

		// Token: 0x040083DB RID: 33755
		public float PointRateSubs = 2f;

		// Token: 0x040083DC RID: 33756
		public float NextActionTime;

		// Token: 0x040083DD RID: 33757
		public int StartingPoints = 100;

		// Token: 0x040083DE RID: 33758
		public float NonSubPointCap = 1000f;

		// Token: 0x040083DF RID: 33759
		public float SubPointCap = 2000f;

		// Token: 0x040083E0 RID: 33760
		public int SubPointAddTier1 = 500;

		// Token: 0x040083E1 RID: 33761
		public int SubPointAddTier2 = 1000;

		// Token: 0x040083E2 RID: 33762
		public int SubPointAddTier3 = 2500;

		// Token: 0x040083E3 RID: 33763
		public int GiftSubPointAddTier1 = 500;

		// Token: 0x040083E4 RID: 33764
		public int GiftSubPointAddTier2 = 1000;

		// Token: 0x040083E5 RID: 33765
		public int GiftSubPointAddTier3 = 2500;

		// Token: 0x040083E6 RID: 33766
		public float ActionSpamDelay = 3f;

		// Token: 0x040083E7 RID: 33767
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_GiftedSubs;

		// Token: 0x040083E8 RID: 33768
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_AddPPAll;

		// Token: 0x040083E9 RID: 33769
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_AddSPAll;

		// Token: 0x040083EA RID: 33770
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ErrorAddingBitCredits;

		// Token: 0x040083EB RID: 33771
		[PublicizedFrom(EAccessModifier.Private)]
		public string chatOutput_ErrorAddingPoints;

		// Token: 0x040083EC RID: 33772
		[PublicizedFrom(EAccessModifier.Private)]
		public string ingameOutput_GiftedSubs;

		// Token: 0x040083ED RID: 33773
		[PublicizedFrom(EAccessModifier.Private)]
		public Dictionary<string, ViewerEntry> ViewerEntries = new Dictionary<string, ViewerEntry>();

		// Token: 0x040083EE RID: 33774
		public Dictionary<int, string> IdToUsername = new Dictionary<int, string>();

		// Token: 0x040083EF RID: 33775
		public List<GiftSubEntry> SubEntries = new List<GiftSubEntry>();

		// Token: 0x040083F0 RID: 33776
		public static char[] UsernameExcludeCharacters = new char[]
		{
			';',
			'\\',
			':'
		};
	}
}

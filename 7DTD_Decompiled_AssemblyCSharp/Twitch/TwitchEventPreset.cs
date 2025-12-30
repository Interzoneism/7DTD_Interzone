using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Twitch
{
	// Token: 0x02001561 RID: 5473
	public class TwitchEventPreset
	{
		// Token: 0x170012A6 RID: 4774
		// (get) Token: 0x0600A815 RID: 43029 RVA: 0x0042312C File Offset: 0x0042132C
		public bool HasCustomEvents
		{
			get
			{
				return this.BitEvents.Count > 0 || this.SubEvents.Count > 0 || this.GiftSubEvents.Count > 0 || this.RaidEvents.Count > 0 || this.CharityEvents.Count > 0 || this.ChannelPointEvents.Count > 0 || this.HypeTrainEvents.Count > 0 || this.CreatorGoalEvents.Count > 0;
			}
		}

		// Token: 0x170012A7 RID: 4775
		// (get) Token: 0x0600A816 RID: 43030 RVA: 0x004231AB File Offset: 0x004213AB
		public bool HasBitEvents
		{
			get
			{
				return this.BitEvents.Count > 0;
			}
		}

		// Token: 0x170012A8 RID: 4776
		// (get) Token: 0x0600A817 RID: 43031 RVA: 0x004231BB File Offset: 0x004213BB
		public bool HasSubEvents
		{
			get
			{
				return this.SubEvents.Count > 0;
			}
		}

		// Token: 0x170012A9 RID: 4777
		// (get) Token: 0x0600A818 RID: 43032 RVA: 0x004231CB File Offset: 0x004213CB
		public bool HasGiftSubEvents
		{
			get
			{
				return this.GiftSubEvents.Count > 0;
			}
		}

		// Token: 0x170012AA RID: 4778
		// (get) Token: 0x0600A819 RID: 43033 RVA: 0x004231DB File Offset: 0x004213DB
		public bool HasRaidEvents
		{
			get
			{
				return this.RaidEvents.Count > 0;
			}
		}

		// Token: 0x170012AB RID: 4779
		// (get) Token: 0x0600A81A RID: 43034 RVA: 0x004231EB File Offset: 0x004213EB
		public bool HasCharityEvents
		{
			get
			{
				return this.CharityEvents.Count > 0;
			}
		}

		// Token: 0x170012AC RID: 4780
		// (get) Token: 0x0600A81B RID: 43035 RVA: 0x004231FB File Offset: 0x004213FB
		public bool HasChannelPointEvents
		{
			get
			{
				return this.ChannelPointEvents.Count > 0;
			}
		}

		// Token: 0x170012AD RID: 4781
		// (get) Token: 0x0600A81C RID: 43036 RVA: 0x0042320B File Offset: 0x0042140B
		public bool HasHypeTrainEvents
		{
			get
			{
				return this.HypeTrainEvents.Count > 0;
			}
		}

		// Token: 0x170012AE RID: 4782
		// (get) Token: 0x0600A81D RID: 43037 RVA: 0x0042321B File Offset: 0x0042141B
		public bool HasCreatorGoalEvents
		{
			get
			{
				return this.CreatorGoalEvents.Count > 0;
			}
		}

		// Token: 0x0600A81E RID: 43038 RVA: 0x0042322B File Offset: 0x0042142B
		public void AddBitEvent(TwitchEventEntry entry)
		{
			this.BitEvents.Add(entry);
		}

		// Token: 0x0600A81F RID: 43039 RVA: 0x00423239 File Offset: 0x00421439
		public void AddSubEvent(TwitchSubEventEntry entry)
		{
			this.SubEvents.Add(entry);
		}

		// Token: 0x0600A820 RID: 43040 RVA: 0x00423247 File Offset: 0x00421447
		public void AddGiftSubEvent(TwitchSubEventEntry entry)
		{
			this.GiftSubEvents.Add(entry);
		}

		// Token: 0x0600A821 RID: 43041 RVA: 0x00423255 File Offset: 0x00421455
		public void AddRaidEvent(TwitchEventEntry entry)
		{
			this.RaidEvents.Add(entry);
		}

		// Token: 0x0600A822 RID: 43042 RVA: 0x00423263 File Offset: 0x00421463
		public void AddCharityEvent(TwitchEventEntry entry)
		{
			this.CharityEvents.Add(entry);
		}

		// Token: 0x0600A823 RID: 43043 RVA: 0x00423271 File Offset: 0x00421471
		public void AddChannelPointEvent(TwitchChannelPointEventEntry entry)
		{
			this.ChannelPointEvents.Add(entry);
		}

		// Token: 0x0600A824 RID: 43044 RVA: 0x0042327F File Offset: 0x0042147F
		public void AddHypeTrainEvent(TwitchHypeTrainEventEntry entry)
		{
			this.HypeTrainEvents.Add(entry);
		}

		// Token: 0x0600A825 RID: 43045 RVA: 0x0042328D File Offset: 0x0042148D
		public void AddCreatorGoalEvent(TwitchCreatorGoalEventEntry entry)
		{
			this.CreatorGoalEvents.Add(entry);
		}

		// Token: 0x0600A826 RID: 43046 RVA: 0x0042329C File Offset: 0x0042149C
		public TwitchSubEventEntry HandleSubEvent(int months, TwitchSubEventEntry.SubTierTypes tier)
		{
			for (int i = 0; i < this.SubEvents.Count; i++)
			{
				if (this.SubEvents[i].IsValid(months, "", tier))
				{
					return this.SubEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A827 RID: 43047 RVA: 0x004232E8 File Offset: 0x004214E8
		public TwitchSubEventEntry HandleGiftSubEvent(int giftCounts, TwitchSubEventEntry.SubTierTypes tier)
		{
			for (int i = 0; i < this.GiftSubEvents.Count; i++)
			{
				if (this.GiftSubEvents[i].IsValid(giftCounts, "", tier))
				{
					return this.GiftSubEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A828 RID: 43048 RVA: 0x00423334 File Offset: 0x00421534
		public TwitchEventEntry HandleBitRedeem(int bitAmount)
		{
			for (int i = 0; i < this.BitEvents.Count; i++)
			{
				if (this.BitEvents[i].IsValid(bitAmount, "", TwitchSubEventEntry.SubTierTypes.Any))
				{
					return this.BitEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A829 RID: 43049 RVA: 0x00423380 File Offset: 0x00421580
		public TwitchChannelPointEventEntry HandleChannelPointsRedeem(string title)
		{
			for (int i = 0; i < this.ChannelPointEvents.Count; i++)
			{
				if (this.ChannelPointEvents[i].ChannelPointTitle == title)
				{
					return this.ChannelPointEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A82A RID: 43050 RVA: 0x004233CC File Offset: 0x004215CC
		public TwitchEventEntry HandleRaid(int viewerAmount)
		{
			for (int i = 0; i < this.RaidEvents.Count; i++)
			{
				if (this.RaidEvents[i].IsValid(viewerAmount, "", TwitchSubEventEntry.SubTierTypes.Any))
				{
					return this.RaidEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A82B RID: 43051 RVA: 0x00423418 File Offset: 0x00421618
		public TwitchEventEntry HandleCharityRedeem(int charityAmount)
		{
			for (int i = 0; i < this.CharityEvents.Count; i++)
			{
				if (this.CharityEvents[i].IsValid(charityAmount, "", TwitchSubEventEntry.SubTierTypes.Any))
				{
					return this.CharityEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A82C RID: 43052 RVA: 0x00423464 File Offset: 0x00421664
		public TwitchEventEntry HandleHypeTrainRedeem(int hypeTrainLevel)
		{
			for (int i = 0; i < this.HypeTrainEvents.Count; i++)
			{
				if (this.HypeTrainEvents[i].IsValid(hypeTrainLevel, "", TwitchSubEventEntry.SubTierTypes.Any))
				{
					return this.HypeTrainEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A82D RID: 43053 RVA: 0x004234B0 File Offset: 0x004216B0
		public TwitchCreatorGoalEventEntry HandleCreatorGoalEvent(string goalType)
		{
			for (int i = 0; i < this.HypeTrainEvents.Count; i++)
			{
				if (this.CreatorGoalEvents[i].IsValid(-1, goalType, TwitchSubEventEntry.SubTierTypes.Any))
				{
					return this.CreatorGoalEvents[i];
				}
			}
			return null;
		}

		// Token: 0x0600A82E RID: 43054 RVA: 0x004234F8 File Offset: 0x004216F8
		public void AddChannelPointRedemptions()
		{
			if (TwitchManager.Current.Authentication != null)
			{
				string userID = TwitchManager.Current.Authentication.userID;
				for (int i = 0; i < this.ChannelPointEvents.Count; i++)
				{
					if (this.ChannelPointEvents[i].ChannelPointID == "" && this.ChannelPointEvents[i].AutoCreate)
					{
						GameManager.Instance.StartCoroutine(TwitchChannelPointEventEntry.CreateCustomRewardPost(this.ChannelPointEvents[i].SetupRewardEntry(userID), delegate(string res)
						{
							TwitchChannelPointEventEntry.CreateCustomRewardResponses createCustomRewardResponses = JsonConvert.DeserializeObject<TwitchChannelPointEventEntry.CreateCustomRewardResponses>(res);
							for (int j = 0; j < this.ChannelPointEvents.Count; j++)
							{
								if (this.ChannelPointEvents[j].ChannelPointTitle == createCustomRewardResponses.data[0].title)
								{
									this.ChannelPointEvents[j].ChannelPointID = createCustomRewardResponses.data[0].id;
									return;
								}
							}
						}, delegate(string err)
						{
							Log.Out(err);
						}));
					}
				}
				this.ChannelPointsSetup = true;
			}
		}

		// Token: 0x0600A82F RID: 43055 RVA: 0x004235C4 File Offset: 0x004217C4
		public void RemoveChannelPointRedemptions(TwitchEventPreset newPreset = null)
		{
			for (int i = 0; i < this.ChannelPointEvents.Count; i++)
			{
				TwitchChannelPointEventEntry twitchChannelPointEventEntry = this.ChannelPointEvents[i];
				if (!(twitchChannelPointEventEntry.ChannelPointID == "") && this.ChannelPointEvents[i].AutoCreate && (newPreset == null || !newPreset.ChannelPointEvents.Contains(twitchChannelPointEventEntry)))
				{
					GameManager.Instance.StartCoroutine(TwitchChannelPointEventEntry.DeleteCustomRewardsDelete(this.ChannelPointEvents[i].ChannelPointID, delegate(string res)
					{
					}, delegate(string err)
					{
						Debug.LogWarning("Remove Channel Point Redeem Failed: " + err);
					}));
					this.ChannelPointEvents[i].ChannelPointID = "";
				}
			}
			this.ChannelPointsSetup = false;
		}

		// Token: 0x0400826E RID: 33390
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchEventEntry> BitEvents = new List<TwitchEventEntry>();

		// Token: 0x0400826F RID: 33391
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSubEventEntry> SubEvents = new List<TwitchSubEventEntry>();

		// Token: 0x04008270 RID: 33392
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchSubEventEntry> GiftSubEvents = new List<TwitchSubEventEntry>();

		// Token: 0x04008271 RID: 33393
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchEventEntry> RaidEvents = new List<TwitchEventEntry>();

		// Token: 0x04008272 RID: 33394
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchEventEntry> CharityEvents = new List<TwitchEventEntry>();

		// Token: 0x04008273 RID: 33395
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchChannelPointEventEntry> ChannelPointEvents = new List<TwitchChannelPointEventEntry>();

		// Token: 0x04008274 RID: 33396
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchHypeTrainEventEntry> HypeTrainEvents = new List<TwitchHypeTrainEventEntry>();

		// Token: 0x04008275 RID: 33397
		[PublicizedFrom(EAccessModifier.Private)]
		public List<TwitchCreatorGoalEventEntry> CreatorGoalEvents = new List<TwitchCreatorGoalEventEntry>();

		// Token: 0x04008276 RID: 33398
		public string Name;

		// Token: 0x04008277 RID: 33399
		public bool IsDefault;

		// Token: 0x04008278 RID: 33400
		public bool IsEmpty;

		// Token: 0x04008279 RID: 33401
		public bool ChannelPointsSetup;

		// Token: 0x0400827A RID: 33402
		public string Title;

		// Token: 0x0400827B RID: 33403
		public string Description;
	}
}

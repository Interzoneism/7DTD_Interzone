using System;

namespace Twitch
{
	// Token: 0x02001554 RID: 5460
	public class BaseTwitchEventEntry
	{
		// Token: 0x0600A7F1 RID: 42993 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool IsValid(int amount = -1, string name = "", TwitchSubEventEntry.SubTierTypes subTier = TwitchSubEventEntry.SubTierTypes.Any)
		{
			return false;
		}

		// Token: 0x0600A7F2 RID: 42994 RVA: 0x004228C6 File Offset: 0x00420AC6
		public virtual string Description(TwitchEventActionEntry entry)
		{
			return string.Format("{0}({1})", this.EventTitle, entry.Count);
		}

		// Token: 0x0600A7F3 RID: 42995 RVA: 0x004228E4 File Offset: 0x00420AE4
		public virtual void HandleInstant(string username, TwitchManager tm)
		{
			if (this.PimpPotAdd > 0)
			{
				tm.AddToPot(this.PimpPotAdd);
			}
			if (this.BitPotAdd > 0)
			{
				tm.AddToBitPot(this.BitPotAdd);
			}
			if (this.CooldownAdd > 0)
			{
				tm.AddCooldownAmount(this.CooldownAdd);
			}
			if (this.PPAmount > 0 || this.SPAmount > 0)
			{
				if (username == "")
				{
					tm.ViewerData.AddPointsAll(this.PPAmount, this.SPAmount, true);
					return;
				}
				ViewerEntry viewerEntry = tm.ViewerData.GetViewerEntry(username);
				viewerEntry.SpecialPoints += (float)this.SPAmount;
				viewerEntry.StandardPoints += (float)this.PPAmount;
			}
		}

		// Token: 0x0600A7F4 RID: 42996 RVA: 0x0042299C File Offset: 0x00420B9C
		public virtual bool HandleEvent(string username, TwitchManager tm)
		{
			if (this.EventName == "")
			{
				return true;
			}
			if (!this.SafeAllowed && tm.IsSafe)
			{
				return false;
			}
			if (TwitchManager.BossHordeActive)
			{
				return false;
			}
			TwitchManager.CooldownTypes cooldownType = tm.CooldownType;
			if (cooldownType != TwitchManager.CooldownTypes.None)
			{
				if (cooldownType == TwitchManager.CooldownTypes.Startup)
				{
					if (!this.StartingCooldownAllowed)
					{
						return false;
					}
				}
				else if (!this.CooldownAllowed)
				{
					return false;
				}
			}
			if (!this.VoteEventAllowed && tm.VotingManager.VotingIsActive)
			{
				return false;
			}
			if (GameEventManager.Current.HandleAction(this.EventName, tm.LocalPlayer, tm.LocalPlayer, false, username, "event", tm.AllowCrateSharing, false, "", null))
			{
				GameEventManager.Current.HandleGameEventApproved(this.EventName, tm.LocalPlayer.entityId, username, "event");
				return true;
			}
			return false;
		}

		// Token: 0x0400822E RID: 33326
		public string EventName = "";

		// Token: 0x0400822F RID: 33327
		public string EventTitle = "";

		// Token: 0x04008230 RID: 33328
		public bool SafeAllowed = true;

		// Token: 0x04008231 RID: 33329
		public bool StartingCooldownAllowed;

		// Token: 0x04008232 RID: 33330
		public bool CooldownAllowed = true;

		// Token: 0x04008233 RID: 33331
		public bool VoteEventAllowed = true;

		// Token: 0x04008234 RID: 33332
		public bool RewardsBitPot;

		// Token: 0x04008235 RID: 33333
		public int PPAmount;

		// Token: 0x04008236 RID: 33334
		public int SPAmount;

		// Token: 0x04008237 RID: 33335
		public int PimpPotAdd;

		// Token: 0x04008238 RID: 33336
		public int BitPotAdd;

		// Token: 0x04008239 RID: 33337
		public int CooldownAdd;

		// Token: 0x0400823A RID: 33338
		public BaseTwitchEventEntry.EventTypes EventType;

		// Token: 0x02001555 RID: 5461
		public enum EventTypes
		{
			// Token: 0x0400823C RID: 33340
			Bits,
			// Token: 0x0400823D RID: 33341
			Subs,
			// Token: 0x0400823E RID: 33342
			GiftSubs,
			// Token: 0x0400823F RID: 33343
			Raid,
			// Token: 0x04008240 RID: 33344
			Charity,
			// Token: 0x04008241 RID: 33345
			ChannelPoints,
			// Token: 0x04008242 RID: 33346
			HypeTrain,
			// Token: 0x04008243 RID: 33347
			CreatorGoal
		}
	}
}

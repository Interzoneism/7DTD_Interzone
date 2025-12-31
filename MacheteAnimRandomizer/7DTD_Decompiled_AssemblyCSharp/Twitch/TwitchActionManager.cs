using System;
using System.Collections.Generic;

namespace Twitch
{
	// Token: 0x0200151B RID: 5403
	public class TwitchActionManager
	{
		// Token: 0x17001244 RID: 4676
		// (get) Token: 0x0600A6CE RID: 42702 RVA: 0x0041DB16 File Offset: 0x0041BD16
		public static TwitchActionManager Current
		{
			get
			{
				if (TwitchActionManager.instance == null)
				{
					TwitchActionManager.instance = new TwitchActionManager();
				}
				return TwitchActionManager.instance;
			}
		}

		// Token: 0x17001245 RID: 4677
		// (get) Token: 0x0600A6CF RID: 42703 RVA: 0x0041DB2E File Offset: 0x0041BD2E
		public static bool HasInstance
		{
			get
			{
				return TwitchActionManager.instance != null;
			}
		}

		// Token: 0x0600A6D0 RID: 42704 RVA: 0x0041DB38 File Offset: 0x0041BD38
		[PublicizedFrom(EAccessModifier.Private)]
		public TwitchActionManager()
		{
		}

		// Token: 0x0600A6D1 RID: 42705 RVA: 0x0041DB4B File Offset: 0x0041BD4B
		public void Cleanup()
		{
			if (TwitchActionManager.TwitchActions != null)
			{
				TwitchActionManager.TwitchActions.Clear();
			}
			if (TwitchActionManager.TwitchVotes != null)
			{
				TwitchActionManager.TwitchVotes.Clear();
			}
			this.CategoryList.Clear();
			if (TwitchManager.HasInstance)
			{
				TwitchManager.Current.CleanupData();
			}
		}

		// Token: 0x0600A6D2 RID: 42706 RVA: 0x0041DB8B File Offset: 0x0041BD8B
		public void AddAction(TwitchAction action)
		{
			if (!TwitchActionManager.TwitchActions.ContainsKey(action.Name))
			{
				TwitchActionManager.TwitchActions.Add(action.Name, action);
			}
		}

		// Token: 0x0600A6D3 RID: 42707 RVA: 0x0041DBB0 File Offset: 0x0041BDB0
		public void AddVoteClass(TwitchVote vote)
		{
			TwitchActionManager.TwitchVotes.Add(vote.VoteName, vote);
		}

		// Token: 0x0600A6D4 RID: 42708 RVA: 0x0041DBC4 File Offset: 0x0041BDC4
		public int GetCategoryIndex(string categoryName)
		{
			for (int i = 0; i < this.CategoryList.Count; i++)
			{
				if (categoryName.StartsWith(this.CategoryList[i].Name))
				{
					return i;
				}
			}
			return 9999;
		}

		// Token: 0x0600A6D5 RID: 42709 RVA: 0x0041DC08 File Offset: 0x0041BE08
		public TwitchActionManager.ActionCategory GetCategory(string categoryName)
		{
			for (int i = 0; i < this.CategoryList.Count; i++)
			{
				if (this.CategoryList[i].Name == categoryName)
				{
					return this.CategoryList[i];
				}
			}
			return null;
		}

		// Token: 0x0400811E RID: 33054
		[PublicizedFrom(EAccessModifier.Private)]
		public static TwitchActionManager instance = null;

		// Token: 0x0400811F RID: 33055
		public List<TwitchActionManager.ActionCategory> CategoryList = new List<TwitchActionManager.ActionCategory>();

		// Token: 0x04008120 RID: 33056
		public static Dictionary<string, TwitchAction> TwitchActions = new Dictionary<string, TwitchAction>();

		// Token: 0x04008121 RID: 33057
		public static Dictionary<string, TwitchVote> TwitchVotes = new Dictionary<string, TwitchVote>();

		// Token: 0x0200151C RID: 5404
		public class ActionCategory
		{
			// Token: 0x04008122 RID: 33058
			public string Name;

			// Token: 0x04008123 RID: 33059
			public string DisplayName;

			// Token: 0x04008124 RID: 33060
			public string Icon;

			// Token: 0x04008125 RID: 33061
			public bool ShowInCommandList = true;

			// Token: 0x04008126 RID: 33062
			public bool AlwaysShowInMenu;
		}
	}
}

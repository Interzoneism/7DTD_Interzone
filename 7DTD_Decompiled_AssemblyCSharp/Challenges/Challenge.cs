using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015D9 RID: 5593
	[Preserve]
	public class Challenge
	{
		// Token: 0x17001331 RID: 4913
		// (get) Token: 0x0600ABAC RID: 43948 RVA: 0x00437A16 File Offset: 0x00435C16
		// (set) Token: 0x0600ABAD RID: 43949 RVA: 0x00437A1E File Offset: 0x00435C1E
		public byte CurrentFileVersion { get; set; }

		// Token: 0x17001332 RID: 4914
		// (get) Token: 0x0600ABAE RID: 43950 RVA: 0x00437A27 File Offset: 0x00435C27
		public bool IsActive
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Active;
			}
		}

		// Token: 0x14000108 RID: 264
		// (add) Token: 0x0600ABAF RID: 43951 RVA: 0x00437A34 File Offset: 0x00435C34
		// (remove) Token: 0x0600ABB0 RID: 43952 RVA: 0x00437A6C File Offset: 0x00435C6C
		public event ChallengeStateChanged OnChallengeStateChanged;

		// Token: 0x17001333 RID: 4915
		// (get) Token: 0x0600ABB1 RID: 43953 RVA: 0x00437AA1 File Offset: 0x00435CA1
		// (set) Token: 0x0600ABB2 RID: 43954 RVA: 0x00437AA9 File Offset: 0x00435CA9
		public bool NeedsPreRequisites
		{
			get
			{
				return this.needsPrerequisites;
			}
			set
			{
				this.needsPrerequisites = value;
				if (this.OnChallengeStateChanged != null)
				{
					this.OnChallengeStateChanged(this);
				}
			}
		}

		// Token: 0x0600ABB3 RID: 43955 RVA: 0x00437AC6 File Offset: 0x00435CC6
		public void SetRequirementGroup(BaseRequirementObjectiveGroup requirementObjectiveGroup)
		{
			requirementObjectiveGroup.Owner = this;
			this.RequirementObjectiveGroup = requirementObjectiveGroup;
		}

		// Token: 0x17001334 RID: 4916
		// (get) Token: 0x0600ABB4 RID: 43956 RVA: 0x00437AD6 File Offset: 0x00435CD6
		public bool ReadyToComplete
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Completed || (this.ChallengeClass.RedeemAlways && this.ChallengeState == Challenge.ChallengeStates.Active);
			}
		}

		// Token: 0x17001335 RID: 4917
		// (get) Token: 0x0600ABB5 RID: 43957 RVA: 0x00437AFC File Offset: 0x00435CFC
		public float FillAmount
		{
			get
			{
				float num = 0f;
				for (int i = 0; i < this.ObjectiveList.Count; i++)
				{
					num += this.ObjectiveList[i].FillAmount;
				}
				return num / (float)this.ObjectiveList.Count;
			}
		}

		// Token: 0x17001336 RID: 4918
		// (get) Token: 0x0600ABB6 RID: 43958 RVA: 0x00437B47 File Offset: 0x00435D47
		public int ActiveObjectives
		{
			get
			{
				if (!this.NeedsPreRequisites)
				{
					return this.ObjectiveList.Count;
				}
				return this.RequirementObjectiveGroup.Count;
			}
		}

		// Token: 0x17001337 RID: 4919
		// (get) Token: 0x0600ABB7 RID: 43959 RVA: 0x00437B68 File Offset: 0x00435D68
		public bool NeedsUIUpdate
		{
			get
			{
				return this.ChallengeState == Challenge.ChallengeStates.Active && this.ChallengeClass.NeedsConstantUIUpdate;
			}
		}

		// Token: 0x0600ABB8 RID: 43960 RVA: 0x00437B80 File Offset: 0x00435D80
		public virtual void Read(BinaryReader _br)
		{
			this.CurrentFileVersion = _br.ReadByte();
			string key = _br.ReadString();
			this.ChallengeState = (Challenge.ChallengeStates)_br.ReadByte();
			byte currentVersion = _br.ReadByte();
			int num = _br.ReadInt32();
			for (int i = 0; i < num; i++)
			{
				ChallengeObjectiveType type = (ChallengeObjectiveType)_br.ReadByte();
				this.ObjectiveList.Add(BaseChallengeObjective.ReadObjective(currentVersion, type, _br));
			}
			if (ChallengeClass.s_Challenges.ContainsKey(key))
			{
				this.ChallengeClass = ChallengeClass.s_Challenges[key];
				this.ChallengeGroup = this.ChallengeClass.ChallengeGroup;
			}
		}

		// Token: 0x0600ABB9 RID: 43961 RVA: 0x00437C14 File Offset: 0x00435E14
		public ChallengeObjectiveChallengeComplete GetChallengeCompleteObjective()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				ChallengeObjectiveChallengeComplete challengeObjectiveChallengeComplete = this.ObjectiveList[i] as ChallengeObjectiveChallengeComplete;
				if (challengeObjectiveChallengeComplete != null)
				{
					return challengeObjectiveChallengeComplete;
				}
			}
			return null;
		}

		// Token: 0x0600ABBA RID: 43962 RVA: 0x00437C4F File Offset: 0x00435E4F
		[PublicizedFrom(EAccessModifier.Internal)]
		public Recipe GetRecipeFromRequirements()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				return this.RequirementObjectiveGroup.GetItemRecipe();
			}
			return null;
		}

		// Token: 0x0600ABBB RID: 43963 RVA: 0x00437C68 File Offset: 0x00435E68
		public virtual void Write(BinaryWriter _bw)
		{
			_bw.Write(Challenge.FileVersion);
			_bw.Write(this.ChallengeClass.Name);
			_bw.Write((byte)this.ChallengeState);
			_bw.Write(BaseChallengeObjective.FileVersion);
			_bw.Write(this.ObjectiveList.Count);
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].WriteObjective(_bw);
			}
		}

		// Token: 0x0600ABBC RID: 43964 RVA: 0x00437CE1 File Offset: 0x00435EE1
		public bool ResetToChallengeClass()
		{
			return this.ChallengeClass != null && this.ChallengeClass.ResetObjectives(this);
		}

		// Token: 0x0600ABBD RID: 43965 RVA: 0x00437CF9 File Offset: 0x00435EF9
		public List<BaseChallengeObjective> GetObjectiveList()
		{
			if (!this.NeedsPreRequisites)
			{
				return this.ObjectiveList;
			}
			return this.RequirementObjectiveGroup.CurrentObjectiveList;
		}

		// Token: 0x0600ABBE RID: 43966 RVA: 0x00437D18 File Offset: 0x00435F18
		public List<Recipe> CraftedRecipes()
		{
			List<Recipe> list = null;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					Recipe[] recipeItems = this.ObjectiveList[i].GetRecipeItems();
					if (recipeItems != null)
					{
						if (list == null)
						{
							list = new List<Recipe>();
						}
						list.AddRange(recipeItems);
					}
				}
			}
			return list;
		}

		// Token: 0x0600ABBF RID: 43967 RVA: 0x00437D78 File Offset: 0x00435F78
		public void StartChallenge()
		{
			if (this.IsActive)
			{
				for (int i = 0; i < this.ObjectiveList.Count; i++)
				{
					this.ObjectiveList[i].HandleAddHooks();
				}
			}
		}

		// Token: 0x0600ABC0 RID: 43968 RVA: 0x00437DB4 File Offset: 0x00435FB4
		public void EndChallenge(bool isCompleted)
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].HandleRemoveHooks();
			}
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleRemoveHooks();
			}
			if (isCompleted)
			{
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Player);
				if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this)
				{
					uiforPlayer.xui.QuestTracker.TrackedChallenge = this.Owner.GetNextChallenge(this);
				}
			}
		}

		// Token: 0x0600ABC1 RID: 43969 RVA: 0x00437E40 File Offset: 0x00436040
		public void HandleComplete(bool showTooltip = true)
		{
			bool flag = false;
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				if (this.IsTracked)
				{
					this.CheckPrerequisites();
				}
				return;
			}
			if (this.ChallengeState == Challenge.ChallengeStates.Active)
			{
				this.ChallengeState = Challenge.ChallengeStates.Completed;
			}
			this.EndChallenge(true);
			QuestEventManager.Current.ChallengeCompleted(this.ChallengeClass, false);
			if (this.ChallengeClass.ChallengeGroup.IsVisible(this.Owner.Player) && showTooltip)
			{
				GameManager.ShowTooltip(this.Owner.Player, string.Format(Localization.Get("challengeMessageComplete", false), this.ChallengeClass.Title), "", "ui_challenge_complete", null, false, false, 0f);
			}
		}

		// Token: 0x0600ABC2 RID: 43970 RVA: 0x00437F14 File Offset: 0x00436114
		public void Redeem()
		{
			GameEventManager.Current.HandleAction(this.ChallengeClass.RewardEvent, null, this.Owner.Player, false, "", "", false, true, "", null);
			this.Owner.HandleChallengeRedeemed(this);
			this.Owner.HandleChallengeGroupComplete(this.ChallengeGroup);
		}

		// Token: 0x0600ABC3 RID: 43971 RVA: 0x00437F74 File Offset: 0x00436174
		public void HandleTrackingStarted()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				if (!this.ObjectiveList[i].Complete)
				{
					this.ObjectiveList[i].HandleTrackingStarted();
				}
			}
		}

		// Token: 0x0600ABC4 RID: 43972 RVA: 0x00437FBC File Offset: 0x004361BC
		public void HandleTrackingEnded()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				this.ObjectiveList[i].HandleTrackingEnded();
			}
		}

		// Token: 0x0600ABC5 RID: 43973 RVA: 0x00437FF0 File Offset: 0x004361F0
		public ChallengeTrackingHandler GetTrackingHelper()
		{
			if (this.TrackingHandler == null)
			{
				this.TrackingHandler = new ChallengeTrackingHandler
				{
					Owner = this,
					LocalPlayer = this.Owner.Player
				};
			}
			return this.TrackingHandler;
		}

		// Token: 0x0600ABC6 RID: 43974 RVA: 0x00438023 File Offset: 0x00436223
		public void AddTrackingEntry(TrackingEntry entry)
		{
			if (this.TrackingHandler == null)
			{
				this.TrackingHandler = new ChallengeTrackingHandler
				{
					Owner = this,
					LocalPlayer = this.Owner.Player
				};
			}
			this.TrackingHandler.AddTrackingEntry(entry);
		}

		// Token: 0x0600ABC7 RID: 43975 RVA: 0x0043805C File Offset: 0x0043625C
		public void RemoveTrackingEntry(TrackingEntry entry)
		{
			if (this.TrackingHandler == null)
			{
				return;
			}
			this.TrackingHandler.RemoveTrackingEntry(entry);
		}

		// Token: 0x0600ABC8 RID: 43976 RVA: 0x00438074 File Offset: 0x00436274
		public Challenge Clone()
		{
			Challenge challenge = new Challenge();
			challenge.ChallengeClass = this.ChallengeClass;
			challenge.ChallengeState = this.ChallengeState;
			challenge.IsTracked = this.IsTracked;
			if (this.RequirementObjectiveGroup != null)
			{
				BaseRequirementObjectiveGroup requirementObjectiveGroup = this.RequirementObjectiveGroup;
				BaseRequirementObjectiveGroup baseRequirementObjectiveGroup = requirementObjectiveGroup.Clone();
				baseRequirementObjectiveGroup.Owner = this;
				baseRequirementObjectiveGroup.ClonePhases(requirementObjectiveGroup);
				challenge.RequirementObjectiveGroup = baseRequirementObjectiveGroup;
			}
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i].Clone();
				baseChallengeObjective.CopyValues(this.ObjectiveList[i], this.ChallengeClass.ObjectiveList[i]);
				baseChallengeObjective.Owner = challenge;
				challenge.ObjectiveList.Add(baseChallengeObjective);
			}
			return challenge;
		}

		// Token: 0x0600ABC9 RID: 43977 RVA: 0x00438139 File Offset: 0x00436339
		public void RemovePrerequisiteHooks()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleRemoveHooks();
			}
		}

		// Token: 0x0600ABCA RID: 43978 RVA: 0x00438150 File Offset: 0x00436350
		public void CheckPrerequisites()
		{
			bool flag = false;
			if (this.RequirementObjectiveGroup != null && this.RequirementObjectiveGroup.HasPrerequisiteCondition())
			{
				if (this.RequirementObjectiveGroup.HandleCheckStatus())
				{
					flag = true;
					this.UIDirty = true;
				}
				this.RequirementObjectiveGroup.UpdateStatus();
			}
			if (this.NeedsPreRequisites != flag)
			{
				this.NeedsPreRequisites = flag;
				LocalPlayerUI uiforPlayer = LocalPlayerUI.GetUIForPlayer(this.Owner.Player);
				if (uiforPlayer.xui.QuestTracker.TrackedChallenge == this)
				{
					uiforPlayer.xui.QuestTracker.HandleTrackedChallengeChanged();
				}
			}
		}

		// Token: 0x0600ABCB RID: 43979 RVA: 0x004381D9 File Offset: 0x004363D9
		public void AddPrerequisiteHooks()
		{
			if (this.RequirementObjectiveGroup != null)
			{
				this.RequirementObjectiveGroup.HandleAddHooks();
			}
			this.CheckPrerequisites();
		}

		// Token: 0x0600ABCC RID: 43980 RVA: 0x004381F4 File Offset: 0x004363F4
		public BaseChallengeObjective GetNavObjective()
		{
			for (int i = 0; i < this.ObjectiveList.Count; i++)
			{
				BaseChallengeObjective baseChallengeObjective = this.ObjectiveList[i];
				if (baseChallengeObjective.NavType != ChallengeClass.UINavTypes.None)
				{
					return baseChallengeObjective;
				}
			}
			return null;
		}

		// Token: 0x0600ABCD RID: 43981 RVA: 0x00438230 File Offset: 0x00436430
		public void CompleteChallenge(bool forceRedeem = false)
		{
			if (!this.IsActive && (!forceRedeem || this.ChallengeState != Challenge.ChallengeStates.Completed))
			{
				return;
			}
			foreach (BaseChallengeObjective baseChallengeObjective in this.ObjectiveList)
			{
				baseChallengeObjective.CompleteObjective(false);
			}
			this.HandleComplete(false);
			if (!forceRedeem)
			{
				return;
			}
			this.ChallengeState = Challenge.ChallengeStates.Redeemed;
			this.Redeem();
			QuestEventManager.Current.ChallengeCompleted(this.ChallengeClass, true);
		}

		// Token: 0x040085D2 RID: 34258
		public static byte FileVersion = 1;

		// Token: 0x040085D4 RID: 34260
		public Challenge.ChallengeStates ChallengeState;

		// Token: 0x040085D5 RID: 34261
		public bool IsTracked;

		// Token: 0x040085D6 RID: 34262
		public bool UIDirty;

		// Token: 0x040085D7 RID: 34263
		public ChallengeClass ChallengeClass;

		// Token: 0x040085D8 RID: 34264
		public ChallengeGroup ChallengeGroup;

		// Token: 0x040085D9 RID: 34265
		public BaseRequirementObjectiveGroup RequirementObjectiveGroup;

		// Token: 0x040085DA RID: 34266
		public List<BaseChallengeObjective> ObjectiveList = new List<BaseChallengeObjective>();

		// Token: 0x040085DC RID: 34268
		public ChallengeTrackingHandler TrackingHandler;

		// Token: 0x040085DD RID: 34269
		[PublicizedFrom(EAccessModifier.Private)]
		public bool needsPrerequisites;

		// Token: 0x040085DE RID: 34270
		public ChallengeJournal Owner;

		// Token: 0x020015DA RID: 5594
		public enum ChallengeStates : byte
		{
			// Token: 0x040085E0 RID: 34272
			Active,
			// Token: 0x040085E1 RID: 34273
			Completed,
			// Token: 0x040085E2 RID: 34274
			Redeemed
		}
	}
}

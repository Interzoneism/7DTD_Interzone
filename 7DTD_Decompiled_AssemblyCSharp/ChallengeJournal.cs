using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using Challenges;
using UniLinq;
using UnityEngine;

// Token: 0x02000570 RID: 1392
public class ChallengeJournal
{
	// Token: 0x06002D02 RID: 11522 RVA: 0x0012D114 File Offset: 0x0012B314
	public void Read(BinaryReader _br)
	{
		this.SetupData();
		if (_br.ReadByte() == 1)
		{
			return;
		}
		int num = _br.ReadInt32();
		this.Challenges.Clear();
		this.ChallengeDictionary.Clear();
		this.CompleteChallengesForMinEvents.Clear();
		for (int i = 0; i < num; i++)
		{
			Challenge challenge = new Challenge();
			challenge.Owner = this;
			challenge.Read(_br);
			if (challenge.ResetToChallengeClass())
			{
				if (challenge.ChallengeState == Challenge.ChallengeStates.Redeemed && this.eventList.ContainsKey(challenge.ChallengeClass.Name))
				{
					this.CompleteChallengesForMinEvents.Add(challenge);
				}
				this.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
			}
		}
		this.Challenges = (from c in this.ChallengeDictionary.Values
		orderby c.ChallengeClass.OrderIndex
		select c).ToList<Challenge>();
		if (this.ChallengeGroups.Count == 0 && !GameManager.Instance.World.IsEditor() && !GameUtils.IsWorldEditor() && !GameUtils.IsPlaytesting())
		{
			foreach (ChallengeGroup group in ChallengeGroup.s_ChallengeGroups.Values)
			{
				ChallengeGroupEntry item = new ChallengeGroupEntry(group);
				this.ChallengeGroups.Add(item);
			}
		}
		num = _br.ReadInt32();
		for (int j = 0; j < num; j++)
		{
			string b = _br.ReadString();
			int lastUpdateDay = _br.ReadInt32();
			for (int k = 0; k < this.ChallengeGroups.Count; k++)
			{
				if (this.ChallengeGroups[k].ChallengeGroup.Name == b)
				{
					this.ChallengeGroups[k].LastUpdateDay = lastUpdateDay;
				}
			}
		}
		string text = _br.ReadString();
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			this.ChallengeDictionary[text].IsTracked = true;
		}
	}

	// Token: 0x06002D03 RID: 11523 RVA: 0x0012D32C File Offset: 0x0012B52C
	public void Write(BinaryWriter _bw)
	{
		string value = "";
		_bw.Write(2);
		_bw.Write(this.Challenges.Count);
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			challenge.Write(_bw);
			if (challenge.IsTracked)
			{
				value = challenge.ChallengeClass.Name;
			}
		}
		int num = 0;
		for (int j = 0; j < this.ChallengeGroups.Count; j++)
		{
			if (this.ChallengeGroups[j].LastUpdateDay != -1)
			{
				num++;
			}
		}
		_bw.Write(num);
		for (int k = 0; k < this.ChallengeGroups.Count; k++)
		{
			if (this.ChallengeGroups[k].LastUpdateDay != -1)
			{
				_bw.Write(this.ChallengeGroups[k].ChallengeGroup.Name);
				_bw.Write(this.ChallengeGroups[k].LastUpdateDay);
			}
		}
		_bw.Write(value);
	}

	// Token: 0x06002D04 RID: 11524 RVA: 0x0012D43C File Offset: 0x0012B63C
	public ChallengeJournal Clone()
	{
		ChallengeJournal challengeJournal = new ChallengeJournal();
		challengeJournal.Player = this.Player;
		for (int i = 0; i < this.ChallengeGroups.Count; i++)
		{
			challengeJournal.ChallengeGroups.Add(this.ChallengeGroups[i]);
		}
		for (int j = 0; j < this.Challenges.Count; j++)
		{
			Challenge challenge = this.Challenges[j].Clone();
			challengeJournal.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
			challengeJournal.Challenges.Add(challenge);
		}
		return challengeJournal;
	}

	// Token: 0x06002D05 RID: 11525 RVA: 0x0012D4D4 File Offset: 0x0012B6D4
	public void Update(World world)
	{
		int num = GameUtils.WorldTimeToDays(world.worldTime);
		if (this.lastDay < num)
		{
			for (int i = 0; i < this.ChallengeGroups.Count; i++)
			{
				this.ChallengeGroups[i].Update(num, this.Player);
			}
			this.lastDay = num;
		}
		if (Time.time - this.lastUpdateTime >= 1f)
		{
			this.FireEvent(MinEventTypes.onSelfChallengeCompleteUpdate, this.Player.MinEventContext);
			this.lastUpdateTime = Time.time;
		}
	}

	// Token: 0x06002D06 RID: 11526 RVA: 0x0012D55C File Offset: 0x0012B75C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupData()
	{
		this.eventList.Clear();
		foreach (ChallengeClass challengeClass in ChallengeClass.s_Challenges.Values)
		{
			if (challengeClass.HasEventsOrPassives())
			{
				this.eventList.Add(challengeClass.Name, challengeClass);
			}
		}
	}

	// Token: 0x06002D07 RID: 11527 RVA: 0x0012D5D4 File Offset: 0x0012B7D4
	public void FireEvent(MinEventTypes _eventType, MinEventParams _params)
	{
		if (this.eventList == null)
		{
			return;
		}
		for (int i = 0; i < this.CompleteChallengesForMinEvents.Count; i++)
		{
			Challenge challenge = this.CompleteChallengesForMinEvents[i];
			ChallengeClass challengeClass = challenge.ChallengeClass;
			_params.Challenge = challenge;
			challengeClass.FireEvent(_eventType, _params);
		}
	}

	// Token: 0x06002D08 RID: 11528 RVA: 0x0012D624 File Offset: 0x0012B824
	public void ModifyValue(PassiveEffects _effect, ref float _base_val, ref float _perc_val, FastTags<TagGroup.Global> _tags)
	{
		for (int i = 0; i < this.CompleteChallengesForMinEvents.Count; i++)
		{
			Challenge challenge = this.CompleteChallengesForMinEvents[i];
			if (challenge != null)
			{
				ChallengeClass challengeClass = challenge.ChallengeClass;
				if (challengeClass != null)
				{
					MinEffectController effects = challengeClass.Effects;
					if (effects != null)
					{
						HashSet<PassiveEffects> passivesIndex = effects.PassivesIndex;
						if (passivesIndex != null && passivesIndex.Contains(_effect))
						{
							challengeClass.ModifyValue(this.Player, _effect, ref _base_val, ref _perc_val, _tags);
						}
					}
				}
			}
		}
		for (int j = 0; j < this.CompleteChallengeGroupsForMinEvents.Count; j++)
		{
			ChallengeGroup challengeGroup = this.CompleteChallengeGroupsForMinEvents[j];
			if (challengeGroup != null)
			{
				MinEffectController effects2 = challengeGroup.Effects;
				if (effects2 != null)
				{
					HashSet<PassiveEffects> passivesIndex2 = effects2.PassivesIndex;
					if (passivesIndex2 != null && passivesIndex2.Contains(_effect))
					{
						challengeGroup.ModifyValue(this.Player, _effect, ref _base_val, ref _perc_val, _tags);
					}
				}
			}
		}
	}

	// Token: 0x06002D09 RID: 11529 RVA: 0x0012D6F8 File Offset: 0x0012B8F8
	public void StartChallenges(EntityPlayerLocal player)
	{
		if (this.Player == null)
		{
			this.Player = player;
		}
		if (this.Player == null)
		{
			return;
		}
		if (this.ChallengeGroups.Count == 0)
		{
			using (Dictionary<string, ChallengeGroup>.ValueCollection.Enumerator enumerator = ChallengeGroup.s_ChallengeGroups.Values.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ChallengeGroup challengeGroup = enumerator.Current;
					ChallengeGroupEntry challengeGroupEntry = new ChallengeGroupEntry(challengeGroup);
					this.ChallengeGroups.Add(challengeGroupEntry);
					challengeGroup.IsComplete = true;
					challengeGroupEntry.CreateChallenges(this.Player);
				}
				goto IL_FE;
			}
		}
		int num = 0;
		foreach (ChallengeGroup challengeGroup2 in ChallengeGroup.s_ChallengeGroups.Values)
		{
			challengeGroup2.IsComplete = true;
			if (num < this.ChallengeGroups.Count)
			{
				ChallengeGroupEntry challengeGroupEntry2 = this.ChallengeGroups[num];
				if (challengeGroupEntry2.ChallengeGroup == challengeGroup2)
				{
					challengeGroupEntry2.AddAnyMissingChallenges(this.Player);
				}
			}
			num++;
		}
		IL_FE:
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			challenge.StartChallenge();
			if (challenge.ChallengeState != Challenge.ChallengeStates.Redeemed)
			{
				challenge.ChallengeGroup.IsComplete = false;
			}
			if (challenge.IsTracked)
			{
				LocalPlayerUI.GetUIForPrimaryPlayer().xui.QuestTracker.TrackedChallenge = challenge;
			}
		}
		foreach (ChallengeGroup challengeGroup3 in ChallengeGroup.s_ChallengeGroups.Values)
		{
			if (challengeGroup3.IsComplete)
			{
				this.CompleteChallengeGroupsForMinEvents.Add(challengeGroup3);
			}
		}
	}

	// Token: 0x06002D0A RID: 11530 RVA: 0x0012D8DC File Offset: 0x0012BADC
	public void EndChallenges()
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			this.Challenges[i].EndChallenge(false);
		}
	}

	// Token: 0x06002D0B RID: 11531 RVA: 0x0012D911 File Offset: 0x0012BB11
	public void AddChallenge(Challenge challenge)
	{
		this.ChallengeDictionary.Add(challenge.ChallengeClass.Name, challenge);
		this.Challenges.Add(challenge);
	}

	// Token: 0x06002D0C RID: 11532 RVA: 0x0012D938 File Offset: 0x0012BB38
	public void RemoveChallengesForGroup(ChallengeGroup challengeGroup)
	{
		for (int i = this.Challenges.Count - 1; i >= 0; i--)
		{
			Challenge challenge = this.Challenges[i];
			if (challenge.ChallengeGroup == challengeGroup)
			{
				challenge.EndChallenge(false);
				this.ChallengeDictionary.Remove(challenge.ChallengeClass.Name);
				this.Challenges.RemoveAt(i);
			}
		}
	}

	// Token: 0x06002D0D RID: 11533 RVA: 0x0012D9A0 File Offset: 0x0012BBA0
	public void ResetChallenges()
	{
		if (!GameManager.Instance.World.IsEditor() && !GameUtils.IsWorldEditor() && !GameUtils.IsPlaytesting())
		{
			this.EndChallenges();
			this.ChallengeDictionary.Clear();
			this.Challenges.Clear();
			this.ChallengeGroups.Clear();
			this.CompleteChallengesForMinEvents.Clear();
			this.CompleteChallengeGroupsForMinEvents.Clear();
			this.StartChallenges(this.Player);
		}
	}

	// Token: 0x06002D0E RID: 11534 RVA: 0x0012DA15 File Offset: 0x0012BC15
	public void HandleChallengeRedeemed(Challenge challenge)
	{
		if (this.eventList.ContainsKey(challenge.ChallengeClass.Name))
		{
			this.CompleteChallengesForMinEvents.Add(challenge);
		}
	}

	// Token: 0x06002D0F RID: 11535 RVA: 0x0012DA3C File Offset: 0x0012BC3C
	[PublicizedFrom(EAccessModifier.Internal)]
	public void HandleChallengeGroupComplete(ChallengeGroup group)
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			Challenge challenge = this.Challenges[i];
			if (challenge.ChallengeGroup == group && challenge.ChallengeState != Challenge.ChallengeStates.Redeemed)
			{
				group.IsComplete = false;
				Manager.PlayInsidePlayerHead("ui_challenge_redeem", -1, 0f, false, false);
				return;
			}
		}
		if (group.RewardEvent != null)
		{
			GameEventManager.Current.HandleAction(group.RewardEvent, null, this.Player, false, "", "", false, true, "", null);
		}
		Manager.PlayInsidePlayerHead("ui_challenge_complete_row", -1, 0f, false, false);
		group.IsComplete = true;
		if (!this.CompleteChallengeGroupsForMinEvents.Contains(group))
		{
			this.CompleteChallengeGroupsForMinEvents.Add(group);
		}
		GameManager.Instance.StartCoroutine(this.unhideRowLater(group));
	}

	// Token: 0x06002D10 RID: 11536 RVA: 0x0012DB10 File Offset: 0x0012BD10
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator unhideRowLater(ChallengeGroup group)
	{
		yield return new WaitForSeconds(1f);
		bool flag = false;
		foreach (ChallengeGroup challengeGroup in ChallengeGroup.s_ChallengeGroups.Values)
		{
			if (challengeGroup.HiddenBy.EqualsCaseInsensitive(group.Name))
			{
				challengeGroup.UIDirty = true;
				flag = true;
			}
		}
		if (flag)
		{
			Manager.PlayInsidePlayerHead("ui_challenge_unhide_row", -1, 0f, false, false);
		}
		yield break;
	}

	// Token: 0x06002D11 RID: 11537 RVA: 0x0012DB20 File Offset: 0x0012BD20
	[PublicizedFrom(EAccessModifier.Internal)]
	public Challenge GetNextChallenge(Challenge challenge)
	{
		Challenge challenge2 = null;
		string text = challenge.ChallengeGroup.ChallengeClasses[0].Name;
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			challenge2 = this.ChallengeDictionary[text];
		}
		while (challenge2 != null && !challenge2.IsActive)
		{
			text = challenge2.ChallengeClass.GetNextChallengeName();
			if (text != "" && this.ChallengeDictionary.ContainsKey(text))
			{
				challenge2 = this.ChallengeDictionary[text];
			}
			else
			{
				challenge2 = null;
			}
		}
		return challenge2;
	}

	// Token: 0x06002D12 RID: 11538 RVA: 0x0012DBB4 File Offset: 0x0012BDB4
	[PublicizedFrom(EAccessModifier.Internal)]
	public Challenge GetNextRedeemableChallenge(Challenge challenge)
	{
		Challenge challenge2 = null;
		string text = challenge.ChallengeGroup.ChallengeClasses[0].Name;
		if (text != "" && this.ChallengeDictionary.ContainsKey(text))
		{
			challenge2 = this.ChallengeDictionary[text];
		}
		while (challenge2 != null && !challenge2.ReadyToComplete)
		{
			text = challenge2.ChallengeClass.GetNextChallengeName();
			if (text != "" && this.ChallengeDictionary.ContainsKey(text))
			{
				challenge2 = this.ChallengeDictionary[text];
			}
			else
			{
				challenge2 = null;
			}
		}
		return challenge2;
	}

	// Token: 0x06002D13 RID: 11539 RVA: 0x0012DC48 File Offset: 0x0012BE48
	public bool HasCompletedChallenges()
	{
		for (int i = 0; i < this.Challenges.Count; i++)
		{
			if (this.Challenges[i].ReadyToComplete)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x040023B8 RID: 9144
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cCurrentSaveVersion = 2;

	// Token: 0x040023B9 RID: 9145
	public List<ChallengeGroupEntry> ChallengeGroups = new List<ChallengeGroupEntry>();

	// Token: 0x040023BA RID: 9146
	public Dictionary<string, Challenge> ChallengeDictionary = new Dictionary<string, Challenge>();

	// Token: 0x040023BB RID: 9147
	public List<Challenge> Challenges = new List<Challenge>();

	// Token: 0x040023BC RID: 9148
	public List<Challenge> CompleteChallengesForMinEvents = new List<Challenge>();

	// Token: 0x040023BD RID: 9149
	public List<ChallengeGroup> CompleteChallengeGroupsForMinEvents = new List<ChallengeGroup>();

	// Token: 0x040023BE RID: 9150
	[PublicizedFrom(EAccessModifier.Private)]
	public Dictionary<string, ChallengeClass> eventList = new Dictionary<string, ChallengeClass>();

	// Token: 0x040023BF RID: 9151
	public EntityPlayerLocal Player;

	// Token: 0x040023C0 RID: 9152
	[PublicizedFrom(EAccessModifier.Private)]
	public int lastDay;

	// Token: 0x040023C1 RID: 9153
	[PublicizedFrom(EAccessModifier.Private)]
	public float lastUpdateTime;
}

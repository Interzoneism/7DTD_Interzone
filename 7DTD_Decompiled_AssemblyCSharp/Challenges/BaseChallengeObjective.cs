using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015E4 RID: 5604
	[Preserve]
	public class BaseChallengeObjective
	{
		// Token: 0x1700133A RID: 4922
		// (get) Token: 0x0600ABFD RID: 44029 RVA: 0x004393FA File Offset: 0x004375FA
		// (set) Token: 0x0600ABFE RID: 44030 RVA: 0x00439402 File Offset: 0x00437602
		public byte CurrentFileVersion { get; set; }

		// Token: 0x1700133B RID: 4923
		// (get) Token: 0x0600ABFF RID: 44031 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Invalid;
			}
		}

		// Token: 0x1700133C RID: 4924
		// (get) Token: 0x0600AC00 RID: 44032 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual ChallengeClass.UINavTypes NavType
		{
			get
			{
				return ChallengeClass.UINavTypes.None;
			}
		}

		// Token: 0x1700133D RID: 4925
		// (get) Token: 0x0600AC01 RID: 44033 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public virtual bool NeedsConstantUIUpdate
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700133E RID: 4926
		// (get) Token: 0x0600AC02 RID: 44034 RVA: 0x0043940B File Offset: 0x0043760B
		// (set) Token: 0x0600AC03 RID: 44035 RVA: 0x00439413 File Offset: 0x00437613
		public bool Complete
		{
			get
			{
				return this.complete;
			}
			set
			{
				if (this.complete != value)
				{
					this.complete = value;
					this.HandleValueChanged();
				}
			}
		}

		// Token: 0x1700133F RID: 4927
		// (get) Token: 0x0600AC04 RID: 44036 RVA: 0x0043942B File Offset: 0x0043762B
		// (set) Token: 0x0600AC05 RID: 44037 RVA: 0x00439433 File Offset: 0x00437633
		public int Current
		{
			get
			{
				return this.current;
			}
			set
			{
				if (this.current != value)
				{
					this.current = value;
					this.HandleValueChanged();
				}
			}
		}

		// Token: 0x0600AC06 RID: 44038 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void BaseInit()
		{
		}

		// Token: 0x17001340 RID: 4928
		// (get) Token: 0x0600AC07 RID: 44039 RVA: 0x0043944B File Offset: 0x0043764B
		public EntityPlayerLocal Player
		{
			get
			{
				return this.Owner.Owner.Player;
			}
		}

		// Token: 0x0600AC08 RID: 44040 RVA: 0x0043945D File Offset: 0x0043765D
		public void ResetComplete()
		{
			this.Complete = false;
			this.current = 0;
		}

		// Token: 0x14000109 RID: 265
		// (add) Token: 0x0600AC09 RID: 44041 RVA: 0x00439470 File Offset: 0x00437670
		// (remove) Token: 0x0600AC0A RID: 44042 RVA: 0x004394A8 File Offset: 0x004376A8
		public event ObjectiveValueChanged ValueChanged;

		// Token: 0x17001341 RID: 4929
		// (get) Token: 0x0600AC0B RID: 44043 RVA: 0x004394DD File Offset: 0x004376DD
		public string ObjectiveText
		{
			get
			{
				return string.Format("{0} {1}", this.DescriptionText, this.StatusText);
			}
		}

		// Token: 0x17001342 RID: 4930
		// (get) Token: 0x0600AC0C RID: 44044 RVA: 0x0002B133 File Offset: 0x00029333
		public virtual string DescriptionText
		{
			get
			{
				return "";
			}
		}

		// Token: 0x17001343 RID: 4931
		// (get) Token: 0x0600AC0D RID: 44045 RVA: 0x004394F5 File Offset: 0x004376F5
		public virtual string StatusText
		{
			get
			{
				return string.Format("{0}/{1}", this.current, this.MaxCount);
			}
		}

		// Token: 0x17001344 RID: 4932
		// (get) Token: 0x0600AC0E RID: 44046 RVA: 0x00439517 File Offset: 0x00437717
		public virtual float FillAmount
		{
			get
			{
				return (float)this.current / (float)this.MaxCount;
			}
		}

		// Token: 0x0600AC0F RID: 44047 RVA: 0x00439528 File Offset: 0x00437728
		[PublicizedFrom(EAccessModifier.Protected)]
		public void HandleValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged();
			}
		}

		// Token: 0x0600AC10 RID: 44048 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void Init()
		{
		}

		// Token: 0x0600AC11 RID: 44049 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void HandleOnCreated()
		{
		}

		// Token: 0x0600AC12 RID: 44050 RVA: 0x0043953D File Offset: 0x0043773D
		public virtual bool HandleCheckStatus()
		{
			this.Complete = this.CheckObjectiveComplete(false);
			return this.Complete;
		}

		// Token: 0x0600AC13 RID: 44051 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void UpdateStatus()
		{
		}

		// Token: 0x0600AC14 RID: 44052 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void HandleAddHooks()
		{
		}

		// Token: 0x0600AC15 RID: 44053 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void HandleRemoveHooks()
		{
		}

		// Token: 0x0600AC16 RID: 44054 RVA: 0x00439552 File Offset: 0x00437752
		public virtual void HandleTrackingStarted()
		{
			this.IsTracking = true;
		}

		// Token: 0x0600AC17 RID: 44055 RVA: 0x0043955C File Offset: 0x0043775C
		public virtual void CopyValues(BaseChallengeObjective obj, BaseChallengeObjective objFromClass)
		{
			this.current = obj.current;
			this.MaxCount = objFromClass.MaxCount;
			this.ShowRequirements = objFromClass.ShowRequirements;
			this.Biome = objFromClass.Biome;
			this.complete = (this.Current >= this.MaxCount);
		}

		// Token: 0x0600AC18 RID: 44056 RVA: 0x004395B0 File Offset: 0x004377B0
		public virtual void HandleTrackingEnded()
		{
			this.IsTracking = false;
		}

		// Token: 0x0600AC19 RID: 44057 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void HandleUpdatingCurrent()
		{
		}

		// Token: 0x0600AC1A RID: 44058 RVA: 0x004395BC File Offset: 0x004377BC
		public static BaseChallengeObjective ReadObjective(byte _currentVersion, ChallengeObjectiveType _type, BinaryReader _br)
		{
			BaseChallengeObjective baseChallengeObjective = null;
			switch (_type)
			{
			case ChallengeObjectiveType.BlockPlace:
				baseChallengeObjective = new ChallengeObjectiveBlockPlace();
				break;
			case ChallengeObjectiveType.BlockUpgrade:
				baseChallengeObjective = new ChallengeObjectiveBlockUpgrade();
				break;
			case ChallengeObjectiveType.Bloodmoon:
				baseChallengeObjective = new ChallengeObjectiveBloodmoon();
				break;
			case ChallengeObjectiveType.Craft:
				baseChallengeObjective = new ChallengeObjectiveCraft();
				break;
			case ChallengeObjectiveType.CureDebuff:
				baseChallengeObjective = new ChallengeObjectiveCureDebuff();
				break;
			case ChallengeObjectiveType.EnterBiome:
				baseChallengeObjective = new ChallengeObjectiveEnterBiome();
				break;
			case ChallengeObjectiveType.Gather:
				baseChallengeObjective = new ChallengeObjectiveGather();
				break;
			case ChallengeObjectiveType.GatherIngredient:
				baseChallengeObjective = new ChallengeObjectiveGatherIngredient();
				break;
			case ChallengeObjectiveType.Harvest:
				baseChallengeObjective = new ChallengeObjectiveHarvest();
				break;
			case ChallengeObjectiveType.Hold:
				baseChallengeObjective = new ChallengeObjectiveHold();
				break;
			case ChallengeObjectiveType.Kill:
				baseChallengeObjective = new ChallengeObjectiveKill();
				break;
			case ChallengeObjectiveType.QuestComplete:
				baseChallengeObjective = new ChallengeObjectiveQuestComplete();
				break;
			case ChallengeObjectiveType.Scrap:
				baseChallengeObjective = new ChallengeObjectiveScrap();
				break;
			case ChallengeObjectiveType.Survive:
				baseChallengeObjective = new ChallengeObjectiveSurvive();
				break;
			case ChallengeObjectiveType.Trader:
				baseChallengeObjective = new ChallengeObjectiveTrader();
				break;
			case ChallengeObjectiveType.Wear:
				baseChallengeObjective = new ChallengeObjectiveWear();
				break;
			case ChallengeObjectiveType.Use:
				baseChallengeObjective = new ChallengeObjectiveUseItem();
				break;
			case ChallengeObjectiveType.ChallengeComplete:
				baseChallengeObjective = new ChallengeObjectiveChallengeComplete();
				break;
			case ChallengeObjectiveType.MeetTrader:
				baseChallengeObjective = new ChallengeObjectiveMeetTrader();
				break;
			case ChallengeObjectiveType.KillByTag:
				baseChallengeObjective = new ChallengeObjectiveKillByTag();
				break;
			case ChallengeObjectiveType.ChallengeStatAwarded:
				baseChallengeObjective = new ChallengeObjectiveChallengeStatAwarded();
				break;
			case ChallengeObjectiveType.SpendSkillPoint:
				baseChallengeObjective = new ChallengeObjectiveSpendSkillPoint();
				break;
			case ChallengeObjectiveType.Twitch:
				baseChallengeObjective = new ChallengeObjectiveTwitch();
				break;
			case ChallengeObjectiveType.Time:
				baseChallengeObjective = new ChallengeObjectiveTime();
				break;
			case ChallengeObjectiveType.GatherByTag:
				baseChallengeObjective = new ChallengeObjectiveGatherByTag();
				break;
			case ChallengeObjectiveType.LootContainer:
				baseChallengeObjective = new ChallengeObjectiveLootContainer();
				break;
			}
			if (baseChallengeObjective != null)
			{
				baseChallengeObjective.Read(_currentVersion, _br);
			}
			return baseChallengeObjective;
		}

		// Token: 0x0600AC1B RID: 44059 RVA: 0x00019766 File Offset: 0x00017966
		public virtual Recipe GetRecipeItem()
		{
			return null;
		}

		// Token: 0x0600AC1C RID: 44060 RVA: 0x0043973C File Offset: 0x0043793C
		public virtual Recipe[] GetRecipeItems()
		{
			if (this.Owner.NeedsPreRequisites)
			{
				Recipe recipeFromRequirements = this.Owner.GetRecipeFromRequirements();
				if (recipeFromRequirements != null)
				{
					return new Recipe[]
					{
						recipeFromRequirements
					};
				}
			}
			return null;
		}

		// Token: 0x0600AC1D RID: 44061 RVA: 0x00439771 File Offset: 0x00437971
		public virtual void Read(byte _currentVersion, BinaryReader _br)
		{
			this.current = _br.ReadInt32();
		}

		// Token: 0x0600AC1E RID: 44062 RVA: 0x0043977F File Offset: 0x0043797F
		public void WriteObjective(BinaryWriter _bw)
		{
			_bw.Write((byte)this.ObjectiveType);
			this.Write(_bw);
		}

		// Token: 0x0600AC1F RID: 44063 RVA: 0x00439794 File Offset: 0x00437994
		public virtual void Write(BinaryWriter _bw)
		{
			_bw.Write(this.current);
		}

		// Token: 0x0600AC20 RID: 44064 RVA: 0x004397A4 File Offset: 0x004379A4
		public virtual bool CheckObjectiveComplete(bool handleComplete = true)
		{
			this.HandleUpdatingCurrent();
			if (this.Current >= this.MaxCount)
			{
				this.Current = this.MaxCount;
				this.Complete = true;
				if (handleComplete)
				{
					this.Owner.HandleComplete(true);
				}
				return true;
			}
			if (handleComplete)
			{
				this.Owner.HandleComplete(true);
			}
			this.Complete = false;
			return false;
		}

		// Token: 0x0600AC21 RID: 44065 RVA: 0x00439800 File Offset: 0x00437A00
		public virtual bool CheckBaseRequirements()
		{
			return this.Biome != "" && (this.Owner.Owner.Player.biomeStandingOn == null || this.Owner.Owner.Player.biomeStandingOn.m_sBiomeName != this.Biome);
		}

		// Token: 0x0600AC22 RID: 44066 RVA: 0x00019766 File Offset: 0x00017966
		public virtual BaseChallengeObjective Clone()
		{
			return null;
		}

		// Token: 0x0600AC23 RID: 44067 RVA: 0x00439860 File Offset: 0x00437A60
		public virtual void ParseElement(XElement e)
		{
			if (e.HasAttribute("count"))
			{
				this.MaxCount = StringParsers.ParseSInt32(e.GetAttribute("count"), 0, -1, NumberStyles.Integer);
			}
			if (e.HasAttribute("show_requirements"))
			{
				this.ShowRequirements = StringParsers.ParseBool(e.GetAttribute("show_requirements"), 0, -1, true);
			}
			if (e.HasAttribute("biome"))
			{
				this.Biome = e.GetAttribute("biome");
			}
		}

		// Token: 0x0600AC24 RID: 44068 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Update(float deltaTime)
		{
		}

		// Token: 0x0600AC25 RID: 44069 RVA: 0x004398F5 File Offset: 0x00437AF5
		public void HandleUpdate(float deltaTime)
		{
			if (this.Owner.IsActive)
			{
				this.Update(deltaTime);
			}
		}

		// Token: 0x0600AC26 RID: 44070 RVA: 0x0043990B File Offset: 0x00437B0B
		public virtual void CompleteObjective(bool handleComplete = true)
		{
			this.Current = this.MaxCount;
			this.CheckObjectiveComplete(handleComplete);
		}

		// Token: 0x0400863B RID: 34363
		public static byte FileVersion = 1;

		// Token: 0x0400863D RID: 34365
		public int MaxCount = 1;

		// Token: 0x0400863E RID: 34366
		public bool IsRequirement;

		// Token: 0x0400863F RID: 34367
		public Challenge Owner;

		// Token: 0x04008640 RID: 34368
		public ChallengeClass OwnerClass;

		// Token: 0x04008641 RID: 34369
		public string Biome = "";

		// Token: 0x04008642 RID: 34370
		public bool ShowRequirements = true;

		// Token: 0x04008643 RID: 34371
		[PublicizedFrom(EAccessModifier.Private)]
		public bool complete;

		// Token: 0x04008644 RID: 34372
		public bool IsTracking;

		// Token: 0x04008645 RID: 34373
		[PublicizedFrom(EAccessModifier.Protected)]
		public int current;
	}
}

using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015FC RID: 5628
	[Preserve]
	public class ChallengeObjectiveTime : BaseChallengeObjective
	{
		// Token: 0x17001374 RID: 4980
		// (get) Token: 0x0600AD23 RID: 44323 RVA: 0x0043D52B File Offset: 0x0043B72B
		// (set) Token: 0x0600AD24 RID: 44324 RVA: 0x0043D533 File Offset: 0x0043B733
		public float CurrentTime
		{
			get
			{
				return this.currentTime;
			}
			set
			{
				this.currentTime = value;
				base.HandleValueChanged();
			}
		}

		// Token: 0x17001375 RID: 4981
		// (get) Token: 0x0600AD25 RID: 44325 RVA: 0x000445B9 File Offset: 0x000427B9
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Time;
			}
		}

		// Token: 0x17001376 RID: 4982
		// (get) Token: 0x0600AD26 RID: 44326 RVA: 0x0043D544 File Offset: 0x0043B744
		public override string DescriptionText
		{
			get
			{
				if (this.Biome == "")
				{
					return Localization.Get("challengeObjectiveTime", false) + ":";
				}
				return string.Format(Localization.Get("challengeObjectiveTimeInBiome", false), Localization.Get("biome_" + this.Biome, false));
			}
		}

		// Token: 0x17001377 RID: 4983
		// (get) Token: 0x0600AD27 RID: 44327 RVA: 0x0043D5A0 File Offset: 0x0043B7A0
		public override string StatusText
		{
			get
			{
				if (this.currentTime == 0f)
				{
					return string.Format("{0}{1}", this.currentTime, Localization.Get("timeAbbreviationSeconds", false)) + " / " + XUiM_PlayerBuffs.GetTimeString(this.maxTime);
				}
				return XUiM_PlayerBuffs.GetTimeString(this.currentTime) + "/ " + XUiM_PlayerBuffs.GetTimeString(this.maxTime);
			}
		}

		// Token: 0x17001378 RID: 4984
		// (get) Token: 0x0600AD28 RID: 44328 RVA: 0x0043D610 File Offset: 0x0043B810
		public override float FillAmount
		{
			get
			{
				return this.currentTime / this.maxTime;
			}
		}

		// Token: 0x17001379 RID: 4985
		// (get) Token: 0x0600AD29 RID: 44329 RVA: 0x0043D61F File Offset: 0x0043B81F
		public override bool NeedsConstantUIUpdate
		{
			get
			{
				return !base.Complete;
			}
		}

		// Token: 0x0600AD2A RID: 44330 RVA: 0x00002914 File Offset: 0x00000B14
		public override void Init()
		{
		}

		// Token: 0x0600AD2B RID: 44331 RVA: 0x0043D62A File Offset: 0x0043B82A
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.AddObjectiveToBeUpdated(this);
		}

		// Token: 0x0600AD2C RID: 44332 RVA: 0x0043D637 File Offset: 0x0043B837
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.RemoveObjectiveToBeUpdated(this);
		}

		// Token: 0x0600AD2D RID: 44333 RVA: 0x0043D644 File Offset: 0x0043B844
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void Update(float deltaTime)
		{
			this.nextCheck -= deltaTime;
			if (this.nextCheck <= 0f)
			{
				this.isInvalid = this.CheckBaseRequirements();
				this.nextCheck = 2f;
			}
			if (this.player == null)
			{
				this.player = this.Owner.Owner.Player;
			}
			if (this.isInvalid || this.player == null || this.player.IsDead())
			{
				return;
			}
			this.CurrentTime += deltaTime;
			if (this.currentTime >= this.maxTime)
			{
				base.Current = this.MaxCount;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600AD2E RID: 44334 RVA: 0x0043D6FB File Offset: 0x0043B8FB
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("max_time"))
			{
				this.maxTime = StringParsers.ParseFloat(e.GetAttribute("max_time"), 0, -1, NumberStyles.Any);
			}
		}

		// Token: 0x0600AD2F RID: 44335 RVA: 0x0043D738 File Offset: 0x0043B938
		public override void Read(byte _currentVersion, BinaryReader _br)
		{
			base.Read(_currentVersion, _br);
			this.currentTime = _br.ReadSingle();
		}

		// Token: 0x0600AD30 RID: 44336 RVA: 0x0043D74E File Offset: 0x0043B94E
		public override void Write(BinaryWriter _bw)
		{
			base.Write(_bw);
			_bw.Write(this.currentTime);
		}

		// Token: 0x0600AD31 RID: 44337 RVA: 0x0043D764 File Offset: 0x0043B964
		public override void CopyValues(BaseChallengeObjective obj, BaseChallengeObjective objFromClass)
		{
			base.CopyValues(obj, objFromClass);
			ChallengeObjectiveTime challengeObjectiveTime = obj as ChallengeObjectiveTime;
			if (challengeObjectiveTime != null)
			{
				this.currentTime = challengeObjectiveTime.currentTime;
			}
		}

		// Token: 0x0600AD32 RID: 44338 RVA: 0x0043D790 File Offset: 0x0043B990
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveTime
			{
				Biome = this.Biome,
				maxTime = this.maxTime,
				currentTime = this.currentTime,
				nextCheck = this.nextCheck,
				isInvalid = this.isInvalid
			};
		}

		// Token: 0x04008696 RID: 34454
		[PublicizedFrom(EAccessModifier.Protected)]
		public float currentTime;

		// Token: 0x04008697 RID: 34455
		[PublicizedFrom(EAccessModifier.Protected)]
		public float maxTime;

		// Token: 0x04008698 RID: 34456
		[PublicizedFrom(EAccessModifier.Protected)]
		public float nextCheck;

		// Token: 0x04008699 RID: 34457
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isInvalid = true;

		// Token: 0x0400869A RID: 34458
		[PublicizedFrom(EAccessModifier.Private)]
		public EntityPlayerLocal player;
	}
}

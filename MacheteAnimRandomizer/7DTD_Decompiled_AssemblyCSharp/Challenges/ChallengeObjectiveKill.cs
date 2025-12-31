using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F4 RID: 5620
	[Preserve]
	public class ChallengeObjectiveKill : BaseChallengeObjective
	{
		// Token: 0x17001363 RID: 4963
		// (get) Token: 0x0600ACDB RID: 44251 RVA: 0x0018853A File Offset: 0x0018673A
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.Kill;
			}
		}

		// Token: 0x17001364 RID: 4964
		// (get) Token: 0x0600ACDC RID: 44252 RVA: 0x0043C84C File Offset: 0x0043AA4C
		public override string DescriptionText
		{
			get
			{
				if (this.Biome == "")
				{
					return Localization.Get("challengeObjectiveKill", false) + " " + Localization.Get(this.entityIDs, false) + ":";
				}
				return string.Format(Localization.Get("challengeObjectiveKillIn", false), Localization.Get(this.entityIDs, false), Localization.Get("biome_" + this.Biome, false));
			}
		}

		// Token: 0x0600ACDD RID: 44253 RVA: 0x0043C8C4 File Offset: 0x0043AAC4
		public override void Init()
		{
			if (this.entityIDs != null)
			{
				string[] array = this.entityIDs.Split(',', StringSplitOptions.None);
				if (array.Length > 1)
				{
					this.entityIDs = array[0];
					this.entityNames = new string[array.Length - 1];
					for (int i = 1; i < array.Length; i++)
					{
						this.entityNames[i - 1] = array[i];
					}
				}
			}
		}

		// Token: 0x0600ACDE RID: 44254 RVA: 0x0043C922 File Offset: 0x0043AB22
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
			QuestEventManager.Current.EntityKill += this.Current_EntityKill;
		}

		// Token: 0x0600ACDF RID: 44255 RVA: 0x0043C950 File Offset: 0x0043AB50
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
		}

		// Token: 0x0600ACE0 RID: 44256 RVA: 0x0043C968 File Offset: 0x0043AB68
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_EntityKill(EntityAlive killedBy, EntityAlive killedEntity)
		{
			string entityClassName = killedEntity.EntityClass.entityClassName;
			bool flag = false;
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.entityIDs == null || entityClassName.EqualsCaseInsensitive(this.entityIDs))
			{
				flag = true;
			}
			if (!flag && this.entityNames != null)
			{
				for (int i = 0; i < this.entityNames.Length; i++)
				{
					if (this.entityNames[i].EqualsCaseInsensitive(entityClassName))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				int num = base.Current;
				base.Current = num + 1;
				this.CheckObjectiveComplete(true);
			}
		}

		// Token: 0x0600ACE1 RID: 44257 RVA: 0x0043C9F1 File Offset: 0x0043ABF1
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("entity_names"))
			{
				this.entityIDs = e.GetAttribute("entity_names");
			}
		}

		// Token: 0x0600ACE2 RID: 44258 RVA: 0x0043CA22 File Offset: 0x0043AC22
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveKill
			{
				entityIDs = this.entityIDs,
				entityNames = this.entityNames,
				Biome = this.Biome
			};
		}

		// Token: 0x04008684 RID: 34436
		[PublicizedFrom(EAccessModifier.Private)]
		public string entityIDs = "";

		// Token: 0x04008685 RID: 34437
		[PublicizedFrom(EAccessModifier.Private)]
		public string[] entityNames;
	}
}

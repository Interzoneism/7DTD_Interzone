using System;
using System.Xml.Linq;
using UnityEngine.Scripting;

namespace Challenges
{
	// Token: 0x020015F5 RID: 5621
	[Preserve]
	public class ChallengeObjectiveKillByTag : BaseChallengeObjective
	{
		// Token: 0x17001365 RID: 4965
		// (get) Token: 0x0600ACE4 RID: 44260 RVA: 0x0005772E File Offset: 0x0005592E
		public override ChallengeObjectiveType ObjectiveType
		{
			get
			{
				return ChallengeObjectiveType.KillByTag;
			}
		}

		// Token: 0x17001366 RID: 4966
		// (get) Token: 0x0600ACE5 RID: 44261 RVA: 0x0043CA60 File Offset: 0x0043AC60
		public override string DescriptionText
		{
			get
			{
				if (this.Biome == "")
				{
					return Localization.Get("challengeObjectiveKill", false) + " " + Localization.Get(this.targetName, false) + ":";
				}
				return string.Format(Localization.Get("challengeObjectiveKillIn", false), Localization.Get(this.targetName, false), Localization.Get("biome_" + this.Biome, false));
			}
		}

		// Token: 0x0600ACE6 RID: 44262 RVA: 0x0043CAD8 File Offset: 0x0043ACD8
		public override void Init()
		{
			this.entityTags = FastTags<TagGroup.Global>.Parse(this.entityTag);
		}

		// Token: 0x0600ACE7 RID: 44263 RVA: 0x0043CAEB File Offset: 0x0043ACEB
		public override void HandleAddHooks()
		{
			QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
			QuestEventManager.Current.EntityKill += this.Current_EntityKill;
		}

		// Token: 0x0600ACE8 RID: 44264 RVA: 0x0043CB19 File Offset: 0x0043AD19
		public override void HandleRemoveHooks()
		{
			QuestEventManager.Current.EntityKill -= this.Current_EntityKill;
		}

		// Token: 0x0600ACE9 RID: 44265 RVA: 0x0043CB34 File Offset: 0x0043AD34
		[PublicizedFrom(EAccessModifier.Private)]
		public void Current_EntityKill(EntityAlive killedBy, EntityAlive killedEntity)
		{
			if (!this.entityTags.Test_AnySet(killedEntity.EntityClass.Tags))
			{
				return;
			}
			if (this.CheckBaseRequirements())
			{
				return;
			}
			if (this.isTwitchSpawn > -1)
			{
				if (this.isTwitchSpawn == 0 && killedEntity.spawnById != -1)
				{
					return;
				}
				if (this.isTwitchSpawn == 1 && killedEntity.spawnById == -1)
				{
					return;
				}
			}
			if (!this.killerHasBuffTag.IsEmpty && !killedBy.Buffs.HasBuffByTag(this.killerHasBuffTag))
			{
				return;
			}
			if (!this.killedHasBuffTag.IsEmpty && !killedEntity.Buffs.HasBuffByTag(this.killedHasBuffTag))
			{
				return;
			}
			int num = base.Current;
			base.Current = num + 1;
			this.CheckObjectiveComplete(true);
		}

		// Token: 0x0600ACEA RID: 44266 RVA: 0x0043CBEC File Offset: 0x0043ADEC
		public override void ParseElement(XElement e)
		{
			base.ParseElement(e);
			if (e.HasAttribute("entity_tags"))
			{
				this.entityTag = e.GetAttribute("entity_tags");
			}
			if (e.HasAttribute("target_name_key"))
			{
				this.targetName = Localization.Get(e.GetAttribute("target_name_key"), false);
			}
			else if (e.HasAttribute("target_name"))
			{
				this.targetName = e.GetAttribute("target_name");
			}
			if (e.HasAttribute("is_twitch_spawn"))
			{
				this.isTwitchSpawn = (StringParsers.ParseBool(e.GetAttribute("is_twitch_spawn"), 0, -1, true) ? 1 : 0);
			}
			if (e.HasAttribute("killer_has_bufftag"))
			{
				this.killerHasBuffTag = FastTags<TagGroup.Global>.Parse(e.GetAttribute("killer_has_bufftag"));
			}
			if (e.HasAttribute("killed_has_bufftag"))
			{
				this.killedHasBuffTag = FastTags<TagGroup.Global>.Parse(e.GetAttribute("killed_has_bufftag"));
			}
		}

		// Token: 0x0600ACEB RID: 44267 RVA: 0x0043CD10 File Offset: 0x0043AF10
		public override BaseChallengeObjective Clone()
		{
			return new ChallengeObjectiveKillByTag
			{
				entityTag = this.entityTag,
				entityTags = this.entityTags,
				Biome = this.Biome,
				targetName = this.targetName,
				isTwitchSpawn = this.isTwitchSpawn,
				killerHasBuffTag = this.killerHasBuffTag,
				killedHasBuffTag = this.killedHasBuffTag
			};
		}

		// Token: 0x04008686 RID: 34438
		[PublicizedFrom(EAccessModifier.Private)]
		public string entityTag = "";

		// Token: 0x04008687 RID: 34439
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> entityTags;

		// Token: 0x04008688 RID: 34440
		[PublicizedFrom(EAccessModifier.Private)]
		public string targetName = "";

		// Token: 0x04008689 RID: 34441
		[PublicizedFrom(EAccessModifier.Private)]
		public int isTwitchSpawn = -1;

		// Token: 0x0400868A RID: 34442
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> killerHasBuffTag;

		// Token: 0x0400868B RID: 34443
		[PublicizedFrom(EAccessModifier.Private)]
		public FastTags<TagGroup.Global> killedHasBuffTag;
	}
}

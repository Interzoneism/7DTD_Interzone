using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Challenges
{
	// Token: 0x020015DB RID: 5595
	public class ChallengeCategory
	{
		// Token: 0x0600ABD0 RID: 43984 RVA: 0x004382DB File Offset: 0x004364DB
		public ChallengeCategory(string name)
		{
			this.Name = name;
		}

		// Token: 0x0600ABD1 RID: 43985 RVA: 0x00438300 File Offset: 0x00436500
		public bool CanShow(EntityPlayer player)
		{
			if (this.showType == ChallengeCategory.ShowTypes.Twitch)
			{
				return player.TwitchEnabled;
			}
			if (this.showType == ChallengeCategory.ShowTypes.CVar)
			{
				return player.Buffs.GetCustomVar(this.showValue) > 0f;
			}
			return this.showType != ChallengeCategory.ShowTypes.BiomeProgression || GameStats.GetBool(EnumGameStats.BiomeProgression);
		}

		// Token: 0x0600ABD2 RID: 43986 RVA: 0x00438354 File Offset: 0x00436554
		public void ParseElement(XElement e)
		{
			if (e.HasAttribute("title_key"))
			{
				this.Title = Localization.Get(e.GetAttribute("title_key"), false);
			}
			else if (e.HasAttribute("title"))
			{
				this.Title = e.GetAttribute("title");
			}
			else
			{
				this.Title = this.Name;
			}
			if (e.HasAttribute("icon"))
			{
				this.Icon = e.GetAttribute("icon");
			}
			if (e.HasAttribute("show_type"))
			{
				this.showType = (ChallengeCategory.ShowTypes)Enum.Parse(typeof(ChallengeCategory.ShowTypes), e.GetAttribute("show_type"), true);
			}
			if (e.HasAttribute("show_value"))
			{
				this.showValue = e.GetAttribute("show_value");
			}
			if (e.HasAttribute("display_key"))
			{
				this.DisplayKey = e.GetAttribute("display_key");
			}
		}

		// Token: 0x040085E3 RID: 34275
		public static Dictionary<string, ChallengeCategory> s_ChallengeCategories = new CaseInsensitiveStringDictionary<ChallengeCategory>();

		// Token: 0x040085E4 RID: 34276
		public string Name;

		// Token: 0x040085E5 RID: 34277
		public string Icon;

		// Token: 0x040085E6 RID: 34278
		public string Title;

		// Token: 0x040085E7 RID: 34279
		public string DisplayKey = "";

		// Token: 0x040085E8 RID: 34280
		[PublicizedFrom(EAccessModifier.Private)]
		public ChallengeCategory.ShowTypes showType;

		// Token: 0x040085E9 RID: 34281
		[PublicizedFrom(EAccessModifier.Private)]
		public string showValue = "";

		// Token: 0x020015DC RID: 5596
		public enum ShowTypes
		{
			// Token: 0x040085EB RID: 34283
			Normal,
			// Token: 0x040085EC RID: 34284
			CVar,
			// Token: 0x040085ED RID: 34285
			Twitch,
			// Token: 0x040085EE RID: 34286
			BiomeProgression
		}
	}
}

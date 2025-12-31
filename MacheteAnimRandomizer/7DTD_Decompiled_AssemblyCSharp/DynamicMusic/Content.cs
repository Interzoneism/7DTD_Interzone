using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x0200170E RID: 5902
	[Preserve]
	public abstract class Content
	{
		// Token: 0x0600B203 RID: 45571 RVA: 0x004550A3 File Offset: 0x004532A3
		public Content()
		{
			Content.AllContent.Add(this);
		}

		// Token: 0x170013B5 RID: 5045
		// (get) Token: 0x0600B204 RID: 45572 RVA: 0x004550B6 File Offset: 0x004532B6
		// (set) Token: 0x0600B205 RID: 45573 RVA: 0x004550BE File Offset: 0x004532BE
		public string Name { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013B6 RID: 5046
		// (get) Token: 0x0600B206 RID: 45574 RVA: 0x004550C7 File Offset: 0x004532C7
		// (set) Token: 0x0600B207 RID: 45575 RVA: 0x004550CF File Offset: 0x004532CF
		public SectionType Section { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013B7 RID: 5047
		// (get) Token: 0x0600B208 RID: 45576 RVA: 0x004550D8 File Offset: 0x004532D8
		// (set) Token: 0x0600B209 RID: 45577 RVA: 0x004550E0 File Offset: 0x004532E0
		public virtual bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x0600B20A RID: 45578
		[Preserve]
		public abstract IEnumerator Load();

		// Token: 0x0600B20B RID: 45579
		[Preserve]
		public abstract void Unload();

		// Token: 0x0600B20C RID: 45580 RVA: 0x004550E9 File Offset: 0x004532E9
		[Preserve]
		public static Content CreateWrapper(string _type)
		{
			return (Content)Activator.CreateInstance(ReflectionHelpers.GetTypeWithPrefix("DynamicMusic.", _type));
		}

		// Token: 0x0600B20D RID: 45581 RVA: 0x00455100 File Offset: 0x00453300
		public virtual void ParseFromXml(XElement _xmlNode)
		{
			this.Name = _xmlNode.GetAttribute("name");
		}

		// Token: 0x04008BA8 RID: 35752
		public static Dictionary<SectionType, int> SamplesFor = new Dictionary<SectionType, int>();

		// Token: 0x04008BA9 RID: 35753
		public static Dictionary<SectionType, string> SourcePathFor = new Dictionary<SectionType, string>();

		// Token: 0x04008BAA RID: 35754
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();

		// Token: 0x04008BAB RID: 35755
		public static List<Content> AllContent = new List<Content>();
	}
}

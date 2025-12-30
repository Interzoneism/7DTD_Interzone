using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x0200172B RID: 5931
	[Preserve]
	public class SingleClip : Content
	{
		// Token: 0x170013DA RID: 5082
		// (get) Token: 0x0600B2C2 RID: 45762 RVA: 0x00457310 File Offset: 0x00455510
		// (set) Token: 0x0600B2C3 RID: 45763 RVA: 0x00457318 File Offset: 0x00455518
		public AudioClip Clip { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B2C4 RID: 45764 RVA: 0x00457321 File Offset: 0x00455521
		public override void Unload()
		{
			this.Clip.UnloadAudioData();
			this.IsLoaded = false;
		}

		// Token: 0x0600B2C5 RID: 45765 RVA: 0x00457336 File Offset: 0x00455536
		public override IEnumerator Load()
		{
			if (!this.IsLoaded)
			{
				SingleClip.<>c__DisplayClass6_0 CS$<>8__locals1 = new SingleClip.<>c__DisplayClass6_0();
				CS$<>8__locals1.requestTask = LoadManager.LoadAsset<AudioClip>(this.path, null, null, false, false);
				yield return new WaitUntil(() => CS$<>8__locals1.requestTask.IsDone);
				this.Clip = CS$<>8__locals1.requestTask.Asset;
				CS$<>8__locals1 = null;
			}
			this.IsLoaded = true;
			yield break;
		}

		// Token: 0x0600B2C6 RID: 45766 RVA: 0x00457345 File Offset: 0x00455545
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			base.Section = EnumUtils.Parse<SectionType>(_xmlNode.GetAttribute("section"), false);
			this.path = _xmlNode.GetAttribute("path");
		}

		// Token: 0x04008C05 RID: 35845
		[PublicizedFrom(EAccessModifier.Private)]
		public string path;
	}
}

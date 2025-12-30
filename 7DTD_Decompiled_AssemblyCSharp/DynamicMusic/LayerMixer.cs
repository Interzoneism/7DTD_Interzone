using System;
using System.Collections;
using System.Collections.Generic;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001739 RID: 5945
	[Preserve]
	public abstract class LayerMixer<ConfigType> : ILayerMixer where ConfigType : IConfiguration
	{
		// Token: 0x170013E9 RID: 5097
		// (get) Token: 0x0600B313 RID: 45843 RVA: 0x00458622 File Offset: 0x00456822
		// (set) Token: 0x0600B314 RID: 45844 RVA: 0x0045862A File Offset: 0x0045682A
		public SectionType Sect { get; set; }

		// Token: 0x0600B315 RID: 45845 RVA: 0x00458633 File Offset: 0x00456833
		public LayerMixer()
		{
			this.clipSetsFor = new EnumDictionary<LayerType, List<LayeredContent>>();
		}

		// Token: 0x170013EA RID: 5098
		public abstract float this[int _idx]
		{
			get;
		}

		// Token: 0x0600B317 RID: 45847 RVA: 0x00458646 File Offset: 0x00456846
		public virtual IEnumerator Load()
		{
			Log.Out(string.Format("Loading new config for {0}...", this.Sect));
			this.config = AbstractConfiguration.Get<ConfigType>(this.Sect);
			if (this.config == null)
			{
				Log.Warning(string.Format("{0} pulled a null config", this.Sect));
			}
			this.clipSetsFor.Clear();
			yield return null;
			yield break;
		}

		// Token: 0x0600B318 RID: 45848 RVA: 0x00458658 File Offset: 0x00456858
		public void Unload()
		{
			this.clipSetsFor.Values.ToList<List<LayeredContent>>().ForEach(delegate(List<LayeredContent> list)
			{
				list.ToList<LayeredContent>().ForEach(delegate(LayeredContent e)
				{
					e.Unload();
				});
			});
			this.clipSetsFor.Clear();
			Log.Out(string.Format("unloaded ClipSets on {0}", this.Sect));
		}

		// Token: 0x04008C3D RID: 35901
		[PublicizedFrom(EAccessModifier.Protected)]
		public ConfigType config;

		// Token: 0x04008C3E RID: 35902
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumDictionary<LayerType, List<LayeredContent>> clipSetsFor;
	}
}

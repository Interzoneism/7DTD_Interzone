using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x0200171C RID: 5916
	[Preserve]
	public class ClipPairAdapter : IClipAdapter
	{
		// Token: 0x170013CD RID: 5069
		// (get) Token: 0x0600B273 RID: 45683 RVA: 0x0045623C File Offset: 0x0045443C
		// (set) Token: 0x0600B274 RID: 45684 RVA: 0x00456244 File Offset: 0x00454444
		public bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B275 RID: 45685 RVA: 0x0045624D File Offset: 0x0045444D
		public ClipPairAdapter()
		{
			this.clipAdapterLo = new ClipAdapter();
			this.clipAdapterHi = new ClipAdapter();
		}

		// Token: 0x0600B276 RID: 45686 RVA: 0x0045626B File Offset: 0x0045446B
		public float GetSample(int idx, params float[] _params)
		{
			return _params[0] * (this.clipAdapterLo.GetSample(idx, null) * (1f - _params[1]) + _params[1] * this.clipAdapterHi.GetSample(idx, null));
		}

		// Token: 0x0600B277 RID: 45687 RVA: 0x0045629A File Offset: 0x0045449A
		public IEnumerator Load()
		{
			yield return this.clipAdapterLo.Load();
			yield return this.clipAdapterHi.Load();
			yield break;
		}

		// Token: 0x0600B278 RID: 45688 RVA: 0x004562A9 File Offset: 0x004544A9
		public void LoadImmediate()
		{
			this.clipAdapterLo.LoadImmediate();
			this.clipAdapterHi.LoadImmediate();
		}

		// Token: 0x0600B279 RID: 45689 RVA: 0x004562C1 File Offset: 0x004544C1
		public void Unload()
		{
			this.clipAdapterLo.Unload();
			this.clipAdapterHi.Unload();
			this.IsLoaded = false;
		}

		// Token: 0x0600B27A RID: 45690 RVA: 0x00002914 File Offset: 0x00000B14
		public void ParseXml(XElement _xmlNode)
		{
		}

		// Token: 0x0600B27B RID: 45691 RVA: 0x004562E0 File Offset: 0x004544E0
		public void SetPaths(int _num, PlacementType _placement, SectionType _section, LayerType _layer, string stress = "")
		{
			this.clipAdapterLo.SetPaths(_num, _placement, _section, _layer, "Lo");
			this.clipAdapterHi.SetPaths(_num, _placement, _section, _layer, "Hi");
		}

		// Token: 0x04008BE3 RID: 35811
		[PublicizedFrom(EAccessModifier.Private)]
		public ClipAdapter clipAdapterLo;

		// Token: 0x04008BE4 RID: 35812
		[PublicizedFrom(EAccessModifier.Private)]
		public ClipAdapter clipAdapterHi;
	}
}

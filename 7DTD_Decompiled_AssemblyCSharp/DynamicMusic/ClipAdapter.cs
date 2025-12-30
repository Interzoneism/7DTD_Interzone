using System;
using System.Collections;
using System.Xml.Linq;
using MusicUtils;
using MusicUtils.Enums;
using Unity.Profiling;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x0200171A RID: 5914
	[Preserve]
	public class ClipAdapter : IClipAdapter
	{
		// Token: 0x0600B263 RID: 45667 RVA: 0x00455FDC File Offset: 0x004541DC
		public float GetSample(int idx, params float[] _params)
		{
			if (idx % 4096 == 0)
			{
				this.reader.Position = 2 * idx;
				this.reader.Read(this.sampleData, 4096);
			}
			return this.sampleData[idx % 4096];
		}

		// Token: 0x170013CA RID: 5066
		// (get) Token: 0x0600B264 RID: 45668 RVA: 0x00456019 File Offset: 0x00454219
		// (set) Token: 0x0600B265 RID: 45669 RVA: 0x00456021 File Offset: 0x00454221
		public bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B266 RID: 45670 RVA: 0x0045602A File Offset: 0x0045422A
		public IEnumerator Load()
		{
			using (ClipAdapter.s_LoadMarker.Auto())
			{
				this.reader = new WaveReader(this.path);
				this.sampleData = MemoryPools.poolFloat.Alloc(4096);
				this.IsLoaded = true;
			}
			yield return null;
			yield break;
		}

		// Token: 0x0600B267 RID: 45671 RVA: 0x0045603C File Offset: 0x0045423C
		public void LoadImmediate()
		{
			using (ClipAdapter.s_LoadImmediateMarker.Auto())
			{
				this.reader = new WaveReader(this.path);
				this.sampleData = MemoryPools.poolFloat.Alloc(4096);
				this.IsLoaded = true;
			}
		}

		// Token: 0x0600B268 RID: 45672 RVA: 0x004560A4 File Offset: 0x004542A4
		public void Unload()
		{
			MemoryPools.poolFloat.Free(this.sampleData);
			this.sampleData = null;
			this.reader.Cleanup();
			this.reader = null;
			this.IsLoaded = false;
		}

		// Token: 0x0600B269 RID: 45673 RVA: 0x004560D6 File Offset: 0x004542D6
		public void ParseXml(XElement _xmlNode)
		{
			this.path = _xmlNode.GetAttribute("value");
		}

		// Token: 0x0600B26A RID: 45674 RVA: 0x004560F0 File Offset: 0x004542F0
		public void SetPaths(int _num, PlacementType _placement, SectionType _section, LayerType _layer, string stress = "")
		{
			this.path = string.Concat(new string[]
			{
				GameIO.GetApplicationPath(),
				"/Data/Music/",
				_num.ToString("000"),
				DMSConstants.PlacementAbbrv[_placement],
				DMSConstants.SectionAbbrvs[_section],
				DMSConstants.LayerAbbrvs[_layer],
				stress,
				".wav"
			});
		}

		// Token: 0x04008BD9 RID: 35801
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_LoadMarker = new ProfilerMarker("DynamicMusic.ClipAdapter.Load");

		// Token: 0x04008BDA RID: 35802
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_LoadImmediateMarker = new ProfilerMarker("DynamicMusic.ClipAdapter.LoadImmediate");

		// Token: 0x04008BDB RID: 35803
		[PublicizedFrom(EAccessModifier.Private)]
		public const int bufferSize = 4096;

		// Token: 0x04008BDC RID: 35804
		[PublicizedFrom(EAccessModifier.Private)]
		public string path;

		// Token: 0x04008BDD RID: 35805
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] sampleData;

		// Token: 0x04008BDE RID: 35806
		[PublicizedFrom(EAccessModifier.Private)]
		public WaveReader reader;
	}
}

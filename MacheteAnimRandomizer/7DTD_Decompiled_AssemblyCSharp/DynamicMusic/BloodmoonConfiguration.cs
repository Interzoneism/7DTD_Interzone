using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001721 RID: 5921
	[Preserve]
	public class BloodmoonConfiguration : AbstractConfiguration, IConfiguration<LayerState>, IConfiguration
	{
		// Token: 0x170013D4 RID: 5076
		// (get) Token: 0x0600B299 RID: 45721 RVA: 0x00456C82 File Offset: 0x00454E82
		// (set) Token: 0x0600B29A RID: 45722 RVA: 0x00456C8A File Offset: 0x00454E8A
		public Dictionary<LayerType, LayerState> Layers { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B29B RID: 45723 RVA: 0x00456C93 File Offset: 0x00454E93
		public BloodmoonConfiguration()
		{
			this.Layers = new Dictionary<LayerType, LayerState>();
		}

		// Token: 0x0600B29C RID: 45724 RVA: 0x00456CA6 File Offset: 0x00454EA6
		public override int CountFor(LayerType _layer)
		{
			if (!this.Layers.ContainsKey(_layer))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x0600B29D RID: 45725 RVA: 0x00456CBC File Offset: 0x00454EBC
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement element in _xmlNode.Elements("layer"))
			{
				LayerType key = EnumUtils.Parse<LayerType>(element.GetAttribute("key"), false);
				float lo = float.Parse(element.GetAttribute("lo"));
				float hi = float.Parse(element.GetAttribute("hi"));
				this.Layers.Add(key, new LayerState((float tl) => BloodmoonConfiguration.getState(tl, lo, hi)));
			}
		}

		// Token: 0x0600B29E RID: 45726 RVA: 0x00456D84 File Offset: 0x00454F84
		[PublicizedFrom(EAccessModifier.Private)]
		public static LayerStateType getState(float _threatLevel, float _enabledThreshold, float _hiThreshold)
		{
			if (_threatLevel < _enabledThreshold)
			{
				return LayerStateType.disabled;
			}
			if (_threatLevel >= _hiThreshold)
			{
				return LayerStateType.hi;
			}
			return LayerStateType.lo;
		}
	}
}

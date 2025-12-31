using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001725 RID: 5925
	[Preserve]
	public class FixedConfiguration : AbstractConfiguration, IConfiguration<FixedConfigurationLayerData>, IConfiguration
	{
		// Token: 0x170013D7 RID: 5079
		// (get) Token: 0x0600B2A9 RID: 45737 RVA: 0x00456EC0 File Offset: 0x004550C0
		// (set) Token: 0x0600B2AA RID: 45738 RVA: 0x00456EC8 File Offset: 0x004550C8
		public Dictionary<LayerType, FixedConfigurationLayerData> Layers { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B2AB RID: 45739 RVA: 0x00456ED1 File Offset: 0x004550D1
		public FixedConfiguration()
		{
			this.Layers = new Dictionary<LayerType, FixedConfigurationLayerData>();
		}

		// Token: 0x0600B2AC RID: 45740 RVA: 0x00456EE4 File Offset: 0x004550E4
		public override int CountFor(LayerType _layer)
		{
			FixedConfigurationLayerData fixedConfigurationLayerData;
			if (this.Layers.TryGetValue(_layer, out fixedConfigurationLayerData))
			{
				return fixedConfigurationLayerData.Count;
			}
			return 0;
		}

		// Token: 0x0600B2AD RID: 45741 RVA: 0x00456F0C File Offset: 0x0045510C
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement e in _xmlNode.Elements("layer"))
			{
				this.ParseLayers(e);
			}
		}

		// Token: 0x0600B2AE RID: 45742 RVA: 0x00456F6C File Offset: 0x0045516C
		[PublicizedFrom(EAccessModifier.Private)]
		public void ParseLayers(XElement e)
		{
			List<PlacementType> list = new List<PlacementType>();
			foreach (string s in e.GetAttribute("value").Split(',', StringSplitOptions.None))
			{
				list.Add((PlacementType)byte.Parse(s));
			}
			LayerType key = EnumUtils.Parse<LayerType>(e.GetAttribute("key"), false);
			FixedConfigurationLayerData fixedConfigurationLayerData;
			if (!this.Layers.TryGetValue(key, out fixedConfigurationLayerData))
			{
				this.Layers.Add(key, fixedConfigurationLayerData = new FixedConfigurationLayerData());
			}
			fixedConfigurationLayerData.Add(list);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001724 RID: 5924
	[Preserve]
	public class Configuration : AbstractConfiguration, IFiniteConfiguration, IConfiguration<IList<PlacementType>>, IConfiguration
	{
		// Token: 0x170013D6 RID: 5078
		// (get) Token: 0x0600B2A3 RID: 45731 RVA: 0x00456DB6 File Offset: 0x00454FB6
		// (set) Token: 0x0600B2A4 RID: 45732 RVA: 0x00456DBE File Offset: 0x00454FBE
		public Dictionary<LayerType, IList<PlacementType>> Layers { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B2A5 RID: 45733 RVA: 0x00456DC7 File Offset: 0x00454FC7
		public Configuration()
		{
			this.Layers = new Dictionary<LayerType, IList<PlacementType>>();
		}

		// Token: 0x0600B2A6 RID: 45734 RVA: 0x00456DDA File Offset: 0x00454FDA
		public override int CountFor(LayerType _layer)
		{
			if (!this.Layers.ContainsKey(_layer))
			{
				return 0;
			}
			return 1;
		}

		// Token: 0x0600B2A7 RID: 45735 RVA: 0x00456DF0 File Offset: 0x00454FF0
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement e in _xmlNode.Elements("layer"))
			{
				this.ParseLayers(e);
			}
		}

		// Token: 0x0600B2A8 RID: 45736 RVA: 0x00456E50 File Offset: 0x00455050
		[PublicizedFrom(EAccessModifier.Private)]
		public void ParseLayers(XElement e)
		{
			List<PlacementType> list = new List<PlacementType>();
			foreach (string s in e.GetAttribute("value").Split(',', StringSplitOptions.None))
			{
				list.Add((PlacementType)byte.Parse(s));
			}
			LayerType key = EnumUtils.Parse<LayerType>(e.GetAttribute("key"), false);
			this.Layers.Add(key, list);
		}
	}
}

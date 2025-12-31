using System;
using System.Xml.Linq;
using MusicUtils.Enums;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001727 RID: 5927
	[Preserve]
	public class BloodmoonClipSet : LayeredContent
	{
		// Token: 0x0600B2B2 RID: 45746 RVA: 0x0045702C File Offset: 0x0045522C
		public override float GetSample(PlacementType _placement, int _idx, params float[] _params)
		{
			return this.clips[_placement].GetSample(_idx, _params);
		}

		// Token: 0x0600B2B3 RID: 45747 RVA: 0x00457044 File Offset: 0x00455244
		public override void ParseFromXml(XElement _xmlNode)
		{
			base.ParseFromXml(_xmlNode);
			foreach (XElement xelement in _xmlNode.Elements("clip"))
			{
				PlacementType key = EnumUtils.Parse<PlacementType>(xelement.GetAttribute("key"), false);
				IClipAdapter clipAdapter = LayeredContent.CreateClipAdapter(xelement.GetAttribute("type"));
				clipAdapter.ParseXml(xelement);
				this.clips.Add(key, clipAdapter);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001754 RID: 5972
	public interface IConfigurable
	{
		// Token: 0x0600B3B2 RID: 46002
		void SetConfiguration(IList<PlacementType> _placements);
	}
}

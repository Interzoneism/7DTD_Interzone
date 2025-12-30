using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x0200170C RID: 5900
	public abstract class AbstractFilter : IFilter<SectionType>
	{
		// Token: 0x0600B200 RID: 45568
		public abstract List<SectionType> Filter(List<SectionType> _list);

		// Token: 0x0600B201 RID: 45569 RVA: 0x0000A7E3 File Offset: 0x000089E3
		[PublicizedFrom(EAccessModifier.Protected)]
		public AbstractFilter()
		{
		}
	}
}

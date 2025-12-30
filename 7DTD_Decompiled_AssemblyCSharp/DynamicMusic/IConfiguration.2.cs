using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001756 RID: 5974
	public interface IConfiguration<T> : IConfiguration
	{
		// Token: 0x17001407 RID: 5127
		// (get) Token: 0x0600B3B6 RID: 46006
		Dictionary<LayerType, T> Layers { get; }
	}
}

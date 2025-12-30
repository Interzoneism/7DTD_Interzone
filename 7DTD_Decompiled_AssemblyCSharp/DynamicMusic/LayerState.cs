using System;
using MusicUtils.Enums;

namespace DynamicMusic
{
	// Token: 0x02001723 RID: 5923
	public class LayerState : ICountable
	{
		// Token: 0x170013D5 RID: 5077
		// (get) Token: 0x0600B2A1 RID: 45729 RVA: 0x000197A5 File Offset: 0x000179A5
		public int Count
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x0600B2A2 RID: 45730 RVA: 0x00456DA7 File Offset: 0x00454FA7
		public LayerState(Func<float, LayerStateType> _getFunc)
		{
			this.Get = _getFunc;
		}

		// Token: 0x04008BFA RID: 35834
		public readonly Func<float, LayerStateType> Get;
	}
}

using System;

namespace DynamicMusic
{
	// Token: 0x02001736 RID: 5942
	public class LayerParams
	{
		// Token: 0x0600B304 RID: 45828 RVA: 0x00457FDA File Offset: 0x004561DA
		public LayerParams(float _volume, float _mix)
		{
			this.Volume = _volume;
			this.Mix = _mix;
		}

		// Token: 0x04008C2E RID: 35886
		public float Volume;

		// Token: 0x04008C2F RID: 35887
		public float Mix;
	}
}

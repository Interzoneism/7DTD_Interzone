using System;
using UnityEngine;

namespace mumblelib
{
	// Token: 0x020013D5 RID: 5077
	public interface ILinkFile : IDisposable
	{
		// Token: 0x17001113 RID: 4371
		// (get) Token: 0x06009E9C RID: 40604
		// (set) Token: 0x06009E9B RID: 40603
		uint UIVersion { get; set; }

		// Token: 0x06009E9D RID: 40605
		void Tick();

		// Token: 0x17001114 RID: 4372
		// (set) Token: 0x06009E9E RID: 40606
		Vector3 AvatarPosition { set; }

		// Token: 0x17001115 RID: 4373
		// (set) Token: 0x06009E9F RID: 40607
		Vector3 AvatarForward { set; }

		// Token: 0x17001116 RID: 4374
		// (set) Token: 0x06009EA0 RID: 40608
		Vector3 AvatarTop { set; }

		// Token: 0x17001117 RID: 4375
		// (set) Token: 0x06009EA1 RID: 40609
		string Name { set; }

		// Token: 0x17001118 RID: 4376
		// (set) Token: 0x06009EA2 RID: 40610
		Vector3 CameraPosition { set; }

		// Token: 0x17001119 RID: 4377
		// (set) Token: 0x06009EA3 RID: 40611
		Vector3 CameraForward { set; }

		// Token: 0x1700111A RID: 4378
		// (set) Token: 0x06009EA4 RID: 40612
		Vector3 CameraTop { set; }

		// Token: 0x1700111B RID: 4379
		// (set) Token: 0x06009EA5 RID: 40613
		string Identity { set; }

		// Token: 0x1700111C RID: 4380
		// (set) Token: 0x06009EA6 RID: 40614
		string Context { set; }

		// Token: 0x1700111D RID: 4381
		// (set) Token: 0x06009EA7 RID: 40615
		string Description { set; }
	}
}

using System;
using UnityEngine;

// Token: 0x020001A8 RID: 424
public interface EntityUINotification
{
	// Token: 0x170000E4 RID: 228
	// (get) Token: 0x06000CEF RID: 3311
	BuffValue Buff { get; }

	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x06000CF0 RID: 3312
	string Icon { get; }

	// Token: 0x06000CF1 RID: 3313
	Color GetColor();

	// Token: 0x170000E6 RID: 230
	// (get) Token: 0x06000CF2 RID: 3314
	float CurrentValue { get; }

	// Token: 0x170000E7 RID: 231
	// (get) Token: 0x06000CF3 RID: 3315
	string Units { get; }

	// Token: 0x170000E8 RID: 232
	// (get) Token: 0x06000CF4 RID: 3316
	EnumEntityUINotificationDisplayMode DisplayMode { get; }

	// Token: 0x170000E9 RID: 233
	// (get) Token: 0x06000CF5 RID: 3317
	EnumEntityUINotificationSubject Subject { get; }
}

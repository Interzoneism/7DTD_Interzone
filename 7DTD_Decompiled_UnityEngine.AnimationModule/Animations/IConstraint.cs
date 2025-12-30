using System;
using System.Collections.Generic;

namespace UnityEngine.Animations
{
	// Token: 0x02000065 RID: 101
	public interface IConstraint
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000587 RID: 1415
		// (set) Token: 0x06000588 RID: 1416
		float weight { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000589 RID: 1417
		// (set) Token: 0x0600058A RID: 1418
		bool constraintActive { get; set; }

		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600058B RID: 1419
		// (set) Token: 0x0600058C RID: 1420
		bool locked { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600058D RID: 1421
		int sourceCount { get; }

		// Token: 0x0600058E RID: 1422
		int AddSource(ConstraintSource source);

		// Token: 0x0600058F RID: 1423
		void RemoveSource(int index);

		// Token: 0x06000590 RID: 1424
		ConstraintSource GetSource(int index);

		// Token: 0x06000591 RID: 1425
		void SetSource(int index, ConstraintSource source);

		// Token: 0x06000592 RID: 1426
		void GetSources(List<ConstraintSource> sources);

		// Token: 0x06000593 RID: 1427
		void SetSources(List<ConstraintSource> sources);
	}
}

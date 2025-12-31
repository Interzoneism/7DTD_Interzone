using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F26 RID: 3878
[PublicizedFrom(EAccessModifier.Internal)]
public static class XUiUpdater
{
	// Token: 0x06007BD3 RID: 31699 RVA: 0x00321CCD File Offset: 0x0031FECD
	public static void Add(XUi _ui)
	{
		if (!XUiUpdater.uiToUpdate.Contains(_ui))
		{
			XUiUpdater.uiToUpdate.Add(_ui);
		}
	}

	// Token: 0x06007BD4 RID: 31700 RVA: 0x00321CE7 File Offset: 0x0031FEE7
	public static void Remove(XUi _ui)
	{
		XUiUpdater.uiToUpdate.Remove(_ui);
	}

	// Token: 0x06007BD5 RID: 31701 RVA: 0x00321CF8 File Offset: 0x0031FEF8
	public static void Update()
	{
		if (XUiUpdater.uiToUpdate.Count > 0)
		{
			for (int i = 0; i < XUiUpdater.uiToUpdate.Count; i++)
			{
				if (XUiUpdater.uiToUpdate[i] != null)
				{
					XUiUpdater.uiToUpdate[i].OnUpdateDeltaTime(Time.deltaTime);
					XUiUpdater.uiToUpdate[i].OnUpdateInput();
				}
			}
		}
	}

	// Token: 0x04005DA9 RID: 23977
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<XUi> uiToUpdate = new List<XUi>();
}

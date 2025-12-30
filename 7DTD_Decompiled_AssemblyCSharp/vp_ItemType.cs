using System;
using UnityEngine;

// Token: 0x0200130E RID: 4878
[Serializable]
public class vp_ItemType : ScriptableObject
{
	// Token: 0x17000F87 RID: 3975
	// (get) Token: 0x060097FE RID: 38910 RVA: 0x003C72E2 File Offset: 0x003C54E2
	[SerializeField]
	public string DisplayNameFull
	{
		get
		{
			return this.IndefiniteArticle + " " + this.DisplayName;
		}
	}

	// Token: 0x0400746D RID: 29805
	public string IndefiniteArticle = "a";

	// Token: 0x0400746E RID: 29806
	public string DisplayName;

	// Token: 0x0400746F RID: 29807
	public string Description;

	// Token: 0x04007470 RID: 29808
	public Texture2D Icon;

	// Token: 0x04007471 RID: 29809
	public float Space;
}

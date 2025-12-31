using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

// Token: 0x02000E9A RID: 3738
[Preserve]
public class XUiC_UnlockByList : XUiController
{
	// Token: 0x17000C05 RID: 3077
	// (get) Token: 0x060075FE RID: 30206 RVA: 0x00300DF5 File Offset: 0x002FEFF5
	// (set) Token: 0x060075FF RID: 30207 RVA: 0x00300DFD File Offset: 0x002FEFFD
	public Recipe Recipe
	{
		get
		{
			return this.recipe;
		}
		set
		{
			this.recipe = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06007600 RID: 30208 RVA: 0x00300E10 File Offset: 0x002FF010
	public override void Init()
	{
		base.Init();
		XUiController[] childrenByType = base.GetChildrenByType<XUiC_UnlockByEntry>(null);
		XUiController[] array = childrenByType;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				this.unlockByEntries.Add(array[i]);
			}
		}
	}

	// Token: 0x06007601 RID: 30209 RVA: 0x00300E50 File Offset: 0x002FF050
	public override void Update(float _dt)
	{
		if (this.isDirty)
		{
			if (this.recipe != null)
			{
				ItemClass forId = ItemClass.GetForId(this.recipe.itemValueType);
				RecipeUnlockData[] unlockedBy;
				if (forId.IsBlock())
				{
					unlockedBy = forId.GetBlock().UnlockedBy;
				}
				else
				{
					unlockedBy = forId.UnlockedBy;
				}
				int num = 0;
				int count = this.unlockByEntries.Count;
				Progression progression = base.xui.playerUI.entityPlayer.Progression;
				if (unlockedBy != null)
				{
					for (int i = 0; i < unlockedBy.Length; i++)
					{
						XUiC_UnlockByEntry xuiC_UnlockByEntry = this.unlockByEntries[i] as XUiC_UnlockByEntry;
						if (xuiC_UnlockByEntry != null)
						{
							xuiC_UnlockByEntry.UnlockData = unlockedBy[i];
							xuiC_UnlockByEntry.Recipe = this.recipe;
							num++;
						}
					}
				}
				for (int j = num; j < count; j++)
				{
					XUiC_UnlockByEntry xuiC_UnlockByEntry2 = this.unlockByEntries[j] as XUiC_UnlockByEntry;
					if (xuiC_UnlockByEntry2 != null)
					{
						xuiC_UnlockByEntry2.UnlockData = null;
					}
				}
			}
			else
			{
				int count2 = this.unlockByEntries.Count;
				for (int k = 0; k < count2; k++)
				{
					XUiC_UnlockByEntry xuiC_UnlockByEntry3 = this.unlockByEntries[k] as XUiC_UnlockByEntry;
					if (xuiC_UnlockByEntry3 != null)
					{
						xuiC_UnlockByEntry3.UnlockData = null;
					}
				}
			}
			this.isDirty = false;
		}
		base.Update(_dt);
	}

	// Token: 0x04005A03 RID: 23043
	[PublicizedFrom(EAccessModifier.Private)]
	public Recipe recipe;

	// Token: 0x04005A04 RID: 23044
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiController> unlockByEntries = new List<XUiController>();

	// Token: 0x04005A05 RID: 23045
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;
}

using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000040 RID: 64
[Serializable]
public class InvGameItem
{
	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000158 RID: 344 RVA: 0x0000E149 File Offset: 0x0000C349
	public int baseItemID
	{
		get
		{
			return this.mBaseItemID;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000159 RID: 345 RVA: 0x0000E151 File Offset: 0x0000C351
	public InvBaseItem baseItem
	{
		get
		{
			if (this.mBaseItem == null)
			{
				this.mBaseItem = InvDatabase.FindByID(this.baseItemID);
			}
			return this.mBaseItem;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x0600015A RID: 346 RVA: 0x0000E172 File Offset: 0x0000C372
	public string name
	{
		get
		{
			if (this.baseItem == null)
			{
				return null;
			}
			return this.quality.ToString() + " " + this.baseItem.name;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x0600015B RID: 347 RVA: 0x0000E1A4 File Offset: 0x0000C3A4
	public float statMultiplier
	{
		get
		{
			float num = 0f;
			switch (this.quality)
			{
			case InvGameItem.Quality.Broken:
				num = 0f;
				break;
			case InvGameItem.Quality.Cursed:
				num = -1f;
				break;
			case InvGameItem.Quality.Damaged:
				num = 0.25f;
				break;
			case InvGameItem.Quality.Worn:
				num = 0.9f;
				break;
			case InvGameItem.Quality.Sturdy:
				num = 1f;
				break;
			case InvGameItem.Quality.Polished:
				num = 1.1f;
				break;
			case InvGameItem.Quality.Improved:
				num = 1.25f;
				break;
			case InvGameItem.Quality.Crafted:
				num = 1.5f;
				break;
			case InvGameItem.Quality.Superior:
				num = 1.75f;
				break;
			case InvGameItem.Quality.Enchanted:
				num = 2f;
				break;
			case InvGameItem.Quality.Epic:
				num = 2.5f;
				break;
			case InvGameItem.Quality.Legendary:
				num = 3f;
				break;
			}
			float num2 = (float)this.itemLevel / 50f;
			return num * Mathf.Lerp(num2, num2 * num2, 0.5f);
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x0600015C RID: 348 RVA: 0x0000E274 File Offset: 0x0000C474
	public Color color
	{
		get
		{
			Color result = Color.white;
			switch (this.quality)
			{
			case InvGameItem.Quality.Broken:
				result = new Color(0.4f, 0.2f, 0.2f);
				break;
			case InvGameItem.Quality.Cursed:
				result = Color.red;
				break;
			case InvGameItem.Quality.Damaged:
				result = new Color(0.4f, 0.4f, 0.4f);
				break;
			case InvGameItem.Quality.Worn:
				result = new Color(0.7f, 0.7f, 0.7f);
				break;
			case InvGameItem.Quality.Sturdy:
				result = new Color(1f, 1f, 1f);
				break;
			case InvGameItem.Quality.Polished:
				result = NGUIMath.HexToColor(3774856959U);
				break;
			case InvGameItem.Quality.Improved:
				result = NGUIMath.HexToColor(2480359935U);
				break;
			case InvGameItem.Quality.Crafted:
				result = NGUIMath.HexToColor(1325334783U);
				break;
			case InvGameItem.Quality.Superior:
				result = NGUIMath.HexToColor(12255231U);
				break;
			case InvGameItem.Quality.Enchanted:
				result = NGUIMath.HexToColor(1937178111U);
				break;
			case InvGameItem.Quality.Epic:
				result = NGUIMath.HexToColor(2516647935U);
				break;
			case InvGameItem.Quality.Legendary:
				result = NGUIMath.HexToColor(4287627519U);
				break;
			}
			return result;
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x0000E394 File Offset: 0x0000C594
	public InvGameItem(int id)
	{
		this.mBaseItemID = id;
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000E3B1 File Offset: 0x0000C5B1
	public InvGameItem(int id, InvBaseItem bi)
	{
		this.mBaseItemID = id;
		this.mBaseItem = bi;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000E3D8 File Offset: 0x0000C5D8
	public List<InvStat> CalculateStats()
	{
		List<InvStat> list = new List<InvStat>();
		if (this.baseItem != null)
		{
			float statMultiplier = this.statMultiplier;
			List<InvStat> stats = this.baseItem.stats;
			int i = 0;
			int count = stats.Count;
			while (i < count)
			{
				InvStat invStat = stats[i];
				int num = Mathf.RoundToInt(statMultiplier * (float)invStat.amount);
				if (num != 0)
				{
					bool flag = false;
					int j = 0;
					int count2 = list.Count;
					while (j < count2)
					{
						InvStat invStat2 = list[j];
						if (invStat2.id == invStat.id && invStat2.modifier == invStat.modifier)
						{
							invStat2.amount += num;
							flag = true;
							break;
						}
						j++;
					}
					if (!flag)
					{
						list.Add(new InvStat
						{
							id = invStat.id,
							amount = num,
							modifier = invStat.modifier
						});
					}
				}
				i++;
			}
			list.Sort(new Comparison<InvStat>(InvStat.CompareArmor));
		}
		return list;
	}

	// Token: 0x040001F5 RID: 501
	[SerializeField]
	[PublicizedFrom(EAccessModifier.Private)]
	public int mBaseItemID;

	// Token: 0x040001F6 RID: 502
	public InvGameItem.Quality quality = InvGameItem.Quality.Sturdy;

	// Token: 0x040001F7 RID: 503
	public int itemLevel = 1;

	// Token: 0x040001F8 RID: 504
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public InvBaseItem mBaseItem;

	// Token: 0x02000041 RID: 65
	public enum Quality
	{
		// Token: 0x040001FA RID: 506
		Broken,
		// Token: 0x040001FB RID: 507
		Cursed,
		// Token: 0x040001FC RID: 508
		Damaged,
		// Token: 0x040001FD RID: 509
		Worn,
		// Token: 0x040001FE RID: 510
		Sturdy,
		// Token: 0x040001FF RID: 511
		Polished,
		// Token: 0x04000200 RID: 512
		Improved,
		// Token: 0x04000201 RID: 513
		Crafted,
		// Token: 0x04000202 RID: 514
		Superior,
		// Token: 0x04000203 RID: 515
		Enchanted,
		// Token: 0x04000204 RID: 516
		Epic,
		// Token: 0x04000205 RID: 517
		Legendary,
		// Token: 0x04000206 RID: 518
		_LastDoNotUse
	}
}

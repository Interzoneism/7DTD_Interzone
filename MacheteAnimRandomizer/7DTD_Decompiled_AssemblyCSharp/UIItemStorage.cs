using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000039 RID: 57
[AddComponentMenu("NGUI/Examples/UI Item Storage")]
public class UIItemStorage : MonoBehaviour
{
	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600013A RID: 314 RVA: 0x0000DA4B File Offset: 0x0000BC4B
	public List<InvGameItem> items
	{
		get
		{
			while (this.mItems.Count < this.maxItemCount)
			{
				this.mItems.Add(null);
			}
			return this.mItems;
		}
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000DA74 File Offset: 0x0000BC74
	public InvGameItem GetItem(int slot)
	{
		if (slot >= this.items.Count)
		{
			return null;
		}
		return this.mItems[slot];
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000DA92 File Offset: 0x0000BC92
	public InvGameItem Replace(int slot, InvGameItem item)
	{
		if (slot < this.maxItemCount)
		{
			InvGameItem result = this.items[slot];
			this.mItems[slot] = item;
			return result;
		}
		return item;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0000DAB8 File Offset: 0x0000BCB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (this.template != null)
		{
			int num = 0;
			Bounds bounds = default(Bounds);
			for (int i = 0; i < this.maxRows; i++)
			{
				for (int j = 0; j < this.maxColumns; j++)
				{
					GameObject gameObject = base.gameObject.AddChild(this.template);
					gameObject.transform.localPosition = new Vector3((float)this.padding + ((float)j + 0.5f) * (float)this.spacing, (float)(-(float)this.padding) - ((float)i + 0.5f) * (float)this.spacing, 0f);
					UIStorageSlot component = gameObject.GetComponent<UIStorageSlot>();
					if (component != null)
					{
						component.storage = this;
						component.slot = num;
					}
					bounds.Encapsulate(new Vector3((float)this.padding * 2f + (float)((j + 1) * this.spacing), (float)(-(float)this.padding) * 2f - (float)((i + 1) * this.spacing), 0f));
					if (++num >= this.maxItemCount)
					{
						if (this.background != null)
						{
							this.background.transform.localScale = bounds.size;
						}
						return;
					}
				}
			}
			if (this.background != null)
			{
				this.background.transform.localScale = bounds.size;
			}
		}
	}

	// Token: 0x040001CC RID: 460
	public int maxItemCount = 8;

	// Token: 0x040001CD RID: 461
	public int maxRows = 4;

	// Token: 0x040001CE RID: 462
	public int maxColumns = 4;

	// Token: 0x040001CF RID: 463
	public GameObject template;

	// Token: 0x040001D0 RID: 464
	public UIWidget background;

	// Token: 0x040001D1 RID: 465
	public int spacing = 128;

	// Token: 0x040001D2 RID: 466
	public int padding = 10;

	// Token: 0x040001D3 RID: 467
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<InvGameItem> mItems = new List<InvGameItem>();
}

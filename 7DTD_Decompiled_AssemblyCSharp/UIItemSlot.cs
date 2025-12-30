using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000038 RID: 56
public abstract class UIItemSlot : MonoBehaviour
{
	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000131 RID: 305
	public abstract InvGameItem observedItem { [PublicizedFrom(EAccessModifier.Protected)] get; }

	// Token: 0x06000132 RID: 306
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract InvGameItem Replace(InvGameItem item);

	// Token: 0x06000133 RID: 307 RVA: 0x0000D674 File Offset: 0x0000B874
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnTooltip(bool show)
	{
		InvGameItem invGameItem = show ? this.mItem : null;
		if (invGameItem != null)
		{
			InvBaseItem baseItem = invGameItem.baseItem;
			if (baseItem != null)
			{
				string text = string.Concat(new string[]
				{
					"[",
					NGUIText.EncodeColor(invGameItem.color),
					"]",
					invGameItem.name,
					"[-]\n"
				});
				text = string.Concat(new string[]
				{
					text,
					"[AFAFAF]Level ",
					invGameItem.itemLevel.ToString(),
					" ",
					baseItem.slot.ToString()
				});
				List<InvStat> list = invGameItem.CalculateStats();
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					InvStat invStat = list[i];
					if (invStat.amount != 0)
					{
						if (invStat.amount < 0)
						{
							text = text + "\n[FF0000]" + invStat.amount.ToString();
						}
						else
						{
							text = text + "\n[00FF00]+" + invStat.amount.ToString();
						}
						if (invStat.modifier == InvStat.Modifier.Percent)
						{
							text += "%";
						}
						text = text + " " + invStat.id.ToString();
						text += "[-]";
					}
					i++;
				}
				if (!string.IsNullOrEmpty(baseItem.description))
				{
					text = text + "\n[FF9900]" + baseItem.description;
				}
				UITooltip.Show(text);
				return;
			}
		}
		UITooltip.Hide();
	}

	// Token: 0x06000134 RID: 308 RVA: 0x0000D7FE File Offset: 0x0000B9FE
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnClick()
	{
		if (UIItemSlot.mDraggedItem != null)
		{
			this.OnDrop(null);
			return;
		}
		if (this.mItem != null)
		{
			UIItemSlot.mDraggedItem = this.Replace(null);
			if (UIItemSlot.mDraggedItem != null)
			{
				NGUITools.PlaySound(this.grabSound);
			}
			this.UpdateCursor();
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x0000D83C File Offset: 0x0000BA3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDrag(Vector2 delta)
	{
		if (UIItemSlot.mDraggedItem == null && this.mItem != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			UIItemSlot.mDraggedItem = this.Replace(null);
			NGUITools.PlaySound(this.grabSound);
			this.UpdateCursor();
		}
	}

	// Token: 0x06000136 RID: 310 RVA: 0x0000D878 File Offset: 0x0000BA78
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnDrop(GameObject go)
	{
		InvGameItem invGameItem = this.Replace(UIItemSlot.mDraggedItem);
		if (UIItemSlot.mDraggedItem == invGameItem)
		{
			NGUITools.PlaySound(this.errorSound);
		}
		else if (invGameItem != null)
		{
			NGUITools.PlaySound(this.grabSound);
		}
		else
		{
			NGUITools.PlaySound(this.placeSound);
		}
		UIItemSlot.mDraggedItem = invGameItem;
		this.UpdateCursor();
	}

	// Token: 0x06000137 RID: 311 RVA: 0x0000D8D0 File Offset: 0x0000BAD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateCursor()
	{
		if (UIItemSlot.mDraggedItem != null && UIItemSlot.mDraggedItem.baseItem != null)
		{
			UICursor.Set(UIItemSlot.mDraggedItem.baseItem.iconAtlas as INGUIAtlas, UIItemSlot.mDraggedItem.baseItem.iconName);
			return;
		}
		UICursor.Clear();
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000D920 File Offset: 0x0000BB20
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		InvGameItem observedItem = this.observedItem;
		if (this.mItem != observedItem)
		{
			this.mItem = observedItem;
			InvBaseItem invBaseItem = (observedItem != null) ? observedItem.baseItem : null;
			if (this.label != null)
			{
				string text = (observedItem != null) ? observedItem.name : null;
				if (string.IsNullOrEmpty(this.mText))
				{
					this.mText = this.label.text;
				}
				this.label.text = ((text != null) ? text : this.mText);
			}
			if (this.icon != null)
			{
				if (invBaseItem == null || invBaseItem.iconAtlas == null)
				{
					this.icon.enabled = false;
				}
				else
				{
					this.icon.atlas = (invBaseItem.iconAtlas as INGUIAtlas);
					this.icon.spriteName = invBaseItem.iconName;
					this.icon.enabled = true;
					this.icon.MakePixelPerfect();
				}
			}
			if (this.background != null)
			{
				this.background.color = ((observedItem != null) ? observedItem.color : Color.white);
			}
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x0000DA38 File Offset: 0x0000BC38
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIItemSlot()
	{
	}

	// Token: 0x040001C3 RID: 451
	public UISprite icon;

	// Token: 0x040001C4 RID: 452
	public UIWidget background;

	// Token: 0x040001C5 RID: 453
	public UILabel label;

	// Token: 0x040001C6 RID: 454
	public AudioClip grabSound;

	// Token: 0x040001C7 RID: 455
	public AudioClip placeSound;

	// Token: 0x040001C8 RID: 456
	public AudioClip errorSound;

	// Token: 0x040001C9 RID: 457
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public InvGameItem mItem;

	// Token: 0x040001CA RID: 458
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string mText = "";

	// Token: 0x040001CB RID: 459
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static InvGameItem mDraggedItem;
}

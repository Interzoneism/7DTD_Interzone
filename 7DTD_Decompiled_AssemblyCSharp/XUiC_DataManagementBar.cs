using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C81 RID: 3201
[Preserve]
public class XUiC_DataManagementBar : XUiController
{
	// Token: 0x060062C1 RID: 25281 RVA: 0x002823B8 File Offset: 0x002805B8
	public override void Init()
	{
		base.Init();
		this.background = (XUiV_Sprite)base.GetChildById("background").ViewComponent;
		this.bar_used = (XUiV_Sprite)base.GetChildById("bar_used").ViewComponent;
		this.bar_selected_primary_fill = (XUiV_Sprite)base.GetChildById("bar_selected_primary_fill").ViewComponent;
		this.bar_selected_secondary_fill = (XUiV_Sprite)base.GetChildById("bar_selected_secondary_fill").ViewComponent;
		this.bar_selected_tertiary_fill = (XUiV_Sprite)base.GetChildById("bar_selected_tertiary_fill").ViewComponent;
		this.bar_selected_primary_outline = (XUiV_Sprite)base.GetChildById("bar_selected_primary_outline").ViewComponent;
		this.bar_selected_secondary_outline = (XUiV_Sprite)base.GetChildById("bar_selected_secondary_outline").ViewComponent;
		this.bar_selected_tertiary_outline = (XUiV_Sprite)base.GetChildById("bar_selected_tertiary_outline").ViewComponent;
		this.bar_hovered_outline = (XUiV_Sprite)base.GetChildById("bar_hovered_outline").ViewComponent;
		this.bar_required = (XUiV_Sprite)base.GetChildById("bar_required").ViewComponent;
		this.bar_pending = (XUiV_Sprite)base.GetChildById("bar_pending").ViewComponent;
		this.selectionFillColor = this.bar_selected_primary_fill.Color;
		this.fullWidth = this.background.Size.x;
		this.fullHeight = this.background.Size.y;
	}

	// Token: 0x060062C2 RID: 25282 RVA: 0x00282536 File Offset: 0x00280736
	public override void OnOpen()
	{
		base.OnOpen();
		this.IsDirty = true;
	}

	// Token: 0x060062C3 RID: 25283 RVA: 0x00282545 File Offset: 0x00280745
	public override void OnClose()
	{
		base.OnClose();
	}

	// Token: 0x060062C4 RID: 25284 RVA: 0x0028254D File Offset: 0x0028074D
	public void SetDisplayMode(XUiC_DataManagementBar.DisplayMode displayMode)
	{
		if (this.displayMode == displayMode)
		{
			return;
		}
		this.displayMode = displayMode;
		this.IsDirty = true;
	}

	// Token: 0x060062C5 RID: 25285 RVA: 0x00282567 File Offset: 0x00280767
	public void SetSelectedByteRegion(XUiC_DataManagementBar.BarRegion primaryRegion)
	{
		this.SetSelectedByteRegion(primaryRegion, XUiC_DataManagementBar.BarRegion.None, XUiC_DataManagementBar.BarRegion.None);
	}

	// Token: 0x060062C6 RID: 25286 RVA: 0x0028257A File Offset: 0x0028077A
	public void SetSelectedByteRegion(XUiC_DataManagementBar.BarRegion primaryRegion, XUiC_DataManagementBar.BarRegion secondaryRegion)
	{
		this.SetSelectedByteRegion(primaryRegion, secondaryRegion, XUiC_DataManagementBar.BarRegion.None);
	}

	// Token: 0x060062C7 RID: 25287 RVA: 0x0028258C File Offset: 0x0028078C
	public void SetSelectedByteRegion(XUiC_DataManagementBar.BarRegion primaryRegion, XUiC_DataManagementBar.BarRegion secondaryRegion, XUiC_DataManagementBar.BarRegion tertiaryRegion)
	{
		if (!this.primaryByteRegion.Equals(primaryRegion))
		{
			this.primaryByteRegion = primaryRegion;
			this.IsDirty = true;
		}
		if (!this.secondaryByteRegion.Equals(secondaryRegion))
		{
			this.secondaryByteRegion = secondaryRegion;
			this.IsDirty = true;
		}
		if (!this.tertiaryByteRegion.Equals(tertiaryRegion))
		{
			this.tertiaryByteRegion = tertiaryRegion;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062C8 RID: 25288 RVA: 0x002825ED File Offset: 0x002807ED
	public void SetHoveredByteRegion(XUiC_DataManagementBar.BarRegion region)
	{
		if (!this.hoveredByteRegion.Equals(region))
		{
			this.hoveredByteRegion = region;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062C9 RID: 25289 RVA: 0x0028260B File Offset: 0x0028080B
	public void SetArchivePreviewRegion(XUiC_DataManagementBar.BarRegion region)
	{
		if (!this.archivePreviewByteRegion.Equals(region))
		{
			this.archivePreviewByteRegion = region;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062CA RID: 25290 RVA: 0x00282629 File Offset: 0x00280829
	public void SetSelectionDepth(XUiC_DataManagementBar.SelectionDepth selectionDepth)
	{
		if (this.focusedSelectionDepth != selectionDepth)
		{
			this.focusedSelectionDepth = selectionDepth;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062CB RID: 25291 RVA: 0x00282642 File Offset: 0x00280842
	public void SetDeleteHovered(bool hovered)
	{
		if (this.deleteHovered != hovered)
		{
			this.deleteHovered = hovered;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062CC RID: 25292 RVA: 0x0028265B File Offset: 0x0028085B
	public void SetDeleteWindowDisplayed(bool displayed)
	{
		if (this.deleteWindowDisplayed != displayed)
		{
			this.deleteWindowDisplayed = displayed;
			this.IsDirty = true;
		}
	}

	// Token: 0x060062CD RID: 25293 RVA: 0x00282674 File Offset: 0x00280874
	public void SetUsedBytes(long usedBytes)
	{
		if (this.usedBytes == usedBytes)
		{
			return;
		}
		this.usedBytes = usedBytes;
		this.IsDirty = true;
	}

	// Token: 0x060062CE RID: 25294 RVA: 0x0028268E File Offset: 0x0028088E
	public void SetAllowanceBytes(long allowanceBytes)
	{
		if (this.allowanceBytes == allowanceBytes)
		{
			return;
		}
		this.allowanceBytes = allowanceBytes;
		this.bytesToPixels = (((float)allowanceBytes > 0f) ? ((float)this.fullWidth / (float)allowanceBytes) : 0f);
		this.IsDirty = true;
	}

	// Token: 0x060062CF RID: 25295 RVA: 0x002826C8 File Offset: 0x002808C8
	public void SetPendingBytes(long pendingBytes)
	{
		if (this.pendingBytes == pendingBytes)
		{
			return;
		}
		this.pendingBytes = pendingBytes;
		this.IsDirty = true;
	}

	// Token: 0x060062D0 RID: 25296 RVA: 0x002826E2 File Offset: 0x002808E2
	public long GetPendingBytes()
	{
		return this.pendingBytes;
	}

	// Token: 0x060062D1 RID: 25297 RVA: 0x002826EA File Offset: 0x002808EA
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty)
		{
			this.Refresh();
		}
	}

	// Token: 0x060062D2 RID: 25298 RVA: 0x00282704 File Offset: 0x00280904
	[PublicizedFrom(EAccessModifier.Private)]
	public void Refresh()
	{
		this.bar_used.IsVisible = false;
		this.bar_selected_primary_fill.IsVisible = false;
		this.bar_selected_secondary_fill.IsVisible = false;
		this.bar_selected_tertiary_fill.IsVisible = false;
		this.bar_selected_primary_outline.IsVisible = false;
		this.bar_selected_secondary_outline.IsVisible = false;
		this.bar_selected_tertiary_outline.IsVisible = false;
		this.bar_hovered_outline.IsVisible = false;
		this.bar_required.IsVisible = false;
		this.bar_pending.IsVisible = false;
		XUiC_DataManagementBar.DisplayMode displayMode = this.displayMode;
		if (displayMode != XUiC_DataManagementBar.DisplayMode.Selection && displayMode == XUiC_DataManagementBar.DisplayMode.Preview)
		{
			this.RefreshPreviewMode();
		}
		else
		{
			this.RefreshSelectionMode();
		}
		base.RefreshBindings(true);
		this.IsDirty = false;
	}

	// Token: 0x060062D3 RID: 25299 RVA: 0x002827B4 File Offset: 0x002809B4
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshPreviewMode()
	{
		if (this.allowanceBytes <= 0L || (this.usedBytes <= 0L && this.pendingBytes <= 0L))
		{
			return;
		}
		int num = Mathf.CeilToInt(this.bytesToPixels * (float)this.usedBytes);
		int num2 = 0;
		int num3 = 0;
		int num6;
		if (this.pendingBytes > 0L)
		{
			long num4 = this.allowanceBytes - this.usedBytes;
			if (num4 < this.pendingBytes)
			{
				num3 = this.fullWidth - num;
				long num5 = this.pendingBytes - num4;
				num2 = Math.Clamp(Mathf.CeilToInt(this.bytesToPixels * (float)num5), 3, num);
				num6 = num - num2;
			}
			else
			{
				num3 = Math.Clamp(Mathf.CeilToInt(this.bytesToPixels * (float)this.pendingBytes), 3, this.fullWidth - 3);
				num6 = Math.Min(num, this.fullWidth - num3);
			}
		}
		else
		{
			num6 = num;
		}
		if (num6 > 0)
		{
			this.bar_used.IsVisible = true;
			this.bar_used.Size = new Vector2i(num6, this.fullHeight);
		}
		if (num2 > 0)
		{
			this.bar_required.IsVisible = true;
			this.bar_required.Position = new Vector2i(num6, 0);
			this.bar_required.Size = new Vector2i(num2, this.fullHeight);
		}
		if (num3 > 0)
		{
			this.bar_pending.IsVisible = true;
			this.bar_pending.Position = new Vector2i(num6 + num2, 0);
			this.bar_pending.Size = new Vector2i(num3, this.fullHeight);
		}
	}

	// Token: 0x060062D4 RID: 25300 RVA: 0x00282920 File Offset: 0x00280B20
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshSelectionMode()
	{
		XUiC_DataManagementBar.<>c__DisplayClass52_0 CS$<>8__locals1;
		CS$<>8__locals1.<>4__this = this;
		if (this.allowanceBytes <= 0L || this.usedBytes <= 0L)
		{
			return;
		}
		int val = Mathf.CeilToInt(this.bytesToPixels * (float)this.usedBytes);
		this.bar_used.IsVisible = true;
		this.bar_used.Size = new Vector2i(Math.Max(val, 8), this.fullHeight);
		CS$<>8__locals1.maxPosition = this.fullWidth;
		this.<RefreshSelectionMode>g__UpdateRegion|52_1(this.primaryByteRegion, this.bar_selected_primary_fill, this.bar_selected_primary_outline, XUiC_DataManagementBar.SelectionDepth.Primary, ref CS$<>8__locals1);
		this.<RefreshSelectionMode>g__UpdateRegion|52_1(this.secondaryByteRegion, this.bar_selected_secondary_fill, this.bar_selected_secondary_outline, XUiC_DataManagementBar.SelectionDepth.Secondary, ref CS$<>8__locals1);
		this.<RefreshSelectionMode>g__UpdateRegion|52_1(this.tertiaryByteRegion, this.bar_selected_tertiary_fill, this.bar_selected_tertiary_outline, XUiC_DataManagementBar.SelectionDepth.Tertiary, ref CS$<>8__locals1);
	}

	// Token: 0x060062D5 RID: 25301 RVA: 0x002829E8 File Offset: 0x00280BE8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (!(_bindingName == "warningtext"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		long num = this.allowanceBytes - this.usedBytes;
		if (this.displayMode == XUiC_DataManagementBar.DisplayMode.Selection || num >= this.pendingBytes)
		{
			_value = string.Empty;
			return true;
		}
		long bytes = this.pendingBytes - num;
		_value = string.Format(Localization.Get("xuiDmBarRequiredSpaceWarning", false), XUiC_DataManagement.FormatMemoryString(bytes));
		return true;
	}

	// Token: 0x060062D8 RID: 25304 RVA: 0x00282AF2 File Offset: 0x00280CF2
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <RefreshSelectionMode>g__GetPixelValues|52_0(XUiC_DataManagementBar.BarRegion byteRegion, int maxPosition, out int position, out int width, ref XUiC_DataManagementBar.<>c__DisplayClass52_0 A_5)
	{
		position = Mathf.Min(Mathf.FloorToInt(this.bytesToPixels * (float)byteRegion.Start), maxPosition - 8);
		width = Mathf.Max(Mathf.CeilToInt(this.bytesToPixels * (float)byteRegion.Size), 8);
	}

	// Token: 0x060062D9 RID: 25305 RVA: 0x00282B30 File Offset: 0x00280D30
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Private)]
	public void <RefreshSelectionMode>g__UpdateRegion|52_1(XUiC_DataManagementBar.BarRegion byteRegion, XUiV_Sprite fillSprite, XUiV_Sprite outlineSprite, XUiC_DataManagementBar.SelectionDepth depth, ref XUiC_DataManagementBar.<>c__DisplayClass52_0 A_5)
	{
		if (this.focusedSelectionDepth == depth && this.hoveredByteRegion.Size > 0L)
		{
			int x;
			int x2;
			this.<RefreshSelectionMode>g__GetPixelValues|52_0(this.hoveredByteRegion, A_5.maxPosition, out x, out x2, ref A_5);
			this.bar_hovered_outline.IsVisible = true;
			this.bar_hovered_outline.Position = new Vector2i(x, 0);
			this.bar_hovered_outline.Size = new Vector2i(x2, this.fullHeight);
		}
		if (byteRegion.Size <= 0L)
		{
			return;
		}
		int num;
		int num2;
		this.<RefreshSelectionMode>g__GetPixelValues|52_0(byteRegion, A_5.maxPosition, out num, out num2, ref A_5);
		fillSprite.IsVisible = true;
		fillSprite.Position = new Vector2i(num, 0);
		fillSprite.Size = new Vector2i(num2, this.fullHeight);
		float num3 = ((this.focusedSelectionDepth == XUiC_DataManagementBar.SelectionDepth.Secondary) ? 1f : 0.5f) * (float)(this.focusedSelectionDepth - depth);
		Color color = this.selectionFillColor;
		color = Color.Lerp(color, Color.white, 0.5f * Mathf.Abs(num3));
		bool flag = this.deleteHovered || this.deleteWindowDisplayed;
		if (num3 > 0f)
		{
			color.a = Mathf.Lerp(color.a, 0.5f, num3);
		}
		else if (flag)
		{
			color = Color.Lerp(color, XUiC_DataManagementBar.selectionOutlineColorDelete, 0.2f);
		}
		fillSprite.Color = color;
		if (!flag || num3 >= 0f)
		{
			outlineSprite.IsVisible = true;
			outlineSprite.Position = fillSprite.Position;
			outlineSprite.Size = fillSprite.Size;
			Color color2;
			if (this.focusedSelectionDepth == depth)
			{
				color2 = (flag ? XUiC_DataManagementBar.selectionOutlineColorDelete : XUiC_DataManagementBar.selectionOutlineColor);
			}
			else
			{
				color2 = XUiC_DataManagementBar.selectionOutlineColorFade;
			}
			outlineSprite.Color = color2;
		}
		if (this.focusedSelectionDepth == depth && this.archivePreviewByteRegion.Size > 0L)
		{
			int x3;
			int x4;
			this.<RefreshSelectionMode>g__GetPixelValues|52_0(this.archivePreviewByteRegion, A_5.maxPosition, out x3, out x4, ref A_5);
			this.bar_pending.IsVisible = true;
			this.bar_pending.Position = new Vector2i(x3, 0);
			this.bar_pending.Size = new Vector2i(x4, this.fullHeight);
		}
		A_5.maxPosition = num + num2;
	}

	// Token: 0x04004A62 RID: 19042
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x04004A63 RID: 19043
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_used;

	// Token: 0x04004A64 RID: 19044
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_required;

	// Token: 0x04004A65 RID: 19045
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_pending;

	// Token: 0x04004A66 RID: 19046
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_primary_fill;

	// Token: 0x04004A67 RID: 19047
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_secondary_fill;

	// Token: 0x04004A68 RID: 19048
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_tertiary_fill;

	// Token: 0x04004A69 RID: 19049
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_primary_outline;

	// Token: 0x04004A6A RID: 19050
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_secondary_outline;

	// Token: 0x04004A6B RID: 19051
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_selected_tertiary_outline;

	// Token: 0x04004A6C RID: 19052
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bar_hovered_outline;

	// Token: 0x04004A6D RID: 19053
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.DisplayMode displayMode;

	// Token: 0x04004A6E RID: 19054
	[PublicizedFrom(EAccessModifier.Private)]
	public long usedBytes;

	// Token: 0x04004A6F RID: 19055
	[PublicizedFrom(EAccessModifier.Private)]
	public long pendingBytes;

	// Token: 0x04004A70 RID: 19056
	[PublicizedFrom(EAccessModifier.Private)]
	public long allowanceBytes;

	// Token: 0x04004A71 RID: 19057
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.BarRegion primaryByteRegion = XUiC_DataManagementBar.BarRegion.None;

	// Token: 0x04004A72 RID: 19058
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.BarRegion secondaryByteRegion = XUiC_DataManagementBar.BarRegion.None;

	// Token: 0x04004A73 RID: 19059
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.BarRegion tertiaryByteRegion = XUiC_DataManagementBar.BarRegion.None;

	// Token: 0x04004A74 RID: 19060
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.BarRegion hoveredByteRegion = XUiC_DataManagementBar.BarRegion.None;

	// Token: 0x04004A75 RID: 19061
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.BarRegion archivePreviewByteRegion = XUiC_DataManagementBar.BarRegion.None;

	// Token: 0x04004A76 RID: 19062
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_DataManagementBar.SelectionDepth focusedSelectionDepth;

	// Token: 0x04004A77 RID: 19063
	[PublicizedFrom(EAccessModifier.Private)]
	public bool deleteHovered;

	// Token: 0x04004A78 RID: 19064
	[PublicizedFrom(EAccessModifier.Private)]
	public bool deleteWindowDisplayed;

	// Token: 0x04004A79 RID: 19065
	[PublicizedFrom(EAccessModifier.Private)]
	public int fullWidth;

	// Token: 0x04004A7A RID: 19066
	[PublicizedFrom(EAccessModifier.Private)]
	public int fullHeight;

	// Token: 0x04004A7B RID: 19067
	[PublicizedFrom(EAccessModifier.Private)]
	public float bytesToPixels;

	// Token: 0x04004A7C RID: 19068
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color32 selectionOutlineColor = new Color32(250, byte.MaxValue, 163, 193);

	// Token: 0x04004A7D RID: 19069
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color32 selectionOutlineColorFade = new Color32(250, byte.MaxValue, 163, 86);

	// Token: 0x04004A7E RID: 19070
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Color32 selectionOutlineColorDelete = new Color32(234, 67, 53, byte.MaxValue);

	// Token: 0x04004A7F RID: 19071
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectionFillColor;

	// Token: 0x02000C82 RID: 3202
	public enum SelectionDepth
	{
		// Token: 0x04004A81 RID: 19073
		Primary,
		// Token: 0x04004A82 RID: 19074
		Secondary,
		// Token: 0x04004A83 RID: 19075
		Tertiary
	}

	// Token: 0x02000C83 RID: 3203
	public struct BarRegion : IEquatable<XUiC_DataManagementBar.BarRegion>
	{
		// Token: 0x060062DA RID: 25306 RVA: 0x00282D5B File Offset: 0x00280F5B
		public BarRegion(long offset, long size)
		{
			this.Start = offset;
			this.Size = size;
			this.End = this.Start + this.Size;
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x00282D7E File Offset: 0x00280F7E
		public bool Equals(XUiC_DataManagementBar.BarRegion other)
		{
			return this.Start == other.Start && this.Size == other.Size;
		}

		// Token: 0x04004A84 RID: 19076
		public readonly long Start;

		// Token: 0x04004A85 RID: 19077
		public readonly long Size;

		// Token: 0x04004A86 RID: 19078
		public readonly long End;

		// Token: 0x04004A87 RID: 19079
		public static readonly XUiC_DataManagementBar.BarRegion None = new XUiC_DataManagementBar.BarRegion(0L, 0L);
	}

	// Token: 0x02000C84 RID: 3204
	public enum DisplayMode
	{
		// Token: 0x04004A89 RID: 19081
		Selection,
		// Token: 0x04004A8A RID: 19082
		Preview
	}
}

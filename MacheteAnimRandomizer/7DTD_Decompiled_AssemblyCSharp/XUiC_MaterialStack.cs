using System;
using InControl;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D28 RID: 3368
[UnityEngine.Scripting.Preserve]
public class XUiC_MaterialStack : XUiC_SelectableEntry
{
	// Token: 0x17000AAA RID: 2730
	// (get) Token: 0x060068D2 RID: 26834 RVA: 0x002A9568 File Offset: 0x002A7768
	// (set) Token: 0x060068D3 RID: 26835 RVA: 0x002A9570 File Offset: 0x002A7770
	public bool IsLocked { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x17000AAB RID: 2731
	// (get) Token: 0x060068D4 RID: 26836 RVA: 0x002A9579 File Offset: 0x002A7779
	// (set) Token: 0x060068D5 RID: 26837 RVA: 0x002A9584 File Offset: 0x002A7784
	public BlockTextureData TextureData
	{
		get
		{
			return this.textureData;
		}
		set
		{
			this.textMaterial.IsVisible = false;
			base.ViewComponent.Enabled = (value != null);
			if (this.textureData != value)
			{
				this.textureData = value;
				this.isDirty = true;
				if (this.textureData == null)
				{
					this.SetItemNameText("");
					this.IsLocked = false;
				}
				else
				{
					this.textMaterial.IsVisible = true;
					MeshDescription meshDescription = MeshDescription.meshes[0];
					int textureID = (int)this.textureData.TextureID;
					Rect uvrect;
					if (textureID == 0)
					{
						uvrect = WorldConstants.uvRectZero;
					}
					else
					{
						uvrect = meshDescription.textureAtlas.uvMapping[textureID].uv;
					}
					this.textMaterial.Texture = meshDescription.textureAtlas.diffuseTexture;
					if (meshDescription.bTextureArray)
					{
						this.textMaterial.Material.SetTexture("_BumpMap", meshDescription.textureAtlas.normalTexture);
						this.textMaterial.Material.SetFloat("_Index", (float)meshDescription.textureAtlas.uvMapping[textureID].index);
						this.textMaterial.Material.SetFloat("_Size", (float)meshDescription.textureAtlas.uvMapping[textureID].blockW);
					}
					else
					{
						this.textMaterial.UVRect = uvrect;
					}
					this.SetItemNameText(string.Format("({0}) {1}", this.textureData.ID, this.textureData.LocalizedName));
				}
			}
			if (this.textureData != null)
			{
				if (!(this.textureData.LockedByPerk != ""))
				{
					this.IsLocked = false;
				}
				this.textMaterial.IsVisible = true;
			}
			base.RefreshBindings(false);
		}
	}

	// Token: 0x17000AAC RID: 2732
	// (get) Token: 0x060068D6 RID: 26838 RVA: 0x002A972F File Offset: 0x002A792F
	// (set) Token: 0x060068D7 RID: 26839 RVA: 0x002A9737 File Offset: 0x002A7937
	public XUiC_MaterialInfoWindow InfoWindow { get; set; }

	// Token: 0x060068D8 RID: 26840 RVA: 0x002A9740 File Offset: 0x002A7940
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void SelectedChanged(bool isSelected)
	{
		if (isSelected)
		{
			this.SetColor(this.selectColor);
			if (base.xui.currentSelectedEntry == this)
			{
				this.InfoWindow.SetMaterial(this.textureData);
				return;
			}
		}
		else
		{
			this.SetColor(XUiC_MaterialStack.backgroundColor);
		}
	}

	// Token: 0x060068D9 RID: 26841 RVA: 0x002A977C File Offset: 0x002A797C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetColor(Color32 color)
	{
		this.background.Color = color;
	}

	// Token: 0x060068DA RID: 26842 RVA: 0x002A9790 File Offset: 0x002A7990
	public override void Init()
	{
		base.Init();
		this.tintedOverlay = base.GetChildById("tintedOverlay");
		this.highlightOverlay = (base.GetChildById("highlightOverlay").ViewComponent as XUiV_Sprite);
		this.background = (base.GetChildById("background").ViewComponent as XUiV_Sprite);
		this.textMaterial = (base.GetChildById("textMaterial").ViewComponent as XUiV_Texture);
		this.textMaterial.CreateMaterial();
	}

	// Token: 0x060068DB RID: 26843 RVA: 0x002A9810 File Offset: 0x002A7A10
	public override void Update(float _dt)
	{
		base.Update(_dt);
		PlayerActionsGUI guiactions = base.xui.playerUI.playerInput.GUIActions;
		if (base.WindowGroup.isShowing)
		{
			CursorControllerAbs cursorController = base.xui.playerUI.CursorController;
			cursorController.GetScreenPosition();
			bool mouseButtonUp = cursorController.GetMouseButtonUp(UICamera.MouseButton.LeftButton);
			cursorController.GetMouseButtonDown(UICamera.MouseButton.LeftButton);
			cursorController.GetMouseButton(UICamera.MouseButton.LeftButton);
			cursorController.GetMouseButtonUp(UICamera.MouseButton.RightButton);
			cursorController.GetMouseButtonDown(UICamera.MouseButton.RightButton);
			cursorController.GetMouseButton(UICamera.MouseButton.RightButton);
			if (this.isOver && UICamera.hoveredObject == base.ViewComponent.UiTransform.gameObject && base.ViewComponent.EventOnPress)
			{
				if (guiactions.LastInputType == BindingSourceType.DeviceBindingSource)
				{
					bool wasReleased = guiactions.Submit.WasReleased;
					bool wasReleased2 = guiactions.HalfStack.WasReleased;
					bool wasReleased3 = guiactions.Inspect.WasReleased;
					bool wasReleased4 = guiactions.RightStick.WasReleased;
					if (wasReleased && this.textureData != null)
					{
						this.SetSelectedTextureForItem();
					}
				}
				else if (mouseButtonUp && this.textureData != null)
				{
					this.SetSelectedTextureForItem();
				}
			}
			else
			{
				this.currentColor = XUiC_MaterialStack.backgroundColor;
				if (this.highlightOverlay != null)
				{
					this.highlightOverlay.Color = XUiC_MaterialStack.backgroundColor;
				}
				if (!base.Selected)
				{
					this.background.Color = this.currentColor;
				}
				this.lastClicked = false;
				if (this.isOver)
				{
					this.isOver = false;
				}
			}
		}
		if (this.isDirty)
		{
			this.isDirty = false;
		}
	}

	// Token: 0x060068DC RID: 26844 RVA: 0x002A9994 File Offset: 0x002A7B94
	public void SetSelectedTextureForItem()
	{
		if (!this.IsLocked)
		{
			if (base.xui.playerUI.entityPlayer.inventory.holdingItem is ItemClassBlock)
			{
				base.xui.playerUI.entityPlayer.inventory.holdingItemItemValue.TextureFullArray[0] = Chunk.TextureIdxToTextureFullValue64(this.textureData.ID);
			}
			else
			{
				((ItemActionTextureBlock.ItemActionTextureBlockData)base.xui.playerUI.entityPlayer.inventory.holdingItemData.actionData[1]).idx = this.textureData.ID;
				base.xui.playerUI.entityPlayer.inventory.holdingItemItemValue.Meta = (int)((byte)this.textureData.ID);
				base.xui.playerUI.entityPlayer.inventory.holdingItemData.actionData[1].invData.itemValue = base.xui.playerUI.entityPlayer.inventory.holdingItemItemValue;
			}
		}
		base.Selected = true;
	}

	// Token: 0x060068DD RID: 26845 RVA: 0x002A9ABC File Offset: 0x002A7CBC
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleItemInspect()
	{
		if (this.textureData != null && this.InfoWindow != null)
		{
			base.Selected = true;
		}
	}

	// Token: 0x060068DE RID: 26846 RVA: 0x002A9AD5 File Offset: 0x002A7CD5
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetItemNameText(string name)
	{
		this.viewComponent.ToolTip = ((this.textureData != null) ? name : string.Empty);
	}

	// Token: 0x060068DF RID: 26847 RVA: 0x002A9AF4 File Offset: 0x002A7CF4
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void OnHovered(bool _isOver)
	{
		this.isOver = _isOver;
		if (!base.Selected)
		{
			if (_isOver)
			{
				this.background.Color = XUiC_MaterialStack.highlightColor;
			}
			else
			{
				this.background.Color = XUiC_MaterialStack.backgroundColor;
			}
		}
		base.OnHovered(_isOver);
	}

	// Token: 0x060068E0 RID: 26848 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x060068E1 RID: 26849 RVA: 0x002A9B46 File Offset: 0x002A7D46
	public void ClearSelectedInfoWindow()
	{
		if (base.Selected)
		{
			this.InfoWindow.SetMaterial(null);
		}
	}

	// Token: 0x060068E2 RID: 26850 RVA: 0x002A9B5C File Offset: 0x002A7D5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "islocked")
		{
			value = this.IsLocked.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x060068E3 RID: 26851 RVA: 0x002A9B8C File Offset: 0x002A7D8C
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "select_color"))
			{
				if (!(name == "press_color"))
				{
					if (!(name == "background_color"))
					{
						if (!(name == "highlight_color"))
						{
							if (!(name == "select_sound"))
							{
								return false;
							}
							base.xui.LoadData<AudioClip>(value, delegate(AudioClip o)
							{
								this.selectSound = o;
							});
						}
						else
						{
							XUiC_MaterialStack.highlightColor = StringParsers.ParseColor32(value);
						}
					}
					else
					{
						XUiC_MaterialStack.backgroundColor = StringParsers.ParseColor32(value);
					}
				}
				else
				{
					this.pressColor = StringParsers.ParseColor32(value);
				}
			}
			else
			{
				this.selectColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04004F0D RID: 20237
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty = true;

	// Token: 0x04004F0E RID: 20238
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bHighlightEnabled;

	// Token: 0x04004F0F RID: 20239
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bDropEnabled = true;

	// Token: 0x04004F10 RID: 20240
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip selectSound;

	// Token: 0x04004F11 RID: 20241
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip placeSound;

	// Token: 0x04004F12 RID: 20242
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isOver;

	// Token: 0x04004F13 RID: 20243
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 currentColor;

	// Token: 0x04004F14 RID: 20244
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 selectColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004F15 RID: 20245
	[PublicizedFrom(EAccessModifier.Private)]
	public Color32 pressColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	// Token: 0x04004F16 RID: 20246
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lastClicked;

	// Token: 0x04004F17 RID: 20247
	public static Color32 backgroundColor = new Color32(96, 96, 96, byte.MaxValue);

	// Token: 0x04004F18 RID: 20248
	public static Color32 highlightColor = new Color32(222, 206, 163, byte.MaxValue);

	// Token: 0x04004F19 RID: 20249
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController tintedOverlay;

	// Token: 0x04004F1A RID: 20250
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label stackValue;

	// Token: 0x04004F1B RID: 20251
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite highlightOverlay;

	// Token: 0x04004F1C RID: 20252
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite background;

	// Token: 0x04004F1D RID: 20253
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture textMaterial;

	// Token: 0x04004F1F RID: 20255
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTextureData textureData;

	// Token: 0x04004F21 RID: 20257
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector3 startMousePos = Vector3.zero;
}

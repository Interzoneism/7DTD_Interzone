using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000C2F RID: 3119
[Preserve]
public class XUiC_CharacterCosmeticWindow : XUiController
{
	// Token: 0x06005FF8 RID: 24568 RVA: 0x0026EC24 File Offset: 0x0026CE24
	public override void Init()
	{
		base.Init();
		this.previewFrame = base.GetChildById("playerPreviewSDCS");
		this.previewFrame.OnPress += this.PreviewFrame_OnPress;
		this.previewFrame.OnHover += this.PreviewFrame_OnHover;
		this.lblName = (XUiV_Label)base.GetChildById("characterName").ViewComponent;
		this.textPreview = (XUiV_Texture)base.GetChildById("playerPreviewSDCS").ViewComponent;
		this.isDirty = true;
		XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment;
		base.GetChildByType<XUiC_EquipmentStackGrid>();
		base.xui.playerUI.OnUIShutdown += this.HandleUIShutdown;
		base.xui.OnShutdown += this.HandleUIShutdown;
	}

	// Token: 0x06005FF9 RID: 24569 RVA: 0x0026ED00 File Offset: 0x0026CF00
	[PublicizedFrom(EAccessModifier.Private)]
	public void HandleUIShutdown()
	{
		base.xui.playerUI.OnUIShutdown -= this.HandleUIShutdown;
		base.xui.OnShutdown -= this.HandleUIShutdown;
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment;
	}

	// Token: 0x06005FFA RID: 24570 RVA: 0x0026ED51 File Offset: 0x0026CF51
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviewFrame_OnHover(XUiController _sender, bool _isOver)
	{
		this.renderTextureSystem.RotateTarget(Time.deltaTime * 10f);
	}

	// Token: 0x06005FFB RID: 24571 RVA: 0x0026ED6C File Offset: 0x0026CF6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void PreviewFrame_OnPress(XUiController _sender, int _mouseButton)
	{
		if (base.xui.dragAndDrop.CurrentStack != ItemStack.Empty)
		{
			ItemStack itemStack = base.xui.PlayerEquipment.EquipItem(base.xui.dragAndDrop.CurrentStack);
			if (base.xui.dragAndDrop.CurrentStack != itemStack)
			{
				base.xui.dragAndDrop.CurrentStack = itemStack;
				base.xui.dragAndDrop.PickUpType = XUiC_ItemStack.StackLocationTypes.Equipment;
			}
		}
	}

	// Token: 0x06005FFC RID: 24572 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment(XUiM_PlayerEquipment _playerEquipment)
	{
	}

	// Token: 0x06005FFD RID: 24573 RVA: 0x0026EDE8 File Offset: 0x0026CFE8
	public override void Update(float _dt)
	{
		if (GameManager.Instance == null || GameManager.Instance.World == null)
		{
			return;
		}
		if (this.ep == null)
		{
			this.ep = base.xui.playerUI.entityPlayer;
		}
		if (this.isDirty)
		{
			if (this.player == null)
			{
				return;
			}
			this.lblName.Text = this.player.PlayerDisplayName;
			this.isDirty = false;
			base.RefreshBindings(false);
		}
		if (this.isPreviewDirty)
		{
			this.MakePreview();
		}
		this.textPreview.Texture = this.renderTextureSystem.RenderTex;
		if (this.previewSDCSObj != null)
		{
			this.previewSDCSObj.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
		}
		base.Update(_dt);
	}

	// Token: 0x06005FFE RID: 24574 RVA: 0x0026EECC File Offset: 0x0026D0CC
	public override void OnOpen()
	{
		base.OnOpen();
		this.isDirty = true;
		this.isPreviewDirty = true;
		this.player = base.xui.playerUI.entityPlayer;
		if (this.previewFrame != null)
		{
			this.previewFrame.OnPress -= this.PreviewFrame_OnPress;
			this.previewFrame.OnHover -= this.PreviewFrame_OnHover;
		}
		this.previewFrame = base.GetChildById("previewFrameSDCS");
		this.previewFrame.OnPress += this.PreviewFrame_OnPress;
		this.previewFrame.OnHover += this.PreviewFrame_OnHover;
		this.textPreview = (XUiV_Texture)base.GetChildById("playerPreviewSDCS").ViewComponent;
		if (this.renderTextureSystem.ParentGO == null)
		{
			this.renderTextureSystem.Create("playerpreview", new GameObject(), new Vector3(0f, -0.5f, 3f), new Vector3(0f, -0.2f, 7.5f), this.textPreview.Size, true, false, 1f);
		}
		if (this.player as EntityPlayerLocal != null && this.player.emodel as EModelSDCS != null)
		{
			XUiM_PlayerEquipment.HandleRefreshEquipment += this.XUiM_PlayerEquipment_HandleRefreshEquipment1;
		}
	}

	// Token: 0x06005FFF RID: 24575 RVA: 0x0026F031 File Offset: 0x0026D231
	[PublicizedFrom(EAccessModifier.Private)]
	public void XUiM_PlayerEquipment_HandleRefreshEquipment1(XUiM_PlayerEquipment playerEquipment)
	{
		if (!base.IsOpen)
		{
			return;
		}
		this.MakePreview();
	}

	// Token: 0x06006000 RID: 24576 RVA: 0x0026F042 File Offset: 0x0026D242
	public override void OnClose()
	{
		base.OnClose();
		XUiM_PlayerEquipment.HandleRefreshEquipment -= this.XUiM_PlayerEquipment_HandleRefreshEquipment1;
		SDCSUtils.DestroyViz(this.previewSDCSObj, false);
		this.renderTextureSystem.Cleanup();
	}

	// Token: 0x06006001 RID: 24577 RVA: 0x0026F074 File Offset: 0x0026D274
	public void MakePreview()
	{
		if (this.ep == null)
		{
			return;
		}
		if (this.ep.emodel == null)
		{
			return;
		}
		EModelSDCS emodelSDCS = this.ep.emodel as EModelSDCS;
		if (emodelSDCS != null)
		{
			this.isPreviewDirty = false;
			SDCSUtils.CreateVizUI(emodelSDCS.Archetype, ref this.previewSDCSObj, ref this.transformCatalog, this.ep, true);
			Utils.SetLayerRecursively(this.previewSDCSObj, 11);
			Transform transform = this.previewSDCSObj.transform;
			transform.SetParent(this.renderTextureSystem.ParentGO.transform, false);
			transform.localPosition = new Vector3(0.022f, -2.9f, 12f);
			transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.renderTextureSystem.SetOrtho(true, 0.95f);
		}
	}

	// Token: 0x0400484B RID: 18507
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Label lblName;

	// Token: 0x0400484C RID: 18508
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiController previewFrame;

	// Token: 0x0400484D RID: 18509
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture textPreview;

	// Token: 0x0400484E RID: 18510
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer ep;

	// Token: 0x0400484F RID: 18511
	[PublicizedFrom(EAccessModifier.Private)]
	public Camera cam;

	// Token: 0x04004850 RID: 18512
	public RuntimeAnimatorController animationController;

	// Token: 0x04004851 RID: 18513
	public float atlasResolutionScale;

	// Token: 0x04004852 RID: 18514
	[PublicizedFrom(EAccessModifier.Private)]
	public RenderTextureSystem renderTextureSystem = new RenderTextureSystem();

	// Token: 0x04004853 RID: 18515
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isDirty;

	// Token: 0x04004854 RID: 18516
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isPreviewDirty;

	// Token: 0x04004855 RID: 18517
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayer player;

	// Token: 0x04004856 RID: 18518
	[PublicizedFrom(EAccessModifier.Private)]
	public float updateTime;

	// Token: 0x04004857 RID: 18519
	[PublicizedFrom(EAccessModifier.Private)]
	public GameObject previewSDCSObj;

	// Token: 0x04004858 RID: 18520
	[PublicizedFrom(EAccessModifier.Private)]
	public SDCSUtils.TransformCatalog transformCatalog;
}

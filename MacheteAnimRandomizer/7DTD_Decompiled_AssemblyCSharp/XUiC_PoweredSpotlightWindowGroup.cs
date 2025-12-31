using System;
using UnityEngine.Scripting;

// Token: 0x02000D86 RID: 3462
[Preserve]
public class XUiC_PoweredSpotlightWindowGroup : XUiC_PoweredGenericWindowGroup
{
	// Token: 0x06006C35 RID: 27701 RVA: 0x002C3CD0 File Offset: 0x002C1ED0
	public override void Init()
	{
		base.Init();
		XUiController xuiController = base.GetChildByType<XUiC_WindowNonPagingHeader>();
		if (xuiController != null)
		{
			this.nonPagingHeader = (XUiC_WindowNonPagingHeader)xuiController;
		}
		xuiController = base.GetChildById("windowPowerCameraControlPreview");
		if (xuiController != null)
		{
			this.cameraWindowPreview = (XUiC_CameraWindow)xuiController;
			this.cameraWindowPreview.Owner = this;
			this.cameraWindowPreview.UseEdgeDetection = false;
		}
	}

	// Token: 0x06006C36 RID: 27702 RVA: 0x002C3D2C File Offset: 0x002C1F2C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void setupWindowTileEntities()
	{
		base.setupWindowTileEntities();
		if (this.cameraWindowPreview != null)
		{
			this.cameraWindowPreview.TileEntity = this.tileEntity;
		}
	}

	// Token: 0x06006C37 RID: 27703 RVA: 0x002C3D50 File Offset: 0x002C1F50
	public override void OnOpen()
	{
		base.OnOpen();
		if (this.nonPagingHeader != null)
		{
			World world = GameManager.Instance.World;
			string localizedBlockName = this.tileEntity.GetChunk().GetBlock(this.tileEntity.localChunkPos).Block.GetLocalizedBlockName();
			this.nonPagingHeader.SetHeader(localizedBlockName);
		}
		base.TileEntity.Destroyed += this.TileEntity_Destroyed;
	}

	// Token: 0x06006C38 RID: 27704 RVA: 0x002C3DC8 File Offset: 0x002C1FC8
	public override void OnClose()
	{
		base.TileEntity.Destroyed -= this.TileEntity_Destroyed;
		base.OnClose();
	}

	// Token: 0x06006C39 RID: 27705 RVA: 0x002C3DE8 File Offset: 0x002C1FE8
	[PublicizedFrom(EAccessModifier.Private)]
	public void TileEntity_Destroyed(ITileEntity te)
	{
		if (base.TileEntity == te)
		{
			if (GameManager.Instance != null)
			{
				base.xui.playerUI.windowManager.Close("powerspotlight");
				base.xui.playerUI.windowManager.Close("powercamera");
				return;
			}
		}
		else
		{
			te.Destroyed -= this.TileEntity_Destroyed;
		}
	}

	// Token: 0x04005253 RID: 21075
	public static string ID = "powerspotlight";

	// Token: 0x04005254 RID: 21076
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_WindowNonPagingHeader nonPagingHeader;

	// Token: 0x04005255 RID: 21077
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CameraWindow cameraWindowPreview;
}

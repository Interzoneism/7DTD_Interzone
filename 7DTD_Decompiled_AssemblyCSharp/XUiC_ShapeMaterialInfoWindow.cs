using System;
using UnityEngine.Scripting;

// Token: 0x02000E24 RID: 3620
[Preserve]
public class XUiC_ShapeMaterialInfoWindow : XUiController
{
	// Token: 0x06007153 RID: 29011 RVA: 0x002E2C6C File Offset: 0x002E0E6C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("backgroundTexture");
		if (childById != null)
		{
			this.backgroundTexture = (XUiV_Texture)childById.ViewComponent;
			this.backgroundTexture.CreateMaterial();
		}
		XUiController childById2 = base.GetChildById("btnDowngrade");
		XUiController childById3 = base.GetChildById("btnUpgrade");
		if (childById2 != null)
		{
			childById2.OnPress += this.BtnDowngrade_OnPress;
		}
		if (childById3 != null)
		{
			childById3.OnPress += this.BtnUpgrade_OnPress;
		}
	}

	// Token: 0x06007154 RID: 29012 RVA: 0x002842C8 File Offset: 0x002824C8
	public override void OnOpen()
	{
		base.OnOpen();
		base.RefreshBindings(false);
	}

	// Token: 0x06007155 RID: 29013 RVA: 0x002E2CEC File Offset: 0x002E0EEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnUpgrade_OnPress(XUiController _sender, int _mouseButton)
	{
		this.windowGroup.Controller.GetChildByType<XUiC_ShapesWindow>().UpgradeDowngradeShapes(this.blockData.UpgradeBlock);
	}

	// Token: 0x06007156 RID: 29014 RVA: 0x002E2D0E File Offset: 0x002E0F0E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnDowngrade_OnPress(XUiController _sender, int _mouseButton)
	{
		this.windowGroup.Controller.GetChildByType<XUiC_ShapesWindow>().UpgradeDowngradeShapes(this.blockData.DowngradeBlock);
	}

	// Token: 0x06007157 RID: 29015 RVA: 0x002A9093 File Offset: 0x002A7293
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty && base.ViewComponent.IsVisible)
		{
			this.IsDirty = false;
		}
	}

	// Token: 0x06007158 RID: 29016 RVA: 0x002E2D30 File Offset: 0x002E0F30
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		if (_bindingName == "hasmaterial")
		{
			_value = (!string.IsNullOrEmpty(this.materialName)).ToString();
			return true;
		}
		if (_bindingName == "materialname")
		{
			_value = (this.materialName ?? "");
			return true;
		}
		if (_bindingName == "has_upgrade")
		{
			_value = (!string.IsNullOrEmpty(this.upgradeMaterial)).ToString();
			return true;
		}
		if (_bindingName == "upgrade_material")
		{
			_value = (this.upgradeMaterial ?? "");
			return true;
		}
		if (_bindingName == "has_downgrade")
		{
			_value = (!string.IsNullOrEmpty(this.downgradeMaterial)).ToString();
			return true;
		}
		if (!(_bindingName == "downgrade_material"))
		{
			return base.GetBindingValueInternal(ref _value, _bindingName);
		}
		_value = (this.downgradeMaterial ?? "");
		return true;
	}

	// Token: 0x06007159 RID: 29017 RVA: 0x002E2E20 File Offset: 0x002E1020
	public void SetShape(Block _newBlockData)
	{
		this.backgroundTexture.IsVisible = false;
		this.blockData = _newBlockData;
		this.materialName = null;
		this.upgradeMaterial = null;
		this.downgradeMaterial = null;
		if (_newBlockData != null && _newBlockData.GetAutoShapeType() != EAutoShapeType.None)
		{
			this.materialName = _newBlockData.blockMaterial.GetLocalizedMaterialName();
			if (!_newBlockData.UpgradeBlock.isair)
			{
				this.upgradeMaterial = _newBlockData.UpgradeBlock.Block.blockMaterial.GetLocalizedMaterialName();
			}
			if (!_newBlockData.DowngradeBlock.isair)
			{
				this.downgradeMaterial = _newBlockData.DowngradeBlock.Block.blockMaterial.GetLocalizedMaterialName();
			}
			if (this.backgroundTexture != null)
			{
				int sideTextureId = _newBlockData.GetSideTextureId(new BlockValue((uint)_newBlockData.blockID), BlockFace.Top, 0);
				if (sideTextureId != 0)
				{
					MeshDescription meshDescription = MeshDescription.meshes[0];
					UVRectTiling uvrectTiling = meshDescription.textureAtlas.uvMapping[sideTextureId];
					this.backgroundTexture.Texture = meshDescription.textureAtlas.diffuseTexture;
					if (meshDescription.bTextureArray)
					{
						this.backgroundTexture.Material.SetTexture("_BumpMap", meshDescription.textureAtlas.normalTexture);
						this.backgroundTexture.Material.SetFloat("_Index", (float)uvrectTiling.index);
						this.backgroundTexture.Material.SetFloat("_Size", (float)uvrectTiling.blockW);
					}
					else
					{
						this.backgroundTexture.UVRect = uvrectTiling.uv;
					}
					this.backgroundTexture.IsVisible = true;
				}
			}
		}
		base.RefreshBindings(false);
	}

	// Token: 0x0400562A RID: 22058
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture backgroundTexture;

	// Token: 0x0400562B RID: 22059
	[PublicizedFrom(EAccessModifier.Private)]
	public Block blockData;

	// Token: 0x0400562C RID: 22060
	[PublicizedFrom(EAccessModifier.Private)]
	public string materialName;

	// Token: 0x0400562D RID: 22061
	[PublicizedFrom(EAccessModifier.Private)]
	public string upgradeMaterial;

	// Token: 0x0400562E RID: 22062
	[PublicizedFrom(EAccessModifier.Private)]
	public string downgradeMaterial;
}

using System;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D26 RID: 3366
[Preserve]
public class XUiC_MaterialInfoWindow : XUiController
{
	// Token: 0x060068C9 RID: 26825 RVA: 0x002A9065 File Offset: 0x002A7265
	public override void Init()
	{
		base.Init();
		this.textMaterial = (base.GetChildById("textMaterial").ViewComponent as XUiV_Texture);
		this.textMaterial.CreateMaterial();
	}

	// Token: 0x060068CA RID: 26826 RVA: 0x002A9093 File Offset: 0x002A7293
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.IsDirty && base.ViewComponent.IsVisible)
		{
			this.IsDirty = false;
		}
	}

	// Token: 0x060068CB RID: 26827 RVA: 0x002A90B8 File Offset: 0x002A72B8
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(bindingName);
		if (num <= 1111481258U)
		{
			if (num <= 107290595U)
			{
				if (num != 74828773U)
				{
					if (num == 107290595U)
					{
						if (bindingName == "materialname")
						{
							value = ((this.TextureData != null) ? Localization.Get(this.TextureData.Name, false) : "");
							return true;
						}
					}
				}
				else if (bindingName == "perklevel")
				{
					value = ((this.TextureData != null) ? this.perklevelFormatter.Format(this.TextureData.RequiredLevel) : "");
					return true;
				}
			}
			else if (num != 461391004U)
			{
				if (num != 1095359625U)
				{
					if (num == 1111481258U)
					{
						if (bindingName == "paintcost")
						{
							value = ((this.TextureData != null) ? this.paintcostFormatter.Format(this.TextureData.PaintCost) : "");
							return true;
						}
					}
				}
				else if (bindingName == "paintunit")
				{
					value = ((this.TextureData != null) ? Localization.Get("xuiPaintUnit", false) : "");
					return true;
				}
			}
			else if (bindingName == "paintcosttitle")
			{
				value = ((this.TextureData != null) ? Localization.Get("xuiPaintCost", false) : "");
				return true;
			}
		}
		else if (num <= 2509437782U)
		{
			if (num != 1605967500U)
			{
				if (num == 2509437782U)
				{
					if (bindingName == "requiredtitle")
					{
						value = ((this.TextureData != null) ? Localization.Get("xuiRequired", false) : "");
						return true;
					}
				}
			}
			else if (bindingName == "group")
			{
				value = ((this.TextureData != null) ? Localization.Get(this.TextureData.Group, false) : "");
				return true;
			}
		}
		else if (num != 2728359946U)
		{
			if (num != 3066960388U)
			{
				if (num == 3069197533U)
				{
					if (bindingName == "perk")
					{
						value = "";
						if (this.TextureData != null && this.TextureData.LockedByPerk != "")
						{
							ProgressionValue progressionValue = base.xui.playerUI.entityPlayer.Progression.GetProgressionValue(this.TextureData.LockedByPerk);
							value = Localization.Get(progressionValue.ProgressionClass.NameKey, false);
						}
						return true;
					}
				}
			}
			else if (bindingName == "hasperklock")
			{
				value = ((this.TextureData != null) ? (this.TextureData.LockedByPerk != "").ToString() : "false");
				return true;
			}
		}
		else if (bindingName == "grouptitle")
		{
			value = ((this.TextureData != null) ? Localization.Get("xuiMaterialGroup", false) : "");
			return true;
		}
		return false;
	}

	// Token: 0x060068CC RID: 26828 RVA: 0x002A93D4 File Offset: 0x002A75D4
	public void SetMaterial(BlockTextureData newTexture)
	{
		this.TextureData = newTexture;
		this.textMaterial.IsVisible = false;
		if (this.TextureData != null)
		{
			this.textMaterial.IsVisible = true;
			MeshDescription meshDescription = MeshDescription.meshes[0];
			int textureID = (int)this.TextureData.TextureID;
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
		}
		base.RefreshBindings(false);
	}

	// Token: 0x04004F06 RID: 20230
	[PublicizedFrom(EAccessModifier.Private)]
	public BlockTextureData TextureData;

	// Token: 0x04004F07 RID: 20231
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture textMaterial;

	// Token: 0x04004F08 RID: 20232
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<ushort> paintcostFormatter = new CachedStringFormatter<ushort>((ushort _i) => _i.ToString());

	// Token: 0x04004F09 RID: 20233
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatter<ushort> perklevelFormatter = new CachedStringFormatter<ushort>((ushort _i) => _i.ToString());
}

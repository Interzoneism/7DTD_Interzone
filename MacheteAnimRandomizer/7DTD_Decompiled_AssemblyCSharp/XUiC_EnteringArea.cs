using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000CAD RID: 3245
[Preserve]
public class XUiC_EnteringArea : XUiController
{
	// Token: 0x06006438 RID: 25656 RVA: 0x00289274 File Offset: 0x00287474
	public override void Init()
	{
		base.Init();
		base.GetChildrenByViewType<XUiV_Label>(this.labels);
		base.GetChildrenByViewType<XUiV_Sprite>(this.sprites);
		XUiController childById = base.GetChildById("background");
		if (childById != null)
		{
			this.bgSprite = (childById.ViewComponent as XUiV_Sprite);
		}
		this.IsDirty = true;
	}

	// Token: 0x06006439 RID: 25657 RVA: 0x002892C8 File Offset: 0x002874C8
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (!this.LocalPlayer)
		{
			if (base.xui && base.xui.playerUI)
			{
				this.LocalPlayer = base.xui.playerUI.entityPlayer;
			}
			if (!this.LocalPlayer)
			{
				return;
			}
		}
		if (!this.LocalPlayer.IsAlive() || !base.xui.playerUI.windowManager.IsHUDEnabled())
		{
			base.ViewComponent.IsVisible = false;
			this.fadePercent = 0f;
			this.message = null;
			return;
		}
		PrefabInstance enteredPrefab = this.LocalPlayer.enteredPrefab;
		Prefab prefab = (enteredPrefab != null) ? enteredPrefab.prefab : null;
		if (prefab != null && prefab != this.prefab && prefab != this.lastPrefab)
		{
			this.LocalPlayer.enteredPrefab = null;
			if (!prefab.Tags.Test_AnySet(this.prefabIgnoreTags))
			{
				this.prefab = prefab;
				this.prefabDiff = (int)this.prefab.DifficultyTier;
				this.CalcBiomeDifficulty();
				this.showTime = 3f;
				this.message = null;
				this.fadePercentTarget = 1f;
				base.RefreshBindings(true);
				base.ViewComponent.IsVisible = true;
			}
		}
		if (this.prefab != null)
		{
			Prefab prefab2 = this.prefab;
			PrefabInstance prefabInstance = this.LocalPlayer.prefab;
			if (prefab2 != ((prefabInstance != null) ? prefabInstance.prefab : null))
			{
				this.showTime = Utils.FastMin(2f, this.showTime);
			}
		}
		if (this.prefab == null)
		{
			BiomeDefinition biomeStandingOn = this.LocalPlayer.biomeStandingOn;
			if (biomeStandingOn != null && this.biome != biomeStandingOn && biomeStandingOn != this.lastBiome)
			{
				if (this.ignoreFirst)
				{
					this.lastBiome = biomeStandingOn;
					this.ignoreFirst = false;
				}
				else
				{
					this.biome = biomeStandingOn;
					this.CalcBiomeDifficulty();
					this.showTime = 3f;
					this.message = null;
					this.fadePercentTarget = 1f;
					base.RefreshBindings(true);
					base.ViewComponent.IsVisible = true;
				}
			}
		}
		if (this.prefab == null && this.biome == null)
		{
			if (this.LocalPlayer.AreaMessage != null)
			{
				this.showTime = 1f;
				this.fadePercentTarget = this.LocalPlayer.AreaMessageAlpha;
				if (this.LocalPlayer.AreaMessage != this.message)
				{
					this.message = this.LocalPlayer.AreaMessage;
					base.RefreshBindings(true);
				}
				base.ViewComponent.IsVisible = true;
			}
		}
		else if (this.LocalPlayer.AreaMessage != null)
		{
			this.showTime = Utils.FastMin(2f, this.showTime);
		}
		this.showTime -= _dt;
		if (this.showTime <= 0f)
		{
			if (this.prefab != null)
			{
				this.lastPrefab = this.prefab;
				this.prefab = null;
			}
			if (this.biome != null)
			{
				this.lastBiome = this.biome;
				this.biome = null;
			}
			this.prefabDiff = 0;
			this.biomeDiff = 0;
			this.showBiomeHalf = false;
			this.fadePercentTarget = 0f;
		}
		if (this.fadePercent != this.fadePercentTarget)
		{
			this.FadeUpdate();
		}
	}

	// Token: 0x0600643A RID: 25658 RVA: 0x002895EC File Offset: 0x002877EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void CalcBiomeDifficulty()
	{
		float num = (float)(this.LocalPlayer.biomeStandingOn.Difficulty - 1) * 0.5f;
		this.biomeDiff = (int)Mathf.Floor(num);
		this.showBiomeHalf = (num - (float)this.biomeDiff == 0.5f);
	}

	// Token: 0x0600643B RID: 25659 RVA: 0x00289638 File Offset: 0x00287838
	[PublicizedFrom(EAccessModifier.Private)]
	public void FadeUpdate()
	{
		float num = (this.fadePercentTarget > 0f) ? 2f : 0.33f;
		this.fadePercent = Utils.FastMoveTowards(this.fadePercent, this.fadePercentTarget, Time.deltaTime * num);
		for (int i = 0; i < this.labels.Count; i++)
		{
			XUiV_Label xuiV_Label = this.labels[i];
			Color color = xuiV_Label.Color;
			color.a = this.fadePercent;
			xuiV_Label.Color = color;
		}
		for (int j = 0; j < this.sprites.Count; j++)
		{
			XUiV_Sprite xuiV_Sprite = this.sprites[j];
			Color color2 = xuiV_Sprite.Color;
			if (xuiV_Sprite == this.bgSprite)
			{
				color2.a = Utils.FastLerp(0f, 0.5f, this.fadePercent);
			}
			else
			{
				color2.a = this.fadePercent;
			}
			xuiV_Sprite.Color = color2;
		}
		if (this.fadePercent == 0f)
		{
			base.ViewComponent.IsVisible = false;
		}
	}

	// Token: 0x0600643C RID: 25660 RVA: 0x00289737 File Offset: 0x00287937
	public override void OnOpen()
	{
		base.OnOpen();
		this.ignoreFirst = true;
		base.RefreshBindings(true);
	}

	// Token: 0x0600643D RID: 25661 RVA: 0x00289750 File Offset: 0x00287950
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 1451212208U)
		{
			if (num <= 955575921U)
			{
				if (num <= 922020683U)
				{
					if (num != 795840031U)
					{
						if (num == 922020683U)
						{
							if (_bindingName == "color1")
							{
								_value = this.GetNumberedColor(1);
								return true;
							}
						}
					}
					else if (_bindingName == "messagename")
					{
						if (this.prefab != null || this.biome != null || this.message == null)
						{
							_value = "";
							return true;
						}
						_value = this.message;
						return true;
					}
				}
				else if (num != 938798302U)
				{
					if (num == 955575921U)
					{
						if (_bindingName == "color3")
						{
							_value = this.GetNumberedColor(3);
							return true;
						}
					}
				}
				else if (_bindingName == "color2")
				{
					_value = this.GetNumberedColor(2);
					return true;
				}
			}
			else if (num <= 989131159U)
			{
				if (num != 972353540U)
				{
					if (num == 989131159U)
					{
						if (_bindingName == "color5")
						{
							_value = this.GetNumberedColor(5);
							return true;
						}
					}
				}
				else if (_bindingName == "color4")
				{
					_value = this.GetNumberedColor(4);
					return true;
				}
			}
			else if (num != 1005908778U)
			{
				if (num != 1022686397U)
				{
					if (num == 1451212208U)
					{
						if (_bindingName == "visible1")
						{
							_value = (this.prefabDiff + this.biomeDiff >= 1).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "color7")
				{
					_value = this.GetNumberedColor(7);
					return true;
				}
			}
			else if (_bindingName == "color6")
			{
				_value = this.GetNumberedColor(6);
				return true;
			}
		}
		else if (num <= 1535100303U)
		{
			if (num <= 1501545065U)
			{
				if (num != 1484767446U)
				{
					if (num == 1501545065U)
					{
						if (_bindingName == "visible2")
						{
							_value = (this.prefabDiff + this.biomeDiff >= 2).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "visible3")
				{
					_value = (this.prefabDiff + this.biomeDiff >= 3).ToString();
					return true;
				}
			}
			else if (num != 1518322684U)
			{
				if (num == 1535100303U)
				{
					if (_bindingName == "visible4")
					{
						_value = (this.prefabDiff + this.biomeDiff >= 4).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "visible5")
			{
				_value = (this.prefabDiff + this.biomeDiff >= 5).ToString();
				return true;
			}
		}
		else if (num <= 1568655541U)
		{
			if (num != 1551877922U)
			{
				if (num == 1568655541U)
				{
					if (_bindingName == "visible6")
					{
						_value = (this.prefabDiff + this.biomeDiff >= 6).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "visible7")
			{
				_value = (this.prefabDiff + this.biomeDiff >= 7).ToString();
				return true;
			}
		}
		else if (num != 1576332949U)
		{
			if (num != 4063875975U)
			{
				if (num == 4157284869U)
				{
					if (_bindingName == "locationname")
					{
						if (this.prefab == null && this.biome == null)
						{
							_value = "";
							return true;
						}
						_value = ((this.prefab != null) ? this.prefab.LocalizedName : this.biome.LocalizedName);
						return true;
					}
				}
			}
			else if (_bindingName == "visible_half")
			{
				_value = this.showBiomeHalf.ToString();
				return true;
			}
		}
		else if (_bindingName == "visible_loot_max")
		{
			if (this.LocalPlayer == null || (this.prefab == null && this.biome == null))
			{
				_value = "false";
				return true;
			}
			_value = this.LocalPlayer.LootAtMax.ToString();
			return true;
		}
		return false;
	}

	// Token: 0x0600643E RID: 25662 RVA: 0x00289BDC File Offset: 0x00287DDC
	[PublicizedFrom(EAccessModifier.Private)]
	public string GetNumberedColor(int _number)
	{
		if (this.prefabDiff >= _number)
		{
			return this.activeColorFormatter.Format(this.difficultyActiveColor);
		}
		if (this.prefabDiff + this.biomeDiff >= _number)
		{
			return this.activeColorFormatter.Format(this.biomeActiveColor);
		}
		return this.inactiveColorFormatter.Format(this.inactiveColor);
	}

	// Token: 0x0600643F RID: 25663 RVA: 0x00289C48 File Offset: 0x00287E48
	public override bool ParseAttribute(string name, string value, XUiController _parent)
	{
		bool flag = base.ParseAttribute(name, value, _parent);
		if (!flag)
		{
			if (!(name == "active_color"))
			{
				if (!(name == "inactive_color"))
				{
					return false;
				}
				this.inactiveColor = StringParsers.ParseColor32(value);
			}
			else
			{
				this.difficultyActiveColor = StringParsers.ParseColor32(value);
			}
			return true;
		}
		return flag;
	}

	// Token: 0x04004B6B RID: 19307
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiV_Label> labels = new List<XUiV_Label>();

	// Token: 0x04004B6C RID: 19308
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Sprite bgSprite;

	// Token: 0x04004B6D RID: 19309
	[PublicizedFrom(EAccessModifier.Private)]
	public List<XUiV_Sprite> sprites = new List<XUiV_Sprite>();

	// Token: 0x04004B6E RID: 19310
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadePercent = 0.0001f;

	// Token: 0x04004B6F RID: 19311
	[PublicizedFrom(EAccessModifier.Private)]
	public float fadePercentTarget;

	// Token: 0x04004B70 RID: 19312
	[PublicizedFrom(EAccessModifier.Private)]
	public float showTime;

	// Token: 0x04004B71 RID: 19313
	[PublicizedFrom(EAccessModifier.Private)]
	public Prefab lastPrefab;

	// Token: 0x04004B72 RID: 19314
	[PublicizedFrom(EAccessModifier.Private)]
	public Prefab prefab;

	// Token: 0x04004B73 RID: 19315
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition lastBiome;

	// Token: 0x04004B74 RID: 19316
	[PublicizedFrom(EAccessModifier.Private)]
	public BiomeDefinition biome;

	// Token: 0x04004B75 RID: 19317
	[PublicizedFrom(EAccessModifier.Private)]
	public string message;

	// Token: 0x04004B76 RID: 19318
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ignoreFirst;

	// Token: 0x04004B77 RID: 19319
	[PublicizedFrom(EAccessModifier.Private)]
	public EntityPlayerLocal LocalPlayer;

	// Token: 0x04004B78 RID: 19320
	[PublicizedFrom(EAccessModifier.Private)]
	public int prefabDiff;

	// Token: 0x04004B79 RID: 19321
	[PublicizedFrom(EAccessModifier.Private)]
	public int biomeDiff;

	// Token: 0x04004B7A RID: 19322
	[PublicizedFrom(EAccessModifier.Private)]
	public bool showBiomeHalf;

	// Token: 0x04004B7B RID: 19323
	[PublicizedFrom(EAccessModifier.Private)]
	public Color difficultyActiveColor = Color.red;

	// Token: 0x04004B7C RID: 19324
	[PublicizedFrom(EAccessModifier.Private)]
	public Color biomeActiveColor = new Color(1f, 0.5f, 0f);

	// Token: 0x04004B7D RID: 19325
	[PublicizedFrom(EAccessModifier.Private)]
	public Color inactiveColor = Color.grey;

	// Token: 0x04004B7E RID: 19326
	[PublicizedFrom(EAccessModifier.Private)]
	public FastTags<TagGroup.Poi> prefabIgnoreTags = FastTags<TagGroup.Poi>.Parse("part,streettile,navonly,hideui");

	// Token: 0x04004B7F RID: 19327
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor activeColorFormatter = new CachedStringFormatterXuiRgbaColor();

	// Token: 0x04004B80 RID: 19328
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly CachedStringFormatterXuiRgbaColor inactiveColorFormatter = new CachedStringFormatterXuiRgbaColor();
}

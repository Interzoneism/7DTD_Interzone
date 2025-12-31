using System;
using System.Globalization;
using GUI_2;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000C78 RID: 3192
[Preserve]
[PublicizedFrom(EAccessModifier.Internal)]
public class XUiC_CustomCharacterWindowGroup : XUiController
{
	// Token: 0x06006256 RID: 25174 RVA: 0x0027E450 File Offset: 0x0027C650
	public override void Init()
	{
		base.Init();
		XUiC_CustomCharacterWindowGroup.ID = this.windowGroup.ID;
		this.previewWindow = base.GetChildByType<XUiC_SDCSPreviewWindow>();
		this.btnBack = (XUiC_SimpleButton)base.GetChildById("btnBack");
		this.btnBack.OnPressed += this.BtnBack_OnPress;
		this.btnApply = (XUiC_SimpleButton)base.GetChildById("btnApply");
		this.btnApply.OnPressed += this.BtnApply_OnPress;
		this.RefreshApplyLabel();
		this.btnRandomize = (XUiC_SimpleButton)base.GetChildById("btnRandomize");
		this.btnRandomize.OnPressed += this.BtnRandomize_OnPressed;
		this.races = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxRace");
		this.races.OnValueChanged += this.Races_OnValueChanged;
		this.genders = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxGender");
		this.genders.OnValueChanged += this.Genders_OnValueChanged;
		this.eyeColors = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxEyeColor");
		this.eyeColors.OnValueChanged += this.EyeColors_OnValueChanged;
		this.hairs = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxHairStyle");
		this.hairs.OnValueChanged += this.Hairs_OnValueChanged;
		this.hairColors = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxHairColor");
		this.hairColors.OnValueChanged += this.HairColors_OnValueChanged;
		this.variants = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxFace");
		this.variants.OnValueChanged += this.Variants_OnValueChanged;
		this.mustaches = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxMustaches");
		this.mustaches.OnValueChanged += this.Mustaches_OnValueChanged;
		this.chops = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxChops");
		this.chops.OnValueChanged += this.Chops_OnValueChanged;
		this.beards = (XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData>)base.GetChildById("cbxBeards");
		this.beards.OnValueChanged += this.Beards_OnValueChanged;
		XUiController childById = base.GetChildById("btnLockRace");
		if (childById != null)
		{
			this.btnLockRace = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockRace_OnPress;
		}
		childById = base.GetChildById("btnLockGender");
		if (childById != null)
		{
			this.btnLockGender = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockGender_OnPress;
		}
		childById = base.GetChildById("btnLockFace");
		if (childById != null)
		{
			this.btnLockFace = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockFace_OnPress;
		}
		childById = base.GetChildById("btnLockEyeColor");
		if (childById != null)
		{
			this.btnLockEyeColor = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockEyeColor_OnPress;
		}
		childById = base.GetChildById("btnLockHairStyle");
		if (childById != null)
		{
			this.btnLockHairStyle = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockHairStyle_OnPress;
		}
		childById = base.GetChildById("btnLockHairColor");
		if (childById != null)
		{
			this.btnLockHairColor = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockHairColor_OnPress;
		}
		childById = base.GetChildById("btnLockMustaches");
		if (childById != null)
		{
			this.btnLockMustaches = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockMustache_OnPress;
		}
		childById = base.GetChildById("btnLockChops");
		if (childById != null)
		{
			this.btnLockChops = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockChops_OnPress;
		}
		childById = base.GetChildById("btnLockBeards");
		if (childById != null)
		{
			this.btnLockBeards = (XUiV_Button)childById.ViewComponent;
			childById.OnPress += this.BtnLockBeards_OnPress;
		}
		this.CustomCharacterWindow = base.GetChildByType<XUiC_CustomCharacterWindow>();
		this.gr = GameRandomManager.Instance.CreateGameRandom();
		SDCSDataUtils.SetupData();
		base.RegisterForInputStyleChanges();
	}

	// Token: 0x06006257 RID: 25175 RVA: 0x0027E88A File Offset: 0x0027CA8A
	[PublicizedFrom(EAccessModifier.Private)]
	public void RefreshApplyLabel()
	{
		InControlExtensions.SetApplyButtonString(this.btnApply, "xuiApply");
	}

	// Token: 0x06006258 RID: 25176 RVA: 0x0027E89C File Offset: 0x0027CA9C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void InputStyleChanged(PlayerInputManager.InputStyle _oldStyle, PlayerInputManager.InputStyle _newStyle)
	{
		base.InputStyleChanged(_oldStyle, _newStyle);
		this.RefreshApplyLabel();
	}

	// Token: 0x06006259 RID: 25177 RVA: 0x0027E8AC File Offset: 0x0027CAAC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetLockButtonState(XUiV_Button btn, bool isLocked)
	{
		btn.DefaultSpriteName = (isLocked ? this.CustomCharacterWindow.lockedSprite : this.CustomCharacterWindow.unlockedSprite);
		btn.DefaultSpriteColor = (isLocked ? this.CustomCharacterWindow.lockedColor : this.CustomCharacterWindow.unlockedColor);
	}

	// Token: 0x0600625A RID: 25178 RVA: 0x0027E8FB File Offset: 0x0027CAFB
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockRace_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedRace = !this.lockedRace;
		this.SetLockButtonState(this.btnLockRace, this.lockedRace);
	}

	// Token: 0x0600625B RID: 25179 RVA: 0x0027E91E File Offset: 0x0027CB1E
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockGender_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedGenders = !this.lockedGenders;
		this.SetLockButtonState(this.btnLockGender, this.lockedGenders);
	}

	// Token: 0x0600625C RID: 25180 RVA: 0x0027E941 File Offset: 0x0027CB41
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockFace_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedVariants = !this.lockedVariants;
		this.SetLockButtonState(this.btnLockFace, this.lockedVariants);
	}

	// Token: 0x0600625D RID: 25181 RVA: 0x0027E964 File Offset: 0x0027CB64
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockEyeColor_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedEyeColors = !this.lockedEyeColors;
		this.SetLockButtonState(this.btnLockEyeColor, this.lockedEyeColors);
	}

	// Token: 0x0600625E RID: 25182 RVA: 0x0027E987 File Offset: 0x0027CB87
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockHairStyle_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedHairs = !this.lockedHairs;
		this.SetLockButtonState(this.btnLockHairStyle, this.lockedHairs);
	}

	// Token: 0x0600625F RID: 25183 RVA: 0x0027E9AA File Offset: 0x0027CBAA
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockHairColor_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedHairColors = !this.lockedHairColors;
		this.SetLockButtonState(this.btnLockHairColor, this.lockedHairColors);
	}

	// Token: 0x06006260 RID: 25184 RVA: 0x0027E9CD File Offset: 0x0027CBCD
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockMustache_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedMustaches = !this.lockedMustaches;
		this.SetLockButtonState(this.btnLockMustaches, this.lockedMustaches);
	}

	// Token: 0x06006261 RID: 25185 RVA: 0x0027E9F0 File Offset: 0x0027CBF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockChops_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedChops = !this.lockedChops;
		this.SetLockButtonState(this.btnLockChops, this.lockedChops);
	}

	// Token: 0x06006262 RID: 25186 RVA: 0x0027EA13 File Offset: 0x0027CC13
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLockBeards_OnPress(XUiController _sender, int _mouseButton)
	{
		this.lockedBeards = !this.lockedBeards;
		this.SetLockButtonState(this.btnLockBeards, this.lockedBeards);
	}

	// Token: 0x06006263 RID: 25187 RVA: 0x0027EA38 File Offset: 0x0027CC38
	public override void OnOpen()
	{
		this.windowGroup.openWindowOnEsc = XUiC_OptionsProfiles.ID;
		base.OnOpen();
		this.playerProfile = null;
		this.archetype = null;
		this.archetype = Archetype.GetArchetype(ProfileSDF.CurrentProfileName());
		if (this.archetype != null)
		{
			this.archetype = this.archetype.Clone();
		}
		else
		{
			string profileName = ProfileSDF.CurrentProfileName();
			this.playerProfile = PlayerProfile.LoadProfile(profileName).Clone();
		}
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.FaceButtonWest, "lblChangeView", XUiC_GamepadCalloutWindow.CalloutType.CharacterEditor);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.LeftTrigger, "igcoRotateLeft", XUiC_GamepadCalloutWindow.CalloutType.CharacterEditor);
		base.xui.calloutWindow.AddCallout(UIUtils.ButtonIcon.RightTrigger, "igcoRotateRight", XUiC_GamepadCalloutWindow.CalloutType.CharacterEditor);
		base.xui.calloutWindow.EnableCallouts(XUiC_GamepadCalloutWindow.CalloutType.CharacterEditor, 0f);
		this.SetInitialOptions();
		base.RefreshBindings(false);
	}

	// Token: 0x06006264 RID: 25188 RVA: 0x0027EB18 File Offset: 0x0027CD18
	public override void OnClose()
	{
		base.OnClose();
		base.xui.calloutWindow.ClearCallouts(XUiC_GamepadCalloutWindow.CalloutType.CharacterEditor);
	}

	// Token: 0x06006265 RID: 25189 RVA: 0x0027EB34 File Offset: 0x0027CD34
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetInitialOptions()
	{
		if (this.playerProfile != null)
		{
			this.SetupGenders();
			this.genders.SelectedIndex = (this.playerProfile.IsMale ? 1 : 0);
			this.SetupRaces(this.playerProfile.IsMale);
			this.SetupVariants(this.playerProfile.IsMale, this.playerProfile.RaceName);
			this.SetupHairStyles(this.playerProfile.IsMale);
			this.SetupMustaches(this.playerProfile.IsMale);
			this.SetupChops(this.playerProfile.IsMale);
			this.SetupBeards(this.playerProfile.IsMale);
			this.SetupEyeColors();
			this.SetupHairColors();
			this.SetSelectedRace(this.playerProfile.RaceName, false);
			this.SetSelectedVariant(this.playerProfile.VariantNumber, false);
			this.SetSelectedEyeColor(this.playerProfile.EyeColor, false);
			this.SetSelectedHair(this.playerProfile.HairName, false);
			this.SetSelectedMustache(this.playerProfile.MustacheName, false);
			this.SetSelectedChops(this.playerProfile.ChopsName, false);
			this.SetSelectedBeard(this.playerProfile.BeardName, false);
			this.SetSelectedHairColor(this.playerProfile.HairColor, false);
		}
		else
		{
			this.genders.SelectedIndex = (this.playerProfile.IsMale ? 1 : 0);
			this.SetupRaces(this.archetype.IsMale);
			this.SetupVariants(this.archetype.IsMale, this.archetype.Race);
			this.SetupHairStyles(this.archetype.IsMale);
			this.SetupMustaches(this.archetype.IsMale);
			this.SetupChops(this.archetype.IsMale);
			this.SetupBeards(this.archetype.IsMale);
			this.SetupEyeColors();
			this.SetupHairColors();
			this.SetSelectedRace(this.archetype.Race, false);
			this.SetSelectedVariant(this.archetype.Variant, false);
			this.SetSelectedEyeColor(this.archetype.EyeColorName, false);
			this.SetSelectedHair(this.archetype.Hair, false);
			this.SetSelectedMustache(this.archetype.MustacheName, false);
			this.SetSelectedChops(this.archetype.ChopsName, false);
			this.SetSelectedBeard(this.archetype.BeardName, false);
			this.SetSelectedHairColor(this.playerProfile.HairColor, false);
		}
		this.ResetLocks();
	}

	// Token: 0x06006266 RID: 25190 RVA: 0x0027EDB0 File Offset: 0x0027CFB0
	[PublicizedFrom(EAccessModifier.Private)]
	public void ResetLocks()
	{
		this.lockedBeards = (this.lockedChops = (this.lockedEyeColors = (this.lockedGenders = (this.lockedHairColors = (this.lockedHairs = (this.lockedMustaches = (this.lockedRace = (this.lockedVariants = false))))))));
		this.SetLockButtonState(this.btnLockGender, false);
		this.SetLockButtonState(this.btnLockRace, false);
		this.SetLockButtonState(this.btnLockFace, false);
		this.SetLockButtonState(this.btnLockEyeColor, false);
		this.SetLockButtonState(this.btnLockHairStyle, false);
		this.SetLockButtonState(this.btnLockHairColor, false);
		this.SetLockButtonState(this.btnLockMustaches, false);
		this.SetLockButtonState(this.btnLockChops, false);
		this.SetLockButtonState(this.btnLockBeards, false);
	}

	// Token: 0x06006267 RID: 25191 RVA: 0x0027EE84 File Offset: 0x0027D084
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnApply_OnPress(XUiController _sender, int _mouseButton)
	{
		if (this.HasChanges)
		{
			if (this.playerProfile != null)
			{
				ProfileSDF.SaveProfile(ProfileSDF.CurrentProfileName(), this.playerProfile.ProfileArchetype, this.playerProfile.IsMale, this.playerProfile.RaceName, this.playerProfile.VariantNumber, this.playerProfile.EyeColor, this.playerProfile.HairName, this.playerProfile.HairColor, this.playerProfile.MustacheName, this.playerProfile.ChopsName, this.playerProfile.BeardName);
			}
			else if (this.archetype != null)
			{
				Archetype.SetArchetype(this.archetype);
				Archetype.SaveArchetypesToFile();
			}
			this.HasChanges = false;
		}
		this.OpenOptions();
	}

	// Token: 0x06006268 RID: 25192 RVA: 0x0027EF43 File Offset: 0x0027D143
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnBack_OnPress(XUiController _sender, int _mouseButton)
	{
		this.OpenOptions();
	}

	// Token: 0x06006269 RID: 25193 RVA: 0x0027EF4C File Offset: 0x0027D14C
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnRandomize_OnPressed(XUiController _sender, int _mouseButton)
	{
		if (!this.lockedGenders)
		{
			this.genders.SelectedIndex = this.gr.RandomRange(2);
			this.genders.TriggerValueChangedEvent(this.genders.Elements[0]);
		}
		if (!this.lockedRace)
		{
			this.races.SelectedIndex = this.gr.RandomRange(this.races.Elements.Count);
			this.races.TriggerValueChangedEvent(this.races.Elements[0]);
		}
		if (!this.lockedVariants)
		{
			this.variants.SelectedIndex = this.gr.RandomRange(this.variants.Elements.Count);
			this.variants.TriggerValueChangedEvent(this.variants.Elements[0]);
		}
		if (!this.lockedEyeColors)
		{
			this.eyeColors.SelectedIndex = this.gr.RandomRange(this.eyeColors.Elements.Count);
			this.eyeColors.TriggerValueChangedEvent(this.eyeColors.Elements[0]);
		}
		if (!this.lockedHairs)
		{
			this.hairs.SelectedIndex = this.gr.RandomRange(this.hairs.Elements.Count);
			this.hairs.TriggerValueChangedEvent(this.hairs.Elements[0]);
		}
		if (!this.lockedHairColors)
		{
			this.hairColors.SelectedIndex = this.gr.RandomRange(this.hairColors.Elements.Count);
			this.hairColors.TriggerValueChangedEvent(this.hairColors.Elements[0]);
		}
		if (!this.lockedMustaches)
		{
			this.mustaches.SelectedIndex = this.gr.RandomRange(this.mustaches.Elements.Count);
			this.mustaches.TriggerValueChangedEvent(this.mustaches.Elements[0]);
		}
		if (!this.lockedChops)
		{
			this.chops.SelectedIndex = this.gr.RandomRange(this.chops.Elements.Count);
			this.chops.TriggerValueChangedEvent(this.chops.Elements[0]);
		}
		if (!this.lockedBeards)
		{
			this.beards.SelectedIndex = this.gr.RandomRange(this.beards.Elements.Count);
			this.beards.TriggerValueChangedEvent(this.beards.Elements[0]);
		}
	}

	// Token: 0x0600626A RID: 25194 RVA: 0x0027F1E4 File Offset: 0x0027D3E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void OpenOptions()
	{
		base.xui.playerUI.windowManager.Close(this.windowGroup.ID);
		base.xui.playerUI.windowManager.Open(XUiC_OptionsProfiles.ID, true, false, true);
	}

	// Token: 0x0600626B RID: 25195 RVA: 0x0027F224 File Offset: 0x0027D424
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupGenders()
	{
		this.genders.Elements.Clear();
		this.genders.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("female", Localization.Get("xuiBoolMaleOff", false)));
		this.genders.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("male", Localization.Get("xuiBoolMaleOn", false)));
	}

	// Token: 0x0600626C RID: 25196 RVA: 0x0027F28C File Offset: 0x0027D48C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupRaces(bool isMale)
	{
		this.races.Elements.Clear();
		int num = 1;
		foreach (string text in SDCSDataUtils.GetRaceList(isMale))
		{
			this.races.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(text, Localization.Get("xuiRace" + text.ToLower(), false)));
			num++;
		}
	}

	// Token: 0x0600626D RID: 25197 RVA: 0x0027F31C File Offset: 0x0027D51C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupVariants(bool isMale, string raceName)
	{
		this.variants.Elements.Clear();
		int num = 1;
		foreach (string internalName in SDCSDataUtils.GetVariantList(isMale, raceName))
		{
			this.variants.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(internalName, Localization.Get("lblFace", false) + " " + num.ToString("00")));
			num++;
		}
	}

	// Token: 0x0600626E RID: 25198 RVA: 0x0027F3B8 File Offset: 0x0027D5B8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupEyeColors()
	{
		this.eyeColors.Elements.Clear();
		int num = 1;
		foreach (string internalName in SDCSDataUtils.GetEyeColorNames())
		{
			this.eyeColors.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(internalName, Localization.Get("xuiCharacterColorSlotEyes", false) + " " + num.ToString("00")));
			num++;
		}
	}

	// Token: 0x0600626F RID: 25199 RVA: 0x0027F450 File Offset: 0x0027D650
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupHairStyles(bool isMale)
	{
		this.hairs.Elements.Clear();
		this.hairs.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("", Localization.Get("xuiCharacterHairStyle", false) + " 00"));
		int num = 1;
		foreach (string internalName in SDCSDataUtils.GetHairNames(isMale, SDCSDataUtils.HairTypes.Hair))
		{
			this.hairs.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(internalName, Localization.Get("xuiCharacterHairStyle", false) + " " + num.ToString("00")));
			num++;
		}
	}

	// Token: 0x06006270 RID: 25200 RVA: 0x0027F51C File Offset: 0x0027D71C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupHairColors()
	{
		this.hairColors.Elements.Clear();
		int num = 1;
		foreach (SDCSDataUtils.HairColorData hairColorData in SDCSDataUtils.GetHairColorNames())
		{
			this.hairColors.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(hairColorData.PrefabName, Localization.Get("xuiCharacterHairColor", false) + " " + num.ToString("00")));
			num++;
		}
	}

	// Token: 0x06006271 RID: 25201 RVA: 0x0027F5BC File Offset: 0x0027D7BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupMustaches(bool isMale)
	{
		this.mustaches.Elements.Clear();
		this.mustaches.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("", Localization.Get("xuiCharacterMustaches", false) + " 00"));
		foreach (string text in SDCSDataUtils.GetHairNames(isMale, SDCSDataUtils.HairTypes.Mustache))
		{
			string text2 = text;
			if (text2.Length == 1)
			{
				text2 = text2.Insert(0, "0");
			}
			this.mustaches.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(text, Localization.Get("xuiCharacterMustaches", false) + " " + Localization.Get(text2, false)));
		}
	}

	// Token: 0x06006272 RID: 25202 RVA: 0x0027F694 File Offset: 0x0027D894
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupChops(bool isMale)
	{
		this.chops.Elements.Clear();
		this.chops.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("", Localization.Get("xuiCharacterChops", false) + " 00"));
		foreach (string text in SDCSDataUtils.GetHairNames(isMale, SDCSDataUtils.HairTypes.Chops))
		{
			string text2 = text;
			if (text2.Length == 1)
			{
				text2 = text2.Insert(0, "0");
			}
			this.chops.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(text, Localization.Get("xuiCharacterChops", false) + " " + Localization.Get(text2, false)));
		}
	}

	// Token: 0x06006273 RID: 25203 RVA: 0x0027F76C File Offset: 0x0027D96C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupBeards(bool isMale)
	{
		this.beards.Elements.Clear();
		this.beards.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData("", Localization.Get("xuiCharacterBeards", false) + " 00"));
		foreach (string text in SDCSDataUtils.GetHairNames(isMale, SDCSDataUtils.HairTypes.Beard))
		{
			string text2 = text;
			if (text2.Length == 1)
			{
				text2 = text2.Insert(0, "0");
			}
			this.beards.Elements.Add(new XUiC_CustomCharacterWindowGroup.NameData(text, Localization.Get("xuiCharacterBeards", false) + " " + Localization.Get(text2, false)));
		}
	}

	// Token: 0x06006274 RID: 25204 RVA: 0x0027F844 File Offset: 0x0027DA44
	[PublicizedFrom(EAccessModifier.Private)]
	public void Genders_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.Sex = internalName;
		bool isMale = internalName == "male";
		if (this.playerProfile != null)
		{
			this.playerProfile.IsMale = isMale;
		}
		string internalName2 = this.races.Value.InternalName;
		int variant = StringParsers.ParseSInt32(this.variants.Value.InternalName, 0, -1, NumberStyles.Integer);
		this.SetupRaces(isMale);
		this.SetSelectedRace(internalName2, true);
		this.SetupVariants(isMale, this.races.Value.InternalName);
		this.SetSelectedVariant(variant, true);
		this.SetupHairStyles(isMale);
		this.SetupMustaches(isMale);
		this.SetupChops(isMale);
		this.SetupBeards(isMale);
		this.SetSelectedMustache("", true);
		this.SetSelectedChops("", true);
		this.SetSelectedBeard("", true);
		base.RefreshBindings(false);
		this.previewWindow.MakePreview();
		this.HasChanges = true;
	}

	// Token: 0x06006275 RID: 25205 RVA: 0x0027F93C File Offset: 0x0027DB3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Races_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.Race = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.RaceName = internalName;
		}
		int variant = StringParsers.ParseSInt32(this.variants.Value.InternalName, 0, -1, NumberStyles.Integer);
		this.SetupVariants(this.previewWindow.Archetype.IsMale, this.races.Value.InternalName);
		this.SetSelectedVariant(variant, true);
		this.previewWindow.MakePreview();
		this.HasChanges = true;
	}

	// Token: 0x06006276 RID: 25206 RVA: 0x0027F9D0 File Offset: 0x0027DBD0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Variants_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		int num = StringParsers.ParseSInt32(_newValue.InternalName, 0, -1, NumberStyles.Integer);
		this.previewWindow.Archetype.Variant = num;
		if (this.playerProfile != null)
		{
			this.playerProfile.VariantNumber = num;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x06006277 RID: 25207 RVA: 0x0027FA30 File Offset: 0x0027DC30
	[PublicizedFrom(EAccessModifier.Private)]
	public void Hairs_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.Hair = internalName;
		this.previewWindow.Archetype.HairColor = this.hairColors.Value.InternalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.HairName = internalName;
			this.playerProfile.HairColor = this.hairColors.Value.InternalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x06006278 RID: 25208 RVA: 0x0027FAC4 File Offset: 0x0027DCC4
	[PublicizedFrom(EAccessModifier.Private)]
	public void HairColors_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.HairColor = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.HairColor = internalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x06006279 RID: 25209 RVA: 0x0027FB1C File Offset: 0x0027DD1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void EyeColors_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.EyeColorName = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.EyeColor = internalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToEye();
		this.HasChanges = true;
	}

	// Token: 0x0600627A RID: 25210 RVA: 0x0027FB74 File Offset: 0x0027DD74
	[PublicizedFrom(EAccessModifier.Private)]
	public void Mustaches_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.MustacheName = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.MustacheName = internalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x0600627B RID: 25211 RVA: 0x0027FBCC File Offset: 0x0027DDCC
	[PublicizedFrom(EAccessModifier.Private)]
	public void Chops_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.ChopsName = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.ChopsName = internalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x0600627C RID: 25212 RVA: 0x0027FC24 File Offset: 0x0027DE24
	[PublicizedFrom(EAccessModifier.Private)]
	public void Beards_OnValueChanged(XUiController _sender, XUiC_CustomCharacterWindowGroup.NameData _oldValue, XUiC_CustomCharacterWindowGroup.NameData _newValue)
	{
		string internalName = _newValue.InternalName;
		this.previewWindow.Archetype.BeardName = internalName;
		if (this.playerProfile != null)
		{
			this.playerProfile.BeardName = internalName;
		}
		this.previewWindow.MakePreview();
		this.previewWindow.ZoomToHead();
		this.HasChanges = true;
	}

	// Token: 0x0600627D RID: 25213 RVA: 0x0027FC7C File Offset: 0x0027DE7C
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedRace(string raceName, bool applyToPreview = false)
	{
		if (raceName == "")
		{
			this.races.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.races.Elements.Count; i++)
			{
				if (this.races.Elements[i].InternalName.EqualsCaseInsensitive(raceName))
				{
					this.races.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.races.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.Race = this.races.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.RaceName = this.previewWindow.Archetype.Race;
			}
		}
	}

	// Token: 0x0600627E RID: 25214 RVA: 0x0027FD44 File Offset: 0x0027DF44
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedVariant(int variant, bool applyToPreview = false)
	{
		if (variant == -1)
		{
			this.variants.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.variants.Elements.Count; i++)
			{
				if (StringParsers.ParseSInt32(this.variants.Elements[i].InternalName, 0, -1, NumberStyles.Integer) == variant)
				{
					this.variants.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.variants.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.Variant = StringParsers.ParseSInt32(this.variants.Value.InternalName, 0, -1, NumberStyles.Integer);
			if (this.playerProfile != null)
			{
				this.playerProfile.VariantNumber = this.previewWindow.Archetype.Variant;
			}
		}
	}

	// Token: 0x0600627F RID: 25215 RVA: 0x0027FE10 File Offset: 0x0027E010
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedEyeColor(string eyeColorName, bool applyToPreview = false)
	{
		if (eyeColorName == "")
		{
			this.eyeColors.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.eyeColors.Elements.Count; i++)
			{
				if (this.eyeColors.Elements[i].InternalName.EqualsCaseInsensitive(eyeColorName))
				{
					this.eyeColors.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.eyeColors.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.EyeColorName = this.eyeColors.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.EyeColor = this.previewWindow.Archetype.EyeColorName;
			}
		}
	}

	// Token: 0x06006280 RID: 25216 RVA: 0x0027FED8 File Offset: 0x0027E0D8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedHair(string hairName, bool applyToPreview = false)
	{
		if (hairName == "")
		{
			this.hairs.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.hairs.Elements.Count; i++)
			{
				if (this.hairs.Elements[i].InternalName.EqualsCaseInsensitive(hairName))
				{
					this.hairs.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.hairs.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.Hair = this.hairs.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.HairName = this.previewWindow.Archetype.Hair;
			}
		}
	}

	// Token: 0x06006281 RID: 25217 RVA: 0x0027FFA0 File Offset: 0x0027E1A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedHairColor(string hairColorName, bool applyToPreview = false)
	{
		if (hairColorName == "")
		{
			this.hairColors.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.hairColors.Elements.Count; i++)
			{
				if (this.hairColors.Elements[i].InternalName.EqualsCaseInsensitive(hairColorName))
				{
					this.hairColors.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.hairColors.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.HairColor = this.hairColors.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.HairColor = this.previewWindow.Archetype.HairColor;
			}
		}
	}

	// Token: 0x06006282 RID: 25218 RVA: 0x00280068 File Offset: 0x0027E268
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedMustache(string mustacheName, bool applyToPreview = false)
	{
		if (mustacheName == "")
		{
			this.mustaches.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.mustaches.Elements.Count; i++)
			{
				if (this.mustaches.Elements[i].InternalName.EqualsCaseInsensitive(mustacheName))
				{
					this.mustaches.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.mustaches.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.MustacheName = this.mustaches.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.MustacheName = this.previewWindow.Archetype.MustacheName;
			}
		}
	}

	// Token: 0x06006283 RID: 25219 RVA: 0x00280130 File Offset: 0x0027E330
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedChops(string chopsName, bool applyToPreview = false)
	{
		if (chopsName == "")
		{
			this.chops.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.chops.Elements.Count; i++)
			{
				if (this.chops.Elements[i].InternalName.EqualsCaseInsensitive(chopsName))
				{
					this.chops.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.chops.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.ChopsName = this.chops.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.ChopsName = this.previewWindow.Archetype.ChopsName;
			}
		}
	}

	// Token: 0x06006284 RID: 25220 RVA: 0x002801F8 File Offset: 0x0027E3F8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetSelectedBeard(string beardName, bool applyToPreview = false)
	{
		if (beardName == "")
		{
			this.beards.SelectedIndex = 0;
		}
		else
		{
			int num = -1;
			for (int i = 0; i < this.beards.Elements.Count; i++)
			{
				if (this.beards.Elements[i].InternalName.EqualsCaseInsensitive(beardName))
				{
					this.beards.SelectedIndex = i;
					return;
				}
			}
			if (num == -1)
			{
				this.beards.SelectedIndex = 0;
			}
		}
		if (applyToPreview)
		{
			this.previewWindow.Archetype.BeardName = this.beards.Value.InternalName;
			if (this.playerProfile != null)
			{
				this.playerProfile.BeardName = this.previewWindow.Archetype.BeardName;
			}
		}
	}

	// Token: 0x06006285 RID: 25221 RVA: 0x002802C0 File Offset: 0x0027E4C0
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (this.HasChanges && PlatformManager.NativePlatform.Input.CurrentInputStyle != PlayerInputManager.InputStyle.Keyboard && base.xui.playerUI.playerInput.GUIActions.Apply.WasPressed)
		{
			this.BtnApply_OnPress(null, 0);
		}
	}

	// Token: 0x06006286 RID: 25222 RVA: 0x00280318 File Offset: 0x0027E518
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string value, string bindingName)
	{
		if (bindingName == "isMale")
		{
			value = (this.genders != null && this.genders.SelectedIndex == 1).ToString();
			return true;
		}
		return false;
	}

	// Token: 0x04004A00 RID: 18944
	public static string ID = "";

	// Token: 0x04004A01 RID: 18945
	public bool IsMale = true;

	// Token: 0x04004A02 RID: 18946
	[PublicizedFrom(EAccessModifier.Private)]
	public bool HasChanges;

	// Token: 0x04004A03 RID: 18947
	public PlayerProfile playerProfile;

	// Token: 0x04004A04 RID: 18948
	public Archetype archetype;

	// Token: 0x04004A05 RID: 18949
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> races;

	// Token: 0x04004A06 RID: 18950
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> genders;

	// Token: 0x04004A07 RID: 18951
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> eyeColors;

	// Token: 0x04004A08 RID: 18952
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> hairs;

	// Token: 0x04004A09 RID: 18953
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> hairColors;

	// Token: 0x04004A0A RID: 18954
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> mustaches;

	// Token: 0x04004A0B RID: 18955
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> chops;

	// Token: 0x04004A0C RID: 18956
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> beards;

	// Token: 0x04004A0D RID: 18957
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxList<XUiC_CustomCharacterWindowGroup.NameData> variants;

	// Token: 0x04004A0E RID: 18958
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SDCSPreviewWindow previewWindow;

	// Token: 0x04004A0F RID: 18959
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnBack;

	// Token: 0x04004A10 RID: 18960
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnApply;

	// Token: 0x04004A11 RID: 18961
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_SimpleButton btnRandomize;

	// Token: 0x04004A12 RID: 18962
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedRace = true;

	// Token: 0x04004A13 RID: 18963
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedGenders = true;

	// Token: 0x04004A14 RID: 18964
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedHairs;

	// Token: 0x04004A15 RID: 18965
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedHairColors;

	// Token: 0x04004A16 RID: 18966
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedVariants;

	// Token: 0x04004A17 RID: 18967
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedEyeColors;

	// Token: 0x04004A18 RID: 18968
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedMustaches;

	// Token: 0x04004A19 RID: 18969
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedChops;

	// Token: 0x04004A1A RID: 18970
	[PublicizedFrom(EAccessModifier.Private)]
	public bool lockedBeards;

	// Token: 0x04004A1B RID: 18971
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockRace;

	// Token: 0x04004A1C RID: 18972
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockGender;

	// Token: 0x04004A1D RID: 18973
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockFace;

	// Token: 0x04004A1E RID: 18974
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockEyeColor;

	// Token: 0x04004A1F RID: 18975
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockHairStyle;

	// Token: 0x04004A20 RID: 18976
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockHairColor;

	// Token: 0x04004A21 RID: 18977
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockMustaches;

	// Token: 0x04004A22 RID: 18978
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockChops;

	// Token: 0x04004A23 RID: 18979
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Button btnLockBeards;

	// Token: 0x04004A24 RID: 18980
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_CustomCharacterWindow CustomCharacterWindow;

	// Token: 0x04004A25 RID: 18981
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom gr;

	// Token: 0x02000C79 RID: 3193
	public enum Gender
	{
		// Token: 0x04004A27 RID: 18983
		Male,
		// Token: 0x04004A28 RID: 18984
		Female
	}

	// Token: 0x02000C7A RID: 3194
	public enum Race
	{
		// Token: 0x04004A2A RID: 18986
		White,
		// Token: 0x04004A2B RID: 18987
		Black,
		// Token: 0x04004A2C RID: 18988
		Asian,
		// Token: 0x04004A2D RID: 18989
		Hispanic
	}

	// Token: 0x02000C7B RID: 3195
	public struct NameData
	{
		// Token: 0x06006289 RID: 25225 RVA: 0x00280381 File Offset: 0x0027E581
		public NameData(string _internalName, string _formattedName)
		{
			this.InternalName = _internalName;
			this.FormattedName = _formattedName;
		}

		// Token: 0x0600628A RID: 25226 RVA: 0x00280391 File Offset: 0x0027E591
		public override string ToString()
		{
			return this.FormattedName;
		}

		// Token: 0x04004A2E RID: 18990
		public string InternalName;

		// Token: 0x04004A2F RID: 18991
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string FormattedName;
	}
}

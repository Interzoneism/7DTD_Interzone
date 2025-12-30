using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000014 RID: 20
public class CharacterUIController : MonoBehaviour
{
	// Token: 0x0600007A RID: 122 RVA: 0x00008BA0 File Offset: 0x00006DA0
	[PublicizedFrom(EAccessModifier.Private)]
	public void Start()
	{
		if (this.characterConstruct != null)
		{
			this.InitializeToggles();
			this.InitializeDropdowns();
		}
		else
		{
			Debug.LogError("CharacterConstruct reference is missing! Please assign it in the inspector.");
		}
		this.SetupToggleListeners();
		this.SetupDropdownListeners();
		this.UpdateHatGearControlsVisibility(this.showHatHairToggle.isOn);
	}

	// Token: 0x0600007B RID: 123 RVA: 0x00008BF0 File Offset: 0x00006DF0
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeToggles()
	{
		this.showCharactersToggle.isOn = this.characterConstruct.ShowCharacters;
		this.showGearToggle.isOn = this.characterConstruct.ShowGear;
		this.showHairToggle.isOn = this.characterConstruct.ShowHair;
		this.showHatHairToggle.isOn = this.characterConstruct.ShowHatHair;
		this.showFacialHairToggle.isOn = this.characterConstruct.ShowFacialHair;
	}

	// Token: 0x0600007C RID: 124 RVA: 0x00008C6C File Offset: 0x00006E6C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitializeDropdowns()
	{
		if (this.raceDropdown != null)
		{
			this.raceDropdown.ClearOptions();
			foreach (string text in this.characterConstruct.GetRaceTypes())
			{
				this.raceDropdown.options.Add(new TMP_Dropdown.OptionData(text));
			}
			this.raceDropdown.value = this.characterConstruct.selectedRaceIndex;
			this.raceDropdown.RefreshShownValue();
		}
		if (this.variantDropdown != null)
		{
			this.variantDropdown.ClearOptions();
			foreach (string text2 in this.characterConstruct.GetVariantTypes())
			{
				this.variantDropdown.options.Add(new TMP_Dropdown.OptionData(text2));
			}
			this.variantDropdown.value = this.characterConstruct.selectedVariantIndex;
			this.variantDropdown.RefreshShownValue();
		}
		if (this.hatGearDropdown != null)
		{
			this.hatGearDropdown.ClearOptions();
			foreach (string text3 in this.characterConstruct.GetGearTypes())
			{
				this.hatGearDropdown.options.Add(new TMP_Dropdown.OptionData(text3));
			}
			this.hatGearDropdown.value = this.characterConstruct.hatHairGearIndex;
			this.hatGearDropdown.RefreshShownValue();
		}
	}

	// Token: 0x0600007D RID: 125 RVA: 0x00008DC8 File Offset: 0x00006FC8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupToggleListeners()
	{
		this.showCharactersToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowCharactersToggled));
		this.showGearToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowGearToggled));
		this.showHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowHairToggled));
		this.showHatHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowHatHairToggled));
		this.showFacialHairToggle.onValueChanged.AddListener(new UnityAction<bool>(this.OnShowFacialHairToggled));
	}

	// Token: 0x0600007E RID: 126 RVA: 0x00008E64 File Offset: 0x00007064
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetupDropdownListeners()
	{
		if (this.raceDropdown != null)
		{
			this.raceDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnRaceDropdownChanged));
		}
		if (this.variantDropdown != null)
		{
			this.variantDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnVariantDropdownChanged));
		}
		if (this.hatGearDropdown != null)
		{
			this.hatGearDropdown.onValueChanged.AddListener(new UnityAction<int>(this.OnHatGearDropdownChanged));
		}
	}

	// Token: 0x0600007F RID: 127 RVA: 0x00008EEF File Offset: 0x000070EF
	public void OnShowCharactersToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowCharacters = isOn;
		}
	}

	// Token: 0x06000080 RID: 128 RVA: 0x00008F0B File Offset: 0x0000710B
	public void OnShowGearToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowGear = isOn;
		}
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00008F27 File Offset: 0x00007127
	public void OnShowHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowHair = isOn;
		}
	}

	// Token: 0x06000082 RID: 130 RVA: 0x00008F43 File Offset: 0x00007143
	public void OnShowHatHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowHatHair = isOn;
			this.UpdateHatGearControlsVisibility(isOn);
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x00008F66 File Offset: 0x00007166
	public void OnShowFacialHairToggled(bool isOn)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.ShowFacialHair = isOn;
		}
	}

	// Token: 0x06000084 RID: 132 RVA: 0x00008F82 File Offset: 0x00007182
	public void OnRaceDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.selectedRaceIndex = index;
			this.characterConstruct.RespawnAllGroups();
		}
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00008FA9 File Offset: 0x000071A9
	public void OnVariantDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.selectedVariantIndex = index;
			this.characterConstruct.RespawnAllGroups();
		}
	}

	// Token: 0x06000086 RID: 134 RVA: 0x00008FD0 File Offset: 0x000071D0
	public void OnHatGearDropdownChanged(int index)
	{
		if (this.characterConstruct != null)
		{
			this.characterConstruct.hatHairGearIndex = index;
			this.characterConstruct.RespawnHatHairGroup();
		}
	}

	// Token: 0x06000087 RID: 135 RVA: 0x00008FF7 File Offset: 0x000071F7
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateHatGearControlsVisibility(bool isHatHairVisible)
	{
		if (this.hatGearControlsPanel != null)
		{
			this.hatGearControlsPanel.SetActive(isHatHairVisible);
		}
	}

	// Token: 0x040000B4 RID: 180
	[Header("References")]
	[Tooltip("Reference to the CharacterConstruct script")]
	public CharacterConstruct characterConstruct;

	// Token: 0x040000B5 RID: 181
	[Header("UI Elements")]
	[Tooltip("Toggle button for showing characters")]
	public Toggle showCharactersToggle;

	// Token: 0x040000B6 RID: 182
	[Tooltip("Toggle button for showing gear")]
	public Toggle showGearToggle;

	// Token: 0x040000B7 RID: 183
	[Tooltip("Toggle button for showing hair")]
	public Toggle showHairToggle;

	// Token: 0x040000B8 RID: 184
	[Tooltip("Toggle button for showing hat hair")]
	public Toggle showHatHairToggle;

	// Token: 0x040000B9 RID: 185
	[Tooltip("Toggle button for showing facial hair")]
	public Toggle showFacialHairToggle;

	// Token: 0x040000BA RID: 186
	[Header("Race and Variant Controls")]
	[Tooltip("Panel containing race and variant controls")]
	public GameObject raceVariantControlsPanel;

	// Token: 0x040000BB RID: 187
	[Tooltip("Dropdown for selecting race")]
	public TMP_Dropdown raceDropdown;

	// Token: 0x040000BC RID: 188
	[Tooltip("Dropdown for selecting variant")]
	public TMP_Dropdown variantDropdown;

	// Token: 0x040000BD RID: 189
	[Header("Hat Hair Controls")]
	[Tooltip("Panel containing hat gear controls")]
	public GameObject hatGearControlsPanel;

	// Token: 0x040000BE RID: 190
	[Tooltip("Dropdown for selecting hat gear type")]
	public TMP_Dropdown hatGearDropdown;

	// Token: 0x040000BF RID: 191
	[Header("UI Panel")]
	[Tooltip("Panel that contains all UI controls")]
	public GameObject controlPanel;
}

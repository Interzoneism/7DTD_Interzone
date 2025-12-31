using System;
using SDF;
using UnityEngine;

// Token: 0x02000951 RID: 2385
public static class ProfileSDF
{
	// Token: 0x060047E7 RID: 18407 RVA: 0x001C26EA File Offset: 0x001C08EA
	[PublicizedFrom(EAccessModifier.Private)]
	static ProfileSDF()
	{
		ProfileSDF.profileSDF.Open(GameIO.GetSaveGameRootDir() + "/sdcs_profiles.sdf");
	}

	// Token: 0x060047E8 RID: 18408 RVA: 0x001C270F File Offset: 0x001C090F
	public static string CurrentProfileName()
	{
		return ProfileSDF.profileSDF.GetString("selectedProfile") ?? "";
	}

	// Token: 0x060047E9 RID: 18409 RVA: 0x001C2729 File Offset: 0x001C0929
	public static void Save()
	{
		ProfileSDF.profileSDF.SaveAndKeepOpen();
	}

	// Token: 0x060047EA RID: 18410 RVA: 0x001C2738 File Offset: 0x001C0938
	public static bool ProfileExists(string _profileName)
	{
		string @string = ProfileSDF.profileSDF.GetString("profileNames");
		if (@string == null)
		{
			return false;
		}
		if (!@string.Contains(","))
		{
			return @string == _profileName;
		}
		string[] array = @string.Split(',', StringSplitOptions.None);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] == _profileName)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060047EB RID: 18411 RVA: 0x001C2798 File Offset: 0x001C0998
	public static void DeleteProfile(string _profileName)
	{
		if (!ProfileSDF.ProfileExists(_profileName))
		{
			return;
		}
		foreach (string text in ProfileSDF.profileSDF.GetKeys())
		{
			if (text.StartsWith(_profileName + "."))
			{
				ProfileSDF.profileSDF.Remove(text);
				ProfileSDF.Save();
			}
		}
		ProfileSDF.removeProfileName(_profileName);
		ProfileSDF.Save();
	}

	// Token: 0x060047EC RID: 18412 RVA: 0x001C27F9 File Offset: 0x001C09F9
	public static void SaveArchetype(string _archetype, bool _isMale)
	{
		if (!ProfileSDF.ProfileExists(_archetype))
		{
			ProfileSDF.addProfileName(_archetype);
			ProfileSDF.setSex(_archetype, _isMale);
			ProfileSDF.setArchetype(_archetype, _archetype);
		}
		ProfileSDF.Save();
	}

	// Token: 0x060047ED RID: 18413 RVA: 0x001C281C File Offset: 0x001C0A1C
	public static void SaveProfile(string _profileName, string _archetype, bool _isMale, string _raceName, int _variantNumber, string _eyeColorName, string _hairName, string _hairColor, string _mustacheName, string _chopsName, string _beardName)
	{
		if (!ProfileSDF.ProfileExists(_profileName))
		{
			ProfileSDF.addProfileName(_profileName);
		}
		if (_archetype == "")
		{
			if (_isMale)
			{
				_archetype = "BaseMale";
			}
			else
			{
				_archetype = "BaseFemale";
			}
		}
		ProfileSDF.setSex(_profileName, _isMale);
		ProfileSDF.setRace(_profileName, _raceName);
		ProfileSDF.setArchetype(_profileName, _archetype);
		ProfileSDF.setVariant(_profileName, _variantNumber);
		ProfileSDF.setEyeColor(_profileName, _eyeColorName);
		ProfileSDF.setHairName(_profileName, _hairName);
		ProfileSDF.setHairColor(_profileName, _hairColor);
		ProfileSDF.setMustacheName(_profileName, _mustacheName);
		ProfileSDF.setChopsName(_profileName, _chopsName);
		ProfileSDF.setBeardName(_profileName, _beardName);
		ProfileSDF.SetSelectedProfile(_profileName);
		ProfileSDF.Save();
	}

	// Token: 0x060047EE RID: 18414 RVA: 0x001C28B0 File Offset: 0x001C0AB0
	public static Archetype CreateTempArchetype(string _profileName)
	{
		Archetype archetype = new Archetype(_profileName, ProfileSDF.GetIsMale(_profileName), true);
		archetype.Race = ProfileSDF.GetRaceName(_profileName);
		archetype.Variant = ProfileSDF.GetVariantNumber(_profileName);
		archetype.Hair = ProfileSDF.GetHairName(_profileName);
		archetype.HairColor = ProfileSDF.GetHairColorName(_profileName);
		archetype.MustacheName = ProfileSDF.GetMustacheName(_profileName);
		archetype.ChopsName = ProfileSDF.GetChopsName(_profileName);
		archetype.BeardName = ProfileSDF.GetBeardName(_profileName);
		archetype.EyeColorName = ProfileSDF.GetEyeColorName(_profileName);
		archetype.AddEquipmentSlot(new SDCSUtils.SlotData
		{
			PartName = "body",
			PrefabName = "@:Entities/Player/{sex}/Gear/LumberJack/gear{sex}LumberJackPrefab.prefab",
			BaseToTurnOff = "body"
		});
		archetype.AddEquipmentSlot(new SDCSUtils.SlotData
		{
			PartName = "hands",
			PrefabName = "@:Entities/Player/{sex}/Gear/LumberJack/gear{sex}LumberJackPrefab.prefab",
			BaseToTurnOff = "hands"
		});
		archetype.AddEquipmentSlot(new SDCSUtils.SlotData
		{
			PartName = "feet",
			PrefabName = "@:Entities/Player/{sex}/Gear/LumberJack/gear{sex}LumberJackPrefab.prefab",
			BaseToTurnOff = "feet"
		});
		return archetype;
	}

	// Token: 0x060047EF RID: 18415 RVA: 0x001C29AE File Offset: 0x001C0BAE
	public static void SetSelectedProfile(string _profileName)
	{
		ProfileSDF.profileSDF.Set("selectedProfile", _profileName);
		if (ProfileSDF.GetIsMale(_profileName))
		{
			GamePrefs.Set(EnumGamePrefs.OptionsPlayerModel, "playerMale");
			return;
		}
		GamePrefs.Set(EnumGamePrefs.OptionsPlayerModel, "playerFemale");
	}

	// Token: 0x060047F0 RID: 18416 RVA: 0x001C29E4 File Offset: 0x001C0BE4
	public static void addProfileName(string _profileName)
	{
		string @string = ProfileSDF.profileSDF.GetString("profileNames");
		if (@string == null || @string.Length == 0)
		{
			ProfileSDF.profileSDF.Set("profileNames", _profileName, false);
			return;
		}
		ProfileSDF.profileSDF.Set("profileNames", ProfileSDF.profileSDF.GetString("profileNames") + "," + _profileName, false);
	}

	// Token: 0x060047F1 RID: 18417 RVA: 0x001C2A48 File Offset: 0x001C0C48
	public static void removeProfileName(string _profileName)
	{
		string @string = ProfileSDF.profileSDF.GetString("profileNames");
		if (@string == null || @string.Length == 0)
		{
			ProfileSDF.profileSDF.Set("profileNames", "", false);
			return;
		}
		string text = "";
		bool flag = true;
		foreach (string text2 in @string.Split(',', StringSplitOptions.None))
		{
			if (text2 != _profileName)
			{
				if (!flag)
				{
					text += ",";
				}
				text += text2;
				flag = false;
			}
		}
		ProfileSDF.profileSDF.Set("profileNames", text, false);
	}

	// Token: 0x060047F2 RID: 18418 RVA: 0x001C2AE5 File Offset: 0x001C0CE5
	public static void setSex(string _profileName, bool _isMale)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".isMale", _isMale);
	}

	// Token: 0x060047F3 RID: 18419 RVA: 0x001C2AFD File Offset: 0x001C0CFD
	public static void setRace(string _profileName, string _raceName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".race", _raceName);
	}

	// Token: 0x060047F4 RID: 18420 RVA: 0x001C2B15 File Offset: 0x001C0D15
	public static void setVariant(string _profileName, int _variantNumber)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".variant", _variantNumber);
	}

	// Token: 0x060047F5 RID: 18421 RVA: 0x001C2B30 File Offset: 0x001C0D30
	public static void setColor(string _profileName, string _colorName, Color _color)
	{
		ProfileSDF.profileSDF.Set(_profileName + "." + _colorName + ".r", _color.r);
		ProfileSDF.profileSDF.Set(_profileName + "." + _colorName + ".g", _color.g);
		ProfileSDF.profileSDF.Set(_profileName + "." + _colorName + ".b", _color.b);
		ProfileSDF.profileSDF.Set(_profileName + "." + _colorName + ".a", _color.a);
	}

	// Token: 0x060047F6 RID: 18422 RVA: 0x001C2BC1 File Offset: 0x001C0DC1
	public static void setEyeColor(string _profileName, string _eyeColorName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".eyeColor", _eyeColorName);
	}

	// Token: 0x060047F7 RID: 18423 RVA: 0x001C2BD9 File Offset: 0x001C0DD9
	public static void setHairName(string _profileName, string _hairName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".hairName", _hairName);
	}

	// Token: 0x060047F8 RID: 18424 RVA: 0x001C2BF1 File Offset: 0x001C0DF1
	public static void setHairColor(string _profileName, string _hairColorName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".hairColor", _hairColorName);
	}

	// Token: 0x060047F9 RID: 18425 RVA: 0x001C2C09 File Offset: 0x001C0E09
	public static void setMustacheName(string _profileName, string _mustacheName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".mustacheName", _mustacheName);
	}

	// Token: 0x060047FA RID: 18426 RVA: 0x001C2C21 File Offset: 0x001C0E21
	public static void setChopsName(string _profileName, string _chopsName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".chopsName", _chopsName);
	}

	// Token: 0x060047FB RID: 18427 RVA: 0x001C2C39 File Offset: 0x001C0E39
	public static void setBeardName(string _profileName, string _beardName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".beardName", _beardName);
	}

	// Token: 0x060047FC RID: 18428 RVA: 0x001C2C51 File Offset: 0x001C0E51
	public static void setEyebrowName(string _profileName, string _eyebrowName)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".eyebrowName", _eyebrowName);
	}

	// Token: 0x060047FD RID: 18429 RVA: 0x001C2C69 File Offset: 0x001C0E69
	public static void setArchetype(string _profileName, string _archetype)
	{
		ProfileSDF.profileSDF.Set(_profileName + ".archetype", _archetype);
	}

	// Token: 0x060047FE RID: 18430 RVA: 0x001C2C84 File Offset: 0x001C0E84
	public static Color GetSkinColor(string _profileName)
	{
		float r = ProfileSDF.profileSDF.GetFloat(_profileName + ".skin.r").GetValueOrDefault();
		float g = ProfileSDF.profileSDF.GetFloat(_profileName + ".skin.g").GetValueOrDefault();
		float b = ProfileSDF.profileSDF.GetFloat(_profileName + ".skin.b").GetValueOrDefault();
		float a = ProfileSDF.profileSDF.GetFloat(_profileName + ".skin.a").GetValueOrDefault();
		return new Color(r, g, b, a);
	}

	// Token: 0x060047FF RID: 18431 RVA: 0x001C2D14 File Offset: 0x001C0F14
	public static Color GetEyebrowColor(string _profileName)
	{
		float r = ProfileSDF.profileSDF.GetFloat(_profileName + ".eyebrow.r").GetValueOrDefault();
		float g = ProfileSDF.profileSDF.GetFloat(_profileName + ".eyebrow.g").GetValueOrDefault();
		float b = ProfileSDF.profileSDF.GetFloat(_profileName + ".eyebrow.b").GetValueOrDefault();
		float a = ProfileSDF.profileSDF.GetFloat(_profileName + ".eyebrow.a").GetValueOrDefault();
		return new Color(r, g, b, a);
	}

	// Token: 0x06004800 RID: 18432 RVA: 0x001C2DA4 File Offset: 0x001C0FA4
	public static bool GetIsMale(string _profileName)
	{
		return ProfileSDF.profileSDF.GetBool(_profileName + ".isMale").GetValueOrDefault();
	}

	// Token: 0x06004801 RID: 18433 RVA: 0x001C2DD0 File Offset: 0x001C0FD0
	public static string GetRaceName(string _profileName)
	{
		string @string = ProfileSDF.profileSDF.GetString(_profileName + ".race");
		if (@string == null)
		{
			return "White";
		}
		return @string;
	}

	// Token: 0x06004802 RID: 18434 RVA: 0x001C2E00 File Offset: 0x001C1000
	public static int GetVariantNumber(string _profileName)
	{
		return ProfileSDF.profileSDF.GetInt(_profileName + ".variant").GetValueOrDefault();
	}

	// Token: 0x06004803 RID: 18435 RVA: 0x001C2E2C File Offset: 0x001C102C
	public static string GetArchetype(string _profileName)
	{
		string @string = ProfileSDF.profileSDF.GetString(_profileName + ".archetype");
		if (@string != null)
		{
			return @string;
		}
		if (!ProfileSDF.GetIsMale(_profileName))
		{
			return "BaseFemale";
		}
		return "BaseMale";
	}

	// Token: 0x06004804 RID: 18436 RVA: 0x001C2E68 File Offset: 0x001C1068
	public static float GetBodyDna(string _profileName, string _bodyPartName)
	{
		return Mathf.Clamp01(ProfileSDF.profileSDF.GetFloat(_profileName + ".bodyData." + _bodyPartName).GetValueOrDefault());
	}

	// Token: 0x06004805 RID: 18437 RVA: 0x001C2E98 File Offset: 0x001C1098
	public static string GetEyeColorName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".eyeColor");
	}

	// Token: 0x06004806 RID: 18438 RVA: 0x001C2EAF File Offset: 0x001C10AF
	public static string GetHairName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".hairName");
	}

	// Token: 0x06004807 RID: 18439 RVA: 0x001C2EC6 File Offset: 0x001C10C6
	public static string GetHairColorName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".hairColor");
	}

	// Token: 0x06004808 RID: 18440 RVA: 0x001C2EDD File Offset: 0x001C10DD
	public static string GetMustacheName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".mustacheName");
	}

	// Token: 0x06004809 RID: 18441 RVA: 0x001C2EF4 File Offset: 0x001C10F4
	public static string GetChopsName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".chopsName");
	}

	// Token: 0x0600480A RID: 18442 RVA: 0x001C2F0B File Offset: 0x001C110B
	public static string GetBeardName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".beardName");
	}

	// Token: 0x0600480B RID: 18443 RVA: 0x001C2F22 File Offset: 0x001C1122
	public static string GetEyebrowName(string _profileName)
	{
		return ProfileSDF.profileSDF.GetString(_profileName + ".eyebrowName");
	}

	// Token: 0x0600480C RID: 18444 RVA: 0x001C2F3C File Offset: 0x001C113C
	public static string[] GetProfiles()
	{
		string @string = ProfileSDF.profileSDF.GetString("profileNames");
		if (@string == null)
		{
			return new string[0];
		}
		if (@string.Contains(","))
		{
			return ProfileSDF.profileSDF.GetString("profileNames").Split(',', StringSplitOptions.None);
		}
		return new string[]
		{
			@string
		};
	}

	// Token: 0x04003716 RID: 14102
	[PublicizedFrom(EAccessModifier.Private)]
	public static SdfFile profileSDF = new SdfFile();

	// Token: 0x04003717 RID: 14103
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PROFILE_NAMES = "profileNames";
}

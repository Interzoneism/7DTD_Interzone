using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

// Token: 0x02000955 RID: 2389
public static class SDCSDataUtils
{
	// Token: 0x17000793 RID: 1939
	// (get) Token: 0x0600481E RID: 18462 RVA: 0x001C3BE1 File Offset: 0x001C1DE1
	public static string baseHairColorLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "AssetBundles/Player/Common/HairColorSwatches";
		}
	}

	// Token: 0x0600481F RID: 18463 RVA: 0x001C3BE8 File Offset: 0x001C1DE8
	public static void SetupData()
	{
		SDCSDataUtils.Load();
	}

	// Token: 0x06004820 RID: 18464 RVA: 0x001C3BF0 File Offset: 0x001C1DF0
	public static List<string> GetRaceList(bool isMale)
	{
		List<string> list = new List<string>();
		foreach (SDCSDataUtils.GenderKey genderKey in SDCSDataUtils.VariantData.Keys)
		{
			if (genderKey.IsMale == isMale)
			{
				list.Add(genderKey.Name);
			}
		}
		list.Sort((string a, string b) => b.CompareTo(a));
		return list;
	}

	// Token: 0x06004821 RID: 18465 RVA: 0x001C3C84 File Offset: 0x001C1E84
	public static List<string> GetVariantList(bool isMale, string raceName)
	{
		List<string> list = new List<string>();
		foreach (SDCSDataUtils.GenderKey genderKey in SDCSDataUtils.VariantData.Keys)
		{
			if (genderKey.IsMale == isMale && genderKey.Name.EqualsCaseInsensitive(raceName))
			{
				for (int i = 0; i < SDCSDataUtils.VariantData[genderKey].Count; i++)
				{
					list.Add(SDCSDataUtils.VariantData[genderKey][i].ToString());
				}
			}
		}
		return list;
	}

	// Token: 0x06004822 RID: 18466 RVA: 0x001C3D30 File Offset: 0x001C1F30
	public static List<string> GetHairNames(bool isMale, SDCSDataUtils.HairTypes hairType)
	{
		List<string> list = new List<string>();
		Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> dictionary = null;
		switch (hairType)
		{
		case SDCSDataUtils.HairTypes.Hair:
			dictionary = SDCSDataUtils.HairDictionary;
			break;
		case SDCSDataUtils.HairTypes.Mustache:
			dictionary = SDCSDataUtils.MustacheDictionary;
			break;
		case SDCSDataUtils.HairTypes.Chops:
			dictionary = SDCSDataUtils.ChopsDictionary;
			break;
		case SDCSDataUtils.HairTypes.Beard:
			dictionary = SDCSDataUtils.BeardDictionary;
			break;
		}
		foreach (SDCSDataUtils.GenderKey genderKey in dictionary.Keys)
		{
			SDCSDataUtils.HairData hairData = dictionary[genderKey];
			if (genderKey.IsMale == isMale)
			{
				list.Add(genderKey.Name);
			}
		}
		return list;
	}

	// Token: 0x06004823 RID: 18467 RVA: 0x001C3DD8 File Offset: 0x001C1FD8
	public static List<string> GetEyeColorNames()
	{
		return SDCSDataUtils.EyeColorList;
	}

	// Token: 0x06004824 RID: 18468 RVA: 0x001C3DE0 File Offset: 0x001C1FE0
	public static List<SDCSDataUtils.HairColorData> GetHairColorNames()
	{
		List<SDCSDataUtils.HairColorData> list = new List<SDCSDataUtils.HairColorData>();
		foreach (SDCSDataUtils.HairColorData item in SDCSDataUtils.HairColorDictionary.Values)
		{
			list.Add(item);
		}
		return list;
	}

	// Token: 0x06004825 RID: 18469 RVA: 0x001C3E40 File Offset: 0x001C2040
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupDataFromResources()
	{
		SDCSDataUtils.LoadRaceDataFromResources();
		SDCSDataUtils.EyeColorList = SDCSDataUtils.GetEyeColorNamesFromResources();
		SDCSDataUtils.LoadHairTypeFromResources(SDCSDataUtils.HairDictionary, SDCSDataUtils.HairTypes.Hair);
		SDCSDataUtils.LoadHairTypeFromResources(SDCSDataUtils.MustacheDictionary, SDCSDataUtils.HairTypes.Mustache);
		SDCSDataUtils.LoadHairTypeFromResources(SDCSDataUtils.ChopsDictionary, SDCSDataUtils.HairTypes.Chops);
		SDCSDataUtils.LoadHairTypeFromResources(SDCSDataUtils.BeardDictionary, SDCSDataUtils.HairTypes.Beard);
		SDCSDataUtils.LoadHairColorFromResources(SDCSDataUtils.HairColorDictionary);
	}

	// Token: 0x06004826 RID: 18470 RVA: 0x001C3E92 File Offset: 0x001C2092
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LoadRaceDataFromResources()
	{
		SDCSDataUtils.VariantData.Clear();
		SDCSDataUtils.ParseRaceVariantFromResources(true);
		SDCSDataUtils.ParseRaceVariantFromResources(false);
	}

	// Token: 0x06004827 RID: 18471 RVA: 0x001C3EAC File Offset: 0x001C20AC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ParseRaceVariantFromResources(bool isMale)
	{
		string str = isMale ? "Male" : "Female";
		foreach (DirectoryInfo directoryInfo in new DirectoryInfo(Application.dataPath + "/AssetBundles/Player/" + str + "/Heads/").GetDirectories())
		{
			SDCSDataUtils.GenderKey key = new SDCSDataUtils.GenderKey(directoryInfo.Name, isMale);
			if (!SDCSDataUtils.VariantData.ContainsKey(key))
			{
				SDCSDataUtils.VariantData.Add(key, new List<int>());
			}
			foreach (DirectoryInfo directoryInfo2 in new DirectoryInfo(directoryInfo.FullName).GetDirectories())
			{
				SDCSDataUtils.VariantData[key].Add(StringParsers.ParseSInt32(directoryInfo2.Name, 0, -1, NumberStyles.Integer));
			}
		}
	}

	// Token: 0x06004828 RID: 18472 RVA: 0x001C3F7C File Offset: 0x001C217C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LoadHairTypeFromResources(Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> dict, SDCSDataUtils.HairTypes hairType)
	{
		dict.Clear();
		List<string> hairNamesFromResources = SDCSDataUtils.GetHairNamesFromResources(true, hairType);
		for (int i = 0; i < hairNamesFromResources.Count; i++)
		{
			dict.Add(new SDCSDataUtils.GenderKey(hairNamesFromResources[i], true), new SDCSDataUtils.HairData
			{
				Name = hairNamesFromResources[i],
				IsMale = true
			});
		}
		hairNamesFromResources = SDCSDataUtils.GetHairNamesFromResources(false, hairType);
		for (int j = 0; j < hairNamesFromResources.Count; j++)
		{
			dict.Add(new SDCSDataUtils.GenderKey(hairNamesFromResources[j], false), new SDCSDataUtils.HairData
			{
				Name = hairNamesFromResources[j],
				IsMale = false
			});
		}
	}

	// Token: 0x06004829 RID: 18473 RVA: 0x001C4028 File Offset: 0x001C2228
	[PublicizedFrom(EAccessModifier.Private)]
	public static void LoadHairColorFromResources(Dictionary<string, SDCSDataUtils.HairColorData> dict)
	{
		dict.Clear();
		List<string> hairColorNamesFromResources = SDCSDataUtils.GetHairColorNamesFromResources();
		for (int i = 0; i < hairColorNamesFromResources.Count; i++)
		{
			string text = hairColorNamesFromResources[i];
			int index = StringParsers.ParseSInt32(text.Substring(0, 2), 0, -1, NumberStyles.Integer);
			string name = text.Substring(3);
			dict.Add(hairColorNamesFromResources[i], new SDCSDataUtils.HairColorData
			{
				Index = index,
				Name = name,
				PrefabName = text
			});
		}
	}

	// Token: 0x0600482A RID: 18474 RVA: 0x001C40A4 File Offset: 0x001C22A4
	public static List<string> GetHairNamesFromResources(bool isMale, SDCSDataUtils.HairTypes hairType)
	{
		List<string> list = new List<string>();
		string str = isMale ? "Male" : "Female";
		string text = Application.dataPath + "/AssetBundles/Player/" + str + "/";
		string path = (hairType == SDCSDataUtils.HairTypes.Hair) ? (text + "Hair/") : string.Format("{0}FacialHair/{1}/", text, hairType);
		if (Directory.Exists(path))
		{
			foreach (DirectoryInfo directoryInfo in new DirectoryInfo(path).GetDirectories())
			{
				list.Add(directoryInfo.Name);
			}
		}
		return list;
	}

	// Token: 0x0600482B RID: 18475 RVA: 0x001C413C File Offset: 0x001C233C
	public static List<string> GetEyeColorNamesFromResources()
	{
		List<string> list = new List<string>();
		foreach (FileInfo fileInfo in new DirectoryInfo(Application.dataPath + "/AssetBundles/Player/Common/Eyes/Materials/").GetFiles())
		{
			if (!fileInfo.Name.EndsWith(".meta"))
			{
				list.Add(fileInfo.Name.Replace(".mat", ""));
			}
		}
		return list;
	}

	// Token: 0x0600482C RID: 18476 RVA: 0x001C41AC File Offset: 0x001C23AC
	public static List<string> GetHairColorNamesFromResources()
	{
		List<string> list = new List<string>();
		foreach (FileInfo fileInfo in new DirectoryInfo(Application.dataPath + "/" + SDCSDataUtils.baseHairColorLoc + "/").GetFiles())
		{
			if (!fileInfo.Name.EndsWith(".meta"))
			{
				list.Add(fileInfo.Name.Replace(".asset", ""));
			}
		}
		return list;
	}

	// Token: 0x0600482D RID: 18477 RVA: 0x001C4224 File Offset: 0x001C2424
	public static void Save()
	{
		SDCSDataUtils.SetupDataFromResources();
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/sdcs.xml");
		string text = "\t";
		streamWriter.WriteLine("<sdcs>");
		foreach (SDCSDataUtils.GenderKey genderKey in SDCSDataUtils.VariantData.Keys)
		{
			if (SDCSDataUtils.VariantData[genderKey].Count > 0)
			{
				for (int i = 0; i < SDCSDataUtils.VariantData[genderKey].Count; i++)
				{
					streamWriter.WriteLine(string.Format("{0}<variant race=\"{1}\" index=\"{2}\" is_male=\"{3}\" />", new object[]
					{
						text,
						genderKey.Name,
						SDCSDataUtils.VariantData[genderKey][i],
						genderKey.IsMale
					}));
				}
			}
		}
		for (int j = 0; j < SDCSDataUtils.EyeColorList.Count; j++)
		{
			streamWriter.WriteLine(string.Format("{0}<eye_color name=\"{1}\" />", text, SDCSDataUtils.EyeColorList[j]));
		}
		foreach (SDCSDataUtils.HairColorData hairColorData in SDCSDataUtils.HairColorDictionary.Values)
		{
			streamWriter.WriteLine(string.Format("{0}<hair_color index=\"{1}\" name=\"{2}\" prefab_name=\"{3}\" />", new object[]
			{
				text,
				hairColorData.Index,
				hairColorData.Name,
				hairColorData.PrefabName
			}));
		}
		foreach (SDCSDataUtils.HairData hairData in SDCSDataUtils.HairDictionary.Values)
		{
			streamWriter.WriteLine(string.Format("{0}<hair name=\"{1}\" is_male=\"{2}\" />", text, hairData.Name, hairData.IsMale));
		}
		foreach (SDCSDataUtils.HairData hairData2 in SDCSDataUtils.MustacheDictionary.Values)
		{
			streamWriter.WriteLine(string.Format("{0}<mustache name=\"{1}\" is_male=\"{2}\" />", text, hairData2.Name, hairData2.IsMale));
		}
		foreach (SDCSDataUtils.HairData hairData3 in SDCSDataUtils.ChopsDictionary.Values)
		{
			streamWriter.WriteLine(string.Format("{0}<chops name=\"{1}\" is_male=\"{2}\" />", text, hairData3.Name, hairData3.IsMale));
		}
		foreach (SDCSDataUtils.HairData hairData4 in SDCSDataUtils.BeardDictionary.Values)
		{
			streamWriter.WriteLine(string.Format("{0}<beard name=\"{1}\" is_male=\"{2}\" />", text, hairData4.Name, hairData4.IsMale));
		}
		streamWriter.WriteLine("</sdcs>");
		streamWriter.Flush();
		streamWriter.Close();
	}

	// Token: 0x0600482E RID: 18478 RVA: 0x001C4584 File Offset: 0x001C2784
	[PublicizedFrom(EAccessModifier.Private)]
	public static void Load()
	{
		XElement root = XDocument.Parse(((TextAsset)Resources.Load("sdcs")).text, LoadOptions.SetLineInfo).Root;
		if (root == null || !root.HasElements)
		{
			return;
		}
		SDCSDataUtils.VariantData.Clear();
		SDCSDataUtils.EyeColorList.Clear();
		SDCSDataUtils.HairDictionary.Clear();
		SDCSDataUtils.HairColorDictionary.Clear();
		SDCSDataUtils.MustacheDictionary.Clear();
		SDCSDataUtils.ChopsDictionary.Clear();
		SDCSDataUtils.BeardDictionary.Clear();
		foreach (XElement xelement in root.Elements())
		{
			if (xelement.Name == "variant")
			{
				int item = -1;
				string name = "";
				bool isMale = false;
				if (xelement.HasAttribute("index"))
				{
					item = StringParsers.ParseSInt32(xelement.GetAttribute("index"), 0, -1, NumberStyles.Integer);
				}
				if (xelement.HasAttribute("race"))
				{
					name = xelement.GetAttribute("race");
				}
				if (xelement.HasAttribute("is_male"))
				{
					isMale = StringParsers.ParseBool(xelement.GetAttribute("is_male"), 0, -1, true);
				}
				SDCSDataUtils.GenderKey key = new SDCSDataUtils.GenderKey(name, isMale);
				if (!SDCSDataUtils.VariantData.ContainsKey(key))
				{
					SDCSDataUtils.VariantData.Add(key, new List<int>());
				}
				SDCSDataUtils.VariantData[key].Add(item);
			}
			else if (xelement.Name == "eye_color")
			{
				SDCSDataUtils.EyeColorList.Add(xelement.GetAttribute("name"));
			}
			else if (xelement.Name == "hair_color")
			{
				int index = -1;
				string text = "";
				string prefabName = "";
				if (xelement.HasAttribute("index"))
				{
					index = StringParsers.ParseSInt32(xelement.GetAttribute("index"), 0, -1, NumberStyles.Integer);
				}
				if (xelement.HasAttribute("name"))
				{
					text = xelement.GetAttribute("name");
				}
				if (xelement.HasAttribute("prefab_name"))
				{
					prefabName = xelement.GetAttribute("prefab_name");
				}
				SDCSDataUtils.HairColorDictionary.Add(text, new SDCSDataUtils.HairColorData
				{
					Index = index,
					Name = text,
					PrefabName = prefabName
				});
			}
			else if (xelement.Name == "hair")
			{
				SDCSDataUtils.HairData hairData = SDCSDataUtils.ParseHair(xelement);
				SDCSDataUtils.HairDictionary.Add(new SDCSDataUtils.GenderKey(hairData.Name, hairData.IsMale), hairData);
			}
			else if (xelement.Name == "mustache")
			{
				SDCSDataUtils.HairData hairData2 = SDCSDataUtils.ParseHair(xelement);
				SDCSDataUtils.MustacheDictionary.Add(new SDCSDataUtils.GenderKey(hairData2.Name, hairData2.IsMale), hairData2);
			}
			else if (xelement.Name == "chops")
			{
				SDCSDataUtils.HairData hairData3 = SDCSDataUtils.ParseHair(xelement);
				SDCSDataUtils.ChopsDictionary.Add(new SDCSDataUtils.GenderKey(hairData3.Name, hairData3.IsMale), hairData3);
			}
			else if (xelement.Name == "beard")
			{
				SDCSDataUtils.HairData hairData4 = SDCSDataUtils.ParseHair(xelement);
				SDCSDataUtils.BeardDictionary.Add(new SDCSDataUtils.GenderKey(hairData4.Name, hairData4.IsMale), hairData4);
			}
		}
	}

	// Token: 0x0600482F RID: 18479 RVA: 0x001C493C File Offset: 0x001C2B3C
	[PublicizedFrom(EAccessModifier.Private)]
	public static SDCSDataUtils.HairData ParseHair(XElement element)
	{
		string name = "";
		bool isMale = false;
		if (element.HasAttribute("name"))
		{
			name = element.GetAttribute("name");
		}
		if (element.HasAttribute("is_male"))
		{
			isMale = StringParsers.ParseBool(element.GetAttribute("is_male"), 0, -1, true);
		}
		return new SDCSDataUtils.HairData
		{
			Name = name,
			IsMale = isMale
		};
	}

	// Token: 0x0400374E RID: 14158
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<SDCSDataUtils.GenderKey, List<int>> VariantData = new Dictionary<SDCSDataUtils.GenderKey, List<int>>();

	// Token: 0x0400374F RID: 14159
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> HairDictionary = new Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData>();

	// Token: 0x04003750 RID: 14160
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> MustacheDictionary = new Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData>();

	// Token: 0x04003751 RID: 14161
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> ChopsDictionary = new Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData>();

	// Token: 0x04003752 RID: 14162
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData> BeardDictionary = new Dictionary<SDCSDataUtils.GenderKey, SDCSDataUtils.HairData>();

	// Token: 0x04003753 RID: 14163
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<string> EyeColorList = new List<string>();

	// Token: 0x04003754 RID: 14164
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<string, SDCSDataUtils.HairColorData> HairColorDictionary = new Dictionary<string, SDCSDataUtils.HairColorData>();

	// Token: 0x02000956 RID: 2390
	public struct HairColorData
	{
		// Token: 0x04003755 RID: 14165
		public int Index;

		// Token: 0x04003756 RID: 14166
		public string Name;

		// Token: 0x04003757 RID: 14167
		public string PrefabName;
	}

	// Token: 0x02000957 RID: 2391
	public struct HairData
	{
		// Token: 0x04003758 RID: 14168
		public string Name;

		// Token: 0x04003759 RID: 14169
		public bool IsMale;
	}

	// Token: 0x02000958 RID: 2392
	public struct GenderKey
	{
		// Token: 0x06004831 RID: 18481 RVA: 0x001C4A0B File Offset: 0x001C2C0B
		public GenderKey(string name, bool isMale)
		{
			this.Name = name;
			this.IsMale = isMale;
		}

		// Token: 0x0400375A RID: 14170
		public string Name;

		// Token: 0x0400375B RID: 14171
		public bool IsMale;
	}

	// Token: 0x02000959 RID: 2393
	public enum HairTypes
	{
		// Token: 0x0400375D RID: 14173
		Hair,
		// Token: 0x0400375E RID: 14174
		Mustache,
		// Token: 0x0400375F RID: 14175
		Chops,
		// Token: 0x04003760 RID: 14176
		Beard
	}
}

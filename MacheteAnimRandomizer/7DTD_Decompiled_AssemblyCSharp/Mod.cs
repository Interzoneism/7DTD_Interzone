using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using JetBrains.Annotations;
using Platform;

// Token: 0x0200067E RID: 1662
public class Mod
{
	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x060031C8 RID: 12744 RVA: 0x00153B29 File Offset: 0x00151D29
	// (set) Token: 0x060031C9 RID: 12745 RVA: 0x00153B31 File Offset: 0x00151D31
	public Mod.EModLoadState LoadState { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004BB RID: 1211
	// (get) Token: 0x060031CA RID: 12746 RVA: 0x00153B3A File Offset: 0x00151D3A
	// (set) Token: 0x060031CB RID: 12747 RVA: 0x00153B42 File Offset: 0x00151D42
	public string Path { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004BC RID: 1212
	// (get) Token: 0x060031CC RID: 12748 RVA: 0x00153B4B File Offset: 0x00151D4B
	// (set) Token: 0x060031CD RID: 12749 RVA: 0x00153B53 File Offset: 0x00151D53
	public string FolderName { [UsedImplicitly] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004BD RID: 1213
	// (get) Token: 0x060031CE RID: 12750 RVA: 0x00153B5C File Offset: 0x00151D5C
	// (set) Token: 0x060031CF RID: 12751 RVA: 0x00153B64 File Offset: 0x00151D64
	public string Name { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004BE RID: 1214
	// (get) Token: 0x060031D0 RID: 12752 RVA: 0x00153B6D File Offset: 0x00151D6D
	// (set) Token: 0x060031D1 RID: 12753 RVA: 0x00153B75 File Offset: 0x00151D75
	public string DisplayName { [UsedImplicitly] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004BF RID: 1215
	// (get) Token: 0x060031D2 RID: 12754 RVA: 0x00153B7E File Offset: 0x00151D7E
	// (set) Token: 0x060031D3 RID: 12755 RVA: 0x00153B86 File Offset: 0x00151D86
	public string Description { [UsedImplicitly] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C0 RID: 1216
	// (get) Token: 0x060031D4 RID: 12756 RVA: 0x00153B8F File Offset: 0x00151D8F
	// (set) Token: 0x060031D5 RID: 12757 RVA: 0x00153B97 File Offset: 0x00151D97
	public string Author { [UsedImplicitly] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C1 RID: 1217
	// (get) Token: 0x060031D6 RID: 12758 RVA: 0x00153BA0 File Offset: 0x00151DA0
	// (set) Token: 0x060031D7 RID: 12759 RVA: 0x00153BA8 File Offset: 0x00151DA8
	public Version Version { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C2 RID: 1218
	// (get) Token: 0x060031D8 RID: 12760 RVA: 0x00153BB1 File Offset: 0x00151DB1
	// (set) Token: 0x060031D9 RID: 12761 RVA: 0x00153BB9 File Offset: 0x00151DB9
	public string VersionString { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C3 RID: 1219
	// (get) Token: 0x060031DA RID: 12762 RVA: 0x00153BC2 File Offset: 0x00151DC2
	// (set) Token: 0x060031DB RID: 12763 RVA: 0x00153BCA File Offset: 0x00151DCA
	public string Website { [UsedImplicitly] get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C4 RID: 1220
	// (get) Token: 0x060031DC RID: 12764 RVA: 0x00153BD3 File Offset: 0x00151DD3
	// (set) Token: 0x060031DD RID: 12765 RVA: 0x00153BDB File Offset: 0x00151DDB
	public bool SkipLoadingWithAntiCheat { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C5 RID: 1221
	// (get) Token: 0x060031DE RID: 12766 RVA: 0x00153BE4 File Offset: 0x00151DE4
	// (set) Token: 0x060031DF RID: 12767 RVA: 0x00153BEC File Offset: 0x00151DEC
	public bool AntiCheatCompatible { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x170004C6 RID: 1222
	// (get) Token: 0x060031E0 RID: 12768 RVA: 0x00153BF5 File Offset: 0x00151DF5
	// (set) Token: 0x060031E1 RID: 12769 RVA: 0x00153BFD File Offset: 0x00151DFD
	public bool GameConfigMod { get; [PublicizedFrom(EAccessModifier.Private)] set; }

	// Token: 0x060031E2 RID: 12770 RVA: 0x00153C06 File Offset: 0x00151E06
	[PublicizedFrom(EAccessModifier.Private)]
	public Mod()
	{
		this.AllAssemblies = new ReadOnlyCollection<Assembly>(this.allAssemblies);
	}

	// Token: 0x060031E3 RID: 12771 RVA: 0x00153C2C File Offset: 0x00151E2C
	public bool LoadMod()
	{
		this.LoadState = Mod.EModLoadState.Failed;
		if (ModManager.ModLoaded(this.Name))
		{
			Log.Warning("[MODS]     Mod with same name (" + this.Name + ") already loaded, ignoring");
			this.LoadState = Mod.EModLoadState.DuplicateModName;
			return false;
		}
		Mod.EModLoadState emodLoadState = this.LoadAssemblies();
		if (emodLoadState != Mod.EModLoadState.Success)
		{
			this.LoadState = emodLoadState;
			return false;
		}
		this.DetectContents();
		Log.Out(string.Concat(new string[]
		{
			"[MODS]     Loaded Mod: ",
			this.Name,
			" (",
			this.VersionString ?? "<unknown version>",
			")"
		}));
		this.LoadState = Mod.EModLoadState.Success;
		return this.LoadState == Mod.EModLoadState.Success;
	}

	// Token: 0x060031E4 RID: 12772 RVA: 0x00153CE0 File Offset: 0x00151EE0
	[PublicizedFrom(EAccessModifier.Private)]
	public Mod.EModLoadState LoadAssemblies()
	{
		string[] files = SdDirectory.GetFiles(this.Path);
		if (files.Length == 0)
		{
			return Mod.EModLoadState.Success;
		}
		foreach (string text in files)
		{
			if (text.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
			{
				if (!GameManager.IsDedicatedServer)
				{
					IAntiCheatClient antiCheatClient = PlatformManager.MultiPlatform.AntiCheatClient;
					if (antiCheatClient != null && antiCheatClient.ClientAntiCheatEnabled())
					{
						if (this.SkipLoadingWithAntiCheat)
						{
							Log.Out("[MODS]     AntiCheat enabled, mod skipped because it is set not to load");
							return Mod.EModLoadState.SkippedDueToAntiCheat;
						}
						if (!this.AntiCheatCompatible)
						{
							Log.Warning("[MODS]     Mod contains custom code, AntiCheat needs to be disabled to load it!");
							return Mod.EModLoadState.NotAntiCheatCompatible;
						}
					}
				}
				try
				{
					this.allAssemblies.Add(Assembly.LoadFrom(text));
				}
				catch (Exception e)
				{
					Log.Error("[MODS]     Failed loading DLL " + text);
					Log.Exception(e);
					return Mod.EModLoadState.FailedLoadingAssembly;
				}
			}
		}
		return Mod.EModLoadState.Success;
	}

	// Token: 0x060031E5 RID: 12773 RVA: 0x00153DB0 File Offset: 0x00151FB0
	public bool InitModCode()
	{
		if (this.allAssemblies.Count > 0)
		{
			Log.Out("[MODS]   Initializing mod " + this.Name);
			bool flag = false;
			Type typeFromHandle = typeof(IModApi);
			foreach (Assembly assembly in this.allAssemblies)
			{
				try
				{
					foreach (Type type in assembly.GetTypes())
					{
						if (typeFromHandle.IsAssignableFrom(type))
						{
							Log.Out("[MODS]     Found ModAPI in " + System.IO.Path.GetFileName(assembly.Location) + ", creating instance");
							IModApi modApi = (IModApi)Activator.CreateInstance(type);
							try
							{
								modApi.InitMod(this);
								Log.Out(string.Concat(new string[]
								{
									"[MODS]     Initialized code in mod '",
									this.Name,
									"' from DLL '",
									System.IO.Path.GetFileName(assembly.Location),
									"'"
								}));
							}
							catch (Exception e)
							{
								Log.Error(string.Concat(new string[]
								{
									"[MODS]     Failed initializing ModAPI instance on mod '",
									this.Name,
									"' from DLL '",
									System.IO.Path.GetFileName(assembly.Location),
									"'"
								}));
								Log.Exception(e);
							}
							flag = true;
						}
					}
				}
				catch (ReflectionTypeLoadException)
				{
					Log.Warning("[MODS]     Failed iterating types in DLL " + System.IO.Path.GetFileName(assembly.Location));
				}
				catch (Exception e2)
				{
					Log.Error("[MODS]     Failed creating ModAPI instance from DLL " + System.IO.Path.GetFileName(assembly.Location));
					Log.Exception(e2);
					return false;
				}
			}
			if (!flag)
			{
				Log.Out("[MODS]     No ModAPI found in mod DLLs");
				return true;
			}
			return true;
		}
		return true;
	}

	// Token: 0x060031E6 RID: 12774 RVA: 0x00153FD4 File Offset: 0x001521D4
	public bool ContainsAssembly(Assembly _assembly)
	{
		return this.allAssemblies.Contains(_assembly);
	}

	// Token: 0x060031E7 RID: 12775 RVA: 0x00153FE4 File Offset: 0x001521E4
	[PublicizedFrom(EAccessModifier.Private)]
	public void DetectContents()
	{
		string path = this.Path + "/Config";
		if (!SdDirectory.Exists(path))
		{
			return;
		}
		string[] fileSystemEntries = SdDirectory.GetFileSystemEntries(path);
		for (int i = 0; i < fileSystemEntries.Length; i++)
		{
			string fileName = System.IO.Path.GetFileName(fileSystemEntries[i]);
			if (!fileName.EqualsCaseInsensitive("XUi_Menu") && !fileName.EqualsCaseInsensitive("loadingscreen.xml") && !fileName.EqualsCaseInsensitive("Localization.txt"))
			{
				this.GameConfigMod = true;
				return;
			}
		}
	}

	// Token: 0x060031E8 RID: 12776 RVA: 0x00154058 File Offset: 0x00152258
	public static Mod LoadDefinitionFromFolder(string _path)
	{
		string text = _path + "/ModInfo.xml";
		string fileName = System.IO.Path.GetFileName(_path);
		if (!SdFile.Exists(text))
		{
			Log.Warning("[MODS]     Folder " + fileName + " does not contain a ModInfo.xml, ignoring");
			return null;
		}
		XmlFile xmlFile = new XmlFile(_path, "ModInfo.xml", false, false);
		XElement root = xmlFile.XmlDoc.Root;
		if (root == null)
		{
			Log.Error("[MODS]     " + fileName + "/ModInfo.xml does not have a root element, ignoring");
			return null;
		}
		Mod mod = (root.Element("ModInfo") != null) ? Mod.parseModInfoV1(_path, fileName, text, xmlFile) : Mod.parseModInfoV2(_path, fileName, root);
		if (mod == null)
		{
			Log.Error("[MODS]     Could not parse " + fileName + "/ModInfo.xml, ignoring");
			return null;
		}
		return mod;
	}

	// Token: 0x060031E9 RID: 12777 RVA: 0x00154110 File Offset: 0x00152310
	[PublicizedFrom(EAccessModifier.Private)]
	public static Mod parseModInfoV2(string _modPath, string _folderName, XElement _xmlRoot)
	{
		string elementAttributeValue = Mod.getElementAttributeValue(_folderName, _xmlRoot, "Name", true);
		if (elementAttributeValue == null)
		{
			return null;
		}
		if (elementAttributeValue.Length == 0)
		{
			Log.Error("[MODS]     " + _folderName + "/ModInfo.xml does not specify a non-empty Name, ignoring");
			return null;
		}
		if (!Mod.nameValidationRegex.IsMatch(elementAttributeValue))
		{
			Log.Error(string.Format("[MODS]     {0}/ModInfo.xml does not define a valid non-empty Name ({1}), ignoring", _folderName, Mod.nameValidationRegex));
			return null;
		}
		Version version = null;
		string text = Mod.getElementAttributeValue(_folderName, _xmlRoot, "Version", true);
		if (text != null)
		{
			if (text.Length == 0)
			{
				text = null;
			}
			else
			{
				Version.TryParse(text, out version);
			}
		}
		if (version == null)
		{
			Log.Warning("[MODS]     " + _folderName + "/ModInfo.xml does not define a valid Version. Please consider updating it for future compatibility.");
		}
		string elementAttributeValue2 = Mod.getElementAttributeValue(_folderName, _xmlRoot, "DisplayName", false);
		if (string.IsNullOrEmpty(elementAttributeValue2))
		{
			Log.Error("[MODS]     " + _folderName + "/ModInfo.xml does not define a non-empty DisplayName, ignoring");
			return null;
		}
		string elementAttributeValue3 = Mod.getElementAttributeValue(_folderName, _xmlRoot, "Description", false);
		string elementAttributeValue4 = Mod.getElementAttributeValue(_folderName, _xmlRoot, "Author", false);
		string elementAttributeValue5 = Mod.getElementAttributeValue(_folderName, _xmlRoot, "Website", false);
		string elementAttributeValue6 = Mod.getElementAttributeValue(_folderName, _xmlRoot, "SkipWithAntiCheat", false);
		bool skipLoadingWithAntiCheat = false;
		if (!string.IsNullOrEmpty(elementAttributeValue6) && !StringParsers.TryParseBool(elementAttributeValue6, out skipLoadingWithAntiCheat, 0, -1, true))
		{
			Log.Warning("[MODS]     " + _folderName + "/ModInfo.xml does have a SkipWithAntiCheat, but its value is not a valid boolean. Assuming 'false'");
			skipLoadingWithAntiCheat = false;
		}
		return new Mod
		{
			Path = _modPath,
			FolderName = _folderName,
			Name = elementAttributeValue,
			DisplayName = elementAttributeValue2,
			Description = elementAttributeValue3,
			Author = elementAttributeValue4,
			Version = version,
			VersionString = text,
			Website = elementAttributeValue5,
			SkipLoadingWithAntiCheat = skipLoadingWithAntiCheat
		};
	}

	// Token: 0x060031EA RID: 12778 RVA: 0x001542A4 File Offset: 0x001524A4
	[PublicizedFrom(EAccessModifier.Private)]
	public static string getElementAttributeValue(string _folderName, XElement _xmlParent, string _elementName, bool _logNonExisting = true)
	{
		List<XElement> list = _xmlParent.Elements(_elementName).ToList<XElement>();
		if (list.Count != 1)
		{
			if (_logNonExisting)
			{
				Log.Error(string.Concat(new string[]
				{
					"[MODS] ",
					_folderName,
					"/ModInfo.xml does not have exactly one '",
					_elementName,
					"' element, ignoring"
				}));
			}
			return null;
		}
		XAttribute xattribute = list[0].Attribute("value");
		if (xattribute == null)
		{
			Log.Error(string.Concat(new string[]
			{
				"[MODS] ",
				_folderName,
				"/ModInfo.xml '",
				_elementName,
				"' element does not have a 'value' attribute, ignoring"
			}));
			return null;
		}
		return xattribute.Value;
	}

	// Token: 0x060031EB RID: 12779 RVA: 0x00154353 File Offset: 0x00152553
	[PublicizedFrom(EAccessModifier.Private)]
	public static Mod parseModInfoV1(string _modPath, string _folderName, string _modInfoFilename, XmlFile _xml)
	{
		Log.Error("[MODS]     " + _folderName + "/ModInfo.xml in legacy format. V2 required to load mod");
		return null;
	}

	// Token: 0x060031EC RID: 12780 RVA: 0x0015436B File Offset: 0x0015256B
	public override string ToString()
	{
		return this.DisplayName;
	}

	// Token: 0x040028AB RID: 10411
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<Assembly> allAssemblies = new List<Assembly>();

	// Token: 0x040028AC RID: 10412
	public readonly ReadOnlyCollection<Assembly> AllAssemblies;

	// Token: 0x040028AE RID: 10414
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex nameValidationRegex = new Regex("^[0-9a-zA-Z_\\-]+$", RegexOptions.Compiled);

	// Token: 0x0200067F RID: 1663
	public enum EModLoadState
	{
		// Token: 0x040028B0 RID: 10416
		LoadNotRequested,
		// Token: 0x040028B1 RID: 10417
		Success,
		// Token: 0x040028B2 RID: 10418
		NotAntiCheatCompatible,
		// Token: 0x040028B3 RID: 10419
		SkippedDueToAntiCheat,
		// Token: 0x040028B4 RID: 10420
		DuplicateModName,
		// Token: 0x040028B5 RID: 10421
		FailedLoadingAssembly,
		// Token: 0x040028B6 RID: 10422
		Failed
	}
}

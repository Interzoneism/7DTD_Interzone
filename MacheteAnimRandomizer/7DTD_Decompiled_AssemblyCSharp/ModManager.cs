using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

// Token: 0x0200069E RID: 1694
public static class ModManager
{
	// Token: 0x170004C8 RID: 1224
	// (get) Token: 0x0600320F RID: 12815 RVA: 0x0015486C File Offset: 0x00152A6C
	public static string ModsBasePath
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return GameIO.GetUserGameDataDir() + "/Mods";
		}
	}

	// Token: 0x06003210 RID: 12816 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public static void initModManager()
	{
	}

	// Token: 0x06003211 RID: 12817 RVA: 0x00154880 File Offset: 0x00152A80
	public static void LoadMods()
	{
		ModManager.initModManager();
		bool flag = ModManager.loadModsFromFolder(ModManager.ModsBasePath);
		bool flag2 = GameIO.PathsEquals(ModManager.ModsBasePath, ModManager.ModsBasePathLegacy, true) || ModManager.loadModsFromFolder(ModManager.ModsBasePathLegacy);
		if (!flag && !flag2)
		{
			Log.Out("[MODS] No mods folder found");
			return;
		}
		int num = ModManager.loadedMods.list.FindIndex((Mod _mod) => _mod.Name == "TFP_Harmony");
		if (num >= 0)
		{
			Mod item = ModManager.loadedMods.list[num];
			ModManager.loadedMods.list.RemoveAt(num);
			ModManager.loadedMods.list.Insert(0, item);
		}
		Log.Out("[MODS] Initializing mod code");
		foreach (Mod mod in ModManager.loadedMods.list)
		{
			mod.InitModCode();
		}
		Log.Out("[MODS] Loading done");
	}

	// Token: 0x06003212 RID: 12818 RVA: 0x00154990 File Offset: 0x00152B90
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool loadModsFromFolder(string _folder)
	{
		if (!SdDirectory.Exists(_folder))
		{
			return false;
		}
		Log.Out("[MODS] Start loading from: '" + _folder + "'");
		string[] directories = SdDirectory.GetDirectories(_folder);
		Array.Sort<string>(directories);
		foreach (string path in directories)
		{
			Log.Out("[MODS]   Trying to load from folder: '" + Path.GetFileName(path) + "'");
			try
			{
				Mod mod = Mod.LoadDefinitionFromFolder(path);
				if (mod != null)
				{
					if (!mod.LoadMod())
					{
						ModManager.failedMods.Add(mod);
					}
					else
					{
						ModManager.loadedMods.Add(mod.Name, mod);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error("[MODS]     Failed loading mod from folder: '" + Path.GetFileName(path) + "'");
				Log.Exception(e);
			}
		}
		return true;
	}

	// Token: 0x06003213 RID: 12819 RVA: 0x00154A5C File Offset: 0x00152C5C
	public static bool ModLoaded(string _modName)
	{
		return ModManager.loadedMods.dict.ContainsKey(_modName);
	}

	// Token: 0x06003214 RID: 12820 RVA: 0x00154A6E File Offset: 0x00152C6E
	public static Mod GetMod(string _modName, bool _onlyLoaded = false)
	{
		if (!ModManager.ModLoaded(_modName))
		{
			return null;
		}
		return ModManager.loadedMods.dict[_modName];
	}

	// Token: 0x06003215 RID: 12821 RVA: 0x00154A8A File Offset: 0x00152C8A
	public static List<Mod> GetLoadedMods()
	{
		return ModManager.loadedMods.list;
	}

	// Token: 0x06003216 RID: 12822 RVA: 0x00154A98 File Offset: 0x00152C98
	public static List<Mod> GetFailedMods(Mod.EModLoadState? _failureReason = null)
	{
		if (_failureReason == null)
		{
			return ModManager.failedMods;
		}
		List<Mod> list = new List<Mod>();
		foreach (Mod mod in ModManager.failedMods)
		{
			if (mod.LoadState == _failureReason.Value)
			{
				list.Add(mod);
			}
		}
		return list;
	}

	// Token: 0x06003217 RID: 12823 RVA: 0x00154B10 File Offset: 0x00152D10
	public static List<Assembly> GetLoadedAssemblies()
	{
		List<Assembly> list = new List<Assembly>();
		for (int i = 0; i < ModManager.loadedMods.Count; i++)
		{
			Mod mod = ModManager.loadedMods.list[i];
			list.AddRange(mod.AllAssemblies);
		}
		return list;
	}

	// Token: 0x06003218 RID: 12824 RVA: 0x00154B58 File Offset: 0x00152D58
	public static Mod GetModForAssembly(Assembly _asm)
	{
		for (int i = 0; i < ModManager.loadedMods.Count; i++)
		{
			Mod mod = ModManager.loadedMods.list[i];
			if (mod.ContainsAssembly(_asm))
			{
				return mod;
			}
		}
		return null;
	}

	// Token: 0x06003219 RID: 12825 RVA: 0x00154B98 File Offset: 0x00152D98
	public static bool AnyConfigModActive()
	{
		for (int i = 0; i < ModManager.loadedMods.Count; i++)
		{
			if (ModManager.loadedMods.list[i].GameConfigMod)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x0600321A RID: 12826 RVA: 0x00154BD4 File Offset: 0x00152DD4
	public static string PatchModPathString(string _pathString)
	{
		if (_pathString.IndexOf('@') < 0)
		{
			return null;
		}
		int num = _pathString.IndexOf("@modfolder(", StringComparison.OrdinalIgnoreCase);
		if (num < 0)
		{
			return null;
		}
		int num2 = _pathString.IndexOf("):", StringComparison.Ordinal);
		int num3 = num + "@modfolder(".Length;
		string text = _pathString.Substring(num3, num2 - num3);
		string str = _pathString.Substring(0, num);
		int num4 = num2 + 2;
		while (_pathString[num4] == '/')
		{
			num4++;
		}
		string str2 = _pathString.Substring(num4);
		Mod mod = ModManager.GetMod(text, true);
		if (mod != null)
		{
			_pathString = str + mod.Path + "/" + str2;
			return _pathString;
		}
		Log.Error("[MODS] Mod reference for a mod that is not loaded: '" + text + "'");
		return null;
	}

	// Token: 0x0600321B RID: 12827 RVA: 0x00154C91 File Offset: 0x00152E91
	public static IEnumerator LoadPatchStuff(bool _isLoadingInGame)
	{
		yield return ModManager.LoadUiAtlases(_isLoadingInGame);
		yield return ModManager.LoadLocalizations(_isLoadingInGame);
		yield break;
	}

	// Token: 0x0600321C RID: 12828 RVA: 0x00154CA0 File Offset: 0x00152EA0
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadUiAtlases(bool _isLoadingInGame)
	{
		if (GameManager.IsDedicatedServer)
		{
			yield break;
		}
		if (_isLoadingInGame)
		{
			yield break;
		}
		int num;
		for (int i = 0; i < ModManager.loadedMods.Count; i = num + 1)
		{
			Mod mod = ModManager.loadedMods.list[i];
			string path = mod.Path + "/UIAtlases";
			if (SdDirectory.Exists(path))
			{
				string[] array = null;
				try
				{
					array = SdDirectory.GetDirectories(path);
				}
				catch (Exception e)
				{
					Log.Exception(e);
				}
				if (array != null)
				{
					string[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						string text = array2[j];
						string fileName = Path.GetFileName(text);
						ModManager.AtlasManagerEntry ame;
						if (!ModManager.atlasManagers.TryGetValue(fileName, out ame))
						{
							Log.Out(string.Concat(new string[]
							{
								"[MODS] Creating new atlas '",
								fileName,
								"' for mod '",
								mod.Name,
								"'"
							}));
							ModManager.RegisterAtlasManager(MultiSourceAtlasManager.Create(ModManager.atlasesParentGo, fileName), true, ModManager.defaultShader, null);
							ame = ModManager.atlasManagers[fileName];
						}
						yield return UIAtlasFromFolder.CreateUiAtlasFromFolder(text, ame.Shader, delegate(UIAtlas _atlas)
						{
							_atlas.transform.parent = ame.Manager.transform;
							ame.Manager.AddAtlas(_atlas, _isLoadingInGame);
							Action<UIAtlas, bool> onNewAtlasLoaded = ame.OnNewAtlasLoaded;
							if (onNewAtlasLoaded == null)
							{
								return;
							}
							onNewAtlasLoaded(_atlas, _isLoadingInGame);
						});
					}
					array2 = null;
					mod = null;
				}
			}
			num = i;
		}
		yield break;
	}

	// Token: 0x0600321D RID: 12829 RVA: 0x00154CAF File Offset: 0x00152EAF
	[PublicizedFrom(EAccessModifier.Private)]
	public static IEnumerator LoadLocalizations(bool _isLoadingInGame)
	{
		if (_isLoadingInGame)
		{
			yield break;
		}
		for (int i = 0; i < ModManager.loadedMods.Count; i++)
		{
			Mod mod = ModManager.loadedMods.list[i];
			string text = mod.Path + "/Config";
			if (SdDirectory.Exists(text))
			{
				try
				{
					Localization.LoadPatchDictionaries(mod.Name, text, _isLoadingInGame);
				}
				catch (Exception e)
				{
					Log.Error("[MODS] Failed loading localization from mod: '" + mod.Name + "'");
					Log.Exception(e);
				}
			}
		}
		Localization.WriteCsv();
		yield break;
	}

	// Token: 0x0600321E RID: 12830 RVA: 0x00154CBE File Offset: 0x00152EBE
	public static void ModAtlasesDefaults(GameObject _parentGo, Shader _defaultShader)
	{
		ModManager.atlasesParentGo = _parentGo;
		ModManager.defaultShader = _defaultShader;
	}

	// Token: 0x0600321F RID: 12831 RVA: 0x00154CCC File Offset: 0x00152ECC
	public static void RegisterAtlasManager(MultiSourceAtlasManager _atlasManager, bool _createdByMod, Shader _shader, Action<UIAtlas, bool> _onNewAtlasLoaded = null)
	{
		ModManager.atlasManagers.Add(_atlasManager.name, new ModManager.AtlasManagerEntry(_atlasManager, _createdByMod, _shader, _onNewAtlasLoaded));
	}

	// Token: 0x06003220 RID: 12832 RVA: 0x00154CE8 File Offset: 0x00152EE8
	public static MultiSourceAtlasManager GetAtlasManager(string _name)
	{
		ModManager.AtlasManagerEntry atlasManagerEntry;
		if (!ModManager.atlasManagers.TryGetValue(_name, out atlasManagerEntry))
		{
			return null;
		}
		return atlasManagerEntry.Manager;
	}

	// Token: 0x06003221 RID: 12833 RVA: 0x00154D0C File Offset: 0x00152F0C
	public static void GameEnded()
	{
		foreach (KeyValuePair<string, ModManager.AtlasManagerEntry> keyValuePair in ModManager.atlasManagers)
		{
			keyValuePair.Value.Manager.CleanupAfterGame();
		}
		Localization.ReloadBaseLocalization();
		ThreadManager.RunCoroutineSync(ModManager.LoadLocalizations(false));
	}

	// Token: 0x040028F7 RID: 10487
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string ModsBasePathLegacy = (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../Mods") : (Application.dataPath + "/../Mods");

	// Token: 0x040028F8 RID: 10488
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly DictionaryList<string, Mod> loadedMods = new DictionaryList<string, Mod>();

	// Token: 0x040028F9 RID: 10489
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Mod> failedMods = new List<Mod>();

	// Token: 0x040028FA RID: 10490
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<string, ModManager.AtlasManagerEntry> atlasManagers = new CaseInsensitiveStringDictionary<ModManager.AtlasManagerEntry>();

	// Token: 0x040028FB RID: 10491
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject atlasesParentGo;

	// Token: 0x040028FC RID: 10492
	[PublicizedFrom(EAccessModifier.Private)]
	public static Shader defaultShader;

	// Token: 0x0200069F RID: 1695
	[PublicizedFrom(EAccessModifier.Private)]
	public class AtlasManagerEntry
	{
		// Token: 0x06003223 RID: 12835 RVA: 0x00154DDD File Offset: 0x00152FDD
		public AtlasManagerEntry(MultiSourceAtlasManager _manager, bool _createdByMod, Shader _shader, Action<UIAtlas, bool> _onNewAtlasLoaded)
		{
			this.Manager = _manager;
			this.CreatedByMod = _createdByMod;
			this.Shader = _shader;
			this.OnNewAtlasLoaded = _onNewAtlasLoaded;
		}

		// Token: 0x040028FD RID: 10493
		public readonly MultiSourceAtlasManager Manager;

		// Token: 0x040028FE RID: 10494
		public readonly bool CreatedByMod;

		// Token: 0x040028FF RID: 10495
		public readonly Shader Shader;

		// Token: 0x04002900 RID: 10496
		public readonly Action<UIAtlas, bool> OnNewAtlasLoaded;
	}
}

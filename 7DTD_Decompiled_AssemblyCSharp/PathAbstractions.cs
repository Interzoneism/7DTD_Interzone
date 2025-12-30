using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

// Token: 0x020011E1 RID: 4577
public static class PathAbstractions
{
	// Token: 0x06008EEA RID: 36586 RVA: 0x00392808 File Offset: 0x00390A08
	public static void InvalidateCaches()
	{
		for (int i = 0; i < PathAbstractions.allSearchDefs.Count; i++)
		{
			PathAbstractions.allSearchDefs[i].InvalidateCache();
		}
	}

	// Token: 0x17000ED1 RID: 3793
	// (get) Token: 0x06008EEB RID: 36587 RVA: 0x0039283A File Offset: 0x00390A3A
	// (set) Token: 0x06008EEC RID: 36588 RVA: 0x00392841 File Offset: 0x00390A41
	public static bool CacheEnabled
	{
		get
		{
			return PathAbstractions.cacheEnabled;
		}
		set
		{
			PathAbstractions.cacheEnabled = value;
			if (!value)
			{
				PathAbstractions.InvalidateCaches();
			}
		}
	}

	// Token: 0x04006E79 RID: 28281
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<PathAbstractions.SearchDefinition> allSearchDefs = new List<PathAbstractions.SearchDefinition>();

	// Token: 0x04006E7A RID: 28282
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Func<string> userDataPath = new Func<string>(GameIO.GetUserGameDataDir);

	// Token: 0x04006E7B RID: 28283
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Func<string> gameDataPath = new Func<string>(GameIO.GetApplicationPath);

	// Token: 0x04006E7C RID: 28284
	public static readonly PathAbstractions.SearchDefinition WorldsSearchPaths = new PathAbstractions.SearchDefinition(true, null, false, false, new PathAbstractions.SearchPath[]
	{
		new PathAbstractions.SearchPathSaves("World", true),
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.UserDataPath, PathAbstractions.userDataPath, "GeneratedWorlds", false),
		new PathAbstractions.SearchPathMods("Worlds", false),
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.GameData, PathAbstractions.gameDataPath, "Data/Worlds", false)
	});

	// Token: 0x04006E7D RID: 28285
	public static readonly PathAbstractions.SearchDefinition PrefabsSearchPaths = new PathAbstractions.SearchDefinition(false, ".tts", true, true, new PathAbstractions.SearchPath[]
	{
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.UserDataPath, PathAbstractions.userDataPath, "LocalPrefabs", false),
		new PathAbstractions.SearchPathMods("Prefabs", false),
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.GameData, PathAbstractions.gameDataPath, "Data/Prefabs", false)
	});

	// Token: 0x04006E7E RID: 28286
	public static readonly PathAbstractions.SearchDefinition PrefabImpostersSearchPaths = new PathAbstractions.SearchDefinition(false, ".mesh", false, true, new PathAbstractions.SearchPath[]
	{
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.UserDataPath, PathAbstractions.userDataPath, "LocalPrefabs", false),
		new PathAbstractions.SearchPathMods("Prefabs", false),
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.GameData, PathAbstractions.gameDataPath, "Data/Prefabs", false)
	});

	// Token: 0x04006E7F RID: 28287
	public static readonly PathAbstractions.SearchDefinition RwgStampsSearchPaths = new PathAbstractions.SearchDefinition(false, "", true, true, new PathAbstractions.SearchPath[]
	{
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.UserDataPath, PathAbstractions.userDataPath, "LocalStamps", false),
		new PathAbstractions.SearchPathMods("Stamps", false),
		new PathAbstractions.SearchPathBasic(PathAbstractions.EAbstractedLocationType.GameData, PathAbstractions.gameDataPath, "Data/Stamps", false)
	});

	// Token: 0x04006E80 RID: 28288
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool cacheEnabled;

	// Token: 0x020011E2 RID: 4578
	public enum EAbstractedLocationType
	{
		// Token: 0x04006E82 RID: 28290
		HostSave,
		// Token: 0x04006E83 RID: 28291
		LocalSave,
		// Token: 0x04006E84 RID: 28292
		UserDataPath,
		// Token: 0x04006E85 RID: 28293
		Mods,
		// Token: 0x04006E86 RID: 28294
		GameData,
		// Token: 0x04006E87 RID: 28295
		None
	}

	// Token: 0x020011E3 RID: 4579
	public readonly struct AbstractedLocation : IEquatable<PathAbstractions.AbstractedLocation>, IComparable<PathAbstractions.AbstractedLocation>, IComparable
	{
		// Token: 0x17000ED2 RID: 3794
		// (get) Token: 0x06008EEE RID: 36590 RVA: 0x003929CF File Offset: 0x00390BCF
		public string FullPath
		{
			get
			{
				return this.Folder + "/" + this.FileNameNoExtension + this.Extension;
			}
		}

		// Token: 0x17000ED3 RID: 3795
		// (get) Token: 0x06008EEF RID: 36591 RVA: 0x003929ED File Offset: 0x00390BED
		public string FullPathNoExtension
		{
			get
			{
				return this.Folder + "/" + this.FileNameNoExtension;
			}
		}

		// Token: 0x06008EF0 RID: 36592 RVA: 0x00392A08 File Offset: 0x00390C08
		public AbstractedLocation(PathAbstractions.EAbstractedLocationType _type, string _name, string _fullPath, string _relativePath, bool _isFolder, Mod _containingMod = null)
		{
			_fullPath = ((_fullPath != null) ? _fullPath.Replace("\\", "/") : null);
			this.Type = _type;
			this.Name = Path.GetFileName(_name);
			string directoryName = Path.GetDirectoryName(_fullPath);
			this.Folder = ((directoryName != null) ? directoryName.Replace("\\", "/") : null);
			this.RelativePath = _relativePath;
			this.FileNameNoExtension = Path.GetFileNameWithoutExtension(_fullPath);
			this.Extension = Path.GetExtension(_fullPath);
			this.Extension = (string.IsNullOrEmpty(this.Extension) ? null : this.Extension);
			this.IsFolder = _isFolder;
			this.ContainingMod = _containingMod;
		}

		// Token: 0x06008EF1 RID: 36593 RVA: 0x00392AB0 File Offset: 0x00390CB0
		public AbstractedLocation(PathAbstractions.EAbstractedLocationType _type, string _name, string _folder, string _relativePath, string _fileNameNoExtension, string _extension, bool _isFolder, Mod _containingMod = null)
		{
			this.Type = _type;
			this.Name = _name;
			this.Folder = ((_folder != null) ? _folder.Replace("\\", "/") : null);
			this.RelativePath = _relativePath;
			this.FileNameNoExtension = _fileNameNoExtension;
			this.Extension = _extension;
			this.IsFolder = _isFolder;
			this.ContainingMod = _containingMod;
		}

		// Token: 0x06008EF2 RID: 36594 RVA: 0x00392B0F File Offset: 0x00390D0F
		public bool Exists()
		{
			if (this.Type == PathAbstractions.EAbstractedLocationType.None)
			{
				return false;
			}
			if (!this.IsFolder)
			{
				return SdFile.Exists(this.FullPath);
			}
			return SdDirectory.Exists(this.FullPath);
		}

		// Token: 0x06008EF3 RID: 36595 RVA: 0x00392B3C File Offset: 0x00390D3C
		public bool Equals(PathAbstractions.AbstractedLocation _other)
		{
			return this.Type == _other.Type && (this.Type == PathAbstractions.EAbstractedLocationType.None || (this.IsFolder == _other.IsFolder && string.Equals(this.Name, _other.Name) && GameIO.PathsEquals(this.FullPath, _other.FullPath, true)));
		}

		// Token: 0x06008EF4 RID: 36596 RVA: 0x00392B9C File Offset: 0x00390D9C
		public override bool Equals(object _obj)
		{
			if (_obj == null)
			{
				return false;
			}
			if (_obj is PathAbstractions.AbstractedLocation)
			{
				PathAbstractions.AbstractedLocation other = (PathAbstractions.AbstractedLocation)_obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06008EF5 RID: 36597 RVA: 0x00392BC6 File Offset: 0x00390DC6
		public static bool operator ==(PathAbstractions.AbstractedLocation _a, PathAbstractions.AbstractedLocation _b)
		{
			return _a.Equals(_b);
		}

		// Token: 0x06008EF6 RID: 36598 RVA: 0x00392BD0 File Offset: 0x00390DD0
		public static bool operator !=(PathAbstractions.AbstractedLocation _a, PathAbstractions.AbstractedLocation _b)
		{
			return !(_a == _b);
		}

		// Token: 0x06008EF7 RID: 36599 RVA: 0x00392BDC File Offset: 0x00390DDC
		public override int GetHashCode()
		{
			return (((this.Name != null) ? this.Name.GetHashCode() : 0) * 397 ^ (int)this.Type) * 397 ^ ((this.FullPath != null) ? this.FullPath.GetHashCode() : 0);
		}

		// Token: 0x06008EF8 RID: 36600 RVA: 0x00392C29 File Offset: 0x00390E29
		public override string ToString()
		{
			return this.Name + " (src: " + this.Type.ToStringCached<PathAbstractions.EAbstractedLocationType>() + ")";
		}

		// Token: 0x06008EF9 RID: 36601 RVA: 0x00392C4C File Offset: 0x00390E4C
		public int CompareTo(PathAbstractions.AbstractedLocation _other)
		{
			int num = string.Compare(this.Name, _other.Name, StringComparison.OrdinalIgnoreCase);
			if (num != 0)
			{
				return num;
			}
			int num2 = this.Type.CompareTo(_other.Type);
			if (num2 != 0)
			{
				return num2;
			}
			int num3 = string.Compare(this.FileNameNoExtension, _other.FileNameNoExtension, StringComparison.OrdinalIgnoreCase);
			if (num3 != 0)
			{
				return num3;
			}
			int num4 = string.Compare(this.Extension, _other.Extension, StringComparison.OrdinalIgnoreCase);
			if (num4 != 0)
			{
				return num4;
			}
			return string.Compare(this.Folder, _other.Folder, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06008EFA RID: 36602 RVA: 0x00392CD8 File Offset: 0x00390ED8
		public int CompareTo(object _obj)
		{
			if (_obj == null)
			{
				return 1;
			}
			if (_obj is PathAbstractions.AbstractedLocation)
			{
				PathAbstractions.AbstractedLocation other = (PathAbstractions.AbstractedLocation)_obj;
				return this.CompareTo(other);
			}
			throw new ArgumentException("Object must be of type AbstractedLocation");
		}

		// Token: 0x04006E88 RID: 28296
		public static readonly PathAbstractions.AbstractedLocation None = new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.None, null, null, null, false, null);

		// Token: 0x04006E89 RID: 28297
		public readonly PathAbstractions.EAbstractedLocationType Type;

		// Token: 0x04006E8A RID: 28298
		public readonly string Name;

		// Token: 0x04006E8B RID: 28299
		public readonly string Folder;

		// Token: 0x04006E8C RID: 28300
		public readonly string RelativePath;

		// Token: 0x04006E8D RID: 28301
		public readonly string FileNameNoExtension;

		// Token: 0x04006E8E RID: 28302
		public readonly string Extension;

		// Token: 0x04006E8F RID: 28303
		public readonly bool IsFolder;

		// Token: 0x04006E90 RID: 28304
		public readonly Mod ContainingMod;
	}

	// Token: 0x020011E4 RID: 4580
	public class SearchDefinition
	{
		// Token: 0x06008EFC RID: 36604 RVA: 0x00392D20 File Offset: 0x00390F20
		public SearchDefinition(bool _isFolder, string _extension, bool _removeExtension, bool _recursive, params PathAbstractions.SearchPath[] _paths)
		{
			this.IsFolder = _isFolder;
			this.Extension = _extension;
			this.RemoveExtension = _removeExtension;
			this.Recursive = _recursive;
			if (this.IsFolder && this.Recursive)
			{
				throw new Exception("SearchDefinition can not be set to target folders and search recursively at the same time!");
			}
			this.paths = new List<PathAbstractions.SearchPath>(_paths);
			for (int i = 0; i < _paths.Length; i++)
			{
				_paths[i].SetOwner(this);
			}
			PathAbstractions.allSearchDefs.Add(this);
		}

		// Token: 0x06008EFD RID: 36605 RVA: 0x00392DA0 File Offset: 0x00390FA0
		public PathAbstractions.AbstractedLocation GetLocation(string _name, string _worldName = null, string _gameName = null)
		{
			foreach (PathAbstractions.SearchPath searchPath in this.paths)
			{
				if (searchPath.CanMatch)
				{
					PathAbstractions.AbstractedLocation location = searchPath.GetLocation(_name, _worldName, _gameName);
					if (location.Type != PathAbstractions.EAbstractedLocationType.None)
					{
						return location;
					}
				}
			}
			return new PathAbstractions.AbstractedLocation(PathAbstractions.EAbstractedLocationType.None, _name, null, null, this.IsFolder, null);
		}

		// Token: 0x06008EFE RID: 36606 RVA: 0x00392E20 File Offset: 0x00391020
		public List<PathAbstractions.AbstractedLocation> GetAvailablePathsList(Regex _nameMatch = null, string _worldName = null, string _gameName = null, bool _ignoreDuplicateNames = false)
		{
			List<PathAbstractions.AbstractedLocation> list = new List<PathAbstractions.AbstractedLocation>();
			foreach (PathAbstractions.SearchPath searchPath in this.paths)
			{
				if (searchPath.CanMatch)
				{
					searchPath.GetAvailablePathsList(list, _nameMatch, _worldName, _gameName, _ignoreDuplicateNames);
				}
			}
			return list;
		}

		// Token: 0x06008EFF RID: 36607 RVA: 0x00392E88 File Offset: 0x00391088
		public void GetAvailablePathsList(List<PathAbstractions.AbstractedLocation> _resultList, Regex _nameMatch = null, string _worldName = null, string _gameName = null, bool _ignoreDuplicateNames = false)
		{
			if (_resultList == null)
			{
				_resultList = new List<PathAbstractions.AbstractedLocation>();
			}
			foreach (PathAbstractions.SearchPath searchPath in this.paths)
			{
				if (searchPath.CanMatch)
				{
					searchPath.GetAvailablePathsList(_resultList, _nameMatch, _worldName, _gameName, _ignoreDuplicateNames);
				}
			}
		}

		// Token: 0x06008F00 RID: 36608 RVA: 0x00392EF4 File Offset: 0x003910F4
		public void InvalidateCache()
		{
			foreach (PathAbstractions.SearchPath searchPath in this.paths)
			{
				searchPath.InvalidateCache();
			}
		}

		// Token: 0x04006E91 RID: 28305
		public readonly bool IsFolder;

		// Token: 0x04006E92 RID: 28306
		public readonly string Extension;

		// Token: 0x04006E93 RID: 28307
		public readonly bool RemoveExtension;

		// Token: 0x04006E94 RID: 28308
		public readonly bool Recursive;

		// Token: 0x04006E95 RID: 28309
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<PathAbstractions.SearchPath> paths;
	}

	// Token: 0x020011E5 RID: 4581
	public abstract class SearchPath
	{
		// Token: 0x17000ED4 RID: 3796
		// (get) Token: 0x06008F01 RID: 36609 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual bool CanMatch
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06008F02 RID: 36610 RVA: 0x00392F44 File Offset: 0x00391144
		[PublicizedFrom(EAccessModifier.Protected)]
		public SearchPath(string _relativePath, bool _pathIsTarget)
		{
			this.RelativePath = _relativePath;
			this.PathIsTarget = _pathIsTarget;
		}

		// Token: 0x06008F03 RID: 36611 RVA: 0x00392F65 File Offset: 0x00391165
		public void SetOwner(PathAbstractions.SearchDefinition _owner)
		{
			this.Owner = _owner;
		}

		// Token: 0x06008F04 RID: 36612
		public abstract PathAbstractions.AbstractedLocation GetLocation(string _name, string _worldName, string _gameName);

		// Token: 0x06008F05 RID: 36613
		public abstract void GetAvailablePathsList(List<PathAbstractions.AbstractedLocation> _targetList, Regex _nameMatch, string _worldName, string _gameName, bool _ignoreDuplicateNames);

		// Token: 0x06008F06 RID: 36614 RVA: 0x00392F70 File Offset: 0x00391170
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathAbstractions.AbstractedLocation getLocationSingleBase(PathAbstractions.EAbstractedLocationType _locationType, string _basePath, string _name, string _worldName, string _gameName, Mod _containingMod, string _subfolder = null)
		{
			this.UseCache(_worldName, _gameName);
			if (!SdDirectory.Exists(_basePath))
			{
				return PathAbstractions.AbstractedLocation.None;
			}
			string text = _basePath + "/" + _name + this.Owner.Extension;
			if (this.Owner.IsFolder)
			{
				if (SdDirectory.Exists(text))
				{
					return new PathAbstractions.AbstractedLocation(_locationType, _name, text, _subfolder, this.Owner.IsFolder, _containingMod);
				}
			}
			else
			{
				if (SdFile.Exists(text))
				{
					string name = this.Owner.RemoveExtension ? GameIO.RemoveExtension(_name, this.Owner.Extension) : _name;
					return new PathAbstractions.AbstractedLocation(_locationType, name, text, _subfolder, this.Owner.IsFolder, _containingMod);
				}
				if (this.Owner.Recursive)
				{
					foreach (string text2 in SdDirectory.GetDirectories(_basePath))
					{
						PathAbstractions.AbstractedLocation locationSingleBase = this.getLocationSingleBase(_locationType, text2, _name, _worldName, _gameName, _containingMod, Path.GetFileName(text2));
						if (!locationSingleBase.Equals(PathAbstractions.AbstractedLocation.None))
						{
							return locationSingleBase;
						}
					}
				}
			}
			return PathAbstractions.AbstractedLocation.None;
		}

		// Token: 0x06008F07 RID: 36615 RVA: 0x00393078 File Offset: 0x00391278
		[PublicizedFrom(EAccessModifier.Protected)]
		public void getAvailablePathsSingleBase(List<PathAbstractions.AbstractedLocation> _targetList, PathAbstractions.EAbstractedLocationType _locationType, string _basePath, Regex _nameMatch, string _worldName, string _gameName, bool _ignoreDuplicateNames, Mod _containingMod, string _subfolder = null)
		{
			if (!SdDirectory.Exists(_basePath))
			{
				return;
			}
			SdDirectoryInfo sdDirectoryInfo = new SdDirectoryInfo(_basePath);
			SdFileSystemInfo[] array;
			SdFileSystemInfo[] array2;
			if (this.Owner.IsFolder)
			{
				array = sdDirectoryInfo.GetDirectories();
				array2 = array;
			}
			else
			{
				array = sdDirectoryInfo.GetFiles("*" + this.Owner.Extension, SearchOption.TopDirectoryOnly);
				array2 = array;
			}
			array = array2;
			for (int i = 0; i < array.Length; i++)
			{
				SdFileSystemInfo sdFileSystemInfo = array[i];
				if ((this.Owner.Extension == null || sdFileSystemInfo.Name.EndsWith(this.Owner.Extension, StringComparison.Ordinal)) && (_nameMatch == null || _nameMatch.IsMatch(sdFileSystemInfo.Name)))
				{
					string filename = this.Owner.RemoveExtension ? GameIO.RemoveExtension(sdFileSystemInfo.Name, this.Owner.Extension) : sdFileSystemInfo.Name;
					if (!_ignoreDuplicateNames || !_targetList.Exists((PathAbstractions.AbstractedLocation _location) => _location.Name.Equals(filename)))
					{
						_targetList.Add(new PathAbstractions.AbstractedLocation(_locationType, filename, sdFileSystemInfo.FullName, _subfolder, this.Owner.IsFolder, _containingMod));
					}
				}
			}
			if (!this.Owner.IsFolder && this.Owner.Recursive)
			{
				foreach (string text in SdDirectory.GetDirectories(_basePath))
				{
					this.getAvailablePathsSingleBase(_targetList, _locationType, text, _nameMatch, _worldName, _gameName, _ignoreDuplicateNames, _containingMod, Path.GetFileName(text));
				}
			}
		}

		// Token: 0x06008F08 RID: 36616 RVA: 0x003931F4 File Offset: 0x003913F4
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool UseCache(string _worldName, string _gameName)
		{
			bool flag = PathAbstractions.CacheEnabled && string.IsNullOrEmpty(_worldName) && string.IsNullOrEmpty(_gameName);
			if (flag && !this.locationsCachePopulated)
			{
				this.PopulateCache();
			}
			return flag;
		}

		// Token: 0x06008F09 RID: 36617 RVA: 0x00393220 File Offset: 0x00391420
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathAbstractions.AbstractedLocation GetCachedLocation(string _name, bool _ignoreName = false)
		{
			if (this.locationsCache.Count == 0)
			{
				return PathAbstractions.AbstractedLocation.None;
			}
			if (_ignoreName)
			{
				foreach (KeyValuePair<string, IList<PathAbstractions.AbstractedLocation>> keyValuePair in this.locationsCache)
				{
					if (keyValuePair.Value.Count != 0)
					{
						return keyValuePair.Value[0];
					}
				}
				return PathAbstractions.AbstractedLocation.None;
			}
			IList<PathAbstractions.AbstractedLocation> list;
			if (!this.locationsCache.TryGetValue(this.Owner.RemoveExtension ? _name : (_name + this.Owner.Extension), out list))
			{
				return PathAbstractions.AbstractedLocation.None;
			}
			if (list.Count <= 0)
			{
				return PathAbstractions.AbstractedLocation.None;
			}
			return list[0];
		}

		// Token: 0x06008F0A RID: 36618 RVA: 0x003932F4 File Offset: 0x003914F4
		[PublicizedFrom(EAccessModifier.Protected)]
		public void GetCachedPathList(List<PathAbstractions.AbstractedLocation> _targetList, Regex _nameMatch, bool _ignoreDuplicateNames)
		{
			if (this.locationsCache.Count == 0)
			{
				return;
			}
			using (Dictionary<string, IList<PathAbstractions.AbstractedLocation>>.Enumerator enumerator = this.locationsCache.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, IList<PathAbstractions.AbstractedLocation>> kvp = enumerator.Current;
					if ((_nameMatch == null || _nameMatch.IsMatch(kvp.Key)) && (!_ignoreDuplicateNames || !_targetList.Exists((PathAbstractions.AbstractedLocation _location) => _location.Name.Equals(kvp.Key))))
					{
						int num = _ignoreDuplicateNames ? Mathf.Min(1, kvp.Value.Count) : kvp.Value.Count;
						for (int i = 0; i < num; i++)
						{
							_targetList.Add(kvp.Value[i]);
						}
					}
				}
			}
		}

		// Token: 0x06008F0B RID: 36619 RVA: 0x003933DC File Offset: 0x003915DC
		public void InvalidateCache()
		{
			this.locationsCache.Clear();
			this.locationsCachePopulated = false;
		}

		// Token: 0x06008F0C RID: 36620
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract void PopulateCache();

		// Token: 0x04006E96 RID: 28310
		[PublicizedFrom(EAccessModifier.Protected)]
		public PathAbstractions.SearchDefinition Owner;

		// Token: 0x04006E97 RID: 28311
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string RelativePath;

		// Token: 0x04006E98 RID: 28312
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly bool PathIsTarget;

		// Token: 0x04006E99 RID: 28313
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly Dictionary<string, IList<PathAbstractions.AbstractedLocation>> locationsCache = new CaseInsensitiveStringDictionary<IList<PathAbstractions.AbstractedLocation>>();

		// Token: 0x04006E9A RID: 28314
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool locationsCachePopulated;
	}

	// Token: 0x020011E8 RID: 4584
	public class SearchPathBasic : PathAbstractions.SearchPath
	{
		// Token: 0x06008F11 RID: 36625 RVA: 0x0039341B File Offset: 0x0039161B
		public SearchPathBasic(PathAbstractions.EAbstractedLocationType _locationType, Func<string> _basePath, string _relativePath, bool _pathIsTarget = false) : base(_relativePath, _pathIsTarget)
		{
			this.locationType = _locationType;
			this.basePath = _basePath;
		}

		// Token: 0x06008F12 RID: 36626 RVA: 0x00393434 File Offset: 0x00391634
		public override PathAbstractions.AbstractedLocation GetLocation(string _name, string _worldName, string _gameName)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				return base.GetCachedLocation(_name, false);
			}
			PathAbstractions.AbstractedLocation locationSingleBase = base.getLocationSingleBase(this.locationType, this.basePath() + "/" + this.RelativePath, _name, _worldName, _gameName, null, null);
			if (!locationSingleBase.Equals(PathAbstractions.AbstractedLocation.None))
			{
				return locationSingleBase;
			}
			return PathAbstractions.AbstractedLocation.None;
		}

		// Token: 0x06008F13 RID: 36627 RVA: 0x00393498 File Offset: 0x00391698
		public override void GetAvailablePathsList(List<PathAbstractions.AbstractedLocation> _targetList, Regex _nameMatch, string _worldName, string _gameName, bool _ignoreDuplicateNames)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				base.GetCachedPathList(_targetList, _nameMatch, _ignoreDuplicateNames);
				return;
			}
			base.getAvailablePathsSingleBase(_targetList, this.locationType, this.basePath() + "/" + this.RelativePath, _nameMatch, _worldName, _gameName, _ignoreDuplicateNames, null, null);
		}

		// Token: 0x06008F14 RID: 36628 RVA: 0x003934EC File Offset: 0x003916EC
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void PopulateCache()
		{
			List<PathAbstractions.AbstractedLocation> list = new List<PathAbstractions.AbstractedLocation>();
			base.getAvailablePathsSingleBase(list, this.locationType, this.basePath() + "/" + this.RelativePath, null, null, null, false, null, null);
			this.locationsCache.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				PathAbstractions.AbstractedLocation abstractedLocation = list[i];
				IList<PathAbstractions.AbstractedLocation> list2;
				if (!this.locationsCache.TryGetValue(abstractedLocation.Name, out list2))
				{
					list2 = new List<PathAbstractions.AbstractedLocation>();
					this.locationsCache[abstractedLocation.Name] = list2;
				}
				list2.Add(abstractedLocation);
			}
			this.locationsCachePopulated = true;
		}

		// Token: 0x04006E9D RID: 28317
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly PathAbstractions.EAbstractedLocationType locationType;

		// Token: 0x04006E9E RID: 28318
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Func<string> basePath;
	}

	// Token: 0x020011E9 RID: 4585
	public class SearchPathSaves : PathAbstractions.SearchPath
	{
		// Token: 0x17000ED5 RID: 3797
		// (get) Token: 0x06008F15 RID: 36629 RVA: 0x0039358C File Offset: 0x0039178C
		public override bool CanMatch
		{
			get
			{
				return SingletonMonoBehaviour<ConnectionManager>.Instance != null;
			}
		}

		// Token: 0x06008F16 RID: 36630 RVA: 0x00393599 File Offset: 0x00391799
		public SearchPathSaves(string _relativePath, bool _pathIsTarget = false) : base(_relativePath, _pathIsTarget)
		{
		}

		// Token: 0x06008F17 RID: 36631 RVA: 0x003935A4 File Offset: 0x003917A4
		[PublicizedFrom(EAccessModifier.Private)]
		public ValueTuple<string, PathAbstractions.EAbstractedLocationType> GetSaveFolder(string _worldName, string _gameName)
		{
			if (!string.IsNullOrEmpty(_worldName) && !string.IsNullOrEmpty(_gameName))
			{
				return new ValueTuple<string, PathAbstractions.EAbstractedLocationType>(GameIO.GetSaveGameDir(_worldName, _gameName), PathAbstractions.EAbstractedLocationType.HostSave);
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
			{
				return new ValueTuple<string, PathAbstractions.EAbstractedLocationType>(GameIO.GetSaveGameDir(), PathAbstractions.EAbstractedLocationType.HostSave);
			}
			if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsClient)
			{
				return new ValueTuple<string, PathAbstractions.EAbstractedLocationType>(GameIO.GetSaveGameLocalDir(), PathAbstractions.EAbstractedLocationType.LocalSave);
			}
			return new ValueTuple<string, PathAbstractions.EAbstractedLocationType>(null, PathAbstractions.EAbstractedLocationType.None);
		}

		// Token: 0x06008F18 RID: 36632 RVA: 0x00393608 File Offset: 0x00391808
		public override PathAbstractions.AbstractedLocation GetLocation(string _name, string _worldName, string _gameName)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				return base.GetCachedLocation(_name, true);
			}
			ValueTuple<string, PathAbstractions.EAbstractedLocationType> saveFolder = this.GetSaveFolder(_worldName, _gameName);
			string item = saveFolder.Item1;
			PathAbstractions.EAbstractedLocationType item2 = saveFolder.Item2;
			if (item == null)
			{
				return PathAbstractions.AbstractedLocation.None;
			}
			string text = item + "/" + this.RelativePath;
			if ((this.Owner.IsFolder && SdDirectory.Exists(text)) || (!this.Owner.IsFolder && SdFile.Exists(text)))
			{
				return new PathAbstractions.AbstractedLocation(item2, this.RelativePath, text, null, this.Owner.IsFolder, null);
			}
			return PathAbstractions.AbstractedLocation.None;
		}

		// Token: 0x06008F19 RID: 36633 RVA: 0x003936A8 File Offset: 0x003918A8
		public override void GetAvailablePathsList(List<PathAbstractions.AbstractedLocation> _targetList, Regex _nameMatch, string _worldName, string _gameName, bool _ignoreDuplicateNames)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				base.GetCachedPathList(_targetList, null, _ignoreDuplicateNames);
				return;
			}
			ValueTuple<string, PathAbstractions.EAbstractedLocationType> saveFolder = this.GetSaveFolder(_worldName, _gameName);
			string item = saveFolder.Item1;
			PathAbstractions.EAbstractedLocationType item2 = saveFolder.Item2;
			if (item == null)
			{
				return;
			}
			string text = item + "/" + this.RelativePath;
			if ((this.Owner.IsFolder && SdDirectory.Exists(text)) || (!this.Owner.IsFolder && SdFile.Exists(text)))
			{
				_targetList.Add(new PathAbstractions.AbstractedLocation(item2, this.RelativePath, text, null, this.Owner.IsFolder, null));
			}
		}

		// Token: 0x06008F1A RID: 36634 RVA: 0x00393744 File Offset: 0x00391944
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void PopulateCache()
		{
			ValueTuple<string, PathAbstractions.EAbstractedLocationType> saveFolder = this.GetSaveFolder(null, null);
			string item = saveFolder.Item1;
			PathAbstractions.EAbstractedLocationType item2 = saveFolder.Item2;
			this.locationsCache.Clear();
			this.locationsCachePopulated = true;
			if (item == null)
			{
				return;
			}
			string text = item + "/" + this.RelativePath;
			if ((this.Owner.IsFolder && SdDirectory.Exists(text)) || (!this.Owner.IsFolder && SdFile.Exists(text)))
			{
				PathAbstractions.AbstractedLocation abstractedLocation = new PathAbstractions.AbstractedLocation(item2, this.RelativePath, text, null, this.Owner.IsFolder, null);
				List<PathAbstractions.AbstractedLocation> list = new List<PathAbstractions.AbstractedLocation>();
				this.locationsCache[abstractedLocation.Name] = list;
				list.Add(abstractedLocation);
			}
		}
	}

	// Token: 0x020011EA RID: 4586
	public class SearchPathMods : PathAbstractions.SearchPath
	{
		// Token: 0x17000ED6 RID: 3798
		// (get) Token: 0x06008F1B RID: 36635 RVA: 0x0039358C File Offset: 0x0039178C
		public override bool CanMatch
		{
			get
			{
				return SingletonMonoBehaviour<ConnectionManager>.Instance != null;
			}
		}

		// Token: 0x06008F1C RID: 36636 RVA: 0x00393599 File Offset: 0x00391799
		public SearchPathMods(string _relativePath, bool _pathIsTarget = false) : base(_relativePath, _pathIsTarget)
		{
		}

		// Token: 0x06008F1D RID: 36637 RVA: 0x003937F8 File Offset: 0x003919F8
		public override PathAbstractions.AbstractedLocation GetLocation(string _name, string _worldName, string _gameName)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				return base.GetCachedLocation(_name, false);
			}
			foreach (Mod mod in ModManager.GetLoadedMods())
			{
				PathAbstractions.AbstractedLocation locationSingleBase = base.getLocationSingleBase(PathAbstractions.EAbstractedLocationType.Mods, mod.Path + "/" + this.RelativePath, _name, _worldName, _gameName, mod, null);
				if (!locationSingleBase.Equals(PathAbstractions.AbstractedLocation.None))
				{
					return locationSingleBase;
				}
			}
			return PathAbstractions.AbstractedLocation.None;
		}

		// Token: 0x06008F1E RID: 36638 RVA: 0x00393894 File Offset: 0x00391A94
		public override void GetAvailablePathsList(List<PathAbstractions.AbstractedLocation> _targetList, Regex _nameMatch, string _worldName, string _gameName, bool _ignoreDuplicateNames)
		{
			if (base.UseCache(_worldName, _gameName))
			{
				base.GetCachedPathList(_targetList, _nameMatch, _ignoreDuplicateNames);
				return;
			}
			foreach (Mod mod in ModManager.GetLoadedMods())
			{
				base.getAvailablePathsSingleBase(_targetList, PathAbstractions.EAbstractedLocationType.Mods, mod.Path + "/" + this.RelativePath, _nameMatch, _worldName, _gameName, _ignoreDuplicateNames, mod, null);
			}
		}

		// Token: 0x06008F1F RID: 36639 RVA: 0x0039391C File Offset: 0x00391B1C
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void PopulateCache()
		{
			List<PathAbstractions.AbstractedLocation> list = new List<PathAbstractions.AbstractedLocation>();
			foreach (Mod mod in ModManager.GetLoadedMods())
			{
				base.getAvailablePathsSingleBase(list, PathAbstractions.EAbstractedLocationType.Mods, mod.Path + "/" + this.RelativePath, null, null, null, false, mod, null);
			}
			this.locationsCache.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				PathAbstractions.AbstractedLocation abstractedLocation = list[i];
				IList<PathAbstractions.AbstractedLocation> list2;
				if (!this.locationsCache.TryGetValue(abstractedLocation.Name, out list2))
				{
					list2 = new List<PathAbstractions.AbstractedLocation>();
					this.locationsCache[abstractedLocation.Name] = list2;
				}
				list2.Add(abstractedLocation);
			}
			this.locationsCachePopulated = true;
		}
	}
}

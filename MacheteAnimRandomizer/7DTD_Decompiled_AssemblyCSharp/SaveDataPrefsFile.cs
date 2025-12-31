using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

// Token: 0x02000945 RID: 2373
public sealed class SaveDataPrefsFile : ISaveDataPrefs
{
	// Token: 0x1700077A RID: 1914
	// (get) Token: 0x06004769 RID: 18281 RVA: 0x001C0F13 File Offset: 0x001BF113
	public static SaveDataPrefsFile INSTANCE
	{
		get
		{
			SaveDataPrefsFile result;
			if ((result = SaveDataPrefsFile.s_instance) == null)
			{
				result = (SaveDataPrefsFile.s_instance = new SaveDataPrefsFile());
			}
			return result;
		}
	}

	// Token: 0x0600476A RID: 18282 RVA: 0x001C0F2C File Offset: 0x001BF12C
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataPrefsFile()
	{
		foreach (SaveDataPrefsFile.PrefType prefType in EnumUtils.Values<SaveDataPrefsFile.PrefType>())
		{
			if (!SaveDataPrefsFile.PrefTypeMapping.ContainsKey(prefType))
			{
				throw new KeyNotFoundException(string.Format("Expected {0} to have key '{1}'.", "PrefTypeMapping", prefType));
			}
		}
		this.m_storageFilePath = GameIO.GetNormalizedPath(Path.Join(GameIO.GetUserGameDataDir(), "prefs.cfg"));
		this.m_storage = new Dictionary<string, SaveDataPrefsFile.Pref>();
		this.Load();
	}

	// Token: 0x0600476B RID: 18283 RVA: 0x001C0FE0 File Offset: 0x001BF1E0
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void Escape(TextWriter writer, ReadOnlySpan<char> raw, bool ignoreSeparator)
	{
		bool flag = false;
		ReadOnlySpan<char> readOnlySpan = raw;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = (char)(*readOnlySpan[i]);
			if ((!ignoreSeparator || c != '=') && SaveDataPrefsFile.EscapeMapping.ContainsKey(c))
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			writer.Write(raw);
			return;
		}
		readOnlySpan = raw;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c2 = (char)(*readOnlySpan[i]);
			char value;
			if ((ignoreSeparator && c2 == '=') || !SaveDataPrefsFile.EscapeMapping.TryGetValue(c2, out value))
			{
				writer.Write(c2);
			}
			else
			{
				writer.Write('\\');
				writer.Write(value);
			}
		}
	}

	// Token: 0x0600476C RID: 18284 RVA: 0x001C1084 File Offset: 0x001BF284
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static void Unescape(StringBuilder builder, ReadOnlySpan<char> escaped)
	{
		if (escaped.IndexOf('\\') < 0)
		{
			builder.Append(escaped);
			return;
		}
		bool flag = false;
		int num = -1;
		ReadOnlySpan<char> readOnlySpan = escaped;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = (char)(*readOnlySpan[i]);
			num++;
			if (flag)
			{
				flag = false;
				char value;
				if (!SaveDataPrefsFile.UnescapeMapping.TryGetValue(c, out value))
				{
					Log.Warning(string.Format("Unexpected character after escape prefix at offset {0} (will be taken as-is): {1}", num, c));
					builder.Append(c);
				}
				builder.Append(value);
			}
			else if (c == '\\')
			{
				flag = true;
			}
			else
			{
				builder.Append(c);
			}
		}
	}

	// Token: 0x0600476D RID: 18285 RVA: 0x001C1124 File Offset: 0x001BF324
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static int IndexOfFirstUnescapedSeparator(ReadOnlySpan<char> search)
	{
		bool flag = false;
		int num = -1;
		ReadOnlySpan<char> readOnlySpan = search;
		for (int i = 0; i < readOnlySpan.Length; i++)
		{
			char c = (char)(*readOnlySpan[i]);
			num++;
			if (flag)
			{
				flag = false;
			}
			else if (c == '\\')
			{
				flag = true;
			}
			else if (c == '=')
			{
				return num;
			}
		}
		return -1;
	}

	// Token: 0x0600476E RID: 18286 RVA: 0x001C1174 File Offset: 0x001BF374
	[PublicizedFrom(EAccessModifier.Private)]
	public void SaveInternal()
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			if (this.m_dirty)
			{
				try
				{
					using (StreamWriter streamWriter = SdFile.CreateText(this.m_storageFilePath))
					{
						int num = 0;
						foreach (KeyValuePair<string, SaveDataPrefsFile.Pref> keyValuePair in this.m_storage)
						{
							string text;
							SaveDataPrefsFile.Pref pref;
							keyValuePair.Deconstruct(out text, out pref);
							string text2 = text;
							SaveDataPrefsFile.Pref pref2 = pref;
							string value;
							if (!pref2.TryToString(out value))
							{
								Log.Out(string.Format("[{0}] Failed to convert pref '{1}' of type {2} to a string representation.", "SaveDataPrefsFile", text2, pref2.Type));
							}
							else
							{
								SaveDataPrefsFile.Escape(streamWriter, text2, false);
								streamWriter.Write('=');
								SaveDataPrefsFile.Escape(streamWriter, value, true);
								streamWriter.WriteLine();
								num++;
							}
						}
						this.m_dirty = false;
						Log.Out(string.Format("[{0}] Saved {1} player pref(s) to: {2}", "SaveDataPrefsFile", num, this.m_storageFilePath));
					}
				}
				catch (IOException ex)
				{
					Log.Error("[SaveDataPrefsFile] Failed to Save: " + ex.Message);
					Log.Exception(ex);
				}
			}
		}
	}

	// Token: 0x0600476F RID: 18287 RVA: 0x001C1318 File Offset: 0x001BF518
	[PublicizedFrom(EAccessModifier.Private)]
	public void LoadInternal()
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			this.m_storage.Clear();
			if (SdFile.Exists(this.m_storageFilePath))
			{
				try
				{
					using (StreamReader streamReader = SdFile.OpenText(this.m_storageFilePath))
					{
						StringBuilder stringBuilder = new StringBuilder();
						int num = 0;
						int num2 = 0;
						for (;;)
						{
							string text = streamReader.ReadLine();
							if (text == null)
							{
								break;
							}
							num2++;
							ReadOnlySpan<char> readOnlySpan = text;
							int num3 = SaveDataPrefsFile.IndexOfFirstUnescapedSeparator(readOnlySpan);
							if (num3 < 0)
							{
								Log.Error(string.Format("[{0}] Skipping line {1} since is missing unescaped separator '{2}'. Contents: {3}", new object[]
								{
									"SaveDataPrefsFile",
									num2,
									'=',
									text
								}));
							}
							else
							{
								StringBuilder builder = stringBuilder;
								ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
								SaveDataPrefsFile.Unescape(builder, readOnlySpan2.Slice(0, num3));
								string text2 = stringBuilder.ToString();
								stringBuilder.Clear();
								StringBuilder builder2 = stringBuilder;
								readOnlySpan2 = readOnlySpan;
								int num4 = num3 + 1;
								SaveDataPrefsFile.Unescape(builder2, readOnlySpan2.Slice(num4, readOnlySpan2.Length - num4));
								string text3 = stringBuilder.ToString();
								stringBuilder.Clear();
								SaveDataPrefsFile.Pref value;
								if (!SaveDataPrefsFile.Pref.TryParse(text3, out value))
								{
									Log.Error("[SaveDataPrefsFile] Failed to parse pref '" + text2 + "' with string representation: " + text3);
								}
								else
								{
									this.m_storage[text2] = value;
									num++;
								}
							}
						}
						Log.Out(string.Format("[{0}] Loaded {1} player pref(s) from: {2}", "SaveDataPrefsFile", num, this.m_storageFilePath));
					}
					return;
				}
				catch (IOException ex)
				{
					Log.Error("[SaveDataPrefsFile] Failed to Load: " + ex.Message);
					Log.Exception(ex);
					return;
				}
			}
			Log.Out("[SaveDataPrefsFile] Using empty player prefs, as none exists at: " + this.m_storageFilePath);
		}
	}

	// Token: 0x06004770 RID: 18288 RVA: 0x001C1528 File Offset: 0x001BF728
	public float GetFloat(string key, float defaultValue)
	{
		object storageLock = this.m_storageLock;
		float result;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			float num;
			result = ((this.m_storage.TryGetValue(key, out pref) && pref.TryGet(out num)) ? num : defaultValue);
		}
		return result;
	}

	// Token: 0x06004771 RID: 18289 RVA: 0x001C1584 File Offset: 0x001BF784
	public void SetFloat(string key, float value)
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			if (this.m_storage.TryGetValue(key, out pref))
			{
				float num;
				if (!pref.TryGet(out num) || num != value)
				{
					pref.Set(value);
					this.m_dirty = true;
				}
			}
			else
			{
				pref = new SaveDataPrefsFile.Pref(value);
				this.m_storage[key] = pref;
				this.m_dirty = true;
			}
		}
	}

	// Token: 0x06004772 RID: 18290 RVA: 0x001C160C File Offset: 0x001BF80C
	public int GetInt(string key, int defaultValue)
	{
		object storageLock = this.m_storageLock;
		int result;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			int num;
			result = ((this.m_storage.TryGetValue(key, out pref) && pref.TryGet(out num)) ? num : defaultValue);
		}
		return result;
	}

	// Token: 0x06004773 RID: 18291 RVA: 0x001C1668 File Offset: 0x001BF868
	public void SetInt(string key, int value)
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			if (this.m_storage.TryGetValue(key, out pref))
			{
				int num;
				if (!pref.TryGet(out num) || num != value)
				{
					pref.Set(value);
					this.m_dirty = true;
				}
			}
			else
			{
				pref = new SaveDataPrefsFile.Pref(value);
				this.m_storage[key] = pref;
				this.m_dirty = true;
			}
		}
	}

	// Token: 0x06004774 RID: 18292 RVA: 0x001C16F0 File Offset: 0x001BF8F0
	public string GetString(string key, string defaultValue)
	{
		object storageLock = this.m_storageLock;
		string result;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			string text;
			result = ((this.m_storage.TryGetValue(key, out pref) && pref.TryGet(out text)) ? text : defaultValue);
		}
		return result;
	}

	// Token: 0x06004775 RID: 18293 RVA: 0x001C174C File Offset: 0x001BF94C
	public void SetString(string key, string value)
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			SaveDataPrefsFile.Pref pref;
			if (this.m_storage.TryGetValue(key, out pref))
			{
				string a;
				if (!pref.TryGet(out a) || !(a == value))
				{
					pref.Set(value);
					this.m_dirty = true;
				}
			}
			else
			{
				pref = new SaveDataPrefsFile.Pref(value);
				this.m_storage[key] = pref;
				this.m_dirty = true;
			}
		}
	}

	// Token: 0x06004776 RID: 18294 RVA: 0x001C17D8 File Offset: 0x001BF9D8
	public bool HasKey(string key)
	{
		object storageLock = this.m_storageLock;
		bool result;
		lock (storageLock)
		{
			result = this.m_storage.ContainsKey(key);
		}
		return result;
	}

	// Token: 0x06004777 RID: 18295 RVA: 0x001C1820 File Offset: 0x001BFA20
	public void DeleteKey(string key)
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			this.m_storage.Remove(key);
		}
	}

	// Token: 0x06004778 RID: 18296 RVA: 0x001C1868 File Offset: 0x001BFA68
	public void DeleteAll()
	{
		object storageLock = this.m_storageLock;
		lock (storageLock)
		{
			this.m_storage.Clear();
		}
	}

	// Token: 0x06004779 RID: 18297 RVA: 0x001C18B0 File Offset: 0x001BFAB0
	public void Save()
	{
		this.SaveInternal();
	}

	// Token: 0x0600477A RID: 18298 RVA: 0x001C18B8 File Offset: 0x001BFAB8
	public void Load()
	{
		this.LoadInternal();
	}

	// Token: 0x1700077B RID: 1915
	// (get) Token: 0x0600477B RID: 18299 RVA: 0x000197A5 File Offset: 0x000179A5
	public bool CanLoad
	{
		get
		{
			return true;
		}
	}

	// Token: 0x040036D7 RID: 14039
	[PublicizedFrom(EAccessModifier.Private)]
	public static SaveDataPrefsFile s_instance;

	// Token: 0x040036D8 RID: 14040
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<char, char> EscapeMapping = new Dictionary<char, char>
	{
		{
			'\0',
			'0'
		},
		{
			'\r',
			'r'
		},
		{
			'\n',
			'n'
		},
		{
			'=',
			'='
		},
		{
			'\\',
			'\\'
		}
	};

	// Token: 0x040036D9 RID: 14041
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<char, char> UnescapeMapping = SaveDataPrefsFile.EscapeMapping.ToDictionary((KeyValuePair<char, char> pair) => pair.Value, (KeyValuePair<char, char> pair) => pair.Key);

	// Token: 0x040036DA RID: 14042
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<SaveDataPrefsFile.PrefType, char> PrefTypeMapping = new Dictionary<SaveDataPrefsFile.PrefType, char>
	{
		{
			SaveDataPrefsFile.PrefType.Float,
			'F'
		},
		{
			SaveDataPrefsFile.PrefType.Int,
			'I'
		},
		{
			SaveDataPrefsFile.PrefType.String,
			'S'
		}
	};

	// Token: 0x040036DB RID: 14043
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Dictionary<char, SaveDataPrefsFile.PrefType> PrefTypeUnmapping = SaveDataPrefsFile.PrefTypeMapping.ToDictionary((KeyValuePair<SaveDataPrefsFile.PrefType, char> kv) => kv.Value, (KeyValuePair<SaveDataPrefsFile.PrefType, char> kv) => kv.Key);

	// Token: 0x040036DC RID: 14044
	[PublicizedFrom(EAccessModifier.Private)]
	public const string StorageFileName = "prefs.cfg";

	// Token: 0x040036DD RID: 14045
	[PublicizedFrom(EAccessModifier.Private)]
	public const char KeyValueSeparator = '=';

	// Token: 0x040036DE RID: 14046
	[PublicizedFrom(EAccessModifier.Private)]
	public const char EscapePrefix = '\\';

	// Token: 0x040036DF RID: 14047
	[PublicizedFrom(EAccessModifier.Private)]
	public const char PrefTypeSeparator = ':';

	// Token: 0x040036E0 RID: 14048
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly string m_storageFilePath;

	// Token: 0x040036E1 RID: 14049
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, SaveDataPrefsFile.Pref> m_storage;

	// Token: 0x040036E2 RID: 14050
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object m_storageLock = new object();

	// Token: 0x040036E3 RID: 14051
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_dirty;

	// Token: 0x02000946 RID: 2374
	[PublicizedFrom(EAccessModifier.Private)]
	public enum PrefType
	{
		// Token: 0x040036E5 RID: 14053
		Float,
		// Token: 0x040036E6 RID: 14054
		Int,
		// Token: 0x040036E7 RID: 14055
		String
	}

	// Token: 0x02000947 RID: 2375
	[PublicizedFrom(EAccessModifier.Private)]
	public class Pref
	{
		// Token: 0x0600477D RID: 18301 RVA: 0x001C198B File Offset: 0x001BFB8B
		public Pref(float value)
		{
			this.Set(value);
		}

		// Token: 0x0600477E RID: 18302 RVA: 0x001C199A File Offset: 0x001BFB9A
		public Pref(int value)
		{
			this.Set(value);
		}

		// Token: 0x0600477F RID: 18303 RVA: 0x001C19A9 File Offset: 0x001BFBA9
		public Pref(string value)
		{
			this.Set(value);
		}

		// Token: 0x1700077C RID: 1916
		// (get) Token: 0x06004780 RID: 18304 RVA: 0x001C19B8 File Offset: 0x001BFBB8
		public SaveDataPrefsFile.PrefType Type
		{
			get
			{
				return this.m_type;
			}
		}

		// Token: 0x06004781 RID: 18305 RVA: 0x001C19C0 File Offset: 0x001BFBC0
		public bool TryGet(out float value)
		{
			if (this.m_type != SaveDataPrefsFile.PrefType.Float)
			{
				value = 0f;
				return false;
			}
			value = this.m_values.Float;
			return true;
		}

		// Token: 0x06004782 RID: 18306 RVA: 0x001C19E1 File Offset: 0x001BFBE1
		public bool TryGet(out int value)
		{
			if (this.m_type != SaveDataPrefsFile.PrefType.Int)
			{
				value = 0;
				return false;
			}
			value = this.m_values.Int;
			return true;
		}

		// Token: 0x06004783 RID: 18307 RVA: 0x001C19FF File Offset: 0x001BFBFF
		public bool TryGet(out string value)
		{
			if (this.m_type != SaveDataPrefsFile.PrefType.String)
			{
				value = null;
				return false;
			}
			value = this.m_refs.String;
			return true;
		}

		// Token: 0x06004784 RID: 18308 RVA: 0x001C1A1D File Offset: 0x001BFC1D
		public void Set(float value)
		{
			this.m_type = SaveDataPrefsFile.PrefType.Float;
			this.m_values.Float = value;
			this.m_refs.String = null;
		}

		// Token: 0x06004785 RID: 18309 RVA: 0x001C1A3E File Offset: 0x001BFC3E
		public void Set(int value)
		{
			this.m_type = SaveDataPrefsFile.PrefType.Int;
			this.m_values.Int = value;
			this.m_refs.String = null;
		}

		// Token: 0x06004786 RID: 18310 RVA: 0x001C1A5F File Offset: 0x001BFC5F
		public void Set(string value)
		{
			this.m_type = SaveDataPrefsFile.PrefType.String;
			this.m_refs.String = value;
			this.m_values.Int = 0;
		}

		// Token: 0x06004787 RID: 18311 RVA: 0x001C1A80 File Offset: 0x001BFC80
		public bool TryToString(out string stringRepresentation)
		{
			char c;
			if (!SaveDataPrefsFile.PrefTypeMapping.TryGetValue(this.m_type, out c))
			{
				Log.Warning(string.Format("[{0}] No char mapping for pref type '{1}'.", "SaveDataPrefsFile", this.m_type));
				stringRepresentation = null;
				return false;
			}
			switch (this.m_type)
			{
			case SaveDataPrefsFile.PrefType.Float:
				stringRepresentation = string.Format("{0}{1}{2:R}", c, ':', this.m_values.Float);
				return true;
			case SaveDataPrefsFile.PrefType.Int:
				stringRepresentation = string.Format("{0}{1}{2}", c, ':', this.m_values.Int);
				return true;
			case SaveDataPrefsFile.PrefType.String:
				stringRepresentation = string.Format("{0}{1}{2}", c, ':', this.m_refs.String);
				return true;
			default:
				Log.Error(string.Format("[{0}] Missing to string implementation for '{1}'.", "SaveDataPrefsFile", this.m_type));
				stringRepresentation = null;
				return false;
			}
		}

		// Token: 0x06004788 RID: 18312 RVA: 0x001C1B80 File Offset: 0x001BFD80
		public unsafe static bool TryParse(ReadOnlySpan<char> stringRepresentation, out SaveDataPrefsFile.Pref pref)
		{
			if (stringRepresentation.Length < 2 || *stringRepresentation[1] != 58)
			{
				pref = null;
				return false;
			}
			SaveDataPrefsFile.PrefType prefType;
			if (!SaveDataPrefsFile.PrefTypeUnmapping.TryGetValue((char)(*stringRepresentation[0]), out prefType))
			{
				pref = null;
				return false;
			}
			ReadOnlySpan<char> readOnlySpan = stringRepresentation;
			ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(2, readOnlySpan.Length - 2);
			switch (prefType)
			{
			case SaveDataPrefsFile.PrefType.Float:
			{
				float value;
				if (!float.TryParse(readOnlySpan2, out value))
				{
					pref = null;
					return false;
				}
				pref = new SaveDataPrefsFile.Pref(value);
				return true;
			}
			case SaveDataPrefsFile.PrefType.Int:
			{
				int value2;
				if (!int.TryParse(readOnlySpan2, out value2))
				{
					pref = null;
					return false;
				}
				pref = new SaveDataPrefsFile.Pref(value2);
				return true;
			}
			case SaveDataPrefsFile.PrefType.String:
				pref = new SaveDataPrefsFile.Pref(new string(readOnlySpan2));
				return true;
			default:
				Log.Error(string.Format("[{0}] Missing parse implementation for '{1}'.", "SaveDataPrefsFile", prefType));
				pref = null;
				return false;
			}
		}

		// Token: 0x040036E8 RID: 14056
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveDataPrefsFile.PrefType m_type;

		// Token: 0x040036E9 RID: 14057
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveDataPrefsFile.Pref.PrefValues m_values;

		// Token: 0x040036EA RID: 14058
		[PublicizedFrom(EAccessModifier.Private)]
		public SaveDataPrefsFile.Pref.PrefRefs m_refs;

		// Token: 0x02000948 RID: 2376
		[PublicizedFrom(EAccessModifier.Private)]
		[StructLayout(LayoutKind.Explicit)]
		public struct PrefValues
		{
			// Token: 0x040036EB RID: 14059
			[FieldOffset(0)]
			public float Float;

			// Token: 0x040036EC RID: 14060
			[FieldOffset(0)]
			public int Int;
		}

		// Token: 0x02000949 RID: 2377
		[PublicizedFrom(EAccessModifier.Private)]
		[StructLayout(LayoutKind.Explicit)]
		public struct PrefRefs
		{
			// Token: 0x040036ED RID: 14061
			[FieldOffset(0)]
			public string String;
		}
	}
}

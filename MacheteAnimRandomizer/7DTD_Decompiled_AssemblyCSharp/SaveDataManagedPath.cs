using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

// Token: 0x0200093C RID: 2364
public class SaveDataManagedPath : IEquatable<SaveDataManagedPath>, IComparable<SaveDataManagedPath>, IComparable
{
	// Token: 0x060046FC RID: 18172 RVA: 0x001C0028 File Offset: 0x001BE228
	public SaveDataManagedPath(StringSpan pathRelativeToRoot)
	{
		string text;
		this..ctor(SaveDataManagedPath.TryFormatPath(pathRelativeToRoot.AsSpan(), out text) ? text : pathRelativeToRoot.ToString(), true);
	}

	// Token: 0x060046FD RID: 18173 RVA: 0x001C005C File Offset: 0x001BE25C
	public SaveDataManagedPath(string pathRelativeToRoot) : this(pathRelativeToRoot, false)
	{
	}

	// Token: 0x060046FE RID: 18174 RVA: 0x001C0068 File Offset: 0x001BE268
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataManagedPath(string pathRelativeToRoot, bool alreadyFormatted)
	{
		if (pathRelativeToRoot == null)
		{
			throw new ArgumentNullException("pathRelativeToRoot");
		}
		if (alreadyFormatted)
		{
			this.PathRelativeToRoot = pathRelativeToRoot;
		}
		else
		{
			string text;
			this.PathRelativeToRoot = (SaveDataManagedPath.TryFormatPath(pathRelativeToRoot, out text) ? text : pathRelativeToRoot);
		}
		bool flag;
		try
		{
			flag = Path.IsPathRooted(this.PathRelativeToRoot);
		}
		catch (ArgumentException innerException)
		{
			throw new ArgumentException("Failed to check if path was rooted. " + this.PathRelativeToRoot, "pathRelativeToRoot", innerException);
		}
		if (flag)
		{
			throw new ArgumentException("Path should not be rooted. " + this.PathRelativeToRoot, "pathRelativeToRoot");
		}
		this.Type = this.GetSaveDataType();
		this.SlotPathRange = this.GetSlotPathRange();
		this.PathRelativeToSlotRange = this.GetPathRelativeToSlotRange();
		this.Slot = new SaveDataSlot(this);
	}

	// Token: 0x060046FF RID: 18175 RVA: 0x001C0138 File Offset: 0x001BE338
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe static bool TryFormatPath(ReadOnlySpan<char> unformattedPath, out string formattedPath)
	{
		ReadOnlySpan<char> readOnlySpan = unformattedPath.Trim(" \\/");
		if (readOnlySpan.Length <= 0)
		{
			formattedPath = string.Empty;
			return true;
		}
		bool flag = false;
		int num = 0;
		bool flag2 = false;
		ReadOnlySpan<char> readOnlySpan2 = readOnlySpan;
		int i = 0;
		while (i < readOnlySpan2.Length)
		{
			char c = (char)(*readOnlySpan2[i]);
			if (c == '/')
			{
				goto IL_4C;
			}
			if (c == '\\')
			{
				flag = true;
				goto IL_4C;
			}
			flag2 = false;
			IL_5B:
			i++;
			continue;
			IL_4C:
			if (!flag2)
			{
				flag2 = true;
				goto IL_5B;
			}
			num++;
			goto IL_5B;
		}
		if (readOnlySpan.Length == unformattedPath.Length && !flag && num == 0)
		{
			formattedPath = null;
			return false;
		}
		fixed (char* pinnableReference = readOnlySpan.GetPinnableReference())
		{
			void* value = (void*)pinnableReference;
			ValueTuple<IntPtr, int> state = new ValueTuple<IntPtr, int>((IntPtr)value, readOnlySpan.Length);
			formattedPath = string.Create<ValueTuple<IntPtr, int>>(readOnlySpan.Length - num, state, delegate(Span<char> span, [TupleElementNames(new string[]
			{
				null,
				"Length"
			})] ValueTuple<IntPtr, int> data)
			{
				IntPtr item = data.Item1;
				int item2 = data.Item2;
				ReadOnlySpan<char> readOnlySpan3 = new ReadOnlySpan<char>(item.ToPointer(), item2);
				int num2 = 0;
				bool flag3 = false;
				ReadOnlySpan<char> readOnlySpan4 = readOnlySpan3;
				for (int j = 0; j < readOnlySpan4.Length; j++)
				{
					char c2 = (char)(*readOnlySpan4[j]);
					if (c2 == '\\' || c2 == '/')
					{
						if (!flag3)
						{
							flag3 = true;
							*span[num2++] = '/';
						}
					}
					else
					{
						flag3 = false;
						*span[num2++] = c2;
					}
				}
			});
		}
		return true;
	}

	// Token: 0x1700076E RID: 1902
	// (get) Token: 0x06004700 RID: 18176 RVA: 0x001C0225 File Offset: 0x001BE425
	public SaveDataType Type { get; }

	// Token: 0x06004701 RID: 18177 RVA: 0x001C0230 File Offset: 0x001BE430
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataType GetSaveDataType()
	{
		foreach (SaveDataType saveDataType in EnumUtils.Values<SaveDataType>())
		{
			if (!saveDataType.IsRoot())
			{
				string pathRaw = saveDataType.GetPathRaw();
				if (this.PathRelativeToRoot.IndexOf(pathRaw, StringComparison.Ordinal) == 0 && this.PathRelativeToRoot.Length >= pathRaw.Length + 2 && this.PathRelativeToRoot[pathRaw.Length] == '/')
				{
					return saveDataType;
				}
			}
		}
		return SaveDataType.User;
	}

	// Token: 0x1700076F RID: 1903
	// (get) Token: 0x06004702 RID: 18178 RVA: 0x001C02C4 File Offset: 0x001BE4C4
	public Range SlotPathRange { [PublicizedFrom(EAccessModifier.Private)] get; }

	// Token: 0x17000770 RID: 1904
	// (get) Token: 0x06004703 RID: 18179 RVA: 0x001C02CC File Offset: 0x001BE4CC
	public StringSpan SlotPath
	{
		get
		{
			string pathRelativeToRoot = this.PathRelativeToRoot;
			Range slotPathRange = this.SlotPathRange;
			int length = pathRelativeToRoot.Length;
			int offset = slotPathRange.Start.GetOffset(length);
			int length2 = slotPathRange.End.GetOffset(length) - offset;
			return pathRelativeToRoot.Substring(offset, length2);
		}
	}

	// Token: 0x17000771 RID: 1905
	// (get) Token: 0x06004704 RID: 18180 RVA: 0x001C031F File Offset: 0x001BE51F
	public SaveDataSlot Slot { get; }

	// Token: 0x06004705 RID: 18181 RVA: 0x001C0328 File Offset: 0x001BE528
	[PublicizedFrom(EAccessModifier.Private)]
	public Range GetSlotPathRange()
	{
		int num = this.Type.GetSlotPathDepth();
		string pathRaw = this.Type.GetPathRaw();
		if (num <= 0 || pathRaw.Length == 0 || this.PathRelativeToRoot.Length < pathRaw.Length + 2)
		{
			return new Range(pathRaw.Length, pathRaw.Length);
		}
		for (int i = pathRaw.Length + 1; i < this.PathRelativeToRoot.Length; i++)
		{
			if (this.PathRelativeToRoot[i] == '/')
			{
				num--;
				if (num <= 0)
				{
					int value = pathRaw.Length + 1;
					int value2 = i;
					return new Range(value, value2);
				}
			}
		}
		return new Range(pathRaw.Length, pathRaw.Length);
	}

	// Token: 0x17000772 RID: 1906
	// (get) Token: 0x06004706 RID: 18182 RVA: 0x001C03F5 File Offset: 0x001BE5F5
	public Range PathRelativeToSlotRange { [PublicizedFrom(EAccessModifier.Private)] get; }

	// Token: 0x17000773 RID: 1907
	// (get) Token: 0x06004707 RID: 18183 RVA: 0x001C0400 File Offset: 0x001BE600
	public StringSpan PathRelativeToSlot
	{
		get
		{
			string pathRelativeToRoot = this.PathRelativeToRoot;
			Range pathRelativeToSlotRange = this.PathRelativeToSlotRange;
			int length = pathRelativeToRoot.Length;
			int offset = pathRelativeToSlotRange.Start.GetOffset(length);
			int length2 = pathRelativeToSlotRange.End.GetOffset(length) - offset;
			return pathRelativeToRoot.Substring(offset, length2);
		}
	}

	// Token: 0x06004708 RID: 18184 RVA: 0x001C0454 File Offset: 0x001BE654
	[PublicizedFrom(EAccessModifier.Private)]
	public Range GetPathRelativeToSlotRange()
	{
		int offset = this.SlotPathRange.End.GetOffset(this.PathRelativeToRoot.Length);
		if (offset >= this.PathRelativeToRoot.Length)
		{
			return new Range(offset, offset);
		}
		int num = (this.PathRelativeToRoot[offset] == '/') ? (offset + 1) : offset;
		if (num >= this.PathRelativeToRoot.Length)
		{
			return new Range(offset, offset);
		}
		return new Range(num, this.PathRelativeToRoot.Length);
	}

	// Token: 0x06004709 RID: 18185 RVA: 0x001C04F5 File Offset: 0x001BE6F5
	public string GetOriginalPath()
	{
		return GameIO.GetNormalizedPath(Path.Combine(SaveDataUtils.s_saveDataRootPathPrefix, this.PathRelativeToRoot));
	}

	// Token: 0x0600470A RID: 18186 RVA: 0x001C050C File Offset: 0x001BE70C
	public SaveDataManagedPath GetChildPath(StringSpan childPath)
	{
		return new SaveDataManagedPath(SpanUtils.Concat(this.PathRelativeToRoot, "/", childPath));
	}

	// Token: 0x0600470B RID: 18187 RVA: 0x001C0530 File Offset: 0x001BE730
	public bool TryGetParentPath(out SaveDataManagedPath parentPath)
	{
		if (this.PathRelativeToRoot.Length <= 0)
		{
			parentPath = null;
			return false;
		}
		int num = this.PathRelativeToRoot.LastIndexOf('/');
		if (num < 0)
		{
			parentPath = SaveDataManagedPath.RootPath;
			return true;
		}
		parentPath = new SaveDataManagedPath(this.PathRelativeToRoot.Substring(0, num));
		return true;
	}

	// Token: 0x0600470C RID: 18188 RVA: 0x001C0580 File Offset: 0x001BE780
	public bool IsParentOf(SaveDataManagedPath childPath)
	{
		return SaveDataManagedPath.<IsParentOf>g__IsParentOfInternal|28_0(this.PathRelativeToRoot, childPath.PathRelativeToRoot);
	}

	// Token: 0x0600470D RID: 18189 RVA: 0x001C0593 File Offset: 0x001BE793
	public override string ToString()
	{
		return this.PathRelativeToRoot;
	}

	// Token: 0x0600470E RID: 18190 RVA: 0x001C059B File Offset: 0x001BE79B
	public bool Equals(SaveDataManagedPath other)
	{
		return other != null && (this == other || this.PathRelativeToRoot == other.PathRelativeToRoot);
	}

	// Token: 0x0600470F RID: 18191 RVA: 0x001C05B9 File Offset: 0x001BE7B9
	public override bool Equals(object obj)
	{
		return obj != null && (this == obj || (!(obj.GetType() != base.GetType()) && this.Equals((SaveDataManagedPath)obj)));
	}

	// Token: 0x06004710 RID: 18192 RVA: 0x001C05E7 File Offset: 0x001BE7E7
	public override int GetHashCode()
	{
		return this.PathRelativeToRoot.GetHashCode();
	}

	// Token: 0x06004711 RID: 18193 RVA: 0x001C05F4 File Offset: 0x001BE7F4
	public static bool operator ==(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return object.Equals(left, right);
	}

	// Token: 0x06004712 RID: 18194 RVA: 0x001C05FD File Offset: 0x001BE7FD
	public static bool operator !=(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return !object.Equals(left, right);
	}

	// Token: 0x06004713 RID: 18195 RVA: 0x001C0609 File Offset: 0x001BE809
	public int CompareTo(SaveDataManagedPath other)
	{
		if (other == null)
		{
			return 1;
		}
		if (this == other)
		{
			return 0;
		}
		return string.Compare(this.PathRelativeToRoot, other.PathRelativeToRoot, StringComparison.Ordinal);
	}

	// Token: 0x06004714 RID: 18196 RVA: 0x001C0628 File Offset: 0x001BE828
	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (this == obj)
		{
			return 0;
		}
		SaveDataManagedPath saveDataManagedPath = obj as SaveDataManagedPath;
		if (saveDataManagedPath == null)
		{
			throw new ArgumentException("Object must be of type SaveDataManagedPath");
		}
		return this.CompareTo(saveDataManagedPath);
	}

	// Token: 0x06004715 RID: 18197 RVA: 0x001C065C File Offset: 0x001BE85C
	public static bool operator <(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return Comparer<SaveDataManagedPath>.Default.Compare(left, right) < 0;
	}

	// Token: 0x06004716 RID: 18198 RVA: 0x001C066D File Offset: 0x001BE86D
	public static bool operator >(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return Comparer<SaveDataManagedPath>.Default.Compare(left, right) > 0;
	}

	// Token: 0x06004717 RID: 18199 RVA: 0x001C067E File Offset: 0x001BE87E
	public static bool operator <=(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return Comparer<SaveDataManagedPath>.Default.Compare(left, right) <= 0;
	}

	// Token: 0x06004718 RID: 18200 RVA: 0x001C0692 File Offset: 0x001BE892
	public static bool operator >=(SaveDataManagedPath left, SaveDataManagedPath right)
	{
		return Comparer<SaveDataManagedPath>.Default.Compare(left, right) >= 0;
	}

	// Token: 0x0600471A RID: 18202 RVA: 0x001C06B8 File Offset: 0x001BE8B8
	[CompilerGenerated]
	[PublicizedFrom(EAccessModifier.Internal)]
	public static bool <IsParentOf>g__IsParentOfInternal|28_0(string parent, string child)
	{
		if (parent.Length >= child.Length)
		{
			return false;
		}
		if (parent.Length == 0)
		{
			return true;
		}
		if (child[parent.Length] != '/')
		{
			return false;
		}
		for (int i = 0; i < parent.Length; i++)
		{
			if (parent[i] != child[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x040036B7 RID: 14007
	public static readonly SaveDataManagedPath RootPath = new SaveDataManagedPath(string.Empty);

	// Token: 0x040036B8 RID: 14008
	public readonly string PathRelativeToRoot;
}

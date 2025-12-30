using System;

// Token: 0x0200093E RID: 2366
public readonly struct SaveDataSlot : IEquatable<SaveDataSlot>, IComparable<SaveDataSlot>, IComparable
{
	// Token: 0x0600471E RID: 18206 RVA: 0x001C07A8 File Offset: 0x001BE9A8
	[PublicizedFrom(EAccessModifier.Private)]
	public SaveDataSlot(SaveDataType type, StringSpan slotPath)
	{
		this = new SaveDataSlot(new SaveDataManagedPath((slotPath.Length > 0) ? SpanUtils.Concat(type.GetPathRaw(), "/", slotPath, "/d") : SpanUtils.Concat(type.GetPathRaw(), "/d")));
		if (this.Type != type)
		{
			throw new ArgumentException(string.Format("Got type {0} but expected {1}. Make sure that concatenating the slot path does not match another type.", this.Type, type), "type");
		}
		if (this.SlotPath != slotPath)
		{
			throw new ArgumentException(SpanUtils.Concat("Expected slot path to be '", slotPath, "', but was: ", this.SlotPath), "slotPath");
		}
	}

	// Token: 0x0600471F RID: 18207 RVA: 0x001C0873 File Offset: 0x001BEA73
	public SaveDataSlot(SaveDataManagedPath managedPath)
	{
		this.m_internalPath = managedPath;
	}

	// Token: 0x17000774 RID: 1908
	// (get) Token: 0x06004720 RID: 18208 RVA: 0x001C087C File Offset: 0x001BEA7C
	public SaveDataType Type
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_internalPath.Type;
		}
	}

	// Token: 0x17000775 RID: 1909
	// (get) Token: 0x06004721 RID: 18209 RVA: 0x001C0889 File Offset: 0x001BEA89
	public StringSpan SlotPath
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return this.m_internalPath.SlotPath;
		}
	}

	// Token: 0x06004722 RID: 18210 RVA: 0x001C0896 File Offset: 0x001BEA96
	public SaveDataSlot GetSimpleSlot()
	{
		if (!(this.m_internalPath.PathRelativeToSlot == "d"))
		{
			return new SaveDataSlot(this.Type, this.SlotPath);
		}
		return this;
	}

	// Token: 0x06004723 RID: 18211 RVA: 0x001C08CC File Offset: 0x001BEACC
	public override string ToString()
	{
		if (this.SlotPath.Length != 0)
		{
			return SpanUtils.Concat(this.Type.ToStringCached<SaveDataType>(), "[", this.SlotPath, "]");
		}
		return this.Type.ToStringCached<SaveDataType>();
	}

	// Token: 0x06004724 RID: 18212 RVA: 0x001C0924 File Offset: 0x001BEB24
	public bool Equals(SaveDataSlot other)
	{
		return this.Type == other.Type && this.SlotPath == other.SlotPath;
	}

	// Token: 0x06004725 RID: 18213 RVA: 0x001C094C File Offset: 0x001BEB4C
	public override bool Equals(object obj)
	{
		if (obj is SaveDataSlot)
		{
			SaveDataSlot other = (SaveDataSlot)obj;
			return this.Equals(other);
		}
		return false;
	}

	// Token: 0x06004726 RID: 18214 RVA: 0x001C0974 File Offset: 0x001BEB74
	public override int GetHashCode()
	{
		return (int)(this.Type * (SaveDataType)397 ^ (SaveDataType)this.SlotPath.GetHashCode());
	}

	// Token: 0x06004727 RID: 18215 RVA: 0x001C09A2 File Offset: 0x001BEBA2
	public static bool operator ==(SaveDataSlot left, SaveDataSlot right)
	{
		return left.Equals(right);
	}

	// Token: 0x06004728 RID: 18216 RVA: 0x001C09AC File Offset: 0x001BEBAC
	public static bool operator !=(SaveDataSlot left, SaveDataSlot right)
	{
		return !left.Equals(right);
	}

	// Token: 0x06004729 RID: 18217 RVA: 0x001C09BC File Offset: 0x001BEBBC
	public int CompareTo(SaveDataSlot other)
	{
		int num = this.Type.CompareTo(other.Type);
		if (num != 0)
		{
			return num;
		}
		return this.SlotPath.CompareTo(other.SlotPath, StringComparison.Ordinal);
	}

	// Token: 0x0600472A RID: 18218 RVA: 0x001C0A08 File Offset: 0x001BEC08
	public int CompareTo(object obj)
	{
		if (obj == null)
		{
			return 1;
		}
		if (obj is SaveDataSlot)
		{
			SaveDataSlot other = (SaveDataSlot)obj;
			return this.CompareTo(other);
		}
		throw new ArgumentException("Object must be of type SaveDataSlot");
	}

	// Token: 0x0600472B RID: 18219 RVA: 0x001C0A3D File Offset: 0x001BEC3D
	public static bool operator <(SaveDataSlot left, SaveDataSlot right)
	{
		return left.CompareTo(right) < 0;
	}

	// Token: 0x0600472C RID: 18220 RVA: 0x001C0A4A File Offset: 0x001BEC4A
	public static bool operator >(SaveDataSlot left, SaveDataSlot right)
	{
		return left.CompareTo(right) > 0;
	}

	// Token: 0x0600472D RID: 18221 RVA: 0x001C0A57 File Offset: 0x001BEC57
	public static bool operator <=(SaveDataSlot left, SaveDataSlot right)
	{
		return left.CompareTo(right) <= 0;
	}

	// Token: 0x0600472E RID: 18222 RVA: 0x001C0A67 File Offset: 0x001BEC67
	public static bool operator >=(SaveDataSlot left, SaveDataSlot right)
	{
		return left.CompareTo(right) >= 0;
	}

	// Token: 0x040036BF RID: 14015
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DUMMY_POSTFIX_WITHOUT_SLASH = "d";

	// Token: 0x040036C0 RID: 14016
	[PublicizedFrom(EAccessModifier.Private)]
	public const string DUMMY_POSTFIX_WITH_SLASH = "/d";

	// Token: 0x040036C1 RID: 14017
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly SaveDataManagedPath m_internalPath;
}

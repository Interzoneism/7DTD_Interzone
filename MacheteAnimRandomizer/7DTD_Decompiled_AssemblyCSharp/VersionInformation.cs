using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

// Token: 0x02001094 RID: 4244
public class VersionInformation : IComparable<VersionInformation>
{
	// Token: 0x060085F2 RID: 34290 RVA: 0x00366710 File Offset: 0x00364910
	public VersionInformation(VersionInformation.EGameReleaseType _releaseType, int _major, int _minor, int _build)
	{
		this.ReleaseType = _releaseType;
		this.Major = _major;
		this.Minor = _minor;
		this.Build = _build;
		this.NumericalRepresentation = (int)((this.Major < 1) ? ((VersionInformation.EGameReleaseType)(-1)) : (((this.ReleaseType * (VersionInformation.EGameReleaseType)100 + this.Major) * (VersionInformation.EGameReleaseType)100 + this.Minor) * (VersionInformation.EGameReleaseType)1000 + this.Build));
		this.ShortString = ((this.Major < 1) ? "Unk" : string.Format("{0}{1}.{2}", this.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>()[0], this.Major, this.Minor));
		this.LongStringNoBuild = ((this.Major < 1) ? "Unknown" : string.Format("{0} {1}.{2}", this.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>(), this.Major, this.Minor));
		this.LongString = ((this.Major < 1) ? "Unknown" : string.Format("{0} {1}.{2} (b{3})", new object[]
		{
			this.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>(),
			this.Major,
			this.Minor,
			this.Build
		}));
		this.SerializableString = string.Format("{0}.{1}.{2}.{3}", new object[]
		{
			this.ReleaseType.ToStringCached<VersionInformation.EGameReleaseType>(),
			this.Major,
			this.Minor,
			this.Build
		});
		this.Version = new Version((int)((this.ReleaseType >= VersionInformation.EGameReleaseType.Alpha) ? this.ReleaseType : VersionInformation.EGameReleaseType.Alpha), (this.Major >= 0) ? this.Major : 0, (this.Minor >= 0) ? this.Minor : 0, (this.Build >= 0) ? this.Build : 0);
		this.IsValid = (this.Major > 0);
	}

	// Token: 0x060085F3 RID: 34291 RVA: 0x00366914 File Offset: 0x00364B14
	public int CompareTo(VersionInformation _other)
	{
		int num = this.ReleaseType.CompareTo(_other.ReleaseType);
		if (num != 0)
		{
			return num;
		}
		int num2 = this.Major.CompareTo(_other.Major);
		if (num2 != 0)
		{
			return num2;
		}
		int num3 = this.Minor.CompareTo(_other.Minor);
		if (num3 != 0)
		{
			return num3;
		}
		return this.Build.CompareTo(_other.Build);
	}

	// Token: 0x060085F4 RID: 34292 RVA: 0x0036698B File Offset: 0x00364B8B
	public bool EqualsMinor(VersionInformation _other)
	{
		return this.ReleaseType == _other.ReleaseType && this.Major == _other.Major && this.Minor == _other.Minor;
	}

	// Token: 0x060085F5 RID: 34293 RVA: 0x003669B9 File Offset: 0x00364BB9
	public bool EqualsMajor(VersionInformation _other)
	{
		return this.ReleaseType == _other.ReleaseType && this.Major == _other.Major;
	}

	// Token: 0x060085F6 RID: 34294 RVA: 0x003669DC File Offset: 0x00364BDC
	public VersionInformation.EVersionComparisonResult CompareToRunningBuild()
	{
		VersionInformation cVersionInformation = Constants.cVersionInformation;
		if (this.ReleaseType != cVersionInformation.ReleaseType || this.Major != cVersionInformation.Major)
		{
			return VersionInformation.EVersionComparisonResult.DifferentMajor;
		}
		if (this.Minor < cVersionInformation.Minor)
		{
			return VersionInformation.EVersionComparisonResult.OlderMinor;
		}
		if (this.Minor > cVersionInformation.Minor)
		{
			return VersionInformation.EVersionComparisonResult.NewerMinor;
		}
		if (this.Build != cVersionInformation.Build)
		{
			return VersionInformation.EVersionComparisonResult.SameMinor;
		}
		return VersionInformation.EVersionComparisonResult.SameBuild;
	}

	// Token: 0x060085F7 RID: 34295 RVA: 0x00366A40 File Offset: 0x00364C40
	public static bool TryParseSerializedString(string _serializedVersionInformation, out VersionInformation _result)
	{
		_result = null;
		string[] array = _serializedVersionInformation.Split('.', StringSplitOptions.None);
		if (array.Length != 4)
		{
			return false;
		}
		VersionInformation.EGameReleaseType releaseType;
		if (!EnumUtils.TryParse<VersionInformation.EGameReleaseType>(array[0], out releaseType, false))
		{
			return false;
		}
		int major;
		if (!StringParsers.TryParseSInt32(array[1], out major, 0, -1, NumberStyles.Integer))
		{
			return false;
		}
		int minor;
		if (!StringParsers.TryParseSInt32(array[2], out minor, 0, -1, NumberStyles.Integer))
		{
			return false;
		}
		int build;
		if (!StringParsers.TryParseSInt32(array[3], out build, 0, -1, NumberStyles.Integer))
		{
			return false;
		}
		_result = new VersionInformation(releaseType, major, minor, build);
		return true;
	}

	// Token: 0x060085F8 RID: 34296 RVA: 0x00366AB4 File Offset: 0x00364CB4
	public static bool TryParseLegacyString(string _legacyVersionString, out VersionInformation _verInfo)
	{
		Match match = VersionInformation.legacyVersionStringMatcher.Match(_legacyVersionString);
		_verInfo = null;
		if (!match.Success)
		{
			return false;
		}
		int major;
		if (!StringParsers.TryParseSInt32(match.Groups[1].Value, out major, 0, -1, NumberStyles.Integer))
		{
			return false;
		}
		int minor;
		if (match.Groups[2].Success)
		{
			if (!StringParsers.TryParseSInt32(match.Groups[2].Value, out minor, 0, -1, NumberStyles.Integer))
			{
				return false;
			}
		}
		else
		{
			minor = 0;
		}
		int build;
		if (!StringParsers.TryParseSInt32(match.Groups[3].Value, out build, 0, -1, NumberStyles.Integer))
		{
			return false;
		}
		_verInfo = new VersionInformation(VersionInformation.EGameReleaseType.Alpha, major, minor, build);
		return true;
	}

	// Token: 0x060085F9 RID: 34297 RVA: 0x00366B58 File Offset: 0x00364D58
	public void Write(BinaryWriter _writer)
	{
		_writer.Write((byte)this.ReleaseType);
		_writer.Write(this.Major);
		_writer.Write(this.Minor);
		_writer.Write(this.Build);
	}

	// Token: 0x060085FA RID: 34298 RVA: 0x00366B8C File Offset: 0x00364D8C
	public static VersionInformation Read(BinaryReader _reader)
	{
		VersionInformation.EGameReleaseType releaseType = (VersionInformation.EGameReleaseType)_reader.ReadByte();
		int major = _reader.ReadInt32();
		int minor = _reader.ReadInt32();
		int build = _reader.ReadInt32();
		return new VersionInformation(releaseType, major, minor, build);
	}

	// Token: 0x04006804 RID: 26628
	public readonly VersionInformation.EGameReleaseType ReleaseType;

	// Token: 0x04006805 RID: 26629
	public readonly int Major;

	// Token: 0x04006806 RID: 26630
	public readonly int Minor;

	// Token: 0x04006807 RID: 26631
	public readonly int Build;

	// Token: 0x04006808 RID: 26632
	public readonly bool IsValid;

	// Token: 0x04006809 RID: 26633
	public readonly int NumericalRepresentation;

	// Token: 0x0400680A RID: 26634
	public readonly string ShortString;

	// Token: 0x0400680B RID: 26635
	public readonly string LongStringNoBuild;

	// Token: 0x0400680C RID: 26636
	public readonly string LongString;

	// Token: 0x0400680D RID: 26637
	public readonly string SerializableString;

	// Token: 0x0400680E RID: 26638
	public readonly Version Version;

	// Token: 0x0400680F RID: 26639
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly Regex legacyVersionStringMatcher = new Regex("^\\s*Alpha\\s*(\\d+)(?:\\.(\\d+))?\\s*(?:\\(b(\\d+)\\))?\\s*$");

	// Token: 0x02001095 RID: 4245
	public enum EGameReleaseType
	{
		// Token: 0x04006811 RID: 26641
		Alpha,
		// Token: 0x04006812 RID: 26642
		V
	}

	// Token: 0x02001096 RID: 4246
	public enum EVersionComparisonResult
	{
		// Token: 0x04006814 RID: 26644
		SameBuild,
		// Token: 0x04006815 RID: 26645
		SameMinor,
		// Token: 0x04006816 RID: 26646
		NewerMinor,
		// Token: 0x04006817 RID: 26647
		OlderMinor,
		// Token: 0x04006818 RID: 26648
		DifferentMajor
	}
}

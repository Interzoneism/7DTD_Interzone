using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using Platform;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x0200005F RID: 95
[DoNotTouchSerializableFlags]
[Preserve]
[Serializable]
public abstract class PlatformUserIdentifierAbs : IEquatable<PlatformUserIdentifierAbs>
{
	// Token: 0x17000036 RID: 54
	// (get) Token: 0x060001BA RID: 442
	public abstract EPlatformIdentifier PlatformIdentifier { get; }

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060001BB RID: 443
	public abstract string PlatformIdentifierString { get; }

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060001BC RID: 444
	public abstract string ReadablePlatformUserIdentifier { get; }

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060001BD RID: 445
	public abstract string CombinedString { get; }

	// Token: 0x060001BE RID: 446
	public abstract bool DecodeTicket(string _ticket);

	// Token: 0x060001BF RID: 447 RVA: 0x0000FB42 File Offset: 0x0000DD42
	[PublicizedFrom(EAccessModifier.ProtectedInternal)]
	public virtual int GetCustomDataLengthEstimate()
	{
		return 0;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.ProtectedInternal)]
	public virtual void WriteCustomData(BinaryWriter _writer)
	{
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.ProtectedInternal)]
	public virtual void ReadCustomData(BinaryReader _reader)
	{
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x0000FB48 File Offset: 0x0000DD48
	public static PlatformUserIdentifierAbs FromStream(Stream _sourceStream, bool _errorOnEmpty = false, bool _inclCustomData = false)
	{
		PlatformUserIdentifierAbs result;
		using (PooledBinaryReader pooledBinaryReader = MemoryPools.poolBinaryReader.AllocSync(true))
		{
			pooledBinaryReader.SetBaseStream(_sourceStream);
			result = PlatformUserIdentifierAbs.FromStream(pooledBinaryReader, _errorOnEmpty, false);
		}
		return result;
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000FB90 File Offset: 0x0000DD90
	public static PlatformUserIdentifierAbs FromStream(BinaryReader _sourceReader, bool _errorOnEmpty = false, bool _inclCustomData = false)
	{
		if (!_sourceReader.ReadBoolean())
		{
			if (_errorOnEmpty)
			{
				Log.Error("Empty user identifier string\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		_sourceReader.ReadByte();
		string platformName = _sourceReader.ReadString();
		string userId = _sourceReader.ReadString();
		bool logErrors = GameManager.Instance;
		PlatformUserIdentifierAbs platformUserIdentifierAbs = PlatformUserIdentifierAbs.FromPlatformAndId(platformName, userId, logErrors);
		if (_inclCustomData)
		{
			platformUserIdentifierAbs.ReadCustomData(_sourceReader);
		}
		return platformUserIdentifierAbs;
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000FBF0 File Offset: 0x0000DDF0
	[return: TupleElementNames(new string[]
	{
		"platformName",
		"userId"
	})]
	public static ValueTuple<string, string>? FieldsFromStream(BinaryReader _sourceReader, bool _errorOnEmpty = false)
	{
		if (!_sourceReader.ReadBoolean())
		{
			if (_errorOnEmpty)
			{
				Log.Error("Empty user identifier string\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		_sourceReader.ReadByte();
		string item = _sourceReader.ReadString();
		string item2 = _sourceReader.ReadString();
		return new ValueTuple<string, string>?(new ValueTuple<string, string>(item, item2));
	}

	// Token: 0x060001C5 RID: 453 RVA: 0x0000FC45 File Offset: 0x0000DE45
	public void ToXml(XmlElement _xmlElement, string _attributePrefix = "")
	{
		_xmlElement.SetAttrib(_attributePrefix + "platform", this.PlatformIdentifierString);
		_xmlElement.SetAttrib(_attributePrefix + "userid", this.ReadablePlatformUserIdentifier);
	}

	// Token: 0x060001C6 RID: 454 RVA: 0x0000FC78 File Offset: 0x0000DE78
	public static PlatformUserIdentifierAbs FromXml(XmlElement _xmlElement, bool _warnings = true, string _attributePrefix = null)
	{
		string text = "platform";
		string text2 = "userid";
		if (!string.IsNullOrEmpty(_attributePrefix))
		{
			text = _attributePrefix + text;
			text2 = _attributePrefix + text2;
		}
		if (_xmlElement.HasAttribute(text) && _xmlElement.HasAttribute(text2))
		{
			string attribute = _xmlElement.GetAttribute(text);
			string attribute2 = _xmlElement.GetAttribute(text2);
			return PlatformUserIdentifierAbs.FromPlatformAndId(attribute, attribute2, true);
		}
		if (_warnings)
		{
			Log.Warning(string.Concat(new string[]
			{
				"Entry missing '",
				text,
				"' or '",
				text2,
				"' attribute: ",
				_xmlElement.OuterXml
			}));
		}
		return null;
	}

	// Token: 0x060001C7 RID: 455 RVA: 0x0000FD0F File Offset: 0x0000DF0F
	public static bool TryFromCombinedString(string _combinedString, out PlatformUserIdentifierAbs _userIdentifier)
	{
		_userIdentifier = PlatformUserIdentifierAbs.FromCombinedString(_combinedString, false);
		return _userIdentifier != null;
	}

	// Token: 0x060001C8 RID: 456 RVA: 0x0000FD20 File Offset: 0x0000DF20
	public static PlatformUserIdentifierAbs FromCombinedString(string _combinedString, bool _logErrors = true)
	{
		if (_combinedString == null)
		{
			if (_logErrors)
			{
				Log.Error("Empty user identifier string\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		int num = _combinedString.IndexOf('_');
		if (num < 0)
		{
			if (_logErrors)
			{
				Log.Error("Missing separator '_' in string: " + _combinedString + "\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		if (num == 0)
		{
			if (_logErrors)
			{
				Log.Error("Missing platform (before the separator '_') in string: " + _combinedString + "\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		if (num + 1 >= _combinedString.Length)
		{
			if (_logErrors)
			{
				Log.Error("Missing user identifier (after the separator '_') in string: " + _combinedString + "\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		string platformName = _combinedString.Substring(0, num);
		string userId = _combinedString.Substring(num + 1, _combinedString.Length - num - 1);
		return PlatformUserIdentifierAbs.FromPlatformAndId(platformName, userId, _logErrors);
	}

	// Token: 0x060001C9 RID: 457 RVA: 0x0000FDE4 File Offset: 0x0000DFE4
	public static PlatformUserIdentifierAbs FromPlatformAndId(string _platformName, string _userId, bool _logErrors = true)
	{
		EPlatformIdentifier key;
		if (!PlatformManager.TryPlatformIdentifierFromString(_platformName, out key))
		{
			if (_logErrors)
			{
				Log.Error("Invalid platform name in user identifier: " + _platformName + "\nFrom: " + StackTraceUtility.ExtractStackTrace());
			}
			return null;
		}
		AbsUserIdentifierFactory absUserIdentifierFactory;
		if (PlatformManager.UserIdentifierFactories.TryGetValue(key, out absUserIdentifierFactory))
		{
			PlatformUserIdentifierAbs result = null;
			try
			{
				result = absUserIdentifierFactory.FromId(_userId);
			}
			catch (Exception)
			{
				if (_logErrors)
				{
					throw;
				}
			}
			return result;
		}
		if (_logErrors)
		{
			throw new ArgumentOutOfRangeException("platformIdentifier", "Invalid platform " + key.ToString());
		}
		return null;
	}

	// Token: 0x060001CA RID: 458
	public abstract bool Equals(PlatformUserIdentifierAbs _other);

	// Token: 0x060001CB RID: 459 RVA: 0x0000FE78 File Offset: 0x0000E078
	public override bool Equals(object _obj)
	{
		if (_obj == null)
		{
			return false;
		}
		if (this == _obj)
		{
			return true;
		}
		PlatformUserIdentifierAbs platformUserIdentifierAbs = _obj as PlatformUserIdentifierAbs;
		return platformUserIdentifierAbs != null && this.Equals(platformUserIdentifierAbs);
	}

	// Token: 0x060001CC RID: 460 RVA: 0x0000FEA3 File Offset: 0x0000E0A3
	public static bool Equals(PlatformUserIdentifierAbs _a, PlatformUserIdentifierAbs _b)
	{
		return _a == _b || (_a != null && _a.Equals(_b));
	}

	// Token: 0x060001CD RID: 461 RVA: 0x0000FEB7 File Offset: 0x0000E0B7
	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	// Token: 0x060001CE RID: 462 RVA: 0x0000FEBF File Offset: 0x0000E0BF
	public override string ToString()
	{
		return this.CombinedString;
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000A7E3 File Offset: 0x000089E3
	[PublicizedFrom(EAccessModifier.Protected)]
	public PlatformUserIdentifierAbs()
	{
	}

	// Token: 0x04000281 RID: 641
	public const byte UserIdentifierVersion = 1;
}

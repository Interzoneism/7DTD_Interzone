using System;
using System.IO;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000828 RID: 2088
[Preserve]
public class PlayerData
{
	// Token: 0x06003BFC RID: 15356 RVA: 0x00181687 File Offset: 0x0017F887
	public PlayerData(PlatformUserIdentifierAbs _primaryId, PlatformUserIdentifierAbs _nativeId, AuthoredText _playerName, EPlayGroup _playGroup)
	{
		this.PrimaryId = _primaryId;
		this.NativeId = _nativeId;
		this.PlayGroup = _playGroup;
		this.PlayerName = _playerName;
		this.PlatformData = PlatformUserManager.GetOrCreate(_primaryId);
		this.PlatformData.NativeId = _nativeId;
	}

	// Token: 0x06003BFD RID: 15357 RVA: 0x001816C4 File Offset: 0x0017F8C4
	public static PlayerData Read(BinaryReader _reader)
	{
		PlatformUserIdentifierAbs primaryId = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		PlatformUserIdentifierAbs nativeId = PlatformUserIdentifierAbs.FromStream(_reader, false, false);
		AuthoredText authoredText = AuthoredText.FromStream(_reader);
		EPlayGroup playGroup = (EPlayGroup)_reader.ReadByte();
		GeneratedTextManager.PrefilterText(authoredText, GeneratedTextManager.TextFilteringMode.Filter);
		return new PlayerData(primaryId, nativeId, authoredText, playGroup);
	}

	// Token: 0x06003BFE RID: 15358 RVA: 0x001816FF File Offset: 0x0017F8FF
	public void Write(BinaryWriter _writer)
	{
		this.PrimaryId.ToStream(_writer, false);
		this.NativeId.ToStream(_writer, false);
		AuthoredText.ToStream(this.PlayerName, _writer);
		_writer.Write((byte)this.PlayGroup);
	}

	// Token: 0x0400309E RID: 12446
	public readonly IPlatformUserData PlatformData;

	// Token: 0x0400309F RID: 12447
	public readonly PlatformUserIdentifierAbs PrimaryId;

	// Token: 0x040030A0 RID: 12448
	public readonly PlatformUserIdentifierAbs NativeId;

	// Token: 0x040030A1 RID: 12449
	public readonly EPlayGroup PlayGroup;

	// Token: 0x040030A2 RID: 12450
	public readonly AuthoredText PlayerName;
}

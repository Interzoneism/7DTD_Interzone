using System;

// Token: 0x02000940 RID: 2368
public static class SaveDataTypeExtensions
{
	// Token: 0x0600472F RID: 18223 RVA: 0x000C07F3 File Offset: 0x000BE9F3
	public static bool IsRoot(this SaveDataType saveDataType)
	{
		return saveDataType == SaveDataType.User;
	}

	// Token: 0x06004730 RID: 18224 RVA: 0x001C0A77 File Offset: 0x001BEC77
	public static int GetSlotPathDepth(this SaveDataType saveDataType)
	{
		switch (saveDataType)
		{
		case SaveDataType.User:
			return 0;
		case SaveDataType.Saves:
			return 2;
		case SaveDataType.SavesLocal:
			return 1;
		case SaveDataType.GeneratedWorlds:
			return 1;
		default:
			Log.Error(string.Format("{0}.{1} does not have a slot path length, defaulting to '0'.", "SaveDataType", saveDataType));
			return 0;
		}
	}

	// Token: 0x06004731 RID: 18225 RVA: 0x001C0AB4 File Offset: 0x001BECB4
	public static string GetPathRaw(this SaveDataType saveDataType)
	{
		string result;
		switch (saveDataType)
		{
		case SaveDataType.User:
			result = string.Empty;
			break;
		case SaveDataType.Saves:
			result = "Saves";
			break;
		case SaveDataType.SavesLocal:
			result = "SavesLocal";
			break;
		case SaveDataType.GeneratedWorlds:
			result = "GeneratedWorlds";
			break;
		default:
			throw new ArgumentOutOfRangeException("saveDataType", saveDataType, string.Format("No path specified for {0}.", saveDataType));
		}
		return result;
	}

	// Token: 0x06004732 RID: 18226 RVA: 0x001C0B1C File Offset: 0x001BED1C
	public static SaveDataManagedPath GetPath(this SaveDataType saveDataType)
	{
		SaveDataManagedPath result;
		switch (saveDataType)
		{
		case SaveDataType.User:
			result = SaveDataTypeExtensions.s_rootPathUser;
			break;
		case SaveDataType.Saves:
			result = SaveDataTypeExtensions.s_rootPathSaves;
			break;
		case SaveDataType.SavesLocal:
			result = SaveDataTypeExtensions.s_rootPathSavesLocal;
			break;
		case SaveDataType.GeneratedWorlds:
			result = SaveDataTypeExtensions.s_rootPathGeneratedWorlds;
			break;
		default:
			throw new ArgumentOutOfRangeException("saveDataType", saveDataType, string.Format("No relative path specified for {0}.", saveDataType));
		}
		return result;
	}

	// Token: 0x040036C7 RID: 14023
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly SaveDataManagedPath s_rootPathUser = new SaveDataManagedPath(SaveDataType.User.GetPathRaw());

	// Token: 0x040036C8 RID: 14024
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly SaveDataManagedPath s_rootPathSaves = new SaveDataManagedPath(SaveDataType.Saves.GetPathRaw());

	// Token: 0x040036C9 RID: 14025
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly SaveDataManagedPath s_rootPathSavesLocal = new SaveDataManagedPath(SaveDataType.SavesLocal.GetPathRaw());

	// Token: 0x040036CA RID: 14026
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly SaveDataManagedPath s_rootPathGeneratedWorlds = new SaveDataManagedPath(SaveDataType.GeneratedWorlds.GetPathRaw());
}

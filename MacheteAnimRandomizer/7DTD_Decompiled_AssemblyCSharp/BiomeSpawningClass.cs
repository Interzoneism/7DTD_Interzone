using System;

// Token: 0x02000976 RID: 2422
public class BiomeSpawningClass
{
	// Token: 0x06004938 RID: 18744 RVA: 0x001CF54D File Offset: 0x001CD74D
	public static void Cleanup()
	{
		BiomeSpawningClass.list.Clear();
	}

	// Token: 0x04003862 RID: 14434
	public static DictionarySave<string, BiomeSpawnEntityGroupList> list = new DictionarySave<string, BiomeSpawnEntityGroupList>();
}

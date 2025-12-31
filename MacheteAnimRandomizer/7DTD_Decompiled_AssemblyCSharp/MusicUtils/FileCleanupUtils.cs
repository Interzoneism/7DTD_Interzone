using System;
using System.Collections.Generic;

namespace MusicUtils
{
	// Token: 0x020016FE RID: 5886
	public static class FileCleanupUtils
	{
		// Token: 0x0600B1EC RID: 45548 RVA: 0x00454E9C File Offset: 0x0045309C
		public static void CleanUpAllWaveFiles()
		{
			for (int i = 0; i < FileCleanupUtils.paths.Count; i++)
			{
				FileCleanupUtils.CleanUpWaveFile(FileCleanupUtils.paths[i]);
			}
		}

		// Token: 0x0600B1ED RID: 45549 RVA: 0x00454ECE File Offset: 0x004530CE
		public static void CleanUpWaveFile(string file)
		{
			WaveCleanUp.Create().GetComponent<WaveCleanUp>().FilePath = file;
		}

		// Token: 0x04008B5F RID: 35679
		public static List<string> paths = new List<string>();
	}
}

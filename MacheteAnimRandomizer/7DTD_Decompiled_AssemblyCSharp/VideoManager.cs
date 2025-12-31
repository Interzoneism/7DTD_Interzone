using System;
using System.Collections.Generic;

// Token: 0x02000B44 RID: 2884
public class VideoManager
{
	// Token: 0x060059CC RID: 22988 RVA: 0x002422DE File Offset: 0x002404DE
	public static void Init()
	{
		VideoManager.initialized = true;
		VideoManager.videos = new Dictionary<string, VideoData>();
	}

	// Token: 0x060059CD RID: 22989 RVA: 0x002422F0 File Offset: 0x002404F0
	public static void AddVideo(VideoData data)
	{
		if (!VideoManager.initialized)
		{
			VideoManager.Init();
		}
		if (!VideoManager.videos.ContainsKey(data.name))
		{
			VideoManager.videos.Add(data.name, data);
			return;
		}
		VideoManager.videos[data.name] = data;
	}

	// Token: 0x060059CE RID: 22990 RVA: 0x00242340 File Offset: 0x00240540
	public static VideoData GetVideoData(string id)
	{
		VideoData result;
		if (VideoManager.videos.TryGetValue(id, out result))
		{
			return result;
		}
		return null;
	}

	// Token: 0x040044AE RID: 17582
	public static Dictionary<string, VideoData> videos;

	// Token: 0x040044AF RID: 17583
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool initialized;
}

using System;

namespace DynamicMusic
{
	// Token: 0x02001766 RID: 5990
	public interface IPlayable
	{
		// Token: 0x1700140D RID: 5133
		// (get) Token: 0x0600B3C8 RID: 46024
		// (set) Token: 0x0600B3C7 RID: 46023
		float Volume { get; set; }

		// Token: 0x1700140E RID: 5134
		// (get) Token: 0x0600B3C9 RID: 46025
		bool IsDone { get; }

		// Token: 0x1700140F RID: 5135
		// (get) Token: 0x0600B3CA RID: 46026
		bool IsPaused { get; }

		// Token: 0x17001410 RID: 5136
		// (get) Token: 0x0600B3CB RID: 46027
		bool IsPlaying { get; }

		// Token: 0x17001411 RID: 5137
		// (get) Token: 0x0600B3CC RID: 46028
		bool IsReady { get; }

		// Token: 0x0600B3CD RID: 46029
		void Init();

		// Token: 0x0600B3CE RID: 46030
		void Play();

		// Token: 0x0600B3CF RID: 46031
		void Pause();

		// Token: 0x0600B3D0 RID: 46032
		void UnPause();

		// Token: 0x0600B3D1 RID: 46033
		void Stop();
	}
}

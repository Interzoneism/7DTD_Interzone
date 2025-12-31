using System;
using System.Collections.Generic;
using MusicUtils.Enums;

namespace DynamicMusic.Legacy
{
	// Token: 0x0200177E RID: 6014
	public class StreamerMaster
	{
		// Token: 0x17001439 RID: 5177
		// (get) Token: 0x0600B446 RID: 46150 RVA: 0x0045ACF7 File Offset: 0x00458EF7
		public bool IsReplacementNecessary
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				return StreamerMaster.currentStreamer == null || (StreamerMaster.currentStreamer.HasReachedLastHyperbar && !StreamerMaster.currentStreamer.IsPlaying);
			}
		}

		// Token: 0x0600B447 RID: 46151 RVA: 0x0045AD1D File Offset: 0x00458F1D
		public static StreamerMaster Create()
		{
			return new StreamerMaster();
		}

		// Token: 0x0600B448 RID: 46152 RVA: 0x0045AD24 File Offset: 0x00458F24
		public static void Init(DynamicMusicManager _dynamicMusicManager)
		{
			_dynamicMusicManager.StreamerMaster = StreamerMaster.Create();
			_dynamicMusicManager.StreamerMaster.dynamicMusicManager = _dynamicMusicManager;
			StreamerMaster.Streamers = new EnumDictionary<ThreatLevelLegacyType, Queue<ThreatLevelStreamer>>();
		}

		// Token: 0x0600B449 RID: 46153 RVA: 0x0045AD48 File Offset: 0x00458F48
		public void Tick()
		{
			LayerReserve.Tick();
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.Tick();
			}
			if (this.IsReplacementNecessary)
			{
				Log.Out("Getting new currentStreamer!");
				this.ReplaceCurrentStreamer();
			}
			if (!this.dynamicMusicManager.IsInDeadWindow)
			{
				if (this.dynamicMusicManager.FrequencyManager.CanScheduleTrack && this.dynamicMusicManager.IsPlayAllowed)
				{
					this.Play();
					return;
				}
			}
			else
			{
				this.Stop();
			}
		}

		// Token: 0x0600B44A RID: 46154 RVA: 0x0045ADBC File Offset: 0x00458FBC
		public void Play()
		{
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.Play();
			}
		}

		// Token: 0x0600B44B RID: 46155 RVA: 0x0045ADCF File Offset: 0x00458FCF
		public void Pause()
		{
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.Pause();
			}
		}

		// Token: 0x0600B44C RID: 46156 RVA: 0x0045ADE2 File Offset: 0x00458FE2
		public void UnPause()
		{
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.UnPause();
			}
		}

		// Token: 0x0600B44D RID: 46157 RVA: 0x0045ADF5 File Offset: 0x00458FF5
		public void Stop()
		{
			if (StreamerMaster.currentStreamer != null && (StreamerMaster.currentStreamer.IsPlaying || StreamerMaster.currentStreamer.IsPaused))
			{
				StreamerMaster.currentStreamer.Stop();
				this.ReplaceCurrentStreamer();
			}
		}

		// Token: 0x0600B44E RID: 46158 RVA: 0x0045AE26 File Offset: 0x00459026
		public void Cleanup()
		{
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.Cleanup();
				StreamerMaster.currentStreamer = null;
			}
			if (StreamerMaster.Streamers != null)
			{
				StreamerMaster.Streamers.Clear();
				StreamerMaster.Streamers = null;
			}
		}

		// Token: 0x0600B44F RID: 46159 RVA: 0x0045AE58 File Offset: 0x00459058
		public void ReplaceCurrentStreamer()
		{
			if (StreamerMaster.currentStreamer != null)
			{
				StreamerMaster.currentStreamer.Cleanup();
			}
			ThreatLevelStreamer item = ThreatLevelStreamer.Create(ThreatLevelLegacyType.Exploration);
			Queue<ThreatLevelStreamer> queue;
			if (!StreamerMaster.Streamers.TryGetValue(ThreatLevelLegacyType.Exploration, out queue))
			{
				StreamerMaster.Streamers.Add(ThreatLevelLegacyType.Exploration, queue = new Queue<ThreatLevelStreamer>());
				queue.Enqueue(ThreatLevelStreamer.Create(ThreatLevelLegacyType.Exploration));
				StreamerMaster.currentStreamer = item;
				return;
			}
			if (queue.Count > 0)
			{
				StreamerMaster.currentStreamer = queue.Dequeue();
				queue.Enqueue(item);
				return;
			}
			StreamerMaster.currentStreamer = item;
			queue.Enqueue(ThreatLevelStreamer.Create(ThreatLevelLegacyType.Exploration));
		}

		// Token: 0x04008CBB RID: 36027
		[PublicizedFrom(EAccessModifier.Private)]
		public static EnumDictionary<ThreatLevelLegacyType, Queue<ThreatLevelStreamer>> Streamers;

		// Token: 0x04008CBC RID: 36028
		[PublicizedFrom(EAccessModifier.Private)]
		public DynamicMusicManager dynamicMusicManager;

		// Token: 0x04008CBD RID: 36029
		public static ThreatLevelStreamer currentStreamer;
	}
}

using System;
using System.Collections.Generic;
using DynamicMusic.Legacy.ObjectModel;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic.Legacy
{
	// Token: 0x0200177F RID: 6015
	public class ThreatLevelStreamer
	{
		// Token: 0x1700143A RID: 5178
		// (get) Token: 0x0600B451 RID: 46161 RVA: 0x0045AEE0 File Offset: 0x004590E0
		public bool InitFinished
		{
			[PublicizedFrom(EAccessModifier.Private)]
			get
			{
				using (Dictionary<LayerType, LayerStreamer>.ValueCollection.Enumerator enumerator = this.LayerStreamers.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.InitFinished)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x1700143B RID: 5179
		// (get) Token: 0x0600B452 RID: 46162 RVA: 0x0045AF40 File Offset: 0x00459140
		public bool HasReachedLastHyperbar
		{
			get
			{
				using (Dictionary<LayerType, LayerStreamer>.ValueCollection.Enumerator enumerator = this.LayerStreamers.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (!enumerator.Current.HasReachedLastHyperbar)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		// Token: 0x0600B453 RID: 46163 RVA: 0x0045AFA0 File Offset: 0x004591A0
		public static ThreatLevelStreamer Create(ThreatLevelLegacyType _tl)
		{
			return new ThreatLevelStreamer(_tl);
		}

		// Token: 0x0600B454 RID: 46164 RVA: 0x0045AFA8 File Offset: 0x004591A8
		public static ThreatLevelStreamer Create(ThreatLevelLegacyType _tl, ThreatLevel _groupTL, ThreatLevelConfig _config)
		{
			return new ThreatLevelStreamer(_tl, _groupTL, _config);
		}

		// Token: 0x0600B455 RID: 46165 RVA: 0x0045AFB4 File Offset: 0x004591B4
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreatLevelStreamer(ThreatLevelLegacyType _tl)
		{
			this.id = ThreatLevelStreamer.numCreated++;
			this.threatLevel = _tl;
			MusicGroup musicGroup = MusicGroup.AllGroups[0];
			Dictionary<LayerType, LayerConfig> dictionary = ConfigSet.AllConfigSets[musicGroup.ConfigIDs[0]][_tl];
			ThreatLevel threatLevel = musicGroup[_tl];
			this.LayerStreamers = new EnumDictionary<LayerType, LayerStreamer>();
			foreach (KeyValuePair<LayerType, LayerConfig> keyValuePair in dictionary)
			{
				this.LayerStreamers.Add(keyValuePair.Key, LayerStreamer.Create(threatLevel[keyValuePair.Key], keyValuePair.Value, this));
			}
		}

		// Token: 0x0600B456 RID: 46166 RVA: 0x0045B084 File Offset: 0x00459284
		[PublicizedFrom(EAccessModifier.Private)]
		public ThreatLevelStreamer(ThreatLevelLegacyType _tl, ThreatLevel _groupTL, ThreatLevelConfig _config)
		{
			this.id = ThreatLevelStreamer.numCreated++;
			this.LayerStreamers = new EnumDictionary<LayerType, LayerStreamer>(_config.Count);
			foreach (KeyValuePair<LayerType, LayerConfig> keyValuePair in _config)
			{
				this.LayerStreamers.Add(keyValuePair.Key, LayerStreamer.Create(_groupTL[keyValuePair.Key], keyValuePair.Value, this));
			}
		}

		// Token: 0x0600B457 RID: 46167 RVA: 0x0045B124 File Offset: 0x00459324
		public void Cleanup()
		{
			foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
			{
				layerStreamer.Cleanup();
			}
		}

		// Token: 0x0600B458 RID: 46168 RVA: 0x0045B17C File Offset: 0x0045937C
		public void Play()
		{
			if (this.InitFinished)
			{
				double time = AudioSettings.dspTime + 0.25;
				Log.Out(string.Format("Calling Play on {0}", this.id));
				foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
				{
					layerStreamer.Play(time);
				}
			}
		}

		// Token: 0x0600B459 RID: 46169 RVA: 0x0045B204 File Offset: 0x00459404
		public void Pause()
		{
			this.IsPaused = true;
			foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
			{
				layerStreamer.Pause();
			}
		}

		// Token: 0x0600B45A RID: 46170 RVA: 0x0045B260 File Offset: 0x00459460
		public void UnPause()
		{
			this.IsPaused = false;
			foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
			{
				layerStreamer.UnPause();
			}
		}

		// Token: 0x0600B45B RID: 46171 RVA: 0x0045B2BC File Offset: 0x004594BC
		public void Stop()
		{
			foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
			{
				layerStreamer.Stop();
			}
		}

		// Token: 0x0600B45C RID: 46172 RVA: 0x0045B314 File Offset: 0x00459514
		public void Tick()
		{
			if (!this.IsPlaying)
			{
				return;
			}
			foreach (LayerStreamer layerStreamer in this.LayerStreamers.Values)
			{
				layerStreamer.Tick();
			}
		}

		// Token: 0x1700143C RID: 5180
		// (get) Token: 0x0600B45D RID: 46173 RVA: 0x0045B374 File Offset: 0x00459574
		public bool IsPlaying
		{
			get
			{
				using (Dictionary<LayerType, LayerStreamer>.ValueCollection.Enumerator enumerator = this.LayerStreamers.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (enumerator.Current.IsPlaying)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		// Token: 0x04008CBE RID: 36030
		public ThreatLevelLegacyType threatLevel;

		// Token: 0x04008CBF RID: 36031
		public static int numCreated;

		// Token: 0x04008CC0 RID: 36032
		public readonly int id;

		// Token: 0x04008CC1 RID: 36033
		public bool IsPaused;

		// Token: 0x04008CC2 RID: 36034
		public Dictionary<LayerType, LayerStreamer> LayerStreamers;
	}
}

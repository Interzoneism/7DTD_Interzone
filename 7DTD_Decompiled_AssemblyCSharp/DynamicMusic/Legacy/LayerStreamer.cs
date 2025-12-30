using System;
using System.Collections.Generic;
using DynamicMusic.Legacy.ObjectModel;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic.Legacy
{
	// Token: 0x0200177D RID: 6013
	public class LayerStreamer
	{
		// Token: 0x17001437 RID: 5175
		// (get) Token: 0x0600B439 RID: 46137 RVA: 0x0045A9C5 File Offset: 0x00458BC5
		// (set) Token: 0x0600B43A RID: 46138 RVA: 0x0045A9CD File Offset: 0x00458BCD
		public bool HasReachedLastHyperbar { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001438 RID: 5176
		// (get) Token: 0x0600B43B RID: 46139 RVA: 0x0045A9D6 File Offset: 0x00458BD6
		public bool IsPlaying
		{
			get
			{
				return this.Src && this.Src.isPlaying;
			}
		}

		// Token: 0x0600B43C RID: 46140 RVA: 0x0045A9F2 File Offset: 0x00458BF2
		public static LayerStreamer Create(Layer _layer, LayerConfig _layerConfig, ThreatLevelStreamer _parent = null)
		{
			return new LayerStreamer(_layer, _layerConfig, _parent);
		}

		// Token: 0x0600B43D RID: 46141 RVA: 0x0045A9FC File Offset: 0x00458BFC
		[PublicizedFrom(EAccessModifier.Private)]
		public LayerStreamer(Layer _layer, LayerConfig _layerConfig, ThreatLevelStreamer _parent)
		{
			this.parentId = _parent.id;
			this.HasReachedLastHyperbar = false;
			this.LayerConfig = _layerConfig;
			this.instrumentID = _layer.GetInstrumentID();
			if (this.instrumentID.IsLoaded)
			{
				this.OnClipSetLoad();
				return;
			}
			this.instrumentID.OnLoadFinished += this.OnClipSetLoad;
		}

		// Token: 0x0600B43E RID: 46142 RVA: 0x0045AA60 File Offset: 0x00458C60
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnClipSetLoad()
		{
			this.Src = UnityEngine.Object.Instantiate<AudioSource>(Resources.Load<AudioSource>(this.instrumentID.SourceName));
			this.Src.volume = this.instrumentID.Volume;
			this.Src.clip = AudioClip.Create(this.instrumentID.Name, this.instrumentID.Frames, this.instrumentID.Channels, this.instrumentID.Frequency, true, new AudioClip.PCMReaderCallback(this.FillStream));
			this.InitFinished = true;
		}

		// Token: 0x0600B43F RID: 46143 RVA: 0x0045AAF0 File Offset: 0x00458CF0
		public void Cleanup()
		{
			if (this.Src)
			{
				UnityEngine.Object.Destroy(this.Src.gameObject);
			}
			this.instrumentID.OnLoadFinished -= this.OnClipSetLoad;
			this.instrumentID.Unload();
		}

		// Token: 0x0600B440 RID: 46144 RVA: 0x0045AB3C File Offset: 0x00458D3C
		public void Play(double _time)
		{
			this.Src.PlayScheduled(_time);
			this.Src.loop = true;
		}

		// Token: 0x0600B441 RID: 46145 RVA: 0x0045AB56 File Offset: 0x00458D56
		public void Pause()
		{
			if (this.Src && this.Src.isPlaying)
			{
				this.Src.Pause();
			}
		}

		// Token: 0x0600B442 RID: 46146 RVA: 0x0045AB7D File Offset: 0x00458D7D
		public void UnPause()
		{
			if (this.Src)
			{
				this.Src.UnPause();
			}
		}

		// Token: 0x0600B443 RID: 46147 RVA: 0x0045AB97 File Offset: 0x00458D97
		public void Stop()
		{
			if (this.Src && this.Src.isPlaying)
			{
				this.Src.Stop();
			}
		}

		// Token: 0x0600B444 RID: 46148 RVA: 0x0045ABC0 File Offset: 0x00458DC0
		public void Tick()
		{
			if (this.HasReachedLastHyperbar && this.Src && this.Src.loop)
			{
				this.Src.loop = false;
				Log.Out(string.Format("Set loop to false on {0}", this.instrumentID.Name));
			}
		}

		// Token: 0x0600B445 RID: 46149 RVA: 0x0045AC18 File Offset: 0x00458E18
		[PublicizedFrom(EAccessModifier.Private)]
		public void FillStream(float[] data)
		{
			if (!this.InFillStream)
			{
				this.InFillStream = true;
				int num = data.Length;
				for (int i = 0; i < num; i++)
				{
					if (this.cursor == 0)
					{
						Dictionary<byte, PlacementType> layerConfig = this.LayerConfig;
						int num2 = this.hyperbar;
						this.hyperbar = num2 + 1;
						PlacementType placementType;
						if (layerConfig.TryGetValue((byte)num2, out placementType))
						{
							this.currentClipData = this.instrumentID.ClipData[placementType];
							this.HasReachedLastHyperbar = (placementType == PlacementType.End);
						}
						else
						{
							this.currentClipData = null;
						}
					}
					data[i] = ((this.currentClipData != null) ? this.currentClipData[this.cursor] : 0f);
					this.cursor++;
					this.cursor %= this.instrumentID.Samples;
				}
				this.InFillStream = false;
				return;
			}
			Log.Warning("FillStream was called while it was still running.");
		}

		// Token: 0x04008CB1 RID: 36017
		public bool InitFinished;

		// Token: 0x04008CB3 RID: 36019
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly int parentId;

		// Token: 0x04008CB4 RID: 36020
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly InstrumentID instrumentID;

		// Token: 0x04008CB5 RID: 36021
		[PublicizedFrom(EAccessModifier.Private)]
		public LayerConfig LayerConfig;

		// Token: 0x04008CB6 RID: 36022
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource Src;

		// Token: 0x04008CB7 RID: 36023
		[PublicizedFrom(EAccessModifier.Private)]
		public int hyperbar;

		// Token: 0x04008CB8 RID: 36024
		[PublicizedFrom(EAccessModifier.Private)]
		public int cursor;

		// Token: 0x04008CB9 RID: 36025
		[PublicizedFrom(EAccessModifier.Private)]
		public float[] currentClipData;

		// Token: 0x04008CBA RID: 36026
		[PublicizedFrom(EAccessModifier.Private)]
		public bool InFillStream;
	}
}

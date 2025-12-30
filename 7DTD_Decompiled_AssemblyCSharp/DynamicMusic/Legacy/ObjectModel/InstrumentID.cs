using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UnityEngine;

namespace DynamicMusic.Legacy.ObjectModel
{
	// Token: 0x02001786 RID: 6022
	public class InstrumentID : Dictionary<PlacementType, string>
	{
		// Token: 0x1400010A RID: 266
		// (add) Token: 0x0600B489 RID: 46217 RVA: 0x0045BBA4 File Offset: 0x00459DA4
		// (remove) Token: 0x0600B48A RID: 46218 RVA: 0x0045BBDC File Offset: 0x00459DDC
		public event InstrumentID.LoadFinishedAction OnLoadFinished;

		// Token: 0x17001448 RID: 5192
		// (get) Token: 0x0600B48B RID: 46219 RVA: 0x0045BC11 File Offset: 0x00459E11
		// (set) Token: 0x0600B48C RID: 46220 RVA: 0x0045BC19 File Offset: 0x00459E19
		public bool IsLoaded { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600B48D RID: 46221 RVA: 0x0045BC22 File Offset: 0x00459E22
		public InstrumentID() : base(3)
		{
			this.IsLoaded = false;
			this.ClipData = new EnumDictionary<PlacementType, float[]>(3);
			this.Clips = new EnumDictionary<PlacementType, AudioClip>(3);
		}

		// Token: 0x0600B48E RID: 46222 RVA: 0x0045BC60 File Offset: 0x00459E60
		public void Load()
		{
			if (!this.IsLoaded)
			{
				if (this.thisEnumerator == null)
				{
					this.thisEnumerator = this.LoadClip();
				}
				this.thisEnumerator.MoveNext();
			}
		}

		// Token: 0x0600B48F RID: 46223 RVA: 0x0045BC8A File Offset: 0x00459E8A
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator<bool> LoadClip()
		{
			if (!this.IsLoaded)
			{
				foreach (KeyValuePair<PlacementType, string> kvp in this)
				{
					LoadManager.AssetRequestTask<AudioClip> requestTask = LoadManager.LoadAsset<AudioClip>(kvp.Value, null, null, false, false);
					while (!requestTask.IsDone)
					{
						yield return false;
					}
					AudioClip clip = requestTask.Asset;
					if (clip != null)
					{
						if (clip.loadState == AudioDataLoadState.Unloaded)
						{
							clip.LoadAudioData();
							while (clip.loadState != AudioDataLoadState.Loaded)
							{
								yield return false;
								if (clip.loadState == AudioDataLoadState.Failed)
								{
									Log.Warning(string.Format("clip load failed in {0}", this.Name));
									break;
								}
							}
						}
						if (!this.hasGrabbedClipProperties)
						{
							this.Frames = clip.samples;
							this.Channels = clip.channels;
							this.Frequency = clip.frequency;
							this.Samples = this.Frames * this.Channels;
							this.hasGrabbedClipProperties = true;
						}
						else if (this.Frames != clip.samples || this.Channels != clip.channels || this.Frequency != clip.frequency)
						{
							Log.Warning(string.Format("Inconsistent clip properties for clips in {0}", this.Name));
						}
						int samplesPerPass = 44100;
						int samplesGrabbed = 0;
						int samplesToGrab = Utils.FastMin(samplesPerPass, this.Samples - samplesGrabbed);
						float[] sampleData = MemoryPools.poolFloat.Alloc(this.Samples);
						float[] buffer = MemoryPools.poolFloat.Alloc(samplesPerPass);
						while (samplesGrabbed < this.Samples)
						{
							if (this.Samples - samplesGrabbed < samplesPerPass)
							{
								buffer = new float[this.Samples - samplesGrabbed];
							}
							clip.GetData(buffer, samplesGrabbed / 2);
							buffer.CopyTo(sampleData, samplesGrabbed);
							samplesGrabbed += samplesToGrab;
							yield return false;
						}
						MemoryPools.poolFloat.Free(buffer);
						this.ClipData.Add(kvp.Key, sampleData);
						yield return false;
						clip.UnloadAudioData();
						this.isClipLoaded = false;
						sampleData = null;
						buffer = null;
					}
					else
					{
						Log.Warning(string.Format("Loaded resource {0} could not be boxed as an AudioClip", kvp.Value));
					}
					requestTask = null;
					clip = null;
					kvp = default(KeyValuePair<PlacementType, string>);
				}
				Dictionary<PlacementType, string>.Enumerator enumerator = default(Dictionary<PlacementType, string>.Enumerator);
			}
			this.IsLoaded = true;
			InstrumentID.LoadFinishedAction onLoadFinished = this.OnLoadFinished;
			if (onLoadFinished != null)
			{
				onLoadFinished();
			}
			yield return true;
			yield break;
			yield break;
		}

		// Token: 0x0600B490 RID: 46224 RVA: 0x0045BC99 File Offset: 0x00459E99
		[PublicizedFrom(EAccessModifier.Private)]
		public void OnRequestFinished(AudioClip clip)
		{
			this.isClipLoaded = true;
		}

		// Token: 0x0600B491 RID: 46225 RVA: 0x0045BCA4 File Offset: 0x00459EA4
		public void Unload()
		{
			foreach (float[] array in this.ClipData.Values)
			{
				MemoryPools.poolFloat.Free(array);
			}
			this.ClipData.Clear();
			this.thisEnumerator = null;
			this.IsLoaded = (this.hasGrabbedClipProperties = false);
			this.Frames = (this.Samples = (this.Channels = 0));
			this.Frequency = 44100;
		}

		// Token: 0x04008CE4 RID: 36068
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator<bool> thisEnumerator;

		// Token: 0x04008CE6 RID: 36070
		public static string BundlePath;

		// Token: 0x04008CE7 RID: 36071
		public string Name;

		// Token: 0x04008CE8 RID: 36072
		public string SourceName;

		// Token: 0x04008CE9 RID: 36073
		public float Volume = 1f;

		// Token: 0x04008CEA RID: 36074
		public int Frames;

		// Token: 0x04008CEB RID: 36075
		public int Samples;

		// Token: 0x04008CEC RID: 36076
		public int Channels;

		// Token: 0x04008CED RID: 36077
		public int Frequency = 44100;

		// Token: 0x04008CEE RID: 36078
		[PublicizedFrom(EAccessModifier.Private)]
		public EnumDictionary<PlacementType, AudioClip> Clips;

		// Token: 0x04008CEF RID: 36079
		public EnumDictionary<PlacementType, float[]> ClipData;

		// Token: 0x04008CF1 RID: 36081
		[PublicizedFrom(EAccessModifier.Private)]
		public bool hasGrabbedClipProperties;

		// Token: 0x04008CF2 RID: 36082
		[PublicizedFrom(EAccessModifier.Private)]
		public bool isClipLoaded;

		// Token: 0x02001787 RID: 6023
		// (Invoke) Token: 0x0600B493 RID: 46227
		public delegate void LoadFinishedAction();
	}
}

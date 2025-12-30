using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	// Token: 0x02001794 RID: 6036
	public class XmlData
	{
		// Token: 0x0600B4D9 RID: 46297 RVA: 0x0045CEE0 File Offset: 0x0045B0E0
		public XmlData()
		{
			this.soundGroupName = "Invalid";
			this.maxVoices = 1;
			this.maxVoicesPerEntity = 5;
			this.audioClipMap = new List<ClipSourceMap>();
			this.noiseData = new NoiseData();
			this.localCrouchVolumeScale = 0.5f;
			this.crouchNoiseScale = 0.5f;
			this.noiseScale = 1f;
			this.maxRepeatRate = 0.1f;
			this.voicesPlaying = 0;
			this.lastRecordedPlayTime = Time.time;
			this.maxVolume = 1f;
			this.sequence = false;
			this.runningVolumeScale = 1f;
			this.lowestPitch = 1f;
			this.highestPitch = 1f;
			this.distantFadeStart = -1f;
			this.distantFadeEnd = -1f;
			this.channel = XmlData.Channel.Environment;
			this.priority = 99;
		}

		// Token: 0x0600B4DA RID: 46298 RVA: 0x0045CFCC File Offset: 0x0045B1CC
		public bool Update()
		{
			if (this.maxRepeatRate > 0f)
			{
				float time = Time.time;
				float num = time - this.lastRecordedPlayTime;
				if (num < this.maxRepeatRate)
				{
					return false;
				}
				this.voicesPlaying = Utils.FastClamp(this.voicesPlaying - (int)(num / this.maxRepeatRate), 0, 999);
				if (this.voicesPlaying >= this.maxVoices)
				{
					return false;
				}
				this.voicesPlaying++;
				this.lastRecordedPlayTime = time;
			}
			return true;
		}

		// Token: 0x0600B4DB RID: 46299 RVA: 0x0045D047 File Offset: 0x0045B247
		public List<ClipSourceMap> GetClipList()
		{
			if (Manager.Instance.bUseAltSounds && this.altAudioClipMap != null)
			{
				return this.altAudioClipMap;
			}
			if (this.hasProfanity && GamePrefs.GetBool(EnumGamePrefs.OptionsFilterProfanity))
			{
				return this.cleanClipMap;
			}
			return this.audioClipMap;
		}

		// Token: 0x0600B4DC RID: 46300 RVA: 0x0045D088 File Offset: 0x0045B288
		public ClipSourceMap GetRandomClip()
		{
			List<ClipSourceMap> clipList = this.GetClipList();
			int num = 0;
			int count = clipList.Count;
			if (count > 1)
			{
				if (count == 2)
				{
					num = (this.randomLastIndex ^ 1);
				}
				else
				{
					num = Manager.random.RandomRange(count - 1);
					if (num >= this.randomLastIndex)
					{
						num++;
					}
				}
				this.randomLastIndex = num;
			}
			else if (count == 0)
			{
				Log.Warning("No Clips in Audio ClipSourceMap " + this.soundGroupName + ", " + ((this.hasProfanity && GamePrefs.GetBool(EnumGamePrefs.OptionsFilterProfanity)) ? "using 'no profanity' map:" : ""));
				return null;
			}
			return clipList[num];
		}

		// Token: 0x0600B4DD RID: 46301 RVA: 0x0045D122 File Offset: 0x0045B322
		public void AddAltClipSourceMap(ClipSourceMap csm)
		{
			if (this.altAudioClipMap == null)
			{
				this.altAudioClipMap = new List<ClipSourceMap>();
			}
			this.altAudioClipMap.Add(csm);
		}

		// Token: 0x04008D80 RID: 36224
		public string soundGroupName;

		// Token: 0x04008D81 RID: 36225
		public int maxVoices;

		// Token: 0x04008D82 RID: 36226
		public List<ClipSourceMap> audioClipMap;

		// Token: 0x04008D83 RID: 36227
		public List<ClipSourceMap> altAudioClipMap;

		// Token: 0x04008D84 RID: 36228
		public List<ClipSourceMap> cleanClipMap;

		// Token: 0x04008D85 RID: 36229
		public NoiseData noiseData;

		// Token: 0x04008D86 RID: 36230
		public float localCrouchVolumeScale;

		// Token: 0x04008D87 RID: 36231
		public float runningVolumeScale;

		// Token: 0x04008D88 RID: 36232
		public float crouchNoiseScale;

		// Token: 0x04008D89 RID: 36233
		public float noiseScale;

		// Token: 0x04008D8A RID: 36234
		public float maxRepeatRate;

		// Token: 0x04008D8B RID: 36235
		public int voicesPlaying;

		// Token: 0x04008D8C RID: 36236
		public float lastRecordedPlayTime;

		// Token: 0x04008D8D RID: 36237
		public bool playImmediate;

		// Token: 0x04008D8E RID: 36238
		public bool sequence;

		// Token: 0x04008D8F RID: 36239
		public float maxVolume;

		// Token: 0x04008D90 RID: 36240
		public float lowestPitch;

		// Token: 0x04008D91 RID: 36241
		public float highestPitch;

		// Token: 0x04008D92 RID: 36242
		public float distantFadeStart;

		// Token: 0x04008D93 RID: 36243
		public float distantFadeEnd;

		// Token: 0x04008D94 RID: 36244
		public int maxVoicesPerEntity;

		// Token: 0x04008D95 RID: 36245
		public bool hasProfanity;

		// Token: 0x04008D96 RID: 36246
		public XmlData.Channel channel;

		// Token: 0x04008D97 RID: 36247
		public int priority;

		// Token: 0x04008D98 RID: 36248
		public bool vibratesController = true;

		// Token: 0x04008D99 RID: 36249
		public float vibrationStrengthMultiplier = 1f;

		// Token: 0x04008D9A RID: 36250
		[PublicizedFrom(EAccessModifier.Private)]
		public int randomLastIndex;

		// Token: 0x02001795 RID: 6037
		public enum Channel
		{
			// Token: 0x04008D9C RID: 36252
			Mouth,
			// Token: 0x04008D9D RID: 36253
			Environment
		}
	}
}

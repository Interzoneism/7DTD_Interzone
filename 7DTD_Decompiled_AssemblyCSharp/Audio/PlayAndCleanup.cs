using System;
using System.Collections;
using UnityEngine;

namespace Audio
{
	// Token: 0x0200179B RID: 6043
	public class PlayAndCleanup
	{
		// Token: 0x0600B4E1 RID: 46305 RVA: 0x0045D144 File Offset: 0x0045B344
		public PlayAndCleanup(LoopingPair _lp)
		{
			this.lp = _lp;
			double num;
			this.lp.sgoBegin.src.PlayScheduled(num = AudioSettings.dspTime + 0.05);
			this.lp.sgoLoop.src.PlayScheduled(num + (double)this.lp.sgoBegin.src.clip.samples / 44100.0);
			Manager.AddPlayingAudioSource(this.lp.sgoBegin.src);
			Manager.AddPlayingAudioSource(this.lp.sgoLoop.src);
			GameManager.Instance.StartCoroutine(this.StopBeginWhenDone(this.lp.sgoBegin.src.clip.length));
		}

		// Token: 0x0600B4E2 RID: 46306 RVA: 0x0045D215 File Offset: 0x0045B415
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopBeginWhenDone(float waitTime)
		{
			yield return new WaitForSeconds(waitTime + 0.1f);
			if (GameManager.Instance.IsPaused())
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.lp.sgoBegin.src == null)
			{
				yield break;
			}
			if (this.lp.sgoBegin.src.isPlaying)
			{
				yield return new WaitForSeconds(0.1f);
			}
			if (this.lp.sgoBegin.src == null)
			{
				yield break;
			}
			Manager.RemovePlayingAudioSource(this.lp.sgoBegin.src);
			if (this.lp.sgoBegin.go)
			{
				UnityEngine.Object.Destroy(this.lp.sgoBegin.go);
			}
			yield break;
		}

		// Token: 0x0600B4E3 RID: 46307 RVA: 0x0045D22C File Offset: 0x0045B42C
		public PlayAndCleanup(GameObject _go, AudioSource _source, float _occlusion = 0f, float delay = 0f, bool isLooping = false, bool hasLoopingAnalog = false)
		{
			this.go = _go;
			this.src = _source;
			float num = 1f - _occlusion;
			float num2 = Utils.FastAbs(Manager.currentListenerPosition.y - this.go.transform.position.y);
			num2 = Utils.FastClamp01(num2 / 30f);
			this.src.volume *= (1f - num2) * num;
			if (num < 0.95f)
			{
				this.go.AddComponent<AudioLowPassFilter>().cutoffFrequency = Utils.FastLerp(10f, 5000f, Mathf.Pow(num, 2f));
			}
			if (delay > 0f)
			{
				this.src.PlayDelayed(delay);
			}
			else
			{
				Manager.PlaySource(this.src);
			}
			Manager.AddPlayingAudioSource(this.src);
			if (!isLooping)
			{
				float waitTime = _source.clip.length * (1f + Utils.FastClamp01(1f - _source.pitch)) + delay;
				GameManager.Instance.StartCoroutine(this.StopWhenDone(waitTime));
			}
		}

		// Token: 0x0600B4E4 RID: 46308 RVA: 0x0045D340 File Offset: 0x0045B540
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator StopWhenDone(float waitTime)
		{
			yield return new WaitForSeconds(waitTime + 0.001f);
			if (GameManager.Instance.IsPaused())
			{
				yield return new WaitForSeconds(0.1f);
			}
			Manager.RemovePlayingAudioSource(this.src);
			if (this.go)
			{
				UnityEngine.Object.Destroy(this.go);
			}
			yield break;
		}

		// Token: 0x04008DB0 RID: 36272
		[PublicizedFrom(EAccessModifier.Private)]
		public GameObject go;

		// Token: 0x04008DB1 RID: 36273
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource src;

		// Token: 0x04008DB2 RID: 36274
		[PublicizedFrom(EAccessModifier.Private)]
		public LoopingPair lp;
	}
}

using System;
using System.Collections;
using System.Text;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001715 RID: 5909
	[Preserve]
	public abstract class Section : ContentPlayer, ISection, IPlayable, IFadeable, ICleanable
	{
		// Token: 0x170013C1 RID: 5057
		// (get) Token: 0x0600B23B RID: 45627 RVA: 0x004557F2 File Offset: 0x004539F2
		// (set) Token: 0x0600B23C RID: 45628 RVA: 0x004557FA File Offset: 0x004539FA
		public SectionType Sect { get; set; }

		// Token: 0x170013C2 RID: 5058
		// (get) Token: 0x0600B23D RID: 45629 RVA: 0x00455803 File Offset: 0x00453A03
		// (set) Token: 0x0600B23E RID: 45630 RVA: 0x0045580B File Offset: 0x00453A0B
		public bool IsInitialized { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

		// Token: 0x170013C3 RID: 5059
		// (get) Token: 0x0600B240 RID: 45632 RVA: 0x0045582F File Offset: 0x00453A2F
		// (set) Token: 0x0600B23F RID: 45631 RVA: 0x00455814 File Offset: 0x00453A14
		public override float Volume
		{
			get
			{
				if (!this.src)
				{
					return 0f;
				}
				return this.src.volume;
			}
			set
			{
				if (this.src)
				{
					this.src.volume = value;
				}
			}
		}

		// Token: 0x0600B241 RID: 45633 RVA: 0x0045584F File Offset: 0x00453A4F
		public override void Init()
		{
			this.coroutines = new EnumDictionary<MusicActionType, Coroutine>();
		}

		// Token: 0x0600B242 RID: 45634 RVA: 0x0045585C File Offset: 0x00453A5C
		public override void Play()
		{
			if (!this.coroutines.ContainsKey(MusicActionType.Play))
			{
				base.Play();
				this.coroutines.Add(MusicActionType.Play, GameManager.Instance.StartCoroutine(this.PlayCoroutine()));
				Log.Out(string.Format("Played {0}", this.Sect));
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Attempted to play {0}, while play was running", this.Sect));
			stringBuilder.AppendLine("Currently running coroutines: ");
			foreach (MusicActionType musicActionType in this.coroutines.Keys)
			{
				stringBuilder.AppendLine(musicActionType.ToString());
			}
			Log.Warning(stringBuilder.ToString());
		}

		// Token: 0x0600B243 RID: 45635 RVA: 0x00455948 File Offset: 0x00453B48
		public override void Pause()
		{
			base.Pause();
			AudioSource audioSource = this.src;
			if (audioSource != null)
			{
				audioSource.Pause();
			}
			Log.Out(string.Format("Paused {0}", this.Sect));
		}

		// Token: 0x0600B244 RID: 45636 RVA: 0x0045597B File Offset: 0x00453B7B
		public override void UnPause()
		{
			base.UnPause();
			AudioSource audioSource = this.src;
			if (audioSource != null)
			{
				audioSource.UnPause();
			}
			Log.Out(string.Format("Unpaused {0}", this.Sect));
		}

		// Token: 0x0600B245 RID: 45637 RVA: 0x004559AE File Offset: 0x00453BAE
		public override void Stop()
		{
			base.Stop();
			AudioSource audioSource = this.src;
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			Log.Out(string.Format("Stopped {0}", this.Sect));
		}

		// Token: 0x0600B246 RID: 45638 RVA: 0x004559E4 File Offset: 0x00453BE4
		public virtual void FadeIn()
		{
			Coroutine routine;
			if (this.coroutines.TryGetValue(MusicActionType.FadeOut, out routine))
			{
				GameManager.Instance.StopCoroutine(routine);
				this.coroutines.Remove(MusicActionType.FadeOut);
			}
			if (this.IsPaused || this.coroutines.ContainsKey(MusicActionType.Play))
			{
				this.UnPause();
			}
			else
			{
				this.Play();
			}
			Log.Out(string.Format("Fading in {0}", this.Sect));
			Coroutine routine2;
			if (this.coroutines.TryGetValue(MusicActionType.FadeIn, out routine2))
			{
				GameManager.Instance.StopCoroutine(routine2);
				this.coroutines.Remove(MusicActionType.FadeIn);
			}
			this.coroutines.Add(MusicActionType.FadeIn, GameManager.Instance.StartCoroutine(this.FadeInCoroutine()));
		}

		// Token: 0x0600B247 RID: 45639 RVA: 0x00455A9C File Offset: 0x00453C9C
		public virtual void FadeOut()
		{
			Coroutine routine;
			if (this.coroutines.TryGetValue(MusicActionType.FadeIn, out routine))
			{
				GameManager.Instance.StopCoroutine(routine);
				this.coroutines.Remove(MusicActionType.FadeIn);
			}
			Log.Out(string.Format("Fading out {0}", this.Sect));
			Coroutine routine2;
			if (this.coroutines.TryGetValue(MusicActionType.FadeOut, out routine2))
			{
				GameManager.Instance.StopCoroutine(routine2);
				this.coroutines.Remove(MusicActionType.FadeOut);
			}
			this.coroutines.Add(MusicActionType.FadeOut, GameManager.Instance.StartCoroutine(this.FadeOutCoroutine()));
		}

		// Token: 0x0600B248 RID: 45640 RVA: 0x00455B2F File Offset: 0x00453D2F
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IEnumerator FadeInCoroutine()
		{
			double dspTime = AudioSettings.dspTime;
			double endTime = AudioSettings.dspTime + 3.0;
			double num = endTime - AudioSettings.dspTime;
			double perc = 1.0 - num / 3.0;
			float startVol = this.Volume;
			while (perc <= 1.0)
			{
				this.Volume = Mathf.Lerp(startVol, 1f, (float)perc);
				num = endTime - AudioSettings.dspTime;
				perc = 1.0 - num / 3.0;
				yield return null;
			}
			this.Volume = 1f;
			this.coroutines.Remove(MusicActionType.FadeIn);
			Log.Out(string.Format("fadeInCo complete on {0}", this.Sect));
			yield break;
		}

		// Token: 0x0600B249 RID: 45641 RVA: 0x00455B3E File Offset: 0x00453D3E
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IEnumerator FadeOutCoroutine()
		{
			double dspTime = AudioSettings.dspTime;
			double endTime = AudioSettings.dspTime + 3.0;
			double num = endTime - AudioSettings.dspTime;
			double perc = 1.0 - num / 3.0;
			float startVol = this.Volume;
			while (perc <= 1.0)
			{
				this.Volume = Mathf.Lerp(startVol, 0f, (float)perc);
				num = endTime - AudioSettings.dspTime;
				perc = 1.0 - num / 3.0;
				yield return null;
			}
			this.Volume = 0f;
			this.Pause();
			double timerStart = AudioSettings.dspTime;
			yield return new WaitUntil(() => AudioSettings.dspTime - timerStart >= 60.0 || !this.IsPaused);
			if (this.IsPaused)
			{
				this.Stop();
			}
			else
			{
				Log.Out(string.Format("{0} was resumed. FadeOut coroutine has been exited.", this.Sect));
			}
			this.coroutines.Remove(MusicActionType.FadeOut);
			Log.Out(string.Format("fadeOutCo complete on {0}", this.Sect));
			yield break;
		}

		// Token: 0x0600B24A RID: 45642
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract IEnumerator PlayCoroutine();

		// Token: 0x0600B24B RID: 45643 RVA: 0x00455B4D File Offset: 0x00453D4D
		public virtual IEnumerator PreloadRoutine()
		{
			yield return null;
			yield break;
		}

		// Token: 0x0600B24C RID: 45644 RVA: 0x00455B58 File Offset: 0x00453D58
		public virtual void CleanUp()
		{
			AudioSource audioSource = this.src;
			if (audioSource != null)
			{
				audioSource.Stop();
			}
			foreach (Coroutine routine in this.coroutines.Values)
			{
				GameManager.Instance.StopCoroutine(routine);
			}
			if (this.LoadRoutine != null)
			{
				GameManager.Instance.StopCoroutine(this.LoadRoutine);
				this.LoadRoutine = null;
			}
			this.coroutines.Clear();
			this.coroutines = null;
			if (this.src)
			{
				UnityEngine.Object.Destroy(this.src.gameObject);
				this.src = null;
			}
			if (Section.parent != null)
			{
				Section.parent.transform.DetachChildren();
			}
		}

		// Token: 0x0600B24D RID: 45645 RVA: 0x00455C38 File Offset: 0x00453E38
		[PublicizedFrom(EAccessModifier.Protected)]
		public Section()
		{
		}

		// Token: 0x04008BC0 RID: 35776
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameObject parent = new GameObject("Music");

		// Token: 0x04008BC1 RID: 35777
		[PublicizedFrom(EAccessModifier.Protected)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();

		// Token: 0x04008BC3 RID: 35779
		[PublicizedFrom(EAccessModifier.Protected)]
		public AudioSource src;

		// Token: 0x04008BC4 RID: 35780
		[PublicizedFrom(EAccessModifier.Protected)]
		public EnumDictionary<MusicActionType, Coroutine> coroutines;

		// Token: 0x04008BC5 RID: 35781
		[PublicizedFrom(EAccessModifier.Protected)]
		public Coroutine LoadRoutine;

		// Token: 0x04008BC7 RID: 35783
		[PublicizedFrom(EAccessModifier.Private)]
		public const float fadeTime = 3f;
	}
}

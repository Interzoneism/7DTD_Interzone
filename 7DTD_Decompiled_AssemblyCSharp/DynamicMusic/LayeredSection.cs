using System;
using System.Collections;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001747 RID: 5959
	[Preserve]
	public abstract class LayeredSection<T> : Section, ISection, IPlayable, IFadeable, ICleanable where T : ILayerMixer, new()
	{
		// Token: 0x0600B36A RID: 45930 RVA: 0x00459432 File Offset: 0x00457632
		public LayeredSection()
		{
			this.Mixer = Activator.CreateInstance<T>();
		}

		// Token: 0x0600B36B RID: 45931 RVA: 0x00459448 File Offset: 0x00457648
		public override void Init()
		{
			base.Init();
			this.Reset();
			this.IsReady = (base.IsInitialized = true);
		}

		// Token: 0x0600B36C RID: 45932 RVA: 0x00459474 File Offset: 0x00457674
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Reset()
		{
			this.cursor = 0;
			this.IsReady = (this.IsDone = false);
			if (this.src)
			{
				this.src.loop = true;
			}
		}

		// Token: 0x0600B36D RID: 45933 RVA: 0x004594B1 File Offset: 0x004576B1
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator PlayCoroutine()
		{
			yield return this.LoadContentCoroutine();
			AudioSource src = this.src;
			if (src != null)
			{
				src.Play();
			}
			yield break;
		}

		// Token: 0x0600B36E RID: 45934 RVA: 0x004594C0 File Offset: 0x004576C0
		public override IEnumerator PreloadRoutine()
		{
			Log.Out("Preloading LayeredSection {0} : Type: {1}", new object[]
			{
				base.Sect.ToString(),
				base.GetType().ToString()
			});
			yield return this.LoadContentCoroutine();
			this.preloaded = true;
			yield break;
		}

		// Token: 0x0600B36F RID: 45935 RVA: 0x004594CF File Offset: 0x004576CF
		[PublicizedFrom(EAccessModifier.Protected)]
		public IEnumerator LoadContentCoroutine()
		{
			this.IsReady = false;
			this.Mixer.Sect = base.Sect;
			if (!this.preloaded)
			{
				yield return this.Mixer.Load();
			}
			else
			{
				this.preloaded = false;
				yield return null;
			}
			if (!this.src)
			{
				using (LayeredSection<T>.s_LoadContentMarker.Auto())
				{
					AudioSource component = DataLoader.LoadAsset<GameObject>(Content.SourcePathFor[base.Sect], false).GetComponent<AudioSource>();
					this.src = UnityEngine.Object.Instantiate<AudioSource>(component);
					this.src.transform.SetParent(Section.parent.transform);
					this.src.name = base.Sect.ToString();
					this.src.loop = true;
					this.src.priority = 0;
					this.src.clip = AudioClip.Create(base.Sect.ToString(), Content.SamplesFor[base.Sect], 2, 44100, true, new AudioClip.PCMReaderCallback(this.FillStream));
				}
			}
			this.IsReady = (base.IsInitialized = true);
			this.LoadRoutine = null;
			yield break;
		}

		// Token: 0x0600B370 RID: 45936 RVA: 0x004594E0 File Offset: 0x004576E0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void FillStream(float[] data)
		{
			using (LayeredSection<T>.s_FillStreamMarker.Auto())
			{
				for (int i = 0; i < data.Length; i++)
				{
					int num = i;
					int num2 = this.cursor;
					this.cursor = num2 + 1;
					data[num] = this.Mixer[num2];
				}
			}
		}

		// Token: 0x04008C5C RID: 35932
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_FillStreamMarker = new ProfilerMarker("DynamicMusic.LayeredSection.FillStream");

		// Token: 0x04008C5D RID: 35933
		[PublicizedFrom(EAccessModifier.Private)]
		public static readonly ProfilerMarker s_LoadContentMarker = new ProfilerMarker("DynamicMusic.LayeredSection.LoadContentCoroutine");

		// Token: 0x04008C5E RID: 35934
		[PublicizedFrom(EAccessModifier.Protected)]
		public T Mixer;

		// Token: 0x04008C5F RID: 35935
		public int cursor;

		// Token: 0x04008C60 RID: 35936
		[PublicizedFrom(EAccessModifier.Private)]
		public bool preloaded;
	}
}

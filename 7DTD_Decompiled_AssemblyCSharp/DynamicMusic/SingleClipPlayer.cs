using System;
using System.Collections;
using UniLinq;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x02001730 RID: 5936
	public abstract class SingleClipPlayer : Section, ISection, IPlayable, IFadeable, ICleanable
	{
		// Token: 0x0600B2DC RID: 45788 RVA: 0x00457585 File Offset: 0x00455785
		public override void Init()
		{
			base.Init();
			this.LoadRoutine = GameManager.Instance.StartCoroutine(this.InitializationCoroutine());
		}

		// Token: 0x0600B2DD RID: 45789 RVA: 0x004575A3 File Offset: 0x004557A3
		[PublicizedFrom(EAccessModifier.Private)]
		public SingleClip GetSingleClip()
		{
			return Content.AllContent.OfType<SingleClip>().First((SingleClip c) => c.Section == base.Sect);
		}

		// Token: 0x0600B2DE RID: 45790 RVA: 0x004575C0 File Offset: 0x004557C0
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual IEnumerator InitializationCoroutine()
		{
			this.IsReady = false;
			this.clip = this.GetSingleClip();
			if (this.clip == null)
			{
				Log.Warning("content could not be cast as an object of type 'SingleClip'");
			}
			else
			{
				yield return this.clip.Load();
				GameObject gameObject = DataLoader.LoadAsset<GameObject>(Content.SourcePathFor[base.Sect], false);
				this.src = gameObject.GetComponent<AudioSource>();
				this.src = UnityEngine.Object.Instantiate<AudioSource>(this.src);
				this.src.name = base.Sect.ToString();
				this.src.transform.SetParent(Section.parent.transform);
				this.src.clip = this.clip.Clip;
				this.IsReady = (base.IsInitialized = true);
			}
			this.LoadRoutine = null;
			yield break;
		}

		// Token: 0x0600B2DF RID: 45791 RVA: 0x004575CF File Offset: 0x004557CF
		[PublicizedFrom(EAccessModifier.Protected)]
		public SingleClipPlayer()
		{
		}

		// Token: 0x04008C12 RID: 35858
		[PublicizedFrom(EAccessModifier.Protected)]
		public SingleClip clip;
	}
}

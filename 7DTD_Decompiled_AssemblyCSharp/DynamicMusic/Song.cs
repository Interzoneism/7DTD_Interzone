using System;
using System.Collections;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x0200174B RID: 5963
	[Preserve]
	public class Song : SingleClipPlayer, ISection, IPlayable, IFadeable, ICleanable
	{
		// Token: 0x0600B384 RID: 45956 RVA: 0x00459854 File Offset: 0x00457A54
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator PlayCoroutine()
		{
			yield return new WaitUntil(() => this.IsReady);
			AudioSource src = this.src;
			if (src != null)
			{
				src.Play();
			}
			yield return new WaitUntil(() => !this.src.isPlaying && !this.IsPaused);
			if (this.IsPlaying)
			{
				this.Stop();
			}
			this.IsDone = true;
			this.coroutines.Remove(MusicActionType.Play);
			yield break;
		}
	}
}

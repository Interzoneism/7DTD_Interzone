using System;
using System.Collections;
using MusicUtils.Enums;
using UnityEngine;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001741 RID: 5953
	[Preserve]
	public class Bloodmoon : LayeredSection<BloodmoonLayerMixer>
	{
		// Token: 0x0600B34B RID: 45899 RVA: 0x00459150 File Offset: 0x00457350
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator PlayCoroutine()
		{
			yield return this.<>n__0();
			yield return new WaitUntil(() => !this.src.isPlaying && !this.IsPaused);
			this.Reset();
			this.Mixer.Unload();
			this.coroutines.Remove(MusicActionType.Play);
			yield break;
		}

		// Token: 0x0600B34C RID: 45900 RVA: 0x00459160 File Offset: 0x00457360
		[PublicizedFrom(EAccessModifier.Protected)]
		public override void FillStream(float[] data)
		{
			for (int i = 0; i < data.Length; i++)
			{
				int num = i;
				LayerMixer<BloodmoonConfiguration> mixer = this.Mixer;
				int cursor = this.cursor;
				this.cursor = cursor + 1;
				data[num] = mixer[cursor];
				this.cursor %= Content.SamplesFor[base.Sect] * 2;
			}
		}
	}
}

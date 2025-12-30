using System;
using UnityEngine.Playables;

namespace UnityEngine.Animations
{
	// Token: 0x0200003E RID: 62
	public static class AnimationPlayableBinding
	{
		// Token: 0x0600029B RID: 667 RVA: 0x000045EC File Offset: 0x000027EC
		public static PlayableBinding Create(string name, Object key)
		{
			return PlayableBinding.CreateInternal(name, key, typeof(Animator), new PlayableBinding.CreateOutputMethod(AnimationPlayableBinding.CreateAnimationOutput));
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000461C File Offset: 0x0000281C
		private static PlayableOutput CreateAnimationOutput(PlayableGraph graph, string name)
		{
			return AnimationPlayableOutput.Create(graph, name, null);
		}
	}
}

using System;
using System.Collections.Generic;
using MusicUtils.Enums;
using UniLinq;
using UnityEngine.Scripting;

namespace DynamicMusic
{
	// Token: 0x02001729 RID: 5929
	[Preserve]
	public class ContentQueue
	{
		// Token: 0x170013D9 RID: 5081
		// (get) Token: 0x0600B2B8 RID: 45752 RVA: 0x000197A5 File Offset: 0x000179A5
		[Preserve]
		public bool IsReady
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600B2B9 RID: 45753 RVA: 0x004571B0 File Offset: 0x004553B0
		public ContentQueue(SectionType _section, LayerType _layer)
		{
			this.section = _section;
			this.layer = _layer;
			this.count = (from e in Content.AllContent.OfType<LayeredContent>()
			where e.Section == this.section && e.Layer == this.layer
			select e).Count<LayeredContent>();
		}

		// Token: 0x0600B2BA RID: 45754 RVA: 0x00457204 File Offset: 0x00455404
		public LayeredContent Next()
		{
			if (this.queue.Count < this.count / 2)
			{
				(from e in Content.AllContent.OfType<LayeredContent>()
				where e.Section == this.section && e.Layer == this.layer && !this.queue.Contains(e)
				orderby ContentQueue.rng.RandomInt
				select e).ToList<LayeredContent>().ForEach(new Action<LayeredContent>(this.queue.Enqueue));
			}
			return this.queue.Dequeue();
		}

		// Token: 0x0600B2BB RID: 45755 RVA: 0x0045728B File Offset: 0x0045548B
		public void Clear()
		{
			this.queue.Clear();
		}

		// Token: 0x04008BFE RID: 35838
		[PublicizedFrom(EAccessModifier.Private)]
		public static GameRandom rng = GameRandomManager.Instance.CreateGameRandom();

		// Token: 0x04008BFF RID: 35839
		[PublicizedFrom(EAccessModifier.Private)]
		public SectionType section;

		// Token: 0x04008C00 RID: 35840
		[PublicizedFrom(EAccessModifier.Private)]
		public LayerType layer;

		// Token: 0x04008C01 RID: 35841
		[PublicizedFrom(EAccessModifier.Private)]
		public int count;

		// Token: 0x04008C02 RID: 35842
		[PublicizedFrom(EAccessModifier.Private)]
		public Queue<LayeredContent> queue = new Queue<LayeredContent>();
	}
}

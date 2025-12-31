using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DynamicMusic
{
	// Token: 0x0200172E RID: 5934
	public class ContentLoader
	{
		// Token: 0x170013DD RID: 5085
		// (get) Token: 0x0600B2D0 RID: 45776 RVA: 0x00457455 File Offset: 0x00455655
		public static ContentLoader Instance
		{
			get
			{
				if (ContentLoader.instance == null)
				{
					ContentLoader.instance = new ContentLoader();
				}
				return ContentLoader.instance;
			}
		}

		// Token: 0x0600B2D1 RID: 45777 RVA: 0x0045746D File Offset: 0x0045566D
		public void Start()
		{
			this.LoadQueue = new Queue<IEnumerator>();
			this.Loader = GameManager.Instance.StartCoroutine(this.Load());
		}

		// Token: 0x0600B2D2 RID: 45778 RVA: 0x00457490 File Offset: 0x00455690
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator Load()
		{
			for (;;)
			{
				yield return new WaitUntil(() => this.LoadQueue.Count > 0);
				IEnumerator enumerator = this.LoadQueue.Dequeue();
				yield return enumerator;
			}
			yield break;
		}

		// Token: 0x0600B2D3 RID: 45779 RVA: 0x0045749F File Offset: 0x0045569F
		public void Cleanup()
		{
			if (this.Loader != null)
			{
				GameManager.Instance.StopCoroutine(this.Loader);
			}
			if (this.LoadQueue != null)
			{
				this.LoadQueue.Clear();
			}
			this.Loader = null;
			this.LoadQueue = null;
		}

		// Token: 0x04008C0C RID: 35852
		public static ContentLoader instance;

		// Token: 0x04008C0D RID: 35853
		[PublicizedFrom(EAccessModifier.Private)]
		public Coroutine Loader;

		// Token: 0x04008C0E RID: 35854
		public Queue<IEnumerator> LoadQueue;
	}
}

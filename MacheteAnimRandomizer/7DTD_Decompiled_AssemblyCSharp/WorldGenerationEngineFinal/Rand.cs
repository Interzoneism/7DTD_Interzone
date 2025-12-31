using System;
using UnityEngine;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001455 RID: 5205
	public class Rand
	{
		// Token: 0x17001184 RID: 4484
		// (get) Token: 0x0600A148 RID: 41288 RVA: 0x003FE7E4 File Offset: 0x003FC9E4
		public static Rand Instance
		{
			get
			{
				if (Rand.instance == null)
				{
					Rand.instance = new Rand();
				}
				return Rand.instance;
			}
		}

		// Token: 0x0600A149 RID: 41289 RVA: 0x003FE7FC File Offset: 0x003FC9FC
		public Rand()
		{
			this.gameRandom = GameRandomManager.Instance.CreateGameRandom();
		}

		// Token: 0x0600A14A RID: 41290 RVA: 0x003FE814 File Offset: 0x003FCA14
		public Rand(int seed)
		{
			this.gameRandom = GameRandomManager.Instance.CreateGameRandom();
			this.SetSeed(seed);
		}

		// Token: 0x0600A14B RID: 41291 RVA: 0x003FE833 File Offset: 0x003FCA33
		public void Cleanup()
		{
			GameRandomManager.Instance.FreeGameRandom(this.gameRandom);
			Rand.instance = null;
		}

		// Token: 0x0600A14C RID: 41292 RVA: 0x003FE84B File Offset: 0x003FCA4B
		public void Free()
		{
			GameRandomManager.Instance.FreeGameRandom(this.gameRandom);
		}

		// Token: 0x0600A14D RID: 41293 RVA: 0x003FE85D File Offset: 0x003FCA5D
		public void SetSeed(int seed)
		{
			this.gameRandom.SetSeed(seed);
		}

		// Token: 0x0600A14E RID: 41294 RVA: 0x003FE86B File Offset: 0x003FCA6B
		public float Float()
		{
			return this.gameRandom.RandomFloat;
		}

		// Token: 0x0600A14F RID: 41295 RVA: 0x003FE878 File Offset: 0x003FCA78
		public int Range(int min, int max)
		{
			return this.gameRandom.RandomRange(min, max);
		}

		// Token: 0x0600A150 RID: 41296 RVA: 0x003FE887 File Offset: 0x003FCA87
		public int Range(int max)
		{
			return this.gameRandom.RandomRange(max);
		}

		// Token: 0x0600A151 RID: 41297 RVA: 0x003FE895 File Offset: 0x003FCA95
		public float Range(float min, float max)
		{
			return this.gameRandom.RandomRange(min, max);
		}

		// Token: 0x0600A152 RID: 41298 RVA: 0x003FE8A4 File Offset: 0x003FCAA4
		public int Angle()
		{
			return this.gameRandom.RandomRange(360);
		}

		// Token: 0x0600A153 RID: 41299 RVA: 0x003FE8B6 File Offset: 0x003FCAB6
		public Vector2 RandomOnUnitCircle()
		{
			return this.gameRandom.RandomOnUnitCircle;
		}

		// Token: 0x0600A154 RID: 41300 RVA: 0x003FE8C3 File Offset: 0x003FCAC3
		public int PeekSample()
		{
			return this.gameRandom.PeekSample();
		}

		// Token: 0x04007C48 RID: 31816
		[PublicizedFrom(EAccessModifier.Private)]
		public static Rand instance;

		// Token: 0x04007C49 RID: 31817
		public GameRandom gameRandom;
	}
}

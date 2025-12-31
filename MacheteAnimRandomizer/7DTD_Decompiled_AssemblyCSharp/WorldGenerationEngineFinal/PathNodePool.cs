using System;
using System.Collections.Generic;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001446 RID: 5190
	public class PathNodePool
	{
		// Token: 0x0600A107 RID: 41223 RVA: 0x003FD16D File Offset: 0x003FB36D
		public PathNodePool(int _initialSize)
		{
			this.pool = new List<PathNode>(_initialSize);
		}

		// Token: 0x0600A108 RID: 41224 RVA: 0x003FD184 File Offset: 0x003FB384
		public PathNode Alloc()
		{
			PathNode pathNode;
			if (this.used >= this.pool.Count)
			{
				pathNode = new PathNode();
				this.pool.Add(pathNode);
			}
			else
			{
				pathNode = this.pool[this.used];
			}
			this.used++;
			return pathNode;
		}

		// Token: 0x0600A109 RID: 41225 RVA: 0x003FD1DC File Offset: 0x003FB3DC
		public void ReturnAll()
		{
			for (int i = 0; i < this.used; i++)
			{
				this.pool[i].Reset();
			}
			this.used = 0;
		}

		// Token: 0x0600A10A RID: 41226 RVA: 0x003FD212 File Offset: 0x003FB412
		public void Cleanup()
		{
			this.ReturnAll();
			this.pool.Clear();
			this.pool.Capacity = 16;
		}

		// Token: 0x0600A10B RID: 41227 RVA: 0x003FD232 File Offset: 0x003FB432
		public void LogStats()
		{
			Log.Out(string.Format("PathNodePool: Capacity={0}, Allocated={1}, InUse={2}", this.pool.Capacity, this.pool.Count, this.used));
		}

		// Token: 0x04007C00 RID: 31744
		[PublicizedFrom(EAccessModifier.Private)]
		public List<PathNode> pool;

		// Token: 0x04007C01 RID: 31745
		[PublicizedFrom(EAccessModifier.Private)]
		public int used;
	}
}

using System;

namespace WorldGenerationEngineFinal
{
	// Token: 0x02001426 RID: 5158
	public class DataMap<T>
	{
		// Token: 0x0600A057 RID: 41047 RVA: 0x003F6CD0 File Offset: 0x003F4ED0
		public DataMap(int tileWidth, T defaultValue)
		{
			this.data = new T[tileWidth, tileWidth];
			for (int i = 0; i < this.data.GetLength(0); i++)
			{
				for (int j = 0; j < this.data.GetLength(1); j++)
				{
					this.data[i, j] = defaultValue;
				}
			}
		}

		// Token: 0x04007B31 RID: 31537
		public T[,] data;
	}
}

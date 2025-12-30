using System;

// Token: 0x02000ADC RID: 2780
public class WorldBlockFiller
{
	// Token: 0x0600559C RID: 21916 RVA: 0x0022EC80 File Offset: 0x0022CE80
	public WorldBlockFiller(int iBiomeColorId, WorldBiomeProviderFromImage _biomeProvider, GameRandom _rand, WorldBiomes _rules)
	{
		this.m_RandomGenerator = _rand;
		this.m_iChunkDimension = 65536;
		this.m_BlocksToFill = new byte[this.m_iChunkDimension];
		this.m_iThisBiomeColorId = iBiomeColorId;
		this.m_GenRules = _rules;
	}

	// Token: 0x0600559D RID: 21917 RVA: 0x0022ECBC File Offset: 0x0022CEBC
	public void resetBlockInformation()
	{
		for (int i = 0; i < this.m_iChunkDimension; i++)
		{
			this.m_BlocksToFill[i] = byte.MaxValue;
		}
		this.m_iMaxX = 0;
		this.m_iMinX = 16;
		this.m_iMaxY = 0;
		this.m_iMinY = 256;
		this.m_iMaxZ = 0;
		this.m_iMinZ = 16;
		this.m_iFillCount = 0;
		this.m_iAreaCount = 0;
	}

	// Token: 0x0600559E RID: 21918 RVA: 0x0022ED28 File Offset: 0x0022CF28
	public void setBlockToFill(int x, int y, int z, byte top)
	{
		this.m_iMaxX = ((x > this.m_iMaxX) ? x : this.m_iMaxX);
		this.m_iMaxY = ((x > this.m_iMaxY) ? x : this.m_iMaxY);
		this.m_iMaxZ = ((x > this.m_iMaxZ) ? x : this.m_iMaxZ);
		this.m_iMinX = ((x < this.m_iMinX) ? x : this.m_iMinX);
		this.m_iMinY = ((x < this.m_iMinY) ? x : this.m_iMinY);
		this.m_iMinZ = ((x < this.m_iMinZ) ? x : this.m_iMinZ);
		this.setBlockArrayValue(x, y, z, top);
		this.m_iFillCount++;
		if (y == 0)
		{
			this.m_iAreaCount++;
		}
	}

	// Token: 0x0600559F RID: 21919 RVA: 0x0022EDF0 File Offset: 0x0022CFF0
	public void fillChunk(Chunk c)
	{
		if (this.m_iAreaCount == 0)
		{
			return;
		}
		BiomeDefinition biomeDefinition = null;
		this.m_GenRules.GetBiomeMap().TryGetValue((uint)this.m_iThisBiomeColorId, out biomeDefinition);
		if (biomeDefinition != null)
		{
			int iAreaCount = this.m_iAreaCount;
			int iLayerDepth = -1;
			for (int i = 0; i < biomeDefinition.m_DecoBlocks.Count; i++)
			{
				BiomeBlockDecoration bb = biomeDefinition.m_DecoBlocks[i];
				this.fillLevel(c, bb, iLayerDepth, ref iAreaCount);
			}
			iAreaCount = this.m_iAreaCount;
			for (int j = 0; j < biomeDefinition.m_Layers.Count; j++)
			{
				iLayerDepth = biomeDefinition.m_Layers[j].m_Depth;
			}
		}
	}

	// Token: 0x060055A0 RID: 21920 RVA: 0x0022EE94 File Offset: 0x0022D094
	[PublicizedFrom(EAccessModifier.Private)]
	public void fillLevel(Chunk c, BiomeBlockDecoration bb, int iLayerDepth, ref int iAvailableCount)
	{
		double num = (double)bb.prob;
		double probability = (double)bb.clusterProb;
		int num2 = (int)((double)this.m_iAreaCount * num);
		int num3 = this.m_RandomGenerator.RandomRange(this.m_iMinX, this.m_iMaxX + 1);
		int num4 = this.m_RandomGenerator.RandomRange(this.m_iMinZ, this.m_iMaxZ + 1);
		byte blockArrayValue = this.getBlockArrayValue(num3, 0, num4);
		if (blockArrayValue == 255)
		{
			return;
		}
		while (iAvailableCount >= 0 && num2 >= 0)
		{
			bool flag = false;
			if (this.getBlockArrayValue(num3, (int)(blockArrayValue + 1), num4) == 255)
			{
				int num5 = 0;
				while (!flag)
				{
					if (num5 >= 9)
					{
						break;
					}
					int num6 = Math.Max(0, num3 - num5);
					while (!flag && num6 < Math.Min(16, num3 + num5))
					{
						int num7 = Math.Max(0, num4 - num5);
						while (!flag && num7 < Math.Min(16, num4 + num5))
						{
							blockArrayValue = this.getBlockArrayValue(num6, 0, num7);
							if (blockArrayValue != 255)
							{
								flag = true;
								num3 = num6;
								num4 = num7;
							}
							num7++;
						}
						num6++;
					}
					num5++;
				}
			}
			else
			{
				flag = true;
			}
			if (!flag)
			{
				Log.Error("did not find spot to place decoration");
				return;
			}
			int num8 = this.setDecorationBlock(c, num3, (int)blockArrayValue, iLayerDepth, num4, probability, bb.blockValues[0]);
			iAvailableCount -= num8;
			num2 -= num8;
			do
			{
				num3 = this.m_RandomGenerator.RandomRange(this.m_iMinX, this.m_iMaxX + 1);
				num4 = this.m_RandomGenerator.RandomRange(this.m_iMinZ, this.m_iMaxZ + 1);
				blockArrayValue = this.getBlockArrayValue(num3, 0, num4);
			}
			while (blockArrayValue == 255);
		}
	}

	// Token: 0x060055A1 RID: 21921 RVA: 0x0022F040 File Offset: 0x0022D240
	[PublicizedFrom(EAccessModifier.Private)]
	public int setDecorationBlock(Chunk c, int x, int y, int d, int z, double probability, BlockValue blockValue)
	{
		int num = 1;
		int num2 = (d >= 0) ? this.m_RandomGenerator.RandomRange(0, d) : d;
		if (num2 >= y)
		{
			return 0;
		}
		if (probability > 0.0)
		{
			for (int i = -1; i <= 1; i++)
			{
				for (int j = -1; j <= 1; j++)
				{
					int num3 = (d > 1) ? -1 : 0;
					while (num3 <= ((d > 1) ? 1 : 0) && y + num3 - num2 > 0 && y + num3 - num2 < y)
					{
						if (i + x >= 0 && i + x < 16 && j + z >= 0 && j + z < 16 && this.m_RandomGenerator.RandomDouble < probability)
						{
							c.SetBlockRaw(i + x, y + num3 - num2, j + z, blockValue);
							this.setBlockArrayValue(i + x, y + num3 - num2, j + z, byte.MaxValue);
							num++;
						}
						num3++;
					}
				}
			}
		}
		c.SetBlockRaw(x, y - num2, z, blockValue);
		this.setBlockArrayValue(x, y - num2, z, byte.MaxValue);
		return num;
	}

	// Token: 0x060055A2 RID: 21922 RVA: 0x0022F154 File Offset: 0x0022D354
	[PublicizedFrom(EAccessModifier.Private)]
	public byte getBlockArrayValue(int x, int y, int z)
	{
		return this.m_BlocksToFill[((x << 4) + z << 8) + y];
	}

	// Token: 0x060055A3 RID: 21923 RVA: 0x0022F166 File Offset: 0x0022D366
	[PublicizedFrom(EAccessModifier.Private)]
	public void setBlockArrayValue(int x, int y, int z, byte value)
	{
		this.m_BlocksToFill[((x << 4) + z << 8) + y] = value;
	}

	// Token: 0x04004228 RID: 16936
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMaxX;

	// Token: 0x04004229 RID: 16937
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMinX;

	// Token: 0x0400422A RID: 16938
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMaxY;

	// Token: 0x0400422B RID: 16939
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMinY;

	// Token: 0x0400422C RID: 16940
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMaxZ;

	// Token: 0x0400422D RID: 16941
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iMinZ;

	// Token: 0x0400422E RID: 16942
	[PublicizedFrom(EAccessModifier.Private)]
	public byte[] m_BlocksToFill;

	// Token: 0x0400422F RID: 16943
	[PublicizedFrom(EAccessModifier.Private)]
	public GameRandom m_RandomGenerator;

	// Token: 0x04004230 RID: 16944
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iChunkDimension;

	// Token: 0x04004231 RID: 16945
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iFillCount;

	// Token: 0x04004232 RID: 16946
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iAreaCount;

	// Token: 0x04004233 RID: 16947
	[PublicizedFrom(EAccessModifier.Private)]
	public int m_iThisBiomeColorId;

	// Token: 0x04004234 RID: 16948
	[PublicizedFrom(EAccessModifier.Private)]
	public WorldBiomes m_GenRules;
}

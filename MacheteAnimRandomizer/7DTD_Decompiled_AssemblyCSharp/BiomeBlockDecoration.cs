using System;

// Token: 0x02000A87 RID: 2695
public class BiomeBlockDecoration
{
	// Token: 0x06005348 RID: 21320 RVA: 0x00215848 File Offset: 0x00213A48
	public BiomeBlockDecoration(string _name, float _prob, float _clusprob, bool _instantiateReferences, int _randomRotateMax, int _checkResource = 2147483647)
	{
		string[] array = _name.Split(',', StringSplitOptions.None);
		if (_instantiateReferences)
		{
			this.blockValues = new BlockValue[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				BlockValue blockValueForName = WorldBiomes.GetBlockValueForName(array[i]);
				if (_randomRotateMax > 3 && !blockValueForName.isair)
				{
					Block block = blockValueForName.Block;
					if (block.isMultiBlock && (block.multiBlockPos.dim.x > 1 || block.multiBlockPos.dim.z > 1))
					{
						Log.Error("Parsing biomes. Block with name '" + array[i] + "' supports only rotations 0-3, setting it to 3");
						_randomRotateMax = 3;
					}
				}
				this.blockValues[i] = blockValueForName;
			}
		}
		this.prob = _prob;
		this.clusterProb = _clusprob;
		this.randomRotateMax = _randomRotateMax;
		this.checkResourceOffsetY = _checkResource;
	}

	// Token: 0x06005349 RID: 21321 RVA: 0x0021591C File Offset: 0x00213B1C
	public static byte GetRandomRotation(float _rnd, int _randomRotateMax)
	{
		byte b = (byte)(_rnd * (float)_randomRotateMax + 0.5f);
		if (b >= 4 && b <= 7)
		{
			b = b - 4 + 24;
		}
		return b;
	}

	// Token: 0x04003F74 RID: 16244
	public BlockValue[] blockValues;

	// Token: 0x04003F75 RID: 16245
	public float prob;

	// Token: 0x04003F76 RID: 16246
	public float clusterProb;

	// Token: 0x04003F77 RID: 16247
	public int randomRotateMax;

	// Token: 0x04003F78 RID: 16248
	public int checkResourceOffsetY;
}

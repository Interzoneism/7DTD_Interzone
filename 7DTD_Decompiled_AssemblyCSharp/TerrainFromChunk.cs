using System;

// Token: 0x02000AD2 RID: 2770
[PublicizedFrom(EAccessModifier.Internal)]
public class TerrainFromChunk : TerrainGeneratorWithBiomeResource
{
	// Token: 0x06005541 RID: 21825 RVA: 0x0022C77C File Offset: 0x0022A97C
	public void Init(RegionFileManager _regionFileManager, IBiomeProvider _biomeProvider, int _seed)
	{
		base.Init(null, _biomeProvider, _seed);
		this.regionFileManager = _regionFileManager;
	}

	// Token: 0x06005542 RID: 21826 RVA: 0x0022C790 File Offset: 0x0022A990
	[PublicizedFrom(EAccessModifier.Private)]
	public void checkTerrainChunk(int _x, int _z)
	{
		int num = World.toChunkXZ(_x);
		int num2 = World.toChunkXZ(_z);
		if (this.terrainChunk == null || this.terrainChunk.X != num || this.terrainChunk.Z != num2)
		{
			this.terrainChunk = this.regionFileManager.GetChunkSync(WorldChunkCache.MakeChunkKey(num, num2));
		}
	}

	// Token: 0x06005543 RID: 21827 RVA: 0x0022C7E7 File Offset: 0x0022A9E7
	public override byte GetTerrainHeightByteAt(int _x, int _z)
	{
		this.checkTerrainChunk(_x, _z);
		if (this.terrainChunk == null)
		{
			return 0;
		}
		return this.terrainChunk.GetHeight(World.toBlockXZ(_x), World.toBlockXZ(_z));
	}

	// Token: 0x06005544 RID: 21828 RVA: 0x0022C812 File Offset: 0x0022AA12
	public override sbyte GetDensityAt(int _xWorld, int _yWorld, int _zWorld, BiomeDefinition _bd, float _biomeIntensity)
	{
		this.checkTerrainChunk(_xWorld, _zWorld);
		if (this.terrainChunk == null)
		{
			return MarchingCubes.DensityAir;
		}
		return this.terrainChunk.GetDensity(_xWorld, _yWorld, _zWorld);
	}

	// Token: 0x06005545 RID: 21829 RVA: 0x0022C838 File Offset: 0x0022AA38
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool isTerrainAt(int _x, int _y, int _z)
	{
		return !this.terrainChunk.GetBlockNoDamage(_x, _y, _z).isair;
	}

	// Token: 0x06005546 RID: 21830 RVA: 0x0022C85E File Offset: 0x0022AA5E
	public void SetTerrainChunk(Chunk _terrainChunk)
	{
		this.terrainChunk = _terrainChunk;
	}

	// Token: 0x06005547 RID: 21831 RVA: 0x0022C868 File Offset: 0x0022AA68
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void fillDensityInBlock(Chunk _chunk, int _x, int _y, int _z, BlockValue _bv)
	{
		sbyte b = this.terrainChunk.GetDensity(_x, _y, _z);
		if (_bv.Block.shape.IsTerrain() && b >= 0)
		{
			b = -1;
		}
		_chunk.SetDensity(_x, _y, _z, b);
	}

	// Token: 0x040041F4 RID: 16884
	[PublicizedFrom(EAccessModifier.Private)]
	public Chunk terrainChunk;

	// Token: 0x040041F5 RID: 16885
	[PublicizedFrom(EAccessModifier.Private)]
	public RegionFileManager regionFileManager;
}

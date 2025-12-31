using System;

// Token: 0x0200098A RID: 2442
public abstract class SpawnManagerAbstract
{
	// Token: 0x0600498A RID: 18826 RVA: 0x001D0FF2 File Offset: 0x001CF1F2
	public SpawnManagerAbstract(World _world)
	{
		this.world = _world;
	}

	// Token: 0x0600498B RID: 18827
	public abstract void Update(string _spawnerName, bool _bSpawnEnemyEntities, object _userData);

	// Token: 0x040038CC RID: 14540
	[PublicizedFrom(EAccessModifier.Protected)]
	public World world;
}

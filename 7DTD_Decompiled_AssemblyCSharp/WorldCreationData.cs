using System;
using System.Xml.Linq;

// Token: 0x02000A94 RID: 2708
public class WorldCreationData
{
	// Token: 0x0600539C RID: 21404 RVA: 0x00218514 File Offset: 0x00216714
	public WorldCreationData(string _levelDir)
	{
		try
		{
			XDocument xdocument = SdXDocument.Load(_levelDir + "/world.xml");
			if (xdocument.Root == null)
			{
				throw new Exception("No root node in world.xml!");
			}
			foreach (XElement propertyNode in xdocument.Root.Elements("property"))
			{
				this.Properties.Add(propertyNode, true, false);
			}
		}
		catch (Exception)
		{
		}
	}

	// Token: 0x0600539D RID: 21405 RVA: 0x002185BC File Offset: 0x002167BC
	public void Apply(World _world, WorldState _worldState)
	{
		if (this.Properties.Values.ContainsKey("ProviderId"))
		{
			_worldState.providerId = (EnumChunkProviderId)int.Parse(this.Properties.Values["ProviderId"]);
		}
	}

	// Token: 0x04003FD3 RID: 16339
	public const string PropProviderId = "ProviderId";

	// Token: 0x04003FD4 RID: 16340
	public const string PropWorld_Class = "World.Class";

	// Token: 0x04003FD5 RID: 16341
	public const string PropWorldEnvironment_Prefab = "WorldEnvironment.Prefab";

	// Token: 0x04003FD6 RID: 16342
	public const string PropWorldEnvironment_Class = "WorldEnvironment.Class";

	// Token: 0x04003FD7 RID: 16343
	public const string PropWorldBiomeProvider_Class = "WorldBiomeProvider.Class";

	// Token: 0x04003FD8 RID: 16344
	public const string PropWorldTerrainGenerator_Class = "WorldTerrainGenerator.Class";

	// Token: 0x04003FD9 RID: 16345
	public DynamicProperties Properties = new DynamicProperties();
}

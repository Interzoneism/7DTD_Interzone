using System;
using UnityEngine.Scripting;

// Token: 0x0200071D RID: 1821
[Preserve]
public class NetPackageEditorAddInfoVolume : NetPackageEditorAddSleeperVolume
{
	// Token: 0x06003584 RID: 13700 RVA: 0x00164087 File Offset: 0x00162287
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			PrefabVolumeManager.Instance.AddInfoVolumeServer(this.startPos, this.size);
		}
	}
}

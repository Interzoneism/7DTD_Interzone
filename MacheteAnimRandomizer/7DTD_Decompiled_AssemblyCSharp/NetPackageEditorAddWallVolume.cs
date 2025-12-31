using System;
using UnityEngine.Scripting;

// Token: 0x02000721 RID: 1825
[Preserve]
public class NetPackageEditorAddWallVolume : NetPackageEditorAddSleeperVolume
{
	// Token: 0x06003591 RID: 13713 RVA: 0x0016416F File Offset: 0x0016236F
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			PrefabVolumeManager.Instance.AddWallVolumeServer(this.startPos, this.size);
		}
	}
}

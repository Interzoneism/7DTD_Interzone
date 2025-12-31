using System;
using UnityEngine.Scripting;

// Token: 0x0200071F RID: 1823
[Preserve]
public class NetPackageEditorAddTeleportVolume : NetPackageEditorAddSleeperVolume
{
	// Token: 0x0600358D RID: 13709 RVA: 0x00164127 File Offset: 0x00162327
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			PrefabVolumeManager.Instance.AddTeleportVolumeServer(this.startPos, this.size);
		}
	}
}

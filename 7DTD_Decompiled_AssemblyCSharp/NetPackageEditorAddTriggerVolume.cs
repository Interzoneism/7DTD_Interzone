using System;
using UnityEngine.Scripting;

// Token: 0x02000720 RID: 1824
[Preserve]
public class NetPackageEditorAddTriggerVolume : NetPackageEditorAddSleeperVolume
{
	// Token: 0x0600358F RID: 13711 RVA: 0x0016414B File Offset: 0x0016234B
	public override void ProcessPackage(World _world, GameManager _callbacks)
	{
		if (_world == null)
		{
			return;
		}
		if (!_world.IsRemote())
		{
			PrefabTriggerVolumeManager.Instance.AddTriggerVolumeServer(this.startPos, this.size);
		}
	}
}

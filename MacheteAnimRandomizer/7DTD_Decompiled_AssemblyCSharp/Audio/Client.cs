using System;
using UnityEngine;

namespace Audio
{
	// Token: 0x020017A9 RID: 6057
	public class Client : IDisposable
	{
		// Token: 0x0600B54A RID: 46410 RVA: 0x00461F40 File Offset: 0x00460140
		public Client(int _entityId)
		{
			this.entityId = _entityId;
		}

		// Token: 0x0600B54B RID: 46411 RVA: 0x00002914 File Offset: 0x00000B14
		public void Dispose()
		{
		}

		// Token: 0x0600B54C RID: 46412 RVA: 0x00461F50 File Offset: 0x00460150
		public void Play(int playOnEntityId, string soundGoupName, float _occlusion)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(playOnEntityId, soundGoupName, _occlusion, true, false);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600B54D RID: 46413 RVA: 0x00461F90 File Offset: 0x00460190
		public void Play(Vector3 position, string soundGoupName, float _occlusion, int entityId = -1)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(position, soundGoupName, _occlusion, true, entityId);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600B54E RID: 46414 RVA: 0x00461FD4 File Offset: 0x004601D4
		public void Stop(int stopOnEntityId, string soundGroupName)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(stopOnEntityId, soundGroupName, 0f, false, false);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x0600B54F RID: 46415 RVA: 0x00462018 File Offset: 0x00460218
		public void Stop(Vector3 position, string soundGroupName)
		{
			NetPackageAudio package = NetPackageManager.GetPackage<NetPackageAudio>().Setup(position, soundGroupName, 0f, false, -1);
			SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(package, false, this.entityId, -1, -1, null, 192, false);
		}

		// Token: 0x04008E06 RID: 36358
		public int entityId;
	}
}

using System;
using UnityEngine;

namespace Audio
{
	// Token: 0x0200179F RID: 6047
	public class Handle
	{
		// Token: 0x0600B4F2 RID: 46322 RVA: 0x0045D5B0 File Offset: 0x0045B7B0
		public Handle(string soundGroupName, AudioSource near, AudioSource far)
		{
			this.name = soundGroupName;
			this.nearSource = near;
			this.farSource = far;
			if (this.nearSource)
			{
				this.basePitch = this.nearSource.pitch;
				this.baseVolume = this.nearSource.volume;
			}
		}

		// Token: 0x0600B4F3 RID: 46323 RVA: 0x0045D608 File Offset: 0x0045B808
		public void SetPitch(float pitch)
		{
			if (this.nearSource)
			{
				this.nearSource.pitch = pitch + this.basePitch;
			}
			if (this.farSource)
			{
				this.farSource.pitch = pitch + this.basePitch;
			}
		}

		// Token: 0x0600B4F4 RID: 46324 RVA: 0x0045D658 File Offset: 0x0045B858
		public void SetVolume(float volume)
		{
			if (this.nearSource)
			{
				this.nearSource.volume = volume * this.baseVolume;
			}
			if (this.farSource)
			{
				this.farSource.volume = volume * this.baseVolume;
			}
		}

		// Token: 0x0600B4F5 RID: 46325 RVA: 0x0045D6A5 File Offset: 0x0045B8A5
		public void Stop(int entityId)
		{
			Manager.Stop(entityId, this.name);
		}

		// Token: 0x0600B4F6 RID: 46326 RVA: 0x0045D6B4 File Offset: 0x0045B8B4
		public float ClipLength()
		{
			if (this.nearSource)
			{
				return this.nearSource.clip.length;
			}
			if (this.farSource)
			{
				return this.farSource.clip.length;
			}
			return 0f;
		}

		// Token: 0x0600B4F7 RID: 46327 RVA: 0x0045D704 File Offset: 0x0045B904
		public bool IsPlaying()
		{
			bool flag = false;
			if (this.nearSource != null)
			{
				flag = this.nearSource.isPlaying;
			}
			if (this.farSource != null)
			{
				flag |= this.farSource.isPlaying;
			}
			return flag;
		}

		// Token: 0x04008DC0 RID: 36288
		[PublicizedFrom(EAccessModifier.Private)]
		public string name;

		// Token: 0x04008DC1 RID: 36289
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource nearSource;

		// Token: 0x04008DC2 RID: 36290
		[PublicizedFrom(EAccessModifier.Private)]
		public AudioSource farSource;

		// Token: 0x04008DC3 RID: 36291
		[PublicizedFrom(EAccessModifier.Private)]
		public float basePitch;

		// Token: 0x04008DC4 RID: 36292
		[PublicizedFrom(EAccessModifier.Private)]
		public float baseVolume;
	}
}

using System;
using DynamicMusic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x0200009B RID: 155
public class AudioMixerManager : MonoBehaviour
{
	// Token: 0x060002CE RID: 718 RVA: 0x000157B4 File Offset: 0x000139B4
	public void Update()
	{
		if (GameManager.Instance == null)
		{
			return;
		}
		if (GameManager.Instance.World != null)
		{
			EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
			if (primaryPlayer != null)
			{
				bool isUnderwaterCamera = primaryPlayer.IsUnderwaterCamera;
				if (primaryPlayer.isDeafened)
				{
					if (!this.wasDeafened)
					{
						this.transitionTo(this.deafenedSnapshot);
					}
				}
				else if (primaryPlayer.isStunned)
				{
					if (!this.wasStunned || this.wasDeafened)
					{
						this.transitionTo(this.stunnedSnapshot);
					}
				}
				else if (isUnderwaterCamera)
				{
					if (!this.bCameraWasUnderWater || this.wasStunned || this.wasDeafened)
					{
						this.transitionTo(this.underwaterSnapshot);
					}
				}
				else if (this.wasStunned || this.wasDeafened || this.bCameraWasUnderWater)
				{
					this.transitionTo(this.defaultSnapshot);
				}
				this.bCameraWasUnderWater = isUnderwaterCamera;
				this.wasStunned = primaryPlayer.isStunned;
				this.wasDeafened = primaryPlayer.isDeafened;
			}
		}
	}

	// Token: 0x060002CF RID: 719 RVA: 0x000158B4 File Offset: 0x00013AB4
	[PublicizedFrom(EAccessModifier.Private)]
	public void transitionTo(AudioMixerManager.SnapshotController _snapshot)
	{
		_snapshot.snapshot.TransitionTo(_snapshot.transitionToTime);
		if (GamePrefs.GetBool(EnumGamePrefs.OptionsDynamicMusicEnabled) && !GameManager.Instance.IsEditMode())
		{
			MixerController.Instance.OnSnapshotTransition();
		}
	}

	// Token: 0x0400033A RID: 826
	public AudioMixerManager.SnapshotController underwaterSnapshot;

	// Token: 0x0400033B RID: 827
	public AudioMixerManager.SnapshotController stunnedSnapshot;

	// Token: 0x0400033C RID: 828
	public AudioMixerManager.SnapshotController deafenedSnapshot;

	// Token: 0x0400033D RID: 829
	public AudioMixerManager.SnapshotController defaultSnapshot;

	// Token: 0x0400033E RID: 830
	public bool bCameraWasUnderWater;

	// Token: 0x0400033F RID: 831
	public bool wasStunned;

	// Token: 0x04000340 RID: 832
	public bool wasDeafened;

	// Token: 0x0200009C RID: 156
	[Serializable]
	public class SnapshotController
	{
		// Token: 0x04000341 RID: 833
		public AudioMixerSnapshot snapshot;

		// Token: 0x04000342 RID: 834
		public float transitionToTime = 1f;
	}
}

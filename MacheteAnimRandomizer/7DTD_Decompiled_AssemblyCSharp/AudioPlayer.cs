using System;
using Audio;
using UnityEngine;

// Token: 0x020000A0 RID: 160
public class AudioPlayer : MonoBehaviour
{
	// Token: 0x060002E1 RID: 737 RVA: 0x000167E0 File Offset: 0x000149E0
	public void Play()
	{
		if (!this.refEntity)
		{
			this.refEntity = RootTransformRefEntity.AddIfEntity(base.transform);
			if (this.refEntity)
			{
				this.attachedEntity = this.refEntity.RootTransform.GetComponent<Entity>();
			}
		}
		if (this.attachedEntity)
		{
			Manager.Play(this.attachedEntity, this.soundName, 1f, false);
			this.isPlaying = true;
			this.queuedForPlaying = false;
		}
		else if (this.refEntity)
		{
			Vector3 position = this.refEntity.transform.position;
			if (position == Vector3.zero)
			{
				this.queuedForPlaying = true;
			}
			else
			{
				this.PlayAtPos(position);
			}
		}
		else
		{
			Vector3 position2 = base.transform.position;
			if (position2 == Vector3.zero)
			{
				this.queuedForPlaying = true;
			}
			else
			{
				this.PlayAtPos(position2);
			}
		}
		if (this.isPlaying && this.duration > 0f)
		{
			this.startTime = Time.time;
		}
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x000168E9 File Offset: 0x00014AE9
	[PublicizedFrom(EAccessModifier.Private)]
	public void PlayAtPos(Vector3 _pos)
	{
		this.playPos = _pos + Origin.position;
		Manager.Play(this.playPos, this.soundName, -1, false);
		this.isPlaying = true;
		this.queuedForPlaying = false;
	}

	// Token: 0x060002E3 RID: 739 RVA: 0x00016920 File Offset: 0x00014B20
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.startDelay > 0f)
		{
			this.startDelay -= Time.deltaTime;
			return;
		}
		if (this.queuedForPlaying)
		{
			this.Play();
		}
		if (this.isPlaying && this.duration > 0f && Time.time > this.startTime + this.duration)
		{
			this.StopAudio();
		}
	}

	// Token: 0x060002E4 RID: 740 RVA: 0x0001698C File Offset: 0x00014B8C
	[PublicizedFrom(EAccessModifier.Private)]
	public void StopAudio()
	{
		if (this.isPlaying)
		{
			if (this.attachedEntity)
			{
				Manager.Stop(this.attachedEntity.entityId, this.soundName);
			}
			else
			{
				Manager.Stop(this.playPos, this.soundName);
			}
			this.isPlaying = false;
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x000169DE File Offset: 0x00014BDE
	public void OnEnable()
	{
		if (!this.playOnDemand)
		{
			this.queuedForPlaying = true;
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x000169EF File Offset: 0x00014BEF
	public void OnDisable()
	{
		this.StopAudio();
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x000169EF File Offset: 0x00014BEF
	public void OnDestroy()
	{
		this.StopAudio();
	}

	// Token: 0x04000375 RID: 885
	public string soundName;

	// Token: 0x04000376 RID: 886
	public float duration = -1f;

	// Token: 0x04000377 RID: 887
	public bool playOnDemand;

	// Token: 0x04000378 RID: 888
	public float startDelay;

	// Token: 0x04000379 RID: 889
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Entity attachedEntity;

	// Token: 0x0400037A RID: 890
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public RootTransformRefEntity refEntity;

	// Token: 0x0400037B RID: 891
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool queuedForPlaying;

	// Token: 0x0400037C RID: 892
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool isPlaying;

	// Token: 0x0400037D RID: 893
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float startTime;

	// Token: 0x0400037E RID: 894
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 playPos;
}

using System;
using UnityEngine;

// Token: 0x02001039 RID: 4153
public class NGuiPanelFade : MonoBehaviour
{
	// Token: 0x06008365 RID: 33637 RVA: 0x0034FE7C File Offset: 0x0034E07C
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Awake()
	{
		this.mWidgets = base.GetComponentsInChildren<UIWidget>();
		this.alpha = new float[this.mWidgets.Length];
		for (int i = 0; i < this.mWidgets.Length; i++)
		{
			this.alpha[i] = this.mWidgets[i].color.a;
		}
	}

	// Token: 0x06008366 RID: 33638 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Start()
	{
	}

	// Token: 0x06008367 RID: 33639 RVA: 0x0034FED5 File Offset: 0x0034E0D5
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnEnable()
	{
		this.bFadeOut = false;
		this.init();
	}

	// Token: 0x06008368 RID: 33640 RVA: 0x0034FEE4 File Offset: 0x0034E0E4
	[PublicizedFrom(EAccessModifier.Protected)]
	public void OnDisable()
	{
		this.reset();
	}

	// Token: 0x06008369 RID: 33641 RVA: 0x0034FEEC File Offset: 0x0034E0EC
	[PublicizedFrom(EAccessModifier.Private)]
	public void reset()
	{
		for (int i = 0; i < this.mWidgets.Length; i++)
		{
			Color color = this.mWidgets[i].color;
			color.a = this.alpha[i];
			this.mWidgets[i].color = color;
		}
	}

	// Token: 0x0600836A RID: 33642 RVA: 0x0034FF38 File Offset: 0x0034E138
	[PublicizedFrom(EAccessModifier.Private)]
	public void init()
	{
		this.mStart = Time.time;
		this.bFadeIn = this.bFadeInWhenEnabled;
		this.mWidgets = base.GetComponentsInChildren<UIWidget>();
		if (this.alpha.Length != this.mWidgets.Length)
		{
			this.alpha = new float[this.mWidgets.Length];
		}
		for (int i = 0; i < this.mWidgets.Length; i++)
		{
			Color color = this.mWidgets[i].color;
			if (color.a != 0f)
			{
				this.alpha[i] = color.a;
			}
			if (this.bFadeIn)
			{
				color.a = 0f;
				this.mWidgets[i].color = color;
			}
		}
	}

	// Token: 0x0600836B RID: 33643 RVA: 0x0034FFEC File Offset: 0x0034E1EC
	[PublicizedFrom(EAccessModifier.Protected)]
	public void Update()
	{
		float num;
		if (this.bFadeIn)
		{
			num = ((this.duration > 0f) ? Mathf.Clamp01((Time.time - this.mStart) / this.duration) : 1f);
		}
		else
		{
			if (!this.bFadeOut)
			{
				return;
			}
			num = ((this.duration > 0f) ? (1f - Mathf.Clamp01((Time.realtimeSinceStartup - this.mStart) / this.duration)) : 0f);
		}
		for (int i = 0; i < this.mWidgets.Length; i++)
		{
			Color color = this.mWidgets[i].color;
			color.a = num * this.alpha[i];
			this.mWidgets[i].color = color;
		}
		if (this.bFadeOut && num <= 0.001f)
		{
			this.reset();
			base.gameObject.SetActive(false);
			this.bFadeOut = false;
		}
		if (this.bFadeIn && num >= 1f)
		{
			this.bFadeIn = false;
		}
	}

	// Token: 0x0600836C RID: 33644 RVA: 0x003500F4 File Offset: 0x0034E2F4
	public void StartFadeOut()
	{
		this.bFadeOut = true;
		this.mStart = Time.time;
	}

	// Token: 0x0400655B RID: 25947
	public float duration = 0.3f;

	// Token: 0x0400655C RID: 25948
	public bool bFadeInWhenEnabled = true;

	// Token: 0x0400655D RID: 25949
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float mStart;

	// Token: 0x0400655E RID: 25950
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public UIWidget[] mWidgets;

	// Token: 0x0400655F RID: 25951
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float[] alpha;

	// Token: 0x04006560 RID: 25952
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bFadeIn;

	// Token: 0x04006561 RID: 25953
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool bFadeOut;
}

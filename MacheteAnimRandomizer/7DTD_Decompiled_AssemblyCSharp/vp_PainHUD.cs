using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02001370 RID: 4976
public class vp_PainHUD : MonoBehaviour
{
	// Token: 0x06009BEE RID: 39918 RVA: 0x003DF751 File Offset: 0x003DD951
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		this.m_Player = base.transform.GetComponent<vp_FPPlayerEventHandler>();
	}

	// Token: 0x06009BEF RID: 39919 RVA: 0x003DF764 File Offset: 0x003DD964
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnEnable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Register(this);
		}
	}

	// Token: 0x06009BF0 RID: 39920 RVA: 0x003DF780 File Offset: 0x003DD980
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnDisable()
	{
		if (this.m_Player != null)
		{
			this.m_Player.Unregister(this);
		}
	}

	// Token: 0x06009BF1 RID: 39921 RVA: 0x003DF79C File Offset: 0x003DD99C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnGUI()
	{
		this.UpdatePainFlash();
		this.UpdateInflictorArrows();
		this.UpdateDeathTexture();
	}

	// Token: 0x06009BF2 RID: 39922 RVA: 0x003DF7B0 File Offset: 0x003DD9B0
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdatePainFlash()
	{
		if (this.m_PainColor.a < 0.01f)
		{
			this.m_PainColor.a = 0f;
			return;
		}
		this.m_PainColor = Color.Lerp(this.m_PainColor, this.m_FlashInvisibleColor, Time.deltaTime * 0.4f);
		GUI.color = this.m_PainColor;
		if (this.PainTexture != null)
		{
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.PainTexture);
		}
		GUI.color = Color.white;
	}

	// Token: 0x06009BF3 RID: 39923 RVA: 0x003DF84C File Offset: 0x003DDA4C
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateInflictorArrows()
	{
		if (this.ArrowTexture == null)
		{
			return;
		}
		for (int i = this.m_Inflictors.Count - 1; i > -1; i--)
		{
			if (this.m_Inflictors[i] == null || this.m_Inflictors[i].Transform == null || !vp_Utility.IsActive(this.m_Inflictors[i].Transform.gameObject))
			{
				this.m_Inflictors.Remove(this.m_Inflictors[i]);
			}
			else
			{
				this.m_ArrowColor.a = (this.ArrowVisibleDuration - (Time.time - this.m_Inflictors[i].DamageTime)) / this.ArrowVisibleDuration;
				if (this.m_ArrowColor.a >= 0f)
				{
					Vector2 vector = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
					float angle = vp_3DUtility.LookAtAngleHorizontal(base.transform.position, base.transform.forward, this.m_Inflictors[i].Transform.position) + this.ArrowAngleOffset;
					float num = (float)Screen.width * this.ArrowScale;
					float num2 = (this.ArrowShakeDuration - (Time.time - this.m_LastInflictorTime)) / this.ArrowShakeDuration;
					num2 = Mathf.Lerp(0f, 1f, num2);
					num += (float)(Screen.width / 100) * num2;
					Matrix4x4 matrix = GUI.matrix;
					GUIUtility.RotateAroundPivot(angle, vector);
					GUI.color = this.m_ArrowColor;
					GUI.DrawTexture(new Rect(vector.x, vector.y, num, num), this.ArrowTexture);
					GUI.matrix = matrix;
				}
			}
		}
	}

	// Token: 0x06009BF4 RID: 39924 RVA: 0x003DFA0A File Offset: 0x003DDC0A
	[PublicizedFrom(EAccessModifier.Private)]
	public void UpdateDeathTexture()
	{
		if (this.DeathTexture == null)
		{
			return;
		}
		if (!this.m_Player.Dead.Active)
		{
			return;
		}
		GUI.color = this.m_SplatColor;
		GUI.DrawTexture(this.m_SplatRect, this.DeathTexture);
	}

	// Token: 0x06009BF5 RID: 39925 RVA: 0x003DFA4C File Offset: 0x003DDC4C
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void OnMessage_HUDDamageFlash(vp_DamageInfo damageInfo)
	{
		if (damageInfo == null || damageInfo.Damage == 0f)
		{
			this.m_PainColor.a = 0f;
			return;
		}
		this.m_PainColor.a = this.m_PainColor.a + damageInfo.Damage * this.PainIntensity;
		if (damageInfo.Source != null)
		{
			this.m_LastInflictorTime = Time.time;
			bool flag = true;
			foreach (vp_PainHUD.Inflictor inflictor in this.m_Inflictors)
			{
				if (inflictor.Transform == damageInfo.Source.transform)
				{
					inflictor.DamageTime = Time.time;
					flag = false;
				}
			}
			if (flag)
			{
				this.m_Inflictors.Add(new vp_PainHUD.Inflictor(damageInfo.Source, Time.time));
			}
		}
	}

	// Token: 0x06009BF6 RID: 39926 RVA: 0x003DFB34 File Offset: 0x003DDD34
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnStart_Dead()
	{
		float num = UnityEngine.Random.value * 0.6f + 0.4f;
		this.m_SplatColor = new Color(num, num, num, 1f);
		float num2 = (float)((UnityEngine.Random.value < 0.5f) ? (Screen.width / UnityEngine.Random.Range(5, 10)) : (Screen.width / UnityEngine.Random.Range(4, 7)));
		this.m_SplatRect = new Rect(UnityEngine.Random.Range(-num2, 0f), UnityEngine.Random.Range(-num2, 0f), (float)Screen.width + num2, (float)Screen.height + num2);
		if (UnityEngine.Random.value < 0.5f)
		{
			this.m_SplatRect.x = (float)Screen.width - this.m_SplatRect.x;
			this.m_SplatRect.width = -this.m_SplatRect.width;
		}
		if (UnityEngine.Random.value < 0.125f)
		{
			num *= 0.5f;
			this.m_SplatColor = new Color(num, num, num, 1f);
			this.m_SplatRect.y = (float)Screen.height - this.m_SplatRect.y;
			this.m_SplatRect.height = -this.m_SplatRect.height;
		}
	}

	// Token: 0x06009BF7 RID: 39927 RVA: 0x003DFC60 File Offset: 0x003DDE60
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnStop_Dead()
	{
		this.m_PainColor.a = 0f;
		for (int i = this.m_Inflictors.Count - 1; i > -1; i--)
		{
			this.m_Inflictors[i].DamageTime = 0f;
		}
	}

	// Token: 0x04007888 RID: 30856
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public List<vp_PainHUD.Inflictor> m_Inflictors = new List<vp_PainHUD.Inflictor>();

	// Token: 0x04007889 RID: 30857
	public Texture PainTexture;

	// Token: 0x0400788A RID: 30858
	public Texture DeathTexture;

	// Token: 0x0400788B RID: 30859
	public Texture ArrowTexture;

	// Token: 0x0400788C RID: 30860
	public float PainIntensity = 0.2f;

	// Token: 0x0400788D RID: 30861
	[Range(0.01f, 0.5f)]
	public float ArrowScale = 0.083f;

	// Token: 0x0400788E RID: 30862
	public float ArrowAngleOffset = -135f;

	// Token: 0x0400788F RID: 30863
	public float ArrowVisibleDuration = 1.5f;

	// Token: 0x04007890 RID: 30864
	public float ArrowShakeDuration = 0.125f;

	// Token: 0x04007891 RID: 30865
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public float m_LastInflictorTime;

	// Token: 0x04007892 RID: 30866
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_PainColor = new Color(0.8f, 0f, 0f, 0f);

	// Token: 0x04007893 RID: 30867
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_ArrowColor = new Color(0.8f, 0f, 0f, 0f);

	// Token: 0x04007894 RID: 30868
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_FlashInvisibleColor = new Color(1f, 0f, 0f, 0f);

	// Token: 0x04007895 RID: 30869
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Color m_SplatColor = Color.white;

	// Token: 0x04007896 RID: 30870
	[PublicizedFrom(EAccessModifier.Protected)]
	[NonSerialized]
	public Rect m_SplatRect;

	// Token: 0x04007897 RID: 30871
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public vp_FPPlayerEventHandler m_Player;

	// Token: 0x02001371 RID: 4977
	[PublicizedFrom(EAccessModifier.Protected)]
	public class Inflictor
	{
		// Token: 0x06009BF9 RID: 39929 RVA: 0x003DFD69 File Offset: 0x003DDF69
		public Inflictor(Transform transform, float damageTime)
		{
			this.Transform = transform;
			this.DamageTime = damageTime;
		}

		// Token: 0x04007898 RID: 30872
		public Transform Transform;

		// Token: 0x04007899 RID: 30873
		public float DamageTime;
	}
}

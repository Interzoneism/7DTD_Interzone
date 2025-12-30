using System;
using TMPro;
using UnityEngine;

// Token: 0x02000AE3 RID: 2787
public class DamageText : MonoBehaviour
{
	// Token: 0x060055C2 RID: 21954 RVA: 0x00230E20 File Offset: 0x0022F020
	public static void Create(string _text, Color _color, Vector3 _worldPos, Vector3 _velocity, float _scale = 1f)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load("Prefabs/DamageText"));
		DamageText component = gameObject.GetComponent<DamageText>();
		TextMeshPro component2 = gameObject.GetComponent<TextMeshPro>();
		component.textMeshPro = component2;
		component2.text = _text;
		component2.color = _color;
		component2.rectTransform.localScale = new Vector3(_scale, _scale, _scale);
		component.worldPos = _worldPos;
		component.velocity = _velocity;
		component.cameraT = Camera.main.transform;
	}

	// Token: 0x060055C3 RID: 21955 RVA: 0x00230E98 File Offset: 0x0022F098
	[PublicizedFrom(EAccessModifier.Private)]
	public void LateUpdate()
	{
		float deltaTime = Time.deltaTime;
		this.TimeDuration -= deltaTime;
		if (this.TimeDuration <= 0f || !this.cameraT)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		this.textMeshPro.alpha = Utils.FastLerp(0f, 1f, this.TimeDuration * 2f);
		this.velocityDecayDelay -= deltaTime;
		if (this.velocityDecayDelay <= 0f)
		{
			this.velocityDecayDelay = 0.1f;
			this.velocity *= 0.8f;
		}
		this.worldPos += this.velocity * deltaTime;
		base.transform.SetPositionAndRotation(Vector3.MoveTowards(this.worldPos - Origin.position, this.cameraT.position + this.cameraT.forward * 0.18f, 0.25f), this.cameraT.rotation);
	}

	// Token: 0x04004267 RID: 16999
	public static bool Enabled;

	// Token: 0x04004268 RID: 17000
	public float TimeDuration = 1.5f;

	// Token: 0x04004269 RID: 17001
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public TextMeshPro textMeshPro;

	// Token: 0x0400426A RID: 17002
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 worldPos;

	// Token: 0x0400426B RID: 17003
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Vector3 velocity;

	// Token: 0x0400426C RID: 17004
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public float velocityDecayDelay = 0.2f;

	// Token: 0x0400426D RID: 17005
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Transform cameraT;
}

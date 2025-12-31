using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

// Token: 0x020010D8 RID: 4312
[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Rendering/Colorize")]
public class ImageEffectManager : MonoBehaviour
{
	// Token: 0x060087C4 RID: 34756 RVA: 0x0036F0B4 File Offset: 0x0036D2B4
	public int GetNumEffects(string _effectGroupName)
	{
		ImageEffectManager.ValidateStaticClassEffects();
		if (ImageEffectManager.staticEffectGroups == null)
		{
			return 0;
		}
		ImageEffectManager.EffectGroup effectGroup;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroupName, out effectGroup))
		{
			return effectGroup.GetNumEffects();
		}
		return 0;
	}

	// Token: 0x060087C5 RID: 34757 RVA: 0x0036F0E8 File Offset: 0x0036D2E8
	public string GetEffectName(string _effectGroupName, int _index)
	{
		if (_index <= 0)
		{
			return "Off";
		}
		ImageEffectManager.ValidateStaticClassEffects();
		if (ImageEffectManager.staticEffectGroups == null)
		{
			return "";
		}
		ImageEffectManager.EffectGroup effectGroup;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroupName, out effectGroup))
		{
			return effectGroup.GetEffectName(_index);
		}
		return "";
	}

	// Token: 0x060087C6 RID: 34758 RVA: 0x0036F130 File Offset: 0x0036D330
	public void DisableEffectGroup(string _effectGroupName)
	{
		if (this.enabledEffects == null)
		{
			return;
		}
		Dictionary<int, float> dictionary;
		if (this.enabledEffects.TryGetValue(_effectGroupName, out dictionary))
		{
			this.numEnabledEffects -= dictionary.Count;
			dictionary.Clear();
		}
	}

	// Token: 0x060087C7 RID: 34759 RVA: 0x0036F170 File Offset: 0x0036D370
	public bool SetEffect_Slow(string _effectGroupName, string _effectName, float _newIntensity = 1f)
	{
		if (!this.ValidateEffects())
		{
			return false;
		}
		ImageEffectManager.EffectGroup effectGroup;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroupName, out effectGroup))
		{
			int num = 0;
			ImageEffectManager.Effect[] effects = effectGroup.effects;
			for (int i = 0; i < effects.Length; i++)
			{
				if (effects[i].name.Equals(_effectName))
				{
					return this.SetEffect(_effectGroupName, num, _newIntensity);
				}
				num++;
			}
		}
		return true;
	}

	// Token: 0x060087C8 RID: 34760 RVA: 0x0036F1CC File Offset: 0x0036D3CC
	[PublicizedFrom(EAccessModifier.Private)]
	public bool ValidateEffects()
	{
		ImageEffectManager.ValidateStaticClassEffects();
		if (ImageEffectManager.staticClass == null)
		{
			return false;
		}
		if (ImageEffectManager.staticEffectGroups == null)
		{
			return false;
		}
		if (this.enabledEffects == null)
		{
			this.enabledEffects = new Dictionary<string, Dictionary<int, float>>();
		}
		return true;
	}

	// Token: 0x060087C9 RID: 34761 RVA: 0x0036F200 File Offset: 0x0036D400
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetEffectIntenal(string _effectGroupName, int _index, float _newIntensity = 1f)
	{
		Dictionary<int, float> dictionary;
		if (this.enabledEffects.TryGetValue(_effectGroupName, out dictionary))
		{
			float num = 0f;
			if (dictionary.TryGetValue(_index, out num))
			{
				if (_newIntensity == 0f)
				{
					dictionary.Remove(_index);
					if (dictionary.Count == 0)
					{
						this.enabledEffects.Remove(_effectGroupName);
					}
					this.numEnabledEffects--;
					return;
				}
				dictionary[_index] = _newIntensity;
				return;
			}
			else if (_newIntensity > 0f)
			{
				dictionary.Add(_index, _newIntensity);
				this.numEnabledEffects++;
			}
		}
		else if (_newIntensity > 0f)
		{
			Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
			dictionary2.Add(_index, _newIntensity);
			this.enabledEffects.Add(_effectGroupName, dictionary2);
			this.numEnabledEffects++;
		}
		base.enabled = (this.numEnabledEffects > 0);
	}

	// Token: 0x060087CA RID: 34762 RVA: 0x0036F2CB File Offset: 0x0036D4CB
	public bool SetEffect(string _effectGroupName, float _newIntensity = 1f)
	{
		return this.SetEffect(_effectGroupName, 0, _newIntensity);
	}

	// Token: 0x060087CB RID: 34763 RVA: 0x0036F2D8 File Offset: 0x0036D4D8
	public bool SetEffect(string _effectGroupName, string _effectName, float _newIntensity = 1f)
	{
		ImageEffectManager.ValidateStaticClassEffects();
		ImageEffectManager.EffectGroup effectGroup;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroupName, out effectGroup))
		{
			int num = 0;
			ImageEffectManager.Effect[] effects = effectGroup.effects;
			for (int i = 0; i < effects.Length; i++)
			{
				if (effects[i].name.Equals(_effectName))
				{
					return this.SetEffect(_effectGroupName, num, _newIntensity);
				}
				num++;
			}
		}
		return false;
	}

	// Token: 0x060087CC RID: 34764 RVA: 0x0036F32F File Offset: 0x0036D52F
	public bool SetEffect(string _effectGroupName, int _index, float _newIntensity = 1f)
	{
		if (_index < 0)
		{
			return false;
		}
		if (!this.ValidateEffects())
		{
			return false;
		}
		this.SetEffectIntenal(_effectGroupName, _index, _newIntensity);
		return true;
	}

	// Token: 0x060087CD RID: 34765 RVA: 0x0036F34B File Offset: 0x0036D54B
	[PublicizedFrom(EAccessModifier.Protected)]
	public virtual void Start()
	{
		ImageEffectManager.ValidateStaticClassEffects();
		ImageEffectManager.staticClass == null;
	}

	// Token: 0x060087CE RID: 34766 RVA: 0x0036F360 File Offset: 0x0036D560
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ValidateStaticClassEffects()
	{
		if (ImageEffectManager.validated)
		{
			return;
		}
		if (ImageEffectManager.staticGameObject == null)
		{
			ImageEffectManager.staticGameObject = Resources.Load<GameObject>("Prefabs/ImageEffectsPrefab");
			if (ImageEffectManager.staticGameObject != null)
			{
				ImageEffectManager.staticClass = ImageEffectManager.staticGameObject.GetComponent<ImageEffectManager>();
			}
		}
		if (ImageEffectManager.staticClass == null)
		{
			return;
		}
		if (ImageEffectManager.staticEffectGroups == null)
		{
			ImageEffectManager.staticEffectGroups = new Dictionary<string, ImageEffectManager.EffectGroup>();
		}
		if (ImageEffectManager.staticClass.effectGroups != null)
		{
			foreach (ImageEffectManager.EffectGroup effectGroup in ImageEffectManager.staticClass.effectGroups)
			{
				ImageEffectManager.staticEffectGroups.Add(effectGroup.name, effectGroup);
			}
		}
		ImageEffectManager.validated = true;
	}

	// Token: 0x060087CF RID: 34767 RVA: 0x0036F40C File Offset: 0x0036D60C
	public void SetFloat_Slow(string _effectGroup, string _effectName, string _propertyName, float _value)
	{
		ImageEffectManager.ValidateStaticClassEffects();
		ImageEffectManager.EffectGroup effectGroup = null;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroup, out effectGroup))
		{
			for (int i = 0; i < effectGroup.effects.Length; i++)
			{
				if (effectGroup.effects[i].name.EqualsCaseInsensitive(_effectName))
				{
					this.SetFloat(effectGroup, i, _propertyName, _value);
					return;
				}
			}
		}
	}

	// Token: 0x060087D0 RID: 34768 RVA: 0x0036F464 File Offset: 0x0036D664
	public void SetFloat(string _effectGroup, int _effectIndex, string _propertyName, float _value)
	{
		ImageEffectManager.ValidateStaticClassEffects();
		if (_effectIndex < 0)
		{
			return;
		}
		ImageEffectManager.EffectGroup effectGroup = null;
		if (ImageEffectManager.staticEffectGroups.TryGetValue(_effectGroup, out effectGroup))
		{
			this.SetFloat(effectGroup, _effectIndex, _propertyName, _value);
		}
	}

	// Token: 0x060087D1 RID: 34769 RVA: 0x0036F497 File Offset: 0x0036D697
	public void SetFloat(ImageEffectManager.EffectGroup _effectGroup, int _effectIndex, string _propertyName, float _value)
	{
		ImageEffectManager.ValidateStaticClassEffects();
		if (_effectGroup.effects.Length > _effectIndex)
		{
			_effectGroup.effects[_effectIndex].SetFloat(_propertyName, _value);
		}
	}

	// Token: 0x060087D2 RID: 34770 RVA: 0x0036F4BC File Offset: 0x0036D6BC
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (source == null)
		{
			return;
		}
		if (this.enabledEffects == null || this.enabledEffects.Count == 0)
		{
			base.enabled = false;
			return;
		}
		ImageEffectManager.ValidateStaticClassEffects();
		RenderTexture renderTexture = null;
		RenderTexture renderTexture2 = null;
		if (this.enabledEffects.Count > 1)
		{
			renderTexture = RenderTexture.GetTemporary(source.width, source.height);
		}
		if (this.enabledEffects.Count > 2)
		{
			renderTexture2 = RenderTexture.GetTemporary(source.width, source.height);
		}
		bool flag = true;
		int num = 0;
		RenderTexture renderTexture3 = source;
		foreach (KeyValuePair<string, Dictionary<int, float>> keyValuePair in this.enabledEffects)
		{
			foreach (KeyValuePair<int, float> keyValuePair2 in keyValuePair.Value)
			{
				if (ImageEffectManager.staticEffectGroups.ContainsKey(keyValuePair.Key))
				{
					ImageEffectManager.Effect effect = ImageEffectManager.staticEffectGroups[keyValuePair.Key].effects[keyValuePair2.Key];
					if (!(effect.material == null))
					{
						effect.GetMaterial().SetFloat("Intensity", Mathf.Clamp01(keyValuePair2.Value));
						Material material = effect.GetMaterial();
						BlendMode blendMode = BlendMode.OneMinusDstAlpha;
						bool flag2 = false;
						if (effect.hasProperty.TryGetValue("BlendSrc", out flag2) && flag2)
						{
							blendMode = (BlendMode)material.GetInt("BlendSrc");
						}
						effect.UpdateMaterial();
						if (renderTexture == null)
						{
							Graphics.Blit(renderTexture3, destination, effect.GetMaterial());
						}
						else if (flag)
						{
							if (blendMode == BlendMode.Zero)
							{
								Graphics.Blit(renderTexture3, renderTexture, effect.GetMaterial());
								renderTexture3 = renderTexture;
							}
							else
							{
								Graphics.Blit(renderTexture3, renderTexture3, effect.GetMaterial());
							}
							flag = false;
						}
						else if (num == this.numEnabledEffects - 1)
						{
							Graphics.Blit(renderTexture3, destination, effect.GetMaterial());
						}
						else if (blendMode == BlendMode.Zero)
						{
							RenderTexture renderTexture4 = (renderTexture3 == renderTexture) ? renderTexture2 : renderTexture;
							Graphics.Blit(renderTexture3, renderTexture4, effect.GetMaterial());
							renderTexture3 = renderTexture4;
						}
						else
						{
							Graphics.Blit(renderTexture3, renderTexture3, effect.GetMaterial());
						}
						num++;
					}
				}
			}
		}
		if (renderTexture != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture);
		}
		if (renderTexture2 != null)
		{
			RenderTexture.ReleaseTemporary(renderTexture2);
		}
	}

	// Token: 0x04006990 RID: 27024
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static GameObject staticGameObject;

	// Token: 0x04006991 RID: 27025
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static ImageEffectManager staticClass;

	// Token: 0x04006992 RID: 27026
	public static Dictionary<string, ImageEffectManager.EffectGroup> staticEffectGroups;

	// Token: 0x04006993 RID: 27027
	public ImageEffectManager.EffectGroup[] effectGroups;

	// Token: 0x04006994 RID: 27028
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, Dictionary<int, float>> enabledEffects;

	// Token: 0x04006995 RID: 27029
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int numEnabledEffects;

	// Token: 0x04006996 RID: 27030
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static bool validated;

	// Token: 0x020010D9 RID: 4313
	[Serializable]
	public class Effect
	{
		// Token: 0x060087D4 RID: 34772 RVA: 0x0036F74C File Offset: 0x0036D94C
		public Effect()
		{
			this.instantiatedMtrl = null;
			this.hasProperty = new Dictionary<string, bool>();
			this.floatPropertyUpdates = new Dictionary<string, float>();
		}

		// Token: 0x060087D5 RID: 34773 RVA: 0x0036F778 File Offset: 0x0036D978
		public void UpdateMaterial()
		{
			if (this.material == null)
			{
				return;
			}
			foreach (KeyValuePair<string, float> keyValuePair in this.floatPropertyUpdates)
			{
				if (this.needToCheckMaterial)
				{
					if (this.GetMaterial().HasProperty(keyValuePair.Key))
					{
						this.GetMaterial().SetFloat(keyValuePair.Key, keyValuePair.Value);
					}
				}
				else
				{
					this.GetMaterial().SetFloat(keyValuePair.Key, keyValuePair.Value);
				}
			}
			this.floatPropertyUpdates.Clear();
			this.needToCheckMaterial = false;
		}

		// Token: 0x060087D6 RID: 34774 RVA: 0x0036F838 File Offset: 0x0036DA38
		public void SetFloat(string _propertyName, float _value)
		{
			bool flag = false;
			if (this.hasProperty.TryGetValue(_propertyName, out flag))
			{
				if (flag)
				{
					float num = 0f;
					if (!this.floatPropertyUpdates.TryGetValue(_propertyName, out num))
					{
						this.floatPropertyUpdates.Add(_propertyName, _value);
						return;
					}
					this.floatPropertyUpdates[_propertyName] = _value;
					return;
				}
			}
			else if (this.material != null)
			{
				bool flag2 = this.GetMaterial().HasProperty(_propertyName);
				this.hasProperty.Add(_propertyName, flag2);
				if (flag2)
				{
					this.floatPropertyUpdates.Add(_propertyName, _value);
					return;
				}
			}
			else
			{
				this.needToCheckMaterial = true;
				float num2 = 0f;
				if (!this.floatPropertyUpdates.TryGetValue(_propertyName, out num2))
				{
					this.floatPropertyUpdates.Add(_propertyName, _value);
					return;
				}
				this.floatPropertyUpdates[_propertyName] = _value;
			}
		}

		// Token: 0x060087D7 RID: 34775 RVA: 0x0036F900 File Offset: 0x0036DB00
		public Material GetMaterial()
		{
			if (this.instantiatedMtrl == null)
			{
				this.instantiatedMtrl = UnityEngine.Object.Instantiate<Material>(this.material);
			}
			return this.instantiatedMtrl;
		}

		// Token: 0x04006997 RID: 27031
		public string name;

		// Token: 0x04006998 RID: 27032
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool needToCheckMaterial = true;

		// Token: 0x04006999 RID: 27033
		public Material material;

		// Token: 0x0400699A RID: 27034
		public Material instantiatedMtrl;

		// Token: 0x0400699B RID: 27035
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public GameObject materialHolder;

		// Token: 0x0400699C RID: 27036
		public Dictionary<string, bool> hasProperty;

		// Token: 0x0400699D RID: 27037
		public Dictionary<string, float> floatPropertyUpdates;
	}

	// Token: 0x020010DA RID: 4314
	[Serializable]
	public class EffectGroup
	{
		// Token: 0x060087D8 RID: 34776 RVA: 0x0036F927 File Offset: 0x0036DB27
		public int GetNumEffects()
		{
			if (this.effects == null)
			{
				return 0;
			}
			return this.effects.Length;
		}

		// Token: 0x060087D9 RID: 34777 RVA: 0x0036F93B File Offset: 0x0036DB3B
		public string GetEffectName(int _index)
		{
			if (this.effects == null)
			{
				return "";
			}
			if (_index < 0)
			{
				return "";
			}
			if (_index >= this.effects.Length)
			{
				return "";
			}
			return this.effects[_index].name;
		}

		// Token: 0x0400699E RID: 27038
		public string name;

		// Token: 0x0400699F RID: 27039
		public ImageEffectManager.Effect[] effects;
	}
}

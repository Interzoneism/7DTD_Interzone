using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

// Token: 0x02000F0D RID: 3853
public class XUiV_Texture : XUiView
{
	// Token: 0x17000CBA RID: 3258
	// (get) Token: 0x06007AA0 RID: 31392 RVA: 0x0031B8BA File Offset: 0x00319ABA
	public UITexture UITexture
	{
		get
		{
			return this.uiTexture;
		}
	}

	// Token: 0x17000CBB RID: 3259
	// (get) Token: 0x06007AA1 RID: 31393 RVA: 0x0031B8C2 File Offset: 0x00319AC2
	// (set) Token: 0x06007AA2 RID: 31394 RVA: 0x0031B8CA File Offset: 0x00319ACA
	public Texture Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CBC RID: 3260
	// (get) Token: 0x06007AA3 RID: 31395 RVA: 0x0031B8DA File Offset: 0x00319ADA
	// (set) Token: 0x06007AA4 RID: 31396 RVA: 0x0031B8E2 File Offset: 0x00319AE2
	public Material Material
	{
		get
		{
			return this.material;
		}
		set
		{
			this.material = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CBD RID: 3261
	// (get) Token: 0x06007AA5 RID: 31397 RVA: 0x0031B8F2 File Offset: 0x00319AF2
	// (set) Token: 0x06007AA6 RID: 31398 RVA: 0x0031B8FA File Offset: 0x00319AFA
	public Rect UVRect
	{
		get
		{
			return this.uvRect;
		}
		set
		{
			this.uvRect = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CBE RID: 3262
	// (get) Token: 0x06007AA7 RID: 31399 RVA: 0x0031B90A File Offset: 0x00319B0A
	// (set) Token: 0x06007AA8 RID: 31400 RVA: 0x0031B912 File Offset: 0x00319B12
	public UIBasicSprite.Type Type
	{
		get
		{
			return this.type;
		}
		set
		{
			this.type = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CBF RID: 3263
	// (get) Token: 0x06007AA9 RID: 31401 RVA: 0x0031B922 File Offset: 0x00319B22
	// (set) Token: 0x06007AAA RID: 31402 RVA: 0x0031B92A File Offset: 0x00319B2A
	public Vector4 Border
	{
		get
		{
			return this.border;
		}
		set
		{
			this.border = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC0 RID: 3264
	// (get) Token: 0x06007AAB RID: 31403 RVA: 0x0031B93A File Offset: 0x00319B3A
	// (set) Token: 0x06007AAC RID: 31404 RVA: 0x0031B942 File Offset: 0x00319B42
	public UIBasicSprite.Flip Flip
	{
		get
		{
			return this.flip;
		}
		set
		{
			this.flip = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC1 RID: 3265
	// (get) Token: 0x06007AAD RID: 31405 RVA: 0x0031B952 File Offset: 0x00319B52
	// (set) Token: 0x06007AAE RID: 31406 RVA: 0x0031B95A File Offset: 0x00319B5A
	public Color Color
	{
		get
		{
			return this.color;
		}
		set
		{
			this.color = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC2 RID: 3266
	// (get) Token: 0x06007AAF RID: 31407 RVA: 0x0031B96A File Offset: 0x00319B6A
	// (set) Token: 0x06007AB0 RID: 31408 RVA: 0x0031B972 File Offset: 0x00319B72
	public UIBasicSprite.FillDirection FillDirection
	{
		get
		{
			return this.fillDirection;
		}
		set
		{
			this.fillDirection = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC3 RID: 3267
	// (get) Token: 0x06007AB1 RID: 31409 RVA: 0x0031B982 File Offset: 0x00319B82
	// (set) Token: 0x06007AB2 RID: 31410 RVA: 0x0031B98A File Offset: 0x00319B8A
	public bool FillCenter
	{
		get
		{
			return this.fillCenter;
		}
		set
		{
			this.fillCenter = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC4 RID: 3268
	// (get) Token: 0x06007AB3 RID: 31411 RVA: 0x0031B99A File Offset: 0x00319B9A
	// (set) Token: 0x06007AB4 RID: 31412 RVA: 0x0031B9A2 File Offset: 0x00319BA2
	public float GlobalOpacityModifier
	{
		get
		{
			return this.globalOpacityModifier;
		}
		set
		{
			this.globalOpacityModifier = value;
			this.isDirty = true;
		}
	}

	// Token: 0x17000CC5 RID: 3269
	// (get) Token: 0x06007AB5 RID: 31413 RVA: 0x0031B9B2 File Offset: 0x00319BB2
	// (set) Token: 0x06007AB6 RID: 31414 RVA: 0x0031B9BA File Offset: 0x00319BBA
	public bool OriginalAspectRatio
	{
		get
		{
			return this.originalAspectRatio;
		}
		set
		{
			this.originalAspectRatio = value;
			this.isDirty = true;
		}
	}

	// Token: 0x06007AB7 RID: 31415 RVA: 0x0031B9CC File Offset: 0x00319BCC
	public XUiV_Texture(string _id) : base(_id)
	{
	}

	// Token: 0x06007AB8 RID: 31416 RVA: 0x0031BA27 File Offset: 0x00319C27
	[PublicizedFrom(EAccessModifier.Protected)]
	public override void CreateComponents(GameObject _go)
	{
		_go.AddComponent<UITexture>();
	}

	// Token: 0x06007AB9 RID: 31417 RVA: 0x0031BA30 File Offset: 0x00319C30
	public void CreateMaterial()
	{
		this.Material = new Material(Shader.Find("Unlit/Transparent Colored Emissive TextureArray"));
		this.isCreatedMaterial = true;
	}

	// Token: 0x06007ABA RID: 31418 RVA: 0x0031BA4E File Offset: 0x00319C4E
	public override void InitView()
	{
		base.InitView();
		this.uiTexture = this.uiTransform.gameObject.GetComponent<UITexture>();
	}

	// Token: 0x06007ABB RID: 31419 RVA: 0x0031BA6C File Offset: 0x00319C6C
	public override void Cleanup()
	{
		base.Cleanup();
		if (this.isCreatedMaterial)
		{
			UnityEngine.Object.Destroy(this.material);
			this.material = null;
			this.isCreatedMaterial = false;
		}
	}

	// Token: 0x06007ABC RID: 31420 RVA: 0x0031889E File Offset: 0x00316A9E
	public override void Update(float _dt)
	{
		base.Update(_dt);
		if (base.xui.GlobalOpacityChanged)
		{
			this.isDirty = true;
		}
	}

	// Token: 0x06007ABD RID: 31421 RVA: 0x0031BA98 File Offset: 0x00319C98
	public override void UpdateData()
	{
		if (!this.wwwAssigned && !string.IsNullOrEmpty(this.pathName) && !this.pathName.StartsWith("@:") && this.pathName.Contains("@"))
		{
			if (!this.www.isDone)
			{
				return;
			}
			if (this.www.result == UnityWebRequest.Result.Success)
			{
				Texture2D texture2D = ((DownloadHandlerTexture)this.www.downloadHandler).texture;
				this.Texture = TextureUtils.CloneTexture(texture2D, false, false, true);
				UnityEngine.Object.DestroyImmediate(texture2D);
			}
			else
			{
				Log.Warning("Retrieving XUiV_Texture file from '" + this.pathName + "' failed: " + this.www.error);
			}
			this.wwwAssigned = true;
		}
		if (!this.isDirty)
		{
			return;
		}
		this.uiTexture.enabled = (this.texture != null);
		this.uiTexture.mainTexture = this.texture;
		this.uiTexture.color = this.color;
		this.uiTexture.keepAspectRatio = this.keepAspectRatio;
		this.uiTexture.aspectRatio = this.aspectRatio;
		this.uiTexture.fixedAspect = this.originalAspectRatio;
		this.uiTexture.SetDimensions(this.size.x, this.size.y);
		this.uiTexture.type = this.type;
		this.uiTexture.border = this.border;
		this.uiTexture.uvRect = this.uvRect;
		this.uiTexture.flip = this.flip;
		this.uiTexture.centerType = (this.fillCenter ? UIBasicSprite.AdvancedType.Sliced : UIBasicSprite.AdvancedType.Invisible);
		this.uiTexture.fillDirection = this.fillDirection;
		this.uiTexture.material = this.material;
		if (this.globalOpacityModifier != 0f && base.xui.ForegroundGlobalOpacity < 1f)
		{
			float a = Mathf.Clamp01(this.color.a * (this.globalOpacityModifier * base.xui.ForegroundGlobalOpacity));
			this.uiTexture.color = new Color(this.color.r, this.color.g, this.color.b, a);
		}
		if (!this.initialized)
		{
			this.initialized = true;
			this.uiTexture.pivot = this.pivot;
			this.uiTexture.depth = this.depth;
			this.uiTransform.localScale = Vector3.one;
			this.uiTransform.localPosition = new Vector3((float)base.PaddedPosition.x, (float)base.PaddedPosition.y, 0f);
			if (this.EventOnHover || this.EventOnPress || this.EventOnScroll || this.EventOnDrag)
			{
				BoxCollider collider = this.collider;
				collider.center = this.uiTexture.localCenter;
				collider.size = new Vector3(this.uiTexture.localSize.x * this.colliderScale, this.uiTexture.localSize.y * this.colliderScale, 0f);
			}
		}
		base.parseAnchors(this.uiTexture, true);
		base.UpdateData();
	}

	// Token: 0x06007ABE RID: 31422 RVA: 0x0031BDDE File Offset: 0x00319FDE
	public void SetTextureDirty()
	{
		this.uiTexture.mainTexture = null;
		base.IsDirty = true;
	}

	// Token: 0x06007ABF RID: 31423 RVA: 0x0031BDF4 File Offset: 0x00319FF4
	public void UnloadTexture()
	{
		if (this.Texture == null)
		{
			return;
		}
		Texture assetToUnload = this.Texture;
		this.uiTexture.mainTexture = null;
		this.Texture = null;
		this.pathName = null;
		this.wwwAssigned = false;
		if (this.www == null)
		{
			Resources.UnloadAsset(assetToUnload);
		}
		this.www = null;
	}

	// Token: 0x06007AC0 RID: 31424 RVA: 0x0031BE50 File Offset: 0x0031A050
	public override bool ParseAttribute(string attribute, string value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(attribute);
		if (num <= 2672033115U)
		{
			if (num <= 1031692888U)
			{
				if (num != 1013213428U)
				{
					if (num == 1031692888U)
					{
						if (attribute == "color")
						{
							this.Color = StringParsers.ParseColor32(value);
							return true;
						}
					}
				}
				else if (attribute == "texture")
				{
					if (this.pathName == value)
					{
						return true;
					}
					this.pathName = value;
					if (this.pathName.Length == 0)
					{
						this.Texture = null;
						return true;
					}
					try
					{
						this.wwwAssigned = false;
						string text = ModManager.PatchModPathString(this.pathName);
						if (text != null)
						{
							this.fetchWwwTexture("file://" + text);
						}
						else if (this.pathName[0] == '@' && this.pathName[1] != ':')
						{
							string text2 = this.pathName.Substring(1);
							if (text2.StartsWith("file:", StringComparison.OrdinalIgnoreCase))
							{
								string text3 = text2.Substring(5);
								if (text3[0] != '/' && text3[0] != '\\')
								{
									text2 = new Uri(((Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXServer) ? (Application.dataPath + "/../../") : (Application.dataPath + "/../")) + text3).AbsoluteUri;
								}
							}
							this.fetchWwwTexture(text2);
						}
						else
						{
							base.xui.LoadData<Texture>(this.pathName, delegate(Texture o)
							{
								this.Texture = o;
							});
						}
					}
					catch (Exception e)
					{
						Log.Error("[XUi] Could not load texture: " + this.pathName);
						Log.Exception(e);
					}
					return true;
				}
			}
			else if (num != 1361572173U)
			{
				if (num == 2672033115U)
				{
					if (attribute == "rect_offset")
					{
						Vector2 vector = StringParsers.ParseVector2(value);
						Rect uvrect = this.uvRect;
						uvrect.x = vector.x;
						uvrect.y = vector.y;
						this.UVRect = uvrect;
						return true;
					}
				}
			}
			else if (attribute == "type")
			{
				this.type = EnumUtils.Parse<UIBasicSprite.Type>(value, true);
				return true;
			}
		}
		else if (num <= 3060355671U)
		{
			if (num != 3007493977U)
			{
				if (num == 3060355671U)
				{
					if (attribute == "globalopacity")
					{
						if (!StringParsers.ParseBool(value, 0, -1, true))
						{
							this.GlobalOpacityModifier = 0f;
						}
						return true;
					}
				}
			}
			else if (attribute == "rect_size")
			{
				Vector2 vector2 = StringParsers.ParseVector2(value);
				Rect uvrect2 = this.uvRect;
				uvrect2.width = vector2.x;
				uvrect2.height = vector2.y;
				this.UVRect = uvrect2;
				return true;
			}
		}
		else if (num != 3538210912U)
		{
			if (num != 4072220735U)
			{
				if (num == 4144336821U)
				{
					if (attribute == "globalopacitymod")
					{
						this.GlobalOpacityModifier = StringParsers.ParseFloat(value, 0, -1, NumberStyles.Any);
						return true;
					}
				}
			}
			else if (attribute == "original_aspect_ratio")
			{
				this.OriginalAspectRatio = StringParsers.ParseBool(value, 0, -1, true);
				return true;
			}
		}
		else if (attribute == "material")
		{
			base.xui.LoadData<Material>(value, delegate(Material o)
			{
				this.material = new Material(o);
			});
			return true;
		}
		return base.ParseAttribute(attribute, value, _parent);
	}

	// Token: 0x06007AC1 RID: 31425 RVA: 0x0031C1FC File Offset: 0x0031A3FC
	[PublicizedFrom(EAccessModifier.Private)]
	public void fetchWwwTexture(string _uri)
	{
		_uri = _uri.Replace("#", "%23").Replace("+", "%2B");
		this.www = UnityWebRequestTexture.GetTexture(_uri);
		this.www.SendWebRequest();
		ThreadManager.StartCoroutine(this.waitForWwwData());
	}

	// Token: 0x06007AC2 RID: 31426 RVA: 0x0031C24E File Offset: 0x0031A44E
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator waitForWwwData()
	{
		while (this.www != null && !this.www.isDone)
		{
			yield return null;
		}
		if (this.www != null)
		{
			this.isDirty = true;
		}
		yield break;
	}

	// Token: 0x04005CD0 RID: 23760
	[PublicizedFrom(EAccessModifier.Protected)]
	public UITexture uiTexture;

	// Token: 0x04005CD1 RID: 23761
	[PublicizedFrom(EAccessModifier.Protected)]
	public Texture texture;

	// Token: 0x04005CD2 RID: 23762
	[PublicizedFrom(EAccessModifier.Protected)]
	public string pathName;

	// Token: 0x04005CD3 RID: 23763
	[PublicizedFrom(EAccessModifier.Protected)]
	public Material material;

	// Token: 0x04005CD4 RID: 23764
	[PublicizedFrom(EAccessModifier.Protected)]
	public Rect uvRect = new Rect(0f, 0f, 1f, 1f);

	// Token: 0x04005CD5 RID: 23765
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Type type;

	// Token: 0x04005CD6 RID: 23766
	[PublicizedFrom(EAccessModifier.Protected)]
	public Vector4 border = Vector4.zero;

	// Token: 0x04005CD7 RID: 23767
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.Flip flip;

	// Token: 0x04005CD8 RID: 23768
	[PublicizedFrom(EAccessModifier.Protected)]
	public Color color = Color.white;

	// Token: 0x04005CD9 RID: 23769
	[PublicizedFrom(EAccessModifier.Protected)]
	public UIBasicSprite.FillDirection fillDirection;

	// Token: 0x04005CDA RID: 23770
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool fillCenter = true;

	// Token: 0x04005CDB RID: 23771
	[PublicizedFrom(EAccessModifier.Private)]
	public bool isCreatedMaterial;

	// Token: 0x04005CDC RID: 23772
	[PublicizedFrom(EAccessModifier.Private)]
	public float globalOpacityModifier = 1f;

	// Token: 0x04005CDD RID: 23773
	[PublicizedFrom(EAccessModifier.Private)]
	public bool originalAspectRatio;

	// Token: 0x04005CDE RID: 23774
	[PublicizedFrom(EAccessModifier.Protected)]
	public UnityWebRequest www;

	// Token: 0x04005CDF RID: 23775
	[PublicizedFrom(EAccessModifier.Protected)]
	public bool wwwAssigned;
}

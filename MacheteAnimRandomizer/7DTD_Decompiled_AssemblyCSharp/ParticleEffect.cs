using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Audio;
using UnityEngine;

// Token: 0x02000802 RID: 2050
public class ParticleEffect
{
	// Token: 0x06003AC9 RID: 15049 RVA: 0x0017AB78 File Offset: 0x00178D78
	public static void Init()
	{
		ParticleEffect.RootT = GameObject.Find("/Particles").transform;
		Origin.Add(ParticleEffect.RootT, 0);
		ParticleEffect.entityParticles.Clear();
	}

	// Token: 0x06003ACA RID: 15050 RVA: 0x0017ABA3 File Offset: 0x00178DA3
	public static IEnumerator LoadResources()
	{
		ParticleEffect.loadedTs.Clear();
		LoadManager.AddressableAssetsRequestTask<GameObject> loadTask = LoadManager.LoadAssetsFromAddressables<GameObject>("particleeffects", delegate(string address)
		{
			if (!address.EndsWith(".prefab"))
			{
				return false;
			}
			StringSpan.CharSplitEnumerator splitEnumerator = address.GetSplitEnumerator('/', StringSplitOptions.None);
			if (!splitEnumerator.MoveNext())
			{
				Log.Error("Particle effect at " + address + " did not have expected folder name");
				return false;
			}
			if (!splitEnumerator.MoveNext())
			{
				Log.Error("Particle effect at " + address + " did not have expected name format");
				return false;
			}
			StringSpan stringSpan = splitEnumerator.Current;
			StringSpan value = ParticleEffect.prefix;
			return stringSpan.IndexOf(value) == 0;
		}, null, false, false);
		while (!loadTask.IsDone)
		{
			yield return null;
		}
		List<GameObject> list = new List<GameObject>();
		loadTask.CollectResults(list);
		foreach (GameObject gameObject in list)
		{
			string text = gameObject.name;
			text = text.Substring(ParticleEffect.prefix.Length);
			int key = ParticleEffect.ToId(text);
			if (ParticleEffect.loadedTs.ContainsKey(key))
			{
				Log.Error("Particle Effect " + text + " already exists! Skipping it!");
			}
			else
			{
				ParticleEffect.loadedTs.Add(key, gameObject.transform);
			}
		}
		yield return null;
		yield break;
	}

	// Token: 0x06003ACB RID: 15051 RVA: 0x0017ABAC File Offset: 0x00178DAC
	public static void LoadAsset(string _path)
	{
		DataLoader.DataPathIdentifier identifier = DataLoader.ParseDataPathIdentifier(_path);
		if (!identifier.IsBundle)
		{
			return;
		}
		int key = ParticleEffect.ToId(_path);
		if (ParticleEffect.loadedTs.ContainsKey(key))
		{
			Log.Warning("Particle Effect {0} already exists! Skipping it!", new object[]
			{
				_path
			});
			return;
		}
		Transform value = DataLoader.LoadAsset<Transform>(identifier, false);
		ParticleEffect.loadedTs.Add(key, value);
	}

	// Token: 0x06003ACC RID: 15052 RVA: 0x0017AC09 File Offset: 0x00178E09
	public ParticleEffect()
	{
	}

	// Token: 0x06003ACD RID: 15053 RVA: 0x0017AC18 File Offset: 0x00178E18
	public ParticleEffect(ParticleType _type, Vector3 _pos, float _lightValue, Color _color)
	{
		this.type = _type;
		this.pos = _pos;
		this.lightValue = _lightValue;
		this.color = _color;
	}

	// Token: 0x06003ACE RID: 15054 RVA: 0x0017AC44 File Offset: 0x00178E44
	public ParticleEffect(ParticleType _type, Vector3 _pos, float _lightValue, Color _color, Transform _parentTransform) : this(_type, _pos, _lightValue, _color)
	{
		this.SetParent(_parentTransform);
	}

	// Token: 0x06003ACF RID: 15055 RVA: 0x0017AC59 File Offset: 0x00178E59
	public ParticleEffect(ParticleType _type, Vector3 _pos, float _lightValue, Color _color, string _soundName, Transform _parentTransform) : this(_type, _pos, _lightValue, _color)
	{
		this.soundName = _soundName;
		this.SetParent(_parentTransform);
	}

	// Token: 0x06003AD0 RID: 15056 RVA: 0x0017AC76 File Offset: 0x00178E76
	public ParticleEffect(string _name, Vector3 _pos, Quaternion _rot, float _lightValue, Color _color) : this(_name, _pos, _rot, _lightValue, _color, null, null)
	{
	}

	// Token: 0x06003AD1 RID: 15057 RVA: 0x0017AC87 File Offset: 0x00178E87
	public ParticleEffect(string _name, Vector3 _pos, Quaternion _rot, float _lightValue, Color _color, string _soundName, Transform _parentTransform) : this(_name, _pos, _lightValue, _color, _soundName, _parentTransform, false)
	{
		this.rot = _rot;
	}

	// Token: 0x06003AD2 RID: 15058 RVA: 0x0017ACA1 File Offset: 0x00178EA1
	public ParticleEffect(string _name, Vector3 _pos, float _lightValue, Color _color, string _soundName, Transform _parentTransform, bool _OLDCreateColliders) : this(ParticleType.Dynamic, _pos, _lightValue, _color)
	{
		this.ParticleId = ((_name != null) ? ParticleEffect.ToId(_name) : 0);
		this.debugName = _name;
		this.soundName = _soundName;
		this.SetParent(_parentTransform);
	}

	// Token: 0x06003AD3 RID: 15059 RVA: 0x0017ACD7 File Offset: 0x00178ED7
	public ParticleEffect(string _name, Vector3 _pos, float _lightValue, Color _color, string _soundName, int _parentEntityId, ParticleEffect.Attachment _attachment) : this(ParticleType.Dynamic, _pos, _lightValue, _color)
	{
		this.ParticleId = ((_name != null) ? ParticleEffect.ToId(_name) : 0);
		this.debugName = _name;
		this.soundName = _soundName;
		this.SetParent(_parentEntityId, _attachment);
	}

	// Token: 0x06003AD4 RID: 15060 RVA: 0x000CCE98 File Offset: 0x000CB098
	public static int ToId(string _name)
	{
		return _name.GetHashCode();
	}

	// Token: 0x06003AD5 RID: 15061 RVA: 0x0017AD10 File Offset: 0x00178F10
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform GetParentTransform()
	{
		if (!this.parentTransform && this.parentEntityId != -1)
		{
			Entity entity;
			GameManager.Instance.World.Entities.dict.TryGetValue(this.parentEntityId, out entity);
			if (entity)
			{
				this.parentTransform = entity.transform;
				if (this.attachment != ParticleEffect.Attachment.None)
				{
					ParticleEffect.Attachment attachment = this.attachment;
					Transform exists;
					if (attachment != ParticleEffect.Attachment.Head)
					{
						if (attachment != ParticleEffect.Attachment.Pelvis)
						{
							exists = null;
						}
						else
						{
							exists = entity.emodel.GetPelvisTransform();
						}
					}
					else
					{
						exists = entity.emodel.GetHeadTransform();
					}
					if (exists)
					{
						this.parentTransform = exists;
					}
				}
			}
		}
		return this.parentTransform;
	}

	// Token: 0x06003AD6 RID: 15062 RVA: 0x0017ADB8 File Offset: 0x00178FB8
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetParent(Transform _parentT)
	{
		this.parentTransform = null;
		this.attachment = ParticleEffect.Attachment.None;
		if (_parentT)
		{
			Entity component = _parentT.GetComponent<Entity>();
			this.parentEntityId = (component ? component.entityId : -1);
		}
	}

	// Token: 0x06003AD7 RID: 15063 RVA: 0x0017ADF9 File Offset: 0x00178FF9
	[PublicizedFrom(EAccessModifier.Private)]
	public void SetParent(int _entityId, ParticleEffect.Attachment _attachment = ParticleEffect.Attachment.None)
	{
		this.parentTransform = null;
		this.attachment = _attachment;
		this.parentEntityId = _entityId;
	}

	// Token: 0x06003AD8 RID: 15064 RVA: 0x0017AE10 File Offset: 0x00179010
	public static Transform GetDynamicTransform(int _particleId)
	{
		Transform result;
		if (ParticleEffect.loadedTs.TryGetValue(_particleId, out result))
		{
			return result;
		}
		Log.Error(string.Format("Unknown particle effect: {0}", _particleId));
		return null;
	}

	// Token: 0x06003AD9 RID: 15065 RVA: 0x0017AE44 File Offset: 0x00179044
	public static bool IsAvailable(string _name)
	{
		return ParticleEffect.loadedTs.ContainsKey(ParticleEffect.ToId(_name));
	}

	// Token: 0x06003ADA RID: 15066 RVA: 0x0017AE58 File Offset: 0x00179058
	public void Read(BinaryReader _br)
	{
		this.ParticleId = _br.ReadInt32();
		this.pos = StreamUtils.ReadVector3(_br);
		this.rot = StreamUtils.ReadQuaterion(_br);
		this.color = StreamUtils.ReadColor32(_br);
		this.soundName = _br.ReadString();
		if (this.soundName == string.Empty)
		{
			this.soundName = null;
		}
		this.parentEntityId = _br.ReadInt32();
		this.attachment = (ParticleEffect.Attachment)_br.ReadByte();
	}

	// Token: 0x06003ADB RID: 15067 RVA: 0x0017AED4 File Offset: 0x001790D4
	public void Write(BinaryWriter _bw)
	{
		_bw.Write(this.ParticleId);
		StreamUtils.Write(_bw, this.pos);
		StreamUtils.Write(_bw, this.rot);
		StreamUtils.WriteColor32(_bw, this.color);
		_bw.Write((this.soundName != null) ? this.soundName : string.Empty);
		_bw.Write(this.parentEntityId);
		_bw.Write((byte)this.attachment);
	}

	// Token: 0x06003ADC RID: 15068 RVA: 0x0017AF44 File Offset: 0x00179144
	public static Transform SpawnParticleEffect(ParticleEffect _pe, int _entityThatCausedIt, bool _forceCreation = false, bool _isWorldPos = false)
	{
		if (_pe.soundName != null && SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
		{
			GameManager.Instance.World.aiDirector.OnSoundPlayedAtPosition(_entityThatCausedIt, _pe.pos, _pe.soundName, 1f);
		}
		if (GameManager.IsDedicatedServer)
		{
			return null;
		}
		if (!string.IsNullOrEmpty(_pe.soundName))
		{
			Manager.Play(_pe.pos, _pe.soundName, _entityThatCausedIt, false);
		}
		List<ParticleEffect.EntityData> list = null;
		Transform transform;
		if (_entityThatCausedIt != -1 && !_forceCreation)
		{
			int num = -1;
			if (ParticleEffect.entityParticles.TryGetValue(_entityThatCausedIt, out list))
			{
				int num2 = 0;
				for (int i = list.Count - 1; i >= 0; i--)
				{
					if (!list[i].t)
					{
						list.RemoveAt(i);
						num--;
					}
					else if (list[i].id == _pe.ParticleId && ++num2 >= 3)
					{
						num = i;
					}
				}
			}
			else
			{
				list = new List<ParticleEffect.EntityData>();
				ParticleEffect.entityParticles[_entityThatCausedIt] = list;
			}
			if (num >= 0 && _pe.attachment == ParticleEffect.Attachment.None)
			{
				ParticleEffect.EntityData entityData = list[num];
				list.RemoveAt(num);
				list.Add(entityData);
				transform = entityData.t;
				transform.position = _pe.pos - Origin.position;
				transform.rotation = _pe.rot;
				foreach (ParticleSystem particleSystem in transform.GetComponentsInChildren<ParticleSystem>())
				{
					particleSystem.Clear();
					particleSystem.Play();
				}
				TemporaryObject[] componentsInChildren2 = transform.GetComponentsInChildren<TemporaryObject>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					componentsInChildren2[j].Restart();
				}
				return null;
			}
		}
		Transform dynamicTransform = ParticleEffect.GetDynamicTransform(_pe.ParticleId);
		if (!dynamicTransform)
		{
			return null;
		}
		if (_isWorldPos)
		{
			transform = UnityEngine.Object.Instantiate<Transform>(dynamicTransform, _pe.pos - Origin.position, _pe.rot);
		}
		else
		{
			transform = UnityEngine.Object.Instantiate<Transform>(dynamicTransform, _pe.pos, _pe.rot);
		}
		if (!transform)
		{
			return null;
		}
		if (list != null)
		{
			ParticleEffect.EntityData item;
			item.id = _pe.ParticleId;
			item.t = transform;
			list.Add(item);
		}
		foreach (Renderer renderer in transform.GetComponentsInChildren<Renderer>())
		{
			if (renderer.GetComponent<ParticleSystem>() == null)
			{
				renderer.material.SetColor("_Color", _pe.color);
			}
		}
		if (_pe.opqueTextureId != 0)
		{
			Material material = transform.GetComponent<ParticleSystem>().GetComponent<Renderer>().material;
			TextureAtlas textureAtlas = MeshDescription.meshes[0].textureAtlas;
			material.SetTexture("_MainTex", textureAtlas.diffuseTexture);
			material.SetTexture("_BumpMap", textureAtlas.normalTexture);
			material.SetFloat("_TexI", (float)textureAtlas.uvMapping[_pe.opqueTextureId].index);
			if (material.HasProperty("_OffsetUV"))
			{
				Rect uv = textureAtlas.uvMapping[_pe.opqueTextureId].uv;
				material.SetVector("_OffsetUV", new Vector4(uv.x, uv.y, uv.width, uv.height));
			}
		}
		Transform transform2 = _pe.GetParentTransform();
		if (transform2)
		{
			transform.SetParent(transform2, false);
			if (_pe.attachment != ParticleEffect.Attachment.None)
			{
				transform.localPosition = _pe.pos;
			}
			else
			{
				transform.localPosition = Vector3.zero;
			}
			transform.localRotation = Quaternion.identity;
		}
		else
		{
			transform.SetParent(ParticleEffect.RootT, false);
		}
		return transform;
	}

	// Token: 0x04002FD0 RID: 12240
	[PublicizedFrom(EAccessModifier.Private)]
	public static string prefix = "p_";

	// Token: 0x04002FD1 RID: 12241
	[PublicizedFrom(EAccessModifier.Private)]
	public const int cEntitySameParticleMax = 3;

	// Token: 0x04002FD2 RID: 12242
	[PublicizedFrom(EAccessModifier.Private)]
	public ParticleType type;

	// Token: 0x04002FD3 RID: 12243
	[PublicizedFrom(EAccessModifier.Private)]
	public ParticleEffect.Attachment attachment;

	// Token: 0x04002FD4 RID: 12244
	public Vector3 pos;

	// Token: 0x04002FD5 RID: 12245
	public Quaternion rot;

	// Token: 0x04002FD6 RID: 12246
	public Color color;

	// Token: 0x04002FD7 RID: 12247
	public float lightValue;

	// Token: 0x04002FD8 RID: 12248
	public int ParticleId;

	// Token: 0x04002FD9 RID: 12249
	public string soundName;

	// Token: 0x04002FDA RID: 12250
	public int opqueTextureId;

	// Token: 0x04002FDB RID: 12251
	[PublicizedFrom(EAccessModifier.Private)]
	public int parentEntityId = -1;

	// Token: 0x04002FDC RID: 12252
	[PublicizedFrom(EAccessModifier.Private)]
	public Transform parentTransform;

	// Token: 0x04002FDD RID: 12253
	public string debugName;

	// Token: 0x04002FDE RID: 12254
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform RootT;

	// Token: 0x04002FDF RID: 12255
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, Transform> loadedTs = new Dictionary<int, Transform>();

	// Token: 0x04002FE0 RID: 12256
	[PublicizedFrom(EAccessModifier.Private)]
	public static Dictionary<int, List<ParticleEffect.EntityData>> entityParticles = new Dictionary<int, List<ParticleEffect.EntityData>>();

	// Token: 0x02000803 RID: 2051
	public enum Attachment : byte
	{
		// Token: 0x04002FE2 RID: 12258
		None,
		// Token: 0x04002FE3 RID: 12259
		Head,
		// Token: 0x04002FE4 RID: 12260
		Pelvis
	}

	// Token: 0x02000804 RID: 2052
	public struct EntityData
	{
		// Token: 0x04002FE5 RID: 12261
		public int id;

		// Token: 0x04002FE6 RID: 12262
		public Transform t;
	}
}

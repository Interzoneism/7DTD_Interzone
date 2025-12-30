using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

// Token: 0x02000013 RID: 19
public static class CharacterConstructUtils
{
	// Token: 0x06000052 RID: 82 RVA: 0x00006ED8 File Offset: 0x000050D8
	public static GameObject Stitch(GameObject sourceObj, GameObject parentObj, SDCSUtils.TransformCatalog boneCatalog, Material eyeMat = null)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceObj, parentObj.transform);
		gameObject.name = sourceObj.name;
		gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(CharacterConstructUtils.tempSMRs);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
		{
			string name = skinnedMeshRenderer.gameObject.name;
			skinnedMeshRenderer.bones = CharacterConstructUtils.TranslateTransforms(skinnedMeshRenderer.bones, boneCatalog);
			skinnedMeshRenderer.rootBone = CharacterConstructUtils.Find<string, Transform>(boneCatalog, skinnedMeshRenderer.rootBone.name);
			skinnedMeshRenderer.updateWhenOffscreen = true;
			Material[] sharedMaterials = skinnedMeshRenderer.sharedMaterials;
			for (int i = 0; i < sharedMaterials.Length; i++)
			{
				Material material = sharedMaterials[i];
				if (material)
				{
					string name2 = material.name;
					if (name2.Contains("_Body"))
					{
						Material material2 = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false);
						sharedMaterials[i] = (material2 ? material2 : sharedMaterials[i]);
					}
					else if (name2.Contains("_Head"))
					{
						Material material3 = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHeadMatLoc, false);
						sharedMaterials[i] = (material3 ? material3 : sharedMaterials[i]);
					}
					else if (name2.Contains("_Hand"))
					{
						Material material4 = DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHandsMatLoc, false);
						sharedMaterials[i] = (material4 ? material4 : sharedMaterials[i]);
					}
				}
			}
			if (name == "eyes" && eyeMat)
			{
				sharedMaterials[0] = eyeMat;
			}
			skinnedMeshRenderer.sharedMaterials = sharedMaterials;
		}
		CharacterConstructUtils.tempSMRs.Clear();
		Transform transform = boneCatalog["Hips"];
		gameObject.GetComponentsInChildren<Cloth>(CharacterConstructUtils.tempCloths);
		foreach (Cloth cloth in CharacterConstructUtils.tempCloths)
		{
			cloth.capsuleColliders = transform.GetComponentsInChildren<CapsuleCollider>();
		}
		CharacterConstructUtils.tempCloths.Clear();
		return gameObject;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00007110 File Offset: 0x00005310
	public static void MatchRigs(SDCSUtils.SlotData wornItem, Transform source, Transform target, SDCSUtils.TransformCatalog transformCatalog)
	{
		Transform transform = source.Find("Origin");
		Transform transform2 = target.Find("Origin");
		if (!transform || !transform2)
		{
			return;
		}
		CharacterConstructUtils.AddMissingChildren(wornItem, transform, transform2, transformCatalog);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00007150 File Offset: 0x00005350
	[PublicizedFrom(EAccessModifier.Private)]
	public static void AddMissingChildren(SDCSUtils.SlotData wornItem, Transform sourceT, Transform targetT, SDCSUtils.TransformCatalog transformCatalog)
	{
		if (!sourceT || !targetT)
		{
			return;
		}
		foreach (object obj in sourceT)
		{
			Transform transform = (Transform)obj;
			string name = transform.name;
			Transform transform2 = null;
			bool flag = false;
			foreach (object obj2 in targetT)
			{
				Transform transform3 = (Transform)obj2;
				string name2 = transform3.name;
				if (name2 == name)
				{
					transform2 = transform3;
					flag = true;
					if (!transformCatalog.ContainsKey(name2))
					{
						transformCatalog.Add(name2, transform2);
						break;
					}
					break;
				}
			}
			if (!flag)
			{
				transform2 = UnityEngine.Object.Instantiate<GameObject>(transform.gameObject).transform;
				transform2.SetParent(targetT, false);
				transform2.name = name;
				transformCatalog[name] = transform2;
			}
			if (!flag)
			{
				transform2.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
				transform2.localScale = transform.localScale;
			}
			CharacterConstructUtils.TransferCharacterJoint(transform, transform2.gameObject, transformCatalog);
			CharacterConstructUtils.AddMissingChildren(wornItem, transform, transform2, transformCatalog);
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000729C File Offset: 0x0000549C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupRigConstraints(RigBuilder rigBuilder, Transform sourceRootT, Transform targetRootT, SDCSUtils.TransformCatalog transformCatalog)
	{
		if (!sourceRootT.GetComponent<RigBuilder>())
		{
			return;
		}
		Transform transform = sourceRootT.Find("RigConstraints");
		if (!transform)
		{
			return;
		}
		string text = transform.name + "_" + transform.parent.name;
		Transform transform2 = targetRootT.Find(text);
		if (!transform2)
		{
			transform2 = UnityEngine.Object.Instantiate<Transform>(transform, targetRootT);
			transform2.name = text;
			transform2.SetLocalPositionAndRotation(transform.localPosition, transform.localRotation);
			transform2.localScale = transform.localScale;
		}
		Rig component = transform2.GetComponent<Rig>();
		if (!component)
		{
			return;
		}
		rigBuilder.layers.Add(new RigLayer(component, true));
		BlendConstraint[] componentsInChildren = transform.GetComponentsInChildren<BlendConstraint>();
		foreach (BlendConstraint blendConstraint in transform2.GetComponentsInChildren<BlendConstraint>())
		{
			string name = blendConstraint.name;
			foreach (BlendConstraint blendConstraint2 in componentsInChildren)
			{
				if (blendConstraint2.name == name)
				{
					blendConstraint.data.constrainedObject = CharacterConstructUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.constrainedObject.name);
					blendConstraint.data.sourceObjectA = CharacterConstructUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.sourceObjectA.name);
					blendConstraint.data.sourceObjectB = CharacterConstructUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.sourceObjectB.name);
					break;
				}
			}
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x00007420 File Offset: 0x00005620
	[PublicizedFrom(EAccessModifier.Private)]
	public static void TransferCharacterJoint(Transform source, GameObject newBone, SDCSUtils.TransformCatalog transformCatalog)
	{
		CharacterJoint component;
		CharacterJoint characterJoint;
		if ((component = source.GetComponent<CharacterJoint>()) != null && (characterJoint = newBone.AddMissingComponent<CharacterJoint>()) != null)
		{
			Joint joint = characterJoint;
			Transform transform = CharacterConstructUtils.Find<string, Transform>(transformCatalog, component.connectedBody.name);
			joint.connectedBody = ((transform != null) ? transform.GetComponent<Rigidbody>() : null);
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x00007470 File Offset: 0x00005670
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform[] TranslateTransforms(Transform[] transforms, SDCSUtils.TransformCatalog transformCatalog)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform)
			{
				transforms[i] = CharacterConstructUtils.Find<string, Transform>(transformCatalog, transform.name);
			}
			else
			{
				Log.Error("Null transform in bone list");
			}
		}
		return transforms;
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000074B4 File Offset: 0x000056B4
	public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
	{
		TValue result;
		source.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x06000059 RID: 89 RVA: 0x000074CC File Offset: 0x000056CC
	public static string baseBodyLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Common/Meshes/player",
				CharacterConstructUtils.archetype.Sex,
				".fbx"
			});
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600005A RID: 90 RVA: 0x0000750C File Offset: 0x0000570C
	public static string baseHeadLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Meshes/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				".fbx"
			});
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600005B RID: 91 RVA: 0x000075B4 File Offset: 0x000057B4
	public static string baseHairLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Hair/",
				CharacterConstructUtils.archetype.Hair,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x0600005C RID: 92 RVA: 0x00007624 File Offset: 0x00005824
	public static string baseMustacheLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Mustache/",
				CharacterConstructUtils.archetype.MustacheName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x0600005D RID: 93 RVA: 0x00007694 File Offset: 0x00005894
	public static string baseChopsLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Chops/",
				CharacterConstructUtils.archetype.ChopsName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x0600005E RID: 94 RVA: 0x00007704 File Offset: 0x00005904
	public static string baseBeardLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/FacialHair/Beard/",
				CharacterConstructUtils.archetype.BeardName,
				"/HairMorphMatrix/",
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x0600005F RID: 95 RVA: 0x00007772 File Offset: 0x00005972
	public static string baseHairColorLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/HairColorSwatches";
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000060 RID: 96 RVA: 0x00007779 File Offset: 0x00005979
	public static string baseEyeColorMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/Eyes/Materials/" + CharacterConstructUtils.archetype.EyeColorName + ".mat";
		}
	}

	// Token: 0x17000012 RID: 18
	// (get) Token: 0x06000061 RID: 97 RVA: 0x00007794 File Offset: 0x00005994
	public static string baseBodyMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Body.mat"
			});
		}
	}

	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000062 RID: 98 RVA: 0x0000783C File Offset: 0x00005A3C
	public static string baseHeadMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Head.mat"
			});
		}
	}

	// Token: 0x17000014 RID: 20
	// (get) Token: 0x06000063 RID: 99 RVA: 0x000078E4 File Offset: 0x00005AE4
	public static string baseHandsMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				CharacterConstructUtils.archetype.Sex,
				"/Heads/",
				CharacterConstructUtils.archetype.Race,
				"/",
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"/Materials/player",
				CharacterConstructUtils.archetype.Sex,
				CharacterConstructUtils.archetype.Race,
				CharacterConstructUtils.archetype.Variant.ToString("00"),
				"_Hand.mat"
			});
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x06000064 RID: 100 RVA: 0x00007989 File Offset: 0x00005B89
	public static string baseRigPrefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/BaseRigs/baseRigPrefab.prefab";
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x06000065 RID: 101 RVA: 0x00007990 File Offset: 0x00005B90
	public static RuntimeAnimatorController UIAnimController
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return DataLoader.LoadAsset<RuntimeAnimatorController>("@:Entities/Player/Common/AnimControllers/MenuSDCSController.controller", false);
		}
	}

	// Token: 0x06000066 RID: 102 RVA: 0x000079A0 File Offset: 0x00005BA0
	public static void CreateViz(Archetype _archetype, ref GameObject baseRigUI, ref SDCSUtils.TransformCatalog boneCatalogUI)
	{
		CharacterConstructUtils.DestroyViz(baseRigUI, false);
		CharacterConstructUtils.archetype = _archetype;
		CharacterConstructUtils.setupRig(ref baseRigUI, ref boneCatalogUI, CharacterConstructUtils.baseRigPrefab, null, CharacterConstructUtils.UIAnimController);
		CharacterConstructUtils.setupBase(baseRigUI, boneCatalogUI, CharacterConstructUtils.baseParts);
		CharacterConstructUtils.setupHairObjects(baseRigUI, boneCatalogUI, _archetype.Hair, _archetype.MustacheName, _archetype.ChopsName, _archetype.BeardName);
		CharacterConstructUtils.setupEquipment(baseRigUI, boneCatalogUI, _archetype.Equipment, false);
		Transform transform = baseRigUI.transform.Find("IKRig");
		if (transform != null)
		{
			transform.GetComponent<Rig>().weight = 0f;
		}
		foreach (HingeJoint hingeJoint in baseRigUI.GetComponentsInChildren<HingeJoint>())
		{
			if (hingeJoint.connectedBody == null)
			{
				Log.Warning("SDCSUtils::CreateVizUI: No connected body for " + hingeJoint.transform.name + "'s HingeJoint! Disabling for UI until this is solved.");
				hingeJoint.gameObject.SetActive(false);
			}
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x00007A8C File Offset: 0x00005C8C
	public static void DestroyViz(GameObject _baseRigUI, bool _keepRig = false)
	{
		if (_baseRigUI)
		{
			Transform transform = _baseRigUI.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name != "Origin")
				{
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, CharacterConstructUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
					{
						Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
						if (MeshMorph.IsInstance(sharedMesh))
						{
							UnityEngine.Object.Destroy(sharedMesh);
						}
						skinnedMeshRenderer.GetSharedMaterials(CharacterConstructUtils.tempMats);
						Utils.CleanupMaterials<List<Material>>(CharacterConstructUtils.tempMats);
						CharacterConstructUtils.tempMats.Clear();
					}
				}
			}
			if (!_keepRig)
			{
				UnityEngine.Object.DestroyImmediate(_baseRigUI);
			}
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00007B64 File Offset: 0x00005D64
	public static void SetVisible(GameObject _baseRigUI, bool _visible)
	{
		if (_baseRigUI)
		{
			Transform transform = _baseRigUI.transform;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name != "Origin")
				{
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, CharacterConstructUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in CharacterConstructUtils.tempSMRs)
					{
						skinnedMeshRenderer.gameObject.SetActive(_visible);
					}
				}
			}
		}
	}

	// Token: 0x06000069 RID: 105 RVA: 0x00007C00 File Offset: 0x00005E00
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupRig(ref GameObject _rigObj, ref SDCSUtils.TransformCatalog _boneCatalog, string prefabLocation, Transform parent, RuntimeAnimatorController animController)
	{
		if (!_rigObj)
		{
			_rigObj = UnityEngine.Object.Instantiate<GameObject>(DataLoader.LoadAsset<GameObject>(prefabLocation, false), parent);
			_boneCatalog = new SDCSUtils.TransformCatalog(_rigObj.transform);
			Animator component = _rigObj.GetComponent<Animator>();
			if (component && component.runtimeAnimatorController != animController)
			{
				component.runtimeAnimatorController = animController;
			}
			BoneRenderer[] componentsInChildren = _rigObj.GetComponentsInChildren<BoneRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].enabled = false;
			}
		}
		else
		{
			CharacterConstructUtils.cleanupEquipment(_rigObj);
		}
		if (!CharacterConstructUtils.archetype.IsMale)
		{
			CapsuleCollider orAddComponent = _boneCatalog["Hips"].gameObject.GetOrAddComponent<CapsuleCollider>();
			orAddComponent.center = new Vector3(0f, 0f, -0.03f);
			orAddComponent.radius = 0.15f;
			orAddComponent.height = 0.375f;
		}
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00007CD4 File Offset: 0x00005ED4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void cleanupEquipment(GameObject _rigObj)
	{
		RigBuilder component = _rigObj.GetComponent<RigBuilder>();
		if (component)
		{
			List<RigLayer> layers = component.layers;
			for (int i = layers.Count - 1; i >= 0; i--)
			{
				if (layers[i].name != "IKRig")
				{
					layers.RemoveAt(i);
				}
			}
			component.Clear();
		}
		Animator component2 = _rigObj.GetComponent<Animator>();
		if (component2)
		{
			component2.UnbindAllStreamHandles();
		}
		GameUtils.DestroyAllChildrenBut(_rigObj.transform, new List<string>
		{
			"Origin",
			"IKRig"
		});
	}

	// Token: 0x0600006B RID: 107 RVA: 0x00007D6C File Offset: 0x00005F6C
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupBase(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] baseParts)
	{
		foreach (string text in baseParts)
		{
			GameObject gameObject;
			if (text == "head")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseHeadLoc, false);
			}
			else if (text == "hands")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false);
			}
			else
			{
				gameObject = DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false);
			}
			if (gameObject == null)
			{
				return;
			}
			GameObject bodyPartContainingName;
			if (!((bodyPartContainingName = CharacterConstructUtils.getBodyPartContainingName(gameObject.transform, text)) == null))
			{
				bodyPartContainingName.name = text;
				CharacterConstructUtils.Stitch(bodyPartContainingName, _rig, _boneCatalog, DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseEyeColorMatLoc, false));
				if (text == "head")
				{
					Transform transform = _rig.transform;
					Transform transform2 = gameObject.transform;
					CharacterGazeController orAddComponent = transform.FindRecursive("Head").gameObject.GetOrAddComponent<CharacterGazeController>();
					orAddComponent.rootTransform = transform.FindRecursive("Origin");
					orAddComponent.neckTransform = _boneCatalog["Neck"];
					orAddComponent.headTransform = _boneCatalog["Head"];
					orAddComponent.leftEyeTransform = _boneCatalog["LeftEye"];
					orAddComponent.rightEyeTransform = _boneCatalog["RightEye"];
					orAddComponent.eyeMaterial = transform.FindRecursive("eyes").GetComponent<SkinnedMeshRenderer>().material;
					orAddComponent.leftEyeLocalPosition = transform2.FindInChildren("LeftEye").localPosition;
					orAddComponent.rightEyeLocalPosition = transform2.FindInChildren("RightEye").localPosition;
					orAddComponent.eyeLookAtTargetAngle = 35f;
					orAddComponent.eyeRotationSpeed = 30f;
					orAddComponent.twitchSpeed = 25f;
					orAddComponent.headLookAtTargetAngle = 45f;
					orAddComponent.headRotationSpeed = 7f;
					orAddComponent.maxLookAtDistance = 5f;
					EyeLidController orAddComponent2 = transform.FindRecursive("Head").gameObject.GetOrAddComponent<EyeLidController>();
					orAddComponent2.leftTopTransform = _boneCatalog["LeftEyelidTop"];
					orAddComponent2.leftBottomTransform = _boneCatalog["LeftEyelidBot"];
					orAddComponent2.rightTopTransform = _boneCatalog["RightEyelidTop"];
					orAddComponent2.rightBottomTransform = _boneCatalog["RightEyelidBot"];
					orAddComponent2.leftTopLocalPosition = transform2.FindInChildren("LeftEyelidTop").localPosition;
					orAddComponent2.leftBottomLocalPosition = transform2.FindInChildren("LeftEyelidBot").localPosition;
					orAddComponent2.leftTopRotation = transform2.FindInChildren("LeftEyelidTop").localRotation;
					orAddComponent2.leftBottomRotation = transform2.FindInChildren("LeftEyelidBot").localRotation;
					orAddComponent2.rightTopLocalPosition = transform2.FindInChildren("RightEyelidTop").localPosition;
					orAddComponent2.rightBottomLocalPosition = transform2.FindInChildren("RightEyelidBot").localPosition;
					orAddComponent2.rightTopRotation = transform2.FindInChildren("RightEyelidTop").localRotation;
					orAddComponent2.rightBottomRotation = transform2.FindInChildren("RightEyelidBot").localRotation;
				}
			}
		}
	}

	// Token: 0x0600006C RID: 108 RVA: 0x00008044 File Offset: 0x00006244
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipment(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, List<SDCSUtils.SlotData> slotData, bool _ignoreDlcEntitlements)
	{
		if (slotData == null)
		{
			return;
		}
		List<Transform> allGears = new List<Transform>();
		Transform transform = _rig.transform.Find("Origin");
		if (transform)
		{
			List<Transform> list = CharacterConstructUtils.findStartsWith(transform, "RigConstraints");
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		foreach (SDCSUtils.SlotData slotData2 in slotData)
		{
			if ("head".Equals(slotData2.PartName, StringComparison.OrdinalIgnoreCase) && slotData2.PrefabName != null && slotData2.PrefabName.Contains("HeadGearMorphMatrix", StringComparison.OrdinalIgnoreCase))
			{
				CharacterConstructUtils.setupHeadgearMorph(_rig, _boneCatalog, slotData2, _ignoreDlcEntitlements);
			}
			else
			{
				Transform transform2 = CharacterConstructUtils.setupEquipmentSlot(_rig, _boneCatalog, slotData2, allGears, _ignoreDlcEntitlements);
				if (transform2)
				{
					float cullDistance = slotData2.CullDistance;
					Morphable componentInChildren = CharacterConstructUtils.Stitch(transform2.gameObject, _rig, _boneCatalog, null).GetComponentInChildren<Morphable>();
					if (componentInChildren)
					{
						componentInChildren.MorphHeadgear(CharacterConstructUtils.archetype, _ignoreDlcEntitlements);
					}
				}
			}
		}
		List<RigBuilder> rbs = new List<RigBuilder>();
		CharacterConstructUtils.setupEquipmentCommon(_rig, _boneCatalog, allGears, rbs);
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00008184 File Offset: 0x00006384
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform setupEquipmentSlot(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, SDCSUtils.SlotData wornItem, List<Transform> allGears, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = CharacterConstructUtils.GetPathForSlotData(wornItem, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return null;
		}
		GameObject gameObject = DataLoader.LoadAsset<GameObject>(pathForSlotData, _ignoreDlcEntitlements);
		if (gameObject == null && wornItem.PartName == "head")
		{
			pathForSlotData = CharacterConstructUtils.GetPathForSlotData(wornItem, false);
			gameObject = DataLoader.LoadAsset<GameObject>(pathForSlotData, _ignoreDlcEntitlements);
		}
		if (!gameObject)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				pathForSlotData,
				" not found for item ",
				wornItem.PrefabName,
				"!"
			}));
			return null;
		}
		CharacterConstructUtils.MatchRigs(wornItem, gameObject.transform, _rig.transform, _boneCatalog);
		if (!allGears.Contains(gameObject.transform))
		{
			allGears.Add(gameObject.transform);
		}
		Transform clothingPartWithName = CharacterConstructUtils.getClothingPartWithName(gameObject, CharacterConstructUtils.parseSexedLocation(wornItem.PartName, CharacterConstructUtils.archetype.Sex));
		if (clothingPartWithName)
		{
			string baseToTurnOff = wornItem.BaseToTurnOff;
			if (baseToTurnOff != null && baseToTurnOff.Length > 0)
			{
				foreach (string name in wornItem.BaseToTurnOff.Split(',', StringSplitOptions.None))
				{
					Transform transform = _rig.transform.FindInChildren(name);
					if (transform)
					{
						UnityEngine.Object.Destroy(transform.gameObject);
					}
				}
			}
			if (!clothingPartWithName.gameObject.activeSelf)
			{
				clothingPartWithName.gameObject.SetActive(true);
			}
		}
		return clothingPartWithName;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x000082E4 File Offset: 0x000064E4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipmentCommon(GameObject _rigObj, SDCSUtils.TransformCatalog _boneCatalog, List<Transform> allGears, List<RigBuilder> rbs)
	{
		RigBuilder rigBuilder = _rigObj.GetComponent<RigBuilder>();
		if (!rigBuilder)
		{
			rigBuilder = _rigObj.AddComponent<RigBuilder>();
		}
		rigBuilder.enabled = false;
		rbs.Add(rigBuilder);
		foreach (Transform sourceRootT in allGears)
		{
			CharacterConstructUtils.SetupRigConstraints(rigBuilder, sourceRootT, _rigObj.transform, _boneCatalog);
		}
		foreach (HingeJoint hingeJoint in _rigObj.GetComponentsInChildren<HingeJoint>())
		{
			if (hingeJoint.connectedBody != null && _boneCatalog.ContainsKey(hingeJoint.connectedBody.transform.name))
			{
				hingeJoint.connectedBody = _boneCatalog[hingeJoint.connectedBody.transform.name].GetComponent<Rigidbody>();
			}
			hingeJoint.autoConfigureConnectedAnchor = true;
		}
		rigBuilder.enabled = true;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x000083D8 File Offset: 0x000065D8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHairObjects(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string hairName, string mustacheName, string chopsName, string beardName)
	{
		HairColorSwatch hairColorSwatch = null;
		if (!string.IsNullOrEmpty(CharacterConstructUtils.archetype.HairColor))
		{
			ScriptableObject scriptableObject = DataLoader.LoadAsset<ScriptableObject>(CharacterConstructUtils.baseHairColorLoc + "/" + CharacterConstructUtils.archetype.HairColor + ".asset", false);
			if (!(scriptableObject == null))
			{
				hairColorSwatch = (scriptableObject as HairColorSwatch);
			}
		}
		bool flag = false;
		bool flag2 = true;
		if (CharacterConstructUtils.archetype.Equipment != null && CharacterConstructUtils.archetype.Equipment.Count > 0)
		{
			foreach (SDCSUtils.SlotData slotData in CharacterConstructUtils.archetype.Equipment)
			{
				if (slotData.PartName == "head")
				{
					flag = (slotData.HairMaskType == SDCSUtils.SlotData.HairMaskTypes.Hat);
					flag2 = (slotData.FacialHairMaskType != SDCSUtils.SlotData.HairMaskTypes.None);
				}
			}
		}
		if (!string.IsNullOrEmpty(hairName))
		{
			if (flag)
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseHairLoc + "/hair_" + hairName + "_hat.asset", hairName);
			}
			else
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseHairLoc + "/hair_" + hairName + ".asset", hairName);
			}
		}
		if (flag2)
		{
			if (!string.IsNullOrEmpty(mustacheName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseMustacheLoc + "/hair_facial_mustache" + mustacheName + ".asset", mustacheName);
			}
			if (!string.IsNullOrEmpty(chopsName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseChopsLoc + "/hair_facial_sideburns" + chopsName + ".asset", chopsName);
			}
			if (!string.IsNullOrEmpty(beardName))
			{
				CharacterConstructUtils.setupHair(_rig, _boneCatalog, CharacterConstructUtils.baseBeardLoc + "/hair_facial_beard" + beardName + ".asset", beardName);
			}
		}
		if (hairColorSwatch != null)
		{
			CharacterConstructUtils.ApplySwatchToGameObject(_rig, hairColorSwatch);
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00008590 File Offset: 0x00006790
	[PublicizedFrom(EAccessModifier.Private)]
	public static void ApplySwatchToGameObject(GameObject targetGameObject, HairColorSwatch hairSwatch)
	{
		if (targetGameObject != null)
		{
			foreach (Renderer renderer in targetGameObject.GetComponentsInChildren<Renderer>(true))
			{
				Material[] array;
				if (Application.isPlaying)
				{
					array = renderer.materials;
				}
				else
				{
					array = renderer.sharedMaterials;
				}
				foreach (Material material in array)
				{
					if (material.shader.name == "Game/SDCS/Hair" && !material.name.Contains("lashes"))
					{
						hairSwatch.ApplyToMaterial(material);
					}
				}
			}
			return;
		}
		Debug.LogWarning("No target GameObject selected.");
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00008638 File Offset: 0x00006838
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHair(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string path, string hairName)
	{
		if (string.IsNullOrEmpty(hairName))
		{
			return;
		}
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(path, false);
		GameObject gameObject = (meshMorph != null) ? meshMorph.GetMorphedSkinnedMesh() : null;
		if (gameObject == null)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				path,
				" not found for hair ",
				hairName,
				"!"
			}));
			return;
		}
		CharacterConstructUtils.MatchRigs(null, gameObject.transform, _rig.transform, _boneCatalog);
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		CharacterConstructUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, null);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x06000072 RID: 114 RVA: 0x000086E0 File Offset: 0x000068E0
	[PublicizedFrom(EAccessModifier.Private)]
	public static string GetPathForSlotData(SDCSUtils.SlotData _slotData, bool _headgearShortHairMask = false)
	{
		if (_slotData == null)
		{
			return null;
		}
		if (string.IsNullOrEmpty(_slotData.PartName))
		{
			return null;
		}
		if (_slotData.PartName.Equals("head", StringComparison.OrdinalIgnoreCase))
		{
			string text = _slotData.PrefabName;
			if (text.Contains("{sex}"))
			{
				text = text.Replace("{sex}", CharacterConstructUtils.archetype.Sex);
			}
			if (text.Contains("{race}"))
			{
				text = text.Replace("{race}", CharacterConstructUtils.archetype.Race);
			}
			if (text.Contains("{variant}"))
			{
				text = text.Replace("{variant}", CharacterConstructUtils.archetype.Variant.ToString("00"));
			}
			if (text.Contains("{hair}"))
			{
				if (_headgearShortHairMask && (string.IsNullOrEmpty(CharacterConstructUtils.archetype.Hair) || CharacterConstructUtils.shortHairNames.ContainsCaseInsensitive(CharacterConstructUtils.archetype.Hair)))
				{
					text = text.Replace("{hair}", "Bald");
				}
				else
				{
					text = text.Replace("{hair}", "");
				}
			}
			return text;
		}
		if (string.IsNullOrEmpty(_slotData.PrefabName))
		{
			return null;
		}
		return CharacterConstructUtils.parseSexedLocation(_slotData.PrefabName, CharacterConstructUtils.archetype.Sex);
	}

	// Token: 0x06000073 RID: 115 RVA: 0x00008814 File Offset: 0x00006A14
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHeadgearMorph(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, SDCSUtils.SlotData _slotData, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = CharacterConstructUtils.GetPathForSlotData(_slotData, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return;
		}
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(pathForSlotData, _ignoreDlcEntitlements);
		if (meshMorph == null)
		{
			pathForSlotData = CharacterConstructUtils.GetPathForSlotData(_slotData, false);
			meshMorph = DataLoader.LoadAsset<MeshMorph>(pathForSlotData, _ignoreDlcEntitlements);
		}
		GameObject gameObject = (meshMorph != null) ? meshMorph.GetMorphedSkinnedMesh() : null;
		if (gameObject == null)
		{
			Log.Warning(string.Concat(new string[]
			{
				"SDCSUtils::",
				pathForSlotData,
				" not found for headgear ",
				_slotData.PrefabName,
				"!"
			}));
			return;
		}
		CharacterConstructUtils.MatchRigs(null, gameObject.transform, _rig.transform, _boneCatalog);
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false);
		CharacterConstructUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, null);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x06000074 RID: 116 RVA: 0x000088EC File Offset: 0x00006AEC
	public static bool BasePartsExist(Archetype _archetype)
	{
		CharacterConstructUtils.archetype = _archetype;
		if (!DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseBodyLoc, false))
		{
			Log.Error("base body not found at " + CharacterConstructUtils.baseBodyLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<GameObject>(CharacterConstructUtils.baseHeadLoc, false))
		{
			Log.Error("base head not found at " + CharacterConstructUtils.baseHeadLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseBodyMatLoc, false))
		{
			Log.Error("body material not found at " + CharacterConstructUtils.baseBodyMatLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(CharacterConstructUtils.baseHeadMatLoc, false))
		{
			Log.Error("head material not found at " + CharacterConstructUtils.baseHeadMatLoc);
			return false;
		}
		return true;
	}

	// Token: 0x06000075 RID: 117 RVA: 0x000089A0 File Offset: 0x00006BA0
	[PublicizedFrom(EAccessModifier.Private)]
	public static List<Transform> findStartsWith(Transform parent, string key)
	{
		List<Transform> list = new List<Transform>();
		foreach (object obj in parent)
		{
			Transform transform = (Transform)obj;
			if (transform.name.StartsWith(key))
			{
				list.Add(transform);
			}
		}
		return list;
	}

	// Token: 0x06000076 RID: 118 RVA: 0x00008A0C File Offset: 0x00006C0C
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject getBodyPartContainingName(Transform parent, string name)
	{
		foreach (object obj in parent.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name.ToLower().Contains(name))
			{
				return transform.gameObject;
			}
		}
		return null;
	}

	// Token: 0x06000077 RID: 119 RVA: 0x00008A80 File Offset: 0x00006C80
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform getClothingPartWithName(GameObject clothingPrefab, string partName)
	{
		foreach (object obj in clothingPrefab.transform)
		{
			Transform transform = (Transform)obj;
			if (transform.name.ToLower() == partName.ToLower())
			{
				return transform;
			}
		}
		return null;
	}

	// Token: 0x06000078 RID: 120 RVA: 0x00008AF4 File Offset: 0x00006CF4
	[PublicizedFrom(EAccessModifier.Private)]
	public static string parseSexedLocation(string sexedLocation, string sex)
	{
		return sexedLocation.Replace("{sex}", sex);
	}

	// Token: 0x0400009C RID: 156
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Cloth> tempCloths = new List<Cloth>();

	// Token: 0x0400009D RID: 157
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Material> tempMats = new List<Material>();

	// Token: 0x0400009E RID: 158
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<SkinnedMeshRenderer> tempSMRs = new List<SkinnedMeshRenderer>();

	// Token: 0x0400009F RID: 159
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly string[] shortHairNames = new string[]
	{
		"buzzcut",
		"comb_over",
		"cornrows",
		"flattop_fro",
		"mohawk",
		"pixie_cut",
		"small_fro"
	};

	// Token: 0x040000A0 RID: 160
	[PublicizedFrom(EAccessModifier.Private)]
	public const string ORIGIN = "Origin";

	// Token: 0x040000A1 RID: 161
	[PublicizedFrom(EAccessModifier.Private)]
	public const string RIGCON = "RigConstraints";

	// Token: 0x040000A2 RID: 162
	[PublicizedFrom(EAccessModifier.Private)]
	public const string IKRIG = "IKRig";

	// Token: 0x040000A3 RID: 163
	public const string HEAD = "head";

	// Token: 0x040000A4 RID: 164
	public const string EYES = "eyes";

	// Token: 0x040000A5 RID: 165
	public const string BEARD = "beard";

	// Token: 0x040000A6 RID: 166
	public const string HAIR = "hair";

	// Token: 0x040000A7 RID: 167
	public const string BODY = "body";

	// Token: 0x040000A8 RID: 168
	public const string HANDS = "hands";

	// Token: 0x040000A9 RID: 169
	public const string FEET = "feet";

	// Token: 0x040000AA RID: 170
	public const string HELMET = "helmet";

	// Token: 0x040000AB RID: 171
	public const string TORSO = "torso";

	// Token: 0x040000AC RID: 172
	public const string GLOVES = "gloves";

	// Token: 0x040000AD RID: 173
	public const string BOOTS = "boots";

	// Token: 0x040000AE RID: 174
	public const string SEX_MARKER = "{sex}";

	// Token: 0x040000AF RID: 175
	public const string RACE_MARKER = "{race}";

	// Token: 0x040000B0 RID: 176
	public const string VARIANT_MARKER = "{variant}";

	// Token: 0x040000B1 RID: 177
	public const string HAIR_MARKER = "{hair}";

	// Token: 0x040000B2 RID: 178
	[PublicizedFrom(EAccessModifier.Private)]
	public static Archetype archetype;

	// Token: 0x040000B3 RID: 179
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] baseParts = new string[]
	{
		"head",
		"body",
		"hands",
		"feet"
	};
}

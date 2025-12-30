using System;
using System.Collections.Generic;
using System.Linq;
using ShinyScreenSpaceRaytracedReflections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;

// Token: 0x0200095B RID: 2395
public static class SDCSUtils
{
	// Token: 0x06004835 RID: 18485 RVA: 0x001C4A30 File Offset: 0x001C2C30
	public static GameObject Stitch(GameObject sourceObj, GameObject parentObj, SDCSUtils.TransformCatalog boneCatalog, EModelSDCS emodel = null, bool isFPV = false, float cullDist = 0f, bool isUI = false, Material eyeMat = null, bool isGear = false)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceObj, parentObj.transform);
		gameObject.name = sourceObj.name;
		gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(SDCSUtils.tempSMRs);
		foreach (SkinnedMeshRenderer skinnedMeshRenderer in SDCSUtils.tempSMRs)
		{
			GameObject gameObject2 = skinnedMeshRenderer.gameObject;
			string name = gameObject2.name;
			skinnedMeshRenderer.bones = SDCSUtils.TranslateTransforms(skinnedMeshRenderer.bones, boneCatalog);
			skinnedMeshRenderer.rootBone = SDCSUtils.Find<string, Transform>(boneCatalog, skinnedMeshRenderer.rootBone.name);
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
						Material material2 = DataLoader.LoadAsset<Material>(SDCSUtils.baseBodyMatLoc, false);
						sharedMaterials[i] = (material2 ? material2 : sharedMaterials[i]);
					}
					else if (name2.Contains("_Head"))
					{
						Material material3 = DataLoader.LoadAsset<Material>(SDCSUtils.baseHeadMatLoc, false);
						sharedMaterials[i] = (material3 ? material3 : sharedMaterials[i]);
					}
					else if (name2.Contains("_Hand"))
					{
						Material material4 = DataLoader.LoadAsset<Material>(SDCSUtils.baseHandsMatLoc, false);
						sharedMaterials[i] = (material4 ? material4 : sharedMaterials[i]);
					}
				}
			}
			if (name == "eyes" && eyeMat)
			{
				sharedMaterials[0] = eyeMat;
			}
			skinnedMeshRenderer.sharedMaterials = sharedMaterials;
			Material material5 = sharedMaterials[0];
			string text = material5 ? material5.shader.name : "";
			if (text.Equals("Game/SDCS/Skin") || text.Equals("Game/SDCS/Hair"))
			{
				gameObject2.AddComponent<ExcludeReflections>().enabled = !isFPV;
			}
			if (text.Equals("Game/Character") || text.Equals("Game/CharacterPlayerSkin") || text.Equals("Game/CharacterPlayerOutfit") || text.Equals("Game/CharacterCloth"))
			{
				Material material6 = skinnedMeshRenderer.material;
				if (name == "hands" || name == "gloves")
				{
					material6.SetFloat("_FirstPerson", 0f);
					material6.SetFloat("_ClipRadius", 0f);
				}
				else if (!isUI)
				{
					if (emodel && isFPV)
					{
						emodel.ClipMaterialsFP.Add(material6);
					}
					material6.SetFloat("_FirstPerson", (float)((emodel && isFPV) ? 1 : 0));
					material6.SetFloat("_ClipRadius", cullDist);
				}
				else
				{
					material6.SetFloat("_FirstPerson", 0f);
					material6.SetFloat("_ClipRadius", 0f);
				}
				material6.SetVector("_ClipCenter", boneCatalog["Head"].position);
				if (!isUI && emodel && isFPV && isGear)
				{
					SDCSUtils.RemoveFPViewObstructingGearPolygons(skinnedMeshRenderer);
				}
			}
		}
		SDCSUtils.tempSMRs.Clear();
		Transform transform = boneCatalog["Hips"];
		gameObject.GetComponentsInChildren<Cloth>(SDCSUtils.tempCloths);
		foreach (Cloth cloth in SDCSUtils.tempCloths)
		{
			cloth.capsuleColliders = transform.GetComponentsInChildren<CapsuleCollider>();
		}
		SDCSUtils.tempCloths.Clear();
		return gameObject;
	}

	// Token: 0x06004836 RID: 18486 RVA: 0x001C4DE8 File Offset: 0x001C2FE8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void RemoveFPViewObstructingGearPolygons(SkinnedMeshRenderer smr)
	{
		if (smr && smr.sharedMesh)
		{
			Mesh mesh = UnityEngine.Object.Instantiate<Mesh>(smr.sharedMesh);
			smr.sharedMesh = mesh;
			Color[] colors = mesh.colors;
			if (colors.Length != 0)
			{
				int[] triangles = mesh.triangles;
				int num = triangles.Length / 3;
				int vertexCount = mesh.vertexCount;
				int[] array = new int[num * 3];
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					int num3 = triangles[i * 3];
					int num4 = triangles[i * 3 + 1];
					int num5 = triangles[i * 3 + 2];
					if (colors[num3].r == 0f && colors[num4].r == 0f && colors[num5].r == 0f)
					{
						array[num2 * 3] = num3;
						array[num2 * 3 + 1] = num4;
						array[num2 * 3 + 2] = num5;
						num2++;
					}
				}
				Array.Resize<int>(ref array, num2 * 3);
				mesh.triangles = array;
				if (num > num2)
				{
					Debug.Log("SDCSUtils::RemoveFPViewObstructingGearPolygons -> Removed " + (num - num2).ToString() + " obstructing polygons from " + mesh.name);
					return;
				}
				Debug.Log("SDCSUtils::RemoveFPViewObstructingGearPolygons -> " + mesh.name + " has no obstructing polygons");
			}
		}
	}

	// Token: 0x06004837 RID: 18487 RVA: 0x001C4F40 File Offset: 0x001C3140
	public static void MatchRigs(SDCSUtils.SlotData wornItem, Transform source, Transform target, SDCSUtils.TransformCatalog transformCatalog)
	{
		Transform transform = source.Find("Origin");
		Transform transform2 = target.Find("Origin");
		if (!transform || !transform2)
		{
			return;
		}
		SDCSUtils.AddMissingChildren(wornItem, transform, transform2, transformCatalog);
	}

	// Token: 0x06004838 RID: 18488 RVA: 0x001C4F80 File Offset: 0x001C3180
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
			SDCSUtils.TransferCharacterJoint(transform, transform2.gameObject, transformCatalog);
			SDCSUtils.AddMissingChildren(wornItem, transform, transform2, transformCatalog);
		}
	}

	// Token: 0x06004839 RID: 18489 RVA: 0x001C50CC File Offset: 0x001C32CC
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
					blendConstraint.data.constrainedObject = SDCSUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.constrainedObject.name);
					blendConstraint.data.sourceObjectA = SDCSUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.sourceObjectA.name);
					blendConstraint.data.sourceObjectB = SDCSUtils.Find<string, Transform>(transformCatalog, blendConstraint2.data.sourceObjectB.name);
					break;
				}
			}
		}
	}

	// Token: 0x0600483A RID: 18490 RVA: 0x001C5250 File Offset: 0x001C3450
	[PublicizedFrom(EAccessModifier.Private)]
	public static void TransferCharacterJoint(Transform source, GameObject newBone, SDCSUtils.TransformCatalog transformCatalog)
	{
		CharacterJoint component;
		CharacterJoint characterJoint;
		if ((component = source.GetComponent<CharacterJoint>()) != null && (characterJoint = newBone.AddMissingComponent<CharacterJoint>()) != null)
		{
			Joint joint = characterJoint;
			Transform transform = SDCSUtils.Find<string, Transform>(transformCatalog, component.connectedBody.name);
			joint.connectedBody = ((transform != null) ? transform.GetComponent<Rigidbody>() : null);
		}
	}

	// Token: 0x0600483B RID: 18491 RVA: 0x001C52A0 File Offset: 0x001C34A0
	[PublicizedFrom(EAccessModifier.Private)]
	public static GameObject AddChild(GameObject source, Transform parent)
	{
		source.transform.parent = parent;
		foreach (object obj in source.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		return source;
	}

	// Token: 0x0600483C RID: 18492 RVA: 0x001C5308 File Offset: 0x001C3508
	[PublicizedFrom(EAccessModifier.Private)]
	public static SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer source, GameObject parent)
	{
		SkinnedMeshRenderer skinnedMeshRenderer = new GameObject(source.name)
		{
			transform = 
			{
				parent = parent.transform
			}
		}.AddComponent<SkinnedMeshRenderer>();
		skinnedMeshRenderer.sharedMesh = source.sharedMesh;
		skinnedMeshRenderer.sharedMaterials = source.sharedMaterials;
		return skinnedMeshRenderer;
	}

	// Token: 0x0600483D RID: 18493 RVA: 0x001C5344 File Offset: 0x001C3544
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform[] TranslateTransforms(Transform[] transforms, SDCSUtils.TransformCatalog transformCatalog)
	{
		for (int i = 0; i < transforms.Length; i++)
		{
			Transform transform = transforms[i];
			if (transform)
			{
				transforms[i] = SDCSUtils.Find<string, Transform>(transformCatalog, transform.name);
			}
			else
			{
				Log.Error("Null transform in bone list");
			}
		}
		return transforms;
	}

	// Token: 0x0600483E RID: 18494 RVA: 0x001C5388 File Offset: 0x001C3588
	public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
	{
		TValue result;
		source.TryGetValue(key, out result);
		return result;
	}

	// Token: 0x17000794 RID: 1940
	// (get) Token: 0x0600483F RID: 18495 RVA: 0x001C53A0 File Offset: 0x001C35A0
	public static string baseBodyLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Common/Meshes/player",
				SDCSUtils.tmpArchetype.Sex,
				".fbx"
			});
		}
	}

	// Token: 0x17000795 RID: 1941
	// (get) Token: 0x06004840 RID: 18496 RVA: 0x001C53E0 File Offset: 0x001C35E0
	public static string baseHeadLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Heads/",
				SDCSUtils.tmpArchetype.Race,
				"/",
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"/Meshes/player",
				SDCSUtils.tmpArchetype.Sex,
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				".fbx"
			});
		}
	}

	// Token: 0x17000796 RID: 1942
	// (get) Token: 0x06004841 RID: 18497 RVA: 0x001C5488 File Offset: 0x001C3688
	public static string baseHairLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Hair/",
				SDCSUtils.tmpArchetype.Hair,
				"/HairMorphMatrix/",
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x17000797 RID: 1943
	// (get) Token: 0x06004842 RID: 18498 RVA: 0x001C54F8 File Offset: 0x001C36F8
	public static string baseMustacheLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/FacialHair/Mustache/",
				SDCSUtils.tmpArchetype.MustacheName,
				"/HairMorphMatrix/",
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x17000798 RID: 1944
	// (get) Token: 0x06004843 RID: 18499 RVA: 0x001C5568 File Offset: 0x001C3768
	public static string baseChopsLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/FacialHair/Chops/",
				SDCSUtils.tmpArchetype.ChopsName,
				"/HairMorphMatrix/",
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x17000799 RID: 1945
	// (get) Token: 0x06004844 RID: 18500 RVA: 0x001C55D8 File Offset: 0x001C37D8
	public static string baseBeardLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/FacialHair/Beard/",
				SDCSUtils.tmpArchetype.BeardName,
				"/HairMorphMatrix/",
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00")
			});
		}
	}

	// Token: 0x1700079A RID: 1946
	// (get) Token: 0x06004845 RID: 18501 RVA: 0x00007772 File Offset: 0x00005972
	public static string baseHairColorLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/HairColorSwatches";
		}
	}

	// Token: 0x1700079B RID: 1947
	// (get) Token: 0x06004846 RID: 18502 RVA: 0x001C5646 File Offset: 0x001C3846
	public static string baseEyeColorMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/Eyes/Materials/" + SDCSUtils.tmpArchetype.EyeColorName + ".mat";
		}
	}

	// Token: 0x1700079C RID: 1948
	// (get) Token: 0x06004847 RID: 18503 RVA: 0x001C5664 File Offset: 0x001C3864
	public static string baseBodyMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Heads/",
				SDCSUtils.tmpArchetype.Race,
				"/",
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"/Materials/player",
				SDCSUtils.tmpArchetype.Sex,
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"_Body.mat"
			});
		}
	}

	// Token: 0x1700079D RID: 1949
	// (get) Token: 0x06004848 RID: 18504 RVA: 0x001C570C File Offset: 0x001C390C
	public static string baseHeadMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Heads/",
				SDCSUtils.tmpArchetype.Race,
				"/",
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"/Materials/player",
				SDCSUtils.tmpArchetype.Sex,
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"_Head.mat"
			});
		}
	}

	// Token: 0x1700079E RID: 1950
	// (get) Token: 0x06004849 RID: 18505 RVA: 0x001C57B4 File Offset: 0x001C39B4
	public static string baseHandsMatLoc
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return string.Concat(new string[]
			{
				"@:Entities/Player/",
				SDCSUtils.tmpArchetype.Sex,
				"/Heads/",
				SDCSUtils.tmpArchetype.Race,
				"/",
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"/Materials/player",
				SDCSUtils.tmpArchetype.Sex,
				SDCSUtils.tmpArchetype.Race,
				SDCSUtils.tmpArchetype.Variant.ToString("00"),
				"_Hand.mat"
			});
		}
	}

	// Token: 0x1700079F RID: 1951
	// (get) Token: 0x0600484A RID: 18506 RVA: 0x00007989 File Offset: 0x00005B89
	public static string baseRigPrefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/BaseRigs/baseRigPrefab.prefab";
		}
	}

	// Token: 0x170007A0 RID: 1952
	// (get) Token: 0x0600484B RID: 18507 RVA: 0x001C5859 File Offset: 0x001C3A59
	public static string baseRigFPPrefab
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return "@:Entities/Player/Common/BaseRigs/baseRigFPPrefab.prefab";
		}
	}

	// Token: 0x170007A1 RID: 1953
	// (get) Token: 0x0600484C RID: 18508 RVA: 0x00007990 File Offset: 0x00005B90
	public static RuntimeAnimatorController UIAnimController
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return DataLoader.LoadAsset<RuntimeAnimatorController>("@:Entities/Player/Common/AnimControllers/MenuSDCSController.controller", false);
		}
	}

	// Token: 0x170007A2 RID: 1954
	// (get) Token: 0x0600484D RID: 18509 RVA: 0x001C5860 File Offset: 0x001C3A60
	public static RuntimeAnimatorController TPAnimController
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return DataLoader.LoadAsset<RuntimeAnimatorController>("@:Entities/Player/Common/AnimControllers/3PPlayer" + SDCSUtils.tmpArchetype.Sex + "Controller" + (SDCSUtils.tmpArchetype.IsMale ? ".controller" : ".overrideController"), false);
		}
	}

	// Token: 0x170007A3 RID: 1955
	// (get) Token: 0x0600484E RID: 18510 RVA: 0x001C5899 File Offset: 0x001C3A99
	public static RuntimeAnimatorController FPAnimController
	{
		[PublicizedFrom(EAccessModifier.Private)]
		get
		{
			return DataLoader.LoadAsset<RuntimeAnimatorController>("@:Entities/Player/Common/AnimControllers/FPPlayerController.controller", false);
		}
	}

	// Token: 0x0600484F RID: 18511 RVA: 0x001C58A8 File Offset: 0x001C3AA8
	public static void CreateVizTP(Archetype _archetype, ref GameObject baseRig, ref SDCSUtils.TransformCatalog boneCatalog, EntityAlive entity, bool isFPV)
	{
		SDCSUtils.DestroyViz(baseRig, true);
		SDCSUtils.tmpArchetype = _archetype;
		SDCSUtils.setupRig(ref baseRig, ref boneCatalog, SDCSUtils.baseRigPrefab, null, SDCSUtils.TPAnimController);
		if (!isFPV)
		{
			SDCSUtils.setupBase(baseRig, boneCatalog, SDCSUtils.baseParts, isFPV);
			SDCSUtils.setupEquipment(baseRig, boneCatalog, SDCSUtils.ignoredParts, entity, false, !(entity is EntityPlayerLocal), false);
			SDCSUtils.setupHairObjects(baseRig, boneCatalog, SDCSUtils.ignoredParts, entity, false);
		}
	}

	// Token: 0x06004850 RID: 18512 RVA: 0x001C5918 File Offset: 0x001C3B18
	public static void CreateVizFP(Archetype _archetype, ref GameObject baseRigFP, ref SDCSUtils.TransformCatalog boneCatalogFP, EntityAlive entity, bool isFPV)
	{
		SDCSUtils.DestroyViz(baseRigFP, true);
		SDCSUtils.tmpArchetype = _archetype;
		Transform transform = entity.transform.FindInChildren("Camera");
		if (transform == null)
		{
			if (!(GameObject.Find("Camera") != null))
			{
				Log.Error("Unable to find first person camera!");
				return;
			}
			transform = GameObject.Find("Camera").transform;
		}
		Transform transform2 = transform.FindInChildren("Pivot");
		if (transform2 != null)
		{
			transform = transform2.parent;
		}
		SDCSUtils.setupRig(ref baseRigFP, ref boneCatalogFP, SDCSUtils.baseRigFPPrefab, transform, SDCSUtils.FPAnimController);
		SDCSUtils.setupBase(baseRigFP, boneCatalogFP, SDCSUtils.basePartsFP, isFPV);
		SDCSUtils.setupEquipment(baseRigFP, boneCatalogFP, SDCSUtils.ignoredPartsFP, entity, false, !(entity is EntityPlayerLocal), false);
		SDCSUtils.setupHairObjects(baseRigFP, boneCatalogFP, SDCSUtils.ignoredPartsFP, entity, false);
		Transform transform3 = baseRigFP.transform;
		transform3.SetParent(transform, false);
		transform3.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		baseRigFP.name = "baseRigFP";
		baseRigFP.AddMissingComponent<AnimationEventBridge>();
		SkinnedMeshRenderer[] componentsInChildren = baseRigFP.GetComponentsInChildren<SkinnedMeshRenderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].gameObject.layer = LayerMask.NameToLayer("HoldingItem");
		}
		foreach (HingeJoint hingeJoint in baseRigFP.GetComponentsInChildren<HingeJoint>())
		{
			if (hingeJoint.connectedBody == null)
			{
				Log.Warning("SDCSUtils::CreateVizFP: No connected body for " + hingeJoint.transform.name + "'s HingeJoint! Disabling for FP as it is never seen.");
				hingeJoint.gameObject.SetActive(false);
			}
		}
		baseRigFP.GetComponentsInChildren<Cloth>(SDCSUtils.tempCloths);
		foreach (Cloth cloth in SDCSUtils.tempCloths)
		{
			cloth.enabled = false;
		}
		SDCSUtils.tempCloths.Clear();
	}

	// Token: 0x06004851 RID: 18513 RVA: 0x00002914 File Offset: 0x00000B14
	[PublicizedFrom(EAccessModifier.Private)]
	public static void SetupBodyColliders(GameObject baseRig)
	{
	}

	// Token: 0x06004852 RID: 18514 RVA: 0x001C5AFC File Offset: 0x001C3CFC
	public static void CreateVizUI(Archetype _archetype, ref GameObject baseRigUI, ref SDCSUtils.TransformCatalog boneCatalogUI, EntityAlive entity, bool useTempCosmetics)
	{
		SDCSUtils.DestroyViz(baseRigUI, true);
		SDCSUtils.tmpArchetype = _archetype;
		SDCSUtils.setupRig(ref baseRigUI, ref boneCatalogUI, SDCSUtils.baseRigPrefab, null, SDCSUtils.UIAnimController);
		SDCSUtils.SetupBodyColliders(baseRigUI);
		SDCSUtils.setupBase(baseRigUI, boneCatalogUI, SDCSUtils.baseParts, false);
		SDCSUtils.setupEquipment(baseRigUI, boneCatalogUI, SDCSUtils.ignoredParts, entity, true, useTempCosmetics, useTempCosmetics);
		SDCSUtils.setupHairObjects(baseRigUI, boneCatalogUI, SDCSUtils.ignoredParts, entity, true);
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

	// Token: 0x06004853 RID: 18515 RVA: 0x001C5BE4 File Offset: 0x001C3DE4
	public static void CreateVizUI(Archetype _archetype, ref GameObject baseRigUI, ref SDCSUtils.TransformCatalog boneCatalogUI)
	{
		SDCSUtils.DestroyViz(baseRigUI, false);
		SDCSUtils.tmpArchetype = _archetype;
		SDCSUtils.setupRig(ref baseRigUI, ref boneCatalogUI, SDCSUtils.baseRigPrefab, null, SDCSUtils.UIAnimController);
		SDCSUtils.setupBase(baseRigUI, boneCatalogUI, SDCSUtils.baseParts, false);
		SDCSUtils.setupHairObjects(baseRigUI, boneCatalogUI, null, false, SDCSUtils.ignoredParts, true, _archetype.Hair, _archetype.MustacheName, _archetype.ChopsName, _archetype.BeardName);
		SDCSUtils.setupEquipment(baseRigUI, boneCatalogUI, SDCSUtils.ignoredParts, true, _archetype.Equipment, false);
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

	// Token: 0x06004854 RID: 18516 RVA: 0x001C5CE0 File Offset: 0x001C3EE0
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
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, SDCSUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in SDCSUtils.tempSMRs)
					{
						Mesh sharedMesh = skinnedMeshRenderer.sharedMesh;
						if (MeshMorph.IsInstance(sharedMesh))
						{
							UnityEngine.Object.Destroy(sharedMesh);
						}
						skinnedMeshRenderer.GetSharedMaterials(SDCSUtils.tempMats);
						Utils.CleanupMaterials<List<Material>>(SDCSUtils.tempMats);
						SDCSUtils.tempMats.Clear();
					}
				}
			}
			if (!_keepRig)
			{
				UnityEngine.Object.DestroyImmediate(_baseRigUI);
			}
		}
	}

	// Token: 0x06004855 RID: 18517 RVA: 0x001C5DB8 File Offset: 0x001C3FB8
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
					child.GetComponentsInChildren<SkinnedMeshRenderer>(true, SDCSUtils.tempSMRs);
					foreach (SkinnedMeshRenderer skinnedMeshRenderer in SDCSUtils.tempSMRs)
					{
						skinnedMeshRenderer.gameObject.SetActive(_visible);
					}
				}
			}
		}
	}

	// Token: 0x06004856 RID: 18518 RVA: 0x001C5E54 File Offset: 0x001C4054
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
			SDCSUtils.cleanupEquipment(_rigObj);
		}
		if (!SDCSUtils.tmpArchetype.IsMale)
		{
			CapsuleCollider orAddComponent = _boneCatalog["Hips"].gameObject.GetOrAddComponent<CapsuleCollider>();
			orAddComponent.center = new Vector3(0f, 0f, -0.03f);
			orAddComponent.radius = 0.15f;
			orAddComponent.height = 0.375f;
		}
	}

	// Token: 0x06004857 RID: 18519 RVA: 0x001C5F28 File Offset: 0x001C4128
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

	// Token: 0x06004858 RID: 18520 RVA: 0x001C5FC0 File Offset: 0x001C41C0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupBase(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] baseParts, bool isFPV)
	{
		foreach (string text in baseParts)
		{
			GameObject gameObject;
			if (text == "head")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(SDCSUtils.baseHeadLoc, false);
			}
			else if (text == "hands")
			{
				gameObject = DataLoader.LoadAsset<GameObject>(SDCSUtils.baseBodyLoc, false);
			}
			else
			{
				gameObject = DataLoader.LoadAsset<GameObject>(SDCSUtils.baseBodyLoc, false);
			}
			if (gameObject == null)
			{
				return;
			}
			GameObject bodyPartContainingName;
			if (!((bodyPartContainingName = SDCSUtils.getBodyPartContainingName(gameObject.transform, text)) == null))
			{
				bodyPartContainingName.name = text;
				GameObject sourceObj = bodyPartContainingName;
				EModelSDCS emodel = null;
				Material eyeMat = DataLoader.LoadAsset<Material>(SDCSUtils.baseEyeColorMatLoc, false);
				SDCSUtils.Stitch(sourceObj, _rig, _boneCatalog, emodel, isFPV, 0f, false, eyeMat, false);
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

	// Token: 0x06004859 RID: 18521 RVA: 0x001C62A4 File Offset: 0x001C44A4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipment(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] ignoredParts, bool isUI, List<SDCSUtils.SlotData> slotData, bool _ignoreDlcEntitlements)
	{
		if (slotData == null)
		{
			return;
		}
		List<Transform> allGears = new List<Transform>();
		Transform transform = _rig.transform.Find("Origin");
		if (transform)
		{
			List<Transform> list = SDCSUtils.findStartsWith(transform, "RigConstraints");
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
				SDCSUtils.setupHeadgearMorph(_rig, _boneCatalog, null, false, isUI, slotData2, _ignoreDlcEntitlements);
			}
			else
			{
				Transform transform2 = SDCSUtils.setupEquipmentSlot(_rig, _boneCatalog, ignoredParts, slotData2, allGears, _ignoreDlcEntitlements);
				if (transform2)
				{
					float cullDistance = slotData2.CullDistance;
					Morphable componentInChildren = SDCSUtils.Stitch(transform2.gameObject, _rig, _boneCatalog, null, false, cullDistance, isUI, null, false).GetComponentInChildren<Morphable>();
					if (componentInChildren)
					{
						componentInChildren.MorphHeadgear(SDCSUtils.tmpArchetype, _ignoreDlcEntitlements);
					}
				}
			}
		}
		List<RigBuilder> rbs = new List<RigBuilder>();
		SDCSUtils.setupEquipmentCommon(_rig, _boneCatalog, allGears, rbs);
	}

	// Token: 0x0600485A RID: 18522 RVA: 0x001C63F4 File Offset: 0x001C45F4
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupEquipment(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] ignoredParts, EntityAlive entity, bool isUI, bool _ignoreDlcEntitlements, bool useTempCosmetics = false)
	{
		if (!entity)
		{
			return;
		}
		EModelSDCS emodelSDCS = entity.emodel as EModelSDCS;
		if (!emodelSDCS)
		{
			return;
		}
		emodelSDCS.HairMaskType = SDCSUtils.SlotData.HairMaskTypes.Full;
		emodelSDCS.FacialHairMaskType = SDCSUtils.SlotData.HairMaskTypes.Full;
		if (!isUI && emodelSDCS.IsFPV)
		{
			emodelSDCS.ClipMaterialsFP.Clear();
		}
		List<Transform> allGears = new List<Transform>();
		Transform transform = _rig.transform.Find("Origin");
		if (transform)
		{
			List<Transform> list = SDCSUtils.findStartsWith(transform, "RigConstraints");
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		int slotCount = entity.equipment.GetSlotCount();
		for (int j = 0; j < slotCount; j++)
		{
			ItemValue slotItem = entity.equipment.GetSlotItem(j);
			ItemClass cosmeticSlot = entity.equipment.GetCosmeticSlot(j, useTempCosmetics);
			object obj;
			if (slotItem == null)
			{
				obj = null;
			}
			else
			{
				ItemClass itemClass = slotItem.ItemClass;
				obj = ((itemClass != null) ? itemClass.SDCSData : null);
			}
			bool flag = obj != null;
			bool flag2 = cosmeticSlot != null && (useTempCosmetics || (flag && (_ignoreDlcEntitlements || entity.equipment.HasCosmeticUnlocked(cosmeticSlot).Item1)));
			if ((flag || flag2) && cosmeticSlot != ItemClass.MissingItem)
			{
				ItemClass itemClass2 = flag2 ? cosmeticSlot : slotItem.ItemClass;
				SDCSUtils.SlotData sdcsdata = itemClass2.SDCSData;
				if (cosmeticSlot != null && !flag2)
				{
					entity.equipment.SetCosmeticSlot(j, 0);
					entity.bPlayerStatsChanged = true;
				}
				ItemClassArmor itemClassArmor = itemClass2 as ItemClassArmor;
				if (itemClassArmor != null && itemClassArmor.EquipSlot == EquipmentSlots.Head)
				{
					emodelSDCS.HairMaskType = sdcsdata.HairMaskType;
					emodelSDCS.FacialHairMaskType = sdcsdata.FacialHairMaskType;
				}
				if ("head".Equals(sdcsdata.PartName, StringComparison.OrdinalIgnoreCase) && sdcsdata.PrefabName != null && sdcsdata.PrefabName.Contains("HeadGearMorphMatrix", StringComparison.OrdinalIgnoreCase))
				{
					SDCSUtils.setupHeadgearMorph(_rig, _boneCatalog, emodelSDCS, emodelSDCS.IsFPV, isUI, sdcsdata, _ignoreDlcEntitlements);
				}
				else if (!"head".Equals(sdcsdata.PartName, StringComparison.OrdinalIgnoreCase) || !emodelSDCS.IsFPV || isUI)
				{
					Transform transform2 = SDCSUtils.setupEquipmentSlot(_rig, _boneCatalog, ignoredParts, sdcsdata, allGears, _ignoreDlcEntitlements);
					if (transform2)
					{
						float cullDistance = sdcsdata.CullDistance;
						Morphable componentInChildren = SDCSUtils.Stitch(transform2.gameObject, _rig, _boneCatalog, emodelSDCS, emodelSDCS.IsFPV, cullDistance, isUI, null, true).GetComponentInChildren<Morphable>();
						if (componentInChildren)
						{
							componentInChildren.MorphHeadgear(SDCSUtils.tmpArchetype, _ignoreDlcEntitlements);
						}
					}
				}
			}
		}
		List<RigBuilder> rbs = new List<RigBuilder>();
		SDCSUtils.setupEquipmentCommon(_rig, _boneCatalog, allGears, rbs);
	}

	// Token: 0x0600485B RID: 18523 RVA: 0x001C6678 File Offset: 0x001C4878
	[PublicizedFrom(EAccessModifier.Private)]
	public static Transform setupEquipmentSlot(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] ignoredParts, SDCSUtils.SlotData wornItem, List<Transform> allGears, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = SDCSUtils.GetPathForSlotData(wornItem, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return null;
		}
		GameObject gameObject = DataLoader.LoadAsset<GameObject>(pathForSlotData, _ignoreDlcEntitlements);
		if (gameObject == null && wornItem.PartName == "head")
		{
			pathForSlotData = SDCSUtils.GetPathForSlotData(wornItem, false);
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
		SDCSUtils.MatchRigs(wornItem, gameObject.transform, _rig.transform, _boneCatalog);
		if (!allGears.Contains(gameObject.transform))
		{
			allGears.Add(gameObject.transform);
		}
		Transform clothingPartWithName = SDCSUtils.getClothingPartWithName(gameObject, SDCSUtils.parseSexedLocation(wornItem.PartName, SDCSUtils.tmpArchetype.Sex));
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

	// Token: 0x0600485C RID: 18524 RVA: 0x001C67DC File Offset: 0x001C49DC
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
			SDCSUtils.SetupRigConstraints(rigBuilder, sourceRootT, _rigObj.transform, _boneCatalog);
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

	// Token: 0x0600485D RID: 18525 RVA: 0x001C68D0 File Offset: 0x001C4AD0
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHairObjects(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, string[] ignoredParts, EntityAlive entity, bool isUI)
	{
		if (!entity)
		{
			return;
		}
		EModelSDCS emodelSDCS = entity.emodel as EModelSDCS;
		if (!emodelSDCS)
		{
			return;
		}
		if (!isUI && emodelSDCS.IsFPV)
		{
			emodelSDCS.ClipMaterialsFP.Clear();
		}
		Transform transform = _rig.transform.Find("Origin");
		if (transform)
		{
			List<Transform> list = SDCSUtils.findStartsWith(transform, "RigConstraints");
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				UnityEngine.Object.DestroyImmediate(list[i].gameObject);
			}
		}
		if (!emodelSDCS.IsFPV || isUI)
		{
			SDCSUtils.setupHairObjects(_rig, _boneCatalog, emodelSDCS, emodelSDCS.IsFPV, ignoredParts, isUI, SDCSUtils.tmpArchetype.Hair, SDCSUtils.tmpArchetype.MustacheName, SDCSUtils.tmpArchetype.ChopsName, SDCSUtils.tmpArchetype.BeardName);
		}
	}

	// Token: 0x0600485E RID: 18526 RVA: 0x001C69A8 File Offset: 0x001C4BA8
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHairObjects(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, EModelSDCS _emodel, bool _isFPV, string[] ignoredParts, bool isUI, string hairName, string mustacheName, string chopsName, string beardName)
	{
		HairColorSwatch hairColorSwatch = null;
		if (!string.IsNullOrEmpty(SDCSUtils.tmpArchetype.HairColor))
		{
			string text = SDCSUtils.baseHairColorLoc + "/" + SDCSUtils.tmpArchetype.HairColor + ".asset";
			ScriptableObject scriptableObject = DataLoader.LoadAsset<ScriptableObject>(text, false);
			if (scriptableObject == null)
			{
				Log.Warning(string.Concat(new string[]
				{
					"SDCSUtils::",
					text,
					" not found for hair color ",
					SDCSUtils.tmpArchetype.HairColor,
					"!"
				}));
			}
			else
			{
				hairColorSwatch = (scriptableObject as HairColorSwatch);
			}
		}
		if (!string.IsNullOrEmpty(hairName))
		{
			if (_emodel != null)
			{
				if (_emodel.HairMaskType != SDCSUtils.SlotData.HairMaskTypes.None)
				{
					string text2 = "";
					if (_emodel.HairMaskType != SDCSUtils.SlotData.HairMaskTypes.Full)
					{
						text2 = "_" + _emodel.HairMaskType.ToString().ToLower();
					}
					SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, string.Concat(new string[]
					{
						SDCSUtils.baseHairLoc,
						"/hair_",
						hairName,
						text2,
						".asset"
					}), hairName);
				}
			}
			else
			{
				SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseHairLoc + "/hair_" + hairName + ".asset", hairName);
			}
		}
		if (!string.IsNullOrEmpty(mustacheName))
		{
			if (_emodel != null)
			{
				if (_emodel.FacialHairMaskType != SDCSUtils.SlotData.HairMaskTypes.None)
				{
					SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseMustacheLoc + "/hair_facial_mustache" + mustacheName + ".asset", mustacheName);
				}
			}
			else
			{
				SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseMustacheLoc + "/hair_facial_mustache" + mustacheName + ".asset", mustacheName);
			}
		}
		if (!string.IsNullOrEmpty(chopsName))
		{
			if (_emodel != null)
			{
				if (_emodel.FacialHairMaskType != SDCSUtils.SlotData.HairMaskTypes.None)
				{
					SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseChopsLoc + "/hair_facial_sideburns" + chopsName + ".asset", chopsName);
				}
			}
			else
			{
				SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseChopsLoc + "/hair_facial_sideburns" + chopsName + ".asset", chopsName);
			}
		}
		if (!string.IsNullOrEmpty(beardName))
		{
			if (_emodel != null)
			{
				if (_emodel.FacialHairMaskType != SDCSUtils.SlotData.HairMaskTypes.None)
				{
					SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseBeardLoc + "/hair_facial_beard" + beardName + ".asset", beardName);
				}
			}
			else
			{
				SDCSUtils.setupHair(_rig, _boneCatalog, _emodel, _isFPV, ignoredParts, isUI, SDCSUtils.baseBeardLoc + "/hair_facial_beard" + beardName + ".asset", beardName);
			}
		}
		if (hairColorSwatch != null)
		{
			SDCSUtils.ApplySwatchToGameObject(_rig, hairColorSwatch);
		}
	}

	// Token: 0x0600485F RID: 18527 RVA: 0x001C6C34 File Offset: 0x001C4E34
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

	// Token: 0x06004860 RID: 18528 RVA: 0x001C6CDC File Offset: 0x001C4EDC
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHair(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, EModelSDCS _emodel, bool _isFPV, string[] ignoredParts, bool isUI, string path, string hairName)
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
		SDCSUtils.MatchRigs(null, gameObject.transform, _rig.transform, _boneCatalog);
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		SDCSUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, _emodel, _isFPV, 0f, isUI, null, false);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x06004861 RID: 18529 RVA: 0x001C6D90 File Offset: 0x001C4F90
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
			if (SDCSUtils.ignoredParts.Contains("head"))
			{
				return null;
			}
			string text = _slotData.PrefabName;
			if (text.Contains("{sex}"))
			{
				text = text.Replace("{sex}", SDCSUtils.tmpArchetype.Sex);
			}
			if (text.Contains("{race}"))
			{
				text = text.Replace("{race}", SDCSUtils.tmpArchetype.Race);
			}
			if (text.Contains("{variant}"))
			{
				text = text.Replace("{variant}", SDCSUtils.tmpArchetype.Variant.ToString("00"));
			}
			if (text.Contains("{hair}"))
			{
				if (_headgearShortHairMask && (string.IsNullOrEmpty(SDCSUtils.tmpArchetype.Hair) || SDCSUtils.shortHairNames.ContainsCaseInsensitive(SDCSUtils.tmpArchetype.Hair)))
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
		else
		{
			if (string.IsNullOrEmpty(_slotData.PrefabName))
			{
				return null;
			}
			foreach (string value in SDCSUtils.ignoredParts)
			{
				if (_slotData.PartName.Contains(value, StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
			}
			return SDCSUtils.parseSexedLocation(_slotData.PrefabName, SDCSUtils.tmpArchetype.Sex);
		}
	}

	// Token: 0x06004862 RID: 18530 RVA: 0x001C6F00 File Offset: 0x001C5100
	[PublicizedFrom(EAccessModifier.Private)]
	public static void setupHeadgearMorph(GameObject _rig, SDCSUtils.TransformCatalog _boneCatalog, EModelSDCS _emodel, bool _isFPV, bool isUI, SDCSUtils.SlotData _slotData, bool _ignoreDlcEntitlements)
	{
		string pathForSlotData = SDCSUtils.GetPathForSlotData(_slotData, true);
		if (string.IsNullOrEmpty(pathForSlotData))
		{
			return;
		}
		if (!isUI && _isFPV)
		{
			return;
		}
		MeshMorph meshMorph = DataLoader.LoadAsset<MeshMorph>(pathForSlotData, _ignoreDlcEntitlements);
		if (meshMorph == null)
		{
			pathForSlotData = SDCSUtils.GetPathForSlotData(_slotData, false);
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
		SDCSUtils.MatchRigs(null, gameObject.transform, _rig.transform, _boneCatalog);
		if (!gameObject.gameObject.activeSelf)
		{
			gameObject.gameObject.SetActive(true);
		}
		DataLoader.LoadAsset<Material>(SDCSUtils.baseBodyMatLoc, false);
		SDCSUtils.Stitch(gameObject.gameObject, _rig, _boneCatalog, _emodel, _isFPV, 0f, isUI, null, false);
		UnityEngine.Object.Destroy(gameObject);
	}

	// Token: 0x06004863 RID: 18531 RVA: 0x001C6FF4 File Offset: 0x001C51F4
	public static bool BasePartsExist(Archetype _archetype)
	{
		SDCSUtils.tmpArchetype = _archetype;
		if (!DataLoader.LoadAsset<GameObject>(SDCSUtils.baseBodyLoc, false))
		{
			Log.Error("base body not found at " + SDCSUtils.baseBodyLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<GameObject>(SDCSUtils.baseHeadLoc, false))
		{
			Log.Error("base head not found at " + SDCSUtils.baseHeadLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(SDCSUtils.baseBodyMatLoc, false))
		{
			Log.Error("body material not found at " + SDCSUtils.baseBodyMatLoc);
			return false;
		}
		if (!DataLoader.LoadAsset<Material>(SDCSUtils.baseHeadMatLoc, false))
		{
			Log.Error("head material not found at " + SDCSUtils.baseHeadMatLoc);
			return false;
		}
		return true;
	}

	// Token: 0x06004864 RID: 18532 RVA: 0x001C70A8 File Offset: 0x001C52A8
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

	// Token: 0x06004865 RID: 18533 RVA: 0x001C7114 File Offset: 0x001C5314
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

	// Token: 0x06004866 RID: 18534 RVA: 0x001C7188 File Offset: 0x001C5388
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

	// Token: 0x06004867 RID: 18535 RVA: 0x00008AF4 File Offset: 0x00006CF4
	[PublicizedFrom(EAccessModifier.Private)]
	public static string parseSexedLocation(string sexedLocation, string sex)
	{
		return sexedLocation.Replace("{sex}", sex);
	}

	// Token: 0x04003763 RID: 14179
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Cloth> tempCloths = new List<Cloth>();

	// Token: 0x04003764 RID: 14180
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<Material> tempMats = new List<Material>();

	// Token: 0x04003765 RID: 14181
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly List<SkinnedMeshRenderer> tempSMRs = new List<SkinnedMeshRenderer>();

	// Token: 0x04003766 RID: 14182
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

	// Token: 0x04003767 RID: 14183
	[PublicizedFrom(EAccessModifier.Private)]
	public const string ORIGIN = "Origin";

	// Token: 0x04003768 RID: 14184
	[PublicizedFrom(EAccessModifier.Private)]
	public const string RIGCON = "RigConstraints";

	// Token: 0x04003769 RID: 14185
	[PublicizedFrom(EAccessModifier.Private)]
	public const string IKRIG = "IKRig";

	// Token: 0x0400376A RID: 14186
	public const string HEAD = "head";

	// Token: 0x0400376B RID: 14187
	public const string EYES = "eyes";

	// Token: 0x0400376C RID: 14188
	public const string BEARD = "beard";

	// Token: 0x0400376D RID: 14189
	public const string HAIR = "hair";

	// Token: 0x0400376E RID: 14190
	public const string BODY = "body";

	// Token: 0x0400376F RID: 14191
	public const string HANDS = "hands";

	// Token: 0x04003770 RID: 14192
	public const string FEET = "feet";

	// Token: 0x04003771 RID: 14193
	public const string HELMET = "helmet";

	// Token: 0x04003772 RID: 14194
	public const string TORSO = "torso";

	// Token: 0x04003773 RID: 14195
	public const string GLOVES = "gloves";

	// Token: 0x04003774 RID: 14196
	public const string BOOTS = "boots";

	// Token: 0x04003775 RID: 14197
	public const string SEX_MARKER = "{sex}";

	// Token: 0x04003776 RID: 14198
	public const string RACE_MARKER = "{race}";

	// Token: 0x04003777 RID: 14199
	public const string VARIANT_MARKER = "{variant}";

	// Token: 0x04003778 RID: 14200
	public const string HAIR_MARKER = "{hair}";

	// Token: 0x04003779 RID: 14201
	[PublicizedFrom(EAccessModifier.Private)]
	public static Archetype tmpArchetype;

	// Token: 0x0400377A RID: 14202
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] baseParts = new string[]
	{
		"head",
		"body",
		"hands",
		"feet"
	};

	// Token: 0x0400377B RID: 14203
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] ignoredParts = new string[0];

	// Token: 0x0400377C RID: 14204
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] basePartsFP = new string[]
	{
		"body",
		"hands"
	};

	// Token: 0x0400377D RID: 14205
	[PublicizedFrom(EAccessModifier.Private)]
	public static string[] ignoredPartsFP = new string[]
	{
		"head",
		"helmet",
		"feet",
		"boots"
	};

	// Token: 0x0200095C RID: 2396
	public class SlotData
	{
		// Token: 0x0400377E RID: 14206
		public string PrefabName;

		// Token: 0x0400377F RID: 14207
		public string PartName;

		// Token: 0x04003780 RID: 14208
		public string BaseToTurnOff;

		// Token: 0x04003781 RID: 14209
		public float CullDistance = 0.32f;

		// Token: 0x04003782 RID: 14210
		public SDCSUtils.SlotData.HairMaskTypes HairMaskType;

		// Token: 0x04003783 RID: 14211
		public SDCSUtils.SlotData.HairMaskTypes FacialHairMaskType;

		// Token: 0x0200095D RID: 2397
		public enum HairMaskTypes
		{
			// Token: 0x04003785 RID: 14213
			Full,
			// Token: 0x04003786 RID: 14214
			Hat,
			// Token: 0x04003787 RID: 14215
			Bald,
			// Token: 0x04003788 RID: 14216
			None
		}
	}

	// Token: 0x0200095E RID: 2398
	public class TransformCatalog : Dictionary<string, Transform>
	{
		// Token: 0x0600486A RID: 18538 RVA: 0x001C72F9 File Offset: 0x001C54F9
		public TransformCatalog(Transform _transform)
		{
			this.AddRecursive(_transform);
		}

		// Token: 0x0600486B RID: 18539 RVA: 0x001C7308 File Offset: 0x001C5508
		[PublicizedFrom(EAccessModifier.Private)]
		public void AddRecursive(Transform _transform)
		{
			string name = _transform.name;
			if (base.ContainsKey(name))
			{
				base[name] = _transform;
			}
			else
			{
				base.Add(name, _transform);
			}
			foreach (object obj in _transform)
			{
				Transform transform = (Transform)obj;
				this.AddRecursive(transform);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using Platform;
using UniLinq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

// Token: 0x02000010 RID: 16
public class CharacterConstruct : MonoBehaviour
{
	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000031 RID: 49 RVA: 0x000051BA File Offset: 0x000033BA
	// (set) Token: 0x06000032 RID: 50 RVA: 0x000051C2 File Offset: 0x000033C2
	public bool ShowCharacters
	{
		get
		{
			return this.showCharacters;
		}
		set
		{
			this.showCharacters = value;
			this.ShowAllCharacters();
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000033 RID: 51 RVA: 0x000051D1 File Offset: 0x000033D1
	// (set) Token: 0x06000034 RID: 52 RVA: 0x000051D9 File Offset: 0x000033D9
	public bool ShowGear
	{
		get
		{
			return this.showGear;
		}
		set
		{
			this.showGear = value;
			this.ShowAllCharacters();
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000035 RID: 53 RVA: 0x000051E8 File Offset: 0x000033E8
	// (set) Token: 0x06000036 RID: 54 RVA: 0x000051F0 File Offset: 0x000033F0
	public bool ShowHair
	{
		get
		{
			return this.showHair;
		}
		set
		{
			this.showHair = value;
			this.ShowAllCharacters();
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000037 RID: 55 RVA: 0x000051FF File Offset: 0x000033FF
	// (set) Token: 0x06000038 RID: 56 RVA: 0x00005207 File Offset: 0x00003407
	public bool ShowHatHair
	{
		get
		{
			return this.showHatHair;
		}
		set
		{
			this.showHatHair = value;
			this.ShowAllCharacters();
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000039 RID: 57 RVA: 0x00005216 File Offset: 0x00003416
	// (set) Token: 0x0600003A RID: 58 RVA: 0x0000521E File Offset: 0x0000341E
	public bool ShowFacialHair
	{
		get
		{
			return this.showFacialHair;
		}
		set
		{
			this.showFacialHair = value;
			this.ShowAllCharacters();
		}
	}

	// Token: 0x0600003B RID: 59 RVA: 0x0000522D File Offset: 0x0000342D
	[PublicizedFrom(EAccessModifier.Private)]
	public string EntityClassName(string sex)
	{
		return string.Format("player{0}", sex);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x0000523C File Offset: 0x0000343C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Awake()
	{
		LoadManager.Init();
		PlatformManager.Init();
		this.characterRoot = new GameObject("CharacterRoot");
		this.characterRoot.transform.SetParent(base.transform);
		this.characterRoot.transform.position = new Vector3(0f, 0f, 0f);
		this.gearRoot = new GameObject("GearRoot");
		this.gearRoot.transform.SetParent(base.transform);
		this.gearRoot.transform.position = new Vector3(0f, 0f, 0f);
		this.hairRoot = new GameObject("HairRoot");
		this.hairRoot.transform.SetParent(base.transform);
		this.hairRoot.transform.position = new Vector3(0f, 0f, 0f);
		this.hatHairRoot = new GameObject("HatHairRoot");
		this.hatHairRoot.transform.SetParent(base.transform);
		this.hatHairRoot.transform.position = new Vector3(0f, 0f, 0f);
		this.facialHairRoot = new GameObject("FacialHairRoot");
		this.facialHairRoot.transform.SetParent(base.transform);
		this.facialHairRoot.transform.position = new Vector3(0f, 0f, 0f);
		for (int i = 0; i < this.sexes.Length; i++)
		{
			GameObject gameObject = new GameObject((i == 0) ? "Male" : "Female");
			gameObject.transform.SetParent(this.characterRoot.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			for (int j = 0; j < this.races.Length; j++)
			{
				for (int k = 0; k < this.variants.Length; k++)
				{
					Archetype archetype = new Archetype("", i == 0, true);
					archetype.Sex = this.sexes[i];
					archetype.Race = this.races[j];
					archetype.Variant = this.variants[k];
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, gameObject.transform);
					SDCSUtils.TransformCatalog transformCatalog = new SDCSUtils.TransformCatalog(gameObject2.transform);
					CharacterConstructUtils.CreateViz(archetype, ref gameObject2, ref transformCatalog);
					gameObject2.name = archetype.Race + this.variants[k].ToString("00");
					Vector3 vector = new Vector3((float)(k + i * 4), 0f, (float)j);
					vector -= new Vector3(3.5f, 0f, 0f);
					gameObject2.transform.SetParent(gameObject.transform);
					gameObject2.transform.localPosition = vector;
					gameObject2.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
					this.Create3DTextLabel(gameObject2, this.sexes[i] + "_" + gameObject2.name);
					this.AddClickableCollider(gameObject2);
					this.allCharacters.Add(gameObject2);
				}
			}
		}
		for (int l = 0; l < this.gear.Length; l++)
		{
			this.allGearKeys.Add(this.gear[l]);
			this.gearPathLookup.Add(this.gear[l], string.Concat(new string[]
			{
				"@:Entities/Player/{sex}/Gear/",
				this.gear[l],
				"/gear{sex}",
				this.gear[l],
				"Prefab.prefab"
			}));
			this.headgearPathLookup.Add(this.gear[l], string.Concat(new string[]
			{
				"@:Entities/Player/{sex}/Gear/",
				this.gear[l],
				"/HeadgearMorphMatrix/{race}{variant}/gear",
				this.gear[l],
				"Head.asset"
			}));
		}
		for (int m = 0; m < this.dlcGear.Length; m++)
		{
			this.allGearKeys.Add(this.dlcGear[m]);
			this.gearPathLookup.Add(this.dlcGear[m], string.Concat(new string[]
			{
				"@:DLC/",
				this.dlcGear[m],
				"Cosmetic/Entities/Player/{sex}/Gear/Prefabs/gear{sex}",
				this.dlcGear[m],
				"Prefab.prefab"
			}));
			this.headgearPathLookup.Add(this.dlcGear[m], string.Concat(new string[]
			{
				"@:DLC/",
				this.dlcGear[m],
				"Cosmetic/Entities/Player/{sex}/Gear/HeadgearMorphMatrix/{race}{variant}/gear",
				this.dlcGear[m],
				"{hair}Head.asset"
			}));
		}
		for (int n = 0; n < this.twitchDropGear.Length; n++)
		{
			string text = this.twitchDropGear[n].Replace("StoreGear", "");
			this.allGearKeys.Add(this.twitchDropGear[n]);
			this.gearPathLookup.Add(this.twitchDropGear[n], string.Concat(new string[]
			{
				"@:TwitchDrops/",
				this.twitchDropGear[n],
				"/Entities/Player/{sex}/Gear/Prefabs/gear{sex}",
				text,
				"Prefab.prefab"
			}));
			this.headgearPathLookup.Add(this.twitchDropGear[n], string.Concat(new string[]
			{
				"@:TwitchDrops/",
				this.twitchDropGear[n],
				"/Entities/Player/{sex}/Gear/HeadgearMorphMatrix/{race}{variant}/gear",
				text,
				"{hair}Head.asset"
			}));
		}
		this.CreateGearInstances();
		this.CreateHairInstances();
		this.CreateHatHairInstances();
		this.CreateFacialHairInstances();
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00005828 File Offset: 0x00003A28
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateGearInstances()
	{
		foreach (GameObject obj in this.allGear)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.allGear.Clear();
		foreach (object obj2 in this.gearRoot.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		for (int i = 0; i < this.sexes.Length; i++)
		{
			GameObject gameObject = new GameObject((i == 0) ? "Male" : "Female");
			gameObject.transform.SetParent(this.gearRoot.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			for (int j = 0; j < this.allGearKeys.Count; j++)
			{
				Archetype archetype = new Archetype("", i == 0, true);
				archetype.Sex = this.sexes[i];
				archetype.Race = this.races[this.selectedRaceIndex];
				archetype.Variant = this.variants[this.selectedVariantIndex];
				archetype.Equipment = new List<SDCSUtils.SlotData>();
				for (int k = 0; k < CharacterConstruct.baseParts.Length; k++)
				{
					SDCSUtils.SlotData slotData = new SDCSUtils.SlotData();
					slotData.PartName = CharacterConstruct.baseParts[k];
					if (CharacterConstruct.baseParts[k] != "head" || this.headHiders.Contains(this.allGearKeys[j]) || this.noMorphs.Contains(this.allGearKeys[j]))
					{
						slotData.PrefabName = this.gearPathLookup[this.allGearKeys[j]];
						slotData.BaseToTurnOff = ((CharacterConstruct.baseParts[k] == "head" && !this.headHiders.Contains(this.allGearKeys[j])) ? null : CharacterConstruct.baseParts[k]);
					}
					else
					{
						slotData.PrefabName = this.headgearPathLookup[this.allGearKeys[j]];
						slotData.BaseToTurnOff = null;
					}
					slotData.CullDistance = 0.32f;
					archetype.Equipment.Add(slotData);
				}
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, gameObject.transform);
				SDCSUtils.TransformCatalog transformCatalog = new SDCSUtils.TransformCatalog(gameObject2.transform);
				CharacterConstructUtils.CreateViz(archetype, ref gameObject2, ref transformCatalog);
				gameObject2.name = this.allGearKeys[j];
				Vector3 localPosition = new Vector3((float)(j + 5), 0f, (float)i);
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.localPosition = localPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				this.Create3DTextLabel(gameObject2, archetype.Sex + "_" + this.allGearKeys[j]);
				this.AddClickableCollider(gameObject2);
				this.allGear.Add(gameObject2);
			}
		}
		foreach (GameObject gameObject3 in this.allGear)
		{
			gameObject3.SetActive(this.showGear);
		}
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00005BE0 File Offset: 0x00003DE0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateHairInstances()
	{
		foreach (GameObject obj in this.allHair)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.allHair.Clear();
		foreach (object obj2 in this.hairRoot.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		for (int i = 0; i < this.sexes.Length; i++)
		{
			GameObject gameObject = new GameObject((i == 0) ? "Male" : "Female");
			gameObject.transform.SetParent(this.hairRoot.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			for (int j = -1; j < this.hairs.Length; j++)
			{
				Archetype archetype = new Archetype("", i == 0, true);
				archetype.Sex = this.sexes[i];
				archetype.Race = this.races[this.selectedRaceIndex];
				archetype.Variant = this.variants[this.selectedVariantIndex];
				archetype.Hair = ((j == -1) ? "none" : this.hairs[j]);
				archetype.HairColor = "04 Brown";
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, gameObject.transform);
				SDCSUtils.TransformCatalog transformCatalog = new SDCSUtils.TransformCatalog(gameObject2.transform);
				CharacterConstructUtils.CreateViz(archetype, ref gameObject2, ref transformCatalog);
				gameObject2.name = ((j == -1) ? "none" : archetype.Hair);
				Vector3 localPosition = new Vector3((float)(j + 6), 0f, (float)(i + 2));
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.localPosition = localPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				this.Create3DTextLabel(gameObject2, (j == -1) ? "none" : (archetype.Sex + "_" + archetype.Hair));
				this.AddClickableCollider(gameObject2);
				this.allHair.Add(gameObject2);
			}
		}
		foreach (GameObject gameObject3 in this.allHair)
		{
			gameObject3.SetActive(this.showHair);
		}
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00005EA0 File Offset: 0x000040A0
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateHatHairInstances()
	{
		foreach (GameObject obj in this.allHatHair)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.allHatHair.Clear();
		foreach (object obj2 in this.hatHairRoot.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		for (int i = 0; i < this.sexes.Length; i++)
		{
			GameObject gameObject = new GameObject((i == 0) ? "Male" : "Female");
			gameObject.transform.SetParent(this.hatHairRoot.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
			for (int j = -1; j < this.hairs.Length; j++)
			{
				Archetype archetype = new Archetype("", i == 0, true);
				archetype.Sex = this.sexes[i];
				archetype.Race = this.races[this.selectedRaceIndex];
				archetype.Variant = this.variants[this.selectedVariantIndex];
				archetype.Hair = ((j == -1) ? "none" : this.hairs[j]);
				archetype.HairColor = "04 Brown";
				archetype.Equipment = new List<SDCSUtils.SlotData>();
				SDCSUtils.SlotData slotData = new SDCSUtils.SlotData();
				slotData.PartName = CharacterConstruct.baseParts[0];
				if (this.headHiders.Contains(this.allGearKeys[this.hatHairGearIndex]) || this.noMorphs.Contains(this.allGearKeys[this.hatHairGearIndex]))
				{
					slotData.PrefabName = this.gearPathLookup[this.allGearKeys[this.hatHairGearIndex]];
					slotData.BaseToTurnOff = ((!this.headHiders.Contains(this.allGearKeys[this.hatHairGearIndex])) ? null : CharacterConstruct.baseParts[0]);
				}
				else
				{
					slotData.PrefabName = this.headgearPathLookup[this.allGearKeys[this.hatHairGearIndex]];
					slotData.BaseToTurnOff = null;
				}
				slotData.HairMaskType = SDCSUtils.SlotData.HairMaskTypes.Hat;
				slotData.CullDistance = 0.32f;
				archetype.Equipment.Add(slotData);
				GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, gameObject.transform);
				SDCSUtils.TransformCatalog transformCatalog = new SDCSUtils.TransformCatalog(gameObject2.transform);
				CharacterConstructUtils.CreateViz(archetype, ref gameObject2, ref transformCatalog);
				gameObject2.name = ((j == -1) ? "none" : (archetype.Hair + "_hat"));
				Vector3 localPosition = new Vector3((float)(j + 6), 0f, (float)(i + 4));
				gameObject2.transform.SetParent(gameObject.transform);
				gameObject2.transform.localPosition = localPosition;
				gameObject2.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
				this.Create3DTextLabel(gameObject2, (j == -1) ? "none" : (archetype.Sex + "_" + archetype.Hair));
				this.AddClickableCollider(gameObject2);
				this.allHatHair.Add(gameObject2);
			}
		}
		foreach (GameObject gameObject3 in this.allHatHair)
		{
			gameObject3.SetActive(this.showHatHair);
		}
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00006268 File Offset: 0x00004468
	[PublicizedFrom(EAccessModifier.Private)]
	public void CreateFacialHairInstances()
	{
		foreach (GameObject obj in this.allFacialHair)
		{
			UnityEngine.Object.Destroy(obj);
		}
		this.allFacialHair.Clear();
		foreach (object obj2 in this.facialHairRoot.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj2).gameObject);
		}
		Archetype archetype = new Archetype("", true, true);
		archetype.Sex = this.sexes[0];
		archetype.Race = this.races[this.selectedRaceIndex];
		archetype.Variant = this.variants[this.selectedVariantIndex];
		for (int i = 0; i < this.facialHairIndexes.Length; i++)
		{
			archetype.Hair = this.hairs[0];
			archetype.HairColor = "04 Brown";
			archetype.MustacheName = this.facialHairIndexes[i].ToString();
			archetype.ChopsName = this.facialHairIndexes[i].ToString();
			archetype.BeardName = this.facialHairIndexes[i].ToString();
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.baseRig, this.facialHairRoot.transform);
			SDCSUtils.TransformCatalog transformCatalog = new SDCSUtils.TransformCatalog(gameObject.transform);
			CharacterConstructUtils.CreateViz(archetype, ref gameObject, ref transformCatalog);
			gameObject.name = "FacialHair " + i.ToString("00");
			Vector3 localPosition = new Vector3((float)(i + 5), 0f, 6f);
			gameObject.transform.SetParent(this.facialHairRoot.transform);
			gameObject.transform.localPosition = localPosition;
			gameObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
			this.Create3DTextLabel(gameObject, "FacialHair " + i.ToString("00"));
			this.AddClickableCollider(gameObject);
			this.allFacialHair.Add(gameObject);
		}
		foreach (GameObject gameObject2 in this.allFacialHair)
		{
			gameObject2.SetActive(this.showFacialHair);
		}
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000064F4 File Offset: 0x000046F4
	public void RespawnAllGroups()
	{
		this.ShowAllCharacters();
		this.CreateGearInstances();
		this.CreateHairInstances();
		this.CreateHatHairInstances();
		this.CreateFacialHairInstances();
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00006514 File Offset: 0x00004714
	public void RespawnHatHairGroup()
	{
		this.ShowAllCharacters();
		this.CreateHatHairInstances();
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00006522 File Offset: 0x00004722
	public string[] GetGearTypes()
	{
		return this.allGearKeys.ToArray();
	}

	// Token: 0x06000044 RID: 68 RVA: 0x0000652F File Offset: 0x0000472F
	public string[] GetRaceTypes()
	{
		return this.races;
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00006538 File Offset: 0x00004738
	public string[] GetVariantTypes()
	{
		string[] array = new string[this.variants.Length];
		for (int i = 0; i < this.variants.Length; i++)
		{
			array[i] = this.variants[i].ToString();
		}
		return array;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x0000657C File Offset: 0x0000477C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Create3DTextLabel(GameObject instance, string labelText)
	{
		GameObject gameObject = new GameObject(instance.name + "_Label");
		gameObject.transform.SetParent(instance.transform);
		gameObject.transform.localPosition = new Vector3(0f, this.labelHeight, 0f);
		GameObject gameObject2 = GameObject.CreatePrimitive(PrimitiveType.Quad);
		gameObject2.name = "LabelBackground";
		gameObject2.transform.SetParent(gameObject.transform);
		gameObject2.transform.localPosition = new Vector3(0f, 0f, 0.01f);
		gameObject2.transform.localScale = new Vector3((float)labelText.Length * 0.175f, 0.4f, 1f);
		MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
		component.material = new Material(Shader.Find("Unlit/Color"));
		component.material.color = new Color(0f, 0f, 0f, 0.7f);
		component.shadowCastingMode = ShadowCastingMode.Off;
		component.receiveShadows = false;
		GameObject gameObject3 = new GameObject("Text");
		gameObject3.transform.SetParent(gameObject.transform);
		gameObject3.transform.localPosition = Vector3.zero;
		TextMesh textMesh = gameObject3.AddComponent<TextMesh>();
		textMesh.text = labelText;
		textMesh.fontSize = 30;
		textMesh.characterSize = 0.1f;
		textMesh.alignment = TextAlignment.Center;
		textMesh.anchor = TextAnchor.MiddleCenter;
		textMesh.color = this.labelColor;
		MeshRenderer component2 = textMesh.GetComponent<MeshRenderer>();
		component2.shadowCastingMode = ShadowCastingMode.Off;
		component2.receiveShadows = false;
		component2.sortingOrder = 1;
		gameObject.transform.localScale = new Vector3(this.labelScale, this.labelScale, this.labelScale);
	}

	// Token: 0x06000047 RID: 71 RVA: 0x00006724 File Offset: 0x00004924
	[PublicizedFrom(EAccessModifier.Private)]
	public void AddClickableCollider(GameObject instance)
	{
		Collider[] componentsInChildren = instance.GetComponentsInChildren<Collider>();
		if (componentsInChildren.Length == 0)
		{
			Debug.LogWarning("No colliders found in " + instance.name + ". Character won't be clickable.");
			return;
		}
		instance.AddComponent<CharacterClickHandler>().parentScript = this;
		Collider[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.AddComponent<CharacterColliderHelper>().mainInstance = instance;
		}
	}

	// Token: 0x06000048 RID: 72 RVA: 0x00006786 File Offset: 0x00004986
	public void OnCharacterClicked(GameObject clickedCharacter)
	{
		if (this.selectedCharacter == clickedCharacter)
		{
			this.ShowAllCharacters();
			return;
		}
		this.HideAllExcept(clickedCharacter);
	}

	// Token: 0x06000049 RID: 73 RVA: 0x000067A4 File Offset: 0x000049A4
	public void ShowAllCharacters()
	{
		this.selectedCharacter = null;
		this.allCharactersVisible = true;
		foreach (GameObject gameObject in this.allCharacters)
		{
			gameObject.SetActive(this.showCharacters);
		}
		foreach (GameObject gameObject2 in this.allGear)
		{
			gameObject2.SetActive(this.showGear);
		}
		foreach (GameObject gameObject3 in this.allHair)
		{
			gameObject3.SetActive(this.showHair);
		}
		foreach (GameObject gameObject4 in this.allHatHair)
		{
			gameObject4.SetActive(this.showHatHair);
		}
		foreach (GameObject gameObject5 in this.allFacialHair)
		{
			gameObject5.SetActive(this.showFacialHair);
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x0000691C File Offset: 0x00004B1C
	[PublicizedFrom(EAccessModifier.Private)]
	public void HideAllExcept(GameObject exception)
	{
		this.selectedCharacter = exception;
		this.allCharactersVisible = false;
		foreach (GameObject gameObject in this.allCharacters)
		{
			gameObject.SetActive(gameObject == exception);
		}
		foreach (GameObject gameObject2 in this.allGear)
		{
			gameObject2.SetActive(gameObject2 == exception);
		}
		foreach (GameObject gameObject3 in this.allHair)
		{
			gameObject3.SetActive(gameObject3 == exception);
		}
		foreach (GameObject gameObject4 in this.allHatHair)
		{
			gameObject4.SetActive(gameObject4 == exception);
		}
		foreach (GameObject gameObject5 in this.allFacialHair)
		{
			gameObject5.SetActive(gameObject5 == exception);
		}
	}

	// Token: 0x0600004B RID: 75 RVA: 0x00006A9C File Offset: 0x00004C9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void Update()
	{
		if (this.allCharactersVisible)
		{
			this.characterRoot.SetActive(this.showCharacters);
			this.gearRoot.SetActive(this.showGear);
			this.hairRoot.SetActive(this.showHair);
			this.hatHairRoot.SetActive(this.showHatHair);
			this.facialHairRoot.SetActive(this.showFacialHair);
		}
		if (Input.GetMouseButtonDown(0))
		{
			this.CheckMouseClick();
		}
	}

	// Token: 0x0600004C RID: 76 RVA: 0x00006B14 File Offset: 0x00004D14
	[PublicizedFrom(EAccessModifier.Private)]
	public void CheckMouseClick()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, out raycastHit))
		{
			CharacterColliderHelper component = raycastHit.collider.gameObject.GetComponent<CharacterColliderHelper>();
			if (component != null && component.mainInstance != null)
			{
				CharacterClickHandler component2 = component.mainInstance.GetComponent<CharacterClickHandler>();
				if (component2 != null)
				{
					component2.HandleClick();
				}
			}
		}
	}

	// Token: 0x04000073 RID: 115
	public GameObject baseRig;

	// Token: 0x04000074 RID: 116
	public int hairColorIndex;

	// Token: 0x04000075 RID: 117
	public int hatHairGearIndex;

	// Token: 0x04000076 RID: 118
	public int selectedRaceIndex;

	// Token: 0x04000077 RID: 119
	public int selectedVariantIndex;

	// Token: 0x04000078 RID: 120
	public float labelHeight = 2f;

	// Token: 0x04000079 RID: 121
	public float labelScale = 0.1f;

	// Token: 0x0400007A RID: 122
	public Color labelColor = Color.white;

	// Token: 0x0400007B RID: 123
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool showCharacters = true;

	// Token: 0x0400007C RID: 124
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool showGear;

	// Token: 0x0400007D RID: 125
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool showHair;

	// Token: 0x0400007E RID: 126
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool showHatHair;

	// Token: 0x0400007F RID: 127
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool showFacialHair;

	// Token: 0x04000080 RID: 128
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject selectedCharacter;

	// Token: 0x04000081 RID: 129
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public bool allCharactersVisible = true;

	// Token: 0x04000082 RID: 130
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] sexes = new string[]
	{
		"Male",
		"Female"
	};

	// Token: 0x04000083 RID: 131
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] races = new string[]
	{
		"White",
		"Black",
		"Asian",
		"Native"
	};

	// Token: 0x04000084 RID: 132
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] variants = new int[]
	{
		1,
		2,
		3,
		4
	};

	// Token: 0x04000085 RID: 133
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] hairs = new string[]
	{
		"buzzcut",
		"comb_over",
		"slicked_back",
		"slicked_back_long",
		"pixie_cut",
		"ponytail",
		"midpart_karen_messy",
		"midpart_long",
		"midpart_mid",
		"midpart_short",
		"midpart_shoulder",
		"sidepart_short",
		"sidepart_mid",
		"sidepart_long",
		"cornrows",
		"mohawk",
		"flattop_fro",
		"small_fro",
		"dreads",
		"afro_curly"
	};

	// Token: 0x04000086 RID: 134
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public int[] facialHairIndexes = new int[]
	{
		1,
		2,
		3,
		4,
		5
	};

	// Token: 0x04000087 RID: 135
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] gear = new string[]
	{
		"Assassin",
		"Biker",
		"Commando",
		"Enforcer",
		"Farmer",
		"Fiber",
		"Fitness",
		"LumberJack",
		"Miner",
		"Nerd",
		"Nomad",
		"Preacher",
		"Raider",
		"Ranger",
		"Scavenger",
		"Stealth"
	};

	// Token: 0x04000088 RID: 136
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] dlcGear = new string[]
	{
		"DarkKnight",
		"Desert",
		"Hoarder",
		"Marauder",
		"CrimsonWarlord",
		"Samurai"
	};

	// Token: 0x04000089 RID: 137
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] twitchDropGear = new string[]
	{
		"Watcher",
		"PimpHatBlue",
		"PimpHatPurple"
	};

	// Token: 0x0400008A RID: 138
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<string> allGearKeys = new List<string>();

	// Token: 0x0400008B RID: 139
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, string> gearPathLookup = new Dictionary<string, string>();

	// Token: 0x0400008C RID: 140
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public Dictionary<string, string> headgearPathLookup = new Dictionary<string, string>();

	// Token: 0x0400008D RID: 141
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] headHiders = new string[]
	{
		"Assassin",
		"Fiber",
		"Nomad",
		"Raider",
		"Watcher"
	};

	// Token: 0x0400008E RID: 142
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public string[] noMorphs = new string[]
	{
		"Hoarder",
		"Marauder"
	};

	// Token: 0x0400008F RID: 143
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public static string[] baseParts = new string[]
	{
		"head",
		"body",
		"hands",
		"feet"
	};

	// Token: 0x04000090 RID: 144
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject characterRoot;

	// Token: 0x04000091 RID: 145
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject gearRoot;

	// Token: 0x04000092 RID: 146
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject hairRoot;

	// Token: 0x04000093 RID: 147
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject hatHairRoot;

	// Token: 0x04000094 RID: 148
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public GameObject facialHairRoot;

	// Token: 0x04000095 RID: 149
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> allCharacters = new List<GameObject>();

	// Token: 0x04000096 RID: 150
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> allGear = new List<GameObject>();

	// Token: 0x04000097 RID: 151
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> allHair = new List<GameObject>();

	// Token: 0x04000098 RID: 152
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> allHatHair = new List<GameObject>();

	// Token: 0x04000099 RID: 153
	[PublicizedFrom(EAccessModifier.Private)]
	[NonSerialized]
	public List<GameObject> allFacialHair = new List<GameObject>();
}

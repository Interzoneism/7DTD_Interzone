using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using InControl;
using UnityEngine;

// Token: 0x02000FC5 RID: 4037
public class GUIWindowScreenshotText : GUIWindow
{
	// Token: 0x060080B5 RID: 32949 RVA: 0x00344184 File Offset: 0x00342384
	public GUIWindowScreenshotText() : base(GUIWindowScreenshotText.ID, new Rect(20f, 70f, (float)(Screen.width / 3), 1f))
	{
		this.alwaysUsesMouseCursor = true;
	}

	// Token: 0x060080B6 RID: 32950 RVA: 0x003441B4 File Offset: 0x003423B4
	public override void OnGUI(bool _inputActive)
	{
		base.OnGUI(_inputActive);
		Vector2i vector2i = new Vector2i(Screen.width, Screen.height);
		if (this.lastResolution != vector2i)
		{
			this.lastResolution = vector2i;
			this.labelStyle = new GUIStyle(GUI.skin.label);
			this.checkboxStyle = new GUIStyle(GUI.skin.toggle);
			this.textfieldStyle = new GUIStyle(GUI.skin.textField);
			this.buttonStyle = new GUIStyle(GUI.skin.button);
			this.labelStyle.wordWrap = true;
			this.labelStyle.fontStyle = FontStyle.Bold;
			this.fontSize = vector2i.y / 54;
			this.lineHeight = this.fontSize + 3;
			this.inputAreaHeight = this.fontSize + 10;
			this.labelStyle.fontSize = this.fontSize;
			this.checkboxStyle.fontSize = this.fontSize;
			this.textfieldStyle.fontSize = this.fontSize;
			this.buttonStyle.fontSize = this.fontSize;
		}
		if (Event.current.type == EventType.KeyDown)
		{
			if (Event.current.keyCode == KeyCode.Escape)
			{
				this.CloseWindow();
			}
			else if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter)
			{
				this.DoScreenshot();
			}
		}
		float xMin = base.windowRect.xMin;
		float yMin = base.windowRect.yMin;
		float width = base.windowRect.width;
		float num = 0f;
		for (int i = 0; i < 2; i++)
		{
			float num2 = yMin;
			if (i == 1)
			{
				GUI.Box(new Rect(xMin, num2, width, num - yMin + 5f), "");
			}
			if (GameManager.Instance.World != null)
			{
				World world = GameManager.Instance.World;
				GameUtils.WorldInfo worldInfo;
				if (world == null)
				{
					worldInfo = null;
				}
				else
				{
					ChunkCluster chunkCache = world.ChunkCache;
					if (chunkCache == null)
					{
						worldInfo = null;
					}
					else
					{
						IChunkProvider chunkProvider = chunkCache.ChunkProvider;
						worldInfo = ((chunkProvider != null) ? chunkProvider.WorldInfo : null);
					}
				}
				GameUtils.WorldInfo worldInfo2 = worldInfo;
				if (i == 1)
				{
					string text = "World: " + GamePrefs.GetString(EnumGamePrefs.GameWorld);
					Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, (float)this.inputAreaHeight), text, this.labelStyle, Color.black, Color.white);
				}
				num2 += (float)this.lineHeight;
				if (!PrefabEditModeManager.Instance.IsActive() && worldInfo2 != null && worldInfo2.RandomGeneratedWorld && worldInfo2.DynamicProperties.Contains("Generation.Seed"))
				{
					if (i == 1)
					{
						Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, (float)this.inputAreaHeight), "World gen seed: " + worldInfo2.DynamicProperties.GetStringValue("Generation.Seed"), this.labelStyle, Color.black, Color.white);
					}
					num2 += (float)this.lineHeight;
				}
				if (i == 1)
				{
					Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, (float)this.inputAreaHeight), "Save name / deco seed: " + (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer ? GamePrefs.GetString(EnumGamePrefs.GameName) : GamePrefs.GetString(EnumGamePrefs.GameNameClient)), this.labelStyle, Color.black, Color.white);
				}
				num2 += (float)this.lineHeight;
				if (LocalPlayerUI.GetUIForPrimaryPlayer() != null)
				{
					EntityPlayer entityPlayer = LocalPlayerUI.GetUIForPrimaryPlayer().entityPlayer;
					if (entityPlayer != null)
					{
						PrefabInstance prefab = entityPlayer.prefab;
						if (i == 1)
						{
							string text2 = string.Format("Coordinates: {0:F0} {1:F0} {2:F0}", entityPlayer.position.x, entityPlayer.position.y, entityPlayer.position.z);
							if (prefab != null)
							{
								text2 += string.Format(" / relative to POI: {0}", prefab.GetPositionRelativeToPoi(Vector3i.Floor(entityPlayer.position)));
							}
							Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, (float)this.inputAreaHeight), text2, this.labelStyle, Color.black, Color.white);
						}
						num2 += (float)this.lineHeight;
						if (prefab != null)
						{
							Prefab prefab2 = prefab.prefab;
							string text3 = ((prefab2 != null) ? prefab2.PrefabName : null) ?? prefab.name;
							Prefab prefab3 = prefab.prefab;
							string text4 = ((prefab3 != null) ? prefab3.LocalizedEnglishName : null) ?? "";
							if (i == 1)
							{
								Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, (float)this.inputAreaHeight), string.Concat(new string[]
								{
									"POI: ",
									text3,
									" (",
									text4,
									")"
								}), this.labelStyle, Color.black, Color.white);
							}
							num2 += (float)this.lineHeight;
						}
						if (!this.confirmed)
						{
							if (i == 1)
							{
								this.savePerks = GUI.Toggle(new Rect(xMin + 5f, num2, width - 10f, (float)this.lineHeight), this.savePerks, "Save Perks, Buffs and CVars", this.checkboxStyle);
							}
							num2 += (float)this.lineHeight;
						}
					}
				}
				num2 += (float)this.lineHeight;
			}
			if (!this.confirmed)
			{
				if (i == 1)
				{
					GUI.SetNextControlName("InputField");
					this.noteInput = GUI.TextField(new Rect(xMin + 5f, num2, width - 60f, (float)this.inputAreaHeight), this.noteInput, 300, this.textfieldStyle);
					if (this.bFirstTime)
					{
						this.bFirstTime = false;
						GUI.FocusControl("InputField");
					}
					if (GUI.Button(new Rect(xMin + width - 50f, num2, 50f, (float)this.inputAreaHeight), "Ok", this.buttonStyle))
					{
						this.DoScreenshot();
						return;
					}
				}
				num2 += (float)this.inputAreaHeight;
			}
			else
			{
				float num3 = this.labelStyle.CalcHeight(new GUIContent("Note: " + this.noteInput), width - 10f);
				if (i == 1)
				{
					Utils.DrawOutline(new Rect(xMin + 5f, num2, width - 10f, num3 + 4f), "Note: " + this.noteInput, this.labelStyle, Color.black, Color.white);
				}
				num2 += num3;
			}
			num = num2;
		}
		if (this.nGuiWdwDebugPanels == null)
		{
			this.nGuiWdwDebugPanels = UnityEngine.Object.FindObjectOfType<NGuiWdwDebugPanels>();
		}
		if (this.nGuiWdwDebugPanels != null)
		{
			this.nGuiWdwDebugPanels.showDebugPanel_FocusedBlock((int)xMin, (int)num + 10, true);
		}
	}

	// Token: 0x060080B7 RID: 32951 RVA: 0x00341D33 File Offset: 0x0033FF33
	[PublicizedFrom(EAccessModifier.Private)]
	public void CloseWindow()
	{
		this.windowManager.Close(this, false);
	}

	// Token: 0x060080B8 RID: 32952 RVA: 0x0034486E File Offset: 0x00342A6E
	[PublicizedFrom(EAccessModifier.Private)]
	public void DoScreenshot()
	{
		this.confirmed = true;
		ThreadManager.StartCoroutine(this.screenshotCo(null));
	}

	// Token: 0x060080B9 RID: 32953 RVA: 0x00344884 File Offset: 0x00342A84
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator screenshotCo(string _filename)
	{
		yield return null;
		bool saved = true;
		yield return ThreadManager.CoroutineWrapperWithExceptionCallback(GameUtils.TakeScreenshotEnum(GameUtils.EScreenshotMode.Both, _filename, 0f, false, 0, 0, false), delegate(Exception _exception)
		{
			saved = false;
			Log.Exception(_exception);
		});
		if (saved && this.savePerks)
		{
			this.StoreAdditionalStats();
		}
		yield return null;
		this.CloseWindow();
		yield break;
	}

	// Token: 0x060080BA RID: 32954 RVA: 0x0034489C File Offset: 0x00342A9C
	[PublicizedFrom(EAccessModifier.Private)]
	public void StoreAdditionalStats()
	{
		string text = GameUtils.lastSavedScreenshotFilename;
		text = text.Substring(0, text.LastIndexOf('.'));
		if (GameManager.Instance.World != null && GameManager.Instance.World.GetPrimaryPlayer() != null)
		{
			this.StorePlayerStats(text);
		}
	}

	// Token: 0x060080BB RID: 32955 RVA: 0x003448EC File Offset: 0x00342AEC
	[PublicizedFrom(EAccessModifier.Private)]
	public void StorePlayerStats(string _filenameBase)
	{
		EntityPlayerLocal primaryPlayer = GameManager.Instance.World.GetPrimaryPlayer();
		_filenameBase += "_playerstats.csv";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("Level,{0}", primaryPlayer.Progression.GetLevel()));
		stringBuilder.AppendLine();
		this.writePlayerSkills(stringBuilder, primaryPlayer);
		this.writePlayerBuffs(stringBuilder, primaryPlayer);
		this.writePlayerCVars(stringBuilder, primaryPlayer);
		SdFile.WriteAllText(_filenameBase, stringBuilder.ToString());
	}

	// Token: 0x060080BC RID: 32956 RVA: 0x00344968 File Offset: 0x00342B68
	[PublicizedFrom(EAccessModifier.Private)]
	public void writePlayerSkills(StringBuilder _sb, EntityPlayerLocal _epl)
	{
		List<ProgressionValue> list = new List<ProgressionValue>();
		foreach (KeyValuePair<int, ProgressionValue> keyValuePair in _epl.Progression.GetDict())
		{
			ProgressionValue value = keyValuePair.Value;
			bool flag;
			if (value == null)
			{
				flag = (null != null);
			}
			else
			{
				ProgressionClass progressionClass = value.ProgressionClass;
				flag = (((progressionClass != null) ? progressionClass.Name : null) != null);
			}
			if (flag)
			{
				list.Add(keyValuePair.Value);
			}
		}
		list.Sort(ProgressionClass.ListSortOrderComparer.Instance);
		_sb.AppendLine("Skills");
		_sb.AppendLine("Name,Level,CalcLevel");
		foreach (ProgressionValue progressionValue in list)
		{
			ProgressionClass progressionClass2 = progressionValue.ProgressionClass;
			if (progressionClass2.IsAttribute && progressionClass2.MaxLevel != 0)
			{
				_sb.AppendLine();
				_sb.AppendLine(string.Format("{0},{1},{2}", progressionClass2.Name, progressionValue.Level, progressionValue.CalculatedLevel(_epl)));
			}
			else if (progressionClass2.IsPerk)
			{
				_sb.AppendLine(string.Format(" - {0},{1},{2}", progressionClass2.Name, progressionValue.Level, progressionValue.CalculatedLevel(_epl)));
			}
		}
		_sb.AppendLine();
		_sb.AppendLine("Books");
		_sb.AppendLine("Name,Level,CalcLevel");
		foreach (ProgressionValue progressionValue2 in list)
		{
			ProgressionClass progressionClass3 = progressionValue2.ProgressionClass;
			if (progressionClass3.IsBook)
			{
				_sb.AppendLine(string.Format("{0},{1},{2}", progressionClass3.Name, progressionValue2.Level, progressionValue2.CalculatedLevel(_epl)));
			}
		}
		_sb.AppendLine();
		_sb.AppendLine("Crafting Skills");
		_sb.AppendLine("Name,Level,CalcLevel");
		foreach (ProgressionValue progressionValue3 in list)
		{
			ProgressionClass progressionClass4 = progressionValue3.ProgressionClass;
			if (progressionClass4.IsCrafting)
			{
				_sb.AppendLine(string.Format("{0},{1},{2}", progressionClass4.Name, progressionValue3.Level, progressionValue3.CalculatedLevel(_epl)));
			}
		}
		_sb.AppendLine();
	}

	// Token: 0x060080BD RID: 32957 RVA: 0x00344C18 File Offset: 0x00342E18
	[PublicizedFrom(EAccessModifier.Private)]
	public void writePlayerBuffs(StringBuilder _sb, EntityPlayerLocal _epl)
	{
		_sb.AppendLine("Buffs");
		_sb.AppendLine("Buff,FromName,FromId,Missing?");
		foreach (BuffValue buffValue in _epl.Buffs.ActiveBuffs)
		{
			BuffClass buffClass = buffValue.BuffClass;
			Entity entity = GameManager.Instance.World.GetEntity(buffValue.InstigatorId);
			string text = string.Format("none (id {0})", buffValue.InstigatorId);
			_sb.AppendLine(string.Concat(new string[]
			{
				buffValue.BuffName,
				",",
				entity ? entity.GetDebugName() : text,
				",",
				entity ? entity.entityId.ToString() : "",
				",",
				(buffClass == null) ? "BuffClass missing" : ""
			}));
		}
		_sb.AppendLine();
	}

	// Token: 0x060080BE RID: 32958 RVA: 0x00344D3C File Offset: 0x00342F3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void writePlayerCVars(StringBuilder _sb, EntityPlayerLocal _epl)
	{
		_sb.AppendLine("Buffs");
		_sb.AppendLine("Name,Value");
		foreach (KeyValuePair<string, float> keyValuePair in _epl.Buffs.CVars)
		{
			string text;
			float num;
			keyValuePair.Deconstruct(out text, out num);
			string arg = text;
			float num2 = num;
			if (num2 != 0f)
			{
				_sb.AppendLine(string.Format("{0},{1}", arg, num2));
			}
		}
		_sb.AppendLine();
	}

	// Token: 0x060080BF RID: 32959 RVA: 0x00344DE0 File Offset: 0x00342FE0
	public override void OnOpen()
	{
		this.confirmed = false;
		this.bFirstTime = true;
		this.noteInput = "";
		this.isInputActive = true;
		if (UIInput.selection != null)
		{
			UIInput.selection.isSelected = false;
		}
		InputManager.Enabled = false;
	}

	// Token: 0x060080C0 RID: 32960 RVA: 0x00344E20 File Offset: 0x00343020
	public override void OnClose()
	{
		base.OnClose();
		this.isInputActive = false;
		InputManager.Enabled = true;
	}

	// Token: 0x060080C1 RID: 32961 RVA: 0x00344E38 File Offset: 0x00343038
	public static void Open(LocalPlayerUI _playerUi, bool _savePerks)
	{
		GUIWindowScreenshotText window = _playerUi.windowManager.GetWindow<GUIWindowScreenshotText>(GUIWindowScreenshotText.ID);
		if (window != null)
		{
			window.savePerks = _savePerks;
		}
		_playerUi.windowManager.Open(GUIWindowScreenshotText.ID, false, false, true);
	}

	// Token: 0x0400636A RID: 25450
	public static readonly string ID = "GUIWindowScreenshotText";

	// Token: 0x0400636B RID: 25451
	[PublicizedFrom(EAccessModifier.Private)]
	public bool bFirstTime;

	// Token: 0x0400636C RID: 25452
	[PublicizedFrom(EAccessModifier.Private)]
	public string noteInput;

	// Token: 0x0400636D RID: 25453
	[PublicizedFrom(EAccessModifier.Private)]
	public bool savePerks;

	// Token: 0x0400636E RID: 25454
	[PublicizedFrom(EAccessModifier.Private)]
	public bool confirmed;

	// Token: 0x0400636F RID: 25455
	[PublicizedFrom(EAccessModifier.Private)]
	public Vector2i lastResolution;

	// Token: 0x04006370 RID: 25456
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIStyle labelStyle;

	// Token: 0x04006371 RID: 25457
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIStyle checkboxStyle;

	// Token: 0x04006372 RID: 25458
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIStyle textfieldStyle;

	// Token: 0x04006373 RID: 25459
	[PublicizedFrom(EAccessModifier.Private)]
	public GUIStyle buttonStyle;

	// Token: 0x04006374 RID: 25460
	[PublicizedFrom(EAccessModifier.Private)]
	public int fontSize;

	// Token: 0x04006375 RID: 25461
	[PublicizedFrom(EAccessModifier.Private)]
	public int lineHeight;

	// Token: 0x04006376 RID: 25462
	[PublicizedFrom(EAccessModifier.Private)]
	public int inputAreaHeight;

	// Token: 0x04006377 RID: 25463
	[PublicizedFrom(EAccessModifier.Private)]
	public NGuiWdwDebugPanels nGuiWdwDebugPanels;
}

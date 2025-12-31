using System;
using System.Collections.Generic;
using System.Globalization;
using Audio;
using InControl;
using UnityEngine;
using UnityEngine.Scripting;

// Token: 0x02000D3D RID: 3389
[UnityEngine.Scripting.Preserve]
public class XUiC_NewsWindow : XUiController
{
	// Token: 0x17000AB9 RID: 2745
	// (get) Token: 0x0600698F RID: 27023 RVA: 0x002ADF8F File Offset: 0x002AC18F
	// (set) Token: 0x06006990 RID: 27024 RVA: 0x002ADF98 File Offset: 0x002AC198
	public int CurrentIndex
	{
		get
		{
			return this.currentIndex;
		}
		set
		{
			if (value >= this.entries.Count)
			{
				value = this.entries.Count - 1;
			}
			if (value >= this.maxEntries)
			{
				value = this.maxEntries - 1;
			}
			if (value < 0)
			{
				value = 0;
			}
			this.currentIndex = value;
			this.IsDirty = true;
		}
	}

	// Token: 0x17000ABA RID: 2746
	// (get) Token: 0x06006991 RID: 27025 RVA: 0x002ADFEC File Offset: 0x002AC1EC
	public NewsManager.NewsEntry CurrentEntry
	{
		get
		{
			if (this.CurrentIndex < 0)
			{
				return null;
			}
			if (this.CurrentIndex >= this.entries.Count)
			{
				return null;
			}
			object obj = this.newsLock;
			NewsManager.NewsEntry result;
			lock (obj)
			{
				result = this.entries[this.CurrentIndex];
			}
			return result;
		}
	}

	// Token: 0x06006992 RID: 27026 RVA: 0x002AE05C File Offset: 0x002AC25C
	public override void Init()
	{
		base.Init();
		XUiController childById = base.GetChildById("btnYounger");
		if (childById != null)
		{
			childById.OnPress += delegate(XUiController _, int _)
			{
				this.cycle(-1, false);
			};
		}
		XUiController childById2 = base.GetChildById("btnOlder");
		if (childById2 != null)
		{
			childById2.OnPress += delegate(XUiController _, int _)
			{
				this.cycle(1, false);
			};
		}
		XUiC_SimpleButton xuiC_SimpleButton = base.GetChildById("btnLink") as XUiC_SimpleButton;
		if (xuiC_SimpleButton != null)
		{
			xuiC_SimpleButton.OnPressed += this.BtnLink_OnPressed;
		}
		else
		{
			XUiController childById3 = base.GetChildById("btnLink");
			if (childById3 != null && childById3.ViewComponent is XUiV_Button)
			{
				childById3.OnPress += this.BtnLink_OnPressed;
				childById3.OnScroll += this.OnScrollEvent;
			}
		}
		base.OnScroll += this.OnScrollEvent;
		List<XUiV_Texture> list = new List<XUiV_Texture>();
		foreach (XUiController xuiController in base.GetChildrenById("newsImage", null))
		{
			XUiV_Texture xuiV_Texture = ((xuiController != null) ? xuiController.ViewComponent : null) as XUiV_Texture;
			if (xuiV_Texture != null)
			{
				list.Add(xuiV_Texture);
			}
		}
		this.bannerTextures = list.ToArray();
		this.selector = base.GetChildByType<XUiC_ComboBoxFloat>();
		if (this.selector != null)
		{
			this.selector.OnValueChanged += this.Selector_OnValueChanged;
		}
		this.hasProviders = (this.newsProviders.Count > 0);
		if (!this.hasProviders)
		{
			Log.Warning(string.Concat(new string[]
			{
				"[XUi] News controller with no sources specified (window group '",
				base.WindowGroup.ID,
				"', window '",
				base.ViewComponent.ID,
				"')"
			}));
		}
		NewsManager.Instance.Updated += this.newsUpdated;
		this.newsUpdated(NewsManager.Instance);
	}

	// Token: 0x06006993 RID: 27027 RVA: 0x002AE233 File Offset: 0x002AC433
	[PublicizedFrom(EAccessModifier.Private)]
	public void OnScrollEvent(XUiController _sender, float _delta)
	{
		if (this.selector == null)
		{
			this.cycle(Math.Sign(_delta), true);
			return;
		}
		this.selector.ScrollEvent(_sender, _delta);
	}

	// Token: 0x06006994 RID: 27028 RVA: 0x002AE258 File Offset: 0x002AC458
	[PublicizedFrom(EAccessModifier.Private)]
	public void Selector_OnValueChanged(XUiController _sender, double _oldValue, double _newValue)
	{
		this.CurrentIndex = Mathf.CeilToInt((float)_newValue) - 1;
		this.selector.Value = (double)(this.CurrentIndex + 1);
		this.autoCycle = false;
	}

	// Token: 0x06006995 RID: 27029 RVA: 0x002AE284 File Offset: 0x002AC484
	[PublicizedFrom(EAccessModifier.Private)]
	public void newsUpdated(NewsManager _newsManager)
	{
		NewsManager.NewsEntry currentEntry = this.CurrentEntry;
		object obj = this.newsLock;
		lock (obj)
		{
			_newsManager.GetNewsData(this.newsProviders, this.entries);
		}
		int num = 0;
		while (num < this.maxEntries && num < this.entries.Count)
		{
			this.entries[num].RequestImage();
			num++;
		}
		this.IsDirty = true;
		if (currentEntry == null || !currentEntry.Equals(this.CurrentEntry))
		{
			this.resetIndex();
		}
	}

	// Token: 0x06006996 RID: 27030 RVA: 0x002AE328 File Offset: 0x002AC528
	public override void Update(float _dt)
	{
		base.Update(_dt);
		this.doAutoCycle(_dt);
		if (this.IsDirty)
		{
			base.RefreshBindings(true);
			if (this.bannerTextures != null && this.bannerTextures.Length != 0)
			{
				NewsManager.NewsEntry currentEntry = this.CurrentEntry;
				Texture2D texture = (currentEntry != null) ? currentEntry.Image : null;
				XUiV_Texture[] array = this.bannerTextures;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Texture = texture;
				}
			}
			this.IsDirty = false;
		}
	}

	// Token: 0x06006997 RID: 27031 RVA: 0x002AE39C File Offset: 0x002AC59C
	[PublicizedFrom(EAccessModifier.Private)]
	public void doAutoCycle(float _dt)
	{
		if (this.selector == null || !this.autoCycle)
		{
			return;
		}
		NewsManager.NewsEntry currentEntry = this.CurrentEntry;
		if (currentEntry == null || (currentEntry.HasImage && !currentEntry.ImageLoaded))
		{
			return;
		}
		float num = (float)this.selector.Value + _dt / this.autoCycleTimePerEntry;
		if ((double)num > this.selector.Max)
		{
			num = 0f;
		}
		this.selector.Value = (double)num;
		this.CurrentIndex = Mathf.CeilToInt((float)this.selector.Value) - 1;
	}

	// Token: 0x06006998 RID: 27032 RVA: 0x002AE428 File Offset: 0x002AC628
	[PublicizedFrom(EAccessModifier.Private)]
	public void BtnLink_OnPressed(XUiController _sender, int _mousebutton)
	{
		NewsManager.NewsEntry currentEntry = this.CurrentEntry;
		if (currentEntry == null)
		{
			return;
		}
		string url = currentEntry.Url;
		if (url == null || !url.StartsWith("openstore://"))
		{
			XUiC_MessageBoxWindowGroup.ShowUrlConfirmationDialog(base.xui, currentEntry.Url, false, null, null, null, null);
			return;
		}
		EntitlementSetEnum entitlementSetEnum;
		try
		{
			entitlementSetEnum = Enum.Parse<EntitlementSetEnum>(currentEntry.Url.Substring("openstore://".Length), true);
		}
		catch (Exception)
		{
			Log.Error("DLC link uses incorrect value!");
			entitlementSetEnum = EntitlementSetEnum.None;
		}
		if (entitlementSetEnum != EntitlementSetEnum.None)
		{
			EntitlementManager.Instance.OpenStore(entitlementSetEnum, delegate(EntitlementSetEnum _)
			{
				Log.Out("DLC dialog complete!");
			});
		}
	}

	// Token: 0x06006999 RID: 27033 RVA: 0x002AE4DC File Offset: 0x002AC6DC
	[PublicizedFrom(EAccessModifier.Private)]
	public void cycle(int _increment, bool _sound)
	{
		object obj = this.newsLock;
		lock (obj)
		{
			int num = this.CurrentIndex;
			this.CurrentIndex += _increment;
			if (this.CurrentIndex != num && _sound && this.browseSound != null)
			{
				Manager.PlayXUiSound(this.browseSound, 1f);
			}
			this.autoCycle = false;
			if (this.selector != null)
			{
				this.selector.Value = (double)(this.CurrentIndex + 1);
			}
			this.IsDirty = true;
		}
	}

	// Token: 0x0600699A RID: 27034 RVA: 0x002AE584 File Offset: 0x002AC784
	[PublicizedFrom(EAccessModifier.Private)]
	public void resetIndex()
	{
		this.CurrentIndex = 0;
		if (this.selector != null)
		{
			this.selector.Value = (double)(this.autoCycle ? 0 : 1);
		}
	}

	// Token: 0x0600699B RID: 27035 RVA: 0x002AE5AD File Offset: 0x002AC7AD
	public override void OnOpen()
	{
		base.OnOpen();
		this.autoCycle = (this.autoCycleTimePerEntry > 0f);
		this.resetIndex();
		base.RefreshBindings(true);
	}

	// Token: 0x0600699C RID: 27036 RVA: 0x002AE5D8 File Offset: 0x002AC7D8
	public override bool ParseAttribute(string _name, string _value, XUiController _parent)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_name);
		if (num <= 1710475198U)
		{
			if (num != 1098871626U)
			{
				if (num != 1570288327U)
				{
					if (num == 1710475198U)
					{
						if (_name == "auto_cycle_time_per_entry")
						{
							this.autoCycleTimePerEntry = StringParsers.ParseFloat(_value, 0, -1, NumberStyles.Any);
							return true;
						}
					}
				}
				else if (_name == "browse_sound")
				{
					base.xui.LoadData<AudioClip>(_value, delegate(AudioClip _o)
					{
						this.browseSound = _o;
					});
					return true;
				}
			}
			else if (_name == "max_entries")
			{
				this.maxEntries = StringParsers.ParseSInt32(_value, 0, -1, NumberStyles.Integer);
				return true;
			}
		}
		else
		{
			if (num <= 2560548053U)
			{
				if (num != 1914607665U)
				{
					if (num != 2560548053U)
					{
						goto IL_26C;
					}
					if (!(_name == "additional_sources"))
					{
						goto IL_26C;
					}
				}
				else if (!(_name == "sources"))
				{
					goto IL_26C;
				}
				string[] array = _value.Split(',', StringSplitOptions.None);
				for (int i = 0; i < array.Length; i++)
				{
					string text = array[i].Trim();
					if (text.Length != 0)
					{
						this.newsProviders.Add(text);
						NewsManager.Instance.RegisterNewsSource(text);
					}
				}
				return true;
			}
			if (num != 3462041365U)
			{
				if (num == 3904898188U)
				{
					if (_name == "button_older")
					{
						this.buttonOlder = base.xui.playerUI.playerInput.GUIActions.GetPlayerActionByName(_value);
						if (this.buttonOlder == null)
						{
							Log.Warning(string.Concat(new string[]
							{
								"[XUi] Could not find GUI action '",
								_value,
								"' for news window (window group '",
								base.WindowGroup.ID,
								"', window '",
								base.ViewComponent.ID,
								"')"
							}));
						}
						return true;
					}
				}
			}
			else if (_name == "button_younger")
			{
				this.buttonYounger = base.xui.playerUI.playerInput.GUIActions.GetPlayerActionByName(_value);
				if (this.buttonYounger == null)
				{
					Log.Warning(string.Concat(new string[]
					{
						"[XUi] Could not find GUI action '",
						_value,
						"' for news window (window group '",
						base.WindowGroup.ID,
						"', window '",
						base.ViewComponent.ID,
						"')"
					}));
				}
				return true;
			}
		}
		IL_26C:
		return base.ParseAttribute(_name, _value, _parent);
	}

	// Token: 0x0600699D RID: 27037 RVA: 0x002AE85C File Offset: 0x002ACA5C
	[PublicizedFrom(EAccessModifier.Protected)]
	public override bool GetBindingValueInternal(ref string _value, string _bindingName)
	{
		NewsManager.NewsEntry newsEntry = this.CurrentEntry ?? NewsManager.EmptyEntry;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(_bindingName);
		if (num <= 2293204330U)
		{
			if (num <= 742476188U)
			{
				if (num != 232457833U)
				{
					if (num != 466561496U)
					{
						if (num == 742476188U)
						{
							if (_bindingName == "age")
							{
								_value = ValueDisplayFormatters.DateAge(newsEntry.Date);
								return true;
							}
						}
					}
					else if (_bindingName == "source")
					{
						_value = (newsEntry.CustomListName ?? "");
						return true;
					}
				}
				else if (_bindingName == "link")
				{
					_value = (newsEntry.Url ?? "");
					return true;
				}
			}
			else if (num <= 1280386353U)
			{
				if (num != 1208723273U)
				{
					if (num == 1280386353U)
					{
						if (_bindingName == "has_news_provider")
						{
							_value = this.hasProviders.ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "headline")
				{
					_value = newsEntry.Headline;
					return true;
				}
			}
			else if (num != 2179810835U)
			{
				if (num == 2293204330U)
				{
					if (_bindingName == "news_count")
					{
						_value = Mathf.Min(this.entries.Count, this.maxEntries).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "has_younger")
			{
				_value = (this.entries.Count > 0 && this.CurrentIndex > 0).ToString();
				return true;
			}
		}
		else if (num <= 3185987134U)
		{
			if (num <= 2874786163U)
			{
				if (num != 2851791573U)
				{
					if (num == 2874786163U)
					{
						if (_bindingName == "has_news")
						{
							_value = (this.entries.Count > 0).ToString();
							return true;
						}
					}
				}
				else if (_bindingName == "is_custom")
				{
					_value = newsEntry.IsCustom.ToString();
					return true;
				}
			}
			else if (num != 3165208740U)
			{
				if (num == 3185987134U)
				{
					if (_bindingName == "text")
					{
						_value = newsEntry.Text;
						return true;
					}
				}
			}
			else if (_bindingName == "has_link")
			{
				_value = (!string.IsNullOrEmpty(newsEntry.Url)).ToString();
				return true;
			}
		}
		else if (num <= 3646525923U)
		{
			if (num != 3564297305U)
			{
				if (num == 3646525923U)
				{
					if (_bindingName == "has_text")
					{
						_value = (!string.IsNullOrEmpty(newsEntry.Text)).ToString();
						return true;
					}
				}
			}
			else if (_bindingName == "date")
			{
				_value = newsEntry.Date.ToString("yyyy-MM-dd");
				return true;
			}
		}
		else if (num != 3805401922U)
		{
			if (num == 3847792289U)
			{
				if (_bindingName == "headline2")
				{
					_value = newsEntry.Headline2;
					return true;
				}
			}
		}
		else if (_bindingName == "has_older")
		{
			_value = (this.entries.Count > 0 && this.CurrentIndex < this.entries.Count - 1).ToString();
			return true;
		}
		return base.GetBindingValueInternal(ref _value, _bindingName);
	}

	// Token: 0x04004F8F RID: 20367
	[PublicizedFrom(EAccessModifier.Private)]
	public const string PlatformStoreLink = "openstore://";

	// Token: 0x04004F90 RID: 20368
	[PublicizedFrom(EAccessModifier.Private)]
	public AudioClip browseSound;

	// Token: 0x04004F91 RID: 20369
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiC_ComboBoxFloat selector;

	// Token: 0x04004F92 RID: 20370
	[PublicizedFrom(EAccessModifier.Private)]
	public XUiV_Texture[] bannerTextures;

	// Token: 0x04004F93 RID: 20371
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly object newsLock = new object();

	// Token: 0x04004F94 RID: 20372
	[PublicizedFrom(EAccessModifier.Private)]
	public int currentIndex;

	// Token: 0x04004F95 RID: 20373
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<NewsManager.NewsEntry> entries = new List<NewsManager.NewsEntry>();

	// Token: 0x04004F96 RID: 20374
	[PublicizedFrom(EAccessModifier.Private)]
	public bool hasProviders;

	// Token: 0x04004F97 RID: 20375
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly List<string> newsProviders = new List<string>();

	// Token: 0x04004F98 RID: 20376
	[PublicizedFrom(EAccessModifier.Private)]
	public int maxEntries = int.MaxValue;

	// Token: 0x04004F99 RID: 20377
	[PublicizedFrom(EAccessModifier.Private)]
	public float autoCycleTimePerEntry;

	// Token: 0x04004F9A RID: 20378
	[PublicizedFrom(EAccessModifier.Private)]
	public bool autoCycle;

	// Token: 0x04004F9B RID: 20379
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction buttonYounger;

	// Token: 0x04004F9C RID: 20380
	[PublicizedFrom(EAccessModifier.Private)]
	public PlayerAction buttonOlder;
}

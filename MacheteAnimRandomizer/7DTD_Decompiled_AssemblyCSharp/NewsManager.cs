using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;
using Platform;
using UnityEngine;

// Token: 0x02001017 RID: 4119
public class NewsManager
{
	// Token: 0x17000DA9 RID: 3497
	// (get) Token: 0x060082A4 RID: 33444 RVA: 0x0034CAB0 File Offset: 0x0034ACB0
	public static NewsManager Instance
	{
		get
		{
			NewsManager result;
			if ((result = NewsManager.instance) == null)
			{
				result = (NewsManager.instance = new NewsManager());
			}
			return result;
		}
	}

	// Token: 0x140000EF RID: 239
	// (add) Token: 0x060082A5 RID: 33445 RVA: 0x0034CAC8 File Offset: 0x0034ACC8
	// (remove) Token: 0x060082A6 RID: 33446 RVA: 0x0034CB00 File Offset: 0x0034AD00
	public event Action<NewsManager> Updated;

	// Token: 0x060082A7 RID: 33447 RVA: 0x0034CB38 File Offset: 0x0034AD38
	public void UpdateNews(bool _force = false)
	{
		foreach (KeyValuePair<string, NewsManager.NewsSource> keyValuePair in this.sources)
		{
			string text;
			NewsManager.NewsSource newsSource;
			keyValuePair.Deconstruct(out text, out newsSource);
			newsSource.RequestData(_force);
		}
	}

	// Token: 0x060082A8 RID: 33448 RVA: 0x0034CB98 File Offset: 0x0034AD98
	public void RegisterNewsSource(string _uri)
	{
		if (this.sources.ContainsKey(_uri))
		{
			return;
		}
		NewsManager.NewsSource newsSource = NewsManager.NewsSource.FromUri(this, _uri);
		this.sources[_uri] = newsSource;
		newsSource.RequestData(false);
	}

	// Token: 0x060082A9 RID: 33449 RVA: 0x0034CBD0 File Offset: 0x0034ADD0
	public void GetNewsData(List<string> _sources, List<NewsManager.NewsEntry> _target)
	{
		_target.Clear();
		foreach (string key in _sources)
		{
			NewsManager.NewsSource newsSource;
			if (this.sources.TryGetValue(key, out newsSource))
			{
				newsSource.GetData(_target);
			}
		}
		this.sortNewsByAge(_target);
	}

	// Token: 0x060082AA RID: 33450 RVA: 0x0034CC3C File Offset: 0x0034AE3C
	[PublicizedFrom(EAccessModifier.Private)]
	public void sortNewsByAge(List<NewsManager.NewsEntry> _list)
	{
		_list.Sort((NewsManager.NewsEntry _entryA, NewsManager.NewsEntry _entryB) => _entryB.Date.CompareTo(_entryA.Date));
	}

	// Token: 0x060082AB RID: 33451 RVA: 0x0034CC63 File Offset: 0x0034AE63
	[PublicizedFrom(EAccessModifier.Private)]
	public void notifyListeners()
	{
		Action<NewsManager> updated = this.Updated;
		if (updated == null)
		{
			return;
		}
		updated(this);
	}

	// Token: 0x040064DE RID: 25822
	[PublicizedFrom(EAccessModifier.Private)]
	public static NewsManager instance;

	// Token: 0x040064DF RID: 25823
	public static readonly NewsManager.NewsEntry EmptyEntry = new NewsManager.NewsEntry(null, null, null, "- No Entries -", null, "", null, DateTime.Now);

	// Token: 0x040064E0 RID: 25824
	[PublicizedFrom(EAccessModifier.Private)]
	public readonly Dictionary<string, NewsManager.NewsSource> sources = new CaseInsensitiveStringDictionary<NewsManager.NewsSource>();

	// Token: 0x02001018 RID: 4120
	public abstract class NewsSource
	{
		// Token: 0x17000DAA RID: 3498
		// (get) Token: 0x060082AE RID: 33454
		public abstract bool IsCustom { get; }

		// Token: 0x060082AF RID: 33455 RVA: 0x0034CCA9 File Offset: 0x0034AEA9
		[PublicizedFrom(EAccessModifier.Protected)]
		public NewsSource(NewsManager _owner, string _uri)
		{
			this.Owner = _owner;
			this.OrigUri = _uri;
		}

		// Token: 0x060082B0 RID: 33456 RVA: 0x0034CCCC File Offset: 0x0034AECC
		public void RequestData(bool _force)
		{
			if (this.isUpdating)
			{
				return;
			}
			DateTime now = DateTime.Now;
			NewsManager owner = this.Owner;
			lock (owner)
			{
				if (!_force && this.entries.Count > 0 && (now - this.lastUpdated).TotalMinutes < 1.0)
				{
					return;
				}
			}
			this.lastUpdated = now;
			this.isUpdating = true;
			ThreadManager.StartCoroutine(this.GetDataCo());
		}

		// Token: 0x060082B1 RID: 33457
		[PublicizedFrom(EAccessModifier.Protected)]
		public abstract IEnumerator GetDataCo();

		// Token: 0x060082B2 RID: 33458 RVA: 0x0034CD64 File Offset: 0x0034AF64
		public void GetData(List<NewsManager.NewsEntry> _target)
		{
			NewsManager owner = this.Owner;
			lock (owner)
			{
				_target.AddRange(this.entries);
			}
		}

		// Token: 0x060082B3 RID: 33459
		public abstract void RequestImage(string _imageRelPath, Action<Texture2D> _callback);

		// Token: 0x060082B4 RID: 33460 RVA: 0x0034CDAC File Offset: 0x0034AFAC
		[PublicizedFrom(EAccessModifier.Protected)]
		public void LoadXml(XmlFile _xml)
		{
			this.isUpdating = false;
			NewsManager owner = this.Owner;
			lock (owner)
			{
				this.entries.Clear();
			}
			XElement xelement = (_xml != null) ? _xml.XmlDoc.Root : null;
			if (xelement == null)
			{
				this.Owner.notifyListeners();
				return;
			}
			string text = xelement.GetAttribute("name").Trim();
			if (text == "")
			{
				text = null;
			}
			owner = this.Owner;
			lock (owner)
			{
				foreach (XElement element in xelement.Elements("entry"))
				{
					NewsManager.NewsEntry newsEntry = NewsManager.NewsEntry.FromXml(this, text, element);
					if (newsEntry != null)
					{
						this.entries.Add(newsEntry);
					}
				}
			}
			this.Owner.notifyListeners();
		}

		// Token: 0x060082B5 RID: 33461 RVA: 0x0034CED4 File Offset: 0x0034B0D4
		public static NewsManager.NewsSource FromUri(NewsManager _owner, string _uri)
		{
			if (_uri.StartsWith("rfs://"))
			{
				return new NewsManager.NewsSourceRfs(_owner, _uri);
			}
			return new NewsManager.NewsSourceWww(_owner, _uri);
		}

		// Token: 0x040064E2 RID: 25826
		[PublicizedFrom(EAccessModifier.Protected)]
		public const string RemoteFileStorageProtocol = "rfs://";

		// Token: 0x040064E3 RID: 25827
		public readonly NewsManager Owner;

		// Token: 0x040064E4 RID: 25828
		[PublicizedFrom(EAccessModifier.Protected)]
		public readonly string OrigUri;

		// Token: 0x040064E5 RID: 25829
		[PublicizedFrom(EAccessModifier.Protected)]
		public bool isUpdating;

		// Token: 0x040064E6 RID: 25830
		[PublicizedFrom(EAccessModifier.Protected)]
		public DateTime lastUpdated;

		// Token: 0x040064E7 RID: 25831
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly List<NewsManager.NewsEntry> entries = new List<NewsManager.NewsEntry>();
	}

	// Token: 0x02001019 RID: 4121
	[PublicizedFrom(EAccessModifier.Private)]
	public class NewsSourceRfs : NewsManager.NewsSource
	{
		// Token: 0x060082B6 RID: 33462 RVA: 0x0034CEF2 File Offset: 0x0034B0F2
		public NewsSourceRfs(NewsManager _owner, string _uri) : base(_owner, _uri)
		{
			this.rfsFilename = _uri.Substring("rfs://".Length);
		}

		// Token: 0x17000DAB RID: 3499
		// (get) Token: 0x060082B7 RID: 33463 RVA: 0x0000FB42 File Offset: 0x0000DD42
		public override bool IsCustom
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060082B8 RID: 33464 RVA: 0x0034CF1D File Offset: 0x0034B11D
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator GetDataCo()
		{
			IRemoteFileStorage storage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (storage == null)
			{
				this.isUpdating = false;
				yield break;
			}
			if (PlatformManager.NativePlatform.User.UserStatus != EUserStatus.LoggedIn)
			{
				storage.GetCachedFile(this.rfsFilename, new IRemoteFileStorage.FileDownloadCompleteCallback(this.<GetDataCo>g__fileDownloadedCallback|4_0));
				this.lastUpdated = DateTime.MinValue;
				yield break;
			}
			bool loggedSlow = false;
			float startTime = Time.time;
			while (!storage.IsReady)
			{
				if (storage.Unavailable)
				{
					Log.Warning("Remote Storage is unavailable");
					this.isUpdating = false;
					yield break;
				}
				yield return null;
				if (!loggedSlow && Time.time > startTime + 30f)
				{
					loggedSlow = true;
					Log.Warning("Waiting for news from remote storage exceeded 30s");
				}
			}
			storage.GetFile(this.rfsFilename, new IRemoteFileStorage.FileDownloadCompleteCallback(this.<GetDataCo>g__fileDownloadedCallback|4_0));
			yield break;
		}

		// Token: 0x060082B9 RID: 33465 RVA: 0x0034CF2C File Offset: 0x0034B12C
		public override void RequestImage(string _imageRelPath, Action<Texture2D> _callback)
		{
			Queue<ValueTuple<string, Action<Texture2D>>> obj = this.requestedImagesQueue;
			lock (obj)
			{
				this.requestedImagesQueue.Enqueue(new ValueTuple<string, Action<Texture2D>>(_imageRelPath, _callback));
			}
			this.startNextImageRequest();
		}

		// Token: 0x060082BA RID: 33466 RVA: 0x0034CF80 File Offset: 0x0034B180
		[PublicizedFrom(EAccessModifier.Private)]
		public void startNextImageRequest()
		{
			Queue<ValueTuple<string, Action<Texture2D>>> obj = this.requestedImagesQueue;
			lock (obj)
			{
				ValueTuple<string, Action<Texture2D>> valueTuple;
				if (this.runningImageRequests < 3 && this.requestedImagesQueue.TryDequeue(out valueTuple))
				{
					ThreadManager.StartCoroutine(this.requestFromRemoteStorage(valueTuple.Item1, valueTuple.Item2));
				}
			}
		}

		// Token: 0x060082BB RID: 33467 RVA: 0x0034CFEC File Offset: 0x0034B1EC
		[PublicizedFrom(EAccessModifier.Private)]
		public void imageRequestCompleted()
		{
			Queue<ValueTuple<string, Action<Texture2D>>> obj = this.requestedImagesQueue;
			lock (obj)
			{
				this.runningImageRequests--;
				this.startNextImageRequest();
			}
		}

		// Token: 0x060082BC RID: 33468 RVA: 0x0034D03C File Offset: 0x0034B23C
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator requestFromRemoteStorage(string _imageRelPath, Action<Texture2D> _callback)
		{
			NewsManager.NewsSourceRfs.<>c__DisplayClass11_0 CS$<>8__locals1 = new NewsManager.NewsSourceRfs.<>c__DisplayClass11_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1._imageRelPath = _imageRelPath;
			CS$<>8__locals1._callback = _callback;
			IRemoteFileStorage remoteFileStorage = PlatformManager.MultiPlatform.RemoteFileStorage;
			if (remoteFileStorage == null)
			{
				yield break;
			}
			Queue<ValueTuple<string, Action<Texture2D>>> obj = this.requestedImagesQueue;
			lock (obj)
			{
				this.runningImageRequests++;
			}
			if (remoteFileStorage.Unavailable)
			{
				remoteFileStorage.GetCachedFile(CS$<>8__locals1._imageRelPath, new IRemoteFileStorage.FileDownloadCompleteCallback(CS$<>8__locals1.<requestFromRemoteStorage>g__imageDownloadedCallback|0));
			}
			else
			{
				remoteFileStorage.GetFile(CS$<>8__locals1._imageRelPath, new IRemoteFileStorage.FileDownloadCompleteCallback(CS$<>8__locals1.<requestFromRemoteStorage>g__imageDownloadedCallback|0));
			}
			yield break;
		}

		// Token: 0x060082BD RID: 33469 RVA: 0x0034D05C File Offset: 0x0034B25C
		[CompilerGenerated]
		[PublicizedFrom(EAccessModifier.Private)]
		public void <GetDataCo>g__fileDownloadedCallback|4_0(IRemoteFileStorage.EFileDownloadResult _result, string _errorDetails, byte[] _data)
		{
			if (_result != IRemoteFileStorage.EFileDownloadResult.Ok)
			{
				Log.Warning(string.Concat(new string[]
				{
					"Retrieving remote news file '",
					this.rfsFilename,
					"' failed: ",
					_result.ToStringCached<IRemoteFileStorage.EFileDownloadResult>(),
					" (",
					_errorDetails,
					")"
				}));
				base.LoadXml(null);
				return;
			}
			XmlFile xml = null;
			if (_data != null && _data.Length != 0)
			{
				try
				{
					xml = new XmlFile(_data, true);
				}
				catch (Exception e)
				{
					Log.Error("Failed loading news XML:");
					Log.Exception(e);
					return;
				}
			}
			base.LoadXml(xml);
		}

		// Token: 0x040064E8 RID: 25832
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string rfsFilename;

		// Token: 0x040064E9 RID: 25833
		[PublicizedFrom(EAccessModifier.Private)]
		public const int MaxParallelImageRequests = 3;

		// Token: 0x040064EA RID: 25834
		[TupleElementNames(new string[]
		{
			"imageRelPath",
			"callback"
		})]
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly Queue<ValueTuple<string, Action<Texture2D>>> requestedImagesQueue = new Queue<ValueTuple<string, Action<Texture2D>>>();

		// Token: 0x040064EB RID: 25835
		[PublicizedFrom(EAccessModifier.Private)]
		public int runningImageRequests;
	}

	// Token: 0x0200101D RID: 4125
	[PublicizedFrom(EAccessModifier.Private)]
	public class NewsSourceWww : NewsManager.NewsSource
	{
		// Token: 0x17000DB0 RID: 3504
		// (get) Token: 0x060082CC RID: 33484 RVA: 0x000197A5 File Offset: 0x000179A5
		public override bool IsCustom
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060082CD RID: 33485 RVA: 0x0034D3B0 File Offset: 0x0034B5B0
		public NewsSourceWww(NewsManager _owner, string _uri) : base(_owner, _uri)
		{
			string text = _uri;
			if (!text.StartsWith("http", StringComparison.Ordinal))
			{
				string text2 = ModManager.PatchModPathString(text);
				if (text2 == null)
				{
					throw new ArgumentException("WWW news source '" + _uri + "' can not be retrieved: Neither is a 'http(s)://' URI nor a '@modfolder:' reference.");
				}
				text = "file://" + text2;
			}
			text = text.Replace("#", "%23").Replace("+", "%2B");
			this.patchedUri = text;
			int num = text.LastIndexOf('/');
			if (num < 0)
			{
				throw new ArgumentException("WWW news source '" + _uri + "' does not have a valid path");
			}
			this.baseUri = text.Substring(0, num + 1);
		}

		// Token: 0x060082CE RID: 33486 RVA: 0x0034D45B File Offset: 0x0034B65B
		[PublicizedFrom(EAccessModifier.Protected)]
		public override IEnumerator GetDataCo()
		{
			NewsManager.NewsSourceWww.<GetDataCo>d__5 <GetDataCo>d__ = new NewsManager.NewsSourceWww.<GetDataCo>d__5(0);
			<GetDataCo>d__.<>4__this = this;
			return <GetDataCo>d__;
		}

		// Token: 0x060082CF RID: 33487 RVA: 0x0034D46A File Offset: 0x0034B66A
		public override void RequestImage(string _imageRelPath, Action<Texture2D> _callback)
		{
			ThreadManager.StartCoroutine(this.requestFromUri(_imageRelPath, _callback));
		}

		// Token: 0x060082D0 RID: 33488 RVA: 0x0034D47A File Offset: 0x0034B67A
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator requestFromUri(string _imageRelPath, Action<Texture2D> _callback)
		{
			NewsManager.NewsSourceWww.<requestFromUri>d__7 <requestFromUri>d__ = new NewsManager.NewsSourceWww.<requestFromUri>d__7(0);
			<requestFromUri>d__.<>4__this = this;
			<requestFromUri>d__._imageRelPath = _imageRelPath;
			<requestFromUri>d__._callback = _callback;
			return <requestFromUri>d__;
		}

		// Token: 0x040064FA RID: 25850
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string patchedUri;

		// Token: 0x040064FB RID: 25851
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string baseUri;
	}

	// Token: 0x02001020 RID: 4128
	public class NewsEntry : IEquatable<NewsManager.NewsEntry>
	{
		// Token: 0x17000DB5 RID: 3509
		// (get) Token: 0x060082DF RID: 33503 RVA: 0x0034D788 File Offset: 0x0034B988
		public bool IsCustom
		{
			get
			{
				return this.owner == null || this.owner.IsCustom;
			}
		}

		// Token: 0x060082E0 RID: 33504 RVA: 0x0034D7A0 File Offset: 0x0034B9A0
		public NewsEntry(NewsManager.NewsSource _owner, string _customListName, string _imageRelPath, string _headline, string _headline2, string _text, string _url, DateTime _date)
		{
			this.owner = _owner;
			this.CustomListName = _customListName;
			this.imageRelPath = _imageRelPath;
			this.Headline = _headline;
			this.Headline2 = _headline2;
			this.Text = _text;
			this.Url = _url;
			this.Date = _date;
		}

		// Token: 0x17000DB6 RID: 3510
		// (get) Token: 0x060082E1 RID: 33505 RVA: 0x0034D7F0 File Offset: 0x0034B9F0
		public bool HasImage
		{
			get
			{
				return !string.IsNullOrEmpty(this.imageRelPath);
			}
		}

		// Token: 0x17000DB7 RID: 3511
		// (get) Token: 0x060082E2 RID: 33506 RVA: 0x0034D800 File Offset: 0x0034BA00
		public bool ImageLoaded
		{
			get
			{
				return this.image != null;
			}
		}

		// Token: 0x17000DB8 RID: 3512
		// (get) Token: 0x060082E3 RID: 33507 RVA: 0x0034D80E File Offset: 0x0034BA0E
		public Texture2D Image
		{
			get
			{
				return this.image;
			}
		}

		// Token: 0x060082E4 RID: 33508 RVA: 0x0034D816 File Offset: 0x0034BA16
		public void RequestImage()
		{
			if (!this.HasImage)
			{
				return;
			}
			if (this.requestedImage)
			{
				return;
			}
			this.requestedImage = true;
			this.owner.RequestImage(this.imageRelPath, new Action<Texture2D>(this.setImage));
		}

		// Token: 0x060082E5 RID: 33509 RVA: 0x0034D850 File Offset: 0x0034BA50
		[PublicizedFrom(EAccessModifier.Private)]
		public void setImage(Texture2D _image)
		{
			this.image = _image;
			this.image.name = "NewsImage_" + this.imageRelPath;
			this.image.Compress(true);
			this.image.Apply(false, true);
			this.owner.Owner.notifyListeners();
		}

		// Token: 0x060082E6 RID: 33510 RVA: 0x0034D8A8 File Offset: 0x0034BAA8
		public bool Equals(NewsManager.NewsEntry _other)
		{
			return _other != null && (this == _other || (this.CustomListName == _other.CustomListName && this.imageRelPath == _other.imageRelPath && this.Headline == _other.Headline && this.Headline2 == _other.Headline2 && this.Text == _other.Text && this.Url == _other.Url && this.Date.Equals(_other.Date)));
		}

		// Token: 0x060082E7 RID: 33511 RVA: 0x0034D948 File Offset: 0x0034BB48
		public override bool Equals(object _obj)
		{
			return _obj != null && (this == _obj || (!(_obj.GetType() != base.GetType()) && this.Equals((NewsManager.NewsEntry)_obj)));
		}

		// Token: 0x060082E8 RID: 33512 RVA: 0x0034D978 File Offset: 0x0034BB78
		public override int GetHashCode()
		{
			return (((((((this.CustomListName != null) ? this.CustomListName.GetHashCode() : 0) * 397 ^ ((this.imageRelPath != null) ? this.imageRelPath.GetHashCode() : 0)) * 397 ^ ((this.Headline != null) ? this.Headline.GetHashCode() : 0)) * 397 ^ ((this.Headline2 != null) ? this.Headline2.GetHashCode() : 0)) * 397 ^ ((this.Text != null) ? this.Text.GetHashCode() : 0)) * 397 ^ ((this.Url != null) ? this.Url.GetHashCode() : 0)) * 397 ^ this.Date.GetHashCode();
		}

		// Token: 0x060082E9 RID: 33513 RVA: 0x0034DA44 File Offset: 0x0034BC44
		public static NewsManager.NewsEntry FromXml(NewsManager.NewsSource _owner, string _customListName, XElement _element)
		{
			string text = null;
			string headline = "";
			string headline2 = "";
			string text2 = "";
			string url = null;
			DateTime minValue = DateTime.MinValue;
			DateTime maxValue = DateTime.MaxValue;
			bool flag = true;
			foreach (XElement xelement in _element.Elements())
			{
				string localName = xelement.Name.LocalName;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(localName);
				if (num <= 1990630727U)
				{
					if (num <= 561879464U)
					{
						if (num != 232457833U)
						{
							if (num == 561879464U)
							{
								if (localName == "devicetypes")
								{
									bool flag2 = false;
									string[] array = xelement.Value.Split(',', StringSplitOptions.None);
									for (int i = 0; i < array.Length; i++)
									{
										string text3 = array[i].Trim();
										if (!string.IsNullOrEmpty(text3))
										{
											EDeviceType edeviceType;
											if (!EnumUtils.TryParse<EDeviceType>(text3, out edeviceType, true))
											{
												Log.Warning(string.Format("News XML has an entry with an invalid 'devicetypes' element '{0}', devicetype '{1}' unknown at line {2}", xelement.Value, text3, ((IXmlLineInfo)_element).LineNumber));
												return null;
											}
											if (edeviceType == PlatformManager.DeviceType)
											{
												flag2 = true;
												break;
											}
										}
									}
									if (!flag2)
									{
										return null;
									}
									continue;
								}
							}
						}
						else if (localName == "link")
						{
							url = xelement.Value.Trim();
							continue;
						}
					}
					else if (num != 589056993U)
					{
						if (num != 1406002643U)
						{
							if (num == 1990630727U)
							{
								if (localName == "showbefore")
								{
									if (!bool.TryParse(xelement.Value, out flag))
									{
										Log.Warning(string.Format("News XML has an entry with an invalid 'showbefore' element '{0}' at line {1}", xelement.Value, ((IXmlLineInfo)_element).LineNumber));
										return null;
									}
									continue;
								}
							}
						}
						else if (localName == "platforms")
						{
							bool flag3 = false;
							string[] array = xelement.Value.Split(',', StringSplitOptions.None);
							for (int i = 0; i < array.Length; i++)
							{
								string text4 = array[i].Trim();
								if (!string.IsNullOrEmpty(text4))
								{
									EPlatformIdentifier eplatformIdentifier;
									if (!EnumUtils.TryParse<EPlatformIdentifier>(text4, out eplatformIdentifier, true))
									{
										Log.Warning(string.Format("News XML has an entry with an invalid 'platforms' element '{0}', platform '{1}' unknown at line {2}", xelement.Value, text4, ((IXmlLineInfo)_element).LineNumber));
										return null;
									}
									if (eplatformIdentifier != PlatformManager.NativePlatform.PlatformIdentifier)
									{
										EPlatformIdentifier eplatformIdentifier2 = eplatformIdentifier;
										IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
										EPlatformIdentifier? eplatformIdentifier3 = (crossplatformPlatform != null) ? new EPlatformIdentifier?(crossplatformPlatform.PlatformIdentifier) : null;
										if (!(eplatformIdentifier2 == eplatformIdentifier3.GetValueOrDefault() & eplatformIdentifier3 != null))
										{
											goto IL_3A0;
										}
									}
									flag3 = true;
									break;
								}
								IL_3A0:;
							}
							if (!flag3)
							{
								return null;
							}
							continue;
						}
					}
					else if (localName == "title2")
					{
						headline2 = xelement.Value;
						continue;
					}
				}
				else if (num <= 2556802313U)
				{
					if (num != 2428985098U)
					{
						if (num == 2556802313U)
						{
							if (localName == "title")
							{
								headline = xelement.Value;
								continue;
							}
						}
					}
					else if (localName == "imagerelpath")
					{
						text = xelement.Value;
						continue;
					}
				}
				else if (num != 3185987134U)
				{
					if (num != 3564297305U)
					{
						if (num == 3808839532U)
						{
							if (localName == "showuntil")
							{
								if (!DateTime.TryParseExact(xelement.Value, "u", null, DateTimeStyles.AssumeUniversal, out maxValue))
								{
									Log.Warning(string.Format("News XML has an entry with an invalid 'showuntil' element '{0}' at line {1}", xelement.Value, ((IXmlLineInfo)_element).LineNumber));
									return null;
								}
								continue;
							}
						}
					}
					else if (localName == "date")
					{
						if (!DateTime.TryParseExact(xelement.Value, "u", null, DateTimeStyles.AssumeUniversal, out minValue))
						{
							Log.Warning(string.Format("News XML has an entry with an invalid 'date' element '{0}' at line {1}", xelement.Value, ((IXmlLineInfo)_element).LineNumber));
							return null;
						}
						continue;
					}
				}
				else if (localName == "text")
				{
					text2 = xelement.Value;
					continue;
				}
				Log.Warning(string.Format("News XML has an entry with an unknown element '{0}' at line {1}", xelement.Name.LocalName, ((IXmlLineInfo)_element).LineNumber));
			}
			if (minValue == DateTime.MinValue)
			{
				Log.Warning(string.Format("News XML has an entry without a date element at line {0}", ((IXmlLineInfo)_element).LineNumber));
				return null;
			}
			DateTime now = DateTime.Now;
			if (!flag && minValue > now)
			{
				return null;
			}
			if (maxValue < now)
			{
				return null;
			}
			return new NewsManager.NewsEntry(_owner, _customListName, text, headline, headline2, text2, url, minValue);
		}

		// Token: 0x04006507 RID: 25863
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly NewsManager.NewsSource owner;

		// Token: 0x04006508 RID: 25864
		public readonly string CustomListName;

		// Token: 0x04006509 RID: 25865
		[PublicizedFrom(EAccessModifier.Private)]
		public readonly string imageRelPath;

		// Token: 0x0400650A RID: 25866
		public readonly string Headline;

		// Token: 0x0400650B RID: 25867
		public readonly string Headline2;

		// Token: 0x0400650C RID: 25868
		public readonly string Text;

		// Token: 0x0400650D RID: 25869
		public readonly string Url;

		// Token: 0x0400650E RID: 25870
		public readonly DateTime Date;

		// Token: 0x0400650F RID: 25871
		[PublicizedFrom(EAccessModifier.Private)]
		public bool requestedImage;

		// Token: 0x04006510 RID: 25872
		[PublicizedFrom(EAccessModifier.Private)]
		public Texture2D image;
	}
}

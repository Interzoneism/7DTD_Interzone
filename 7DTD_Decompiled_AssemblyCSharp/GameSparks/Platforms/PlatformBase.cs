using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using GameSparks.Core;
using UnityEngine;

namespace GameSparks.Platforms
{
	// Token: 0x02001995 RID: 6549
	public abstract class PlatformBase : MonoBehaviour, IGSPlatform
	{
		// Token: 0x0600C09E RID: 49310 RVA: 0x0048F418 File Offset: 0x0048D618
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Start()
		{
			this.DeviceName = SystemInfo.deviceName.ToString();
			this.DeviceType = SystemInfo.deviceType.ToString();
			if (Application.platform == RuntimePlatform.PS4 || Application.platform == RuntimePlatform.XboxOne || "n/a" == SystemInfo.deviceUniqueIdentifier)
			{
				if ("n/a" == SystemInfo.deviceUniqueIdentifier)
				{
					this.DeviceId = Guid.NewGuid().ToString();
				}
				else
				{
					this.DeviceId = SystemInfo.deviceUniqueIdentifier.ToString();
				}
			}
			else
			{
				this.DeviceId = SystemInfo.deviceUniqueIdentifier.ToString();
			}
			char[] separator = new char[]
			{
				' ',
				',',
				'.',
				':',
				'-',
				'_',
				'(',
				')'
			};
			int processorCount = SystemInfo.processorCount;
			string text = "Unknown";
			string value = SystemInfo.deviceModel;
			string value2 = SystemInfo.systemMemorySize.ToString() + " MB";
			string text2 = SystemInfo.operatingSystem;
			string value3 = SystemInfo.operatingSystem;
			string text3 = SystemInfo.processorType;
			string value4 = Screen.width.ToString() + "x" + Screen.height.ToString();
			string version = GS.Version;
			string sdk = this.SDK;
			string unityVersion = Application.unityVersion;
			string deviceOS = this.DeviceOS;
			uint num = <PrivateImplementationDetails>.ComputeStringHash(deviceOS);
			string[] array;
			if (num <= 2697922028U)
			{
				if (num <= 920978609U)
				{
					if (num != 63313862U)
					{
						if (num != 650872197U)
						{
							if (num != 920978609U)
							{
								goto IL_87E;
							}
							if (!(deviceOS == "SWITCH"))
							{
								goto IL_87E;
							}
							text = "Nintendo";
							value = "Switch";
							value3 = "Unknown";
							goto IL_87E;
						}
						else
						{
							if (!(deviceOS == "XBOXSERIES"))
							{
								goto IL_87E;
							}
							text = "Microsoft";
							value = "Xbox Series";
							value2 = (SystemInfo.systemMemorySize / 1000).ToString() + " MB";
							value3 = "Unknown";
							text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
							RegexOptions options = RegexOptions.None;
							text3 = new Regex("[ ]{2,}", options).Replace(text3, " ");
							goto IL_87E;
						}
					}
					else if (!(deviceOS == "IOS"))
					{
						goto IL_87E;
					}
				}
				else if (num <= 2062687802U)
				{
					if (num != 1874269580U)
					{
						if (num != 2062687802U)
						{
							goto IL_87E;
						}
						if (!(deviceOS == "WSA"))
						{
							goto IL_87E;
						}
						goto IL_439;
					}
					else
					{
						if (!(deviceOS == "ANDROID"))
						{
							goto IL_87E;
						}
						array = SystemInfo.deviceModel.Split(separator);
						text = array[0];
						value = SystemInfo.deviceModel.Replace(text, "").Substring(1);
						array = SystemInfo.operatingSystem.Split(separator);
						text2 = array[0] + " " + array[1];
						value3 = array[7];
						text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
						goto IL_87E;
					}
				}
				else if (num != 2077565087U)
				{
					if (num != 2697922028U)
					{
						goto IL_87E;
					}
					if (!(deviceOS == "XBOXONE"))
					{
						goto IL_87E;
					}
					goto IL_439;
				}
				else
				{
					if (!(deviceOS == "TIZEN"))
					{
						goto IL_87E;
					}
					text = "Tizen";
					goto IL_87E;
				}
			}
			else if (num <= 3522446090U)
			{
				if (num <= 3313477467U)
				{
					if (num != 3221571746U)
					{
						if (num != 3313477467U)
						{
							goto IL_87E;
						}
						if (!(deviceOS == "WIIU"))
						{
							goto IL_87E;
						}
						text = "Nintendo";
						value = "WiiU";
						goto IL_87E;
					}
					else if (!(deviceOS == "MACOS"))
					{
						goto IL_87E;
					}
				}
				else if (num != 3466466665U)
				{
					if (num != 3522446090U)
					{
						goto IL_87E;
					}
					if (!(deviceOS == "WEBGL"))
					{
						goto IL_87E;
					}
					array = SystemInfo.deviceModel.Split(separator);
					value = array[0];
					array = SystemInfo.operatingSystem.Split(separator);
					text2 = array[0];
					if (text2.Equals("Mac"))
					{
						text2 = string.Concat(new string[]
						{
							text2,
							" ",
							array[1],
							" ",
							array[2]
						});
						value3 = string.Concat(new string[]
						{
							array[3],
							".",
							array[4],
							".",
							array[5]
						});
						goto IL_87E;
					}
					value3 = array[1];
					goto IL_87E;
				}
				else if (!(deviceOS == "TVOS"))
				{
					goto IL_87E;
				}
			}
			else if (num <= 4197334560U)
			{
				if (num != 3805831818U)
				{
					if (num != 4197334560U)
					{
						goto IL_87E;
					}
					if (!(deviceOS == "PS4"))
					{
						goto IL_87E;
					}
					text = "Sony";
					value = "PS4";
					value2 = (SystemInfo.systemMemorySize / 1000000).ToString() + " MB";
					array = SystemInfo.operatingSystem.Split(separator);
					text2 = array[0];
					value3 = string.Concat(new string[]
					{
						array[1],
						".",
						array[2],
						".",
						array[3]
					});
					text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
					goto IL_87E;
				}
				else
				{
					if (!(deviceOS == "WINDOWS"))
					{
						goto IL_87E;
					}
					goto IL_439;
				}
			}
			else if (num != 4214112179U)
			{
				if (num != 4225300267U)
				{
					goto IL_87E;
				}
				if (!(deviceOS == "GC_XBOXONE"))
				{
					goto IL_87E;
				}
				text = "Microsoft";
				value = "Xbox One";
				value2 = (SystemInfo.systemMemorySize / 1000).ToString() + " MB";
				value3 = "Unknown";
				text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
				RegexOptions options2 = RegexOptions.None;
				text3 = new Regex("[ ]{2,}", options2).Replace(text3, " ");
				goto IL_87E;
			}
			else
			{
				if (!(deviceOS == "PS5"))
				{
					goto IL_87E;
				}
				text = "Sony";
				value = "PS4";
				value2 = (SystemInfo.systemMemorySize / 1000000).ToString() + " MB";
				array = SystemInfo.operatingSystem.Split(separator);
				text2 = array[0];
				value3 = string.Concat(new string[]
				{
					array[1],
					".",
					array[2],
					".",
					array[3]
				});
				text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
				goto IL_87E;
			}
			text = "Apple";
			array = SystemInfo.operatingSystem.Split(separator);
			if (this.DeviceOS.Equals("MACOS"))
			{
				text2 = string.Concat(new string[]
				{
					array[0],
					" ",
					array[1],
					" ",
					array[2]
				});
				value3 = string.Concat(new string[]
				{
					array[3],
					".",
					array[4],
					".",
					array[5]
				});
				goto IL_87E;
			}
			text2 = array[0];
			value3 = array[1] + "." + array[2];
			goto IL_87E;
			IL_439:
			text = "Microsoft";
			if (this.DeviceOS.Equals("XBOXONE"))
			{
				value = "Xbox One";
				value2 = (SystemInfo.systemMemorySize / 1000).ToString() + " MB";
				value3 = "Unknown";
			}
			else
			{
				value = "PC";
				array = SystemInfo.operatingSystem.Split(separator, StringSplitOptions.RemoveEmptyEntries);
				text2 = array[0] + " " + array[1];
				value3 = string.Concat(new string[]
				{
					array[2],
					".",
					array[3],
					".",
					array[4]
				});
			}
			text3 = text3 + " " + SystemInfo.processorFrequency.ToString() + "MHz";
			RegexOptions options3 = RegexOptions.None;
			text3 = new Regex("[ ]{2,}", options3).Replace(text3, " ");
			IL_87E:
			this.DeviceStats = new GSData(new Dictionary<string, object>
			{
				{
					"manufacturer",
					text
				},
				{
					"model",
					value
				},
				{
					"memory",
					value2
				},
				{
					"os.name",
					text2
				},
				{
					"os.version",
					value3
				},
				{
					"cpu.cores",
					processorCount.ToString()
				},
				{
					"cpu.vendor",
					text3
				},
				{
					"resolution",
					value4
				},
				{
					"gssdk",
					version
				},
				{
					"engine",
					sdk
				},
				{
					"engine.version",
					unityVersion
				}
			});
			this.Platform = Application.platform.ToString();
			GameSparksSettings.SetInstance(base.GetComponent<GameSparksUnity>().settings);
			this.ExtraDebug = GameSparksSettings.DebugBuild;
			this.PersistentDataPath = Application.persistentDataPath;
			GS.Initialise(this);
			UnityEngine.Object.DontDestroyOnLoad(this);
		}

		// Token: 0x0600C09F RID: 49311 RVA: 0x0048FDA0 File Offset: 0x0048DFA0
		public void ExecuteOnMainThread(Action action)
		{
			List<Action> actions = this._actions;
			lock (actions)
			{
				this._actions.Add(action);
			}
		}

		// Token: 0x0600C0A0 RID: 49312 RVA: 0x0048FDE8 File Offset: 0x0048DFE8
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void Update()
		{
			List<Action> actions = this._actions;
			lock (actions)
			{
				if (this._actions.Count > 0)
				{
					this._currentActions.AddRange(this._actions);
					this._actions.Clear();
				}
			}
			int count = this._currentActions.Count;
			if (count > 0)
			{
				for (int i = 0; i < count; i++)
				{
					Action action = this._currentActions[i];
					if (action != null)
					{
						try
						{
							action();
						}
						catch (Exception ex)
						{
							if (this.ExceptionReporter != null)
							{
								this.ExceptionReporter(ex);
							}
							else
							{
								Debug.Log(ex);
							}
						}
					}
				}
				this._currentActions.Clear();
			}
		}

		// Token: 0x0600C0A1 RID: 49313 RVA: 0x0048FEBC File Offset: 0x0048E0BC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnApplicationPause(bool paused)
		{
			if (!paused)
			{
				try
				{
					GS.Reconnect();
				}
				catch (Exception obj)
				{
					if (this.ExceptionReporter != null)
					{
						this.ExceptionReporter(obj);
					}
				}
			}
		}

		// Token: 0x0600C0A2 RID: 49314 RVA: 0x0048FEFC File Offset: 0x0048E0FC
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnApplicationQuit()
		{
			GS.ShutDown();
			base.StartCoroutine("DelayedQuit");
			if (!this._allowQuitting)
			{
				Application.CancelQuit();
			}
		}

		// Token: 0x0600C0A3 RID: 49315 RVA: 0x0048FF1C File Offset: 0x0048E11C
		[PublicizedFrom(EAccessModifier.Private)]
		public IEnumerator DelayedQuit()
		{
			yield return new WaitForSeconds(1f);
			while (GS.Available)
			{
				yield return new WaitForSeconds(0.1f);
			}
			this._allowQuitting = true;
			Application.Quit();
			yield break;
		}

		// Token: 0x1700161B RID: 5659
		// (get) Token: 0x0600C0A4 RID: 49316 RVA: 0x0048FF2C File Offset: 0x0048E12C
		public string DeviceOS
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.PS4)
				{
					switch (platform)
					{
					case RuntimePlatform.OSXEditor:
					case RuntimePlatform.OSXPlayer:
						return "MACOS";
					case RuntimePlatform.WindowsPlayer:
					case RuntimePlatform.WindowsEditor:
						return "WINDOWS";
					case RuntimePlatform.OSXWebPlayer:
					case RuntimePlatform.OSXDashboardPlayer:
					case RuntimePlatform.WindowsWebPlayer:
					case (RuntimePlatform)6:
					case RuntimePlatform.PS3:
					case RuntimePlatform.XBOX360:
					case RuntimePlatform.NaCl:
					case (RuntimePlatform)14:
					case RuntimePlatform.FlashPlayer:
					case RuntimePlatform.LinuxEditor:
						break;
					case RuntimePlatform.IPhonePlayer:
						return "IOS";
					case RuntimePlatform.Android:
						return "ANDROID";
					case RuntimePlatform.LinuxPlayer:
						return "LINUX";
					case RuntimePlatform.WebGLPlayer:
						return "WEBGL";
					case RuntimePlatform.MetroPlayerX86:
					case RuntimePlatform.MetroPlayerX64:
					case RuntimePlatform.MetroPlayerARM:
						return "WSA";
					default:
						if (platform == RuntimePlatform.PS4)
						{
							return "PS4";
						}
						break;
					}
				}
				else
				{
					if (platform == RuntimePlatform.XboxOne)
					{
						return "XBOXONE";
					}
					if (platform == RuntimePlatform.tvOS)
					{
						return "TVOS";
					}
					switch (platform)
					{
					case RuntimePlatform.GameCoreXboxSeries:
						return "XBOXSERIES";
					case RuntimePlatform.GameCoreXboxOne:
						return "GC_XBOXONE";
					case RuntimePlatform.PS5:
						return "PS5";
					}
				}
				return "UNKNOWN";
			}
		}

		// Token: 0x1700161C RID: 5660
		// (get) Token: 0x0600C0A5 RID: 49317 RVA: 0x00490019 File Offset: 0x0048E219
		// (set) Token: 0x0600C0A6 RID: 49318 RVA: 0x00490021 File Offset: 0x0048E221
		public string DeviceName { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700161D RID: 5661
		// (get) Token: 0x0600C0A7 RID: 49319 RVA: 0x0049002A File Offset: 0x0048E22A
		// (set) Token: 0x0600C0A8 RID: 49320 RVA: 0x00490032 File Offset: 0x0048E232
		public string DeviceType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700161E RID: 5662
		// (get) Token: 0x0600C0A9 RID: 49321 RVA: 0x0049003B File Offset: 0x0048E23B
		// (set) Token: 0x0600C0AA RID: 49322 RVA: 0x00490043 File Offset: 0x0048E243
		public GSData DeviceStats { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x1700161F RID: 5663
		// (get) Token: 0x0600C0AB RID: 49323 RVA: 0x0049004C File Offset: 0x0048E24C
		// (set) Token: 0x0600C0AC RID: 49324 RVA: 0x00490054 File Offset: 0x0048E254
		public virtual string DeviceId { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001620 RID: 5664
		// (get) Token: 0x0600C0AD RID: 49325 RVA: 0x0049005D File Offset: 0x0048E25D
		// (set) Token: 0x0600C0AE RID: 49326 RVA: 0x00490065 File Offset: 0x0048E265
		public string Platform { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001621 RID: 5665
		// (get) Token: 0x0600C0AF RID: 49327 RVA: 0x0049006E File Offset: 0x0048E26E
		// (set) Token: 0x0600C0B0 RID: 49328 RVA: 0x00490076 File Offset: 0x0048E276
		public bool ExtraDebug { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x17001622 RID: 5666
		// (get) Token: 0x0600C0B1 RID: 49329 RVA: 0x0049007F File Offset: 0x0048E27F
		public string ApiKey
		{
			get
			{
				return GameSparksSettings.ApiKey;
			}
		}

		// Token: 0x17001623 RID: 5667
		// (get) Token: 0x0600C0B2 RID: 49330 RVA: 0x00490086 File Offset: 0x0048E286
		public string ApiSecret
		{
			get
			{
				return GameSparksSettings.ApiSecret;
			}
		}

		// Token: 0x17001624 RID: 5668
		// (get) Token: 0x0600C0B3 RID: 49331 RVA: 0x0049008D File Offset: 0x0048E28D
		public string ApiCredential
		{
			get
			{
				return GameSparksSettings.Credential;
			}
		}

		// Token: 0x17001625 RID: 5669
		// (get) Token: 0x0600C0B4 RID: 49332 RVA: 0x00490094 File Offset: 0x0048E294
		public string ApiStage
		{
			get
			{
				if (!GameSparksSettings.PreviewBuild)
				{
					return "live";
				}
				return "preview";
			}
		}

		// Token: 0x17001626 RID: 5670
		// (get) Token: 0x0600C0B5 RID: 49333 RVA: 0x00019766 File Offset: 0x00017966
		public string ApiDomain
		{
			get
			{
				return null;
			}
		}

		// Token: 0x17001627 RID: 5671
		// (get) Token: 0x0600C0B6 RID: 49334 RVA: 0x004900A8 File Offset: 0x0048E2A8
		// (set) Token: 0x0600C0B7 RID: 49335 RVA: 0x004900B0 File Offset: 0x0048E2B0
		public string PersistentDataPath { get; [PublicizedFrom(EAccessModifier.Private)] set; }

		// Token: 0x0600C0B8 RID: 49336 RVA: 0x004900BC File Offset: 0x0048E2BC
		public void DebugMsg(string message)
		{
			if (GameSparksSettings.DebugBuild)
			{
				if (message.Length < 1500)
				{
					Log.Out("GS: " + message);
					return;
				}
				Log.Out("GS: " + message.Substring(0, 1500) + "...");
			}
		}

		// Token: 0x17001628 RID: 5672
		// (get) Token: 0x0600C0B9 RID: 49337 RVA: 0x0049010E File Offset: 0x0048E30E
		public string SDK
		{
			get
			{
				return "Unity";
			}
		}

		// Token: 0x17001629 RID: 5673
		// (get) Token: 0x0600C0BA RID: 49338 RVA: 0x00490115 File Offset: 0x0048E315
		// (set) Token: 0x0600C0BB RID: 49339 RVA: 0x0049011D File Offset: 0x0048E31D
		public string AuthToken
		{
			get
			{
				return this.m_authToken;
			}
			set
			{
				this.m_authToken = value;
			}
		}

		// Token: 0x1700162A RID: 5674
		// (get) Token: 0x0600C0BC RID: 49340 RVA: 0x00490126 File Offset: 0x0048E326
		// (set) Token: 0x0600C0BD RID: 49341 RVA: 0x0049012E File Offset: 0x0048E32E
		public string UserId
		{
			get
			{
				return this.m_userId;
			}
			set
			{
				this.m_userId = value;
			}
		}

		// Token: 0x1700162B RID: 5675
		// (get) Token: 0x0600C0BE RID: 49342 RVA: 0x00490137 File Offset: 0x0048E337
		// (set) Token: 0x0600C0BF RID: 49343 RVA: 0x0049013F File Offset: 0x0048E33F
		public Action<Exception> ExceptionReporter { get; set; }

		// Token: 0x0600C0C0 RID: 49344
		public abstract IGameSparksTimer GetTimer();

		// Token: 0x0600C0C1 RID: 49345
		public abstract string MakeHmac(string stringToHmac, string secret);

		// Token: 0x0600C0C2 RID: 49346
		public abstract IGameSparksWebSocket GetSocket(string url, Action<string> messageReceived, Action closed, Action opened, Action<string> error);

		// Token: 0x0600C0C3 RID: 49347
		public abstract IGameSparksWebSocket GetBinarySocket(string url, Action<byte[]> messageReceived, Action closed, Action opened, Action<string> error);

		// Token: 0x0600C0C4 RID: 49348 RVA: 0x00490148 File Offset: 0x0048E348
		[PublicizedFrom(EAccessModifier.Protected)]
		public PlatformBase()
		{
		}

		// Token: 0x0400963A RID: 38458
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static string PLAYER_PREF_AUTHTOKEN_KEY = "gamesparks.authtoken";

		// Token: 0x0400963B RID: 38459
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static string PLAYER_PREF_USERID_KEY = "gamesparks.userid";

		// Token: 0x0400963C RID: 38460
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public static string PLAYER_PREF_DEVICEID_KEY = "gamesparks.deviceid";

		// Token: 0x0400963D RID: 38461
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public List<Action> _actions = new List<Action>();

		// Token: 0x0400963E RID: 38462
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public List<Action> _currentActions = new List<Action>();

		// Token: 0x0400963F RID: 38463
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public bool _allowQuitting;

		// Token: 0x04009647 RID: 38471
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public string m_authToken = "0";

		// Token: 0x04009648 RID: 38472
		[PublicizedFrom(EAccessModifier.Private)]
		[NonSerialized]
		public string m_userId = "";
	}
}

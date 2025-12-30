using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace Twitch
{
	// Token: 0x02001566 RID: 5478
	public class TwitchIRCClient
	{
		// Token: 0x170012AF RID: 4783
		// (get) Token: 0x0600A83C RID: 43068 RVA: 0x00423850 File Offset: 0x00421A50
		public bool IsConnected
		{
			get
			{
				return this.tcpClient.Connected;
			}
		}

		// Token: 0x0600A83D RID: 43069 RVA: 0x00423860 File Offset: 0x00421A60
		public TwitchIRCClient(string ip, int port, string channel, string password)
		{
			this.userName = channel;
			this.password = password;
			this.channel = channel;
			this.ip = ip;
			this.port = port;
			this.Reconnect();
		}

		// Token: 0x0600A83E RID: 43070 RVA: 0x004238B4 File Offset: 0x00421AB4
		public void Reconnect()
		{
			this.tcpClient = new TcpClient(this.ip, this.port);
			this.inputStream = new StreamReader(this.tcpClient.GetStream());
			this.outputStream = new StreamWriter(this.tcpClient.GetStream());
			this.outputStream.WriteLine("PASS " + this.password);
			this.outputStream.WriteLine("NICK " + this.userName);
			this.outputStream.WriteLine("JOIN #" + this.channel);
			this.pingTimerRunning = true;
			this.outputStream.Flush();
		}

		// Token: 0x0600A83F RID: 43071 RVA: 0x00423967 File Offset: 0x00421B67
		public void Disconnect()
		{
			if (this.tcpClient != null)
			{
				this.tcpClient.Close();
			}
			if (this.inputStream != null)
			{
				this.inputStream.Close();
			}
			if (this.outputStream != null)
			{
				this.outputStream.Close();
			}
		}

		// Token: 0x0600A840 RID: 43072 RVA: 0x004239A4 File Offset: 0x00421BA4
		public bool Update(float deltaTime)
		{
			if (this.pingTimerRunning)
			{
				this.PingTimer -= deltaTime;
				if (this.PingTimer <= 0f)
				{
					if (this.tcpClient.Connected)
					{
						this.SendIrcMessage("PING irc.twitch.tv", false);
					}
					else
					{
						this.Reconnect();
						this.pingTimerRunning = false;
					}
					this.PingTimer = 250f;
				}
			}
			if (this.outputQueue.Count > 0)
			{
				this.outputStream.WriteLine(this.outputQueue[0]);
				this.outputQueue.RemoveAt(0);
				this.outputStream.Flush();
			}
			return true;
		}

		// Token: 0x0600A841 RID: 43073 RVA: 0x00423A44 File Offset: 0x00421C44
		public void SendIrcMessage(string message, bool useQueue)
		{
			if (!this.tcpClient.Connected)
			{
				this.Reconnect();
			}
			if (useQueue)
			{
				this.outputQueue.Add(message);
				return;
			}
			this.outputStream.WriteLine(message);
			this.outputStream.Flush();
		}

		// Token: 0x0600A842 RID: 43074 RVA: 0x00423A80 File Offset: 0x00421C80
		public void SendIrcMessages(List<string> messages, bool useQueue)
		{
			if (useQueue)
			{
				this.outputQueue.AddRange(messages);
				return;
			}
			for (int i = 0; i < messages.Count; i++)
			{
				this.outputStream.WriteLine(messages[i]);
			}
			this.outputStream.Flush();
		}

		// Token: 0x0600A843 RID: 43075 RVA: 0x00423ACC File Offset: 0x00421CCC
		public void SendChannelMessage(string message, bool useQueue)
		{
			if (useQueue)
			{
				this.outputQueue.Add("PRIVMSG #" + this.userName + " :/me " + message);
				return;
			}
			this.outputStream.WriteLine("PRIVMSG #" + this.userName + " :/me " + message);
			this.outputStream.Flush();
		}

		// Token: 0x0600A844 RID: 43076 RVA: 0x00423B2C File Offset: 0x00421D2C
		public void SendChannelMessages(List<string> messages, bool useQueue)
		{
			if (useQueue)
			{
				for (int i = 0; i < messages.Count; i++)
				{
					this.outputQueue.Add("PRIVMSG #" + this.userName + " :/me " + messages[i]);
				}
				return;
			}
			for (int j = 0; j < messages.Count; j++)
			{
				this.outputStream.WriteLine("PRIVMSG #" + this.userName + " :/me " + messages[j]);
			}
			this.outputStream.Flush();
		}

		// Token: 0x0600A845 RID: 43077 RVA: 0x00423BB8 File Offset: 0x00421DB8
		public bool AvailableMessage()
		{
			return this.tcpClient.Available > 0;
		}

		// Token: 0x0600A846 RID: 43078 RVA: 0x00423BC8 File Offset: 0x00421DC8
		public TwitchIRCClient.TwitchChatMessage ReadMessage()
		{
			return this.ParseMessage();
		}

		// Token: 0x0600A847 RID: 43079 RVA: 0x00423BD0 File Offset: 0x00421DD0
		public TwitchIRCClient.TwitchChatMessage ParseMessage()
		{
			return new TwitchIRCClient.TwitchChatMessage(this.inputStream.ReadLine());
		}

		// Token: 0x0600A848 RID: 43080 RVA: 0x00423BE2 File Offset: 0x00421DE2
		public void SendChatMessage(string message)
		{
			this.SendIrcMessage(string.Format(":{0}!{0}@{0}.tmi.twitch.tv PRIVMSG #{1} :{2}", this.userName, this.channel, message), true);
		}

		// Token: 0x04008289 RID: 33417
		[PublicizedFrom(EAccessModifier.Private)]
		public string userName;

		// Token: 0x0400828A RID: 33418
		[PublicizedFrom(EAccessModifier.Private)]
		public string channel;

		// Token: 0x0400828B RID: 33419
		[PublicizedFrom(EAccessModifier.Private)]
		public string password;

		// Token: 0x0400828C RID: 33420
		[PublicizedFrom(EAccessModifier.Private)]
		public string ip;

		// Token: 0x0400828D RID: 33421
		[PublicizedFrom(EAccessModifier.Private)]
		public int port;

		// Token: 0x0400828E RID: 33422
		[PublicizedFrom(EAccessModifier.Private)]
		public TcpClient tcpClient;

		// Token: 0x0400828F RID: 33423
		[PublicizedFrom(EAccessModifier.Private)]
		public StreamReader inputStream;

		// Token: 0x04008290 RID: 33424
		[PublicizedFrom(EAccessModifier.Private)]
		public StreamWriter outputStream;

		// Token: 0x04008291 RID: 33425
		[PublicizedFrom(EAccessModifier.Private)]
		public static float pingMaxTimer = 100f;

		// Token: 0x04008292 RID: 33426
		[PublicizedFrom(EAccessModifier.Private)]
		public float PingTimer = TwitchIRCClient.pingMaxTimer;

		// Token: 0x04008293 RID: 33427
		[PublicizedFrom(EAccessModifier.Private)]
		public bool pingTimerRunning;

		// Token: 0x04008294 RID: 33428
		public List<string> outputQueue = new List<string>();

		// Token: 0x04008295 RID: 33429
		[PublicizedFrom(EAccessModifier.Private)]
		public static string TWITCH_SYSTEM_STRING = "tmi.twitch.tv";

		// Token: 0x04008296 RID: 33430
		[PublicizedFrom(EAccessModifier.Private)]
		public static string TWITCH_CONNECTION_STRING = ":tmi.twitch.tv 001";

		// Token: 0x04008297 RID: 33431
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PRIV_MSG_STRING = "PRIVMSG";

		// Token: 0x04008298 RID: 33432
		[PublicizedFrom(EAccessModifier.Private)]
		public static string PRIV_MSG_STRING_PARSE = "PRIVMSG #";

		// Token: 0x04008299 RID: 33433
		[PublicizedFrom(EAccessModifier.Private)]
		public static string MSG_RAID_STRING = "msg-id=raid";

		// Token: 0x0400829A RID: 33434
		[PublicizedFrom(EAccessModifier.Private)]
		public static string MSG_CHARITY_STRING = "msg-id=charitydonation";

		// Token: 0x02001567 RID: 5479
		public class TwitchChatMessage
		{
			// Token: 0x170012B0 RID: 4784
			// (get) Token: 0x0600A84A RID: 43082 RVA: 0x00423C57 File Offset: 0x00421E57
			// (set) Token: 0x0600A84B RID: 43083 RVA: 0x00423C5F File Offset: 0x00421E5F
			public virtual TwitchIRCClient.TwitchChatMessage.MessageTypes MessageType { get; [PublicizedFrom(EAccessModifier.Private)] set; }

			// Token: 0x0600A84C RID: 43084 RVA: 0x00423C68 File Offset: 0x00421E68
			public TwitchChatMessage(string message)
			{
				if (message.IndexOf(TwitchIRCClient.TWITCH_SYSTEM_STRING) != -1)
				{
					if (message.StartsWith(TwitchIRCClient.TWITCH_CONNECTION_STRING))
					{
						this.Message = message;
						this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Authenticated;
						return;
					}
					int num = -1;
					int num2 = -1;
					if (message.Contains(TwitchIRCClient.PRIV_MSG_STRING))
					{
						string[] array = message.Split(';', StringSplitOptions.None);
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i].StartsWith("@badge-info"))
							{
								if (array[i].Length >= 15 && (array[i][12] == 'f' || array[i][12] == 's'))
								{
									this.isSub = true;
								}
							}
							else if (array[i].StartsWith("badges"))
							{
								if (array[i].Contains("broadcaster"))
								{
									this.isSub = true;
									this.isMod = true;
									this.isVIP = true;
									this.isBroadcaster = true;
								}
								else if (array[i].Contains("vip"))
								{
									this.isVIP = true;
								}
							}
							else if (array[i].StartsWith("mod"))
							{
								if (array[i][4] == '1')
								{
									this.isMod = true;
								}
							}
							else if (array[i].StartsWith("user-type"))
							{
								message = message.Substring(message.IndexOf('@', 1) + 1);
							}
							else if (array[i].StartsWith("user-id"))
							{
								this.UserID = Convert.ToInt32(array[i].Substring(8));
							}
							else if (array[i].StartsWith("room-id"))
							{
								num2 = Convert.ToInt32(array[i].Substring(8));
							}
							else if (array[i].StartsWith("source-room-id"))
							{
								num = Convert.ToInt32(array[i].Substring(15));
							}
							else if (array[i].StartsWith("color"))
							{
								if (array[i].Length > 7)
								{
									this.UserNameColor = array[i].Substring(7);
								}
							}
							else if (array[i].StartsWith("reply-parent-msg-body"))
							{
								this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Invalid;
								return;
							}
						}
						if (num2 != num && num != -1)
						{
							this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Invalid;
							return;
						}
						message.IndexOf(TwitchIRCClient.PRIV_MSG_STRING_PARSE);
						int num3 = message.IndexOf('.', 1);
						string userName = message.Substring(0, num3);
						num3 = message.IndexOf(":");
						message = message.Substring(num3 + 1);
						this.UserName = userName;
						this.Message = message;
						this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Message;
						return;
					}
					else
					{
						if (message.Contains(TwitchIRCClient.MSG_RAID_STRING))
						{
							string[] array2 = message.Split(';', StringSplitOptions.None);
							for (int j = 0; j < array2.Length; j++)
							{
								if (array2[j].StartsWith("msg-param-displayName"))
								{
									this.UserName = array2[j].Substring(22);
								}
								else if (array2[j].StartsWith("msg-param-viewerCount"))
								{
									this.Message = array2[j].Substring(22);
								}
								else if (array2[j].StartsWith("user-id"))
								{
									this.UserID = Convert.ToInt32(array2[j].Substring(8));
								}
							}
							this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Raid;
							return;
						}
						if (message.Contains(TwitchIRCClient.MSG_CHARITY_STRING))
						{
							string[] array3 = message.Split(';', StringSplitOptions.None);
							for (int k = 0; k < array3.Length; k++)
							{
								if (array3[k].StartsWith("display-name"))
								{
									this.UserName = array3[k].Substring(13);
								}
								else if (array3[k].StartsWith("msg-param-donation-amount"))
								{
									this.Message = array3[k].Substring(26);
								}
								else if (array3[k].StartsWith("user-id"))
								{
									this.UserID = Convert.ToInt32(array3[k].Substring(8));
								}
							}
							this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Charity;
							return;
						}
					}
				}
				this.Message = message;
				this.MessageType = TwitchIRCClient.TwitchChatMessage.MessageTypes.Output;
			}

			// Token: 0x0400829C RID: 33436
			public bool isMod;

			// Token: 0x0400829D RID: 33437
			public bool isVIP;

			// Token: 0x0400829E RID: 33438
			public bool isSub;

			// Token: 0x0400829F RID: 33439
			public bool isBroadcaster;

			// Token: 0x040082A0 RID: 33440
			public string UserName;

			// Token: 0x040082A1 RID: 33441
			public int UserID;

			// Token: 0x040082A2 RID: 33442
			public string UserNameColor = "FFFFFF";

			// Token: 0x040082A3 RID: 33443
			public string Message;

			// Token: 0x02001568 RID: 5480
			public enum MessageTypes
			{
				// Token: 0x040082A5 RID: 33445
				Invalid = -1,
				// Token: 0x040082A6 RID: 33446
				Message,
				// Token: 0x040082A7 RID: 33447
				Output,
				// Token: 0x040082A8 RID: 33448
				Authenticated,
				// Token: 0x040082A9 RID: 33449
				Raid,
				// Token: 0x040082AA RID: 33450
				Charity
			}
		}
	}
}

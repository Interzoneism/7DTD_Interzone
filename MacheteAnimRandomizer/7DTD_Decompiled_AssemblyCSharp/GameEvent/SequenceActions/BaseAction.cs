using System;
using System.Collections;
using System.Collections.Generic;
using GameEvent.SequenceRequirements;
using UnityEngine;

namespace GameEvent.SequenceActions
{
	// Token: 0x020016C0 RID: 5824
	public class BaseAction
	{
		// Token: 0x1700139D RID: 5021
		// (get) Token: 0x0600B0D5 RID: 45269 RVA: 0x0045039F File Offset: 0x0044E59F
		// (set) Token: 0x0600B0D6 RID: 45270 RVA: 0x004503A8 File Offset: 0x0044E5A8
		public GameEventActionSequence Owner
		{
			get
			{
				return this.owner;
			}
			set
			{
				this.owner = value;
				if (this.Requirements != null)
				{
					for (int i = 0; i < this.Requirements.Count; i++)
					{
						this.Requirements[i].Owner = value;
					}
				}
			}
		}

		// Token: 0x1700139E RID: 5022
		// (get) Token: 0x0600B0D7 RID: 45271 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual bool UseRequirements
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600B0D8 RID: 45272 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnInit()
		{
		}

		// Token: 0x0600B0D9 RID: 45273 RVA: 0x004503EC File Offset: 0x0044E5EC
		public void Init()
		{
			this.OnInit();
			this.IsComplete = false;
		}

		// Token: 0x0600B0DA RID: 45274 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual bool CanPerform(Entity target)
		{
			return true;
		}

		// Token: 0x0600B0DB RID: 45275 RVA: 0x00002914 File Offset: 0x00000B14
		public virtual void OnClientPerform(Entity target)
		{
		}

		// Token: 0x0600B0DC RID: 45276 RVA: 0x000197A5 File Offset: 0x000179A5
		public virtual BaseAction.ActionCompleteStates OnPerformAction()
		{
			return BaseAction.ActionCompleteStates.InCompleteRefund;
		}

		// Token: 0x0600B0DD RID: 45277 RVA: 0x004503FC File Offset: 0x0044E5FC
		public BaseAction.ActionCompleteStates PerformAction()
		{
			if (this.UseRequirements && this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					this.Requirements[i].Owner = this.Owner;
					if (!this.Requirements[i].CanPerform(this.Owner.Target))
					{
						return BaseAction.ActionCompleteStates.Complete;
					}
				}
			}
			return this.OnPerformAction();
		}

		// Token: 0x0600B0DE RID: 45278 RVA: 0x0045046C File Offset: 0x0044E66C
		public void Reset()
		{
			this.IsComplete = false;
			this.OnReset();
		}

		// Token: 0x0600B0DF RID: 45279 RVA: 0x00002914 File Offset: 0x00000B14
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual void OnReset()
		{
		}

		// Token: 0x0600B0E0 RID: 45280 RVA: 0x0045047C File Offset: 0x0044E67C
		public virtual void ParseProperties(DynamicProperties properties)
		{
			this.Properties = properties;
			this.Owner.HandleVariablesForProperties(properties);
			properties.ParseInt(BaseAction.PropPhase, ref this.Phase);
			properties.ParseInt(BaseAction.PropPhaseOnComplete, ref this.PhaseOnComplete);
			properties.ParseBool(BaseAction.PropIgnoreRefund, ref this.IgnoreRefund);
		}

		// Token: 0x0600B0E1 RID: 45281 RVA: 0x004504D0 File Offset: 0x0044E6D0
		public virtual void HandleTemplateInit(GameEventActionSequence seq)
		{
			seq.HandleVariablesForProperties(this.Properties);
			this.Owner = seq;
			if (this.Properties != null)
			{
				this.ParseProperties(this.Properties);
			}
			this.Init();
			if (this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					seq.HandleVariablesForProperties(this.Requirements[i].Properties);
					if (this.Requirements[i].Properties != null)
					{
						this.Requirements[i].ParseProperties(this.Requirements[i].Properties);
					}
					this.Requirements[i].Init();
				}
			}
		}

		// Token: 0x0600B0E2 RID: 45282 RVA: 0x00450585 File Offset: 0x0044E785
		public void AddRequirement(BaseRequirement req)
		{
			if (this.Requirements == null)
			{
				this.Requirements = new List<BaseRequirement>();
			}
			req.Owner = this.Owner;
			this.Requirements.Add(req);
		}

		// Token: 0x0600B0E3 RID: 45283 RVA: 0x004505B4 File Offset: 0x0044E7B4
		public virtual BaseAction Clone()
		{
			BaseAction baseAction = this.CloneChildSettings();
			if (this.Properties != null)
			{
				baseAction.Properties = new DynamicProperties();
				baseAction.Properties.CopyFrom(this.Properties, null);
			}
			baseAction.Phase = this.Phase;
			baseAction.PhaseOnComplete = this.PhaseOnComplete;
			baseAction.IsComplete = false;
			baseAction.ActionIndex = this.ActionIndex;
			baseAction.IgnoreRefund = this.IgnoreRefund;
			if (this.Requirements != null)
			{
				for (int i = 0; i < this.Requirements.Count; i++)
				{
					baseAction.AddRequirement(this.Requirements[i].Clone());
				}
			}
			return baseAction;
		}

		// Token: 0x0600B0E4 RID: 45284 RVA: 0x00019766 File Offset: 0x00017966
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual BaseAction CloneChildSettings()
		{
			return null;
		}

		// Token: 0x0600B0E5 RID: 45285 RVA: 0x0045065A File Offset: 0x0044E85A
		public IEnumerator TeleportEntity(Entity entity, Vector3 position, float teleportDelay)
		{
			yield return new WaitForSeconds(teleportDelay);
			EntityPlayer entityPlayer = entity as EntityPlayer;
			if (entityPlayer != null)
			{
				if (entityPlayer.isEntityRemote)
				{
					SingletonMonoBehaviour<ConnectionManager>.Instance.SendPackage(NetPackageManager.GetPackage<NetPackageCloseAllWindows>().Setup(entityPlayer.entityId), false, entityPlayer.entityId, -1, -1, null, 192, false);
					SingletonMonoBehaviour<ConnectionManager>.Instance.Clients.ForEntityId(entityPlayer.entityId).SendPackage(NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(position, null, false));
				}
				else
				{
					((EntityPlayerLocal)entityPlayer).PlayerUI.windowManager.CloseAllOpenWindows(null, false);
					((EntityPlayerLocal)entityPlayer).TeleportToPosition(position, false, null);
				}
			}
			else if (entity.AttachedToEntity != null)
			{
				entity.AttachedToEntity.SetPosition(position, true);
			}
			else
			{
				entity.SetPosition(position, true);
			}
			yield break;
		}

		// Token: 0x0600B0E6 RID: 45286 RVA: 0x000B040E File Offset: 0x000AE60E
		[PublicizedFrom(EAccessModifier.Protected)]
		public virtual string ParseTextElement(string element)
		{
			return element;
		}

		// Token: 0x0600B0E7 RID: 45287 RVA: 0x00450678 File Offset: 0x0044E878
		[PublicizedFrom(EAccessModifier.Protected)]
		public string GetTextWithElements(string text)
		{
			int num = text.IndexOf("{", StringComparison.Ordinal);
			Dictionary<string, string> dictionary = null;
			while (num != -1)
			{
				int num2 = text.IndexOf("}", num, StringComparison.Ordinal);
				if (num2 == -1)
				{
					break;
				}
				string text2 = text.Substring(num + 1, num2 - num - 1);
				string text3 = this.ParseTextElement(text2);
				if (text3 != text2)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<string, string>();
					}
					dictionary.Add(text.Substring(num, num2 - num + 1), text3);
				}
				num = text.IndexOf("{", num2, StringComparison.Ordinal);
			}
			if (dictionary != null)
			{
				foreach (string text4 in dictionary.Keys)
				{
					text = text.Replace(text4, dictionary[text4]);
				}
			}
			return text;
		}

		// Token: 0x0600B0E8 RID: 45288 RVA: 0x00450750 File Offset: 0x0044E950
		public virtual BaseAction HandleAssignFrom(GameEventActionSequence newSeq, GameEventActionSequence oldSeq)
		{
			BaseAction baseAction = this.Clone();
			baseAction.Properties = new DynamicProperties();
			if (this.Properties != null)
			{
				baseAction.Properties.CopyFrom(this.Properties, null);
			}
			baseAction.Owner = newSeq;
			if (baseAction.Requirements != null)
			{
				for (int i = 0; i < baseAction.Requirements.Count; i++)
				{
					baseAction.Requirements[i].Properties = new DynamicProperties();
					if (this.Requirements[i].Properties != null)
					{
						baseAction.Requirements[i].Properties.CopyFrom(this.Requirements[i].Properties, null);
					}
					baseAction.Requirements[i].Owner = newSeq;
					baseAction.Requirements[i].Init();
				}
			}
			return baseAction;
		}

		// Token: 0x04008A5B RID: 35419
		[PublicizedFrom(EAccessModifier.Protected)]
		public GameEventActionSequence owner;

		// Token: 0x04008A5C RID: 35420
		public int Phase;

		// Token: 0x04008A5D RID: 35421
		public int PhaseOnComplete = -1;

		// Token: 0x04008A5E RID: 35422
		public int ActionIndex;

		// Token: 0x04008A5F RID: 35423
		public bool IgnoreRefund;

		// Token: 0x04008A60 RID: 35424
		public bool IsComplete;

		// Token: 0x04008A61 RID: 35425
		public DynamicProperties Properties;

		// Token: 0x04008A62 RID: 35426
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPhase = "phase";

		// Token: 0x04008A63 RID: 35427
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropPhaseOnComplete = "phase_on_complete";

		// Token: 0x04008A64 RID: 35428
		[PublicizedFrom(EAccessModifier.Protected)]
		public static string PropIgnoreRefund = "ignore_refund";

		// Token: 0x04008A65 RID: 35429
		public List<BaseRequirement> Requirements;

		// Token: 0x020016C1 RID: 5825
		public enum ActionCompleteStates
		{
			// Token: 0x04008A67 RID: 35431
			InComplete,
			// Token: 0x04008A68 RID: 35432
			InCompleteRefund,
			// Token: 0x04008A69 RID: 35433
			Complete
		}
	}
}

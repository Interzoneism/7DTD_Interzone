using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using InControl;

// Token: 0x020004DE RID: 1246
public abstract class PlayerActionsBase : PlayerActionSet
{
	// Token: 0x17000427 RID: 1063
	// (get) Token: 0x0600285D RID: 10333 RVA: 0x00106228 File Offset: 0x00104428
	// (set) Token: 0x0600285C RID: 10332 RVA: 0x0010621F File Offset: 0x0010441F
	public string Name { get; [PublicizedFrom(EAccessModifier.Protected)] set; }

	// Token: 0x0600285E RID: 10334 RVA: 0x00106230 File Offset: 0x00104430
	public PlayerActionsBase()
	{
		FieldInfo field = typeof(PlayerActionSet).GetField("actionsByName", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			field.SetValue(this, new CaseInsensitiveStringDictionary<PlayerAction>());
		}
		base.ListenOptions = new BindingListenOptions
		{
			UnsetDuplicateBindingsOnSet = false,
			MaxAllowedBindings = 0U,
			MaxAllowedBindingsPerType = 1U,
			AllowDuplicateBindingsPerSet = true,
			IncludeKeys = false,
			IncludeMouseButtons = false,
			IncludeControllers = false,
			IncludeModifiersAsFirstClassKeys = true
		};
		base.ListenOptions.OnBindingFound = delegate(PlayerAction _action, BindingSource _binding)
		{
			if (!_action.HasBinding(_binding))
			{
				return true;
			}
			Log.Out("Binding already bound.");
			_action.StopListeningForBinding();
			return false;
		};
		BindingListenOptions listenOptions = base.ListenOptions;
		listenOptions.OnBindingAdded = (Action<PlayerAction, BindingSource>)Delegate.Combine(listenOptions.OnBindingAdded, new Action<PlayerAction, BindingSource>(delegate(PlayerAction _action, BindingSource _binding)
		{
			Log.Out("Binding added for action {0} on device {1}: {2}", new object[]
			{
				_action.Name,
				_binding.DeviceName,
				_binding.Name
			});
		}));
		BindingListenOptions listenOptions2 = base.ListenOptions;
		listenOptions2.OnBindingRejected = (Action<PlayerAction, BindingSource, BindingSourceRejectionType>)Delegate.Combine(listenOptions2.OnBindingRejected, new Action<PlayerAction, BindingSource, BindingSourceRejectionType>(delegate(PlayerAction _action, BindingSource _binding, BindingSourceRejectionType _reason)
		{
			Log.Out("Binding rejected for action {0}: {1}", new object[]
			{
				_action.Name,
				_reason.ToStringCached<BindingSourceRejectionType>()
			});
		}));
		this.InitActionSet();
	}

	// Token: 0x0600285F RID: 10335 RVA: 0x0010635C File Offset: 0x0010455C
	[PublicizedFrom(EAccessModifier.Private)]
	public void InitActionSet()
	{
		this.CreateActions();
		this.CreateDefaultKeyboardBindings();
		this.CreateDefaultJoystickBindings();
	}

	// Token: 0x06002860 RID: 10336
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void CreateActions();

	// Token: 0x06002861 RID: 10337
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void CreateDefaultKeyboardBindings();

	// Token: 0x06002862 RID: 10338
	[PublicizedFrom(EAccessModifier.Protected)]
	public abstract void CreateDefaultJoystickBindings();

	// Token: 0x06002863 RID: 10339 RVA: 0x00106370 File Offset: 0x00104570
	public void ResetControllerBindings()
	{
		this.AsyncResetControllerBindings();
	}

	// Token: 0x06002864 RID: 10340 RVA: 0x00106378 File Offset: 0x00104578
	[PublicizedFrom(EAccessModifier.Private)]
	public void AsyncResetControllerBindings()
	{
		PlayerActionsBase.<AsyncResetControllerBindings>d__11 <AsyncResetControllerBindings>d__;
		<AsyncResetControllerBindings>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<AsyncResetControllerBindings>d__.<>4__this = this;
		<AsyncResetControllerBindings>d__.<>1__state = -1;
		<AsyncResetControllerBindings>d__.<>t__builder.Start<PlayerActionsBase.<AsyncResetControllerBindings>d__11>(ref <AsyncResetControllerBindings>d__);
	}

	// Token: 0x04001F18 RID: 7960
	public List<PlayerAction> ControllerRebindableActions = new List<PlayerAction>();
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Platform;
using UnityEngine.Scripting;

// Token: 0x02000216 RID: 534
[Preserve]
public class ConsoleCmdPermissionsAllowed : ConsoleCmdAbstract
{
	// Token: 0x1700017D RID: 381
	// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool IsExecuteOnClient
	{
		get
		{
			return true;
		}
	}

	// Token: 0x1700017E RID: 382
	// (get) Token: 0x06000FA6 RID: 4006 RVA: 0x000197A5 File Offset: 0x000179A5
	public override bool AllowedInMainMenu
	{
		get
		{
			return true;
		}
	}

	// Token: 0x06000FA7 RID: 4007 RVA: 0x000655F0 File Offset: 0x000637F0
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string[] getCommands()
	{
		return new string[]
		{
			"permissionsallowed",
			"pallowed",
			"pa"
		};
	}

	// Token: 0x06000FA8 RID: 4008 RVA: 0x00065610 File Offset: 0x00063810
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getDescription()
	{
		return "Apply a mask to permissions for testing purposes (respects the existing conditions though).";
	}

	// Token: 0x06000FA9 RID: 4009 RVA: 0x00065618 File Offset: 0x00063818
	[PublicizedFrom(EAccessModifier.Protected)]
	public override string getHelp()
	{
		string str = string.Join<EUserPerms>("|", EnumUtils.Values<EUserPerms>());
		return string.Join('\n', new string[]
		{
			"pa i[nfo] - Prints info about the current permissions.",
			"pa g[rant] <" + str + "> - Adds the given permissions to the current debug permissions mask (still respects existing permissions though).",
			"pa rev[oke] <" + str + "> - Removes the given permissions from the current debug permissions mask.",
			"pa res[olve] <" + str + "> <true|false> - Attempt to resolve the specified permissions. True allows prompting the user for input, otherwise it is a silent resolution."
		});
	}

	// Token: 0x06000FAA RID: 4010 RVA: 0x00065684 File Offset: 0x00063884
	public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
	{
		string[] array = _params.ToArray();
		if (_params.Count > 0)
		{
			string text = _params[0].ToLowerInvariant();
			uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
			ConsoleCmdPermissionsAllowed.ExecuteSubCommand executeSubCommand;
			if (num <= 2935168099U)
			{
				if (num <= 804546073U)
				{
					if (num != 263456517U)
					{
						if (num != 804546073U)
						{
							goto IL_152;
						}
						if (!(text == "res"))
						{
							goto IL_152;
						}
					}
					else
					{
						if (!(text == "info"))
						{
							goto IL_152;
						}
						goto IL_124;
					}
				}
				else if (num != 854878930U)
				{
					if (num != 2935168099U)
					{
						goto IL_152;
					}
					if (!(text == "resolve"))
					{
						goto IL_152;
					}
				}
				else
				{
					if (!(text == "rev"))
					{
						goto IL_152;
					}
					goto IL_13B;
				}
				executeSubCommand = new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(this.ExecuteResolve);
				goto IL_154;
			}
			if (num <= 3691399731U)
			{
				if (num != 2975164269U)
				{
					if (num != 3691399731U)
					{
						goto IL_152;
					}
					if (!(text == "revoke"))
					{
						goto IL_152;
					}
					goto IL_13B;
				}
				else if (!(text == "grant"))
				{
					goto IL_152;
				}
			}
			else if (num != 3792446982U)
			{
				if (num != 3960223172U)
				{
					goto IL_152;
				}
				if (!(text == "i"))
				{
					goto IL_152;
				}
				goto IL_124;
			}
			else if (!(text == "g"))
			{
				goto IL_152;
			}
			executeSubCommand = ConsoleCmdPermissionsAllowed.ExecuteGrant;
			goto IL_154;
			IL_124:
			executeSubCommand = new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(ConsoleCmdPermissionsAllowed.ExecuteInfo);
			goto IL_154;
			IL_13B:
			executeSubCommand = ConsoleCmdPermissionsAllowed.ExecuteRevoke;
			goto IL_154;
			IL_152:
			executeSubCommand = null;
			IL_154:
			ConsoleCmdPermissionsAllowed.ExecuteSubCommand executeSubCommand2 = executeSubCommand;
			if (executeSubCommand2 == null)
			{
				Log.Warning("Unknown sub-command: " + _params[0]);
			}
			else if (executeSubCommand2(RuntimeHelpers.GetSubArray<string>(array, Range.StartAt(1))))
			{
				return;
			}
		}
		Log.Warning(this.GetHelp());
	}

	// Token: 0x06000FAB RID: 4011 RVA: 0x0006582C File Offset: 0x00063A2C
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool ExecuteInfo(ReadOnlySpan<string> parameters)
	{
		EUserPerms permissions = PlatformManager.NativePlatform.User.Permissions;
		IPlatform crossplatformPlatform = PlatformManager.CrossplatformPlatform;
		EUserPerms? euserPerms;
		if (crossplatformPlatform == null)
		{
			euserPerms = null;
		}
		else
		{
			IUserClient user = crossplatformPlatform.User;
			euserPerms = ((user != null) ? new EUserPerms?(user.Permissions) : null);
		}
		EUserPerms? euserPerms2 = euserPerms;
		StringBuilder stringBuilder = new StringBuilder(string.Format("User Native: {0}\nUser Cross: {1}", permissions, ((euserPerms2 != null) ? euserPerms2.GetValueOrDefault().ToString() : null) ?? "N/A"));
		foreach (object obj in Enum.GetValues(typeof(PermissionsManager.PermissionSources)))
		{
			PermissionsManager.PermissionSources permissionSources = (PermissionsManager.PermissionSources)obj;
			stringBuilder.Append(string.Format("\n{0}: {1}", permissionSources, PermissionsManager.GetPermissions(permissionSources)));
		}
		Log.Out(stringBuilder.ToString());
		return true;
	}

	// Token: 0x06000FAC RID: 4012 RVA: 0x00065944 File Offset: 0x00063B44
	[PublicizedFrom(EAccessModifier.Private)]
	public static EUserPerms MaskModifierGrant(EUserPerms previous, EUserPerms input)
	{
		return previous | input;
	}

	// Token: 0x06000FAD RID: 4013 RVA: 0x00065949 File Offset: 0x00063B49
	[PublicizedFrom(EAccessModifier.Private)]
	public static EUserPerms MaskModifierRevoke(EUserPerms previous, EUserPerms input)
	{
		return previous & ~input;
	}

	// Token: 0x06000FAE RID: 4014 RVA: 0x0006594F File Offset: 0x00063B4F
	[PublicizedFrom(EAccessModifier.Private)]
	public static ConsoleCmdPermissionsAllowed.ExecuteSubCommand CreateExecuteMaskModifier(ConsoleCmdPermissionsAllowed.MaskModifier modifier)
	{
		ConsoleCmdPermissionsAllowed.<>c__DisplayClass16_0 CS$<>8__locals1 = new ConsoleCmdPermissionsAllowed.<>c__DisplayClass16_0();
		CS$<>8__locals1.modifier = modifier;
		return new ConsoleCmdPermissionsAllowed.ExecuteSubCommand(CS$<>8__locals1.<CreateExecuteMaskModifier>g__ExecuteMaskModifier|0);
	}

	// Token: 0x06000FAF RID: 4015 RVA: 0x00065968 File Offset: 0x00063B68
	[PublicizedFrom(EAccessModifier.Private)]
	public unsafe bool ExecuteResolve(ReadOnlySpan<string> parameters)
	{
		if (parameters.Length != 2)
		{
			Log.Warning("Expected a two arguments.");
			return false;
		}
		EUserPerms permissionsToResolve;
		if (!ConsoleCmdPermissionsAllowed.TryParsePermission(*parameters[0], out permissionsToResolve))
		{
			return false;
		}
		bool shouldPrompt;
		if (!ConsoleCmdPermissionsAllowed.TryParseBoolean(*parameters[1], out shouldPrompt))
		{
			return false;
		}
		ThreadManager.StartCoroutine(this.ResolveCoroutine(permissionsToResolve, shouldPrompt));
		return true;
	}

	// Token: 0x06000FB0 RID: 4016 RVA: 0x000659C2 File Offset: 0x00063BC2
	[PublicizedFrom(EAccessModifier.Private)]
	public IEnumerator ResolveCoroutine(EUserPerms permissionsToResolve, bool shouldPrompt)
	{
		if (this.m_resolveCoroutineRunning)
		{
			Log.Warning("[Perms Resolve] Resolving in progress already.");
			yield break;
		}
		yield return null;
		try
		{
			this.m_resolveCoroutineRunning = true;
			Log.Out(string.Format("[Perms Resolve] Resolving Permissions '{0}'.", permissionsToResolve));
			yield return PermissionsManager.ResolvePermissions(permissionsToResolve, shouldPrompt, null);
		}
		finally
		{
			this.m_resolveCoroutineRunning = false;
		}
		yield break;
		yield break;
	}

	// Token: 0x06000FB1 RID: 4017 RVA: 0x000659E0 File Offset: 0x00063BE0
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool TryParseBoolean(string input, out bool result)
	{
		string text = input.ToLowerInvariant();
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1303515621U)
		{
			if (num <= 873244444U)
			{
				if (num != 184981848U)
				{
					if (num != 873244444U)
					{
						goto IL_102;
					}
					if (!(text == "1"))
					{
						goto IL_102;
					}
				}
				else
				{
					if (!(text == "false"))
					{
						goto IL_102;
					}
					goto IL_FD;
				}
			}
			else if (num != 890022063U)
			{
				if (num != 1303515621U)
				{
					goto IL_102;
				}
				if (!(text == "true"))
				{
					goto IL_102;
				}
			}
			else
			{
				if (!(text == "0"))
				{
					goto IL_102;
				}
				goto IL_FD;
			}
		}
		else if (num <= 2872740362U)
		{
			if (num != 1630810064U)
			{
				if (num != 2872740362U)
				{
					goto IL_102;
				}
				if (!(text == "off"))
				{
					goto IL_102;
				}
				goto IL_FD;
			}
			else if (!(text == "on"))
			{
				goto IL_102;
			}
		}
		else if (num != 3809224601U)
		{
			if (num != 4044111267U)
			{
				goto IL_102;
			}
			if (!(text == "t"))
			{
				goto IL_102;
			}
		}
		else
		{
			if (!(text == "f"))
			{
				goto IL_102;
			}
			goto IL_FD;
		}
		result = true;
		return true;
		IL_FD:
		result = false;
		return true;
		IL_102:
		Log.Warning("Expected true/false instead of: " + input);
		result = false;
		return false;
	}

	// Token: 0x06000FB2 RID: 4018 RVA: 0x00065B04 File Offset: 0x00063D04
	[PublicizedFrom(EAccessModifier.Private)]
	public static bool TryParsePermission(string input, out EUserPerms result)
	{
		if (EnumUtils.TryParse<EUserPerms>(input, out result, true))
		{
			return true;
		}
		EUserPerms? euserPerms = null;
		foreach (ValueTuple<EUserPerms, string> valueTuple in EnumUtils.Values<EUserPerms>().Zip(EnumUtils.Names<EUserPerms>(), (EUserPerms perm, string name) => new ValueTuple<EUserPerms, string>(perm, name)))
		{
			EUserPerms item = valueTuple.Item1;
			if (valueTuple.Item2.StartsWith(input, StringComparison.OrdinalIgnoreCase))
			{
				if (euserPerms != null)
				{
					Log.Warning(string.Format("Input '{0}' is ambiguous between '{1}' and '{2}'.", input, euserPerms.Value, item));
					result = (EUserPerms)0;
					return false;
				}
				euserPerms = new EUserPerms?(item);
			}
		}
		if (euserPerms == null)
		{
			Log.Warning("Input '" + input + "' did not match any permissions.");
			result = (EUserPerms)0;
			return false;
		}
		result = euserPerms.Value;
		return true;
	}

	// Token: 0x04000B2B RID: 2859
	[PublicizedFrom(EAccessModifier.Private)]
	public bool m_resolveCoroutineRunning;

	// Token: 0x04000B2C RID: 2860
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ConsoleCmdPermissionsAllowed.ExecuteSubCommand ExecuteGrant = ConsoleCmdPermissionsAllowed.CreateExecuteMaskModifier(new ConsoleCmdPermissionsAllowed.MaskModifier(ConsoleCmdPermissionsAllowed.MaskModifierGrant));

	// Token: 0x04000B2D RID: 2861
	[PublicizedFrom(EAccessModifier.Private)]
	public static readonly ConsoleCmdPermissionsAllowed.ExecuteSubCommand ExecuteRevoke = ConsoleCmdPermissionsAllowed.CreateExecuteMaskModifier(new ConsoleCmdPermissionsAllowed.MaskModifier(ConsoleCmdPermissionsAllowed.MaskModifierRevoke));

	// Token: 0x02000217 RID: 535
	// (Invoke) Token: 0x06000FB6 RID: 4022
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate bool ExecuteSubCommand(ReadOnlySpan<string> parameters);

	// Token: 0x02000218 RID: 536
	// (Invoke) Token: 0x06000FBA RID: 4026
	[PublicizedFrom(EAccessModifier.Private)]
	public delegate EUserPerms MaskModifier(EUserPerms previous, EUserPerms input);
}

using System;

namespace Audio
{
	// Token: 0x02001793 RID: 6035
	public class StrAudioClip
	{
		// Token: 0x04008D16 RID: 36118
		public const string ItemCollected = "item_pickup";

		// Token: 0x04008D17 RID: 36119
		public const string ItemPlantCollected = "item_plant_pickup";

		// Token: 0x04008D18 RID: 36120
		public const string EntityHitsGround = "entityhitsground";

		// Token: 0x04008D19 RID: 36121
		public const string ItemDropped = "itemdropped";

		// Token: 0x04008D1A RID: 36122
		public const string OpenDoor = "open_door_wood";

		// Token: 0x04008D1B RID: 36123
		public const string CloseDoor = "close_door_wood";

		// Token: 0x04008D1C RID: 36124
		public const string FallingIntoWater = "waterfallinginto";

		// Token: 0x04008D1D RID: 36125
		public const string BuildingCompleted = "placeblock";

		// Token: 0x04008D1E RID: 36126
		public const string RotateBlock = "rotateblock";

		// Token: 0x04008D1F RID: 36127
		public const string TrunkBreaks = "trunkbreak";

		// Token: 0x04008D20 RID: 36128
		public const string TrunkFallImpact = "treefallimpact";

		// Token: 0x04008D21 RID: 36129
		public const string FlashlightToggle = "flashlight_toggle";

		// Token: 0x04008D22 RID: 36130
		public const string GenericHolster = "generic_holster";

		// Token: 0x04008D23 RID: 36131
		public const string GenericUnholster = "generic_unholster";

		// Token: 0x04008D24 RID: 36132
		public const string MissingItemToRepair = "missingitemtorepair";

		// Token: 0x04008D25 RID: 36133
		public const string CraftingClick = "craft_click_craft";

		// Token: 0x04008D26 RID: 36134
		public const string RecipeUnlocked = "recipe_unlocked";

		// Token: 0x04008D27 RID: 36135
		public const string OpenInventory = "open_inventory";

		// Token: 0x04008D28 RID: 36136
		public const string CloseInventory = "close_inventory";

		// Token: 0x04008D29 RID: 36137
		public const string CampfireOpen = "campfire_open";

		// Token: 0x04008D2A RID: 36138
		public const string CampfireClose = "campfire_close";

		// Token: 0x04008D2B RID: 36139
		public const string CampfireCookClick = "campfire_cook_click";

		// Token: 0x04008D2C RID: 36140
		public const string ForgeOpen = "forge_open";

		// Token: 0x04008D2D RID: 36141
		public const string ForgeClose = "forge_close";

		// Token: 0x04008D2E RID: 36142
		public const string ForgeSmeltClick = "forge_smelt_click";

		// Token: 0x04008D2F RID: 36143
		public const string ForgeBurn = "forge_burn_fuel";

		// Token: 0x04008D30 RID: 36144
		public const string ForgeFireDie = "forge_fire_die";

		// Token: 0x04008D31 RID: 36145
		public const string CementMixerOpen = "cement_mixer_open";

		// Token: 0x04008D32 RID: 36146
		public const string CementMixerClose = "cement_mixer_close";

		// Token: 0x04008D33 RID: 36147
		public const string CementMixerClick = "cement_mixer_start_click";

		// Token: 0x04008D34 RID: 36148
		public const string CraftComplete = "craft_complete_item";

		// Token: 0x04008D35 RID: 36149
		public const string CampfireComplete = "campfire_complete_item";

		// Token: 0x04008D36 RID: 36150
		public const string ForgeComplete = "forge_item_complete";

		// Token: 0x04008D37 RID: 36151
		public const string CementMixerComplete = "cement_mixer_complete";

		// Token: 0x04008D38 RID: 36152
		public const string ChemStationOpen = "chem_station_open";

		// Token: 0x04008D39 RID: 36153
		public const string ChemStationClose = "chem_station_close";

		// Token: 0x04008D3A RID: 36154
		public const string ChemStationClick = "chem_station_mix_click";

		// Token: 0x04008D3B RID: 36155
		public const string ChemStationComplete = "chem_station_complete_item";

		// Token: 0x04008D3C RID: 36156
		public const string QuestNoteOffered = "quest_note_offer";

		// Token: 0x04008D3D RID: 36157
		public const string QuestNoteDeclined = "quest_note_decline";

		// Token: 0x04008D3E RID: 36158
		public const string QuestStarted = "quest_started";

		// Token: 0x04008D3F RID: 36159
		public const string QuestFailed = "quest_failed";

		// Token: 0x04008D40 RID: 36160
		public const string QuestCompleted = "quest_subtask_complete";

		// Token: 0x04008D41 RID: 36161
		public const string QuestObjective = "quest_objective_complete";

		// Token: 0x04008D42 RID: 36162
		public const string QuestChainCompleted = "quest_master_complete";

		// Token: 0x04008D43 RID: 36163
		public const string SkillPurchase = "ui_skill_purchase";

		// Token: 0x04008D44 RID: 36164
		public const string TraderPurchase = "ui_trader_purchase";

		// Token: 0x04008D45 RID: 36165
		public const string VendingPurchase = "ui_vending_purchase";

		// Token: 0x04008D46 RID: 36166
		public const string VendingOpen = "open_vending";

		// Token: 0x04008D47 RID: 36167
		public const string VendingClose = "close_vending";

		// Token: 0x04008D48 RID: 36168
		public const string UITab = "ui_tab";

		// Token: 0x04008D49 RID: 36169
		public const string UIHover = "ui_hover";

		// Token: 0x04008D4A RID: 36170
		public const string UIDenied = "ui_denied";

		// Token: 0x04008D4B RID: 36171
		public const string MapZoomIn = "map_zoom_in";

		// Token: 0x04008D4C RID: 36172
		public const string MapZoomOut = "map_zoom_out";

		// Token: 0x04008D4D RID: 36173
		public const string WaypointAdd = "ui_waypoint_add";

		// Token: 0x04008D4E RID: 36174
		public const string WaypointDelete = "ui_waypoint_delete";

		// Token: 0x04008D4F RID: 36175
		public const string PickupItem = "craft_take_item";

		// Token: 0x04008D50 RID: 36176
		public const string PlaceItem = "craft_place_item";

		// Token: 0x04008D51 RID: 36177
		public const string SignOpen = "open_sign";

		// Token: 0x04008D52 RID: 36178
		public const string SignClose = "close_sign";

		// Token: 0x04008D53 RID: 36179
		public const string BatteryBankStart = "batterybank_start";

		// Token: 0x04008D54 RID: 36180
		public const string BatteryBankStop = "batterybank_stop";

		// Token: 0x04008D55 RID: 36181
		public const string GeneratorStart = "generator_start";

		// Token: 0x04008D56 RID: 36182
		public const string GeneratorStop = "generator_stop";

		// Token: 0x04008D57 RID: 36183
		public const string SolarPanelStart = "solarpanel_on";

		// Token: 0x04008D58 RID: 36184
		public const string SolarPanelStop = "solarpanel_off";

		// Token: 0x04008D59 RID: 36185
		public const string SwitchOn = "switch_up";

		// Token: 0x04008D5A RID: 36186
		public const string SwitchOff = "switch_down";

		// Token: 0x04008D5B RID: 36187
		public const string WireConnectLive = "wire_live_connect";

		// Token: 0x04008D5C RID: 36188
		public const string WireConnectDead = "wire_dead_connect";

		// Token: 0x04008D5D RID: 36189
		public const string WireBreakLive = "wire_live_break";

		// Token: 0x04008D5E RID: 36190
		public const string WireBreakDead = "wire_dead_break";

		// Token: 0x04008D5F RID: 36191
		public const string PressurePlateDown = "pressureplate_down";

		// Token: 0x04008D60 RID: 36192
		public const string PressurePlateUp = "pressureplate_up";

		// Token: 0x04008D61 RID: 36193
		public const string MotionSensorTrigger = "motion_sensor_trigger";

		// Token: 0x04008D62 RID: 36194
		public const string TripWireTrigger = "trip_wire_trigger";

		// Token: 0x04008D63 RID: 36195
		public const string LightOn = "light_on";

		// Token: 0x04008D64 RID: 36196
		public const string LightOff = "light_off";

		// Token: 0x04008D65 RID: 36197
		public const string TimerRelayStart = "timer_start";

		// Token: 0x04008D66 RID: 36198
		public const string TimerRelayStop = "timer_stop";

		// Token: 0x04008D67 RID: 36199
		public const string ItemBreak = "itembreak";

		// Token: 0x04008D68 RID: 36200
		public const string TwitchNoAttack = "twitch_no_attack";

		// Token: 0x04008D69 RID: 36201
		public const string TwitchVoteAdded = "twitch_vote_received";

		// Token: 0x04008D6A RID: 36202
		public const string TwitchVoteStarted = "twitch_vote_started";

		// Token: 0x04008D6B RID: 36203
		public const string TwitchVoteEnded = "twitch_vote_ended";

		// Token: 0x04008D6C RID: 36204
		public const string TwitchInflate = "twitch_bighead_inflate";

		// Token: 0x04008D6D RID: 36205
		public const string TwitchDeflate = "twitch_bighead_deflate";

		// Token: 0x04008D6E RID: 36206
		public const string TwitchCelebrate = "twitch_celebrate";

		// Token: 0x04008D6F RID: 36207
		public const string TwitchBalloonPop = "twitch_baseball_balloon_pop";

		// Token: 0x04008D70 RID: 36208
		public const string TwitchBalloonSpawn = "twitch_balloon_spawn";

		// Token: 0x04008D71 RID: 36209
		public const string TwitchBalloonDespawn = "twitch_balloon_despawn";

		// Token: 0x04008D72 RID: 36210
		public const string TwitchRefund = "twitch_refund";

		// Token: 0x04008D73 RID: 36211
		public const string TwitchPausedActions = "twitch_pause";

		// Token: 0x04008D74 RID: 36212
		public const string TwitchUnPausedActions = "twitch_unpause";

		// Token: 0x04008D75 RID: 36213
		public const string PartyInviteRecieve = "party_invite_receive";

		// Token: 0x04008D76 RID: 36214
		public const string PartyJoin = "party_join";

		// Token: 0x04008D77 RID: 36215
		public const string PartyLeave = "party_leave";

		// Token: 0x04008D78 RID: 36216
		public const string PartyMemberJoin = "party_member_join";

		// Token: 0x04008D79 RID: 36217
		public const string PartyMemberLeave = "party_member_leave";

		// Token: 0x04008D7A RID: 36218
		public const string ChallengeTrack = "ui_challenge_track";

		// Token: 0x04008D7B RID: 36219
		public const string ChallengeRedeem = "ui_challenge_redeem";

		// Token: 0x04008D7C RID: 36220
		public const string ChallengeComplete = "ui_challenge_complete";

		// Token: 0x04008D7D RID: 36221
		public const string ChallengeCompleteRow = "ui_challenge_complete_row";

		// Token: 0x04008D7E RID: 36222
		public const string ChallengeUnhideRow = "ui_challenge_unhide_row";

		// Token: 0x04008D7F RID: 36223
		public const string ChallengeObjectiveComplete = "ui_challenge_objective_complete";
	}
}

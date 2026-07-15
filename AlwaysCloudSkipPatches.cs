using HarmonyLib;
using Pigeon.Movement;
using System.Reflection;

static class AlwaysCloudSkipPatches
{
    private static readonly FieldInfo airJumpsField = AccessTools.Field(typeof(Player), "airJumps");
    private static readonly FieldInfo airJumpUpSpeedField = AccessTools.Field(typeof(Player), "airJumpUpSpeed");

    private static bool applied;
    private static int savedAirJumps;
    private static float savedAirJumpUpSpeed;

    [HarmonyPatch(typeof(Player), "Movement")]
    [HarmonyPrefix]
    public static bool MovementPrefix(Player __instance)
    {
        if (!__instance.IsLocalPlayer) return true;

        bool enabled = SparrohPlugin.enableCloudSkip != null && SparrohPlugin.enableCloudSkip.Value;

        if (enabled)
        {
            if (!applied)
            {
                savedAirJumps = (int)airJumpsField.GetValue(__instance);
                savedAirJumpUpSpeed = (float)airJumpUpSpeedField.GetValue(__instance);
                applied = true;
            }

            airJumpsField.SetValue(__instance, 1);
            airJumpUpSpeedField.SetValue(__instance, 18.3f);
        }
        else if (applied)
        {
            airJumpsField.SetValue(__instance, savedAirJumps);
            airJumpUpSpeedField.SetValue(__instance, savedAirJumpUpSpeed);
            applied = false;
        }

        return true;
    }
}

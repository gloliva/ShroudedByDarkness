using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LightMasterController
{
    #region public_light_variables
    public const int numLightItems = 4;
    public enum LIGHTS {SmallSpotlight, MedSpotlight, LargeSpotlight, Flashlight};
    public static string[] lanternNames = { "Small", "Medium", "Large", "Flashlight" };
    public static float[,] spotlightLanternValues = new float[4, 5] {
        { 12, 8, 133, -8, 1 }, // small lantern
        { 14, 8, 155, -8, 1 }, // medium lantern
        { 20, 10, 179, -12, 1 }, // large lantern
        { 12, 3, 65, -4, 1} // flashlight
    };
    public static float[] colliderRadius = { 0.5f, 0.5f, 0.5f };
    public static float[] lightDurations = {18, 18, 14, 18};
    #endregion
}

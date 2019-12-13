using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    #if UNITY_EDITOR
    public static string asset_path = Application.dataPath + "/StreamingAssets";
#elif UNITY_IPHONE || UNITY_ANDROID
    public static string asset_path = Application.persistentDataPath;
#endif

}

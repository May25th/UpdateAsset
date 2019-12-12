using UnityEditor;
using System.IO;

public class CreateAssetBundles:Editor
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string dir = "AssetBundles";
        if (Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }
        //BuildTarget 选择build出来的AB包要使用的平台
        /*
            BuildAssetBundleOptions.None LZMA算法压缩，压缩的包更小，但是加载时间更长
            BuildAssetBundleOptions.UncompressedAssetBundle：不压缩，包大，加载快
            BuildAssetBundleOptions.ChunkBasedCompression：使用LZ4压缩，压缩率没有LZMA高，但是我们可以加载指定资源而不用解压全部
        */
        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}

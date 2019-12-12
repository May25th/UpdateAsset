using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LoadFromLocal : MonoBehaviour
{
    private void Start()
    {
        //TestDiqiu();
        //StartCoroutine("Load");
        StartCoroutine(WWWLoad("http://bi.bellcat.cn/package/AssetBundles/test/fangkuai"));
    }

    private void TestDiqiu()
    {
        //加载对应材质球的依赖
        AssetBundle ab1 = AssetBundle.LoadFromFile("AssetBundles/ziyuan/ziyuan_01");
        AssetBundle ab = AssetBundle.LoadFromFile("AssetBundles/test/fangkuai");
        if (ab == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        GameObject obj = ab.LoadAsset<GameObject>("Diqiu");
        Instantiate(obj);
    }

    private void TestFangkuai()
    {
        AssetBundle ab = AssetBundle.LoadFromFile("AssetBundles/test/fangkuai");
        if (ab == null) {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }
        GameObject obj = ab.LoadAsset<GameObject>("Fangkuai");
        Instantiate(obj);
    }

    //使用协程来进行异步加载
    IEnumerator Load()
    { 
        //异步加载，返回一个结果 
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync("AssetBundles/test/fangkuai"); 
        yield return request; 
        AssetBundle ab = request.assetBundle; 
        //获取目标对象 
        GameObject go = ab.LoadAsset<GameObject>("Fangkuai");
        //实例化对象 
        Instantiate(go);
    }

    //使用这种方式加载，它会先把资源包存在本地的cache中
    //然后在从cache中加载ab包
    IEnumerator WWWLoad(string path)
    {
        //cache是否准备好
        while(Caching.ready==false){
            yield return null;
        }
        //根据路径加载，可以是网上的路径，也可以是本地路径
        //UnityWebRequest request = UnityWebRequest.GetAssetBundle(path);
        WWW www = new WWW(path);
        yield return www;
        if(www.error!=null){
            Debug.Log(www.error);
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        //获取目标对象
        GameObject go = ab.LoadAsset<GameObject>("Fangkuai");
        //实例化对象
        Instantiate(go);
    }
}

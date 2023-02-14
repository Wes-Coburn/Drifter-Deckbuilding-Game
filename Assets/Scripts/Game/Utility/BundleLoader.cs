using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    private static GameObject initObject;
    public static void BuildManagers(Action onComplete)
    {
        if (initObject == null) initObject = new("BundleLoader_InitGameObject");
        initObject.AddComponent<BundleLoader>();
        initObject.GetComponent<BundleLoader>().StartCoroutine(BuildManagers_Numerator(onComplete));
    }
    private static IEnumerator BuildManagers_Numerator(Action onComplete)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "managers"));
        yield return bundleLoadRequest;

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        foreach (string asset in myLoadedAssetBundle.GetAllAssetNames())
        {
            var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(asset);
            yield return assetLoadRequest;

            GameObject prefab = assetLoadRequest.asset as GameObject;
            Instantiate(prefab);
        }

        myLoadedAssetBundle.Unload(false);
        onComplete?.Invoke();
    }
}

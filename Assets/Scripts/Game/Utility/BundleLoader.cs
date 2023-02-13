using System.Collections;
using System.IO;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    private static GameObject initObject;
    public static void BuildBundle(string bundle, System.Action onComplete)
    {
        if (initObject == null) initObject = new("BundleLoader_InitGameObject");
        initObject.AddComponent<BundleLoader>();
        initObject.GetComponent<BundleLoader>().StartCoroutine(LoadBundle(bundle, onComplete));
    }
    private static IEnumerator LoadBundle(string bundle, System.Action onComplete)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundle));
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
        onComplete();
    }
}

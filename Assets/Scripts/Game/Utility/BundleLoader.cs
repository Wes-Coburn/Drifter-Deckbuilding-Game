using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class BundleLoader : MonoBehaviour
{
    public static BundleLoader Instance;

    private static GameObject initObject;

    private static void InitIfNeeded()
    {
        if (initObject == null)
        {
            initObject = new("BundleLoader");
            Instance = initObject.AddComponent<BundleLoader>();
        }
    }

    private static string GetAssetPath(string path) =>
        Path.Combine(Application.streamingAssetsPath, path);

    public static void BuildManagers(Action onComplete)
    {
        InitIfNeeded();
        Instance.StartCoroutine(BuildManagers_Numerator(onComplete));
    }
    private static IEnumerator BuildManagers_Numerator(Action onComplete)
    {
        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(GetAssetPath("managers"));
        yield return bundleLoadRequest;

        var myLoadedAssetBundle = bundleLoadRequest.assetBundle;
        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        GameObject gameManager = null;
        foreach (string asset in myLoadedAssetBundle.GetAllAssetNames())
        {
            var assetLoadRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>(asset);
            yield return assetLoadRequest;

            var prefab = assetLoadRequest.asset as GameObject;
            if (prefab.TryGetComponent(out GameManager _)) gameManager = prefab;
            else Instantiate(prefab);
        }

        Instantiate(gameManager); // GameManager.Start() must be called LAST
        myLoadedAssetBundle.Unload(false);
        onComplete?.Invoke();
    }

    /*
    public static AssetBundleCreateRequest RequestEffectBundle() =>
        AssetBundle.LoadFromFileAsync(GetAssetPath("effects"));
    
    public static AssetBundleCreateRequest RequestAbilityBundle() =>
        AssetBundle.LoadFromFileAsync(GetAssetPath("abilities"));
    */
}

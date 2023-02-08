using System.Collections;
using System.IO;
using UnityEngine;

public class ManagerSceneHandler : MonoBehaviour
{
    /*
    void Start()
    {
        var managers = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "managers"));

        if (managers == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            return;
        }

        foreach (string asset in managers.GetAllAssetNames())
        {
            var prefab = managers.LoadAsset<GameObject>(asset);
            Instantiate(prefab);
        }
    }
    */
    
    IEnumerator Start()
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

        SceneLoader.LoadScene(SceneLoader.Scene.TitleScene, false, false);
    }
}


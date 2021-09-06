using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StudioSplashFader : MonoBehaviour
{
    private CanvasGroup splashGroup;
    [SerializeField] private Image logoImage;
    //private float logoScale;

    private void Start()
    {
        splashGroup = GetComponentInParent<CanvasGroup>();
        //logoScale = logoImage.transform.localScale.x;
        StartCoroutine(FadeSplashNumerator());
    }
    private IEnumerator FadeSplashNumerator()
    {
        yield return new WaitForSeconds(2f); // NORMALLY !<2>!
        UIManager.Instance.SetSceneFader(false);
        GetComponent<SoundPlayer>().PlaySound(0);
        yield return new WaitForSeconds(4f);
        while (splashGroup.alpha > 0)
        {
            splashGroup.alpha -= 0.02f;
            //if (logoScale > 0) logoScale -= 0.01f;
            //logoImage.transform.localScale = new Vector2(logoScale, logoScale);
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}

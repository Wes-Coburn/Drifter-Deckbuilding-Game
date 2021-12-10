using UnityEngine;

public class BETAWelcomePopupDisplay : MonoBehaviour
{
    public void SurveyButton_OnClick()
    {
        Application.OpenURL("https://www.surveymonkey.com/r/R3HCNMK");
    }

    public void WesbsiteButton_OnClick()
    {
        Application.OpenURL("https://www.drifterthegame.com");
    }
}

using UnityEngine;

public class BETAWelcomePopupDisplay : MonoBehaviour
{
    public void DiscordButton_OnClick()
    {
        Application.OpenURL("https://discord.com/invite/X49ju9VAEY");
    }

    public void WesbsiteButton_OnClick()
    {
        Application.OpenURL("https://www.drifterthegame.com");
    }
}

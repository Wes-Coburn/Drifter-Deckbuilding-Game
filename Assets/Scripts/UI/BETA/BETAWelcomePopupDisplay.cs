using UnityEngine;

public class BETAWelcomePopupDisplay : MonoBehaviour
{
    public void DiscordButton_OnClick() => WebButton("https://discord.com/invite/X49ju9VAEY");
    public void WesbsiteButton_OnClick() => WebButton("https://www.drifterthegame.com");

    private void WebButton(string url)
    {
        Managers.AN_MAN.CreateParticleSystem(gameObject, ParticleSystemHandler.ParticlesType.ButtonPress);
        Application.OpenURL(url);
    }
}

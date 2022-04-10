using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimeClockDisplay : MonoBehaviour
{
    [SerializeField] private GameObject clockHand;
    [SerializeField] private GameObject screenDimmer;

    [SerializeField] private GameObject hour1Text;
    [SerializeField] private GameObject hour2Text;
    [SerializeField] private GameObject hour3Text;
    [SerializeField] private GameObject hour4Text;

    [SerializeField] private Color activeHourColor;
    [SerializeField] private Color inActiveHourColor;
    [SerializeField] private Color restHourColor;

    private RectTransform handRect;
    private Image hand;
    private Image dimmer;
    private List<GameObject> hours;

    private Quaternion zRot;
    private float dimAlph;

    private AudioManager auMan;

    private void Awake()
    {
        handRect = clockHand.GetComponent<RectTransform>();
        hand = clockHand.GetComponent<Image>();
        dimmer = screenDimmer.GetComponent<Image>();
        hours = new List<GameObject>()
        {
            hour1Text, hour2Text, hour3Text, hour4Text
        };

        auMan = AudioManager.Instance;
    }

    public void SetClockValues(int newHour, bool isNewHour)
    {
        if (isNewHour) StartCoroutine(SetClockValuesNumerator(newHour));
        else
        {
            SetActiveHour(newHour);
            GetClockValues(newHour);
            clockHand.transform.rotation = zRot;
            Color dimColor = dimmer.color;
            dimColor.a = dimAlph;
            dimmer.color = dimColor;
        }
    }

    private IEnumerator SetClockValuesNumerator(int newHour)
    {
        int previousHour;
        if (newHour == 1) previousHour = 4;
        else previousHour = newHour - 1;

        // PREVIOUS HOUR
        SetActiveHour(previousHour);
        GetClockValues(previousHour);
        clockHand.transform.rotation = zRot;
        Color dimColor = dimmer.color;
        dimColor.a = dimAlph;
        dimmer.color = dimColor;

        ParticleSystemHandler psh = AnimationManager.Instance.CreateParticleSystem
            (clockHand.transform.parent.gameObject, ParticleSystemHandler.ParticlesType.Drag);
        Color previousColor = hand.color;
        hand.color = activeHourColor;

        auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, false, true);
        for (int i = 0; i < 90; i++)
        {
            handRect.Rotate(0, 0, -1);
            if (i % 5 == 0) auMan.StartStopSound("SFX_TimeLapse", null);
            yield return new WaitForSeconds(0.05f);
        }
        
        auMan.StartStopSound("SFX_SceneLoading", null, AudioManager.SoundType.SFX, true);
        psh.StopParticles();

        hand.color = previousColor;
        SetActiveHour(newHour);
        auMan.StartStopSound("SFX_TimeLapse", null);
        yield return new WaitForSeconds(0.3f);

        hand.color = activeHourColor;
        SetActiveHour(0);
        yield return new WaitForSeconds(0.3f);

        hand.color = previousColor;
        SetActiveHour(newHour);
        auMan.StartStopSound("SFX_TimeLapse", null);
        yield return new WaitForSeconds(0.3f);

        hand.color = activeHourColor;
        SetActiveHour(0);
        yield return new WaitForSeconds(0.3f);

        hand.color = previousColor;
        SetActiveHour(newHour);
        auMan.StartStopSound("SFX_TimeLapse", null);

        GetClockValues(newHour); // TESTING
        if (newHour == 1)
        {
            while (dimmer.color.a > dimAlph)
            {
                dimColor.a -= 0.01f;
                dimmer.color = dimColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            while (dimmer.color.a < dimAlph)
            {
                dimColor.a += 0.01f;
                dimmer.color = dimColor;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }

    private void GetClockValues(int hour)
    {
        switch (hour)
        {
            case 1:
                zRot = Quaternion.Euler(0, 0, 45);
                dimAlph = 0;
                break;
            case 2:
                zRot = Quaternion.Euler(0, 0, -45);
                dimAlph = 0.1f;
                break;
            case 3:
                zRot = Quaternion.Euler(0, 0, -135);
                dimAlph = 0.5f;
                break;
            case 4:
                zRot = Quaternion.Euler(0, 0, -225);
                dimAlph = 0.7f;
                break;
            default:
                Debug.LogError("INVALID HOUR! <" + hour + ">");
                return;
        }
    }

    private void SetActiveHour(int hour)
    {
        GameObject activeHour;
        switch (hour)
        {
            case 0:
                activeHour = null;
                break;
            case 1:
                activeHour = hour1Text;
                break;
            case 2:
                activeHour = hour2Text;
                break;
            case 3:
                activeHour = hour3Text;
                break;
            case 4:
                activeHour = hour4Text;
                break;
            default:
                Debug.LogError("INVALID HOUR! <" + hour + ">");
                return;
        }

        foreach (GameObject go in hours)
        {
            Color color;
            if (go == activeHour)
            {
                if (hour == 4) color = restHourColor;
                else color = activeHourColor;
            }
            else color = inActiveHourColor;
            go.GetComponent<TextMeshProUGUI>().color = color;
        }
    }
}

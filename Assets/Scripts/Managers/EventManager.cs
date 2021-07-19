using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    /* SINGELTON_PATTERN */
    public static EventManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        delayedActions = new List<DelayedAction>();
        DelayedAction.CurrentAction = 0;
    }

    private class DelayedAction
    {
        public Action Action;
        public float Delay;
        public static int CurrentAction;
    }

    private List<DelayedAction> delayedActions;
    
    public void NewDelayedAction(Action action, float delay)
    {
        DelayedAction da = new DelayedAction
        {
            Action = action,
            Delay = delay
        };
        delayedActions.Add(da);
        if (delayedActions.Count == 1) NextDelayedAction();
    }

    private void NextDelayedAction()
    {
        //Debug.Log("ACTION # " + DelayedAction.CurrentAction + " / " + delayedActions.Count);
        if (DelayedAction.CurrentAction < delayedActions.Count)
        {
            StartCoroutine(ActionNumerator());
        }
        else
        {
            delayedActions.Clear();
            DelayedAction.CurrentAction = 0;
        }
    }

    IEnumerator ActionNumerator()
    {
        delayedActions[DelayedAction.CurrentAction].Action();
        yield return new WaitForSeconds(delayedActions[DelayedAction.CurrentAction++].Delay);
        NextDelayedAction();
    }
}

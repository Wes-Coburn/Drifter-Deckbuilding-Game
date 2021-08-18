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

        delayedActions = new List<DelayedAction>(); // STATIC
    }

    public class DelayedAction
    {
        public Action Action;
        public float Delay;
        public static int CurrentAction;
    }
    
    private static List<DelayedAction> delayedActions;
    
    public DelayedAction NewDelayedAction(Action action, float delay)
    {
        DelayedAction da = new DelayedAction
        {
            Action = action,
            Delay = delay
        };

        delayedActions.Add(da);
        if (delayedActions.Count == 1)
        {
            DelayedAction.CurrentAction = 0;
            NextDelayedAction();
        }
        return da;
    }

    private void NextDelayedAction()
    {
        if (DelayedAction.CurrentAction < delayedActions.Count)
        {
            Debug.Log("ACTION # " + (DelayedAction.CurrentAction + 1) + " / " + delayedActions.Count);
            StartCoroutine(ActionNumerator());
        }
        else ClearDelayedActions();
    }

    public void ClearDelayedActions()
    {
        delayedActions.Clear();
        DelayedAction.CurrentAction = 0;
    }

    IEnumerator ActionNumerator()
    {
        yield return new WaitForSeconds(delayedActions[DelayedAction.CurrentAction].Delay);
        delayedActions[DelayedAction.CurrentAction++].Action();
        NextDelayedAction();
    }
}

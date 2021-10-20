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
        isPaused = false;
    }

    private static List<DelayedAction> delayedActions;
    private Coroutine currentActionRoutine;
    private bool isPaused;

    public class DelayedAction
    {
        public Action Action;
        public float Delay;
        public static int CurrentAction;
    }
    
    public void NewDelayedAction(Action action, float delay, bool resolveNext = false)
    {
        DelayedAction da = new DelayedAction
        {
            Action = action,
            Delay = delay
        };

        if (resolveNext) delayedActions.Insert(DelayedAction.CurrentAction, da); // TESTING
        else delayedActions.Add(da);

        if (delayedActions.Count == 1)
        {
            DelayedAction.CurrentAction = 0;
            NextDelayedAction();
        }
    }

    private void NextDelayedAction()
    {
        if (DelayedAction.CurrentAction < delayedActions.Count)
        {
            if (isPaused) return; // TESTING
            //Debug.Log("ACTION # " + (DelayedAction.CurrentAction + 1) + " / " + delayedActions.Count);
            currentActionRoutine = StartCoroutine(ActionNumerator());
        }
        else ClearDelayedActions();
    }

    public void PauseDelayedActions(bool isPaused) // TESTING
    {
        bool isResuming = false;
        if (this.isPaused && !isPaused) isResuming = true;
        this.isPaused = isPaused;
        if (isResuming) NextDelayedAction();
    }

    public void ClearDelayedActions()
    {
        delayedActions.Clear();
        DelayedAction.CurrentAction = 0;
        if (currentActionRoutine != null) 
            StopCoroutine(currentActionRoutine);
    }

    IEnumerator ActionNumerator()
    {
        yield return new WaitForSeconds(delayedActions[DelayedAction.CurrentAction].Delay);
        delayedActions[DelayedAction.CurrentAction++].Action();
        NextDelayedAction();
    }
}

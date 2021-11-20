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

    public bool ActionsDelayed
    {
        get
        {
            if (delayedActions.Count > 0) return true;
            else return false;
        }
    }

    public class DelayedAction
    {
        public Action Action;
        public float Delay;
        public static int CurrentAction;
    }
    
    public void NewDelayedAction(Action action,
        float delay, bool resolveNext = false)
    {
        DelayedAction da = new DelayedAction
        {
            Action = action,
            Delay = delay,
        };

        if (resolveNext)
            delayedActions.Insert(DelayedAction.CurrentAction, da);
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
            //Debug.Log("ACTION # " + (DelayedAction.CurrentAction + 1) + " / " + delayedActions.Count);
            if (isPaused) return;
            currentActionRoutine = StartCoroutine(ActionNumerator());
        }
        else ClearDelayedActions();
    }

    public void PauseDelayedActions(bool isPaused)
    {
        if (this.isPaused == isPaused) return;
        bool isResuming = false;
        if (this.isPaused && !isPaused &&
            currentActionRoutine == null) isResuming = true;
        this.isPaused = isPaused;
        if (isResuming) NextDelayedAction();
    }

    public void ClearDelayedActions()
    {
        if (currentActionRoutine != null)
        {
            StopCoroutine(currentActionRoutine);
            currentActionRoutine = null;
        }
        delayedActions.Clear();
        DelayedAction.CurrentAction = 0;
    }

    IEnumerator ActionNumerator()
    {
        yield return new WaitForSeconds(delayedActions[DelayedAction.CurrentAction].Delay);
        delayedActions[DelayedAction.CurrentAction++].Action();
        currentActionRoutine = null;
        NextDelayedAction();
    }
}

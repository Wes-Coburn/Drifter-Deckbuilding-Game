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
        delayedActions_priority = new List<DelayedAction>();
        isPaused = false;
    }

    private static List<DelayedAction> delayedActions;
    private static List<DelayedAction> delayedActions_priority;
    private Coroutine currentActionRoutine;
    private bool isPaused;

    public bool ActionsDelayed
    {
        get
        {
            if (delayedActions.Count > 0 || isPaused) return true;
            else return false;
        }
    }

    public bool ActionsPaused => isPaused;

    public class DelayedAction
    {
        public Action Action;
        public float Delay;
    }

    /******
     * *****
     * ****** RESET_EVENT_MANAGER
     * *****
     *****/
    public void Reset_EventManager()
    {
        ClearDelayedActions();
        PauseDelayedActions(false);
    }

    /******
     * *****
     * ****** NEW_DELAYED_ACTION
     * *****
     *****/
    public void NewDelayedAction(Action action, float delay, bool resolveNext = false, bool priorityAction = false)
    {
        DelayedAction da = new DelayedAction
        {
            Action = action,
            Delay = delay,
        };

        if (priorityAction)
        {
            if (resolveNext) delayedActions_priority.Insert(0, da);
            else delayedActions_priority.Add(da);
        }
        else
        {
            if (resolveNext) delayedActions.Insert(0, da);
            else delayedActions.Add(da);
        }

        if (currentActionRoutine == null && !isPaused) NextDelayedAction();
    }

    private void NextDelayedAction()
    {
        if (isPaused) return;

        if (currentActionRoutine != null)
        {
            Debug.LogError("ACTION ROUTINE IS NOT NULL!");
            return;
        }

        if (delayedActions.Count > 0 || delayedActions_priority.Count > 0)
            currentActionRoutine = StartCoroutine(ActionNumerator());

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

        UIManager.Instance.UpdateEndTurnButton(!isPaused);
    }

    public void ClearDelayedActions()
    {
        if (currentActionRoutine != null)
        {
            StopCoroutine(currentActionRoutine);
            currentActionRoutine = null;
        }

        delayedActions.Clear();
        delayedActions_priority.Clear();
    }

    IEnumerator ActionNumerator()
    {
        DelayedAction da;
        if (delayedActions_priority.Count > 0)
        {
            da = delayedActions_priority[0];
            delayedActions_priority.RemoveAt(0);
        }
        else
        {
            da = delayedActions[0];
            delayedActions.RemoveAt(0);
        }

        yield return new WaitForSeconds(da.Delay);
        da.Action();

        currentActionRoutine = null;
        NextDelayedAction();
    }
}

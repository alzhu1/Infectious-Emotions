using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour {
    public static EventBus instance = null;

    public event Action OnLevelStart = delegate {};
    public event Action OnLevelComplete = delegate {};
    public event Action OnLevelRestart = delegate {};

    public event Action OnStep = delegate {};
    public event Action OnArrowShoot = delegate {};
    public event Action<bool> OnArrowHit = delegate {};

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            instance.TriggerOnLevelStart();
            Destroy(gameObject);
            return;
        }
    }

    public void TriggerOnLevelStart() {
        OnLevelStart?.Invoke();
    }

    public void TriggerOnLevelComplete() {
        OnLevelComplete?.Invoke();
    }

    public void TriggerOnLevelRestart() {
        OnLevelRestart?.Invoke();
    }

    public void TriggerOnStep() {
        OnStep?.Invoke();
    }

    public void TriggerOnArrowShoot() {
        OnArrowShoot?.Invoke();
    }

    public void TriggerOnArrowHit(bool hitWall) {
        OnArrowHit?.Invoke(hitWall);
    }
}

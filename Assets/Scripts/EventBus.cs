using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventBus : MonoBehaviour {
    public static EventBus instance = null;

    public event Action OnLevelStart = delegate {};
    public event Action OnLevelComplete = delegate {};

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
}

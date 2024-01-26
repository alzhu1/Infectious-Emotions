using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title : MonoBehaviour {
    private bool loadNextLevel = false;

    void Update() {
        if (!loadNextLevel && Input.GetButtonDown("Start")) {
            loadNextLevel = true;
            EventBus.instance.TriggerOnLevelComplete();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    private static LevelManager instance = null;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Start() {
        EventBus.instance.OnLevelComplete += ReceiveLevelCompleteEvent;
    }

    void OnDestroy() {
        EventBus.instance.OnLevelComplete -= ReceiveLevelCompleteEvent;
    }

    void ReceiveLevelCompleteEvent() {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}

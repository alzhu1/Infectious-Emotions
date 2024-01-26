using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    private static UIManager instance = null;

    [SerializeField] private float transitionTime;
    
    private Text[] texts;
    private Image transitionImage;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }

        texts = GetComponentsInChildren<Text>();
        transitionImage = GetComponentInChildren<Image>();
    }

    void Start() {
        EventBus.instance.OnLevelStart += ReceiveLevelStartEvent;
        EventBus.instance.OnLevelComplete += ReceiveLevelCompleteEvent;
        EventBus.instance.OnLevelRestart += ReceiveLevelRestartEvent;
    }

    void OnDestroy() {
        EventBus.instance.OnLevelStart -= ReceiveLevelStartEvent;
        EventBus.instance.OnLevelComplete -= ReceiveLevelCompleteEvent;
        EventBus.instance.OnLevelRestart -= ReceiveLevelRestartEvent;
    }

    void ReceiveLevelStartEvent() {
        StartCoroutine(Transition(false, false));
    }

    void ReceiveLevelCompleteEvent() {
        StartCoroutine(Transition(true, false));
    }

    void ReceiveLevelRestartEvent() {
        StartCoroutine(Transition(false, true));
    }

    IEnumerator Transition(bool levelComplete, bool levelRestart) {
        if (LevelManager.instance.IsTitle()) {
            foreach (Text text in texts) {
                text.gameObject.SetActive(true);
            }
        }

        Color startColor = (levelComplete || levelRestart) ? Color.clear : Color.black;
        Color endColor = (levelComplete || levelRestart) ? Color.black : Color.clear;

        float t = 0;
        while (t < transitionTime) {
            transitionImage.color = Color.Lerp(startColor, endColor, t / transitionTime);
            yield return null;
            t += Time.deltaTime;
        }

        transitionImage.color = endColor;

        if (levelRestart) {
            LevelManager.instance.ReloadLevel();
        } else if (levelComplete) {
            if (LevelManager.instance.IsTitle()) {
                foreach (Text text in texts) {
                    text.gameObject.SetActive(false);
                }
            }

            LevelManager.instance.LoadNextLevel();
        }
    }
}

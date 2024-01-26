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
    }

    void OnDestroy() {
        EventBus.instance.OnLevelStart -= ReceiveLevelStartEvent;
        EventBus.instance.OnLevelComplete -= ReceiveLevelCompleteEvent;
    }

    void ReceiveLevelStartEvent() {
        StartCoroutine(Transition(false));
    }

    void ReceiveLevelCompleteEvent() {
        StartCoroutine(Transition(true));
    }

    IEnumerator Transition(bool levelComplete) {
        if (LevelManager.instance.IsTitle()) {
            foreach (Text text in texts) {
                text.gameObject.SetActive(true);
            }
        }

        Color startColor = levelComplete ? Color.clear : Color.black;
        Color endColor = levelComplete ? Color.black : Color.clear;

        float t = 0;
        while (t < transitionTime) {
            transitionImage.color = Color.Lerp(startColor, endColor, t / transitionTime);
            yield return null;
            t += Time.deltaTime;
        }

        transitionImage.color = endColor;

        if (levelComplete) {
            if (LevelManager.instance.IsTitle()) {
                foreach (Text text in texts) {
                    text.gameObject.SetActive(false);
                }
            }

            LevelManager.instance.LoadNextLevel();
        }
    }
}

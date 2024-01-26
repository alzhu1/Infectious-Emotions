using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour {
    public static LevelManager instance = null;

    void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
    }

    public bool IsTitle() {
        return SceneManager.GetActiveScene().buildIndex == 0;
    }

    public void LoadNextLevel() {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene((buildIndex + 1) % SceneManager.sceneCountInBuildSettings);
    }
}

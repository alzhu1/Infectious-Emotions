using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {
    public static TilemapManager instance = null;

    [SerializeField] private Tilemap blockmap;

    void Awake() {
        if (instance == null) {
            instance = this;
            // TODO: Do I want DontDestroyOnLoad here?
        } else {
            Destroy(gameObject);
            return;
        }
    }

    void Update() {
        
    }

    public bool IsTileBlocked(Vector3Int pos) {
        return blockmap.GetTile(pos) != null;
    }
}

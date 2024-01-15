using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {
    public static TilemapManager instance = null;

    [SerializeField] private Tilemap blockmap;
    [SerializeField] private Tilemap switchmap;

    private HashSet<Vector3Int> switchPositions;

    void Awake() {
        if (instance == null) {
            instance = this;
            // TODO: Do I want DontDestroyOnLoad here?
        } else {
            Destroy(gameObject);
            return;
        }

        switchPositions = new HashSet<Vector3Int>();
        foreach (Vector3Int switchPos in switchmap.cellBounds.allPositionsWithin) {
            Tile tile = switchmap.GetTile<Tile>(switchPos);
            if (tile != null) {
                switchmap.SetTileFlags(switchPos, TileFlags.None);
                switchPositions.Add(switchPos);
            }
        }
    }

    public bool IsTileBlocked(Vector3Int pos) {
        return blockmap.GetTile(pos) != null;
    }

    public void UpdateSwitchTiles(Dictionary<Vector3Int, Unit> unitPositions) {
        bool allSwitchesPressed = true;
        foreach (Vector3Int switchPos in switchPositions) {
            bool switchPressed = unitPositions.ContainsKey(switchPos);
            allSwitchesPressed &= switchPressed;
            switchmap.SetColor(switchPos, switchPressed ? Color.green : Color.white);
        }

        if (allSwitchesPressed) {
            EventBus.instance.TriggerOnLevelComplete();
        }
    }
}

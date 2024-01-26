using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour {
    public static TilemapManager instance = null;

    [SerializeField] private Tilemap blockmap;
    [SerializeField] private Tilemap switchmap;

    [SerializeField] private Tile arrowBlockmapPass;

    private HashSet<Vector3Int> switchPositions;

    void Awake() {
        switchPositions = new HashSet<Vector3Int>();
        foreach (Vector3Int switchPos in switchmap.cellBounds.allPositionsWithin) {
            Tile tile = switchmap.GetTile<Tile>(switchPos);
            if (tile != null) {
                switchmap.SetTileFlags(switchPos, TileFlags.None);
                switchPositions.Add(switchPos);
            }
        }
    }

    public bool IsTileBlocked(Vector3Int pos, bool attacking = false) {
        TileBase tile = blockmap.GetTile(pos);
        if (attacking) {
            return tile != null && tile != arrowBlockmapPass;
        }
        return tile != null;
    }

    public bool UpdateSwitchTiles(Dictionary<Vector3Int, Unit> unitPositions) {
        bool allSwitchesPressed = true;
        foreach (Vector3Int switchPos in switchPositions) {
            bool switchPressed = unitPositions.ContainsKey(switchPos);
            allSwitchesPressed &= switchPressed;
            switchmap.SetColor(switchPos, switchPressed ? Color.green : Color.white);
        }

        if (allSwitchesPressed) {
            EventBus.instance.TriggerOnLevelComplete();
        }

        return allSwitchesPressed;
    }
}

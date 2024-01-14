using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private UnitType type;

    public UnitType Type {
        get { return type; }
    }

    private bool horizontalPressed;
    private bool verticalPressed;

    void Start() {
        
    }

    void Update() {

    }

    public Vector3Int GetTilePos() {
        return Vector3Int.FloorToInt(transform.position);
    }

    public Vector3Int GetNextPos(Vector3Int delta) {
        switch (type) {
            case UnitType.PLAYER_MAIN:
            case UnitType.PLAYER_LOVE: {
                // Love should mimic player movement
                return GetTilePos() + delta;
            }

            case UnitType.PLAYER_HATE: {
                // Hate should be the opposite
                return GetTilePos() - delta;
            }

            case UnitType.PLAYER_ENVY: {
                // TODO: Fill this in, envy should have this player follow the mainPlayer
                Debug.Log("ENVY not added");
                break;
            }

            default: {
                Debug.LogWarning($"Unexpected unit type: {type}");
                break;
            }
        }

        return GetTilePos();
    }

    // public void Move(Vector3Int delta) {
    //     transform.position = GetNextPos(delta);
    // }

    public void MoveTo(Vector3Int location) {
        transform.position = location;
    }

    public void HandleAttacked() {
        type = UnitType.PLAYER_LOVE;
    }
}

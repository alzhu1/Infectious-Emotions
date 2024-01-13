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

    public void Move(Vector3 delta) {
        transform.position += delta;
    }

    public void HandleAttacked() {
        type = UnitType.PLAYER_LOVE;
    }
}

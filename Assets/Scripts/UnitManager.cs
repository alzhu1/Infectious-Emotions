using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    private Dictionary<Vector3Int, Unit> playerUnits;
    private Dictionary<Vector3Int, Unit> npcUnits;
    private Unit mainPlayer;

    private bool horizontalPressed;
    private bool verticalPressed;
    private bool attacked;

    void Awake() {
        playerUnits = new Dictionary<Vector3Int, Unit>();
        npcUnits = new Dictionary<Vector3Int, Unit>();

        Unit[] units = GetComponentsInChildren<Unit>();
        foreach (Unit unit in units) {
            Vector3Int pos = unit.GetTilePos();
            
            if (unit.Type == UnitType.PLAYER_MAIN) {
                mainPlayer = unit;
            } else if (unit.Type.IsPlayerControlled()) {
                playerUnits.Add(pos, unit);
            } else {
                npcUnits.Add(pos, unit);
            }
        }
    }

    void Update() {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (!attacked && Input.GetButton("AttackMode")) {
            if (horizontal > 0) { AttackWithMainPlayer(Vector3Int.right); }
            else if (horizontal < 0) { AttackWithMainPlayer(Vector3Int.left); }
            else if (vertical > 0) { AttackWithMainPlayer(Vector3Int.up); }
            else if (vertical < 0) { AttackWithMainPlayer(Vector3Int.down); }

            horizontalPressed = true;
            verticalPressed = true;
        } else {
            if (horizontal != 0 && !horizontalPressed) {
                horizontalPressed = true;
                MovePlayerUnits(new Vector3(Mathf.RoundToInt(horizontal), 0, 0));
            } else if (horizontal == 0) {
                horizontalPressed = false;
            }


            if (vertical != 0 && !verticalPressed) {
                verticalPressed = true;
                MovePlayerUnits(new Vector3(0, Mathf.RoundToInt(vertical), 0));
            } else if (vertical == 0) {
                verticalPressed = false;
            }
        }

        if (!Input.GetButton("AttackMode")) {
            attacked = false;
        }
    }

    void MovePlayerUnits(Vector3 delta) {
        List<Unit> playerUnitsToMove = new List<Unit>(playerUnits.Values);

        // Main player should always move correctly
        mainPlayer.Move(delta);

        foreach (Unit playerUnit in playerUnitsToMove) {
            playerUnits.Remove(playerUnit.GetTilePos());

            switch (playerUnit.Type) {
                case UnitType.PLAYER_LOVE: {
                    // Love should mimic player movement
                    playerUnit.Move(delta);
                    break;
                }

                case UnitType.PLAYER_HATE: {
                    // Hate should be the opposite
                    playerUnit.Move(-delta);
                    break;
                }

                case UnitType.PLAYER_ENVY: {
                    // TODO: Fill this in, envy should have this player follow the mainPlayer
                    Debug.Log("ENVY not added");
                    break;
                }

                default: {
                    Debug.LogWarning($"Unexpected unit type: {playerUnit.Type}");
                    break;
                }
            }

            playerUnits.Add(playerUnit.GetTilePos(), playerUnit);
        }
    }

    void AttackWithMainPlayer(Vector3Int direction) {
        Debug.Log($"Main player at {mainPlayer.GetTilePos()}, attacking direction {direction}");

        Vector3Int attackedPos = mainPlayer.GetTilePos() + direction;
        if (playerUnits.ContainsKey(attackedPos)) {
            Unit attackedUnit = playerUnits[attackedPos];
            playerUnits.Remove(attackedPos);
            Destroy(attackedUnit.gameObject);
        } else if (npcUnits.ContainsKey(attackedPos)) {
            Unit attackedUnit = npcUnits[attackedPos];
            npcUnits.Remove(attackedPos);
            playerUnits.Add(attackedPos, attackedUnit);

            attackedUnit.HandleAttacked();
        }

        attacked = true;
    }
}

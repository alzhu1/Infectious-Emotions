using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour {

    [SerializeField] private float unitMoveTime = 1f;
    [SerializeField] private int attackRange = 2;

    // Object pooled arrow
    [SerializeField] private Arrow arrow;

    private TilemapManager tilemapManager;

    private Dictionary<Vector3Int, Unit> playerUnits;
    private Dictionary<Vector3Int, Unit> npcUnits;
    private Unit mainPlayer;

    private bool horizontalPressed;
    private bool verticalPressed;

    private bool paused;
    private bool levelComplete;

    void Awake() {
        tilemapManager = FindObjectOfType<TilemapManager>();

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
        if (paused || levelComplete) {
            return;
        }

        if (Input.GetButtonDown("Restart")) {
            levelComplete = true;
            EventBus.instance.TriggerOnLevelRestart();
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetButton("AttackMode")) {
            if (horizontal > 0) { AttackWithMainPlayer(Vector3Int.right); }
            else if (horizontal < 0) { AttackWithMainPlayer(Vector3Int.left); }
            else if (vertical > 0) { AttackWithMainPlayer(Vector3Int.up); }
            else if (vertical < 0) { AttackWithMainPlayer(Vector3Int.down); }

            horizontalPressed = true;
            verticalPressed = true;
        } else {
            if (horizontal != 0 && !horizontalPressed) {
                horizontalPressed = true;
                MovePlayerUnits(new Vector3Int(Mathf.RoundToInt(horizontal), 0, 0));
            } else if (horizontal == 0) {
                horizontalPressed = false;
            }

            if (!horizontalPressed && vertical != 0 && !verticalPressed) {
                verticalPressed = true;
                MovePlayerUnits(new Vector3Int(0, Mathf.RoundToInt(vertical), 0));
            } else if (vertical == 0) {
                verticalPressed = false;
            }
        }
    }

    void MovePlayerUnits(Vector3Int delta) {
        List<Unit> unitsToMove = new List<Unit>(playerUnits.Values)
        {
            mainPlayer
        };
        Dictionary<Vector3Int, Unit> finalPositions = new Dictionary<Vector3Int, Unit>();
        int totalUnitCount = unitsToMove.Count;

        // Algorithm to move units
        HashSet<Unit> tempUnits = new HashSet<Unit>();
        for (int i = 0; i < unitsToMove.Count; i++) {
            Unit unit = unitsToMove[i];
            Vector3Int nextPos = unit.GetNextPos(delta);

            if (tilemapManager.IsTileBlocked(nextPos) ||
                finalPositions.ContainsKey(nextPos) ||
                npcUnits.ContainsKey(nextPos)) {
                // Blocked or occupied means this person cannot move
                finalPositions.Add(unit.GetTilePos(), unit);
                unitsToMove.RemoveAt(i);
                i = -1;
                tempUnits.Clear();
                continue;
            }

            // Otherwise, mark unit as temp
            tempUnits.Add(unit);
        }

        foreach (Unit tempUnit in tempUnits) {
            finalPositions.Add(tempUnit.GetNextPos(delta), tempUnit);
        }

        // Just to debug
        if (finalPositions.Count != totalUnitCount) {
            Debug.LogWarning($"Final position count: {finalPositions.Count}, total unit count: {totalUnitCount}");
        }

        StartCoroutine(ProcessMoveAnimations(finalPositions));
    }

    void AttackWithMainPlayer(Vector3Int direction) {
        Debug.Log($"Main player at {mainPlayer.GetTilePos()}, attacking direction {direction}");

        StartCoroutine(ProcessAttack(direction));
    }

    // https://www.reddit.com/r/Unity3D/comments/11imces/wait_for_all_coroutines_to_finish/
    IEnumerator ProcessMoveAnimations(Dictionary<Vector3Int, Unit> finalPositions) {
        paused = true;
        int coroutineTally = 0;

        EventBus.instance.TriggerOnStep();

        foreach (var entry in finalPositions) {
            Unit unit = entry.Value;
            playerUnits.Remove(unit.GetTilePos());
            StartCoroutine(RunAwaitedCoroutine(unit.MoveTo(entry.Key, unitMoveTime)));
        }

        while (coroutineTally > 0) {
            yield return null;
        }

        foreach (Unit unit in finalPositions.Values) {
            if (unit != mainPlayer) {
                playerUnits.Add(unit.GetTilePos(), unit);
            }
        }
        levelComplete = tilemapManager.UpdateSwitchTiles(finalPositions);

        paused = false;

        IEnumerator RunAwaitedCoroutine(IEnumerator coroutine) {
            coroutineTally++;
            yield return StartCoroutine(coroutine);
            coroutineTally--;
        }
    }

    IEnumerator ProcessAttack(Vector3Int direction) {
        paused = true;
        yield return null;

        Vector3Int attackedPos = mainPlayer.GetTilePos() + direction;
        int range = 0;

        GameObject toDestroy = null;

        bool hitWall = false;
        while (range++ < attackRange) {
            if (tilemapManager.IsTileBlocked(attackedPos, true)) {
                Debug.Log($"Hitting wall at {attackedPos}");
                hitWall = true;
                break;
            }

            // Disable killing player units
            if (playerUnits.ContainsKey(attackedPos)) {
                // Unit attackedUnit = playerUnits[attackedPos];
                // playerUnits.Remove(attackedPos);
                // toDestroy = attackedUnit.gameObject;
                hitWall = true;
                break;
            }

            if (npcUnits.ContainsKey(attackedPos)) {
                Unit attackedUnit = npcUnits[attackedPos];
                npcUnits.Remove(attackedPos);
                playerUnits.Add(attackedPos, attackedUnit);
                attackedUnit.HandleAttacked();
                break;
            }

            attackedPos += direction;
        }

        EventBus.instance.TriggerOnArrowShoot();

        // Need to animate arrow going from mainPlayer.GetTilePos() to attackedPos
        yield return StartCoroutine(arrow.Travel(mainPlayer.GetTilePos(), direction, attackRange, range));

        if (range <= attackRange) {
            EventBus.instance.TriggerOnArrowHit(hitWall);
        }

        if (toDestroy != null) {
            Destroy(toDestroy);
        }

        paused = false;
    }
}

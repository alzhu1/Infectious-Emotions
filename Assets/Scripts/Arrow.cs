using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    private readonly Color START_COLOR = Color.white;
    private readonly Color END_COLOR = new Color(1, 1, 1, 0);
    private readonly Vector3 HALF_COORDS = new Vector3(0.5f, 0.5f);

    [SerializeField] private float timePerTile;

    private SpriteRenderer sr;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    public IEnumerator Travel(Vector3Int startPos, Vector3Int direction, int attackRange, int moveRange) {
        transform.position = startPos + HALF_COORDS;
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.z = Vector3.SignedAngle(Vector3Int.right, direction, Vector3Int.forward);
        transform.eulerAngles = eulerAngles;

        sr.color = START_COLOR;
        yield return null;

        int range = moveRange > attackRange ? attackRange : moveRange;
        Vector3Int endPos = startPos + (direction * range);

        float totalTime = timePerTile * range;
        float t = 0;
        while (t < totalTime) {
            transform.position = Vector3.Lerp(startPos, endPos, t / totalTime) + HALF_COORDS;
            yield return null;
            t += Time.deltaTime;
        }

        sr.color = END_COLOR;
    }
}

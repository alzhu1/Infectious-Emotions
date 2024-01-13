using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    [SerializeField] private UnitType type;

    private bool horizontalPressed;
    private bool verticalPressed;

    void Start() {
        
    }

    void Update() {
        // TODO: Decide if NPCs need their own actions, move accordingly
        if (!type.IsPlayerControlled()) {
            return;
        }

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal != 0 && !horizontalPressed) {
            horizontalPressed = true;
            transform.position += new Vector3(Mathf.RoundToInt(horizontal), 0, 0);
        } else if (horizontal == 0) {
            horizontalPressed = false;
        }


        if (vertical != 0 && !verticalPressed) {
            verticalPressed = true;
            transform.position += new Vector3(0, Mathf.RoundToInt(vertical), 0);
        } else if (vertical == 0) {
            verticalPressed = false;
        }
    }
}

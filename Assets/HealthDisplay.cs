using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthDisplay : MonoBehaviour {
    //TextMesh shield = Ge
    public TextMesh shieldText;

    // Use this for initialization
    void Start () {
        shieldText.text = "43";	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeShield(int shield) {
        shieldText.text = shield.ToString();
    }
}

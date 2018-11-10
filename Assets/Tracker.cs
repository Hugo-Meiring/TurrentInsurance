using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Vuforia;

public class Tracker : MonoBehaviour {

    public bool hasPlayedTurn = false;
    public bool isPlayerOnesTurn = true;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        StateManager sm = TrackerManager.Instance.GetStateManager();

        IEnumerable<TrackableBehaviour> tbs = sm.GetActiveTrackableBehaviours();
        //Debug.Log("Size of TBS: " + tbs.GetEnumerator.si)

        if (!hasPlayedTurn) {
            foreach (TrackableBehaviour tb in tbs)
            {
                string tbName = tb.TrackableName;

                if (tb.GetType().Equals(typeof(Vuforia.ImageTargetBehaviour)))
                {
                    ImageTarget it = tb.Trackable as ImageTarget;

                    switch (tbName) {
                        case "Astronaut_scaled":
                        case "Fissure_scaled":
                        case "Drone_scaled":
                        case "Oxygen_scaled":
                            hasPlayedTurn = true;
                            isPlayerOnesTurn = !isPlayerOnesTurn;
                            break;
                        default:
                            break;
                    }

                    //TODO deal damage
                }
            }
        }
	}
}

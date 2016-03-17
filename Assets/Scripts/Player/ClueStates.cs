using UnityEngine;
using System.Collections;

public class ClueStates : MonoBehaviour {
    private ClueState clueState;
	// Use this for initialization
	void Start () {
        clueState = ClueState.None;
	}
	
	// Update is called once per frame
	public void SetClueState (ClueState clue) {
        clueState = clue;
	}

    public ClueState GetClueState() {
        return clueState;
    }
}

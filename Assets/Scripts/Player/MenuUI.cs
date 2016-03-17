using UnityEngine;
using System.Collections;

/**
 * Controllers can call the public functions to control menu items:
 * e.g. ToggleMenu(); HighlightItem(GameObject menuItem); SelectItem(GameObject menuItem)
 * This UI is attached to the player prefab because it is easier to access player's attributes this way.
 */ 
public class MenuUI : MonoBehaviour
{
    public bool isActive = false;
    public Transform playerMenuTransform;
    private const string EXIT = "Exit";
    private const string CLUE = "Clue";
    private const string TOOL = "Tool";
    private const string KEY = "Key";
    private PlayerMenu playerMenu;
    private ClueStates clueStatesScript;
    // Use this for initialization
    void Awake()
    {
        // Initialize all material color to white.
        GameObject[] menuItems = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject menuItem in menuItems)
        {
            MeshRenderer mesh = menuItem.transform.GetComponentInChildren<MeshRenderer>();
            mesh.material.color = Color.white;
        }
        playerMenu = playerMenuTransform.GetComponent<PlayerMenu>();
        clueStatesScript = GetComponent<ClueStates>();
    }

    public void clearSelection() {
        GameObject[] menuItems = GameObject.FindGameObjectsWithTag("UI");
        foreach (GameObject menuItem in menuItems)
        {
            MeshRenderer mesh = menuItem.transform.GetComponentInChildren<MeshRenderer>();
            mesh.material.color = Color.white;
        }
    }

    public void displayClue(ClueState clueState)
    {
        Debug.Log("Clue state is " + clueState);
        playerMenu.displayClue(clueState);
    }
	
    // Open arm menu.
    public void ToggleMenu()
    {
        Debug.Log("Space opens the menu.");
        isActive = !isActive;
        playerMenu.SetActive(isActive);
    }

    public void HighlightItem(GameObject menuItem)
    {
        MeshRenderer mesh = menuItem.transform.GetComponentInChildren<MeshRenderer>();
        mesh.material.color = Color.yellow;
        switch (menuItem.name)
        {
            case EXIT:
                menuItem.transform.GetComponent<doorAnimation>().setAnimationActive();
                break;
            case CLUE:
                break;
            case TOOL:
                break;
            case KEY:
                break;
            default:
                Debug.LogWarning("Undefined item:--" + menuItem.name + "-- was selected.");
                break;
        }
    }

    public void SelectItem(GameObject menuItem)
    {
        switch (menuItem.name)
        {
            case EXIT:
                ExitGame();
                break;
            case CLUE:
                displayClue(clueStatesScript.GetClueState());
                break;
            case TOOL:
                Debug.Log("Tool item is selected, not sure if we need this.");
                break;
            case KEY:
                Debug.Log("TODO(sylvia): Key needs to be implemented!");
                break;
            default:
                Debug.LogWarning("Undefined item:--" + menuItem.name + "-- was selected.");
                break;
        }
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        // Does not work during webplayer and editor.
        Application.Quit();
    }
}

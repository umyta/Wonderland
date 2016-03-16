using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MenuUI))]
public class MouseOverDetector : MonoBehaviour
{
    int UIMask;
    MenuUI menu;
    // Use this for initialization
    void Awake()
    {
        UIMask = LayerMask.GetMask("UI");
        menu = GetComponent<MenuUI>();
    }
	
    // Update is called once per frame
    void Update()
    {  
        GameObject mousePointedAt = HelperLibrary.RaycastObject(Input.mousePosition, UIMask).hitObject;
        if (mousePointedAt != null)
        {
            menu.HighlightItem(mousePointedAt);
        }
    }
}

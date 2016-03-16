using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
	
    }
	
    // Update is called once per frame
    void Update()
    {
	
    }

    public void SetActive(bool enable)
    {
        transform.gameObject.SetActive(enable);
    }
}

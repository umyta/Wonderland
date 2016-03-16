using UnityEngine;
using System.Collections;

public class PlayerCamera : MonoBehaviour
{
    public void SetActive(bool enable)
    {
        transform.gameObject.SetActive(enable);
    }
}

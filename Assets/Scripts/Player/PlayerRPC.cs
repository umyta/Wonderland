using UnityEngine;
using System.Collections;

public class PlayerRPC : MonoBehaviour
{

    [PunRPC]
    public void Resize(float scale)
    {
        // TODO(sainan): we are currently relying on network serilization to sync this.
        // We'll see if this works or do we need RPC calls.
        transform.localScale = new Vector3(
            transform.localScale.x * scale, 
            transform.localScale.y * scale, 
            transform.localScale.z * scale);
    }

}

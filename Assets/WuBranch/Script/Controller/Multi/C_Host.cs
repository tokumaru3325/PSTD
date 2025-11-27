using Unity.Netcode;
using UnityEngine;

public class C_Host : MonoBehaviour
{

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }


}

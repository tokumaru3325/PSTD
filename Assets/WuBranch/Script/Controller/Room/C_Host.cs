using System.Text;
using Unity.Netcode;
using UnityEngine;

public class C_Host : MonoBehaviour
{

    private C_GlobalVariable _globalVariable;

    void Start()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        }
        NetworkManager.Singleton.StartHost();
    }

    /// <summary>
    /// 接続承認チェック
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        // ホストの場合は自動承認
        if (request.ClientNetworkId == NetworkManager.Singleton.LocalClientId)
        {
            response.Approved = true;
            return;
        }

        // パスワードがない場合は自動承認
        string password = _globalVariable.GetRoomData().Password;
        if (string.IsNullOrEmpty(password))
        {
            response.Approved = true;
            return;
        }

        string payload = Encoding.UTF8.GetString(request.Payload);
        response.Approved = payload == password;
    }
}

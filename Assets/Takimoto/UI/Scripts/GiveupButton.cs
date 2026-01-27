using UnityEngine;

public class GiveupButton : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private C_PlayerTowerController playerController;

    private void Start()
    {
        playerController = _player.GetComponent<C_PlayerTowerController>();
    }

    public void OnButton_Giveup()
    {
        if (!playerController) return;

        playerController.DecreaseHP(9999);
    }
}

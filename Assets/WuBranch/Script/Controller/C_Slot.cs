using UnityEngine;

public class C_Slot : MonoBehaviour
{
    [SerializeField]
    private string _playerTag;

    [SerializeField]
    private string _enemyTag;

    [SerializeField, Tooltip("0 = お金｜1 = モンスター｜2 = バ")]
    private int _slotSelect;

    public void OpenSlot()
    {
        if (_slotSelect == 0)
        {
            SlotSceneManager.ChangeSceneToMoney(_playerTag, _enemyTag);
            return;
        }
        if (_slotSelect == 1)
        {
            SlotSceneManager.ChangeSceneToMonster(_playerTag, _enemyTag);
            return;
        }
        if (_slotSelect == 2)
        {
            SlotSceneManager.ChangeSceneToBuff(_playerTag, _enemyTag);
            return;
        }
    }
}

using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class V_Slot : MonoBehaviour
{
    [SerializeField]
    private string _playerTag;

    [SerializeField]
    private string _enemyTag;

    [SerializeField, Tooltip("0 = お金｜1 = モンスター｜2 = バ")]
    private int _slotSelect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSlot()
    {
        //SceneManager.LoadScene("Slot", LoadSceneMode.Additive);
    //    int random = (int)Random.Range(0, 3);
    
        if(_slotSelect == 0)
        {
            SlotSceneManager.ChangeSceneToMoney(_playerTag, _enemyTag);
            return;
        }
        if(_slotSelect == 1)
        {
            SlotSceneManager.ChangeSceneToMonster(_playerTag, _enemyTag);
            return;
        }
        if ( _slotSelect == 2)
        {
            SlotSceneManager.ChangeSceneToBuff(_playerTag, _enemyTag);
            return;
        }
    }
}

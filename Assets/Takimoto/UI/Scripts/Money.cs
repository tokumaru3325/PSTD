using TMPro;
using UnityEngine;

public class Money : MonoBehaviour
{
    public TextMeshProUGUI _textMoney;
    private Player _player;

    private int _money;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //_player = FindAnyObjectByType<Player>();
        _player = GameObject.FindGameObjectWithTag("Player1").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // 2026.01.13 ウー start
        if (!_player)
            return;
        // 2026.01.13 ウー end
        _money = (int)_player.Money;
        //_textMoney.SetText(_money.ToString());
        _textMoney.SetText(((int)_player.Money).ToString());
        Debug.Log($"今{(int)_player.Money}");
    }
}

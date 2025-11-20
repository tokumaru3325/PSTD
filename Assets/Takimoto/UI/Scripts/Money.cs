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
        _player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        _money = (int)_player.Money;
        _textMoney.SetText(_money.ToString());
    }
}

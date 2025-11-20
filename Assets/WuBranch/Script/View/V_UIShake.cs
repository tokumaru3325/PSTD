using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class V_UIShake : MonoBehaviour
{
    private Transform _target;

    /// <summary>
    /// 継続時間
    /// </summary>
    [SerializeField]
    private float _duration = 0.5f;

    /// <summary>
    /// 振動の強さ
    /// </summary>
    [SerializeField]
    private float _strength = 20.0f;

    /// <summary>
    /// 振動の細かさ
    /// </summary>
    [SerializeField]
    private int _vibrate = 100;

    /// <summary>
    /// クールダウン時間
    /// </summary>
    [SerializeField]
    private float _coolDown = 3.0f;

    /// <summary>
    /// クールダウンタイマー
    /// </summary>
    private float _coolDownTimer;

    void Start()
    {
        _target = this.transform;
        _coolDownTimer = 0.0f;
    }

    void FixedUpdate()
    {
        if (_coolDownTimer > 0.0f)
        {
            _coolDownTimer -= Time.fixedDeltaTime;
        }
    }

    public void Shake()
    {
        if (_coolDownTimer <= 0.0f)
        {
            _target.DOShakePosition(_duration * 0.5f, _strength, _vibrate);
            _coolDownTimer = _coolDown;
        }
    }
}

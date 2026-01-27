using UnityEngine;

public class UnitSpriteScript : MonoBehaviour
{
    [SerializeField] private UnitView _view;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void OnDeathAnimationEnd()
    {
        _view.OnDeathAnimationEnd();
    }
    public void OnDefeatAnimationStart()
    {
        _view.OnDefeatAnimationStart();
    }
    public void OnDefeatAnimationEnd()
    {
        _view.OnDefeatAnimationEnd();
    }
    public void FaceLeft()
    {
        _view.FaceLeft();
    }
    public void FaceRight()
    {
        _view.FaceRight();
    }

    public void PlaySwordAttackSE()
    {
        _view.PlaySwordAttackSE();
    }

    public void PlaySwordBlockSE()
    {
        _view.PlaySwordBlockSE();
    }

    public void PlaySwordImpactSE()
    {
        _view.PlaySwordImpactSE();
    }

    public void PlaySwordParrySE()
    {
        _view.PlaySwordParrySE();
    }

    public void PlayBowAttackSE()
    {
        _view.PlayBowAttackSE();
    //    DebugManager.Instance.Log("Bow atk SE", LogType.Error);
    }

    public void PlayBowBlockSE()
    {
        _view.PlayBowBlockSE();
    }

    public void PlayBowImpactSE()
    {
        _view.PlayBowImpactSE();
    }

    public void PlayBuffSE()
    {
        _view.PlayBuffSE();
    }

    public void PlayBigBuffSE()
    {
        _view.PlayBigBuffSE();
    }

    public void PlayFireBallSE()
    {
        _view.PlayFireBallSE();
    }

    public void PlaySpellImpactSE()
    {
        _view.PlaySpellImpactSE();
    }
}

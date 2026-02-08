public interface IObstacleStoneView : IView
{
    void HandleDamageEffect();
    void UpdateHP(float hp, float maxHp);

    void Highlight();
    void Unhighlight();
}

using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    static TextMeshProUGUI _battleResult;

    static EffectManager _effectManager;

    private static bool _isWin; // 勝敗結果を保持

    public static void LoadEnding(bool isWin)
    {
        _isWin = isWin;
        //ロード完了時に実行される関数を登録
        SceneManager.sceneLoaded += OnEndingSceneLoaded;
        //シーンを読み込む
        SceneManager.LoadScene("EndingScene", LoadSceneMode.Additive);
    }

    public void OnButton_ReturnTitle()
    {
        SceneManager.LoadScene("TitleScene");
        SlotSceneManager.yey();
    }

    public static void OnEndingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 目的のシーン以外なら無視
        if (scene.name != "EndingScene") return;

        // 2026.02.01 ウー start
        V_DarkMask.Instance.OpenMask();
        // 2026.02.01 ウー end

        //ロードが終わったので中身を探す
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            _effectManager = obj.GetComponentInChildren<EffectManager>();
            if (_effectManager != null && _effectManager.name == "EffectManager")
            {
                _effectManager.Initialize();
                break;
            }
        }

        foreach (GameObject obj in rootObjects)
        {
            // "Winner" という名前のオブジェクトを探す            
            _battleResult = obj.GetComponentInChildren<TextMeshProUGUI>(true);
            if (_battleResult != null && _battleResult.name == "Winner")
            {
                ApplyResult(_battleResult, _isWin);
                break;
            }
        }

        // 4. 二重実行を防ぐためにイベントを解除
        SceneManager.sceneLoaded -= OnEndingSceneLoaded;
    }

    private static void ApplyResult(TextMeshProUGUI textUI, bool isWin)
    {
        if (isWin)
        {
            textUI.text = "Player Win!";
            textUI.color = Color.red;

            _effectManager.PaperFubukiEffectPlay(100);
        }
        else
        {
            textUI.text = "Player Lose...";
            textUI.color = Color.blue;
        }
    }
}

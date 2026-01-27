using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    static GameObject _battleResultUI;
    static TextMeshProUGUI _battleResult;

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
    }

    public static void OnEndingSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 目的のシーン以外なら無視
        if (scene.name != "EndingScene") return;

        //ロードが終わったので中身を探す
        GameObject[] rootObjects = scene.GetRootGameObjects();
        foreach (GameObject obj in rootObjects)
        {
            // "Winner" という名前のオブジェクトを探す            
            TextMeshProUGUI tmp = obj.GetComponentInChildren<TextMeshProUGUI>(true);

            if (tmp != null && tmp.name == "Winner")
            {
                ApplyResult(tmp, _isWin);
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
        }
        else
        {
            textUI.text = "Player Lose...";
            textUI.color = Color.blue;
        }
    }
}

using TMPro;
using UnityEngine;

public class ResultTextManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TextMeshProUGUI;
    private bool textChange = false;
    float currentTimer = 0.0f;
    float resetSecond = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_TextMeshProUGUI.SetText("");
        currentTimer = resetSecond;
    }

    // Update is called once per frame
    void Update()
    {
        if (textChange)
        {
            TextReset();
        }
    }

    public void TextChange(int slotNum,int resultNum,bool koyaku)
    {
        if (!koyaku)
        {
            m_TextMeshProUGUI.SetText("はずれ");
        }
        else
        {
            if (slotNum == 0)
            {
                switch (resultNum)
                {
                    case 0:
                        m_TextMeshProUGUI.SetText("15円！");
                        break;
                    case 1:
                        m_TextMeshProUGUI.SetText("30円！");
                        break;
                    case 2:
                        m_TextMeshProUGUI.SetText("100円！");
                        break;
                    case 3:
                        m_TextMeshProUGUI.SetText("150円！");
                        break;
                    case 4:
                        m_TextMeshProUGUI.SetText("300円！");
                        break;
                    default:
                        m_TextMeshProUGUI.SetText("はずれ");
                        break;

                }
            }
            else if(slotNum == 1)
            {
                switch (resultNum)
                {
                    case 0:
                        m_TextMeshProUGUI.SetText("Knight（2倍出現）！");
                        break;
                    case 1:
                        m_TextMeshProUGUI.SetText("Archer（2倍出現）！");
                        break;
                    case 2:
                        m_TextMeshProUGUI.SetText("Archer（3倍出現）！");
                        break;
                    case 3:
                        m_TextMeshProUGUI.SetText("Knight（3倍出現）！");
                        break;
                    case 4:
                        m_TextMeshProUGUI.SetText("Mage（3倍出現）！");
                        break;
                    default:
                        m_TextMeshProUGUI.SetText("Mage（2倍出現）！");
                        break;

                }
            }
            else
            {
                switch (resultNum)
                {
                    case 0:
                        m_TextMeshProUGUI.SetText("はずれ");
                        break;
                    case 1:
                        m_TextMeshProUGUI.SetText("攻撃力上昇!!!");
                        break;
                    case 2:
                        m_TextMeshProUGUI.SetText("攻撃速度上昇!!!");
                        break;
                    case 3:
                        m_TextMeshProUGUI.SetText("攻撃範囲拡大!!!");
                        break;
                    case 4:
                        m_TextMeshProUGUI.SetText("移動速度上昇!!!");
                        break;
                    default:
                        m_TextMeshProUGUI.SetText("はずれ");
                        break;

                }
            }
        }
        textChange = true;
    }

    public void SetText(string text)
    {
        m_TextMeshProUGUI.SetText(text);
        textChange = true;
    }

    public void ResetText()
    {
        m_TextMeshProUGUI.SetText("");
        textChange = false;
        currentTimer = resetSecond;
    }

    public void EnterCurrentTimer()
    {
        currentTimer = resetSecond;
    }

    void TextReset()
    {
        currentTimer -= 1 * Time.deltaTime;
        if(currentTimer <= 0.0f)
        {
            m_TextMeshProUGUI.SetText("");
            textChange = false;
            currentTimer = resetSecond;
        }
    }
}

using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI BoldMsg = null;
    [SerializeField] TextMeshProUGUI Msg = null;

    internal void SetMessage(string msg, string boldMsg)
    {
        Msg.text = msg;
        BoldMsg.text = boldMsg;
    }

    public void OnClickOkButton() //from popoup msg
    {
        gameObject.SetActive(false);
    }
}

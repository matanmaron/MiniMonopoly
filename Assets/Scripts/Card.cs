using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] Sprite FreeCard = null;
    [SerializeField] Sprite Player1 = null;
    [SerializeField] Sprite Player2 = null;

    internal void SetCard(PropertyEnum owner)
    {
        switch (owner)
        {
            case PropertyEnum.Start:
            case PropertyEnum.Reward:
                GetComponent<Image>().color = new Color(0, 0, 0, 0);
                return;
            case PropertyEnum.Free:
                GetComponent<Image>().sprite = FreeCard;
                break;
            case PropertyEnum.BelongsTo1:
                GetComponent<Image>().sprite = Player1;
                break;
            case PropertyEnum.BelongsTo2:
                GetComponent<Image>().sprite = Player2;
                break;
            default:
                Debug.LogWarning("Card - ChangeOwner - no such card exists");
                break;
        }


    }
}

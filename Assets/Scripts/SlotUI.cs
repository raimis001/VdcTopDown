using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour
{
    public TMP_Text countText;
    public Image icon;

    public void SetCount(int count)
    {
        countText.text = count.ToString();
        countText.gameObject.SetActive(count > 1);

        icon.gameObject.SetActive(count > 0);
    }

    public void SetItem(ItemClass item, int count)
    {
        icon.sprite = item.icon;
        icon.color = Color.white;
        SetCount(count);
    }
}

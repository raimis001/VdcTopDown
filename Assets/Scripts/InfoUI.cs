using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoUI : MonoBehaviour
{
    public TMP_Text messageText;
    public float maxDeltaTime = 3;

    float deltaTime;
    Color textColor;

    void Start()
    {
        textColor = messageText.color;
        textColor.a = 0;
        messageText.color = textColor;

        messageText.text = "";
    }

    void Update()
    {
        if (deltaTime > 0)
        {
            deltaTime -= Time.deltaTime;
            textColor.a = deltaTime / maxDeltaTime;
            messageText.color = textColor;
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        textColor.a = 1;
        messageText.color = textColor;

        deltaTime = maxDeltaTime;
        
    }
}

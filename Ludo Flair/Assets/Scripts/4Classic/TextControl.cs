using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextControl : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI m_text;
    string append;

    // Update is called once per frame
    void Update()
    {
        if(GameMovement.colNumber == 0)
        {
            m_text.color = Color.blue;
        }
        else if(GameMovement.colNumber == 1)
        {
            m_text.color = Color.red;
        }
        else if (GameMovement.colNumber == 2)
        {
            m_text.color = Color.green;
        }
        else if (GameMovement.colNumber == 3)
        {
            m_text.color = Color.yellow;
        }

        if (Dice.needPiece == true) 
        {
            append = "...";
        }
        else
        {
            append = "";
        }
        

        m_text.text = "Round Number: " + Dice.roundNo.ToString() + append;

    }
}

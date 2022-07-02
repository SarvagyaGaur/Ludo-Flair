using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;


public class TextControl1 : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI m_text;

    // Account Of Bombs and Dolls
    public TextMeshProUGUI accountB_text;
    string append;

    // Update is called once per frame
    void Update()
    {
        // Changes the colour of the text depending on Whose turn it is 
        if (GameMovement1.colNumber == 0)
        {
            m_text.color = Color.blue;
            accountB_text.color = Color.blue;
        }
        else if (GameMovement1.colNumber == 1)
        {
            m_text.color = Color.red;
            accountB_text.color = Color.red;

        }
        else if (GameMovement1.colNumber == 2)
        {
            m_text.color = Color.green;
            accountB_text.color = Color.green;
        }
        else if (GameMovement1.colNumber == 3)
        {
            m_text.color = Color.yellow;
            accountB_text.color = Color.yellow;
        }

        if (Dice1.needPiece == true)
        {
            append = "...";
        }
        else
        {
            append = "";
        }


        m_text.text = "Round Number: " + Dice1.roundNo.ToString() + append;

        accountB_text.text = ": " + (Waypoints1.bombAccount[GameMovement1.colNumber]).ToString() + "             : "+ (Waypoints1.dollAccount[GameMovement1.colNumber]).ToString(); 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    // Identifies the objects for me :(

    public int pieceColour;
    public int pieceNumber;

    public static int[] bombAccount = new int[4] { 0, 0, 0, 0 };


    private void OnMouseDown()
    {
        if (Dice.diceEngaged == false) // Does not Allow you to play if A turn Is In Motion
        {
            Debug.Log(this.pieceNumber);
            Debug.Log(GameMovement.colNumber);
        }
    }
}







using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Waypoints1 : MonoBehaviour
{
    // Identifies the objects for me :(
    public int pieceColour;
    public int pieceNumber;
    public int golbalLocation;
    public int baseColour;
    public int roundOfPlacement;

    /* NOTE ON THE USE OF WAYPOINT 
     * 
     * This script is placed on the following gameobjects
     * 
     * - Game Pieces
     * --pieceColour stores the color of that piece
     * --pieceNumber stores the index of that piece
     *
     * - Bum Prefab Object
     * -- pieceColour stores the colour that placed that bum
     * -- pieceNumber stores the round in which the bum was placed #NEED TO CHANGE to round of placement
     * 
     * - Waypoints That Make Up The Game Loop
     * -- pieceNumber stores the golbal position value of that point #NEED TO CHANGE to golbalLocation
     * 
     * - Bomb Button & Curse Button
     * -- The Objects use any unique variable and is used only to make clicking on it easy
     * 
     * - Alternate Childern Of The Cursed Status
     * -- pieceColour stores the colour that cast the curse
     * -- roundOfPlacement stores the round in which the curse was cast
     * 
     */


    // TODO: Function that clears out the curse once completed



    // Bomb Inventory Of Each Player
    public static int[] bombAccount = new int[4] { 0, 0 , 0, 0 };

    // Doll Inventory Of Each Player
    public static int[] dollAccount = new int[4] { 0, 0, 0, 0 };

    // Is A Item being placed
    public static bool placingToken;

    // Is A Bomb being placed
    public static bool placingBomb = false;

    // Is A Curse Being Cast?
    public static bool placingCurse = false;

    // The tile on which the item is gonna be placed
    // Its a valid position only if it is below 100
    int positionOfPlacement = 100;

    // The Bomb Tokens Objects
    public static GameObject Bomb1;
    public static GameObject Bomb2;

    // The Doll Tokens Objects
    public static GameObject Doll1;
    public static GameObject Doll2;

    //The Bum Prefab || Object
    public static GameObject bombPrefab;

    // Instance Of Active Bomb
    public static GameObject activeBombInstance;

    // The transforms of the game loop
    public static Transform[] gameLoop;

    public static GameObject[] activeBombs = new GameObject[] { };
    
    // The position of the tokens {bomb1, bomb2, doll1, doll2}
    public static int[] token_index = new int[4] { 0, 0, 0, 0};

    // Does the token need to be relocated?
    public static bool[] needToken = new bool[4] { true, true, true, true };

    // A Hack To Prevent Tokens From Being Wiped Out When Token Kills Piece
    public static bool beingKilled = false;

    public static GameObject CursesIcons;

    public static int[] curseStatus = new int[4] { 0,0,0,0};


    void Start()
    {
        Bomb1 = GameObject.Find("bomb (1)");
        Bomb2 = GameObject.Find("bomb (2)");

        Doll1 = GameObject.Find("doll (1)");
        Doll2 = GameObject.Find("doll (2)");

        CursesIcons = GameObject.Find("CurseStatus");

        bombPrefab = GameObject.Find("bum");
        gameLoop = GameObject.Find("BoardWayPoints").GetComponentsInChildren<Transform>();

    }

    // Assigns A valid position to the token, to be regenerated upon
    int assignPosition()
    {
        while (true)
        {
            // chooses a golbal position at random
            int potential = Random.Range(1, 53);
            
            // If overLap turn out to be true, the position is invalid and we look for another position
            bool overLap = false;

            // The Spots the token cant generate upon
            int[] baseSpots = new int[4] { 1, 14,  27,  40};
            
            foreach (int spot in baseSpots)
            {
                if (spot == potential)
                {
                    overLap = true;
                    continue;
                }
            }
            // The token shoudn't spawn on top of a player
            for (int c = 0; c < 4; c++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (GameMovement1.pos_index_golbal[c,i] == potential)
                    {
                        overLap = true;
                        continue;
                    } 
                }
            }
            // Shouldn't spawn on a pre existing token
            foreach(int spot in token_index)
            {
                if (spot == potential)
                {
                    overLap = true;
                    continue;
                }
            }
            if (overLap)
            {
                continue;
            }
            else
            {
                //Debug.Log(potential);
                return potential ;   
            }


        }
    }
    
    void Update()
    {
        placingToken = placingBomb || placingCurse; // simple logic

        //for(int loc = 0; loc < token_index.Length;loc++){ }

        

        // Checks if token needs a new spawn point
        for(int k =0; k<4; k++)
        {
            if (needToken[k])
            {
                token_index[k] = assignPosition();
                needToken[k] = false;

                // Updates the transform of the gameObject depending upon the token index
                Bomb1.transform.position = (gameLoop[token_index[0]]).position;
                Bomb2.transform.position = (gameLoop[token_index[1]]).position;
                Doll1.transform.position = (gameLoop[token_index[2]]).position;
                Doll2.transform.position = (gameLoop[token_index[3]]).position;
            }
            

        }
        curseUpdater();
        explode();
        // Note: Picking Up Tokens Is Detected In GameMovement
        
        // debugging tool - respawns on spacebar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            for (int loc = 0; loc < token_index.Length; loc++) 
            {
                needToken[loc] = true;
            }
        }

    }

    void OnMouseDown()
    {
        if (Dice1.diceEngaged == false) // Does not Allow you to play if A turn Is In Motion
        {
            if (this.name == "BombButton") // if the bomb button is pressed
            {
                if (placingBomb == true) // Disengages BombMode If allready activated
                {
                    //StopCoroutine("placeBomb");
                    placingBomb = false;
                    Debug.Log("Chill");
                }
                else// Engages Bomb Mode
                {
                    Debug.Log("Allah Ho Akbar");
                    placingBomb = true;
                    positionOfPlacement = 100;
                    //StartCoroutine("placeBomb");
                }
            }

            if (this.name == "CurseButton")
            {
                if (placingCurse == true)
                {
                    placingCurse = false;
                }
                else
                {
                    Debug.Log("WITCH MODE");
                    placingCurse = true;
                }
            }


        }
        if (placingBomb) //Checks for clicking on tiles
        {
            if (this.tag == "gamePath")
            {
                //Debug.Log(this.GetComponent<Waypoints1>().pieceNumber);

                int potential = this.GetComponent<Waypoints1>().pieceNumber;
                bool overLap = false;
                // The Spots the item cant generate upon
                int[] baseSpots = new int[4] { 1, 14, 27, 40 };
                foreach (int spot in baseSpots)
                {
                    if (spot == potential)
                    {
                        overLap = true;
                        continue;
                    }
                }
                // The token shoudn't spawn on top of a player
                for (int c = 0; c < 4; c++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (GameMovement1.pos_index_golbal[c, i] == potential)
                        {
                            overLap = true;
                            continue;
                        }
                    }
                }
                if (!overLap)
                {
                    positionOfPlacement = potential;
                    
                    if (positionOfPlacement < 100 & bombAccount[GameMovement1.colNumber] > 0)
                    {
                        //Debug.Log("Valid" + positionOfPlacement.ToString());
                        activeBombInstance = Instantiate(bombPrefab, gameLoop[positionOfPlacement].position, Quaternion.identity);

                        activeBombInstance.GetComponent<Waypoints1>().pieceNumber = Dice1.roundNo;
                        activeBombInstance.GetComponent<Waypoints1>().pieceColour = GameMovement1.colNumber;
                        activeBombInstance.GetComponent<Waypoints1>().golbalLocation = positionOfPlacement;

                        activeBombs = activeBombs.Append(activeBombInstance).ToArray();
                        bombAccount[GameMovement1.colNumber]--;
                        positionOfPlacement = 100;
                    }
                }

                

            }
        }

        if (placingCurse)
        {
            if(this.tag == "Base")
            {
                int baseColour = this.GetComponent<Waypoints1>().baseColour;

                bool canPlaceDoll = false;

                int[,] colourWiseSafeSpots = new int[,] { { 1, 9 }, { 14, 22 }, { 27, 35 }, { 40, 48 } };
                
                for (int p = 0; p < 4; p++)
                {
                    for(int spot = 0; spot < 2; spot++)
                    {
                        if(colourWiseSafeSpots[baseColour,spot] == GameMovement1.pos_index_golbal[GameMovement1.colNumber , p])
                        {
                            canPlaceDoll = true;
                            break;
                        }
                    }
                }
                
                if (canPlaceDoll & dollAccount[GameMovement1.colNumber] > 0)// doll account
                {
                    if (curseStatus[baseColour] < 2)
                    {
                        // Increase Curse Level
                        curseStatus[baseColour]++;
                        //Debug.Log("Curse Status Of " + (baseColour).ToString() + " is " + (curseStatus[baseColour]).ToString());
                    }
                       
                    // Who Cursed The Colour
                    CursesIcons.transform.GetChild((baseColour * 2) + 0).gameObject.GetComponent<Waypoints1>().pieceColour = GameMovement1.colNumber;

                    // Which Round Was the Curse Placed
                    CursesIcons.transform.GetChild((baseColour * 2) + 0).gameObject.GetComponent<Waypoints1>().roundOfPlacement = Dice1.roundNo;

                    dollAccount[GameMovement1.colNumber]--;
                }


            }
        }

    }

    void explode()
    {

        for (int bom = 0; bom < activeBombs.Length; bom++)
        {
            bool condition1Executed = false;
            if (!Dice1.diceEngaged & !beingKilled)
            {
                
                for (int c = 0; c < 4; c++)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (GameMovement1.pos_index_golbal[c, i] == activeBombs[bom].GetComponent<Waypoints1>().golbalLocation)
                        {
                            //Debug.Log(c.ToString() + "Colour");
                            //Debug.Log(i.ToString() + "Number");
                            //Debug.Log(activeBombs.Length.ToString() + " Length");
                            //Debug.Log(activeBombs[bom].GetComponent<Waypoints1>().golbalLocation.ToString() +" loc");
                            blastBomb(activeBombs[bom]);
                            beingKilled = true;
                            StartCoroutine("MiniKilling", new int[] { c, i });
                            condition1Executed = true;
                            
                        }
                    }
                }
            }
            if(!condition1Executed)
            {
                if (activeBombs[bom].GetComponent<Waypoints1>().pieceNumber + 3 == Dice1.roundNo & activeBombs[bom].GetComponent<Waypoints1>().pieceColour == GameMovement1.colNumber)
                {
                    blastBomb(activeBombs[bom]);
                    break;
                }

            }

        }
        
        
    }

    private IEnumerator MiniKilling(int[] array)
    {
        Debug.Log(array[0].ToString() + "Colour");
        Debug.Log(array[1].ToString() + "Number");
        Dice1.sixCondition[array[0], array[1]] = false;
        Debug.Log("Set to false");

        while (GameMovement1.pos_index[array[0], array[1]] != 0)
        {
            GameMovement1.pos_index[array[0], array[1]]--;
            yield return new WaitForSeconds(0.1f);
        }
        Debug.Log("Backed Up");
        beingKilled = false;
    }

    void blastBomb(GameObject X)
    {
        GameObject[] tempArray = new GameObject[] { };
        //Debug.Log("Booomm!!");
        int placeOfBombing = X.GetComponent<Waypoints1>().golbalLocation;
        //Debug.Log(placeOfBombing);
        Destroy(X, 0);

        foreach (GameObject bomb in activeBombs)
        {
            if (bomb != X & bomb.GetComponent<Waypoints1>().golbalLocation != placeOfBombing)
            {
                tempArray = tempArray.Append(bomb).ToArray();
            }
            if(bomb.GetComponent<Waypoints1>().golbalLocation == placeOfBombing)
            {
                Destroy(bomb, 0);
            }
        }        

        for(int pos = 0; pos < 4; pos++)
        {
            if (token_index[pos] == placeOfBombing)
            {
                needToken[pos] = true;
            
                //Debug.Log(token_index[pos]);
            }
        }

        activeBombs = tempArray;
    }

    void curseUpdater()
    {
        for(int c = 0; c<4; c++)
        {
            int status = curseStatus[c];
            GameObject primaryDoll = CursesIcons.transform.GetChild((c * 2) + 0).gameObject;
            GameObject secondaryDoll = CursesIcons.transform.GetChild((c * 2) + 1).gameObject;

            if (primaryDoll.GetComponent<Waypoints1>().roundOfPlacement + 5 == Dice1.roundNo & primaryDoll.GetComponent<Waypoints1>().pieceColour == GameMovement.colNumber)
            {
                curseStatus[c] = 0;
            }
            
            if (status == 0)
            {
                primaryDoll.GetComponent<Renderer>().enabled = false;
                secondaryDoll.GetComponent<Renderer>().enabled = false;
            }
            else if (status == 1)
            {
                primaryDoll.GetComponent<Renderer>().enabled = true;
                secondaryDoll.GetComponent<Renderer>().enabled = false;
            }
            else
            {
                primaryDoll.GetComponent<Renderer>().enabled = true;
                secondaryDoll.GetComponent<Renderer>().enabled = true;
            }



        }


    }








}

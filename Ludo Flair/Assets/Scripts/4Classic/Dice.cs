using System.Collections;
using UnityEngine;
using System.Linq;

public class Dice : MonoBehaviour {

    // Array of dice sides sprites to load from Resources folder
    private Sprite[] diceSides;

    // Reference to sprite renderer to change sprites
    private SpriteRenderer rend;
    
    // This Prevents The Dice From Being Clicked When A Turn Is Being Played
    public static bool diceEngaged = false;

    // Is It Time For The Player To Now Pick A Piece
    public static bool needPiece = false;

    // Which Piece Did The Player Select
    // The Loop In Update Is Setup As Such: The Piece selected is considered valid only if its value is less than 4;
    int pieceSelection = 6;

    // The Number Outputed By The Dice
    int finalSide;

    // Did A Player Just Put A Piece Home
    bool justReached = false;

    // The Round Being At
    public static int roundNo = 1;

    

    // TODO  an array of all the possible pieces a player can move 


    // Triple 6 Condition
    int continuousSixes = 0;

    // If Each Piece Has Been Freed From Base With A Six
    public static bool[,] sixCondition = new bool[4, 4] { {false, false, false, false },{false, false, false, false },
                                                          {false, false, false, false}, {false, false, false, false } };

    // If Each Piece Has Reached Its At Its Path End
    public static bool[,] finishCondition = new bool[4, 4] { {false, false, false, false },{false, false, false, false },
                                                             {false, false, false, false}, {false, false, false, false } };

    public static bool stp_out = false; // also used for backtracking 
    public static bool upscale = false;


    private void Start () 
    {
        // Loading In Dice Sprites 
        rend = GetComponent<SpriteRenderer>();

        diceSides = Resources.LoadAll<Sprite>("DiceSides/");

    }

    private void Update()
    {
        // If That Color Has Won We Skip That Color's Turn
        if (wonYet(GameMovement.colNumber) == true)
        {
            needPiece = false;
            //diceEngaged = false;
            GameMovement.colNumber = nextColor(GameMovement.colNumber);
        }

        // Start Checking If A Piece Is Needed To Be Selected By A Player
        if (needPiece == true)
        {
            // Calling the Function That Spits out a Piece That The Player Has Selected
            pieceSelection = pieceDecider(GameMovement.colNumber, finalSide);
            //Debug.Log("I got you bro");
            
            // pieceDecider() is setup such that its a valid number only if it is < 4
            if (pieceSelection < 4 )
            {
                // If the piece selected has satisfied the six condtion 
                //if(sixCondition[GameMovement.colNumber, pieceSelection] == true)

                // The Piece is now moved
                StartCoroutine("MoveThePieces");

                

                // And We dont need a piece anymore
                needPiece = false;

            }

        }
    }

    int pieceDecider(int b, int dNum) //(GameMovement.colNumber, finalSide)
    {
        int a = 6; // the Output
        
        if (Input.GetMouseButtonDown(0)) // If Click Registered
        {
            // RayCast Something Something.. idk All i know it returns a gameobject that i can extract data from
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            
            if (hit.collider != null)// Checks If An Object Was Hit
            {
                // All the pieces have a tag called Piece
                if(hit.collider.gameObject.tag == "Piece") // checks if the object hit is a piece
                {
                    
                    int choosenColour = hit.collider.gameObject.GetComponent<Waypoints>().pieceColour;
                    int choosenPiece = hit.collider.gameObject.GetComponent<Waypoints>().pieceNumber;
                    int pieceCode = choosenColour * 10 + choosenPiece;
                    foreach (int elgPiece in moveablePiecesArray)
                    {
                        if (elgPiece == pieceCode)
                        {
                            a = choosenPiece;
                        }
                    }
                }
            }
        }     
        return a;
    }

    // If you left click over the dice then RollTheDice coroutine is started
    private void OnMouseDown()
    {
        if (diceEngaged == false) // Does not Allow you to play if A turn Is In Motion
        {
            StartCoroutine("RollTheDice");
        }
    }

    // Function That Moves The Turn To The Next Player
    int nextColor(int x)
    {
        if (x == 3) 
        { 
            x = 0;
            roundNo++;
        }
        else 
        { 
            x++; 
        }
        return x;
    }

    int modulo52(int a)
    {

        if (a > 52)
        {
            a -= 52;
        }
        return a;
    }

    int[] moveablePiecesArray;


    int[] collisionPrediction(int curCol, int pieceNumber, int diceNumber)
    {
        int[] collisionsArray = new int[] {};

        int predictedLocation = modulo52(GameMovement.pos_index_golbal[curCol, pieceNumber] + diceNumber);
        
        // Locations of Safe Spots On Absolute Path : 1, 9, 14, 22, 27, 35, 40, 48
        int[] safeSpots = new int[8] { 1, 9, 14, 22, 27, 35, 40, 48 };
        bool isSafeSpot = false;
        foreach (int spot in safeSpots)
        {
            if (spot == predictedLocation)
            {
                isSafeSpot = true;
            }
        }

        bool eatingPosibile = ( (GameMovement.pos_index[curCol, pieceNumber] + diceNumber < 50) & (sixCondition[curCol, pieceNumber] == true) & (!isSafeSpot) );

        if (eatingPosibile)
        {
            for (int c = 0; c < 4; c++)
            {
                if (c == curCol) { continue; }
                for (int i = 0; i < 4; i++)
                {
                    if (predictedLocation == GameMovement.pos_index_golbal[c, i])
                    {
                        collisionsArray = collisionsArray.Append(c * 10 + i).ToArray();

                    }
                }
            }
        }
        return collisionsArray;
    }

    
    
    // Is The Player In A Position To Make A Move
    bool eligibleToMove(int curCol, int diceNumber)
    {
        // Incomplete - The Function Should Work But Im Not sure
        // If A Condition I have not considered Pops up
        // The Last Else statement will kick in and print to console

        int atBase = 0;
        int atPlay = 0;


        bool isSix = (diceNumber == 6);

        moveablePiecesArray = new int[] { };
        //moveablePiecesArray = moveablePiecesArray.Append(item);

        

        for (int i = 0; i < 4; i++)
        {
            if (sixCondition[curCol,i] == false & isSix)
            {
                atBase++;
                moveablePiecesArray = moveablePiecesArray.Append(curCol * 10 + i).ToArray(); 
            }

       

            if ((GameMovement.pos_index[curCol,i] + diceNumber < 58) & (sixCondition[curCol, i] == true)) 
            {
                if(collisionPrediction(curCol, i, diceNumber).Length < 2)
                {
                    atPlay++;
                    moveablePiecesArray = moveablePiecesArray.Append(curCol * 10 + i).ToArray();
                }
            }
        }
        // Debugging Tool START
        /*if (moveablePiecesArray.Length > 0)
        {
            string checker = "";
            for (int i = 0; i < moveablePiecesArray.Length; i++)
            {
                checker += "/" + (moveablePiecesArray[i]).ToString() + "/";
            }
            Debug.Log(checker);
        }*/
        // Debugging Tool END
        
       
        if (continuousSixes == 3) // Catches Triple 6 condition
        {
            continuousSixes = 0;
            return false;
        }
        else
        {
            if (!(atBase == 0) & isSix)
            {
                return true;
            }
            else if (atBase == 4 & !(isSix))
            {
                return false;
            }


            else if ((isSix & atBase == 0) || (!isSix & !(atBase == 4)))
            {

                if (atPlay > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                Debug.Log("Not Accounted - dice script, take sc");
                return false;

            }
        }
        
    }

    // Has The The Player Won?
    bool wonYet(int curCol)
    {
        int won = 0;
        for (int i = 0; i < 4; i++)
        {
            if (finishCondition[curCol, i] == true)
            {
                won++;
            }

        }
        if (won == 4)
        {
            return true;
        }
        else
        {
            return false;
        }

    }
    
    // Coroutine that rolls the dice
    private IEnumerator RollTheDice()
    {
        // Dice Is Now Engaged
        diceEngaged = true;
        
        // Variable to contain random dice side number.
        // It needs to be assigned. Let it be 0 initially
        int randomDiceSide = 0;

        // Final side or value that dice reads in the end of coroutine
        // Loop to switch dice sides ramdomly
        // before final side appears. 20 itterations here.
        for (int i = 0; i <= 10; i++)
        {
            // Pick up random value from 0 to 5 (All inclusive)
            randomDiceSide = Random.Range(0, 6);

            // Set sprite to upper face of dice from array according to random value
            rend.sprite = diceSides[randomDiceSide];

            // Pause before next iteration
            yield return new WaitForSeconds(0.05f);
        }

        // Assigning final side so you can use this value later in your game
        finalSide = randomDiceSide + 1;

        bool isSix = (finalSide == 6);
        
        if (isSix)
        {
            continuousSixes++;
        }
        else
        {
            continuousSixes = 0;
        }

        if (eligibleToMove(GameMovement.colNumber, finalSide) == false) // If Player Is Not Eligible to Move 
        {
            // This Catches the condition where the player is unable to make a move and shifts the turn to the next player
            // We Dont need A Piece
            needPiece = false;

            // Next Player
            GameMovement.colNumber = nextColor(GameMovement.colNumber);

            // Dice In Now Disengaged
            diceEngaged = false;
        }
        else
        {
            // If Eligible, The Player Can Now Select A Piece
            // TODO: add a condition where in if only one piece is able to move we move it automatically

            //Debug.Log(movablePieces);
            if (moveablePiecesArray.Length == 1)
            {
                pieceSelection = moveablePiecesArray[0] - (10* GameMovement.colNumber);
                StartCoroutine("MoveThePieces");
            }
            else 
            {
                needPiece = true;
            }
            
        }

    }
   


    private IEnumerator MoveThePieces()
    {
        // Storing the colnumber locally so that the color number changing doesnt affect the player movement
        int localColNum = GameMovement.colNumber;
        bool isSix = (finalSide == 6);
        
        // Resetting To False
        justReached = false;

        // Did The Piece Reach?
        if (GameMovement.pos_index[localColNum, pieceSelection] + finalSide == 57)
        {
            finishCondition[localColNum, pieceSelection] = true;
            justReached = true;
        }

        bool hasEaten = false;// needs to be updated

        int[] collision= collisionPrediction(localColNum, pieceSelection, finalSide);

        upscale = false;

        /*
        int[] code = new int[16];
        int tc, ti;
        float og_piecesize = 1.75845f;
        Vector3 descale;
        
        for(int i = 0; i < code.Length; i++)
        {
            tc = code[i] / 10;
            ti = code[i] % 10;
            if(GameMovement.pos_index_golbal[tc, ti] != GameMovement.pos_index_golbal[localColNum, pieceSelection])
            {
                GameMovement.allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.localScale = new Vector3(og_piecesize, og_piecesize, og_piecesize);
                scaling = false;
            }
        }
        */

        if (isSix)
        {
            if (sixCondition[localColNum, pieceSelection] == false)
            {
                stp_out = true;
                sixCondition[localColNum, pieceSelection] = true;
                GameMovement.pos_index[localColNum, pieceSelection]++;
            }
            else
            {
                // Moving The Piece Step By Step
                for (int j = 0; j < finalSide; j++)
                {
                    stp_out = false;
                    upscale = true;
                    GameMovement.pos_index[localColNum, pieceSelection]++;
                    yield return new WaitForSeconds(0.2f);
                }
            }
        }
        else
        {
            // Moving The Piece Step By Step
            for (int j = 0; j < finalSide; j++)
            {
                stp_out = false;
                upscale = true;
                GameMovement.pos_index[localColNum, pieceSelection]++;
                
                yield return new WaitForSeconds(0.2f);
            }
        }
        if (collision.Length == 1)
        {

            hasEaten = true;
            int pieceColour = (collision[0]) / 10;
            int pieceNumber = (collision[0])%10;
            sixCondition[pieceColour, pieceNumber] = false;
            while(GameMovement.pos_index[pieceColour, pieceNumber] != 0)
            {
                stp_out = true;
                GameMovement.pos_index[pieceColour, pieceNumber]--;
                yield return new WaitForSeconds(0.07f);
            }
        }
        /*
        int count = 0;
        code[count] = localColNum * 10 + pieceSelection;
        for (int c = 0; c < 4; c++) 
        {
            for (int i = 0; i < 4; i++)
            {
                if ((c == localColNum && i != pieceSelection) || c != localColNum)
                {
                    if(GameMovement.pos_index_golbal[c, i] == GameMovement.pos_index_golbal[localColNum, pieceSelection])
                    {
                        count++;
                        code[count] = c * 10 + i;
                    }
                }
            }
        }
        if (count == 1)
        {
            scaling = true;
            tc = code[0] / 10;
            ti = code[0] % 10;
            descale = new Vector3(og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f));
            GameMovement.allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.localScale = descale;
            // GameMovement.allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.position -= new Vector3(0.3f, 0, 0);           
            tc = code[1] / 10;
            ti = code[1] % 10;
            descale = new Vector3(og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f));
            GameMovement.allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.localScale = descale;
        }
        */
        if (isSix || justReached || hasEaten) // Extra Turn
        {
            //GameMovement.colNumber = nextColor(GameMovement.colNumber);
            diceEngaged = false;
        }
        else // No Extra Turn
        {
            GameMovement.colNumber = nextColor(localColNum);
            diceEngaged = false;
        }
        
    }

}

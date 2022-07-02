using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMovement1 : MonoBehaviour
{
    public float speed = 6f;

    // Declaring A GameObject That Contains All The Pieces
    public static GameObject allPieces;

    // Declaring The Arrays That Holds The Path For Each Blue Piece
    public Transform[] blue1Path;
    public Transform[] blue2Path;
    public Transform[] blue3Path;
    public Transform[] blue4Path;

    // Declaring The Arrays That Holds The Path For Each Red Piece
    public Transform[] red1Path;
    public Transform[] red2Path;
    public Transform[] red3Path;
    public Transform[] red4Path;

    // Declaring The Arrays That Holds The Path For Each Green Piece
    public Transform[] green1Path;
    public Transform[] green2Path;
    public Transform[] green3Path;
    public Transform[] green4Path;

    // Declaring The Arrays That Holds The Path For Each Yellow Piece
    public Transform[] yellow1Path;
    public Transform[] yellow2Path;
    public Transform[] yellow3Path;
    public Transform[] yellow4Path;

    // Declaring The 2-DArrays That Holds The Paths For All The Pieces Of That Colour
    public Transform[][] bluePaths;
    public Transform[][] redPaths;
    public Transform[][] greenPaths;
    public Transform[][] yellowPaths;

    // Declaring The 3-DArrays That Holds Array Of The Paths For All The Colour
    public Transform[][][] allPaths;

    // A 2-D Array That Stores How Far Along Their Path Each Piece Is
    public static int[,] pos_index = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

    // A 2-D Array That Stores How Far Along The Absolute Path Each Piece Is
    // Defination of Absolute Path:
    //      0 means the piece is at home (irrespective of the piece)
    //      1 - 52 consitutes the game Loop 1 being blue start and 52 being the position just before blue start
    //          This is the magical range where if 2 pieces have the same index they are on the same path. 
    //      53 - 58 is the home stretch.
    public static int[,] pos_index_golbal = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

    // Whose Turn Is It
    public static int colNumber;

    // Start is called before the first frame update
    void Start()
    {
        colNumber = 0;
        allPieces = GameObject.Find("Pieces");
        //bluePieces = allPieces.transform.GetChild(0).gameObject;

        bluePaths = new Transform[][] { blue1Path, blue2Path, blue3Path, blue4Path };
        redPaths = new Transform[][] { red1Path, red2Path, red3Path, red4Path };
        greenPaths = new Transform[][] { green1Path, green2Path, green3Path, green4Path };
        yellowPaths = new Transform[][] { yellow1Path, yellow2Path, yellow3Path, yellow4Path };

        allPaths = new Transform[][][] { bluePaths, redPaths, greenPaths, yellowPaths };
    }

    // Update is called once per frame
    void Update()
    {
        // the loop is continously updating the positions of all the pieces
        for (int c = 0; c < 4; c++)
        {
            for (int i = 0; i < 4; i++)
            {
                // for each colour and for each of its piece it look for changes in position and moves them accordingly
                allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position =
                  Vector3.MoveTowards(allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position,
                                      allPaths[c][i][pos_index[c, i]].position, Time.deltaTime * speed);


                // Actively Calculating the global position index from position index 
                if ((pos_index[c, i] > 0) & (pos_index[c, i] < 53)) // If in Game Loop
                {
                    // calculating absolute path - fuck its not golobal path its actually an absolute path
                    // nevermind too late to change
                    pos_index_golbal[c, i] = pos_index[c, i] + (13 * c);

                    // Making sure we are not leaving bounds of the board
                    if (pos_index_golbal[c, i] > 52)
                    {
                        pos_index_golbal[c, i] -= 52;
                    }

                }
                // If at base or home stretch.
                else
                {
                    //if at base 0, if home stretch (53, 54, 55, 56, 57, 58)
                    pos_index_golbal[c, i] = pos_index[c, i];
                }

                // Picking Up Tokens
                if (!Dice1.diceEngaged & !(Waypoints1.beingKilled)) // so that just passing over a token doesnt pick it up
                {
                    for (int bo = 0; bo < 2; bo++)// for each of the bombs
                    {
                        if (pos_index_golbal[c, i] == Waypoints1.token_index[bo]) // if thier position coinsides with a piece
                        {
                            Waypoints1.needToken[bo] = true; // we relocate the piece
                            Waypoints1.bombAccount[c]++; // increase the bomb account of the color
                        }
                    }
                    for (int dol = 2; dol<4; dol++)
                    {
                        if (pos_index_golbal[c, i] == Waypoints1.token_index[dol]) // if thier position coinsides with a piece
                        {
                            Waypoints1.needToken[dol] = true; // we relocate the piece
                            Waypoints1.dollAccount[c]++; // increase the bomb account of the color
                        }
                    }

                }          


            }
        }

    }


    /*int[] combinations = new int[] {0001, 0002, 0003, 0010, 0011, 0012, 0013, 0020, 0021, 0022, 0023, 0030, 0031, 0032, 0033, 0102, 0103, 0110, 0111, 0112, 0113, 0120, 0121, 0122, 0123, 0130, 0131, 0132, 0133, 0203, 0210, 0211, 0212, 0213, 0220, 0221, 0222, 0223, 0230, 0231, 0232, 0233, 0310, 0311, 0312, 0313, 0320, 0321, 0322, 0323, 0330, 0331, 0332, 0333, 1011, 1012, 1013, 1020, 1021, 1022, 1023, 1030, 1031, 1032, 1033, 1112, 1113, 1120, 1121, 1122, 1123, 1130, 1131, 1132, 1133, 1213, 1220, 1221, 1222, 1223, 1230, 1231, 1232, 1233, 1320, 1321, 1322, 1323, 1330, 1331, 1332, 1333, 2021, 2022, 2023, 2030, 2031, 2032, 2033, 2122, 2123, 2130, 2131, 2132, 2133, 2223, 2230, 2231, 2232, 2233, 2330, 2331, 2332, 2333, 3031, 3032, 3033, 3132, 3133, 3233 };
    void toScaleOrNotToScale()
    {
        foreach (int comb in combinations)
        {
            int pieceColor1 = 
        }


    }*/

}

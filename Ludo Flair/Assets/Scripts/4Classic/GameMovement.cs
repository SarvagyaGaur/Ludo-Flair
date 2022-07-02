using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMovement : MonoBehaviour
{
    public float speed = 4f;
    
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
    public static int[,] pos_index = new int[4, 4] { { 0,0,0,0}, { 0, 0, 0, 0 }, { 0,0,0,0}, { 0,0,0,0 } };

    // A 2-D Array That Stores How Far Along The Absolute Path Each Piece Is
    // Defination of Absolute Path:
    //      0 means the piece is at home (irrespective of the piece)
    //      1 - 52 consitutes the game Loop 1 being blue start and 52 being the position just before blue start
    //          This is the magical range where if 2 pieces have the same index they are on the same path. 
    //      53 - 58 is the home stretch.
    public static int[,] pos_index_golbal = new int[4, 4] { { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 } };

    // Whose Turn Is It
    public static int colNumber;

    public static float og_piecesize = 1.75845f;
    public static int[] code = new int[16];
    int count;
    public static bool oneshot;
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
        oneshot = false;
        // the loop is continously updating the positions of all the pieces
        for (int c = 0; c < 4; c++)
        {
            for (int i = 0; i < 4; i++)
            {
                if (Dice.stp_out == false)
                {
                    if (count == 0)
                    {
                        allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.localScale = new Vector3(og_piecesize, og_piecesize, og_piecesize);
                    }
                    if (Dice.upscale == true || count == 0)
                    {   // for each colour and for each of its piece it look for changes in position and moves them accordingly
                        allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position =
                        Vector3.MoveTowards(allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position,
                                            allPaths[c][i][pos_index[c, i]].position, Time.deltaTime * speed);
                    }
                    
                }
                else if(Dice.stp_out == true)
                {
                        allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position =
                        Vector3.MoveTowards(allPieces.transform.GetChild(c).gameObject.transform.GetChild(i).gameObject.transform.position,
                                            allPaths[c][i][pos_index[c, i]].position, Time.deltaTime * (speed + 4f));   
                }
                
                
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
                
            }
        }
        int tc, ti;
        Vector3 descale;
        for (int c = 0; c < 4; c++)
        {
            for(int i = 0; i < 4; i++)
            {
                if (!Dice.diceEngaged)
                {
                    count = 0;
                    code[count] = c * 10 + i;
                    for (int c1 = c; c1 < 4; c1++)
                    {
                        for (int i1 = 0; i1 < 4; i1++)
                        {
                            if ((c == c1 && i != i1) || c != c1)
                            {
                                if (pos_index_golbal[c, i] == pos_index_golbal[c1, i1] && (pos_index[c, i] != 0) && (pos_index[c1, i1] != 0))
                                {
                                    count++;
                                    code[count] = c1 * 10 + i1;
                                }
                            }
                        }
                    }
                }
                if (count == 1)
                {
                    oneshot = true;
                    descale = new Vector3(og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f), og_piecesize - (og_piecesize * 0.5f));
                    tc = code[0] / 10;
                    ti = code[0] % 10;
                    allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.localScale = descale;
                    if (oneshot == true)
                    {
                       allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.position -= new Vector3(0.3f, 0, 0);
                    }
                    tc = code[1] / 10;
                    ti = code[1] % 10;
                    allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.localScale = descale;
                    if (oneshot == true)
                    {
                        allPieces.transform.GetChild(tc).gameObject.transform.GetChild(ti).gameObject.transform.position += new Vector3(0.3f, 0, 0);
                    }
                    oneshot = false;
                }

            }
        }
        
    }   

}

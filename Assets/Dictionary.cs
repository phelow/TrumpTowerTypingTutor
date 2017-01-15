//#define printContents
/// <summary>
/// Dictionary. This class is responsible for reading in and containing the 60,000+ words which will appear in this game.
/// This class has functionality to assign words for the many enemies in this game.
/// </summary>
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.IO;

public class Dictionary : MonoBehaviour
{
    /// sourceFile and reader are needed for accessing the classes we need to read in the data
    [SerializeField]
    protected TextAsset dictionaryFile;
    protected string text = " ";
    public static Dictionary ms_instance;

    //we have 100,000+ words
    //todo: set pagelengths to private
    private int[] pageLengths; //used to store the number of words on each page
    private ArrayList[] pages; //we want 11 pages so 0-10 0 is 2 characters, 10 is 12 or more

    private List<string> m_wordsInPlay;

    void Awake()
    {
        m_wordsInPlay = new List<string>();
        ms_instance = this;
    }

    public void ReAddWord(string word)
    {
        int difficulty;
        difficulty = calculateDifficulty(word); //find out the difficulty of this string (determined by length)
        pages[difficulty].Add(word); //add the element to an arraylist in the pages array
        pageLengths[difficulty] += 1; //increase the length of this entry
    }

    // Use this for initialization
    void Start()
    {
        //Initialize 10 difficulty levels
        pages = new ArrayList[10];//initialize array of 11 arrraylits
        pageLengths = new int[10];

        for (int i = 0; i < 10; i++)
        {
            pages[i] = new ArrayList();
            pageLengths[i] = -1; //set to -1 so we don't have to subtract 1 in pickword
        }

        try
        {
            //In order to initialize each letter in the alphabet we iterate through a string containing the entire alphabet
            //and insert each indivicual element into the pages array at an index correlating to the difficulty (determined by length)
            //of the string
#if oneLetterWords
		string alphabet = "abcdefghijklmnopqrstuvwxyz";

		for(int i =0; i <alphabet.Length-2; i++){
			pages[0].Add(alphabet.Substring(i,1));
			pageLengths[0]+=1;
		}
#endif
            //add the contents from the dictionary
            string[] textArr = dictionaryFile.text.Split('\n');

            foreach (string s in textArr)
            {
                string editedString = s.Trim(' ');
                editedString = editedString.Substring(0, editedString.Length - 1);
                int difficulty;
                difficulty = calculateDifficulty(editedString); //find out the difficulty of this string (determined by length)
                pages[difficulty].Add(editedString); //add the element to an arraylist in the pages array
                pageLengths[difficulty] += 1; //increase the length of this entry

            }

            //after everything is loaded print out one item frome ach page as well as the length of each page
#if printContents
			Debug.Log ("-----Printing the lengths of the dictionaries-----");
		for(int i = 0; i < 10; i++){
			Debug.Log ("Length of page " + i + " :" + pageLengths[i]); //set to -1 so we don't have to subtract 1 in pickword
		}
#endif
        }
        catch
        {
        }

    }

    /// <summary>
    /// Calculates the difficulty of this string.
    /// </summary>
    /// <returns>The difficulty.</returns> an integer between 0 and 9
    /// <param name="text">Text.</param>
    public int calculateDifficulty(string text)
    {
        if (text == null || text == "")
        {
            text = "ashfbewnnxcyviufhg";
        }
        int _difficulty = text.Length - 3;
        if (_difficulty > 9)
        { //don't let the difficulty be larger than 9 (prevent out of bounds exception)
            _difficulty = 9;
        }
        else if (_difficulty < 0)
        {
            _difficulty = 0;
        }
        return _difficulty;
    }

    /// <summary>
    /// Picks the word. Return one word from a specific difficulty level
    /// </summary>
    /// <returns>The word.</returns> a word of that difficulty level randomly selected
    /// <param name="difficulty">Difficulty.</param> How long the word should be 
    public string PickWord(int difficulty)
    {
        try
        {
            int index;
            int maxLength = pageLengths[difficulty];
            index = (int)UnityEngine.Random.Range(0, maxLength);
            string word = (string)(pages[difficulty])[index];
            pages[difficulty].Remove(word);
            pageLengths[difficulty]--;
            return word;
        }
        catch
        {
            try
            {
                Start();
                Debug.LogWarning("Dictionary line 69 difficulty:" + difficulty);

                int index;
                int maxLength = pageLengths[difficulty];

                index = (int)UnityEngine.Random.Range(0, maxLength);
                return (string)(pages[difficulty])[index];
            }
            catch
            {
                return "pwf";
            }
        }
    }
}

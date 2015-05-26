//Developed by Allan Moore 
//Unorthodox Game Studios

//This script is used to keep track of scores and display them. 


using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {
	
	
	public string PlayerName; 
	public int totalScore = 0;//The Players Score 

    public GUIStyle titleStyle;

    public Rect scoreBoardRect = new Rect(20,20,400,500);

    public Transform score3dText; 
    //public GUISkin scoreBoardSkin;//The Skin to use. 
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        score3dText.gameObject.GetComponent<TextMesh>().text = totalScore.ToString();
	}

    void OnGUI()
    {
        //scoreBoardRect = GUI.Window(0, scoreBoardRect, ScoreBoardWindow, "ScoreBoard");
        Vector2 scaleMultiplier = new Vector2(Screen.width/1920, Screen.height/1080);

        //GUI.Label(new Rect(20*scaleMultiplier.x, 20*scaleMultiplier.y, 1920*scaleMultiplier.x, 1080*scaleMultiplier.y), "Epic Tetris",titleStyle);
        //GUI.Label(new Rect(20*scaleMultiplier.x, 500*scaleMultiplier.y, 1920*scaleMultiplier.x, 1080*scaleMultiplier.y), "by Allan Moore",titleStyle);

    }
    /*
    void ScoreBoardWindow(int WinID)
    {
       
    }
    */
}

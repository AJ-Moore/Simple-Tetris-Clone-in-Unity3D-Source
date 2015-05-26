using UnityEngine;
using System.Collections;

public class Tetris : MonoBehaviour {
	
	
	public Vector2 gridSize = new Vector2(10,40);
	public Transform gridBlock; //The blocks that make up the grid. 
	
	public Transform shapeBlock; // The blocks that are used to build up the shape. 

    //all the shapes, use prefabs so we can set up different materials and stuff for each one. 
    public Transform LShape;
    public Transform TShape;
    public Transform JShape;
    public Transform LineShape;
    public Transform SquareShape;
    public Transform SShape;
    public Transform ZShape; 

	public Transform text3dPoints; //shows how many points were made. 

    public AudioClip blockPlacedSound; 
	
	public float dropSpeedMs = 500.0f;//the speed at which the shapes drop.
	float dropSpeed;//the current dropspeed. 

    public Transform scoreBoard; 

	#region Shapes 	
	int[,] lShape = new int[4,4]{
		{0,1,0,0},
		{0,1,0,0},
		{0,1,1,0},
		{0,0,0,0}
	};
	
	int[,] jShape = new int[4,4]{
		{0,0,1,0},
		{0,0,1,0},
		{0,1,1,0},
		{0,0,0,0}
	};
	
	int[,] lineShape = new int[4,4]{
		{0,1,0,0},
		{0,1,0,0},
		{0,1,0,0},
		{0,1,0,0}
	};
	
	int[,] sShape = new int[4,4]{
		{0,0,0,0},
		{0,1,1,0},
		{1,1,0,0},
		{0,0,0,0}
	};
	
	int[,] zShape = new int[4,4]{
		{0,0,0,0},
		{1,1,0,0},
		{0,1,1,0},
		{0,0,0,0}
	};
	int[,] squareShape = new int[4,4]{
		{0,0,0,0},
		{0,1,1,0},
		{0,1,1,0},
		{0,0,0,0}
	};
	int[,] tShape = new int[4,4]{
		{0,0,0,0},
		{0,0,0,0},
		{0,1,1,1},
		{0,0,1,0}
	};
	#endregion 
	
	
	//store refernce to blocks being droped 
	Transform[] currentTransforms = new Transform[4];
	//store the current shape.
	int[,] currentShape; 
	Transform[,] gridTransforms;//holds all transforms in the grid 
	bool[,] grid;//holds whether the grid position is empty or not. 
	
	//the current position of the piece in the grid 
	Vector2 gridPosition = new Vector2(0,0); 
	
	bool blockDropping = false;//are there any blocks being dropped 
	
	
	// Use this for initialization
	void Start () {
		
		print("Initialising game"); 
		
		//build a grid out of blocks as big as the gridsize
		buildTetrisGrid(); 
		
		//set up arrays 
		gridTransforms = new Transform[(int)gridSize.x,(int)gridSize.y];
		grid = new bool[(int)gridSize.x,(int)gridSize.y];
		
		//set the start drop speed 
		dropSpeed = dropSpeedMs; 
		
		//should pause the game hopefully--->> epic  testing 
		//Time.timeScale = 0; 
	
	}
	
	
	
	// Update is called once per frame
	void Update () {
	
		//if block is dropping move block down 1 else create a new shape/block  
		if (blockDropping)
		{
			//drops the shape down a square.
			dropDown(); 
			//checks the input devices 
			checkInput();
		}
		else
		{
			blockDropping = true; 
			createRandomBlock();//creates a random block
		}
		
		//checks to see if a row is full of blocks 
		checkRowComplete();

        //updates the 3d text that display the points. 
        pointsUpdate();
		
	}
	
    //checks through the whole grid for rows that are complete. 
	void checkRowComplete()
	{
		
		bool[] rowComplete = new bool[(int)gridSize.y]; //true until proven false below...
		
		for(int i = 0;i<(int)gridSize.y; i++)
		{
			rowComplete[i] = true;	
		}
		
		//check through each row 
		for (int i =0; i<gridSize.y;i++)
		{
			//then check each column in that row 
			for (int p = 0; p<gridSize.x; p++)
			{
				//if any are null then this row is set to false 
				if(gridTransforms[p,i]== null)
				{
					rowComplete[i] = false;//then the row isnt complete 	
				}
			}
			
		}

        int lineCount = 0;//counts completed lines 

		//delete full rows 
		//check through each row 
		for (int i =0; i<gridSize.y;i++)
		{
			if (rowComplete[i])
			{
				//then remove all objects in this row
				for (int p = 0; p<gridSize.x; p++)
				{
                    lineCount++; 

					//destroys each row object. 
					Destroy(gridTransforms[p,i].gameObject);
			
                    //move all rows above down 1 if there is an object in the grid position.
                    for (int rowAbove = i - 1; rowAbove > 0; rowAbove--)
                    {
                        if (gridTransforms[p, rowAbove] != null)
                        {
                            gridTransforms[p, rowAbove].position -= new Vector3(0, 1, 0);
                            //switch the grid positions and then remove old refernce.
                            gridTransforms[p, rowAbove + 1] = gridTransforms[p, rowAbove];
                            //remove refernce
                            gridTransforms[p, rowAbove] = null; 

                        }
                    }
					
				}				

				
			}
			
		}

        //display Score 

        if (lineCount != 0)
        {
            //Add Points play Sounds. 
            addPoints(new Vector3(transform.position.x + ((gridSize.x / 2) * gridBlock.localScale.x), transform.position.y + ((gridSize.y / 2) * gridBlock.localScale.y), transform.position.z), 600 * lineCount);

        }

		
		
	}
	


	//create a block and stores the refernce to the prefabs used in order to change the position as the block drops
	void createRandomBlock()
	{
		
		int randomSelection = Random.Range(0,7);
		
		//select the block 
		switch (randomSelection)
		{
		case 0:
			currentShape = lShape;
            shapeBlock = LShape; 
			break; 
		case 1:
			currentShape = jShape;
            shapeBlock = JShape; 
			break; 
		case 2:
			currentShape = lineShape;
            shapeBlock = LineShape; 
			break; 
		case 3:
			currentShape = sShape;
            shapeBlock = SShape; 
			break; 
		case 4:
			currentShape = zShape;
            shapeBlock = ZShape; 
			break; 
		case 5:
			currentShape = squareShape;
            shapeBlock = SquareShape; 
			break; 
		case 6:
			currentShape = tShape;
            shapeBlock = TShape; 
			break; 
		}
		
		
		gridPosition = new Vector2(0,-1); 
		//check if the top row of the array is empty if it is we want the shape to start a row up 
		for (int i = 0; i<4; i++)
		{
			if(currentShape[0,i]!=0)
			{
				gridPosition = new Vector2(0,0);
				break; 
			}
		}
		
		//set the xposition to a centerish position 
		gridPosition.x = ((int)(gridSize.x/2))-2;

        //Check if shape will collide with anything if so gameover. 
        if (checkShapeCollision(currentShape))
        {
            //Gameover 
            //clear grid 
            foreach(Transform transform in gridTransforms)
            {
                if (transform != null)
                    Destroy(transform.gameObject);

                scoreBoard.GetComponent<Score>().totalScore = 0; 
            }
        }
			
			int blockCount = 0;
			//loop through current shape and find the first position represented by a value !=0 
			for (int xpos = 0; xpos<4; xpos++)
			{
				for (int ypos = 0; ypos<4;ypos++)
				{
					if(currentShape[ypos,xpos]!= 0)
					{
						//the position of 0,0 on the grid top right corner
						Vector3 startPosition = new Vector3((float)(this.transform.position.x+(0.5f)),this.transform.position.y+(gridSize.y * gridBlock.transform.localScale.y)-(gridBlock.transform.localScale.y/2),this.transform.position.z);
					
						float gridOffsetX = gridBlock.transform.localScale.x*(xpos + gridPosition.x);
						float gridOffsetY = gridBlock.transform.localScale.y*(ypos + gridPosition.y);
					
						Vector3 blockPosition = startPosition + new Vector3(gridOffsetX,-gridOffsetY,0);
					
						currentTransforms[blockCount] = Instantiate(shapeBlock, blockPosition, Quaternion.identity) as Transform;
						//currentTransforms[blockCount].gameObject.renderer.material = currentMaterial; 
						
						blockCount++;
					}
				}
			}

		}
	
	
	//makes an instance of the block passed in to create the grid surrounding 
	void buildTetrisGrid()
	{
		Vector3 blockPosition = new Vector3(0,0,0); 
		
		//top & bottom of the grid. 
		for (int p = 0; p<2;p++)
		{
			for (int i = 0; i < gridSize.x + 2; i++)
			{
				float posX = (this.transform.position.x - (gridBlock.transform.localScale.x/2))+(gridBlock.transform.localScale.x*i);
				float posY = (this.transform.position.y - (gridBlock.transform.localScale.y/2))+(p*(gridBlock.transform.localScale.y*(gridSize.y+1)));
				blockPosition = new Vector3(posX,posY,this.transform.position.z);
				Instantiate(gridBlock, blockPosition, Quaternion.identity);
			}
		}
		
		//left and side of grid 
		for (int p = 0; p<2;p++)
		{
			for (int i = 0; i < gridSize.y; i++)
			{
				float posX = (this.transform.position.x - (gridBlock.transform.localScale.x/2))+(p*(gridBlock.transform.localScale.x*(gridSize.x+1)));
				float posY = (this.transform.position.y + (gridBlock.transform.localScale.y/2))+(gridBlock.transform.localScale.y*i);;
				blockPosition = new Vector3(posX,posY,this.transform.position.z);
				Instantiate(gridBlock, blockPosition, Quaternion.identity);
			}
		}
		
	}
	
	
	float timePassed;//holds how much time has passed  
	
	
	void dropDown()
	{
		//check if anything is below else move down a line 
		
		//wait for defined period between drops. 
		timePassed+= Time.deltaTime*1000; 
		if (timePassed > dropSpeed)
		{
			//print("500 ms passed"); 
			timePassed = 0; 
			
			//work out if there is something below? 
			bool emptyBelow = true; 
			//go through each block and check, if just one has something below then we cant move down
			for (int xpos = 0; xpos<4; xpos++)
			{
				for (int ypos = 0; ypos<4;ypos++)
				{
					if(currentShape[ypos,xpos]!= 0)
					{
						int gridPosX = (int)(gridPosition.x + xpos);
						int gridPosY = (int)(gridPosition.y + ypos); 
		
						if (gridPosY == gridSize.y-1)
						{
							emptyBelow = false;
							break; 
						}	
						if (gridTransforms[gridPosX,(gridPosY+1)] != null && gridPosY < gridSize.y)
						{
							emptyBelow = false;
							break; 
						}
					}
				}
			}
					
			if (emptyBelow)
			{
				gridPosition.y++;//minus 1 from the grid position 
				updatePosition();//updates the position of the current shape 
			}
			else 
			{
				//store the position of the blocks into the array. 
				int blockCount = 0; 
				for (int xpos = 0; xpos<4; xpos++)
				{
					for (int ypos = 0; ypos<4;ypos++)
					{
						if(currentShape[ypos,xpos]!= 0)
						{
							int gridPosX = (int)(gridPosition.x + xpos);
							int gridPosY = (int)(gridPosition.y + ypos); 
							
							gridTransforms[gridPosX,gridPosY] = currentTransforms[blockCount];
							blockCount++;
						}
					}
				}
				
				blockDropping = false;//will cause a new block to drown when update is called 	
				
				//Play sound for when a block is placed 
				
				//add score /display score 
                float _xpos = this.transform.position.x+(gridPosition.x+2 * gridBlock.localScale.x); 
                float _ypos = this.transform.position.y+((gridSize.y-(gridPosition.y-1)) * gridBlock.localScale.y); 
                float _zpos = this.transform.position.z;
                addPoints(new Vector3(_xpos, _ypos, _zpos),300); 
				
                //Play Sound 
                this.gameObject.GetComponent<AudioSource>().PlayOneShot(blockPlacedSound, 10);

				
			}
			
		}
		
		
	}
	
	//1 clockwise, 0 anti-clockwise
	void RotateShape(int direction)
	{
		//rotate clockwise 
		if (direction == 1)
		{
			//first check if the shape is able to be rotated in its current position 
			//rotate the array then check if that array would colliding with anything 
			
			int[,] rotatedShape = new int[4,4]; 
			
            //rotates the shape. 
			for (int x = 0; x<4; x++)
			{
				for (int y = 0; y<4; y++)
				{
					rotatedShape[x,y] = currentShape[3-y,x];
				
				}	
			}


            //Check if rotated shape collides with any other blocks when rotated
            if (!checkShapeCollision(rotatedShape))
            {
                currentShape = rotatedShape;
                updatePosition();
            }
			
			
		}
		else//rotate anticlockwise.  
		{
			
		}
		
		
	}
	
    //whether the shape is overlapping anything in the grid or the wall.  
    //precheck, true when overlapping
    bool checkShapeCollision(int[,] _shape)
    {
        bool returnValue = false; 
        for (int xpos = 0; xpos < 4; xpos++)
        {
            for (int ypos = 0; ypos < 4; ypos++)
            {
                if (_shape[ypos, xpos] != 0)
                {
                    int gridPosX = (int)gridPosition.x + xpos; 
                    int gridPosY = (int)gridPosition.y + ypos; 

                    //if the block is outside of the grid 
                    if (gridPosX > gridSize.x || gridPosX < 0 || gridPosY > gridSize.y || gridPosY < 0)
                    {
                        //out of grid bounds 
                        returnValue = true;//collision 
                        break; 
                    }

                    //if just one block would overlap on the grid return true 
                    if (gridTransforms[gridPosX, gridPosY] != null)
                    {
                        //overlay detected 
                        returnValue = true;//collision 
                        break; 
                    }

                }
            }
        }

        return returnValue;

    }
	
	//updates the position of the current object in the grid using the grid position. 
	//basically repositions the objects based on the currentShape Array + grid Position. 
	void updatePosition()
	{
		int blockCount = 0; 
			//loop through current shape and find the first position represented by a value !=0 
			for (int xpos = 0; xpos<4; xpos++)
			{
				for (int ypos = 0; ypos<4;ypos++)
				{
					if(currentShape[ypos,xpos]!= 0)
					{
						//The position in the grid 
						//gridPosX = gridPosition.x + xpos; 
						//gridPosY = gridPosition.y + ypos; 
					
						//the position of 0,0 on the grid top right corner
						Vector3 startPosition = new Vector3((float)(this.transform.position.x+(0.5f)),this.transform.position.y+(gridSize.y * gridBlock.transform.localScale.y)-(gridBlock.transform.localScale.y/2),this.transform.position.z);
					
						float gridOffsetX = gridBlock.transform.localScale.x*(xpos + gridPosition.x);
						float gridOffsetY = gridBlock.transform.localScale.y*(ypos + gridPosition.y);
					
						Vector3 blockPosition = startPosition + new Vector3(gridOffsetX,-gridOffsetY,0);
					
						currentTransforms[blockCount].position = blockPosition;//set to new position. 
						blockCount++;
					}
				}
			}		
		
	}
	
	float inputTimePassed = 0; 
	//checks the input 
	void checkInput()
	{
		inputTimePassed += Time.deltaTime*1000; 
		if (inputTimePassed > 150)
		{
			inputTimePassed = 0; 
			if (Input.GetAxis("Horizontal")!= 0)
			{
				//check theres nothing to the sides before moving. 
				bool emptyLeft = true; 
				bool emptyRight = true; 
				//go through each block and check, if just one has something below then we cant move down
				for (int xpos = 0; xpos<4; xpos++)
				{
					for (int ypos = 0; ypos<4;ypos++)
					{
						if(currentShape[ypos,xpos]!= 0)
						{
							int gridPosX = (int)(gridPosition.x + xpos);
							int gridPosY = (int)(gridPosition.y + ypos); 
		
							//check if anything to the left
							if (gridPosX == 0)
							{
								emptyLeft = false;

							}	
							else if (gridTransforms[gridPosX-1,(gridPosY)] != null)
							{
								emptyLeft = false;
							}
							
							//check if anything to the right. 
							if (gridPosX == gridSize.x-1)
							{
								emptyRight = false;

							}	
							else if (gridTransforms[gridPosX+1,(gridPosY)] != null)
							{
								emptyRight = false;
							}
							
						}
					}
				}
				
				
				if (Input.GetAxis("Horizontal")<0 && emptyLeft)
					gridPosition.x--;
				
				if (Input.GetAxis("Horizontal")>0 && emptyRight)
					gridPosition.x++;

				updatePosition();
				
				
			}
			
			
			if (Input.GetAxis("Jump")!= 0)
			{
				RotateShape(1); 
			}

			
		}	//end of delayed period 		

		//if down is pressed.
		if (Input.GetAxis("Vertical") <=0)
			dropSpeed = dropSpeedMs - (Input.GetAxis("Vertical") * -470);
		
	}
	

    //Hold Point Objects so they can be updated 
    Transform[] pointObject = new Transform[10];//can only have 10 at one time which should be enough
    Vector3[] pointTargetPos = new Vector3[10];
    int pointCount = 0;//keeps track of objects. 

    //visuale shows points earnt and adds to the score. 
    //Position to place 3d text, Points earnt. 
    void addPoints(Vector3 Position, int Points)
    {
        //Add points to total score
        scoreBoard.gameObject.GetComponent<Score>().totalScore += Points;

        if (pointObject[pointCount] != null)
            Destroy(pointObject[pointCount].gameObject);

        pointObject[pointCount] = Instantiate(text3dPoints, Position, Quaternion.identity) as Transform;

        pointTargetPos[pointCount] = new Vector3(pointObject[pointCount].position.x, pointObject[pointCount].position.y + 10, pointObject[pointCount].position.z);
        pointObject[pointCount].GetComponent<TextMesh>().text = Points.ToString();
        pointObject[pointCount].position = Vector3.Lerp(pointObject[pointCount].position, pointTargetPos[pointCount], Time.deltaTime * 10);

        pointCount++;
        if (pointCount >= 10)
        {
            pointCount = 0; 
        }
    }

    //generally used so the text will rise then dissappear after it is shown. 
    void pointsUpdate()
    {
        int count = 0; 
        foreach (Transform obj in pointObject)
        {
            if (obj != null)
            {
                obj.position = Vector3.Lerp(obj.position, pointTargetPos[count], Time.deltaTime);
                if (obj.position.y > (pointTargetPos[count].y-1))//if its within 1/ reached the target destroy it. 
                {
                    Destroy(obj.gameObject);
                }
            }
            count++; 
        }
    }
	
}

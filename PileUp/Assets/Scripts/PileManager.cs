using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PileManager : MonoBehaviour 

{


	private GameObject[] Pile;

	private int combo = 0;

	private float cubeTransition = 0.0f;

	private Vector2 PileBounds = new Vector2 (BOUNDS_SIZE, BOUNDS_SIZE);

	private int scoreCount = 0;



	public Material PileMat;

	public GameObject endPanel;

	public Color32[] cubeColors = new Color32[4];

	public Text gameScore;


	private const float BOUNDS_SIZE = 3.5f;

	private const float STACK_BOUNDS_GAIN = 0.25f;

	private const float PILE_MOVING_SPEED = 5.0f;

	private const float ERROR_MARGIN = 0.1f;

	private const int COMBO_START_GAIN = 3;


	private float a, b, c, d = 0.0f;

	private int pileIndex;


	private float cubeSpeed = 2.5f;

	private Vector3 desiredPosition;

	private bool gameOver = false; 

	private Vector3 lastCubePosition;

	private bool isMovingoOnX = true;

	private float secondaryPosition;


	// Use this for initialization
	void Start () 
	{
		//Get all the piles in an Array
		Pile = new GameObject[transform.childCount];
		for (int i = 0; i < transform.childCount; i++) 
		{
			Pile [i] = transform.GetChild (i).gameObject;
			ColorMesh (Pile[i].GetComponent<MeshFilter>().mesh); // Apply colors to all the piles
		}

		//Decrement the pileIndex to move the piles
		pileIndex = transform.childCount - 1;


	}

	//Creating the waste material which will fall down.
	private void createWaste (Vector3 pos, Vector3 scale)
	{
		
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;

		//Adding Rigidbody so that it can fall down.
		go.AddComponent<Rigidbody> ();

		//Adding Colors
		go.GetComponent<MeshRenderer> ().material = PileMat;
		ColorMesh(go.GetComponent<MeshFilter> ().mesh);
	}

	// Update is called once per frame
	void Update () 
	{

		if (gameOver)
			return;

		//When user touches the screen
		if (Input.GetMouseButtonDown (0)) 
		{

			//When Cube is Piled up correctly
			if (PileUp ()) 
			{
				SpawnTile ();
				scoreCount++;
				gameScore.text = scoreCount.ToString ();
			} 
			//When Cube failed to Pile Up.
			else 
			{
				EndGame ();
			}

		}

		MoveTile ();

		//Move the Pile
		transform.position = Vector3.Lerp(transform.position, desiredPosition, PILE_MOVING_SPEED * Time.deltaTime);



	}

	private void MoveTile()
	{
		cubeTransition += Time.deltaTime * cubeSpeed;

		//Moving the Cube smoothly in X direction
		if (isMovingoOnX) 
		{
			Pile [pileIndex].transform.localPosition = new Vector3 (Mathf.Sin (cubeTransition) * BOUNDS_SIZE, scoreCount, secondaryPosition);
		} 

		//Moving the Cube smoothly in Y direction
		else 
		{
			Pile [pileIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (cubeTransition) * BOUNDS_SIZE);
		}

	}

	//Creating the new Cubes from existing ones.
	private void SpawnTile()
	{
		lastCubePosition = Pile [pileIndex].transform.localPosition;
		pileIndex--;

		if (pileIndex < 0) 
		{
			pileIndex = transform.childCount - 1;
		}

		desiredPosition = (Vector3.down) * scoreCount;
		Pile [pileIndex].transform.localPosition = new Vector3 (0, scoreCount, 0);
		Pile [pileIndex].transform.localScale = new Vector3 (PileBounds.x, 1, PileBounds.y);

		ColorMesh(Pile[pileIndex].GetComponent<MeshFilter> ().mesh);

	}

	private bool PileUp()
	{
		Transform t = Pile [pileIndex].transform;

		//When Cube is moving in X direction
		if (isMovingoOnX) 
		{
			float deltaX = lastCubePosition.x - t.position.x;

			//If the Cube does not piles up correctly 
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) 
			{
				//Cut the Tile
				combo = 0;
				PileBounds.x -= Mathf.Abs (deltaX);
				if (PileBounds.x <= 0) 
				{
					return false;
				}

				float middle = lastCubePosition.x + t.localPosition.x / 2;
				t.localScale = new Vector3 (PileBounds.x, 1, PileBounds.y);

				//Creating the waste material.
				if (t.position.x > 0) 
				{
					a = t.position.x + (t.localScale.x / 2);
					createWaste (new Vector3 (a, t.position.y, t.position.z), new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z));
				} 

				else 
				{
					b = t.position.x - (t.localScale.x / 2);
					createWaste(new Vector3 (b, t.position.y, t.position.z), new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z));
				}



				t.localPosition = new Vector3 (middle - (lastCubePosition.x / 2), scoreCount, lastCubePosition.z);

			} 

			//When the Cube piles up - For adjusting the pile so that error margin can not be seen in X direction.
			else 
			{
				if (combo > COMBO_START_GAIN) 
				{
					PileBounds.x += STACK_BOUNDS_GAIN;
					if (PileBounds.x > BOUNDS_SIZE) 
					{
						PileBounds.x = BOUNDS_SIZE;
					}


					float middle = lastCubePosition.x + t.localPosition.x / 2;
					t.localScale = new Vector3 (PileBounds.x, 1, PileBounds.y);
					t.localPosition = new Vector3 (middle - (lastCubePosition.x / 2), scoreCount, lastCubePosition.z);

				}

				combo++;
				t.localPosition = new Vector3 (lastCubePosition.x, scoreCount, lastCubePosition.z);
			}
		} 


		//When Cube is moving in Z direction
		else 
		{
			float deltaZ = lastCubePosition.z - t.position.z;

			//If the Cube does not piles up correctly 
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) 
			{
				//Cut the Tile
				combo = 0;
				PileBounds.y -= Mathf.Abs (deltaZ);
				if (PileBounds.y <= 0) {
					return false;
				}

				float middle = lastCubePosition.z + t.localPosition.z / 2;
				t.localScale = new Vector3 (PileBounds.x, 1, PileBounds.y);

				//Creating the waste material.
				if (t.position.z > 0) 
				{
					c = t.position.z + (t.localScale.z / 2);
					createWaste (new Vector3 (t.position.x, t.position.y, c), new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ)));
				} 

				else 
				{
					d = t.position.z - (t.localScale.z / 2);
					createWaste(new Vector3 (t.position.x, t.position.y, d), new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ)));
				}

			
				t.localPosition = new Vector3 (lastCubePosition.x, scoreCount, middle - (lastCubePosition.z / 2));

			}

			//When the Cube piles up - For adjusting the pile so that error margin can not be seen in Z direction.
			else 
			{
				if (combo > COMBO_START_GAIN) 
				{
					PileBounds.y += STACK_BOUNDS_GAIN;

					if (PileBounds.y > BOUNDS_SIZE) 
					{
						PileBounds.y = BOUNDS_SIZE;
					}

					float middle = lastCubePosition.z + t.localPosition.z / 2;
					t.localScale = new Vector3 (PileBounds.x, 1, PileBounds.y);
					t.localPosition = new Vector3 (lastCubePosition.x, scoreCount, middle - (lastCubePosition.z / 2));

				}

				combo++;
				t.localPosition = new Vector3 (lastCubePosition.x, scoreCount, lastCubePosition.z);
			}
		}

		//For Moving the Tile in X and Z direction.
		if (isMovingoOnX)
		{
			secondaryPosition = t.localPosition.x;
		}
		else
		{
			secondaryPosition = t.localPosition.z;
		}

		//After each Moving on X, the next Cube is in Z direction.
		isMovingoOnX = !isMovingoOnX;

		return true;
	}

	//For Coloring the Cubes with 4 different Colors
	private void ColorMesh(Mesh mesh)
	{ 
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);

		for(int i = 0; i < vertices.Length; i++)
		{
			colors[i] = Lerp4(cubeColors[0], cubeColors[1], cubeColors[2], cubeColors[3], f);
		}

		mesh.colors32 = colors;


	}

	//Smootly transitioning of 4 different Colors
	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d, float t)
	{
		if(t < 0.33)
		{
			return Color.Lerp(a, b, t / 0.33f);
		}
		else if (t < 0.66f)
		{
			return Color.Lerp(b, c, (t - 0.33f) / 0.33f);
		}
		else
		{
			return Color.Lerp(c, d, (t - 0.66f) / 0.66f);
		}
	}

	//Ending the Game - Setting the Final Score and Falling of Final Cube which Failed.
	private void EndGame()
	{
		if (PlayerPrefs.GetInt ("score") < scoreCount) 
		{
			PlayerPrefs.SetInt ("score" , scoreCount);
		}

		gameOver = true;
		endPanel.SetActive (true);
		Pile [pileIndex].AddComponent<Rigidbody> ();
	}

	//For Restart and Main Menu.
	public void OnButtonClick(string sceneName)
	{
		SceneManager.LoadScene (sceneName);
	}
	
}

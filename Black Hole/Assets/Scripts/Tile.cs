using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class Tile : MonoBehaviour {

	public Tile[] neighbours;
	public int counterValue = -1; // -1 means no tile placed here yet
	public Sprite placedImage;
	public Text valueText;
	public string team;
	public Color redColour;
	public Color blueColour;
	public Color neutralColour;
	public Text redScoreText;
	public Text blueScoreText;
	public GameObject scoreParent;
	public Counter counter;
	public Button newGameButton;
	public Image winStar;
	
	private GameManager gameManager;
	private Image thisImage;



	void Start () {
		thisImage = GetComponent<Image>();
		gameManager = GameObject.Find("SCRIPTS").GetComponent<GameManager>();
		ResetTile();
	}
	
	
	public void ResetTile() {
		team = "Neutral";
		counterValue = -1;
	}


	public void PlaceTile() {
		if (team != "Neutral" || counterValue != -1) {
			//Debug.Log("Can't place a counter on a non-neutral tile!", this);
			return;
		}

		team = gameManager.activeCounter.team;
		counterValue = gameManager.activeCounter.counterValue;
		gameManager.activeCounter.state = CounterState.Board;
		counter = gameManager.activeCounter;
		
		// move the mouse cursor to the middle of the screen so the placed counter isn't covered by the held one
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.lockState = CursorLockMode.None;
		
		// visual effects
		gameManager.activeCounter.transform.DOScale(Vector2.one, 0.5f);
		gameManager.activeCounter.transform.SetParent(this.transform);
		gameManager.activeCounter.transform.DOLocalMove(Vector3.zero, 0.5f);

		gameManager.EndTurn ();
	}


	public void TriggerBlackhole(out int redScore, out int blueScore) {
		redScore = 0;
		blueScore = 0;
		
		int redTileCount = 0;
		int blueTileCount = 0;

		// count score and num of tiles
		foreach (var neighbour in neighbours) {
			if (neighbour.team == "Red") {
				redScore += neighbour.counterValue;
				redTileCount ++;
			} else if (neighbour.team == "Blue") {
				blueScore += neighbour.counterValue;
				blueTileCount ++;
			} else {
				Debug.LogError ("Invalid team " + neighbour.team, neighbour.gameObject);
			}
		}
		
		string higherTeam = (redScore > blueScore) ? "Red" : "Blue";
		string lowerTeam  = (redScore > blueScore) ? "Blue" : "Red";
		int higherScore   = (redScore > blueScore) ? redScore : blueScore;
		int lowerScore    = (redScore > blueScore) ? blueScore : redScore;
		int higherCount   = (redScore > blueScore) ? redTileCount : blueTileCount;
		int lowerCount    = (redScore > blueScore) ? blueTileCount : redTileCount;
		
		float suckDelay = 0.3f;
		float suckDuration = 0.5f;
		
		int higherRunningCount = 0;
		int lowerRunningCount = 0;
		
		// do visual effects
		// lower score team tiles are sucked individually first, then higher score team tiles
		foreach (var neighbour in neighbours) {
			if (neighbour.team == higherTeam) {
				StartCoroutine(SuckTileAfterDelay(neighbour, suckDelay*higherRunningCount + lowerCount*suckDelay*2, suckDuration));
				higherRunningCount++;
			} else if (neighbour.team == lowerTeam) {
				StartCoroutine(SuckTileAfterDelay(neighbour, suckDelay * lowerRunningCount, suckDuration));
				lowerRunningCount++;
			}
		}
		
		// display score texts
		scoreParent.transform.localScale = Vector2.zero;
		scoreParent.transform.DOScale(Vector2.one, 0.05f);
		scoreParent.SetActive(true);
		redScoreText.text = "0";
		blueScoreText.text = "0";
		winStar.gameObject.SetActive(false);
		
		Text winnerScoreText = (redScore > blueScore) ? blueScoreText : redScoreText;

		StartCoroutine(DoAfterScoringAnimationDelay(suckDelay*higherCount + lowerCount*suckDelay*2 + 1f, 0.5f, winnerScoreText));

		// todo particles? screenshake?
	}
	
	IEnumerator SuckTileAfterDelay(Tile tile, float delay, float duration) {
		yield return new WaitForSeconds(delay);
		tile.counter.transform.DOMove(transform.position, duration);
		tile.counter.transform.DOScale(Vector2.zero, duration);
		tile.counter.transform.DOShakeRotation(duration, 50, 10, 90);
		
		AddToScoreDisplay(tile.counterValue, tile.team);
	}
	
	void AddToScoreDisplay(int amountToAdd, string team) {
		Text scoreText = (team == "Red") ? redScoreText : blueScoreText;
		scoreText.text = (int.Parse(scoreText.text) +  amountToAdd).ToString();
		scoreText.transform.localScale = Vector2.one;
		scoreText.transform.DOPunchScale(Vector2.one * 1.3f, 0.2f);
		scoreText.transform.DOShakeRotation(0.2f, 50, 10, 90);
	}
	
	IEnumerator DoAfterScoringAnimationDelay(float delay, float newGameButtonDelay, Text winnerScoreText) {
		yield return new WaitForSeconds(delay);
		
		// display the winner star on the winner's score
		winStar.gameObject.SetActive(true);
		winStar.transform.position = new Vector2(winnerScoreText.transform.position.x, winStar.transform.position.y);
		winStar.transform.localScale = Vector2.zero;
		winStar.transform.DOScale(Vector2.one, 0.7f);
		winStar.transform.DOPunchRotation(new Vector3(0f, 0f, 30f), 0.7f);
		winnerScoreText.transform.DOPunchScale(Vector2.one * 1.2f, 0.8f);
		
		yield return new WaitForSeconds(newGameButtonDelay);
		
		newGameButton.gameObject.SetActive(true);
		newGameButton.transform.localScale = Vector2.zero;
		newGameButton.transform.DOScale(Vector2.one, 0.2f);
	}
}

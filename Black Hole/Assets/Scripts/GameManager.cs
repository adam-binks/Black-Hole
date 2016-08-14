using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class GameManager : MonoBehaviour {

	[HideInInspector]
	public string turn;
	public Counter activeCounter; // counter that is about to be placed
	public int numCounters; // per player
	public GameObject counterPrefab;
	public GameObject redStack;
	public GameObject blueStack;
	public int stackPadding; // space between each counter in the stack
	public Button newGameButton;
	public GameObject scoreParent;

	private Counter[] counters;
	public Tile[] tiles;



	void Start () {
		StartGame ();	
	}


	void StartGame() {
		counters = new Counter[numCounters * 2]; // enough for both players
		// count down from numCounters, creating a red and blue counter for each value and add it to the stacks
		for (int i = numCounters; i > 0; i--) {
			counters [i - 1] = GenerateCounter ("Red", i);
			counters [numCounters + i - 1] = GenerateCounter ("Blue", i);
		}
		// animate in
		foreach (Counter counter in counters) {
			counter.transform.localScale = Vector2.zero;
			counter.transform.DOScale(Vector2.one, 0.2f);	
		}
		
		StartFirstTurn ();
	}


	Counter GenerateCounter(string team, int value) {
		GameObject counterGO = Instantiate (counterPrefab);
		Counter counter = counterGO.GetComponent<Counter> ();
		counter.Setup (team, value);

		if (team == "Red") {
			counterGO.transform.SetParent(redStack.transform, false);
		} else {
			counterGO.transform.SetParent(blueStack.transform, false);
		}
		counterGO.GetComponent<RectTransform>().localPosition = new Vector2(0, (numCounters - value) * stackPadding);
		return counter;
	}


	void StartFirstTurn() {
		// flip a coin to decide who goes first
		turn = (Random.Range (0, 2) == 0) ? "Red" : "Blue";
		EndTurn ();
	}


	public void EndTurn() {
		if (CheckForEndOfGame ()) {
			EndGame ();
			return;
		} else {
			// otherwise play continues as usual
			if (turn == "Red") {
				turn = "Blue";
			} else {
				turn = "Red";
			}

			if (activeCounter != null) {
				// put the current active counter on the board
				activeCounter.state = CounterState.Board;
			}

			// set the smallest counter in a stack that is of [turn]'s colour as the new activeCounter
			foreach (var counter in counters) {
				if (counter.team == turn && counter.state == CounterState.Stack) {
					activeCounter = counter;
					activeCounter.state = CounterState.Hand;
					activeCounter.transform.DOScale(Vector3.one * 1.1f, 0.1f);
					break;
				}
			}

			// todo turn alert/colouration? change cursor?
		}
	}
		

	/// <returns><c>true</c>, if all counters have been placed, <c>false</c> otherwise.</returns>
	bool CheckForEndOfGame() {
		foreach (var counter in counters) {
			if (counter.state != CounterState.Board) {
				return false;
			}
		}
		return true;
	}


	void EndGame() {
		foreach (Tile tile in tiles) {
			if (tile.counterValue == -1) { // this is the empty tile
				int redScore = 0;
				int blueScore = 0;
				tile.TriggerBlackhole(out redScore, out blueScore);
				return;
			}
		}
	}
	
	public void NewGameButtonPress() {
		StartCoroutine(PrepForNewGame());
	}
	
	IEnumerator PrepForNewGame() {
		// start animations
		float animationTime = 0.5f;
		
		newGameButton.transform.DOScale(Vector2.zero, 0.2f);
		
		foreach (Counter counter in counters) {
			counter.transform.DOScale(Vector3.zero, animationTime);
		}
		
		foreach (Tile tile in tiles) {
			tile.ResetTile();
		}
		
		scoreParent.transform.DOScale(Vector2.zero, animationTime * 0.5f);
		
		yield return new WaitForSeconds(animationTime);
		// actually start the new game
		StartGame();
		// get rid of UI stuff
		newGameButton.gameObject.SetActive(false);
		scoreParent.transform.localScale = Vector2.one;
		scoreParent.SetActive(false);
	}
}

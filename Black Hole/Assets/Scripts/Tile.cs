using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tile : MonoBehaviour {

	public Tile[] neighbours;
	public int value = -1; // -1 means no tile placed here yet
	public Sprite placedImage;
	public Text valueText;
	public string team;
	public Color redColour;
	public Color blueColour;
	public Color neutralColour;
	public GameManager gameManager;

	private Image thisImage;



	void Start () {
		thisImage = GetComponent<Image> ();
		team = "Neutral";
	}


	public void PlaceTile() {
		if (team != "Neutral" || value != -1) {
			Debug.LogError ("Can't place a counter on a non-neutral tile!", this);
			return;
		}

		team = gameManager.activeCounter.team;
		value = gameManager.activeCounter.value;
		valueText.text = value.ToString();
		if (team == "Red") {
			thisImage.color = redColour;
		} else if (team == "Blue") {
			thisImage.color = blueColour;
		} else {
			Debug.LogError ("Invalid team: " + team, this);
		}

		gameManager.EndTurn ();
	}


	public void TriggerBlackhole(out int redScore, out int blueScore) {
		redScore = 0;
		blueScore = 0;

		foreach (var neighbour in neighbours) {
			if (neighbour.team == "Red") {
				redScore += neighbour.value;
			} else if (neighbour.team == "Blue") {
				blueScore += neighbour.value;
			} else {
				Debug.LogError ("Invalid team " + neighbour.team, neighbour.gameObject);
			}
		}

		// todo visual effects. scale, rotate AND position tweens? particles? screenshake?
	}
}

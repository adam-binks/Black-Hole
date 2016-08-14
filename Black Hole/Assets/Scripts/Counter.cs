using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public enum CounterState { Stack, Hand, Board }

public class Counter : MonoBehaviour {

	public string team;
	public int counterValue;
	public Color redColour;
	public Color blueColour;
	public CounterState state;

	private Image thisImage;


	public void Setup(string thisTeam, int thisValue) {
		thisImage = GetComponent<Image> ();

		team = thisTeam;
		counterValue = thisValue;
		GetComponentInChildren<Text>().text = counterValue.ToString();
		if (team == "Red") {
			thisImage.color = redColour;
		} else if (team == "Blue") {
			thisImage.color = blueColour;
		}
		state = CounterState.Stack;
	}

	void TransitionIn() {
		transform.localScale = Vector2.zero;
		transform.DOScale (Vector2.one, 0.1f);
	}
	
	void Update () {
		if (state == CounterState.Hand) {
			transform.position = Vector3.Lerp (transform.position, Input.mousePosition, 0.3f);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour {

	GameManager gameManager;

	public int startHealth = 1;		//The health at the start of the level
	private int health;			//The current health

	SpriteRenderer sprite;		//The sprite attached to this object

	void Start () {
		health = startHealth;											//Assign the health to be equal to the start health
		sprite = GetComponent<SpriteRenderer>();						//Get the sprite renderer on this object
		if(!sprite) sprite = GetComponentInChildren<SpriteRenderer>();
		gameManager = GameManager.instance;								//Get the GameManager instance
	}
	
	//Function to inflict a certain amount of damage on character
	public IEnumerator InflictDamage(int damage){	
		health -= damage;			//Reduce the health
		if(health <= 0) Kill();		//If the health is below zero, kill this character

		int flashes = 0;
		while(flashes < 5){	//Make the sprite flash 5 times with an interval of 0.1 seconds between each
			sprite.enabled = !sprite.enabled;
			yield return new WaitForSeconds(0.1f);
			flashes ++;
		}
		sprite.enabled = true;	//Ensure the sprite is enabled when the loop ends
	}

	//Public functioin to get health remaining
	public int GetHealth(){
		return health;
	}

	//Public function to directly set the health of this character
	public void SetHealth(int newHealth){
		health = newHealth;
		if(health <= 0) Kill();		//If the health is below zero, kill this character
	}

	void Kill(){
		//If this game object is the player, set the game to be over
		if(this.gameObject.tag == "Player") gameManager.GameOver(false, true);
		gameObject.SetActive(false);
	}
}

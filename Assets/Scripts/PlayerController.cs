using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Character {

	[Header("Basic Movement")]
	public float runSpeed;
	public float jumpVel;				//The running speed and jump velocity of the player
	public Vector2 regularBoxSize;
	public Vector2 regularBoxOffset;
	private float moveSpeed;

	[Header("Wall Jumping")]
	public Vector2 wallJump;
	public Vector2 wallLeap;
	public float wallSlideSpeed;

	[Header("Sliding")]
	public float slideSpeed;
	public float maxSlideTime;
	public Vector2 slideBoxSize;
	public Vector2 slideBoxOffset;
	private bool isSliding;

	[Header("Animations")]
	public Animator anim;
	public string idle;
	public string run;
	public string slide;
	public string jumpUp;
	public string jumpDown;
	public string wallHold;

	[Header("Input")]
	public KeyCode[] jumpButtons;
	public KeyCode[] slideButtons;

	GameManager gameManager;

	public static PlayerController instance;	//Create a singleton of the PlayerController so there is only one player
	new void Awake(){
		base.Awake();	//Call the Awake function from the base Character class
		moveSpeed = runSpeed;
		if(instance == null) instance = this;
		else Destroy(this.gameObject);
	}

	void Start(){
		gameManager = GameManager.instance;
	}

	new void Update(){
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));	//Get the raw input from the keyboard/controller

		Vector2 targetVel = Vector2.zero;
		//If the player is not running into a wall, set the target velocity
		if(!isSliding) {
			if((!physics.collision.left && input.x < 0) || (!physics.collision.right && input.x > 0)) targetVel.x = input.x * moveSpeed * Time.deltaTime;
		} else if(!physics.collision.left || !physics.collision.right) targetVel.x = moveSpeed * Time.deltaTime;
		//else if(physics.collision.left || physics.collision.right) targetVel.y = input.y * wallClimbSpeed * Time.deltaTime;
		vel.x = Mathf.MoveTowards(vel.x, targetVel.x, (runSpeed * Time.deltaTime) / 5.0f);	//Move the x velocity towards the target velocity
		if(vel.x != 0) sprite.flipX = vel.x < 0;	//Flip the sprite based on the velocity
		if(GetKeyPressed(jumpButtons)){
			if(physics.collision.below) Jump();	//Jump
			else{
				if((physics.collision.left && input.x < 0) || (physics.collision.right && input.x > 0)){
					vel.y = wallJump.y;
					vel.x = wallJump.x * -physics.GetDirection().x;
				} else if(physics.collision.left || physics.collision.right){
					vel.y = wallLeap.y;
					vel.x = wallLeap.x * -physics.GetDirection().x;
				}
			}
		}
		if(physics.collision.below && GetKeyPressed(slideButtons) && !isSliding) StartCoroutine(Slide());	
		if((physics.collision.left || physics.collision.right) && !physics.collision.below && vel.y <= 0) vel.y = -wallSlideSpeed * Time.deltaTime;
		if(!isSliding){
			if(physics.collision.below){
				box.size = regularBoxSize;
				box.offset = regularBoxOffset;
			}
			box.offset = new Vector2(physics.GetDirection().x * -Mathf.Abs(box.offset.x), box.offset.y);
		}
		if(physics.CheckCollision("Obstacle")) StartCoroutine(health.InflictDamage(1));
		PlayAnimation();
		vel *= gameManager.GetMultiplier();
		base.Update();	//Call the update function in the base class
	}

	private bool GetKeyPressed(KeyCode[] keys){
		foreach(KeyCode key in keys){
			if(Input.GetKeyDown(key)) return true;
		}
		return false;
	}

	private void PlayAnimation(){
			string animToPlay = idle;
			if(physics.collision.below){
				if(isSliding) animToPlay = slide;
				else animToPlay = vel.x == 0 ? idle : run;
			} else{
				if(physics.collision.right || physics.collision.left) animToPlay = wallHold;
				else if(isSliding) animToPlay = slide;
				else animToPlay = vel.y < 0 ? jumpDown : jumpUp;
			}
		anim.Play(animToPlay);
	}

	void Jump(){
		if(!isSliding) vel.y = jumpVel;
	}

	private IEnumerator Slide(){
		float slideTimer = maxSlideTime;
		isSliding = true;
		moveSpeed = slideSpeed * physics.GetDirection().x;
		while(slideTimer > 0 && isSliding && !physics.collision.left && !physics.collision.right){
			slideTimer -= Time.deltaTime * gameManager.GetMultiplier();
			box.offset = slideBoxOffset;
			box.size = slideBoxSize;
			yield return null;
		}
		isSliding = false;
		moveSpeed = runSpeed;
		box.offset = regularBoxOffset;
		box.size = regularBoxSize;
	}

	public bool IsSliding(){
		return isSliding;
	}

}

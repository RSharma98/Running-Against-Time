using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(PhysicsSystem), typeof(HealthManager))]
public class Character : MonoBehaviour {

	protected SpriteRenderer sprite;			//The sprite for this character

	protected float gravity = 1f, maxGravity;	//The gravity rate to apply each frame, and the maximum value the gravity should reach
	protected BoxCollider2D box;				//Get the box collider attached to this object
	protected Vector2 pos;						//Position
	protected Vector2 vel;						//Velocity

	protected bool verticalCollision, horizontalCollision;	//Booleans for whether the raycasts are colliding on a certain axis
	protected RaycastHit2D[] verticalRays, horizontalRays;	//An array of raycasts for each direction
	protected PhysicsSystem physics;						//The physics system for this character
	protected HealthManager health;							//The health manager for this character

	// Use this for initialization
	protected void Awake () {
		Physics2D.queriesStartInColliders = false;	//Prevent raycasts from detecting the collider of this object
		Physics2D.queriesHitTriggers = false;		//Prevent raycasts from interacting with trigger objects
		verticalRays = new RaycastHit2D[2];			//Initialise the vertical ray array
		horizontalRays = new RaycastHit2D[2];		//Initialise the horizontal ray array
		sprite = GetComponent<SpriteRenderer>();	//Get the sprite renderer on this character
		if(!sprite) sprite = GetComponentInChildren<SpriteRenderer>();	//If the sprite renderer is not on this object, check the child objects
		box = GetComponent<BoxCollider2D>();		//Get the box collider
		pos = transform.position;					//Get the position
		physics = GetComponent<PhysicsSystem>();	//Get the Physics System
		if(!physics) physics = gameObject.AddComponent<PhysicsSystem>();	//If there is no Physics System, add it
		physics.Initialise(box, 3, 3, 1);		//Initialise the Physics System
		health = GetComponent<HealthManager>();
		if(!health) health = gameObject.AddComponent<HealthManager>();
	}

	protected void Update(){
		pos = physics.UpdatePosition(ref pos, vel);		//Get the position from the physics system
		transform.position = pos;						//Update the position of this character
		physics.UpdateRaycasts(ref pos, ref vel);		//Update the raycasts in the physics system
	}

	//Gravity and other physics should be calculated in FixedUpdate
	protected void FixedUpdate(){
		float grav = vel.y > 0 ? gravity : gravity * 1.5f;
		if(!physics.collision.below) vel.y -= grav * Time.fixedDeltaTime;
	}

	/* Getter and Setter functions */
	public Vector2 GetVelocity(){
		return vel;
	}

	public Vector2 GetPosition(){
		return pos;
	}

	public void SetPosition(Vector2 pos){
		this.pos = pos;
	}

	public BoxCollider2D GetBox(){
		return box;
	}

	public bool IsGrounded(){
		return physics.collision.below;
	}

	public bool GetHorizontalCollision(){
		return physics.collision.right || physics.collision.left;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	This code was created with help from a YouTube tutorial by Sebastian Lague
	https://www.youtube.com/watch?v=MbWK8bCAU2w&t=3s
 */
[RequireComponent(typeof(BoxCollider2D))]
public class PhysicsSystem : MonoBehaviour {

	private Vector2 dir;									//The position, velocity, and direction of this object
	private float gravity = 1;								//The rate of gravity to apply to this object

	private int horizontalRayCount, verticalRayCount;		//The number of raycasts to fire in each direction
	private RaycastHit2D[] verticalRays, horizontalRays;	//An array of raycasts for each direction
	private BoxCollider2D box;								//The box collider attached to this object
	private Vector2 rayLength;								//The length of the raycasts

	public Collisions collision;							//A reference to the collisions struct (this stores direction of collisions)

	public struct Collisions{
		public bool above, below, left, right;				//Booleans to return if collision in any direction
		public List<string> tags;
		public List<GameObject> objects;
	}

	//Initialise the Physics System
	public void Initialise(BoxCollider2D box, int horizontalRayCount, int verticalRayCount, float gravity){
		Physics2D.queriesStartInColliders = false;	//Prevent raycasts from detecting the collider of this object
		Physics2D.queriesHitTriggers = false;		//Prevent raycasts from interacting with trigger objects

		this.box = box;										//Get the box collider
		this.horizontalRayCount = horizontalRayCount;		//How many horizontal rays there should be
		this.verticalRayCount = verticalRayCount;			//How many vertical rays there should be
		this.gravity = gravity;								//The rate of gravity

		verticalRays = new RaycastHit2D[verticalRayCount];		//Initialise the vertical ray array
		horizontalRays = new RaycastHit2D[horizontalRayCount];	//Initialise the horizontal ray array
		dir = new Vector2(1, -1);								//Initialise the direction vector
	}

	public void UpdateRaycasts(ref Vector2 pos, ref Vector2 vel){
		dir.x = vel.x == 0 ? dir.x : Mathf.Abs(vel.x) / vel.x;	//Calculate the x and y direction of the player
		dir.y = vel.y == 0 ? dir.y : Mathf.Abs(vel.y) / vel.y;	//If the player is stationary, use a default direction

		Vector2 size = new Vector2((box.size.x * transform.localScale.x) / 2f, (box.size.y * transform.localScale.y) / 2f);	//Calculate the accurate size of the box
		Vector2 position = new Vector2(pos.x + (box.offset.x * transform.localScale.x), pos.y + (box.offset.y * transform.localScale.y));	//Calculate the accurate position of the box
		if(!collision.left && !collision.right) rayLength.x = size.x + Mathf.Abs(vel.x);	//If there is no collision, set the ray length to 1.5x the size of the collider
		if(!collision.above && !collision.below) rayLength.y = size.y + Mathf.Abs(vel.y);

		Vector2 skinWidth = new Vector2(size.x / 10, size.y / 10);	//How much the raycasts should be positioned inside the box collider
		float horizontalRaySpacing = ((size.y * 2) - skinWidth.y) / (horizontalRays.Length - 1);	//Calculate how much spacing there should be between each ray on the horizontal and vertical axes
		float verticalRaySpacing = ((size.x * 2) - skinWidth.x) / (verticalRays.Length - 1);

		collision.above = collision.below = collision.left = collision.right = false;	//Set all collisions to false
		collision.tags = new List<string>();
		collision.objects = new List<GameObject>();

		//Loop through all horizontal raycasts
		for(int i = 0; i < horizontalRays.Length; i++){
			horizontalRays[i] = Physics2D.Raycast(new Vector2(position.x, position.y - ((size.y + skinWidth.y / 2f) * dir.y) + (horizontalRaySpacing * i * dir.y)), Vector2.right * dir.x, rayLength.x);	//Update raycast
			Debug.DrawRay(new Vector2(position.x, position.y - (size.y - skinWidth.y / 2) + (horizontalRaySpacing * i)), Vector2.right * dir.x * rayLength.x, Color.blue);	//Draw the raycast
			if(horizontalRays[i]){	//If the rays collide with something
				collision.right = dir.x > 0;			//If the direction is greater than 0 the collision happened on the right
				collision.left = !collision.right;		//Otherwise, the collision happened on the left
				pos.x += ((rayLength.x - size.x) - (rayLength.x - horizontalRays[i].distance)) * dir.x;	//Update the position to prevent object from going through other object
				rayLength.x = size.x;	//Update the length of the ray
				vel.x = 0;	//Set the x velocity to zero
				collision.tags.Add(horizontalRays[i].collider.tag);
				collision.objects.Add(horizontalRays[i].collider.gameObject);
			} 
		}

		//Loop through all vertical raycasts
		for(int i = 0; i < verticalRays.Length; i++){
			verticalRays[i] = Physics2D.Raycast(new Vector2(position.x - ((size.x - skinWidth.x / 2f) * dir.x) + (verticalRaySpacing * i * dir.x), position.y), Vector2.up * dir.y, rayLength.y);	//Update raycast
			Debug.DrawRay(new Vector2(position.x - (size.x - skinWidth.x / 2f) + (verticalRaySpacing * i), position.y), Vector2.up * dir.y * rayLength.y, Color.blue);	//Draw raycast in editor
			if(verticalRays[i]){	//If the rays collide with something
				collision.below = dir.y < 0;			//If the direction is less than zero, then a collision happened below
				collision.above = !collision.below;		//If there was no collision below, then it was above
				pos.y += ((rayLength.y - size.y) - (rayLength.y - verticalRays[i].distance)) * dir.y;	//Update the position to be on top of collided object
				rayLength.y = size.y;	//Update the ray length
				vel.y = 0;	//Set the y velocity 
				collision.tags.Add(verticalRays[i].collider.tag);
				collision.objects.Add(verticalRays[i].collider.gameObject);
			} 
		}
	}

	public bool WallGrab(Vector2 pos){
		Vector2 size = new Vector2((box.size.x * transform.localScale.x) / 2f, (box.size.y * transform.localScale.y) / 2f);	//Calculate the accurate size of the box
		Vector2 position = new Vector2(pos.x + (box.offset.x * transform.localScale.x), pos.y + (box.offset.y * transform.localScale.y) + 0.1f);	//Calculate the accurate
		Debug.DrawRay(new Vector2(position.x, position.y + size.y + 0.1f), Vector2.right * dir.x * size.x * 1.5f);
		return Physics2D.Raycast(new Vector2(position.x, position.y + size.y), Vector2.right * dir.x, size.x * 1.5f);
	}

	public void AddGravity(ref Vector2 vel){
		float grav = vel.y > 0 ? gravity : gravity * 1.5f;
		if(!collision.below) vel.y -= grav * Time.fixedDeltaTime;
	}

	public bool HasCollided(){
		return collision.above || collision.below || collision.left || collision.right;
	}

	//Function to check if a collision occured between an object with a specific tag
	public bool CheckCollision(string collisionTag){
		if(HasCollided()){
			for(int i = 0; i < collision.tags.Count; i++){
				if(collision.tags[i].Equals(collisionTag)) return true;
			}
		}
		return false;
	}

	public List<GameObject> GetCollisionObjectsByTag(string collisionTag){
		List<GameObject> objects = new List<GameObject>();
		foreach(GameObject collisionObject in collision.objects){
			if(collisionObject.tag.Equals(collisionTag)) objects.Add(collisionObject);
		}
		if(objects.Count > 0) return objects;
		return null;
	}

	//Function to update the position
	public Vector2 UpdatePosition(ref Vector2 position, Vector2 vel){
		return position + vel;
	}

	public Vector2 GetDirection(){
		return dir;
	}
}

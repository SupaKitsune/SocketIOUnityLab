using UnityEngine;
using System.Collections;

public class CharacterMovement : MonoBehaviour 
{
	public float speed = 6f;			
	public float turnSpeed = 60f;		
	public float turnSmoothing = 15f;

	private Vector3 movement;
	private Vector3 position;
	private float movementSpeed;
	private Vector3 turning;
	private Animator anim;
	private Rigidbody playerRigidbody;

	public bool playerOfThisInstance = false;

	void Awake()
	{
		//Get references to components
		anim = GetComponent<Animator>();
		playerRigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
	{
		if (playerOfThisInstance)
		{
			position = transform.position;
			float h = Input.GetAxis("Horizontal");
			float v = Input.GetAxis ("Vertical");
			movement.Set (h, 0f, v);

			EmitMovement ();
		}

		Move ();

		//Animating without BlendTree
		Animate();
		//Animating withBlendtree
		//Animating(movementSpeed);
	}

	void Move()
	{
		float h = movement.x;
		float v = movement.z;
		//Move the player
		//movement.Set (h, 0f, v);
		movementSpeed = movement.magnitude;
		movement = movement * speed * Time.deltaTime;


        playerRigidbody.MovePosition(transform.position + movement);

		if(h != 0f || v != 0f)
		{
			Rotating(h, v);
		}

	}

	public void NetworkMovement(Vector3 inPosition, Vector3 inMovement){
		transform.position = inPosition;

		movement.Set (inMovement.x, 0f, inMovement.z);
	}

	void Rotating(float h, float v)
	{
		Vector3 targetDirection = new Vector3(h, 0f, v);
		Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp (GetComponent<Rigidbody>().rotation, targetRotation, turnSmoothing * Time.deltaTime);
		GetComponent<Rigidbody>().MoveRotation(newRotation);
	}

	//Regular Animation
	void Animate()
	{
		float h = movement.x;
		float v = movement.z;

		bool running = h != 0f || v != 0f;

		anim.SetBool ("IsRunning", running);
	}
	//Blend Animation
//	void Animating(float mag)
//	{
//		//bool running = lh != 0f || lv != 0f;
//		if (Input.GetKey (KeyCode.RightShift)) {
//			anim.SetFloat ("Speed", movementSpeed *= 0.5f);
//		} else {
//
//			anim.SetFloat ("Speed", movementSpeed);
//		}
//	}

	public void EmitMovement(){
		string networkMovement = VectorToJson(position, movement);
		//Debug.Log ("Send this position to server: " + networkPostion);
		Network.socket.Emit ("move", new JSONObject(networkMovement));
	}

	string VectorToJson(Vector3 pos, Vector3 move){
		return string.Format(@"{{""x"":""{0}"", ""y"":""{1}"", ""h"":""{2}"", ""v"":""{3}""}}", pos.x, pos.z, move.x, move.z);
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour {
    [SerializeField]
	private EnemyAIStates state = EnemyAIStates.Patrolling;
	static private List<GameObject> patrolPoints = null;

	#region Enemy Options
	public float walkingSpeed = 3.0f;
	public float chasingSpeed = 5.0f;
	public float attackingSpeed = 1.5f;
	public float attackingDistance = 1.0f;
	#endregion

	private GameObject patrollingInterestPoint;
	private GameObject playerOfInterest;

	#region Standard MonoBehaviour Methods
	void Start () {
		if(patrolPoints==null) {
			patrolPoints = new List<GameObject>();
			foreach(GameObject go in GameObject.FindGameObjectsWithTag("PatrolPoints")) {
				Debug.Log("Patrol Point: " + go.transform.position);
				patrolPoints.Add(go);
			}
		}
		ChangeToPatrolling();
	}
	
	void Update () {
		switch(state) {
			case EnemyAIStates.Attacking:
				OnAttackingUpdate();
				break;
			case EnemyAIStates.Chasing:
				OnChasingUpdate();
				break;
			case EnemyAIStates.Patrolling:
				OnPatrollingUpdate();
				break;
		}
	}
	#endregion

	#region Update Methods
	void OnAttackingUpdate() {
		float step = attackingSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);

		float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);
		if(distance>attackingDistance) {
			ChangeToChasing(playerOfInterest);
		}
	}

	void OnChasingUpdate() {
		float step = chasingSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, playerOfInterest.transform.position, step);

		float distance = Vector3.Distance(transform.position, playerOfInterest.transform.position);
		if(distance<=attackingDistance) {
			ChangeToAttacking(playerOfInterest);
		}
	}

	void OnPatrollingUpdate() {
		float step = walkingSpeed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, patrollingInterestPoint.transform.position, step);

		float distance = Vector3.Distance(transform.position, patrollingInterestPoint.transform.position);
		if(distance==0) {
			SelectRandomPatrolPoint();
		}
	}
	#endregion

	#region Collider Methods
	void OnTriggerEnter(Collider collider) {
		ChangeToChasing(collider.gameObject);
	}

	void OnTriggerExit(Collider collider) {
		ChangeToPatrolling();
	}
	#endregion

	#region Changes to states
	void ChangeToPatrolling() {
		state = EnemyAIStates.Patrolling;
		GetComponent<Renderer>().material.color = new Color(0.0f, 1.0f, 0.0f);
		SelectRandomPatrolPoint();
		playerOfInterest = null;
	}

	void ChangeToAttacking(GameObject target) {
		state = EnemyAIStates.Attacking;
		GetComponent<Renderer>().material.color = new Color(1.0f, 0.0f, 0.0f);
	}

	void ChangeToChasing(GameObject target) {
		state = EnemyAIStates.Chasing;
		GetComponent<Renderer>().material.color = new Color(1.0f, 1.0f, 0.0f);
		playerOfInterest = target;
	}
	#endregion

	void SelectRandomPatrolPoint() {
		int choice = Random.Range(0,patrolPoints.Count);
		patrollingInterestPoint = patrolPoints[choice];
		Debug.Log("Enemy going to patrol to point " + patrollingInterestPoint.name);
	}
}

﻿using UnityEngine;
using System.Collections;

/// <summary>
/// Place the labels for the Transitions in this enum.
/// Don't change the first label, NullTransition as FSMSystem class uses it.
/// </summary>
public enum Transition
{
	NullTransition = 0, // Use this transition to represent a non-existing transition in your system
	SawPlayer = 1,
	LostPlayer = 2,
    Dead = 3,
}

/// <summary>
/// Place the labels for the States in this enum.
/// Don't change the first label, NullTransition as FSMSystem class uses it.
/// </summary>
public enum StateID
{
	NullStateID = 0, // Use this ID to represent a non-existing State in your system
	PatrollingID = 1,
	ChasingPlayerID = 2,
    DeathID = 3,
}

public class EnemyBehaviour : MonoBehaviour
{

	public FSMSystem fsm;
	public GameObject player;
	public Transform[] wp;
	
	public float fieldOfViewAngle = 55f;
	public float sightRange = 200f;
    public int healthPoints = 15;

	// Use this for initialization
	void Start () {
		MakeFSM();
	}

	private void MakeFSM()
	{
		//TODO: Create two states (PatrolState and ChasePlayerState), using the constructors defined below.
		//   Add one transition to each state (so each state can transit to the other), using the function FSMState.AddTransition
		//   Add both states to the FSM at the end of this method

		PatrolState patrol = new PatrolState(gameObject, wp);
        patrol.AddTransition(Transition.SawPlayer, StateID.ChasingPlayerID);
		ChasePlayerState chase = new ChasePlayerState(gameObject, player);
        chase.AddTransition(Transition.LostPlayer, StateID.PatrollingID);
        chase.AddTransition(Transition.Dead, StateID.DeathID);
        DeathState death = new DeathState(gameObject);

		fsm = new FSMSystem();
        fsm.AddState(patrol);
        fsm.AddState(chase);
        fsm.AddState(death);
	}

	public void SetTransition(Transition t) { fsm.PerformTransition(t); }

	// Update is called once per frame
	void Update () {
		fsm.CurrentState.Reason(player, gameObject);
		fsm.CurrentState.Act(player, gameObject);
	}

	
	public bool PlayerInSight(GameObject player, GameObject npc)
	{
		Vector3 toPlayer = player.transform.position - npc.transform.position;
		float angle = Vector3.Angle (npc.transform.forward, toPlayer);

        //TODO: Check if the player is in sight. Consider that:
        //    He can only be in sight if angle is less than the field of view.
        //    You should use Raycasting to determine if obstacles are blocking the view of the player.
        if(Vector3.Distance(npc.transform.position, player.transform.position) < sightRange)
        {
            return true;
        }

		return false;
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "PlayerWeapon")
        {
            healthPoints -= 5;
        }
    }
}


public class PatrolState : FSMState
{
	private int currentWayPoint;
	private Transform[] waypoints;
	private EnemyAnimation enemyAnimation;
	private float patrolSpeed = 2.5f;
	
	public PatrolState(GameObject thisObject, Transform[] wp) 
	{ 
		waypoints = wp;
		currentWayPoint = 0;
		stateID = StateID.PatrollingID;
		enemyAnimation = thisObject.GetComponent<EnemyAnimation> ();
	}
	
	public override void Reason(GameObject player, GameObject npc)
	{
		//Check line of sight.
		if (npc.GetComponent<EnemyBehaviour>().PlayerInSight (player, npc)) {
            //TODO: Make a transition using Transition.SawPlayer.
            npc.GetComponent<EnemyBehaviour>().fsm.PerformTransition(Transition.SawPlayer);
		}
	}


	public override void Act(GameObject player, GameObject npc)
	{
		//TODO: Program the Patrol State Act. It should update the currentWayPoint to the next one, 
		// in case the current one is reached by the agent.
        if (Vector3.Distance(npc.transform.position, waypoints[currentWayPoint].position) < 2)
        {
            currentWayPoint = (currentWayPoint + 1) % waypoints.Length;
        }




		//Update the target.
		enemyAnimation.setTarget (waypoints[currentWayPoint], patrolSpeed);
	}	
}


public class ChasePlayerState : FSMState
{
	private EnemyAnimation enemyAnimation;
	private float chaseSpeed = 4f;
	private float stopDist = 4f;
	
	public ChasePlayerState(GameObject thisObject, GameObject tgt) 
	{ 
		stateID = StateID.ChasingPlayerID;
		enemyAnimation = thisObject.GetComponent<EnemyAnimation> ();
	}
	
	public override void Reason(GameObject player, GameObject npc)
	{
		//Check line of sight.
		if (!npc.GetComponent<EnemyBehaviour>().PlayerInSight (player, npc)) {
            //TODO: Make a transition using Transition.LostPlayer.
            npc.GetComponent<EnemyBehaviour>().fsm.PerformTransition(Transition.LostPlayer);
        }
        if (npc.GetComponent<EnemyBehaviour>().healthPoints < 0)
        {
            Debug.Log("He dead, gun'nor");
            npc.GetComponent<EnemyBehaviour>().fsm.PerformTransition(Transition.Dead);
        }
	}
	
	public override void Act(GameObject player, GameObject npc)
	{
        if (!npc.GetComponent<EnemyAnimation>().isDead)
        {
            //TODO: Program the Chase State Act. It should chase the player's position until being 
            //  at a distance less than 'stopDist'. You can use the methods from EnemyAnimation.
            if (Vector3.Distance(npc.transform.position, player.transform.position) > stopDist)
                npc.GetComponent<EnemyAnimation>().setTarget(player.transform, chaseSpeed);
            else
                npc.GetComponent<EnemyAnimation>().Attack();
        }
	}	
}

public class DeathState : FSMState
{
    private EnemyAnimation enemyAnimation;

    public DeathState(GameObject thisObject)
    {
        stateID = StateID.DeathID;
        enemyAnimation = thisObject.GetComponent<EnemyAnimation>();
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        //no transition out of this state
    }

    public override void Act(GameObject player, GameObject npc)
    {
        //TODO: Program the Chase State Act. It should chase the player's position until being 
        //  at a distance less than 'stopDist'. You can use the methods from EnemyAnimation.
        npc.GetComponent<EnemyAnimation>().DeathAnimation();

    }
}



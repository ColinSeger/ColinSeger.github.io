using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class IdleAnimation
{
    [Header("Idle Animation")]
    [SerializeField] float speed = 0.01f;
    [SerializeField] Vector2 upDownMovement = Vector2.zero;
    [Header("Rotate Idle")]
    [SerializeField] bool shouldRotate = false;
    [SerializeField] float radius = 1f;

    public IdleAnimation()
    {
        speed = 0.01f;
        shouldRotate = false;
        radius = 1f;
        upDownMovement = Vector2.zero;
    }
    public float Speed { get { return speed; } }
    public bool ShouldRotate { get { return shouldRotate; } }
    public float Radius { get { return radius; } }
    public Vector2 MovementRange { get{ return upDownMovement; } }
}

[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    [SerializeField] EnemyMovement movement;
    [SerializeField] int startPointIndex = 0;
    [SerializeField] float time;
    [SerializeField] IdleAnimation idleAnimation = new IdleAnimation();
    [HideInInspector] public Vector3 location;
    [HideInInspector] public Vector3 offset;
    [SerializeReference] public List<BaseShootingStats> shootingStats;
    [SerializeField] public ShootMode modeToAdd;
    [InspectorButton("AddShootBehavior")]
    public bool AddShootMode;
    protected EnemyBehaviourList enemyBehaviorList;

    public float Speed { get { return idleAnimation.Speed; } }
    public EnemyBehaviourList EnemyBehaviourList {
        get
        {
            if (shootingStats.Count >= 1)
            {
                enemyBehaviorList.ApplyBehavior(shootingStats);
            }
            else
            {
                shootingStats = enemyPrefab.GetComponent<EnemyBehaviourList>().ShootingStats;
                enemyBehaviorList.ApplyBehavior(shootingStats);
            }
            return enemyBehaviorList;
        }
        set { enemyBehaviorList = value; } }
    bool isIn = false;

    public void MoveInOut()
    {
        movement.IdleAnimation = idleAnimation;
        if (enemyPrefab.TryGetComponent<EnemyBehaviourList>(out enemyBehaviorList))
        {
            enemyBehaviorList.DisableTasks();
            if (shootingStats.Count >= 1)
            {
                enemyBehaviorList.ApplyBehavior(shootingStats);
            }
            else
            {
                enemyBehaviorList.ApplyBehavior(enemyBehaviorList.ShootingStats);
            }
        }

        movement.isReady += Ready;
        movement.startIndex = startPointIndex;
        movement.pointIndex = startPointIndex;
        movement.stepIndex = startPointIndex+1;
        movement.StartCoroutine(SpawnDelay());

    }

    void Ready(bool ready)
    {
        isIn = ready;
        if (enemyBehaviorList && ready)
        {
            enemyBehaviorList.TriggerAction("Intro");
            enemyBehaviorList.StartTasks();
        }
        movement.isReady -= Ready;
    }

    public void MoveTick()
    {
        if (!movement || !isIn) return;
        ReturnMove move = movement.Move();
        if (move.GetMoveType() == MoveType.Offset)
        {
            offset = move.GetValue();
        }
        else
        {
            location = move.GetValue();
        }
    }
    public void FaceLocation(Vector3 location)
    {
        enemyPrefab.transform.LookAt(location);
    }

    public void SetMovement(EnemyMovement movementScript)
    {
        movement = movementScript;
    }
    public EnemyMovement GetEnemyMovement()
    {
        return movement;
    }

    IEnumerator SpawnDelay()
    {
        if (isIn)
        {
            movement.InOutLerp(this, !isIn);
            yield break;
        }
        enemyPrefab.SetActive(false);
        yield return new WaitForSeconds(this.time);
        if (enemyPrefab)
        {
            enemyPrefab?.SetActive(true);
            movement.InOutLerp(this, !isIn);                
        }
    }
    void AddShootBehavior()
    {
        switch (modeToAdd)
        {
            case ShootMode.SimpleShoot:
                this.shootingStats.Add(new BaseShootingStats());
                break;
            case ShootMode.BurstLinear:
                this.shootingStats.Add(new BurstShootingStats());
                break;
            case ShootMode.BurstSweep:
                this.shootingStats.Add(new BurstSweepShootingStats());
                break;
            case ShootMode.MultiShoot:
                this.shootingStats.Add(new MultiShootStats());
                break;
            case ShootMode.WaveShoot:
                this.shootingStats.Add(new WaveShootStats());
                break;
            default:
                Debug.LogWarning("You are trying to add something that does not exist");
                break;
        }
    }
}

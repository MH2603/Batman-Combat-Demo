using UnityEngine;

namespace MH
{

    public enum EnemyState
    {
        Idle, // idling
        Circulate, // move circle around player
        Chase, // move toward player to attack
        Attack,
        Retreat // after attack, enemy will retreat
    }

    public class EnemyController : MonoBehaviour
    {

        #region ----------------- Fields ----------------------

        [Space]
        public float RotateSpeed = 100f;
        public float Acceleration = 10f;
        [Space]
        public float CirculateSpeed = 2f;
        public int rightMove = 1;
        [Space]
        public float ChaseSpeed = 3f;
        [Space]
        public float AttackConditionDistance = 1f;
        [Space]
        public float RetreatSpeed = 2f;
        public float RetreatDistance = 6f;
        [Space]
        public int Hp = 5;

        #endregion

        #region ----------------- Properties ----------------------

        [Space(10)]
        [SerializeField] private EnemyState currentState;
        private Animator animator;
        private EnemyManager manager;
        private CombatController player;

        
        [SerializeField] private float currentSpeed = 0;
        [SerializeField] private Vector3 currentDirection = Vector3.zero;    
        [SerializeField] private bool lockMove = false;
        [SerializeField] private EnemyState requireState;

        #endregion


        #region ----------------- Unity Methods ----------------------

        private void Start()
        {
            
            //Init(); 
        }

        private void FixedUpdate()
        {
           
            switch (currentState)
            {
                case EnemyState.Idle:
                    Idling();
                    break;
                case EnemyState.Circulate:
                    Circulating();
                    break;
                case EnemyState.Chase:
                    Chasing();  
                    break;
                case EnemyState.Attack:
                    break;
                case EnemyState.Retreat:
                    Retreating();
                    break;
            }

            //if (lockMove)
            //{
            //    if (currentSpeed > 0)
            //    {
            //        currentSpeed -= Acceleration * 10 * Time.fixedDeltaTime;
            //        currentSpeed = Mathf.Clamp(currentSpeed, 0, currentSpeed + 1f);
            //    }

            //    currentDirection = Vector3.zero;    
            //}

            animator.SetFloat("Speed", currentSpeed);
        }

        #endregion


        #region ----------------- Public Methods ----------------------

        public bool IsAttackable()
        {
            return true;
        }
       

        public void TakeDmg()
        {
            animator.SetTrigger("Hit"); 

            if (Hp <= 0) return;

            Hp--;
        }

        public void OnLockMove()
        {
            LockMove();
        }

        public void OnUnlockMove()
        {
            UnlockMove();   
        }

        public void OnChase()
        {
            StartChase();   
        }
        
        public void OnHit()
        {
            if ( player.CanTakeDmg() )
            {
                player.TakeDmg();
            }
        }

        // call by animation event
        public void OnEndAttackProcess()
        {
            StartRetreat();
            manager.OnSortBehavior();
        }

        //public void OnRetreat()
        //{
        //    StartRetreat(); 
        //}

        public void ChoseRequireState()
        {
            float random = Random.Range(-1, 1);
            requireState = ( random >= 0 ) ? EnemyState.Idle : EnemyState.Circulate;

            if (currentState == EnemyState.Retreat) return;

            if (requireState == EnemyState.Idle)
            {
                StartIdle();
            }
            else
            {
                StartCirculate();
            }
        }

        #endregion

        #region ----------------- Private Methods ----------------------

        public void Init()
        {
            animator = GetComponentInChildren<Animator>();
            currentState = EnemyState.Idle;

            manager = transform.parent.GetComponent<EnemyManager>();
            player = manager.Player;

            //StartIdle();

        }

        #region Ilde 

        private bool CanRetreatToIdle()
        {
            if ( Vector3.Distance(transform.position, player.transform.position) >= RetreatDistance )
            {
                return true;
            }

            return false;   
        }

        private void StartIdle()
        {
            currentState = EnemyState.Idle;
            currentDirection = Vector3.zero;
        }

        private void Idling()
        {
            if( currentSpeed > 0)
            {
                currentSpeed -= Acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, currentSpeed + 1f);
            }
            
            RotateTowardTarget();
        }

        #endregion

        #region Circulate

        private void StartCirculate()
        {
            currentState = EnemyState.Circulate;

            rightMove *= -1;
            
        }

        private void Circulating()
        {
            RotateTowardTarget();

            if (!lockMove && currentSpeed < CirculateSpeed)
            {
                currentSpeed += Acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, CirculateSpeed);    
            }

            currentDirection = transform.right * rightMove;
            MoveToward();
            //transform.position += transform.right * rightMove * currentSpeed * Time.fixedDeltaTime;
        }

        #endregion

        #region Chase Process 

        private void StartChase()
        {
            currentSpeed = ChaseSpeed;

            currentState = EnemyState.Chase;
            currentDirection = transform.forward;
        }

        private void Chasing()
        {
            RotateTowardTarget();
            if (!lockMove && currentSpeed < ChaseSpeed)
            {
                currentSpeed += Acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, ChaseSpeed);    
            }

            currentDirection = transform.forward;
            MoveToward();

            if ( CanChaseToAttack() )
            {
                StartAttack();
            }
        }

        #endregion


        #region Attack Process

        private bool CanChaseToAttack()
        {
            return Vector3.Distance(transform.position, player.transform.position) <= AttackConditionDistance;
        }

        private void StartAttack()
        {
            if (player.IsAttacking())
            {
                OnEndAttackProcess();
                return;
            }

            currentSpeed = 0;
            animator.SetTrigger("Attack");

            currentState = EnemyState.Attack;   
        }

        #endregion

        #region Retreat Process

        private void StartRetreat()
        {
            currentState = EnemyState.Retreat;  
        }

        private void Retreating()
        {
            RotateTowardTarget();

            if (!lockMove && currentSpeed < RetreatSpeed)
            {
                currentSpeed += Acceleration * Time.fixedDeltaTime;
                currentSpeed = Mathf.Clamp(currentSpeed, 0, RetreatSpeed);

                
            }

            //transform.position += transform.forward * -1 * currentSpeed * Time.fixedDeltaTime;
            currentDirection = transform.forward * -1;
            MoveToward();

            if ( CanRetreatToIdle() )
            {
                if( requireState == EnemyState.Idle) StartIdle();
                else StartCirculate();
            }
        }

        #endregion


        private void RotateTowardTarget()
        {
            Vector3 dir = -transform.position + player.transform.position;
            dir.y = 0;
            dir.Normalize();

            Quaternion toRotation = Quaternion.LookRotation(dir, Vector3.up);

            transform.rotation = Quaternion.RotateTowards
                                 (animator.transform.rotation, toRotation, RotateSpeed * Time.deltaTime);
        }

        private void MoveToward()
        {
            if (lockMove) return;

            currentDirection.y = 0;
            currentDirection.Normalize();
            transform.position += currentDirection * currentSpeed * Time.fixedDeltaTime;
        }

        private void LockMove()
        {
            lockMove = true;
        }

        private void UnlockMove()
        {
            lockMove = false;
        }

        

        #endregion
    }

}

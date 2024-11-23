using UnityEngine;
using MH;
using DG.Tweening;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;

namespace MH
{

    public class CombatController : MonoBehaviour
    {
        #region -------------------- Fields -------------------
        public List<AttackConfig> attacks;
        public EnemyManager enemyManager;
        public CinemachineHandler cineHandler;
        [Space]
        public float AttackCoolDown = 1f;
        public int TargetFPS = 90;
        #endregion

        #region -------------------- Properties -------------------
        private ThirdPersonController movementController;
        private StarterAssetsInputs inputController;
        private DetectionHandler detection;
        private Animator animator;

        private EnemyController lockTaget;
        private AttackConfig currentAttack;

        [SerializeField] private bool canAttack = true;
        [SerializeField] private bool isAttacking = false;
        #endregion


        #region --------------------- Unity Methods -----------------------

        private void Start()
        {
            Init();
        }


        #endregion


        #region --------------------- Public Methods ----------------------------

        public void OnAttack()
        {
            if (inputController.LockInput || !canAttack) return;

            StartAttack();
        }

        public bool CanTakeDmg()
        {
            return !isAttacking;
        }

        public bool IsAttacking()
        {
            return isAttacking ;
        }

        public void TakeDmg()
        {
            animator.SetTrigger("Hit");
            LockInput();
        }

        public void OnUnlockInput()
        {
            UnLockInput();
        }

        #endregion

        #region -------------------- Private Methods ----------------------

        private void Init()
        {
            detection = GetComponentInChildren<DetectionHandler>(); 
            animator = GetComponentInChildren<Animator>();
            movementController = GetComponent<ThirdPersonController>();
            inputController = GetComponent<StarterAssetsInputs>();

            LockFPS();

            foreach (var attackConfig in attacks)
            {
                attackConfig.Init();
            }
        }

        private void LockFPS()
        {
            // Make the game run as fast as possible
            Application.targetFrameRate = -1;
            // Limit the framerate to 60
            Application.targetFrameRate = TargetFPS;
        }

        private void FreezeTime(float value,float delay ,float duration)
        {
            if (value >= 1f) return;

            cineHandler.ZoomEffect(40, currentAttack.FreezeTimeDelay, 300, currentAttack.FreezeTimeDuration);

            StopCoroutine(SetTime());
            StartCoroutine(SetTime());

            IEnumerator SetTime()
            {
                yield return new WaitForSecondsRealtime(delay);
                Time.timeScale = value;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
                yield return new WaitForSecondsRealtime(duration);

                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }


        private void MoveTowardTarget(Transform target, float duration)
        {
            transform.DOLookAt(target.position, duration);
            transform.DOMove(TargetOffset(target), duration).SetEase(Ease.Linear);
        }

        private Vector3 TargetOffset(Transform target)
        {
            Vector3 position;
            position = target.position;
            return Vector3.MoveTowards(position, transform.position, currentAttack.Offset);
        }

        private void LockInput()
        {
            inputController.LockInput = true;
            inputController.move = Vector2.zero;
            inputController.look = Vector2.zero;
        }

        private void UnLockInput()
        {
            inputController.LockInput = false;
            inputController.move = inputController.moveInput;
        }

        private bool FindToLockEnemy()
        {
            lockTaget = detection.CurrentTarget();

            if (!lockTaget)
            {
                lockTaget = enemyManager.GetRandomEnemy();
            }

            return lockTaget != null;
        }

        private void StartAttack()
        {
            if(!FindToLockEnemy()) return;

            ChoseAttackType();
            
            if (currentAttack == null) return;

            isAttacking = true;
            canAttack = false;
            lockTaget.OnLockMove(); // lock move of enemy
            LockInput();
            MoveTowardTarget(lockTaget.transform, currentAttack.MoveDuration); // move to lock target
            animator.SetTrigger(currentAttack.GetRandomTrigger());

            // call delay when start attack to freeze frame
            FreezeTime(currentAttack.FreezeTimeValue, currentAttack.FreezeTimeDelay, currentAttack.FreezeTimeDuration);

            StopCoroutine(WaitUnlockMovement());
            StartCoroutine(WaitUnlockMovement());

            StopCoroutine(CoolDownProcess());
            StartCoroutine(CoolDownProcess());

            IEnumerator WaitUnlockMovement()
            {
                yield return new WaitForSeconds(currentAttack.MoveDuration);

                lockTaget.TakeDmg();
                currentAttack.OnHitVFX();
                cineHandler.ShakeCamera(2f, 0.2f);
                
                yield return new WaitForSeconds(0.2f);
                isAttacking = false;
                UnLockInput();

                if ( lockTaget == enemyManager.GetAttackEnemy() ) lockTaget.OnEndAttackProcess();
                

            }

            IEnumerator CoolDownProcess()
            {
                yield return new WaitForSeconds(AttackCoolDown);
                canAttack = true;
            }
        }


        private void ChoseAttackType()
        {
            currentAttack = null;
            float distance = Vector3.Distance(transform.position, lockTaget.transform.position);

            foreach (var attackConfig in attacks)
            {
                if ( attackConfig.CanUse(distance) )
                {
                    currentAttack = attackConfig;
                    return;
                }
            }
        }


        #endregion
    }

}

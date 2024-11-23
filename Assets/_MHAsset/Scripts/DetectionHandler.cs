using UnityEngine;
using MH;
using StarterAssets;
using System.Linq;

namespace MH
{

    public class DetectionHandler : MonoBehaviour
    {



        #region ------------- Fields ---------------
        public float castRadius = 0.5f;
        public float castDistance = 10f;
        //public float castAngle = 30f;
        public LayerMask targetLayer;


        #endregion

        #region ------------- Properties ---------------
        [Space]
        [SerializeField] Vector3 inputDirection;
        [SerializeField] private EnemyController currentTarget;
        private StarterAssetsInputs movementInput;
        private CombatController combatScript;

        #endregion

        private void Start()
        {
            movementInput = GetComponentInParent<StarterAssetsInputs>();
            combatScript = GetComponentInParent<CombatController>();
        }

        private void Update()
        {
            if(movementInput.moveInput == Vector2.zero)
            {
                currentTarget = null;
                return;
            }

            var camera = Camera.main;
            var forward = camera.transform.forward;
            var right = camera.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            inputDirection = forward * movementInput.move.y + right * movementInput.move.x;
            inputDirection = inputDirection.normalized;

            RaycastHit info;

            if (Physics.SphereCast(transform.position, castRadius, inputDirection, out info, castDistance, targetLayer, QueryTriggerInteraction.UseGlobal))
            {
                if (info.collider.transform.GetComponent<EnemyController>().IsAttackable())
                    currentTarget = info.collider.transform.GetComponent<EnemyController>();
            }
        }

        //private void FindTarget(Vector3 dir)
        //{
        //    Collider[] colliders = Physics.OverlapSphere(transform.position, castDistance, targetLayer);

        //    if (colliders.Length == 0) return;

        //    float minAngle = 360;
        //    foreach ( var collider in colliders)
        //    {
        //        if ( collider.transform &&
        //             collider.GetComponent<EnemyController>() )
        //        {
                    
        //        }
        //    }
        //}

        public EnemyController CurrentTarget()
        {
            return currentTarget;
        }

        public void SetCurrentTarget(EnemyController target)
        {
            currentTarget = target;
        }

        public float InputMagnitude()
        {
            return inputDirection.magnitude;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, inputDirection.normalized * castDistance);
            //Gizmos.DrawWireSphere(transform.position, 1);
            if (CurrentTarget() != null)
                Gizmos.DrawSphere(CurrentTarget().transform.position, 0.5f);
        }

    }

}

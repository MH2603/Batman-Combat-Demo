using UnityEngine;
using MH;

namespace MH
{
    [System.Serializable]
    public class AttackConfig 
    {
        public string[] TriggerKey;
        public Vector2 ConditionDistance;
        public float Offset = 1f;
        public float MoveDuration;

        [Space]
        public float FreezeTimeValue = 1f;
        public float FreezeTimeDelay = 0.1f;
        public float FreezeTimeDuration = 0.2f;
        [Space]
        public ParticleSystem HitVFX;


        private Vector3 startHitLocalPos;
        private Transform parentVFX;

        public void Init()
        {
            if (HitVFX)
            {
                startHitLocalPos = HitVFX.transform.localPosition;
                parentVFX = HitVFX.transform.parent;
            }
            
        }

        public bool CanUse(float distance)
        {
            return distance >= ConditionDistance.x && distance < ConditionDistance.y;
        }

        public string GetRandomTrigger()
        {
            return TriggerKey[Random.Range(0, TriggerKey.Length)];
        }

        public void OnHitVFX()
        {
            if (!HitVFX) return;

            HitVFX.transform.SetParent(parentVFX);
            HitVFX.transform.localPosition = startHitLocalPos;

            HitVFX.transform.SetParent(null);

            HitVFX.Play();
           
        }
    }

}

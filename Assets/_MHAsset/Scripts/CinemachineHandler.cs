using UnityEngine;
using MH;
using Cinemachine;
using System.Collections;

namespace MH
{

    public class CinemachineHandler : MonoBehaviour
    {
        #region Properties

        private CinemachineVirtualCamera _cinemachine;
        private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
        [Space]
        private float targetZoom = 0;
        private float originalZoom = 0;
        private float currentZoomSpeed = 0;
        private bool isZooming = false; 
        #endregion

        #region Unity Methods

        private void Start()
        {
            Init();
        }

        private void FixedUpdate()
        {
            Zooming();
        }


        #endregion

        public void ShakeCamera(float intensity = 1f, float duration = 0.5f)
        {
            _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

            StopCoroutine(LifeTime());
            StartCoroutine(LifeTime());

            IEnumerator LifeTime()
            {
                yield return new WaitForSecondsRealtime(duration);
                _cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }

            
        }

        public void ZoomEffect(float fieldOfView,float delay ,float zoomSpeed = 10f, float duration = 0.2f )
        {
            currentZoomSpeed = zoomSpeed;

            StopCoroutine(LifeTime());
            StartCoroutine(LifeTime()); 

            IEnumerator LifeTime()
            {
                yield return new WaitForSecondsRealtime(delay);
                targetZoom = fieldOfView;

                yield return new WaitForSecondsRealtime(duration);
                targetZoom = originalZoom;

            }
        }


        #region Private Methods

        private void Init()
        {
            _cinemachine = GetComponent<CinemachineVirtualCamera>();
            _cinemachineBasicMultiChannelPerlin = _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            originalZoom = _cinemachine.m_Lens.FieldOfView;
            targetZoom = originalZoom;
        }

        private void Zooming()
        {
            float offset = targetZoom - _cinemachine.m_Lens.FieldOfView;
            if ( Mathf.Abs(offset) >= 1 )
            {
                float value = _cinemachine.m_Lens.FieldOfView + Mathf.Sign(offset) * currentZoomSpeed * Time.fixedDeltaTime;
                
                _cinemachine.m_Lens.FieldOfView = (Mathf.Sign(offset) >= 1) ? Mathf.Clamp(value, 0, targetZoom) : Mathf.Clamp(value, targetZoom, _cinemachine.m_Lens.FieldOfView);
            }
        }

        #endregion
    }

}

using System.Collections;
using UnityEngine;

namespace GridSystem
{
    public class TileChildController : MonoBehaviour
    {
        public Animator animator;
        public AnimationCurve animationCurve;
        private float _t;
        private static readonly int Blast = Animator.StringToHash("Blast");
        private const float LerpTime = 1f;
        public ParticleSystem particleSys;
        
        private void Start()
        {
            CreatedChild();
        }

        public void CreatedChild()
        {
            particleSys = gameObject.transform.GetChild(1).GetComponent<ParticleSystem>();
            particleSys.Stop();
        }

        public void BlastAnimation()//Before blast blasting animation trigger.
        {
            animator.SetTrigger(Blast);
        }

        public void FallingAnimation()
        {
            StartCoroutine(ChangePosition());
        }

        public void ActivateParticleSystem()
        {
            particleSys.Play();
        }
        private IEnumerator ChangePosition()//For Tile type's falling to the down.
        {
            _t = 0;
            //yield return new WaitForSeconds(0.3f);
            while (_t < LerpTime)
            {
                _t += Time.deltaTime;

                var rate = animationCurve.Evaluate(_t / LerpTime);
                transform.localPosition = Vector3.SlerpUnclamped(transform.localPosition,
                    Vector3.zero, rate);
                yield return null;
            }
            _t = LerpTime;
            transform.localPosition = Vector3.zero;
        }
    }
}

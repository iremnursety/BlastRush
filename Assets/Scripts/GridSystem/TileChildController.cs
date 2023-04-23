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
            StopAnimation();
        }

        public void StopAnimation()
        {
            particleSys.Stop();
        }

        public bool PowerUpBool { get; set; }

        public void BlastTileChild()
        {
            StartCoroutine(BlastAnimation());
        }

        private IEnumerator BlastAnimation()//Before blast blasting animation trigger.
        {
            animator.SetTrigger(Blast);
            yield return new WaitForSeconds(0.3f);
            ActivateParticleSystem();
        }

        public void FallingAnimation()
        {
            StartCoroutine(ChangePosition());
        }

        private void ActivateParticleSystem()
        {
            particleSys.Play();
        }
        
        private IEnumerator ChangePosition()//For Tile type's falling to the down.
        {
            _t = 0;
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

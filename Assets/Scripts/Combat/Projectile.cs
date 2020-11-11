using UnityEngine;
using RPG.Attributes;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] public UnityEvent onHit;

        [SerializeField] public float projectileSpeed = 5f;
        [SerializeField] public bool isHoming = true;
        [SerializeField] public GameObject hitEffect = null;
        [SerializeField] public float maxLifeTime = 10;         //How long before projectile is destroyed in scene
        [SerializeField] private GameObject[] destroyOnHit = null;      //Array of projectile parts to destroy on impact with enemy
        [SerializeField] public float lifeAfterImpact = 2;

        private GameObject instigator = null;           //the person who shot the projectile
        private Health target = null;
        private float damage = 0;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null)     //Protect against null target
            {
                return;
            }

            if (isHoming && !target.IsDead())   //If missle ishoming & target is not dead then look at target
            {
                transform.LookAt(GetAimLocation());

            }
            transform.Translate(Vector3.forward * projectileSpeed * Time.deltaTime);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            this.target = target;
            this.damage = damage;
            this.instigator = instigator;
            Destroy(gameObject, maxLifeTime);
        }
        
        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = target.GetComponent<CapsuleCollider>();

            if (targetCapsule == null)
            {
                return target.transform.position; //Protects against null capsule collider default returns target.position.
            }

            return target.transform.position + Vector3.up * targetCapsule.height / 2;         //Should increase by 1 on y axis offset and then get middle of capsule collider
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != target) return;     //if health component is not equal to target exit
            if (target.IsDead()) return;    //If target is dead exit


            target.TakeDamage(instigator, damage);  //target.take damage
            projectileSpeed = 0;

            onHit.Invoke();       //Invokes the projectile hit sound effect

            if (hitEffect != null)
            {
                Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destroyOnHit)          //For all items in destroyOnHit array destroy
            {
                Destroy(toDestroy);
            }


            Destroy(gameObject, lifeAfterImpact);            //destroy projectile missle    / how much time before the file projectile object is destroyed
        }
    }
}

using System;
using Aircraft;
using UnityEngine;

namespace Weapons
{
    public class Laser : MonoBehaviour
    {
        public Action OnExpire;

        public float Speed => speed;
        
        [SerializeField] private float speed = 1000f;
        [SerializeField] private float maxDistance = 1000f;
        [SerializeField] private int damage = 33;
        [SerializeField] private int armorPiercing = 5;
        [SerializeField] private GameObject impactAudioPrefab;

        private Vector3 startPosition;
        private Faction shooterFaction;
        
        private void Awake()
        {
            startPosition = transform.localPosition;
        }

        private void Update()
        {
            Move();
            if (IsOutOfRange())
            {
                Die();
            }
        }

        public void AimAtTarget(Vector3 targetPosition)
        {
            transform.LookAt(targetPosition);
        }
        
        public void SetFaction(Faction faction)
        {
            shooterFaction = faction;
        }
        
        private bool IsOutOfRange()
        {
            float distance = Vector3.Distance(startPosition, transform.position);
            return distance >= maxDistance;
        }

        private void Die()
        {
            OnExpire?.Invoke();
            PlayImpactSound();
            Destroy(gameObject);
        }

        private void PlayImpactSound()
        {
            GameObject impactSound = Instantiate(impactAudioPrefab, transform.position, Quaternion.identity);
            var audioSource = impactSound.GetComponent<AudioSource>();
            Destroy(impactSound, audioSource.clip.length);
            Destroy(gameObject);
        }

        private void Flash()
        {
            Die();
        }

        private void Move()
        {
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            GameObject hitObject = other.gameObject;
            if (hitObject.TryGetComponent(out Health health))
            {
                if (health.Faction == shooterFaction)
                {
                    return;
                }
                
                health.TakeDamage(damage, armorPiercing, shooterFaction);
                Die();
            }
            else
            {
                Flash();
            }
        }
    }
}
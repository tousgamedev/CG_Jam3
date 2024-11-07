using System;
using Aircraft;
using UnityEngine;

namespace Weapons
{
    public class Laser : MonoBehaviour
    {
        public Action OnExpire;

        [SerializeField] private float speed = 10f;
        [SerializeField] private float maxDistance = 1000f;
        [SerializeField] private int damage = 33;
        [SerializeField] private int armorPiercing = 5;
        [SerializeField] private string[] ignoredNames;

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
            string otherName = hitObject.name;
            foreach (string ignoredName in ignoredNames)
            {
                if (otherName.Contains(ignoredName))
                {
                    return;
                }
            }

            if (hitObject.TryGetComponent(out Health health))
            {
                Debug.Log("Hit");
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
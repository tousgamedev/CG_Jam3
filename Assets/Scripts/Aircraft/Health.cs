using System;
using UnityEngine;

namespace Aircraft
{
    public enum Faction
    {
        None = 0,
        Neutral = 1,
        Ally = 2,
        Enemy = 3
    }

    public class Health : MonoBehaviour
    {
        public Action OnHealthChange;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float Percentage => currentHealth / maxHealth;

        public Faction Faction => faction;

        [SerializeField] private Faction faction = Faction.Neutral;

        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float armor = 10;

        private float currentHealth;

        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage, float armorPiercing, Faction shooterFaction)
        {
            if (shooterFaction == faction || shooterFaction == Faction.None)
            {
                return;
            }

            float armorNegation = armor - armorPiercing;
            currentHealth = Mathf.Max(0, currentHealth - (damage - armorNegation));
            OnHealthChange?.Invoke();
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
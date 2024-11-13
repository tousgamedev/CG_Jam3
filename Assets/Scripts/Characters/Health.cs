using System;
using Characters;
using Movement;
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
        public Action<float> OnHealthChange;
        public Action<BaseCharacterController> OnHealthDepleted;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public float Percentage => currentHealth / maxHealth;
        public Faction Faction => faction;

        [SerializeField] private Faction faction = Faction.Neutral;

        [SerializeField] private float maxHealth = 100;
        [SerializeField] private float armor = 10;

        private float currentHealth;
        private BaseCharacterController controller;
        
        private void Awake()
        {
            currentHealth = maxHealth;
        }

        public void SetController(BaseCharacterController controller)
        {
            this.controller = controller;
        }
        
        public void TakeDamage(float damage, float armorPiercing, Faction shooterFaction)
        {
            if (shooterFaction == faction || shooterFaction == Faction.None)
            {
                return;
            }

            float armorNegation = Mathf.Clamp(armor - armorPiercing, 0, armor);
            currentHealth = Mathf.Max(0, currentHealth - (damage - armorNegation));
            OnHealthChange?.Invoke(Percentage);
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            OnHealthDepleted?.Invoke(controller);
        }
    }
}
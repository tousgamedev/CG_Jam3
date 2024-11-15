using UnityEngine;

namespace VFX
{
    public class EngineEffects : MonoBehaviour
    {
        public EngineEffectsConfig BaseConfig => baseEffectsConfig;
        public EngineEffectsConfig BoostConfig => boostConfigConfig;
        public EngineEffectsConfig BrakeConfig => brakeEffectsConfig;
        
        [SerializeField]
        private ParticleSystem engineParticleSystem;
        
        [SerializeField]
        private EngineEffectsConfig baseEffectsConfig;
        
        [SerializeField]
        private EngineEffectsConfig boostConfigConfig;

        [SerializeField]
        private EngineEffectsConfig brakeEffectsConfig;

        private ParticleSystem.MainModule mainModule;

        private void Awake()
        {
            mainModule = engineParticleSystem.main;
        }
        
        public void ApplyEffects(EngineEffectsConfig engineEffectsConfig, float progress)
        {
            Color primaryColor = Color.Lerp(baseEffectsConfig.primaryColor, engineEffectsConfig.primaryColor, progress);
            Color secondaryColor = Color.Lerp(baseEffectsConfig.secondaryColor, engineEffectsConfig.secondaryColor, progress);
            Vector2 size = Vector2.Lerp(baseEffectsConfig.sizeRange, engineEffectsConfig.sizeRange, progress);
            Vector2 speed = Vector2.Lerp(baseEffectsConfig.speedRange, engineEffectsConfig.speedRange, progress);

            mainModule.startColor = new(primaryColor, secondaryColor);
            mainModule.startSize = new(size.x, size.y);
            mainModule.startSpeed = new(speed.x, speed.y);
        }
    }
}

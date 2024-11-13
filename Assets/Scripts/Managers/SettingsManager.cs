using UnityEngine;

namespace Managers
{
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<SettingsManager>();
                }

                return instance;
            }
        }

        private static SettingsManager instance;
        
        public bool InvertYAxis => invertYAxis;
        
        [SerializeField]
        private bool invertYAxis = true;
    }
}

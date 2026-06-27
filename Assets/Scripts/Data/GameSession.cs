using UnityEngine;

namespace Data
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }

        public float Sensitivity { get; set; }
        public float Volume { get; set; }

        private const string SensKey = "Sensitivity";
        private const string VolumeKey = "Volume";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Load();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(SensKey, Sensitivity);
            PlayerPrefs.SetFloat(VolumeKey, Volume);
            PlayerPrefs.Save();
        }

        public void Load()
        {
            Sensitivity = PlayerPrefs.GetFloat(SensKey, 2.5f);
            Volume = PlayerPrefs.GetFloat(VolumeKey, 100f);
        }
    }
}

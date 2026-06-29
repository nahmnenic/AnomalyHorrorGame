using Player;
using UnityEngine;

namespace Data
{
    public class GameSession : MonoBehaviour
    {
        public static GameSession Instance { get; private set; }
        
        private CameraController _cameraController;

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
            SetSettings();
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

        public void SetSettings()
        {
            _cameraController.mouseSense = Sensitivity / 10;
            SetVolume();
        }
        
        private void SetVolume()
        {
            string vcaPath = "vca:/Master";
            FMOD.Studio.VCA vca = FMODUnity.RuntimeManager.GetVCA(vcaPath);
            vca.setVolume(Volume / 100);
        }
    }
}

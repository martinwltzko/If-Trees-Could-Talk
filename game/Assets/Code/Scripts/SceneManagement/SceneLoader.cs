using System;
using System.Threading.Tasks;
using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement {
    public class SceneLoader : MonoBehaviour, IDependencyProvider { 
        [SerializeField] StatusBar loadingBar;
        [SerializeField] float fillSpeed = 0.5f;
        [SerializeField] Canvas loadingCanvas;
        [SerializeField] Camera loadingCamera;
        [SerializeField] SceneGroup[] sceneGroups;

        float targetProgress;
        bool isLoading;

        public readonly SceneGroupManager manager = new SceneGroupManager();
        
        [Provide] public SceneLoader ProvideSceneLoader() => this;
        
        void Awake() {
            manager.OnSceneLoaded += sceneName => Debug.Log("Loaded: " + sceneName);
            manager.OnSceneUnloaded += sceneName => Debug.Log("Unloaded: " + sceneName);
            manager.OnSceneGroupLoaded += () => Debug.Log("Scene group loaded");
        }

        async void Start() {
            await LoadSceneGroup(0);
        }
        
        void Update() {
            if (!isLoading) return;

            float currentFillAmount = loadingBar.FillAmount;
            float progressDifference = Mathf.Abs(currentFillAmount - targetProgress);

            float dynamicFillSpeed = progressDifference * fillSpeed;

            loadingBar.UpdateStatusBar(Mathf.Lerp(currentFillAmount, targetProgress, Time.deltaTime * dynamicFillSpeed),
                1f);
        }

        public async Task LoadSceneGroup(int index) {
            loadingBar.UpdateStatusBar(0f, 1f);
            targetProgress = 1f;

            if (index < 0 || index >= sceneGroups.Length) {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => targetProgress = Mathf.Max(target, targetProgress);
            
            EnableLoadingCanvas();
            await manager.LoadScenes(sceneGroups[index], progress);
            EnableLoadingCanvas(false);
        }
    
        void EnableLoadingCanvas(bool enable = true) {
            isLoading = enable;
            loadingCanvas.gameObject.SetActive(enable);
            loadingCamera.gameObject.SetActive(enable);
        }
    }
    
    public class LoadingProgress : IProgress<float> {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value) {
            Progressed?.Invoke(value / ratio);
        }
    }
}
using System;
using System.Threading.Tasks;
using DependencyInjection;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.SceneManagement {
    public class SceneLoader : MonoBehaviour, IDependencyProvider { 
        [SerializeField] StatusBar loadingBar;
        [SerializeField] FlipbookImageAnimation loadingAnimation;
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

        async void Start()
        {
            await LoadSceneGroup(0);
        }

        public async Task LoadSceneGroup(int index) {
            loadingAnimation.Play();
            targetProgress = 1f;
            if(!WebHandler.Authenticated) await WebHandler.AuthenticationTask; //TODO: Remove if this class should be reusable

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
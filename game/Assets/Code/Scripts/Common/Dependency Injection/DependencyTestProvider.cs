using UnityEngine;

namespace DependencyInjection
{
    public interface IEnvironmentSystem
    {
        IEnvironmentSystem ProvideEnvironmentSystem();
        void Initialize();
    }

    public class DependencyTestProvider : MonoBehaviour, IDependencyProvider, IEnvironmentSystem
    {
        [Provide]
        public IEnvironmentSystem ProvideEnvironmentSystem() {
            return this;
        }
        
        private TestService service;
        [Provide] 
        public TestService GetTestService() {
            return new TestService();
        }

        public void Initialize()
        {
            Debug.Log("EnvironmentSystem.Initialize()");
        }
    }
    
    public class TestService
    {
        public void Initialize(string message)
        {
            Debug.Log(message);
        }
    }
}
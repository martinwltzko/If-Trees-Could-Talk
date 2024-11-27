using System;
using UnityEngine;

namespace DependencyInjection
{
    public class DependencyTester : MonoBehaviour
    {
        [Inject] IEnvironmentSystem environmentSystem;
        [Inject] TestService testService;

        private void Start()
        {
            environmentSystem.Initialize();
            testService.Initialize("TestService initialized from DependencyTester");
        }
    }
}
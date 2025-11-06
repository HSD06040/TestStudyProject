using UnityEngine;

namespace DI
{
    public class Provider : MonoBehaviour, IDependencyProvider
    {
        [Provider]
        private ServiceA ProvideServiceA()
        {
            return new ServiceA();
        }
    }

    public class ServiceA
    {
        public void DoSomething()
        {
            Debug.Log("ServiceA is doing something.");
        }
    }
}

// =================================	
// Namespaces.
// =================================

using UnityEngine;

// =================================	
// Define namespace.
// =================================

namespace MirzaBeig
{

    namespace ParticleSystems
    {

        // =================================	
        // Classes.
        // =================================

        //[ExecuteInEditMode]
        [System.Serializable]

        //[RequireComponent(typeof(TrailRenderer))]

        public class Rotator : MonoBehaviour
        {
            // =================================	
            // Nested classes and structures.
            // =================================

            // ...

            // =================================	
            // Variables.
            // =================================

            public Vector3 localRotationSpeed;
            public Vector3 worldRotationSpeed;

            // =================================	
            // Functions.
            // =================================

            // ...

            void Awake()
            {

            }

            // ...

            void Start()
            {

            }

            // ...

            void Update()
            {
                transform.Rotate(localRotationSpeed * Time.deltaTime, Space.Self);
                transform.Rotate(worldRotationSpeed * Time.deltaTime, Space.World);
            }

            // =================================	
            // End functions.
            // =================================

        }

        // =================================	
        // End namespace.
        // =================================

    }

}

// =================================	
// --END-- //
// =================================

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SceneBasedComponents : MonoBehaviour
{
    [Header("Open World Components")]
    public MonoBehaviour[] openWorldComponents;

    [Header("Battle Components")]
    public MonoBehaviour[] battleComponents;

    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Battle")
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }

            foreach (var component in openWorldComponents)
            {
                if (component != null)
                    component.enabled = false;
            }

            foreach (var component in battleComponents)
            {
                if (component != null)
                    component.enabled = true;
            }
        }
        else
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
            }

            foreach (var component in openWorldComponents)
            {
                if (component != null)
                    component.enabled = true;
            }

            foreach (var component in battleComponents)
            {
                if (component != null)
                    component.enabled = false;
            }
        }
    }
}
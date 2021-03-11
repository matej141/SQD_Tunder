using UnityEngine;

public class InitScene : MonoBehaviour
{
    private bool finished = false;

    void Update()
    {
        // Disable myself to stop looping for every tick in Update() method

        if (finished) gameObject.SetActive(false);

        // Wait for dependecnies to get initialised
        if (FactoryRegister.Singleton == null) return;
        if (LogManager.Singleton == null) return;

        // Initialize myself when all dependencies are met
        FileSerializationManager.LoadElementsFromFile("./Examples/Fowler.json");
        //FileSerializationManager.LoadElementsFromFile("Empty.json");

        // Save my finish status
        finished = true;
    }
}

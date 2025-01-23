using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    public Transform[] layers; // Les différentes couches d'objets parallaxe
    public float[] parallaxFactors; // Les facteurs de parallaxe pour chaque couche
    public Camera killMe; // Ma camera

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = killMe.transform.position;
    }

    void Update()
    {
        Vector3 deltaMovement = killMe.transform.position - lastCameraPosition;

        for (int i = 0; i < layers.Length; i++)
        {
            float parallax = deltaMovement.x * parallaxFactors[i];
            Vector3 layerPosition = layers[i].position;
            layerPosition.x += parallax;
            layers[i].position = layerPosition;
        }

        lastCameraPosition = killMe.transform.position;
    }
}

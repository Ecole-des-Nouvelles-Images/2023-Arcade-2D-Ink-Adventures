using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering.Universal;

public class TrailColor : MonoBehaviour
{
    public TrailRenderer trailRenderer; // Référence au composant TrailRenderer
    public ParticleSystem particles; // Référence au composant ParticleSystem
    public GameObject otherGameObject; // Référence à l'autre GameObject avec le script Light2D
    public float trailOpacity = 0.5f; // Ajustez l'opacité du trail comme vous le souhaitez (0 = transparent, 1 = opaque)
    public float trailEndOpacity = 0.1f;

    void Update()
    {
        // Assurez-vous que les références nécessaires sont définies
        if (trailRenderer == null)
        {
            Debug.LogError("TrailRenderer reference is not set.");
            return;
        }

        if (particles == null)
        {
            Debug.LogError("ParticleSystem reference is not set.");
            return;
        }

        if (otherGameObject == null)
        {
            Debug.LogError("Other GameObject reference is not set.");
            return;
        }

        // Obtenez le composant Light2D du GameObject cible
        Light2D light2D = otherGameObject.GetComponent<Light2D>();

        // Assurez-vous que le composant Light2D existe sur l'autre GameObject
        if (light2D == null)
        {
            Debug.LogError("Light2D component not found on the other GameObject.");
            return;
        }

        // Obtenez la couleur de la lumière
        Color lightColor = light2D.color;

        // Ajustez l'opacité de la couleur
        Color trailColor = new Color(lightColor.r, lightColor.g, lightColor.b, trailOpacity);
        Color trailEndColor = new Color(lightColor.r, lightColor.g, lightColor.b, trailEndOpacity);

        // Appliquer la couleur ajustée au composant TrailRenderer
        trailRenderer.startColor = trailColor;
        trailRenderer.endColor = trailEndColor;

        // Appliquer la couleur ajustée au composant ParticleSystem
        var mainModule = particles.main;
        mainModule.startColor = trailColor;
    }
}

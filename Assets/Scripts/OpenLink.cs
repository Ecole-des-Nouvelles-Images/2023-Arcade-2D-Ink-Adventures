using UnityEngine;

public class OpenLink : MonoBehaviour
{
    // Lien Internet à ouvrir
    public string linkToOpen = "https://www.example.com";

    // Méthode appelée lorsque le bouton est cliqué
    public void OpenInternetLink()
    {
        // Ouvre le lien Internet
        Application.OpenURL(linkToOpen);
    }
}
using Unity.VisualScripting;
using UnityEngine;

public class Tags
{
    public static bool CompareTags(string tag, GameObject obj)
    {
        string[] tabTags = obj.tag.Split(",");
        foreach (string str in tabTags)
        {
            if (tag == str) return true;
        }
        return false;
    }
}

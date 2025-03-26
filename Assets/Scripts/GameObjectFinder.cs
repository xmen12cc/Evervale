using UnityEngine;

public static class GameObjectFinder
{
    /// <summary>
    /// Recursively searches for a child with a matching name.
    /// </summary>
    /// <param name="parent">The parent transform to start the search from.</param>
    /// <param name="childName">The name of the child to find.</param>
    /// <returns>Transform of the found child, or null if not found.</returns>
    public static GameObject FindChildRecursive(GameObject parent, string childName)
    {
        foreach (Transform child in parent.transform)
        {
            if (child.name == childName)
            {
                return child.gameObject;
            }

            GameObject found = FindChildRecursive(child.gameObject, childName);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }
}

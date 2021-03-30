using System;
using System.Reflection;
using UnityEngine;

public class Examples2 : MonoBehaviour
{
    [SerializeField] private Material SkyboxMaterial = null;

    private Camera MainCamera;

    // Start is called before the first frame update
    private void Start()
    {
        GameObject SkyDome = null;

        Type SkyComponentHolderType = null;
        object SkyComponentHolder = null;

        // Similar Component finder to the first example
        Component[] AllComponents = FindObjectsOfType<Component>();
        foreach (Component c in AllComponents)
        {
            if (c.GetType().Name == "TOD_Components")
            {
                SkyDome = c.gameObject;
                SkyComponentHolderType = c.GetType();
                SkyComponentHolder = c;
            }
        }
        if (SkyDome == null)
        {
            Debug.LogError("No SkyDome found");
            return;
        }

        // Similar Field finder to the first example
        FieldInfo[] fields = SkyComponentHolderType.GetFields();
        foreach (FieldInfo field in fields)
        {
            // Get all GameObjects referenced by TOD and destroy them
            if (field.FieldType.Name == "GameObject")
            {
                GameObject RefGameObject = (GameObject)field.GetValue(SkyComponentHolder);
                Debug.Log("Destroyed GameObject " + RefGameObject.name);
                Destroy(RefGameObject);
            }
        }

        // Finally destroy the Sky Dome itself to prevent errors from appearing in the dev console
        Debug.Log("Destroyed GameObject " + SkyDome.name);
        Destroy(SkyDome);

        // Replace the skybox, if a skybox material is defined
        if (SkyboxMaterial)
        {
            RenderSettings.skybox = SkyboxMaterial;
        }

        // Set up the MainCamera to use the skybox
        // Since there is more than one Camera tagged MainCamera, we cannot use Camera.main
        // and must instead find the correct camera by name
        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        MainCamera.clearFlags = CameraClearFlags.Skybox;
        Debug.Log("Applied settings on Camera " + MainCamera.name);
    }
}

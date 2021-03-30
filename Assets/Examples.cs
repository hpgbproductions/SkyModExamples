using System;
using System.Reflection;
using UnityEngine;

public class Examples : MonoBehaviour
{
    // Sky Dome GameObject.
    private GameObject SkyDome;

    // Sky Dome "TOD_Sky" component type
    private Type SkyComponentType;

    // A reference to the actual component of type "TOD_Sky"
    private object SkyComponent;

    // "TOD_AtmosphereParameters" type found in "TOD_Sky"
    private Type AtmosphereType;

    // A reference to the object of type "TOD_AtmosphereParameters" in SkyComponent
    private object Atmosphere;

    // The brightness field defined in the "TOD_AtmosphereParameters" type
    private FieldInfo BrightnessField;

    // Start is called before the first frame update
    private void Start()
    {
        ServiceProvider.Instance.DevConsole.RegisterCommand<float>("ChangeSkyBrightness", ChangeSkyBrightness);
    }

    // This is called by our dev console command
    private void ChangeSkyBrightness(float brightness)
    {
        // Get all components, and looks through them to find a TOD_Sky component
        Component[] AllComponents = FindObjectsOfType<Component>();
        foreach (Component c in AllComponents)
        {
            if (c.GetType().Name == "TOD_Sky")
            {
                // Saves Sky Dome information
                SkyDome = c.gameObject;
                SkyComponentType = c.GetType();
                SkyComponent = c;
            }
        }
        if (SkyDome == null)
        {
            // Returns if no TOD_Sky component is currently loaded
            // Occurs in the designer, or if the Sky Dome was removed by another script
            Debug.LogError("No SkyDome found");
            return;
        }

        // Get all fields in the TOD_Sky type,
        // and looks through them to find the Atmosphere field
        FieldInfo[] fields = SkyComponentType.GetFields();
        foreach (FieldInfo field in fields)
        {
            if (field.Name == "Atmosphere")    // also try (field.FieldType.Name == "TOD_AtmosphereParameters")
            {
                AtmosphereType = field.FieldType;
                Atmosphere = field.GetValue(SkyComponent);

                // Get all fields in the TOD_AtmosphereParameters type,
                // and looks through them to find the Brightness field
                FieldInfo[] atmoFields = AtmosphereType.GetFields();
                foreach (FieldInfo atmoField in atmoFields)
                {
                    if (atmoField.Name == "Brightness")
                        BrightnessField = atmoField;
                }
            }
        }

        float OldValue = (float)BrightnessField.GetValue(Atmosphere);
        Debug.Log("Old brightness was " + OldValue);

        // Sets the value of the Brightness field in the Atmosphere object
        // to the one entered in the command
        BrightnessField.SetValue(Atmosphere, brightness);
        Debug.Log("New brightness is " + brightness);
    }
}

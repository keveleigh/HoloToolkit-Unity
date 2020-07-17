using UnityEngine;

public class TestMaterial : MonoBehaviour
{
    [SerializeField]
    private Material material1 = null;
    
    [SerializeField]
    private Material material2 = null;

    private void OnValidate()
    {
        if (material1 == null || material2 == null)
        {
            return;
        }

        LogProperty("_EmissionMap");
        LogProperty("_EMISSION");
        LogProperty("_EmissiveColor");
        LogProperty("_EmissionColor");

        //material1.EnableKeyword("_EMISSION");
        //material1.EnableKeyword("_ALPHABLEND_ON");
        //material1.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        //material2.EnableKeyword("_ALPHATEST_ON");
        //material2.EnableKeyword("_ALPHABLEND_ON");
        //material2.EnableKeyword("_ALPHAPREMULTIPLY_ON");

        Debug.Log($"Material1: {string.Join(", ", material1.shaderKeywords)}");
        Debug.Log($"Material2: {string.Join(", ", material2.shaderKeywords)}");
    }

    private void LogProperty(string property)
    {
        Debug.Log($"{property} exists: {material1.HasProperty(property)} | {material2.HasProperty(property)}");
        Debug.Log($"{property} enabled: {material1.IsKeywordEnabled(property)} | {material2.IsKeywordEnabled(property)}");
    }
}

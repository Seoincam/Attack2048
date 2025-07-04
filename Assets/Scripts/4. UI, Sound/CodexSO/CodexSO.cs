using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CodexSO", menuName = "Scriptable Objects/CodexSO")]
public class CodexSO : ScriptableObject
{
    public string slimeName;
    // public Image slimeImage;
    [TextArea(2, 15)] public string slimeDescription;
}

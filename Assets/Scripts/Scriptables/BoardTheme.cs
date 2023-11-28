using UnityEngine;

[CreateAssetMenu(fileName = "BoardTheme", menuName = "Theme/BoardTheme")]
public class BoardTheme : ScriptableObject {
    [SerializeField] public SquareTheme lightTheme;
    [SerializeField] public SquareTheme darkTheme;
    
    [System.Serializable]
    public struct SquareTheme {
        public Color defaultColor;
        public Color highlightColor;
        public Color moveColor;
    }
}
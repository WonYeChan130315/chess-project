using UnityEngine;

[CreateAssetMenu(fileName = "PieceTheme", menuName = "Theme/PieceTheme")]
public class PieceTheme : ScriptableObject {
    [SerializeField] public Sprite[] whiteTheme;
    [SerializeField] public Sprite[] blackTheme;
}
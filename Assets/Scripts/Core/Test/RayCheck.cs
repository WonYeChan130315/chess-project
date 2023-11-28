using UnityEngine;

public class RayCheck : MonoBehaviour
{
    public LayerMask layer;

    private void Update() {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouse, Vector2.zero, Mathf.Infinity, layer);

        if(hit.collider != null) {
            print(hit.collider.name);
        }
    }
}

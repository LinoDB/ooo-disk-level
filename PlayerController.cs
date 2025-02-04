using UnityEngine;

public class PlayerController : MonoBehaviour {
    public DiskController occupied = null;
    public bool mouse_on = false;

    private void OnMouseEnter() {
        this.mouse_on = true;
    }
    private void OnMouseExit() {
        this.mouse_on = false;
    }
}

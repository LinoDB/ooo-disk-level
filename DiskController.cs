using UnityEngine;

public class DiskController : MonoBehaviour {
    private bool dragged = false;
    private bool in_player = false;
    private float cam_offset;
    private Vector3 initial_position;
    private GameObject player;
    private PlayerController controller;
    private void Start() {
        this.cam_offset = this.gameObject.transform.position.z - Camera.main.transform.position.z;
        this.initial_position = this.gameObject.transform.position;
        this.player = GameObject.Find("player-center");
        this.controller = this.player.GetComponent<PlayerController>();
    }
    private void OnMouseDown() {
        // Control object being picked up
        if(this.in_player) {
            controller.occupied = null;
        }
        this.in_player = false;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = this.cam_offset;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        this.dragged = true;
    }
    private void OnMouseUp() {
        // Control object being dropped
        if(controller.mouse_on) {
            transform.position = player.transform.position;
            this.in_player = true;
            DiskController occupator = controller.occupied;
            if(occupator) {
                occupator.SendHome();
            }
            controller.occupied = this;
        }
        else {
            this.SendHome();
        }
        this.dragged = false;
    }

    private void Update() {
        if(this.dragged) {
            // Place the disk where the mouse is.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = this.cam_offset;
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    public void SendHome() {
        // Move the object to its initial position
        transform.position = this.initial_position;
        this.in_player = false;
    }
}

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
        this.player = GameObject.Find("player");
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
        if(this.CheckOnPlayer()) {
            transform.position = player.transform.GetChild(1).position;
            this.in_player = true;
            if(controller.occupied) {
                controller.occupied.SendHome();
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

    private bool CheckOnPlayer() {
        // Check whether the disk position is over the player.
        Transform upperleft = this.player.transform.GetChild(0);
        Transform lowerright = this.player.transform.GetChild(2);
        if(
            upperleft.position.x < this.gameObject.transform.position.x &&
            upperleft.position.y > this.gameObject.transform.position.y &&
            lowerright.position.x > this.gameObject.transform.position.x &&
            lowerright.position.y < this.gameObject.transform.position.y
        ) {
            return true;
        }
        return false;
    }

    public void SendHome() {
        // Move the object to its initial position
        transform.position = this.initial_position;
        this.in_player = false;
    }
}

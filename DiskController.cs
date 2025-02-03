using UnityEngine;


public class DiskController : MonoBehaviour {
    private bool dragged = false;
    private bool let_go = false;
    private bool in_player = false;
    private float offset;
    private float cam_offset;
    private Vector3 initial_position;
    void Start() {
        Transform center = this.gameObject.transform.GetChild(1);
        this.offset = center.position.y - this.gameObject.transform.position.y;
        this.cam_offset = this.gameObject.transform.position.z - Camera.main.transform.position.z;
        this.initial_position = this.gameObject.transform.position;
    }
    void Update() {
        if(Input.GetKeyDown(KeyCode.Mouse0)) {
            // If the mouse gets pressed, check if the mouse is over the disk.
            this.CheckDragActivation(Input.mousePosition);
            this.in_player = false;
        }
        else if(Input.GetKeyUp(KeyCode.Mouse0)) {
            // If the mouse gets released, let go of the disk.
            if(this.dragged) {
                this.let_go = true;
            }
        }
    }
    void LateUpdate()
    {
        if(this.let_go) {
            // Place the disk back or on the disk player.
            this.dragged = false;
            this.let_go = false;
            GameObject player = GameObject.Find("player");
            if(this.CheckOnPlayer(player)) {
                Vector3 new_position = player.transform.GetChild(1).position;
                new_position.y -= this.offset;
                transform.position = new_position;
                this.in_player = true;
                PlayerController controller = player.GetComponent<PlayerController>();
                DiskController occupator = controller.occupied;
                if(occupator) {
                    occupator.SendHome();
                }
                controller.occupied = this;
            }
            else {
                this.SendHome();
            }
        }
        else if(this.dragged) {
            // Place the disk where the mouse is.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = this.cam_offset;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            mousePos.y -= this.offset;
            transform.position = mousePos;
        }
    }
    private void CheckDragActivation(Vector3 mousePos) {
        // Check whether the mouse position is over the disk position.
        mousePos.z = this.cam_offset;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Transform upperleft = this.gameObject.transform.GetChild(0);
        Transform lowerright = this.gameObject.transform.GetChild(2);
        if(
            upperleft.position.x < mousePos.x &&
            upperleft.position.y > mousePos.y &&
            lowerright.position.x > mousePos.x &&
            lowerright.position.y < mousePos.y
        ) {
            this.dragged = true;
            if(this.in_player) {
                GameObject player = GameObject.Find("player");
                player.GetComponent<PlayerController>().occupied = null;
            }
        }
    }
    private bool CheckOnPlayer(GameObject player) {
        // Check whether the disk position is over the player.
        Transform upperleft = player.transform.GetChild(0);
        Transform lowerright = player.transform.GetChild(2);
        if(
            upperleft.position.x < this.gameObject.transform.position.x &&
            upperleft.position.y > this.gameObject.transform.position.y + this.offset &&
            lowerright.position.x > this.gameObject.transform.position.x &&
            lowerright.position.y < this.gameObject.transform.position.y + this.offset
        ) {
            return true;
        }
        return false;
    }
    public void SendHome() {
        transform.position = this.initial_position;
        this.in_player = false;
    }
}

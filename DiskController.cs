using UnityEngine;

public class DiskController : MonoBehaviour {
    private bool dragged = false;
    private bool in_player = false;
    private float cam_offset;
    private Collider2D sink;
    private Vector3 initial_position;
    private int layer;
    private void Start() {
        this.cam_offset = this.gameObject.transform.position.z - Camera.main.transform.position.z;
        this.initial_position = this.gameObject.transform.position;
        this.layer = this.GetComponent<SpriteRenderer>().sortingOrder;
    }
    private void OnMouseDown() {
        // Control object being picked up
        this.drag(true);
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        if(this.in_player) {
            GameObject.Find("player-trigger")
                .GetComponent<PlayerController>()
                .occupied = null;
        }
        this.in_player = false;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = this.cam_offset;
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    private void OnMouseUp() {
        // Control object being dropped
        this.drag(false);
        if(this.sink) {
            this.in_player = true;
            PlayerController controller = GameObject.Find("player-trigger")
                .GetComponent<PlayerController>();
            if(controller.occupied) {
                controller.occupied.SendHome();
            }
            controller.occupied = this;
            this.transform.position = sink.transform.position;
        }
        else {
            this.SendHome();
        }
    }

    private void Update() {
        if(this.dragged) {
            // Place the disk where the mouse is.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = this.cam_offset;
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private void drag(bool dragging) {
        SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
        this.make_transparent(dragging && this.sink == null);
        if(dragging) {
            sprite.sortingOrder = this.layer + 1;
        }
        else {
            sprite.sortingOrder = this.layer;
        }
        this.dragged = dragging;
    }

    private void make_transparent(bool transp) {
        SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
        Color white = Color.white;
        if(transp) {
            white.a = .6f;
        }
        sprite.color = white;
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if(trigger.CompareTag("Player")) {
            this.make_transparent(false);
            this.sink = trigger;
        }
    }
    private void OnTriggerExit2D(Collider2D trigger) {
        if(trigger.CompareTag("Player") && this.dragged) {
            this.make_transparent(true);
            this.sink = null;
        }
    }

    public void SendHome() {
        // Move the object to its initial position
        this.GetComponent<BoxCollider2D>().isTrigger = false;
        this.transform.position = this.initial_position;
        this.in_player = false;
        this.sink = null;
    }
}

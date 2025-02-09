using UnityEngine;

public class DiskController : MonoBehaviour {
    private bool dragged = false;
    private bool in_player = false;
    private bool is_transparent = false;
    private float cam_offset;
    private GameObject player;
    private int layer;
    private void Start() {
        this.cam_offset = this.gameObject.transform.position.z - Camera.main.transform.position.z;
        this.layer = this.GetComponent<SpriteRenderer>().sortingOrder;
        this.player = GameObject.Find("player");
    }
    private void OnMouseDown() {
        // Control object being picked up
        this.drag(true);
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        if(this.in_player) {
            this.player
                .GetComponent<PlayerController>()
                .occupied = null;
        }
        this.in_player = false;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = this.cam_offset;
        Rigidbody2D rigid_body = this.GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = true;
        rigid_body.isKinematic = true;
        this.transform.eulerAngles = new Vector3(0, 0, 0);
        this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }
    private void OnMouseUp() {
        // Control object being dropped
        this.drag(false);
        if(this.CheckOnPlayer()) {
            PlayerController controller = this.player
                .GetComponent<PlayerController>();
            if(controller.occupied && controller.occupied.name != this.name) {
                controller.occupied.SetFree();
            }
            this.in_player = true;
            controller.occupied = this;
            this.transform.position = this.player.transform.Find("center").position;
        }
        else {
            this.SetFree();
        }
    }

    private bool CheckOnPlayer() {
        // Check whether the disk position is over the player.
        Transform upperleft = this.player.transform.Find("upperleft");
        Transform lowerright = this.player.transform.Find("lowerright");
        if(
            upperleft.position.x < this.transform.position.x &&
            upperleft.position.y > this.transform.position.y &&
            lowerright.position.x > this.transform.position.x &&
            lowerright.position.y < this.transform.position.y
        ) return true;
        return false;
    }

    private void Update() {
        if(this.dragged) {
            // Place the disk where the mouse is.
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = this.cam_offset;
            this.make_transparent(!this.CheckOnPlayer());
            this.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private void drag(bool dragging) {
        SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
        if(dragging) {
            this.make_transparent(!this.CheckOnPlayer());
            sprite.sortingOrder = this.layer + 1;
        }
        else {
            sprite.sortingOrder = this.layer;
        }
        this.dragged = dragging;
    }

    private void make_transparent(bool transp) {
        if(this.is_transparent != transp) {
            SpriteRenderer sprite = this.GetComponent<SpriteRenderer>();
            Color white = Color.white;
            if(transp) {
                white.a = .6f;
            }
            sprite.color = white;
            this.is_transparent = transp;
        }
    }

    public void SetFree() {
        // Move the object to its initial position
        this.GetComponent<BoxCollider2D>().isTrigger = false;
        Rigidbody2D rigid_body = this.GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = false;
        rigid_body.isKinematic = false;
        this.make_transparent(false);
        this.in_player = false;
    }
}

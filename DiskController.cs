using UnityEngine;

public class DiskController : MonoBehaviour {
    private const float transparency = .6f;
    private bool dragged = false;
    private bool in_player = false;
    private bool is_transparent = false;
    private float cam_offset;
    private int layer;
    private PlayerController controller;
    private Vector3 player_upperleft;
    private Vector3 player_lowerright;
    private Vector3 player_center;

    private void Start() {
        // Initialize variables
        cam_offset = transform.position.z - Camera.main.transform.position.z;
        layer = GetComponent<SpriteRenderer>().sortingOrder;
        GameObject player = GameObject.Find("player");
        controller = player.GetComponent<PlayerController>();
        player_upperleft = player.transform.Find("upperleft").position;
        player_lowerright = player.transform.Find("lowerright").position;
        player_center = player.transform.Find("center").position;
    }

    private void OnMouseDown() {
        // Control object being picked up
        Drag(true);
        GetComponent<BoxCollider2D>().isTrigger = true;
        if(in_player) {
            controller.occupied = null;
        }
        in_player = false;

        Rigidbody2D rigid_body = GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = true;
        rigid_body.isKinematic = true;

        transform.eulerAngles = new Vector3(0, 0, 0);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam_offset;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseUp() {
        // Control object being dropped
        Drag(false);
        if(CheckOnPlayer()) {
            if(controller.occupied && controller.occupied.name != name) {
                controller.occupied.SetFree();
            }
            in_player = true;
            controller.occupied = this;
            transform.position = player_center;
        }
        else {
            SetFree();
        }
    }

    private bool CheckOnPlayer() {
        // Check whether the disk position is over the player
        if(
            player_upperleft.x < transform.position.x &&
            player_upperleft.y > transform.position.y &&
            player_lowerright.x > transform.position.x &&
            player_lowerright.y < transform.position.y
        ) return true;
        return false;
    }

    private void Update() {
        if(dragged) {
            // Place the disk where the mouse is
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam_offset;
            MakeTransparent(!CheckOnPlayer());
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private void Drag(bool dragging) {
        // Toggle dragging state of the object
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if(dragging) {
            MakeTransparent(!CheckOnPlayer());
            sprite.sortingOrder = layer + 1;
        }
        else {
            sprite.sortingOrder = layer;
        }
        dragged = dragging;
    }

    private void MakeTransparent(bool transp) {
        // Set transparency of the object
        if(is_transparent != transp) {
            SpriteRenderer sprite = GetComponent<SpriteRenderer>();
            Color white = Color.white;
            if(transp) {
                white.a = transparency;
            }
            sprite.color = white;
            is_transparent = transp;
        }
    }

    public void SetFree() {
        // Activate physics on the object
        GetComponent<BoxCollider2D>().isTrigger = false;
        Rigidbody2D rigid_body = GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = false;
        rigid_body.isKinematic = false;
        MakeTransparent(false);
        in_player = false;
    }
}

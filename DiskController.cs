using UnityEngine;

public class DiskController : MonoBehaviour {
    /*
        Script for a disk GameObject. The object needs to have a BoxCollider2D
        and a Rigidbody2D attached. This script interacts with any GameObject
        that has the PlayerSlot script and a BoxCollider attached, and is placed
        on layer 1 - TransparentFX.
    */
    private const float transparency = .6f;
    private bool dragged = false;
    private bool is_transparent = false;
    private float cam_offset;
    private int layer;

    private void Start() {
        // Initialize variables
        cam_offset = transform.position.z - Camera.main.transform.position.z;
        layer = GetComponent<SpriteRenderer>().sortingOrder;        
    }

    private void OnMouseDown() {
        // Control object being picked up
        Drag(true);
        GetComponent<BoxCollider2D>().isTrigger = true;
        Collider collider = CheckOverSlot();
        if(collider) {
            PlayerSlot slot = collider.GetComponent<PlayerSlot>();
            if(slot && slot.occupied && slot.occupied.name == name) {
                slot.occupied = null;
            }
        }

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
        Collider collider = CheckOverSlot();
        if(collider) {
            PlayerSlot slot = collider.GetComponent<PlayerSlot>();
            if(slot.occupied) {
                slot.occupied.SetFree();
            }
            slot.occupied = this;
            transform.position = collider.transform.position;
        }
        else {
            SetFree();
        }
    }

    private void Update() {
        // Place the disk where the mouse is if the object is being dragged
        if(dragged) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam_offset;
            MakeTransparent(CheckOverSlot() == null);
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
        }
    }

    private Collider CheckOverSlot() {
        // Check if the curser is over a slot
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 2)) {
            if(hit.collider.CompareTag("Slot"))
			    return hit.collider;
		}
        return null;
    }

    private void Drag(bool dragging) {
        // Toggle dragging state of the object
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if(dragging) {
            MakeTransparent(CheckOverSlot() == null);
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
    }
}

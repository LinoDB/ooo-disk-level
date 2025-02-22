using UnityEngine;

public class DiskController : MonoBehaviour {
    /*
        Script for a disk GameObject. The object needs to have a BoxCollider2D
        and a Rigidbody2D attached. This script interacts with any GameObject
        that has the PlayerSlot script and a BoxCollider attached, and is placed
        on layer 1 - TransparentFX.
    */
    private const float damp = 2;
    private const float max_speed = 5;
    private const float transparency = .6f;
    private bool dragged = false;
    private bool is_transparent = false;
    private bool let_go = false;
    private float cam_offset;
    private int layer;
    private Vector2 velocity = new Vector2(0, 0);
    private Vector2 last_position = new Vector2(0, 0);

    public GameObject game;
    public bool game_loaded = false;

    private void Start() {
        // Initialize variables
        cam_offset = transform.position.z - Camera.main.transform.position.z;
        layer = GetComponent<SpriteRenderer>().sortingOrder;        
    }

    private void OnMouseDown() {
        // Control object being picked up
        let_go = false;
        dragged = true;
        GetComponent<SpriteRenderer>().sortingOrder = layer + 1;
        MakeTransparent(CheckOverSlot() == null);

        GetComponent<BoxCollider2D>().isTrigger = true;
        Collider collider = CheckOverSlot();
        if(collider) {
            PlayerSlot slot = collider.GetComponent<PlayerSlot>();
            if(slot && slot.occupied && slot.occupied.name == name) {
                slot.occupied = null;
                UnloadGame();
            }
        }

        Rigidbody2D rigid_body = GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = true;
        rigid_body.isKinematic = true;
        velocity = new Vector2(0, 0);
        rigid_body.velocity = velocity;

        transform.eulerAngles = new Vector3(0, 0, 0);
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = cam_offset;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void OnMouseUp() {
        // Control object being dropped
        let_go = true;
    }

    private void Update() {
        // Place the disk where the mouse is if the object is being dragged
        if(dragged) {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = cam_offset;
            MakeTransparent(CheckOverSlot() == null);
            transform.position = Camera.main.ScreenToWorldPoint(mousePos);
            velocity.x = LimitSpeed((transform.position.x - last_position.x) / Time.deltaTime / damp);
            velocity.y = LimitSpeed((transform.position.y - last_position.y) / Time.deltaTime / damp);
            last_position.x = transform.position.x;
            last_position.y = transform.position.y;
        }
    }

    private void LateUpdate() {
        // Use late update to simulate letting go after the last update
        if(let_go) {
            dragged = false;

            Collider collider = CheckOverSlot();
            if(collider) {
                PlayerSlot slot = collider.GetComponent<PlayerSlot>();
                if(slot.occupied) {
                    slot.occupied.SetFree();
                }
                GetComponent<SpriteRenderer>().sortingOrder = layer - 1;
                slot.occupied = this;
                velocity = new Vector2(0, 0);
                transform.position = collider.transform.position;
            }
            else {
                SetFree();
            }
        }
        let_go = false;
    }

    private float LimitSpeed(float speed) {
        // Limit the object speed by max_speed
        if(speed > max_speed) {
            return max_speed;
        }
        if(speed < -max_speed) {
            return -max_speed;
        }
        return speed;
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

    private void MakeTransparent(bool transp) {
        // Set transparency of the object
        if(is_transparent != transp) {
            Color white = Color.white;
            if(transp) {
                white.a = transparency;
            }
            GetComponent<SpriteRenderer>().color = white;
            is_transparent = transp;
        }
    }

    public void SetFree() {
        // Activate physics on the object
        GetComponent<SpriteRenderer>().sortingOrder = layer;
        GetComponent<BoxCollider2D>().isTrigger = false;
        Rigidbody2D rigid_body = GetComponent<Rigidbody2D>();
        rigid_body.freezeRotation = false;
        rigid_body.velocity = velocity;
        rigid_body.isKinematic = false;
        MakeTransparent(false);
        UnloadGame();
    }

    public void LoadGame() {
        game.SetActive(true);
        game_loaded = true;
    }

    public void UnloadGame() {
        game.SetActive(false);
        game_loaded = false;
    }
}

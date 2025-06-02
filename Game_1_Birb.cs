using UnityEngine;

public class Game_1_Birb : MonoBehaviour {
    private float speed = 0f;
    private float speed_cap = 4f;
    private float up_boost = 1.8f;
    private float gravity = 5f;
    private bool active = false;
    private Game_1 parent;

    public void Start() {
        parent = GetComponentInParent<Game_1>();
    }

    public void go_up() {
        if(parent.pipe_speed > 0) {
            speed = Mathf.Max(speed, 0) + up_boost;
            speed = Mathf.Min(speed, speed_cap);
            active = true;
        }
    }

    public void Update() {
        if(active) {
            speed -= gravity * Time.deltaTime;
            Vector3 v = transform.position;
            v.y += speed * Time.deltaTime;
            transform.position = v;
        }
        if(parent.pipe_speed > 0) {
            CheckAlive();
        }
    }

    private void CheckAlive() {
        Ray ray = Camera.main.ScreenPointToRay(
            Camera.main.WorldToScreenPoint(transform.position)
        );
        // hit layers 1 + 3 (0b1010)
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 10)) {
            if(
                hit.collider.CompareTag("Death") ||
                hit.collider.CompareTag("Pipe")
            ) {
			    active = false;
                parent.pipe_speed = 0;
                parent.show_end_screen();
            }
		}
    }
}

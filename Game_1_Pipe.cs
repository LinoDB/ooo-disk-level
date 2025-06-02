using UnityEngine;

public class Game_1_Pipe : MonoBehaviour {
    private Game_1 parent;
    private Transform top;

    private void Start()
    {
        top = transform.GetChild(0);
        parent = GetComponentInParent<Game_1>();
    }

    private void Update () {
        Vector3 pos = transform.position;
        pos.x -= Time.deltaTime * parent.pipe_speed;
        transform.position = pos;
        CheckAlive();
    }

    private void CheckAlive() {
        Ray ray = Camera.main.ScreenPointToRay(
            Camera.main.WorldToScreenPoint(top.position)
        );
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 2)) {
            // see if you can check several colliders since it doesn't always work
            if(hit.collider.CompareTag("Death")) {
			    Destroy(gameObject);
                parent.pipe_count += 1;
            }
		}
    }
}

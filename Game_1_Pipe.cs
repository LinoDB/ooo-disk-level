using UnityEngine;

public class Game_1_Pipe : MonoBehaviour {
    private float speed = 1.0f;
    private Transform top;

    private void Start()
    {
        top = transform.GetChild(0);
    }

    private void Update () {
        Vector3 pos = transform.position;
        pos.x -= Time.deltaTime * speed;
        transform.position = pos;
        CheckAlive();
    }

    private void CheckAlive() {
        Ray ray = Camera.main.ScreenPointToRay(
            Camera.main.WorldToScreenPoint(top.position)
        );
        if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 2)) {
            if(hit.collider.CompareTag("Death")) {
			    Destroy(gameObject);
            }
		}
    }
}

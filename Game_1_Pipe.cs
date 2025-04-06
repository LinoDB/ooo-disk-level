using UnityEngine;

public class Game_1_Pipe : MonoBehaviour {
    private float speed = 5.0f;

    private void Update () {
        Vector3 pos = transform.position;
        pos.x -= Time.deltaTime * speed;
        transform.position = pos;
    }

    private void OnTriggerEnter2D(Collider2D trigger) {
        if(trigger.CompareTag("Death")) {
            Destroy(gameObject);
        }
    }
}

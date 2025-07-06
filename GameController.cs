using UnityEngine;

public class GameController : MonoBehaviour {
    public virtual void StartGame() {
        GameObject.Find("display_screen")
            .GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        gameObject.SetActive(true);
    }

    public virtual void ResetGame() {
        EndGame();
        StartGame();
    }

    public virtual void EndGame() {
        GameObject.Find("display_screen")
            .GetComponent<SpriteRenderer>()
            .color = new Color(63f/255, 63f/255, 63f/255, 1f); // 0x3F/0xFF
        gameObject.SetActive(false);
    }
}

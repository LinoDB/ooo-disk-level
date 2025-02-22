using UnityEngine;

public class ButtonController : MonoBehaviour {
    private PlayerSlot player = null;

    private void Start() {
        // Initialize variables
        GameObject player_instance = transform.parent.Find("Slot").gameObject;
        if(player_instance.CompareTag("Slot")) {
            player = player_instance.GetComponent<PlayerSlot>();
        }
        else {
            Debug.LogError(
                "Disk player slot could not be found by 'ButtonController.cs'"
            );
        }
    }

    private void OnMouseUpAsButton() {
        if(player.occupied) {
            if(player.occupied.game_loaded) {
                player.occupied.UnloadGame();
            }
            else {
                player.occupied.LoadGame();
            }
        }
    }
}

using UnityEngine;

public class Game_1 : GameController {
    public GameObject pipe;

    public override void StartGame() {
        Instantiate(pipe);
    }

}
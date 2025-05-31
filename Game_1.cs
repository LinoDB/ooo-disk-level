using System.IO.Pipes;
using System.Threading;
using UnityEngine;

/*
    - Make hardcoded pipe positions instead relative to the display size
    - Make a global speed value on Game_1 that all pipes update with
*/

public class Game_1 : GameController
{
    public GameObject pipe;
    public float speed;
    private float pacer;
    private float start_time;
    private int spawn_rate_pointer;
    private float[] spawn_rates = { 3f, 2f, 1f, .7f };
    private float switch_spawn_rate = 30f;
    private float additional_delay = 0f;

    public override void StartGame() {
        spawn_rate_pointer = 0;
        start_time = Time.time;
        pacer = start_time - spawn_rates[spawn_rate_pointer];
    }

    public override void Update() {
        if(spawn_rate_pointer < spawn_rates.Length - 1) {
            if(Time.time > start_time + switch_spawn_rate) {
                spawn_rate_pointer += 1;
                start_time = Time.time;
            }
        }
        if(
            Time.time > pacer +
            spawn_rates[spawn_rate_pointer] +
            additional_delay
        ) {
            Vector3 pos = transform.position;
            pos.x += 2.2f;
            if(Random.Range(0, 3) == 1){
                top_pipe(pos);
            }
            else {
                bottom_pipe(pos);
            }
            pacer = Time.time;
            additional_delay = Random.Range(
                0, 0.5f * spawn_rates[spawn_rate_pointer]
            );
        }
    }

    private void bottom_pipe(Vector3 pos) {
        Quaternion rot = new Quaternion(0, 0, 0, 1);
        pos.y -= Random.Range(0.6f, 2f);
        Instantiate(pipe, pos, rot, this.transform);
    }

    private void top_pipe(Vector3 pos) {
        Quaternion rot = new Quaternion(0, 0, 180, 1);
        pos.y += Random.Range(0.6f, 2f);
        Instantiate(pipe, pos, rot, this.transform);
    }

    private void dual_pipe() {
        
    }

    public void OnDisable() {
        // destroy remaining pipes
        for(int i = transform.childCount; i > 0; i--){
            GameObject g_o = transform.GetChild(i - 1).gameObject;
            if(g_o.name.ToLower().Contains("pipe")){
                Destroy(g_o);
            }
        }
    }
}
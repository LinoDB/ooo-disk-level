using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
    - Make hardcoded pipe positions instead relative to the display size
    - Make a global speed value on Game_1 that all pipes update with
*/

public class Game_1 : GameController
{
    public GameObject pipe;
    public GameObject birb;
    public GameObject game_text;
    public float pipe_speed;
    public int pipe_count;
    private float pipe_speed_step;
    private float pipe_base_speed = 3f;
    private float pipe_max_speed = 6.6f;
    private float pipe_max_speed_reach = 100f;
    private float pacer;
    private float start_time;
    private int spawn_rate_pointer;
    private float[] spawn_rates = { 3f, 2f, 1f, .7f };
    private float switch_spawn_rate = 15f;
    private float additional_delay = 0f;
    private float height;
    private float width;
    private float[] height_range = { 0.49f, 2.3f };
    private float width_displacement = 2f;
    private Game_1_Birb birb_control;

    public override void StartGame() {
        pipe_count = 0;
        pipe_speed = pipe_base_speed;
        spawn_rate_pointer = 0;
        start_time = Time.time;
        pacer = start_time - spawn_rates[spawn_rate_pointer];
        Instantiate(birb, parent: transform);
        birb_control = GetComponentInChildren<Game_1_Birb>();
        pipe_speed_step = (pipe_max_speed - pipe_base_speed) /
            pipe_max_speed_reach * Time.deltaTime;
        width = GetComponent<BoxCollider2D>().size.x;
        height = GetComponent<BoxCollider2D>().size.y;
    }

    public override void Update() {
        if(pipe_speed > 0) {
            if(pipe_speed < pipe_max_speed) {
                pipe_speed += pipe_speed_step;
            }
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
                pos.x += width_displacement * width;
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
    }

    private void bottom_pipe(Vector3 pos) {
        Quaternion rot = new Quaternion(0, 0, 0, 1);
        pos.y -= Random.Range(
            height_range[0] * height, height_range[1] * height
        );
        Instantiate(pipe, pos, rot, transform);
    }

    private void top_pipe(Vector3 pos) {
        Quaternion rot = new Quaternion(0, 0, 180, 1);
        pos.y += Random.Range(
            height_range[0] * height, height_range[1] * height
        );
        Instantiate(pipe, pos, rot, transform);
    }

    private void dual_pipe() {
        
    }

    public void OnDisable() {
        // destroy all game objects
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
        game_text.SetActive(false);
    }

    public void OnMouseDown() {
        birb_control.go_up();
    }

    public void show_end_screen() {
        TextMeshProUGUI text = game_text.GetComponent<TextMeshProUGUI>();
        text.text = "Final score\n" + pipe_count.ToString();
        game_text.SetActive(true);
    }
}
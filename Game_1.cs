using TMPro;
using UnityEngine;

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
    private float[] spawn_rates = { 3f, 2f, 1f, .75f };
    private float switch_spawn_rate = 15f;
    private float additional_delay;
    private float height;
    private float width;
    private float[] height_range = { 0.49f, 2.3f };
    private float width_displacement = 2f;
    private float last_displacement;
    private bool last_displacement_top = false;
    private float displacement_cap;
    private float displacement_cap_per_stage = 0.125f;
    private float[] pipe_gap_width = { 0.1f, 1f };
    private Game_1_Birb birb_control;

    public override void StartGame() {
        width = GetComponent<BoxCollider2D>().size.x;
        height = GetComponent<BoxCollider2D>().size.y;
        base.StartGame();
        pipe_count = 0;
        additional_delay = 0f;
        last_displacement = height_range[0];
        pipe_speed = pipe_base_speed;
        spawn_rate_pointer = 0;
        CalculateDisplacementCap();
        start_time = Time.time;
        pacer = start_time - spawn_rates[spawn_rate_pointer];
        GameObject birb_p = Instantiate(birb, parent: transform);
        birb_control = birb_p.GetComponent<Game_1_Birb>();
        pipe_speed_step = (pipe_max_speed - pipe_base_speed) /
            pipe_max_speed_reach * Time.deltaTime;
    }

    private void CalculateDisplacementCap() {
        displacement_cap = (
            height_range[0] +
            (1 - ((spawn_rate_pointer + 1) * displacement_cap_per_stage)) *
            (height_range[1] - height_range[0])
        ) * height;
    }

    private float GetDisplacement(bool top, float cap = 0f) {
        // something is still wrong, debug with better logs
        float displacement = Random.Range(
            height_range[0] * height + cap, height_range[1] * height
        );
        if(
            last_displacement_top != top &&
            last_displacement < displacement_cap &&
            displacement < displacement_cap
        ) {
            displacement = displacement_cap;
        }
        last_displacement_top = top;
        last_displacement = displacement;
        return displacement;
    }

    public void Update() {
        if(pipe_speed > 0) {
            if(pipe_speed < pipe_max_speed) {
                pipe_speed += pipe_speed_step;
            }
            if(spawn_rate_pointer < spawn_rates.Length - 1) {
                if(Time.time > start_time + switch_spawn_rate) {
                    spawn_rate_pointer += 1;
                    CalculateDisplacementCap();
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
                int flip = Random.Range(0, 6);
                if(flip < 3){
                    if(last_displacement_top)
                        bottom_pipe(pos, GetDisplacement(false));
                    else
                        top_pipe(pos, GetDisplacement(true));
                }
                else if(flip < 5) {
                    if(last_displacement_top)
                        top_pipe(pos, GetDisplacement(true));
                    else
                        bottom_pipe(pos, GetDisplacement(false));
                }
                else {
                    dual_pipe(pos);
                }
                pacer = Time.time;
                additional_delay = Random.Range(
                    0, 0.5f * spawn_rates[spawn_rate_pointer]
                );
            }
        }
    }

    private void bottom_pipe(Vector3 pos, float displacement) {
        Quaternion rot = new Quaternion(0, 0, 0, 1);
        pos.y -= displacement;
        Instantiate(pipe, pos, rot, transform);
    }

    private void top_pipe(Vector3 pos, float displacement) {
        Quaternion rot = new Quaternion(0, 0, 180, 1);
        pos.y += displacement;
        Instantiate(pipe, pos, rot, transform);
    }

    private void dual_pipe(Vector3 pos) {
        float cap = Random.Range(
            pipe_gap_width[0] * height, pipe_gap_width[1] * height
        );
        if(last_displacement_top) {
            float bottom_displacement = GetDisplacement(false, cap);
            bottom_pipe(pos, bottom_displacement);
            top_pipe(
                pos,
                (
                    height_range[1] * height - bottom_displacement +
                    height_range[0] * height + cap
                )
            );
        }
        else {
            float top_displacement = GetDisplacement(true, cap);
            top_pipe(pos, top_displacement);
            bottom_pipe(
                pos,
                (
                    height_range[1] * height - top_displacement +
                    height_range[0] * height + cap
                )
            );
        }        
    }

    public override void EndGame() {
        UnloadObjects();
        game_text.SetActive(false);
        base.EndGame();
    }

    private void UnloadObjects() {
        // destroy all game objects
        birb_control = null;
        for(int i = 0; i < transform.childCount; i++){
            Destroy(transform.GetChild(i).gameObject);
        }
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
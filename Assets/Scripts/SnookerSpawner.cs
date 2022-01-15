using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class SnookerSpawner : MonoBehaviour {

    [HideInInspector]
    public bool isCorrectlyAlign = false;
    [HideInInspector]
    public bool isInRange = false;
    [HideInInspector]
    public SpriteRenderer sprite;
    [SerializeField]
    private SnookerRange range;
    private int colCounter = 0;

    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        if (colCounter > 0) isCorrectlyAlign = false;
        else isCorrectlyAlign = true;

        if (range.CheckOffset(transform.position)) isInRange = true;
        else isInRange = false;
    }

    private void LateUpdate() {
        if (isCorrectlyAlign && isInRange) 
        {
            sprite.color = Color.white;
        }
        else
        {
            sprite.color = Color.red;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
       colCounter++; 
    }

    private void OnTriggerExit2D(Collider2D other) {
        colCounter--;
    }
}
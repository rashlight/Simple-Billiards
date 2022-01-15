using UnityEngine;

public class SnookerPivot : MonoBehaviour {
    public ShotSnooker owner;
    public SpriteRenderer sprite;
    
    private void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }
    
    private void Update() {
        if (owner.pivotObject != this) Destroy(this.gameObject);
    }
}
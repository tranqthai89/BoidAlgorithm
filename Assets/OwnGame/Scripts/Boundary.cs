using UnityEngine;

public class Boundary {
    private float xLimit; // Giới hạn trên trục x
    private float yLimit; // Giới hạn trên trục y

    public float XLimit{
        get{
            return xLimit;
        }
    }
    public float YLimit{
        get{
            return yLimit;
        }
    }

    public Boundary(){
        CalculateLimit();
    }

    private void CalculateLimit(){
        yLimit = Camera.main.orthographicSize + 1f;
        xLimit = yLimit * Screen.width / Screen.height + 1f;
    }
}
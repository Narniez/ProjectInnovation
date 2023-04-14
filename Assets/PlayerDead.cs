using UnityEngine;

public class PlayerDead : MonoBehaviour
{
    public void TankDead(float health)
    {
        if (health > 0) return;

        if (health <= 0)
            Debug.Log("The tank is destroyed");
    }
}

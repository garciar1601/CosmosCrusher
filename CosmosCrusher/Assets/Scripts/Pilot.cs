using UnityEngine;
using System.Collections;

public interface Pilot {
    void MoveShip(GameObject ship);
    void Fire(GameObject ship, GameObject bullet, GameObject bulletPool);
}

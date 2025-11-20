using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
    public Room[] Rooms;

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchRoom();
        }*/
    }
    
    public void SwitchRoom()
    {
        var rooms = Enumerable.Range(0, Rooms.Length).OrderBy(x => Random.value).ToArray();
    
        SwapRoomTransforms(Rooms[rooms[0]].gameObject, Rooms[rooms[1]].gameObject);
    
        if (Random.Range(0, 2) == 1)
            SwapRoomTransforms(Rooms[rooms[2]].gameObject, Rooms[rooms[3]].gameObject);
    }
    
    private void SwapRoomTransforms(GameObject roomA, GameObject roomB)
    {
        (roomB.transform.position, roomA.transform.position) = (roomA.transform.position, roomB.transform.position);
        (roomB.transform.rotation, roomA.transform.rotation) = (roomA.transform.rotation, roomB.transform.rotation);
    }
}

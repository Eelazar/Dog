using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager current;

    public BaseObject player;

    public BaseObject door1;

    public BaseObject door2;

    public BaseObject throwBro;

    private void Awake()
    {
        current = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ObjectManager.AddObject(door1);
        //ObjectManager.AddObject(door2);

        ObjectManager.AddObject(throwBro);

        ObjectManager.AddObject(player);
    }

    // Update is called once per frame
    void Update()
    {

    }
}

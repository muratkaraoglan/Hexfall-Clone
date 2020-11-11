using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Hexagon : MonoBehaviour
{
    Manage m;
    enum Colors
    {
        Yellow,
        Orange,
        Red,
        Green,
        Purple,
        Blue
    }
    public bool oddCol;
    float x;
    float y;
    public Text bombText;
    public int bombTimer;
    public string hexID;
    bool bomb;
    [Header("Neighbours")]
    [SerializeField]
    private Vector2 Top;
    [SerializeField]
    private Vector2 TRight;
    [SerializeField]
    private Vector2 BRight;
    [SerializeField]
    private Vector2 Bottom;
    [SerializeField]
    private Vector2 BLeft;
    [SerializeField]
    private Vector2 Tleft;
    [Header("Color")]
    [SerializeField]
    private Color32 c;

    [Header("GRID")]
    [SerializeField]
    private int xGrid;
    [SerializeField]
    private int yGrid;


    void Start()
    {
        x = transform.position.x;
        y = transform.position.y;
        m = Manage.instance;
        int rnd = Random.Range(0, 6);
        switch ( rnd )
        {
            case (int)Colors.Yellow:
                c = new Color32(234, 191, 27, 255);
                break;
            case (int)Colors.Orange:
                c = new Color32(226, 124, 38, 255);
                break;
            case (int)Colors.Red:
                c = new Color32(227, 74, 59, 255);
                break;
            case (int)Colors.Green:
                c = new Color32(68, 185, 109, 255);
                break;
            case (int)Colors.Purple:
                c = new Color32(148, 88, 164, 255);
                break;
            case (int)Colors.Blue:
                c = new Color32(51, 148, 209, 255);
                break;
        }

        this.gameObject.transform.GetChild(0).GetComponent<SpriteRenderer>().color = c;
    }

    Vector2 destination;
    void Update()
    {
        if ( dropping )
        {
            transform.position = Vector2.MoveTowards(transform.position, destination, Time.deltaTime * 4f);
            if ( Vector2.Distance(transform.position, destination) <= m.yOffset / 2 )
            {
                dropping = false;
                transform.position = destination;
                x = transform.position.x;
                y = transform.position.y;
                GetNeighbour();
            }
        }
        else
        {
            x = transform.position.x;
            y = transform.position.y;
        }
        //Debug.Log(bomb ? true : false);
        if ( bomb )
        {
            if ( bombTimer == 0 )
            {
                m.gameOver = true;
            }
            setText();
        }
    }

    bool dropping = false;
    public void SetDestination(Vector2 des)
    {
        destination = des;
        dropping = true;
    }
    public Vector2[] GetNeighbour()
    {
        Top = new Vector2(x, y + m.yOffset);
        TRight = new Vector2(x + m.xOffset, y + m.yOffset / 2f);
        BRight = new Vector2(x + m.xOffset, y - m.yOffset / 2f);
        Bottom = new Vector2(x, y - m.yOffset);
        BLeft = new Vector2(x - m.xOffset, y - m.yOffset / 2f);
        Tleft = new Vector2(x - m.xOffset, y + m.yOffset / 2f);
        return new Vector2[] { Top, TRight, BRight, Bottom, BLeft, Tleft };
    }
    public void setBomb()
    {
        bomb = true;
        bombTimer = 5;
        bombText.text = bombTimer.ToString();
    }
    public void setText() 
    {
        bombText.text = bombTimer.ToString();
    }
    public Color32 GetColor() { return c; }
    public void SetXGrid(int x) { xGrid = x; }
    public int GetXGrid() { return xGrid; }
    public void SetYGrid(int y) { yGrid = y; }
    public int GetYGrid() { return yGrid; }


}

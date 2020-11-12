using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class Manage : MonoBehaviour
{
    public static Manage instance = null;
    public GameObject hexPrefab;
    public GameObject selectedBGPrefab;
    public GameObject midPointPrefab;
    public GameObject hexBombPrefab;
    public TextMeshProUGUI sTxt;
    int score;
    public int width = 8;
    public int height = 9;
    public int bombBornScore;
    List<Hexagon> bombList = new List<Hexagon>();
    int bombGoal;
    public float xOffset = 0.77f;
    public float yOffset = 0.9f;
    public bool gameOver;

    public GameObject panelGameOver;
    private void Awake()
    {
        if ( instance == null )
        {
            instance = this;
            gameOver = false;
        }
        else
        {
            Destroy(this);
        }
    }

    List<Hexagon> colListH;
    List<List<Hexagon>> hexListH;

    public Transform[,] gridT;
    void Start()
    {
        
        bombBornScore = 200;
        score = 0;
        sTxt.text = score.ToString();
        hexListH = new List<List<Hexagon>>();
        gridT = new Transform[width, height];
        StartCoroutine(ProduceHex());
        bombGoal = bombBornScore;
    }


    GameObject selectedHexagon, Neigh1, Neigh2;
    GameObject selectedBG1, selectedBG2, selectedBG3;
    bool selection = false;
    public bool turn = false;
    public bool clockWise = true;
    bool runOne = true;

    void Update()
    {
        if ( gameOver )
        {
            //Debug.Log("Game Over");
            panelGameOver.SetActive(true);
        }
        else
        {
            if ( produced && !selection && !explosion )
            {
                CheckExplosion(clockWise);
                runOne = false;
            }
            if ( Input.GetMouseButtonDown(0) && !explosion && !selection )
            {
                Debug.Log(Input.mousePosition);
                DestroyOutSide();
                selection = true;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
                //Debug.DrawLine(mousePos, Vector3.forward,Color.red,5);
                RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector3.forward, 30);
                if ( hit.collider != null )
                {
                    Vector2[] neighbours = hit.collider.gameObject.GetComponent<Hexagon>().GetNeighbour();
                    //seçilen hex
                    selectedHexagon = hit.collider.gameObject;
                    List<float> distanceList = new List<float>();
                    int NIndex1, NIndex2;
                    float distance;
                    for ( int i = 0; i < neighbours.Length; i++ )
                    {
                        distance = Vector3.Distance(mousePos2D, neighbours[i]);
                        distanceList.Add(distance);
                        //Debug.Log(distance);
                    }
                    //seçilen hex'in tıklanılan pozisyona göre en yakın komşularının pozisyonları için neighbours dizisindeki indislerinin belirlenmesi
                    var min1 = distanceList.Min();
                    NIndex1 = distanceList.IndexOf(min1);
                    distanceList[NIndex1] = 9f;

                    var min2 = distanceList.Min();
                    NIndex2 = distanceList.IndexOf(min2);

                    //seçilen hex'in komşuları
                    Neigh1 = FindGameObject(neighbours[NIndex1]);
                    Neigh2 = FindGameObject(neighbours[NIndex2]);
                    //Seçilen hex'lerin arka planlarının değiştirilmesi
                    selectedBG1 = Instantiate(selectedBGPrefab, selectedHexagon.transform.position, Quaternion.identity);
                    selectedBG2 = Instantiate(selectedBGPrefab, Neigh1.transform.position, Quaternion.identity);
                    selectedBG3 = Instantiate(selectedBGPrefab, Neigh2.transform.position, Quaternion.identity);

                    StartCoroutine(RotateSelectedHexagons());
                }
            }
            if ( Input.GetMouseButtonDown(1) )
            {
                //GameObject go = Instantiate(hexPrefab, new Vector2(-1f, 10f), Quaternion.identity);
                //go.GetComponent<Hexagon>().SetDestination(new Vector2(-1f, 0));
                CheckExplosion(clockWise);
                //GameObject go = Instantiate(hexBombPrefab, new Vector2(5f, 10f), Quaternion.identity);
                //go.GetComponent<Hexagon>().setBomb();
                //go.GetComponent<Hexagon>().bombText.text = (--go.GetComponent<Hexagon>().bombTimer).ToString();
            }

            if (Input.touchCount>0 && !explosion && !selection) 
            {
                Touch touch = Input.GetTouch(0);
                Debug.Log(touch.position);
                DestroyOutSide();
                selection = true;
                Vector3 pos3D = Camera.main.ScreenToWorldPoint(touch.position);
                Vector2 pos2D = new Vector2(pos3D.x, pos3D.y);
                //Debug.DrawLine(mousePos, Vector3.forward,Color.red,5);
                RaycastHit2D hit = Physics2D.Raycast(pos3D, Vector3.forward, 30);
                if ( hit.collider != null )
                {
                    Vector2[] neighbours = hit.collider.gameObject.GetComponent<Hexagon>().GetNeighbour();
                    //seçilen hex
                    selectedHexagon = hit.collider.gameObject;
                    List<float> distanceList = new List<float>();
                    int NIndex1, NIndex2;
                    float distance;
                    for ( int i = 0; i < neighbours.Length; i++ )
                    {
                        distance = Vector3.Distance(pos2D, neighbours[i]);
                        distanceList.Add(distance);
                        //Debug.Log(distance);
                    }
                    //seçilen hex'in tıklanılan pozisyona göre en yakın komşularının pozisyonları için neighbours dizisindeki indislerinin belirlenmesi
                    var min1 = distanceList.Min();
                    NIndex1 = distanceList.IndexOf(min1);
                    //distanceList[NIndex1] = 9f;

                    //var min2 = distanceList.Min();
                    //NIndex2 = distanceList.IndexOf(min2);

                    //seçilen hex'in komşuları
                    Neigh1 = FindGameObject(neighbours[NIndex1]);
                    if ( NIndex1 == 5 )
                    {
                        Neigh2 = FindGameObject(neighbours[0]);
                    }
                    else 
                    {
                        Neigh2 = FindGameObject(neighbours[NIndex1+1]);
                    }
                   
                    //Seçilen hex'lerin arka planlarının değiştirilmesi
                    selectedBG1 = Instantiate(selectedBGPrefab, selectedHexagon.transform.position, Quaternion.identity);
                    selectedBG2 = Instantiate(selectedBGPrefab, Neigh1.transform.position, Quaternion.identity);
                    selectedBG3 = Instantiate(selectedBGPrefab, Neigh2.transform.position, Quaternion.identity);

                    StartCoroutine(RotateSelectedHexagons());
                }
            }
        }
    }
    IEnumerator RotateSelectedHexagons()
    {
        turn = true;

        for ( int i = 0; i < 3; i++ )
        {
            if ( clockWise && !explosion && turn )
            {
                Vector2 first = selectedHexagon.transform.position;
                Vector2 second = Neigh1.transform.position;
                Vector3 third = Neigh2.transform.position;

                int x1, x2, x3;
                int y1, y2, y3;
                x1 = selectedHexagon.GetComponent<Hexagon>().GetXGrid();
                x2 = Neigh1.GetComponent<Hexagon>().GetXGrid();
                x3 = Neigh2.GetComponent<Hexagon>().GetXGrid();

                y1 = selectedHexagon.GetComponent<Hexagon>().GetYGrid();
                y2 = Neigh1.GetComponent<Hexagon>().GetYGrid();
                y3 = Neigh2.GetComponent<Hexagon>().GetYGrid();

                Hexagon temp = hexListH[x1][y1];
                Hexagon temp2 = hexListH[x2][y2];
                Hexagon temp3 = hexListH[x3][y3];

                //Debug.Log("Before =>" + hexListH[x1][y1].hexID);
                //Debug.Log("Before =>" + hexListH[x2][y2].hexID);
                //Debug.Log("Before =>" + hexListH[x3][y3].hexID);
                //Debug.Log("");
                selectedHexagon.GetComponent<Hexagon>().SetDestination(second);
                selectedHexagon.GetComponent<Hexagon>().SetXGrid(x2);
                selectedHexagon.GetComponent<Hexagon>().SetYGrid(y2);
                hexListH[x2][y2] = temp;

                Neigh1.GetComponent<Hexagon>().SetDestination(third);
                Neigh1.GetComponent<Hexagon>().SetXGrid(x3);
                Neigh1.GetComponent<Hexagon>().SetYGrid(y3);
                hexListH[x3][y3] = temp2;

                Neigh2.GetComponent<Hexagon>().SetDestination(first);
                Neigh2.GetComponent<Hexagon>().SetXGrid(x1);
                Neigh2.GetComponent<Hexagon>().SetYGrid(y1);
                hexListH[x1][y1] = temp3;

                //Debug.Log("After =>" + hexListH[x1][y1].hexID);
                //Debug.Log("After =>" + hexListH[x2][y2].hexID);
                //Debug.Log("After =>" + hexListH[x3][y3].hexID);
                //Debug.Log("-------------------------------------------------------------");
                yield return new WaitForSeconds(0.5f);
                if ( CheckExplosion(clockWise) )
                {
                    break;
                }
            }
            yield return new WaitForSeconds(0.4f);
        }
        turn = false;
        DestroyOutSide();
    }

    void Refresh() 
    {
            
    }

    void DestroyOutSide()
    {
        if ( selectedBG1 != null )
        {
            Destroy(selectedBG1);
            Destroy(selectedBG2);
            Destroy(selectedBG3);
            selection = false;
            StopCoroutine(RotateSelectedHexagons());
        }
    }

    public GameObject FindGameObject(Vector2 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector3.forward, 30);
        return hit.collider.gameObject;
    }

    bool produced = false;
    List<string> hexIDs = new List<string>();
    IEnumerator ProduceHex()/*Hexagon'ların hazırlanması*/
    {
        for ( int x = 0; x < width; x++ )
        {
            colListH = new List<Hexagon>();
            for ( int y = 0; y < height; y++ )
            {
                //y*yOffset+yOffset/2f
                //yOffset * (y + 0.5f)
                float xPos = x * xOffset;
                float yPos = (x % 2 == 0 ? y * yOffset : y * yOffset + yOffset / 2f);

                GameObject newHex = Instantiate(hexPrefab, new Vector2(xPos, yPos), Quaternion.identity);
                newHex.GetComponent<Hexagon>().oddCol = x % 2 == 1 ? true : false;
                newHex.GetComponent<Hexagon>().SetXGrid(x);
                newHex.GetComponent<Hexagon>().SetYGrid(y);
                string id = Guid.NewGuid().ToString();
                newHex.GetComponent<Hexagon>().hexID = id;
                hexIDs.Add(id);
                colListH.Add(newHex.GetComponent<Hexagon>());
            }
            hexListH.Add(colListH);
        }
        produced = true;
        yield return new WaitForSeconds(1f);
    }

    bool explosion = false;
    bool CheckExplosion(bool clockWise)/*Explosion kontrolü*/
    {
        Hexagon hex;
        List<Hexagon> expList = new List<Hexagon>();
        for ( int i = 0; i < width; i++ )
        {
            for ( int j = 0; j < height; j++ )
            {
                if ( !explosion )
                {
                    hex = hexListH[i][j];
                    RaycastHit2D hit1 = Physics2D.Raycast(hex.GetNeighbour()[0], Vector3.forward, 30);
                    RaycastHit2D hit2 = Physics2D.Raycast(hex.GetNeighbour()[1], Vector3.forward, 30);
                    RaycastHit2D hit3 = Physics2D.Raycast(hex.GetNeighbour()[2], Vector3.forward, 30);
                    RaycastHit2D hit4 = Physics2D.Raycast(hex.GetNeighbour()[3], Vector3.forward, 30);
                    RaycastHit2D hit5 = Physics2D.Raycast(hex.GetNeighbour()[4], Vector3.forward, 30);
                    RaycastHit2D hit6 = Physics2D.Raycast(hex.GetNeighbour()[5], Vector3.forward, 30);
                    //RaycastHit2D hitOwn = Physics2D.Raycast(hex.transform.position, Vector3.forward, 30);

                    if ( clockWise )
                    {

                        if ( hit1.collider != null && hit2.collider != null )
                        {
                            if ( hit1.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor()) && (hit2.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                //expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                                expList.Add(hit1.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hit2.collider.gameObject.GetComponent<Hexagon>());
                            }
                        }

                        if ( hit2.collider != null && hit3.collider != null )
                        {
                            if ( hit2.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor()) && (hit3.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                if ( !expList.Contains(hit2.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit2.collider.gameObject.GetComponent<Hexagon>());

                                expList.Add(hit3.collider.gameObject.GetComponent<Hexagon>());

                                //if ( !expList.Contains(hitOwn.collider.gameObject.GetComponent<Hexagon>()) )
                                //    expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                            }
                        }

                        if ( hit3.collider != null && hit4.collider != null )
                        {
                            if ( (hit3.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) && (hit4.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                if ( !expList.Contains(hit3.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit3.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hit4.collider.gameObject.GetComponent<Hexagon>());

                                //if ( !expList.Contains(hitOwn.collider.gameObject.GetComponent<Hexagon>()) )
                                //    expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                            }
                        }

                        if ( hit4.collider != null && hit5.collider != null )
                        {
                            if ( (hit4.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) && (hit5.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                if ( !expList.Contains(hit4.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit4.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hit5.collider.gameObject.GetComponent<Hexagon>());

                                //if ( !expList.Contains(hitOwn.collider.gameObject.GetComponent<Hexagon>()) )
                                //    expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                            }
                        }
                        if ( hit5.collider != null && hit6.collider != null )
                        {
                            if ( (hit5.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) && (hit6.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                if ( !expList.Contains(hit5.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit5.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hit6.collider.gameObject.GetComponent<Hexagon>());

                                //if ( !expList.Contains(hitOwn.collider.gameObject.GetComponent<Hexagon>()) )
                                //    expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                            }
                        }
                        if ( hit6.collider != null && hit1.collider != null )
                        {
                            if ( (hit6.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) && (hit1.collider.gameObject.GetComponent<Hexagon>().GetColor().Equals(hex.GetColor())) )
                            {
                                if ( !expList.Contains(hit6.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit6.collider.gameObject.GetComponent<Hexagon>());
                                if ( !expList.Contains(hit1.collider.gameObject.GetComponent<Hexagon>()) )
                                    expList.Add(hit1.collider.gameObject.GetComponent<Hexagon>());
                                //if ( !expList.Contains(hitOwn.collider.gameObject.GetComponent<Hexagon>()) )
                                //    expList.Add(hitOwn.collider.gameObject.GetComponent<Hexagon>());
                                expList.Add(hexListH[i][j]);
                            }
                        }
                    }
                }
            }
        }

        if ( expList.Count > 0 && !explosion )
        {
            turn = false;
            expList = expList.Distinct().ToList();//Hex'lerin sadelestirilmesi
            StartCoroutine(DestroyAndProduce(expList));
            expList.Clear();
            return true;
        }
        else return false;

    }
    
    IEnumerator DestroyAndProduce(List<Hexagon> expList)/*yok edilen hex'lerin yerine yenilerinin getirilmesi */
    {
        List<int> xList = new List<int>();
        
        explosion = true;
        expList = expList.OrderByDescending(y => y.GetYGrid()).ToList();//yukarıdan asagiya dogru yok etme
        //Yok etme
        //Debug.Log("Destroy Count =>" + expList.Count);
        foreach ( var item in expList )
        {
            int x = item.GetXGrid();
            int y = item.GetYGrid();
            string id = item.hexID;
            if ( bombList.Count>0 )
            {
                foreach ( var bomb in bombList )
                {
                    if ( bomb.Equals(item) )
                    {
                        bombList.Remove(bomb);
                        break;
                    }
                }
            }
            Destroy(item.gameObject);
            hexListH[x].RemoveAt(y);
            hexIDs.Remove(id);
            xList.Add(x);
            //Debug.Log("Yok edilen hex =>" + x + "  " + y);
            score += 5;
            sTxt.text = score.ToString();
        }

        if ( bombList.Count>0 )
        {
            foreach ( var bomb in bombList )
            {
                bomb.bombTimer--;
            }
        }

        ////////////////////////////
        //Kaydırma
        List<int> xCopy = xList.Distinct().ToList();
        foreach ( int gX in xCopy )
        {
            //Debug.Log(gX + ". Col count: " + hexListH[gX].Count);
            for ( int i = hexListH[gX].Count - 1; i >= 0; i-- )
            {
                //Debug.Log("Y:" + hexListH[gX][i].GetYGrid() + " i:" + i);
                if ( hexListH[gX][i].GetYGrid() != i )
                {
                    //Debug.Log("Kaydirma =>" + hexListH[gX][i].GetXGrid() + " " + hexListH[gX][i].GetYGrid() + " to " + i);
                    hexListH[gX][i].SetYGrid(i);
                    hexListH[gX][i].SetDestination(new Vector2(gX * xOffset, gX % 2 == 0 ? i * yOffset : i * yOffset + yOffset / 2f));

                }
            }
        }
        yield return new WaitForSeconds(1f);
        /////////////////////////////////////////////////////////////
        /// Yeni hex
        //Debug.Log("Score => " + score);
        foreach ( int gX in xList )
        {
            int last = hexListH[gX].Count;
            
            if ( score >= bombGoal )
            {
                Debug.Log("Bomb");
                GameObject go = Instantiate(hexBombPrefab, new Vector2(gX * xOffset, 10f), Quaternion.identity);
                go.GetComponent<Hexagon>().SetXGrid(gX);
                go.GetComponent<Hexagon>().SetYGrid(last);
                go.GetComponent<Hexagon>().SetDestination(new Vector2(gX * xOffset, gX % 2 == 0 ? (last) * yOffset : last * yOffset + yOffset / 2f));
                string id = Guid.NewGuid().ToString();
                go.GetComponent<Hexagon>().hexID = id;
                go.GetComponent<Hexagon>().setBomb();
                bombList.Add(go.GetComponent<Hexagon>());
                hexIDs.Add(id);
                hexListH[gX].Add(go.GetComponent<Hexagon>());
                bombGoal += bombBornScore;
            }
            else
            {
                GameObject go = Instantiate(hexPrefab, new Vector2(gX * xOffset, 10f), Quaternion.identity);
                go.GetComponent<Hexagon>().SetXGrid(gX);
                go.GetComponent<Hexagon>().SetYGrid(last);
                go.GetComponent<Hexagon>().SetDestination(new Vector2(gX * xOffset, gX % 2 == 0 ? (last) * yOffset : last * yOffset + yOffset / 2f));
                string id = Guid.NewGuid().ToString();
                go.GetComponent<Hexagon>().hexID = id;
                hexIDs.Add(id);
                //last*yOffset+yOffset/2f
                //last + 0.5f
                hexListH[gX].Add(go.GetComponent<Hexagon>());
            }

            //Debug.Log("Yeni Hex => " + "x: " + gX + " y: " + last);
           
        }



        yield return new WaitForSeconds(1f);

        //int size = hexListH.SelectMany(list => list).Count();
        //Debug.Log("Before Distinct: " + size);
        //hexListH = hexListH.Distinct().ToList();
        //size = hexListH.SelectMany(list => list).Count();
        //Debug.Log("After Distinct: " + size);

        //GameObject[] gos = GameObject.FindGameObjectsWithTag("HexagonTAG");

        //Debug.Log("Hexagon sayisi: " + gos.Length);
        //Debug.Log("Id sayisi: " + hexIDs.Count);

        //Debug.Log("///////////////////////////////////////////////");
        explosion = false;
        selection = false;
    }
}


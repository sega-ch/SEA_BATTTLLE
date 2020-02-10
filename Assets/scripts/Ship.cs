using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ship : Dispatcher
{
    Canvas canvas;
    Animator[] animators;
    public bool IsPositionCorrect = true, IsWithInCell = false;
    public Vector2 CellPosition, LastPosition;
    public enum Orientation
    {
        Horizontal, Vertical
    }

    public Orientation orientation = Orientation.Horizontal;
    public GameObject floorButtonPref;
    Orientation lastOrientation;
    bool toMove = false, wasAllocatedOnce = false;
    
    float angle = -90f;
    float floorSize;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        canvas = GetComponentInParent<Canvas>();
        floorSize = 0;
        animators = new Animator[FloorsNum()];
        for (int i = 0; i < FloorsNum(); i++)
        {
            var floor = transform.GetChild(i);
            var floorPos = transform.position;
            floorSize = floor.GetComponent<SpriteRenderer>().bounds.size.x;
            if (orientation == Orientation.Horizontal) floorPos.x += i * floorSize;
            else if (orientation == Orientation.Vertical) floorPos.y += i * floorSize;
            floor.transform.position = floorPos; // placing floor where need to
            if (!gameObject.name.Contains("(Clone)"))
            {
                var floorButtonObj = Instantiate(floorButtonPref, floor.transform);
                floorButtonObj.transform.position = floorPos; // allocating button
                var buttonRectTransf = floorButtonObj.GetComponent<RectTransform>();
                buttonRectTransf.sizeDelta = new Vector2(floorSize, floorSize); // sizing button
                var buttonScript = floorButtonObj.GetComponent<Button>();
                buttonScript.onClick.AddListener(OnFloorClick); // making button clickable
            }
            else
            {
                floor.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                floor.GetComponentInChildren<Button>().onClick.AddListener(OnFloorClick);
            }
            var animator = floor.GetComponent<Animator>();
            animators[i] = animator;
        }
        
    }
    public int FloorsNum()
    {
        return transform.childCount;
    }
    // Update is called once per frame
    void Update()
    {
        toMove = Equals(currentShip);
        if (!toMove) return;

        var mousePos = Input.mousePosition;
        var canvasRect = canvas.transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mousePos, Camera.main, out Vector2 result);
        result = canvas.transform.TransformPoint(result);
        transform.position = result; // Привязка корабля к мышке.

        GameField.CheckShipLocation(this, result);
        SwitchErrorAnimation();

        if (IsWithInCell) // Привязка корабля к клетке
        {
            transform.position = CellPosition;
        }

        if (Input.GetKeyDown(KeyCode.Escape)) // Передумали ставить
        {
            currentShip = null;
            if (wasAllocatedOnce)
            {
                transform.position = LastPosition;
                if (lastOrientation != orientation) Rotate();
                IsPositionCorrect = IsWithInCell = true;
                SwitchErrorAnimation();
                GameField.RegisterShip(this);
            }
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            currentShip = null;
            Destroy(gameObject);
        }

        else if (Input.GetKeyDown(KeyCode.Space)) // Поворачиваем корабль
        {
            Rotate();
        }
    }

    void OnFloorClick()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (toMove && IsPositionCorrect)
            {
                wasAllocatedOnce = true; // Сообщаем кораблю что он расставлен
                LastPosition = transform.position; // Запоминаем куда сбросить
                GameField.RegisterShip(this);
            }
            else if (wasAllocatedOnce) GameField.TakeShipOff(this);
            OnShipClick();
            if (IsPositionCorrect) wasAllocatedOnce = true;

        }
    }
    void SwitchErrorAnimation() // Анимация смены цвета, когда корабль вне игрового поля
    {
        foreach (var animator in animators)
        {
            animator.SetBool("IsMissPlaced", !IsPositionCorrect);
        }
        
    }

    void Rotate() // Поворот
    {
        if (orientation == Orientation.Horizontal)
        {
            orientation = Orientation.Vertical;
            transform.Rotate(new Vector3(0, 0, angle), Space.Self);
        }
        else if(orientation == Orientation.Vertical)
        {
            orientation = Orientation.Horizontal;
            transform.Rotate(new Vector3(0, 0, -angle), Space.Self);
        }
        if(IsPositionCorrect) lastOrientation = orientation;

    }

    public bool WasAllocatedOnce() // Поставлен ли корабль
    {
        return wasAllocatedOnce;
    }

}

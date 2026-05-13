using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    public enum ScreenShotSize
    {
        HD,
        FHD,
        UHD
    }

    public PlayerObj _prefabObj;
    public List<SPUM_Prefabs> _savedUnitList = new();
    public Vector2 _startPos;
    public Vector2 _addPos;
    public int _columnNum;
    public int UnitMaxCount = 20;
    public Transform _playerPool;
    public List<PlayerObj> _playerList = new();
    public PlayerObj _nowObj;
    public Transform _playerObjCircle;
    public Transform _goalObjCircle;
    public Camera _camera;
    public GameObject _bg;
    public RectTransform CommandPanel;
    public Button AnimationButton;
    public Transform AnimationPanelParent;
    public GameObject AnimationPanel;
    public ScreenShotSize _screenShotSize = ScreenShotSize.HD;
    private Texture2D imageSave;

    private void Start()
    {
        if (_savedUnitList.Count.Equals(0) || _playerList.Count.Equals(0))
            GetPlayerList();
    }

    // Update is called once per frame
    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                var isHitPlayer = hit.collider.CompareTag("Player");
                CommandPanel.gameObject.SetActive(isHitPlayer);

                if (isHitPlayer)
                {
                    _nowObj = hit.collider.GetComponent<PlayerObj>();
                    CreateAnimationPanel(_nowObj);
                }
                else
                {
                    //Set move Player object to this point
                    if (_nowObj != null)
                    {
                        var goalPos = hit.point;
                        _goalObjCircle.transform.position = hit.point;
                        _nowObj.SetMovePos(goalPos);
                    }
                }
            }
        }

        if (_nowObj != null) _playerObjCircle.transform.position = _nowObj.transform.position;
    }

    public void CreateAnimationPanel(PlayerObj Unit)
    {
        foreach (Transform item in AnimationPanelParent.transform) Destroy(item.gameObject);
        var Info = Unit._prefabs.StateAnimationPairs;
        foreach (var StateName in Info.Keys)
        {
            var Panel = Instantiate(AnimationPanel, AnimationPanelParent);
            var StateNameformat = $"{StateName} State";
            Panel.GetComponentInChildren<Text>().text = StateNameformat;
            var ParentTranform = Panel.GetComponentInChildren<ContentSizeFitter>().transform;
            foreach (var clip in Info[StateName])
            {
                var Button = Instantiate(AnimationButton, ParentTranform);
                Button.GetComponentInChildren<Text>().text = clip.name;
                Button.onClick.AddListener(() =>
                {
                    if (Enum.TryParse(StateName, true, out PlayerState State))
                    {
                        Unit.isAction = true;
                        var index = Info[StateName].FindIndex(x => x == clip);
                        Debug.Log(State + ":" + index);
                        Unit._prefabs._anim.Rebind();
                        Unit.SetStateAnimationIndex(State, index);
                        Unit.PlayStateAnimation(State);
                    }
                });
            }
        }
    }

    public void ClearPlayerList()
    {
        var tList = new List<GameObject>();
        for (var i = 0; i < _playerPool.transform.childCount; i++)
        {
            var tOBjj = _playerPool.transform.GetChild(i).gameObject;
            tList.Add(tOBjj);
        }

        foreach (var obj in tList) DestroyImmediate(obj);

        //net Edited. 2022.01.18
        _savedUnitList.Clear();
        _playerList.Clear();
    }

    public void GetPlayerList()
    {
        ClearPlayerList();

        var saveArray = Resources.LoadAll<SPUM_Prefabs>("");
        foreach (var unit in saveArray)
            if (unit.ImageElement.Count > 0)
                _savedUnitList.Add(unit);


        var numXStart = _startPos.x;
        var numYStart = _startPos.y;

        var numX = _addPos.x;
        var numY = _addPos.y;
        float ttV = 0;

        var sColumnNum = _columnNum;

        for (var i = 0; i < UnitMaxCount; i++)
        {
            if (i > _savedUnitList.Count - 1) continue;
            if (i > sColumnNum - 1)
            {
                numYStart -= 1f;
                numXStart -= numX * _columnNum;
                sColumnNum += _columnNum;
                ttV += numY;
            }

            var ttObj = Instantiate(_prefabObj.gameObject);
            ttObj.transform.SetParent(_playerPool);
            ttObj.transform.localScale = new Vector3(1, 1, 1);


            var tObj = Instantiate(_savedUnitList[i]);
            tObj.transform.SetParent(ttObj.transform);
            tObj.transform.localScale = new Vector3(1, 1, 1);
            tObj.transform.localPosition = Vector3.zero;

            ttObj.name = _savedUnitList[i].name;

            var tObjST = ttObj.GetComponent<PlayerObj>();

            tObjST._prefabs = tObj;

            ttObj.transform.localPosition = new Vector3(numXStart + numX * i, numYStart + ttV, 0);
            _playerList.Add(tObjST);
        }
    }

    public void SetAlignUnits()
    {
        var numXStart = _startPos.x;
        var numYStart = _startPos.y;

        var numX = _addPos.x;
        var numY = _addPos.y;
        float ttV = 0;

        var sColumnNum = _columnNum;
        _playerList = _playerList.Where(s => s != null).ToList();
        for (var i = 0; i < _playerList.Count - 1; i++)
        {
            if (i > sColumnNum - 1)
            {
                numYStart -= 1f;
                numXStart -= numX * _columnNum;
                sColumnNum += _columnNum;
                ttV += numY;
            }

            var ttObj = _playerList[i].gameObject;

            ttObj.transform.localPosition = new Vector3(numXStart + numX * i, numYStart + ttV, 0);
        }
    }

    //스크린샷 찍기
    public void SetScreenShot()
    {
        _bg.SetActive(false);
        var _nowSize = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
        switch (_screenShotSize)
        {
            case ScreenShotSize.HD:
                Screen.SetResolution(1280, 720, false);
                break;

            case ScreenShotSize.FHD:
                Screen.SetResolution(1920, 1080, false);
                break;

            case ScreenShotSize.UHD:
                Screen.SetResolution(3840, 2160, false);
                break;
        }

        var tX = _camera.scaledPixelWidth;
        var tY = _camera.scaledPixelHeight;

        var tempRT = new RenderTexture(tX, tY, 24, RenderTextureFormat.ARGB32)
        {
            antiAliasing = 4
        };

        _camera.targetTexture = tempRT;
        RenderTexture.active = tempRT;
        _camera.Render();

        imageSave = new Texture2D(tX, tY, TextureFormat.ARGB32, false, true);

        var tXPos = tX * 0.5f - imageSave.width * 0.5f;
        var tYPos = tY * 0.5f - imageSave.height * 0.5f;

        imageSave.ReadPixels(new Rect(tXPos, tYPos, imageSave.width, imageSave.height), 0, 0);
        imageSave.Apply();

        var bytes = imageSave.EncodeToPNG();
        var tName = string.Format("{0:yyyy-MM-dd_HH-mm-ss-fff}", DateTime.Now);
        var filename = string.Format("{0}/SPUM/ScreenShots/{1}.png", Application.dataPath, tName);
        File.WriteAllBytes(filename, bytes);

        RenderTexture.active = null;
        _camera.targetTexture = null;

        DestroyImmediate(tempRT);
        DestroyImmediate(imageSave);

        Screen.SetResolution((int)_nowSize.x, (int)_nowSize.y, false);
        Debug.Log("Screenshot Saved : " + filename);
        _bg.SetActive(true);
    }
}
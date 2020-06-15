#if  UNITY_EDITOR
using Luna;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public class TerrainCreateCon : MonoBehaviour
{
    public Transform _terrain;
    public GameObject _goGroundTile;
    public int X;
    public int Y;
    public float Height;

    private static TerrainCreateCon _instace;
    public static TerrainCreateCon Insatnce
    {
        get
        {
            if (_instace != null) return _instace;
            _instace = GameObject.Find("TCon").GetComponent<TerrainCreateCon>();
            _instace.Init();
            return _instace;
        }
    }

    public void LoadTerrain()
    {
        //Init();
        _terrain = GameObject.Find("RootTerrain").transform;
        _goGroundTile = _terrain.Find("GroundTile").gameObject;

        for (var x = 0; x < 36; x++)
        {
            for (var y = 0; y < 36; y++)
            {
                var go = Instantiate(_goGroundTile);
                go.transform.SetParent(_terrain);

                var gt = go.GetComponent<GroundTile>();
                gt.X = x;
                gt.Z = y;
                gt.Y = JsonDataMgr.Instance.TerrainMap[y + 1].XList[x];
                gt.Init();

                go.transform.localScale = Vector3.one;
                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = new Vector3(0.5f + 0.60f * x + 0.595f * y,
                    -0.38f - 0.30f * x + 0.306f * y + gt.Y * 0.167f,
                - 0.01f * x + 0.01f * y);
                //0);

                var tile = JsonDataMgr.Instance.TerrainInfoMap[gt.Z+1].XList[gt.X];
                var tileInfo = JsonDataMgr.Instance.TerrainTileInfoMap[tile];
                gt.UseAble = tileInfo.DropAble == 1;
                gt.MoveAble = tileInfo.MoveAble == 1;
                if (tileInfo.ID == 132 || tileInfo.ID == 133)
                {
                    gt.Height = 1.1f;
                }

                _ = SetIcon(go.GetComponent<SpriteRenderer>(), AssetMgr.TerrainImage_PATH, tileInfo.Icon);
            }
        }

        Destroy(_goGroundTile);
    }

    private async void Init()
    {
        await JsonDataMgr.Instance.LoadTerrin();
    }

    public async UniTask SetIcon(SpriteRenderer img,string pass, string pname)
    {
        img.sprite = await AssetMgr.Instance.LoadSprite(pass, pname);
    }
}

#endif
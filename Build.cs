using Google.Protobuf;
using JsonData;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace Luna
{
    public class Build
    {
        public bool[][] Area;
        public List<GroundTile> myGroundTiles;
        private List<GroundTile> _testGroundTiles;
        public GameObject Obj;
        public Transform Tr;
        public JButton JB;
        public SpriteRenderer SpIcon;
        public SpriteRenderer SpR;
        public BoxCollider Collider;

        public JButton SkinBtn; 
        public JButton FlipBtn; 
        public JButton RecBtn;
        public JButton CancelBtn;
        public JButton ConfirmBtn;

        public PLandBuild BuildData;
        public BuildJd BuildJd;
        public BuildingSkinJd BSJd;

        public Vector3 OffectVector3;

        public bool IsNull;
        public bool IsFilp;

        protected int _needGtConut;

        private Transform _tr_putCon;
        private Vector3 _exPos;
        private BuildLvJd _buildLvJd;
        public virtual Build New(GameObject _Obj)
        {
            Obj = _Obj;
            Tr = _Obj.transform;
            SpIcon = Tr.Find("img_icon").GetComponent<SpriteRenderer>();
            JB = SpIcon.GetComponent<JButton>();
            SpR = _Obj.transform.GetChild(1).GetComponent<SpriteRenderer>();
            Collider = SpIcon.GetComponent<BoxCollider>();

            myGroundTiles = new List<GroundTile>();
            _testGroundTiles = new List<GroundTile>();

            SkinBtn = Tr.Find("img_skin").GetComponent<JButton>();
            SkinBtn.gameObject.SetActive(false);

            _tr_putCon = Tr.Find("img_putCon");
            _tr_putCon.gameObject.SetActive(false);

            FlipBtn = _tr_putCon.Find("img_flip").GetComponent<JButton>();
            RecBtn = _tr_putCon.Find("img_recover").GetComponent<JButton>();
            CancelBtn = _tr_putCon.Find("img_cancel").GetComponent<JButton>();
            ConfirmBtn = _tr_putCon.Find("img_confirm").GetComponent<JButton>();

            Area = new bool[5][];
            for (int i = 0; i < 5; i++)
            {
                Area[i] = new bool[5];
                for (int j = 0; j < 5; j++)
                {
                    Area[i][j] = false;
                }
            }

            BuildJd = null;
            BSJd = null;

            return this;
        }

        public Vector3 GetVector3(GroundTile gt)
        {
            var pos = gt.transform.position;
            if (BuildJd.Type == BuildType.Base)
            {
                return new Vector3(2.465f + pos.x, 0.84f + pos.y, -15 - gt.X + 0.1f * gt.Z);
            }
            else if (BuildJd.Type == BuildType.Decorate)
            {
                return new Vector3(pos.x, 0.555f + pos.y, -15 - gt.X + 0.1f * gt.Z);
            }
            else
            {
                return new Vector3(0.6f + pos.x, 0.8f + pos.y, -15 - gt.X + 0.1f * gt.Z);
            }
        }

        public virtual void TurnBuild()
        {
            this.SpIcon.flipX = !SpIcon.flipX;
            IsFilp = SpIcon.flipX;
        }

        public void SetBuild(PLandBuild pLandBuild)
        {
            BuildData = pLandBuild;
            SpIcon.flipX = pLandBuild.Turn;
            IsFilp = pLandBuild.Turn;
        }

        public virtual void CreateBuid(int DicID)
        {
            Tr.localScale = Vector3.one;

            BuildJd = JsonDataMgr.Instance.BuildMap[DicID];
            _buildLvJd = JsonDataMgr.Instance.BuildLvMap[BuildJd.ID][1];

            if (BuildJd.Type == BuildType.Base)
            {
                Collider.size = new Vector3(3,2,0);
                _exPos = new Vector3(0.2f, 0.5f, 0);
            }
            else if (BuildJd.Type == BuildType.Decorate)
            {
                Collider.center = Vector3.zero;
                Collider.size = new Vector3(0.8f, 1.2f, 0);
                _exPos = Vector3.zero;
            }
            else
            {
                Collider.size = new Vector3(1.4f, 1.3f, 0);
                _exPos = new Vector3(0.45f, 0.3f, 0);
            }
            GetBuild();
        }

        public async UniTask SetIcon()
        {
            var icon = await AssetMgr.Instance.LoadSprite(AssetMgr.Build_PATH, _buildLvJd.Icon);
            if (icon != null)
            {
                SpIcon.sprite = icon;
            }
        }

        public void RefeshBuid()
        {
            KingdomDataMgr.Instance.BuildDataMap.TryGetValue(BuildData.InsID, out BuildData);
        }

        public bool CheckIsSetBuid()
        {
            return _tr_putCon.gameObject.activeSelf;
        }

        public void ShowBtns(bool isShow)
        {
            SetAlpha(isShow);

            ShowPutBuildBtns(isShow, BuildJd.Type == BuildType.Decorate ? 4 : 2);
        }

        public void GetBuild()
        {
            SetAlpha(true);
            if (myGroundTiles.Count != 0)
            {
                foreach (var GT in myGroundTiles)
                {
                    GT.RemoveBuild(this);
                }
                foreach (var GT in myGroundTiles)
                {
                    GT.CheckError();
                }
                myGroundTiles.Clear();
            }
        }

        public void RecoveryBuild()
        {
            foreach (var GT in myGroundTiles)
            {
                GT.RemoveBuild(this);
            }
            foreach (var GT in myGroundTiles)
            {
                GT.CheckError();
            }
            myGroundTiles.Clear();
        }

        public void PutBuild(GroundTile gt,List<GroundTile> GTs)
        {
            SetAlpha(false);
            foreach (var GroundTile in myGroundTiles)
            {
                GroundTile.RemoveBuild(this);
            }
            foreach (var GT in myGroundTiles)
            {
                GT.CheckError();
            }

            myGroundTiles.Clear();
            myGroundTiles.AddRange(GTs);
            foreach (var GT in GTs)
            {
                GT.AddBuild(this);
            }
            foreach (var GT in myGroundTiles)
            {
                GT.CheckError();
            }

            Tr.position = GetVector3(gt);

            if (BuildData != null)
            {
                BuildData.PosX = gt.X;
                BuildData.PosY = gt.Z;
            }
            
            //Tr.localPosition =new Vector3(0, 0.00437f * (300 - SpIcon.sprite.rect.height),0) + GetVector3(gt.transform.localPosition);
        }

        public bool CheckCanPut(GroundTile gt, List<GroundTile> GTs)
        {
            _testGroundTiles.Clear();
            _testGroundTiles.AddRange(GTs);
            foreach (var GT in GTs)
            {
                GT.SetfalseGT(this);
            }
            foreach (var GT in _testGroundTiles)
            {
                GT.CheckCanPut();
            }

            if (_testGroundTiles.Count != 0)
            {
                var tileHight = _testGroundTiles[0].Y;
                foreach (var GT in _testGroundTiles)
                {
                    if (_testGroundTiles.Count < _needGtConut)
                    {
                        return false;
                    }

                    if (!GT.Unlock)
                    {
                        return false;
                    }

                    if (!GT.UseAble)
                    {
                        return false;
                    }

                    if (tileHight != GT.Y)
                    {
                        return false;
                    }

                    if (GT.CheckBuildList.Count > 1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public bool CheckErrorBuild()
        {
            if (myGroundTiles.Count != 0)
            {
                var tileHight = myGroundTiles[0].Y;
                foreach (var GT in myGroundTiles)
                {
                    if (myGroundTiles.Count < _needGtConut)
                    {
                        return true;
                    }

                    if (!GT.Unlock)
                    {
                        return true;
                    }

                    if (!GT.UseAble)
                    {
                        return true;
                    }

                    if (tileHight != GT.Y)
                    {
                        return true;
                    }

                    if (GT.UsedBuildList.Count > 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void SetAlpha(bool _bool)
        {
            if (_bool)
            {
                SpIcon.color = new Color32(255, 255, 255, 125);
                SpR.color = new Color32(255, 255, 255, 125);
                //Tr.localPosition = new Vector3(Tr.localPosition.x, Tr.localPosition.y, -0.1f);
            }
            else
            {
                SpIcon.color = new Color32(255, 255, 255, 255);
                SpR.color = new Color32(255, 255, 255, 255);
                //Tr.localPosition = new Vector3(Tr.localPosition.x, Tr.localPosition.y, -0.7f);
            }
        }
        public void SetNull()
        {
            IsNull = true;
            Object.Destroy(Obj);
            RecoveryBuild();
        }

        /// <summary>
        /// 1 = 普通建筑建造
        /// 2 = 普通建筑控制
        /// 3 = 装饰建筑建造
        /// 4 = 装饰建筑控制
        /// </summary>
        /// <param name="showtypeindex"></param>
        public void ShowPutBuildBtns(bool isshow,int showtypeindex = 5)
        {
            _tr_putCon.gameObject.SetActive(isshow);
            CancelBtn.gameObject.SetActive(false);
            switch (showtypeindex)
            {
                case 1:
                    RecBtn.gameObject.SetActive(false);
                    CancelBtn.gameObject.SetActive(true);
                    ConfirmBtn.gameObject.SetActive(true);
                    break;
                case 2:
                    RecBtn.gameObject.SetActive(false);
                    ConfirmBtn.gameObject.SetActive(true);
                    break;
                case 3:
                    RecBtn.gameObject.SetActive(false);
                    CancelBtn.gameObject.SetActive(true);
                    ConfirmBtn.gameObject.SetActive(true);
                    break;
                case 4:
                    RecBtn.gameObject.SetActive(true);
                    ConfirmBtn.gameObject.SetActive(true);
                    break;
            }
        }

        public void SetDragEndPos(GroundTile gt)
        {
            Collider.enabled = true;
            Tr.position = GetVector3(gt);
            ShowBtns(true);
            ShowPutBuildBtns(true, BuildJd.Type == BuildType.Decorate ? 4 : 2);
        }

        public void SetCurPos(Vector3 pos)
        {
            Tr.position = pos + _exPos;
        }
    }

    public class DecBuild : Build
    {
        public override void CreateBuid(int DicID)
        {
            base.CreateBuid(DicID);
            Area[0][0] = true;
            _needGtConut = 1;
        }
    }

    public class FuncBuild : Build
    {
        public override void CreateBuid(int DicID)
        {
            base.CreateBuid(DicID);

            _needGtConut = 0;
            for (int i = 0; i <= BuildJd.Area[0].X - 1; i++)
            {
                for (int j = 0; j <= BuildJd.Area[0].Y - 1; j++)
                {
                    Area[i][j] = true;
                    _needGtConut++;
                }
            }
        }
    }

}

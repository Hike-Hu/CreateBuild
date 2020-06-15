using DG.Tweening;
using System;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Luna
{
    public partial class BuildView
    {
        private bool _isMoveBuild;
        private bool _isBuildConIn;
        private bool _canDrag;
        private bool _canAdd;
        private bool _isCreateBuild; 
        private bool _isMoveCamera; 
        private Build _nowCreateBuild;

        public BuildView()
        {
            _canDrag = true;
        }

        #region 拖动创建建筑按钮

        private Vector3 oldpos;
        private void GetBtnDragBegin(BaseEventData ed)
        {
            if (_isBuildConIn) return;
            if (!m_IsSingleFinger) return;
            _isMoveBuild = true;
            var ped = (PointerEventData)ed;
            if (ped.delta.y < 4)
            {
                _canAdd = false;
                return;
            }
            _canAdd = true;
            _nowBuild?.ShowBtns(false);

            var buildid = 0;
            _nowGetBuildItem = NowTagIndex == 0
                ? _items[int.Parse(ed.selectedObject.transform.parent.name)]
                : _itemsDec[int.Parse(ed.selectedObject.transform.parent.name)];

            buildid = _nowGetBuildItem.BuildJd.ID;
            if (!_nowGetBuildItem.Canadd())
            {
                PopInfoCtrl.Insatnce.Show(true, "建造上限");
                _canAdd = false;
                return;
            }

            oldpos = _nowGetBuildItem.JB.ScrollRect.content.position;
            CreateBuild(buildid);
            _nowBuild.SetIcon();

            _isCreateBuild = true;
            CloseGetBuildView(null);
        }

        private void GetBtnDraging(BaseEventData arg0)
        {
            if (_isBuildConIn) return;
            if (!m_IsSingleFinger) return;
            if (!_canAdd) return;

            _nowGetBuildItem.JB.ScrollRect.content.position = oldpos;
            BuildDrag(arg0);
        }

        private void GetBtnDragEnd(BaseEventData arg0)
        {
            if (_isBuildConIn) return;
            if (!m_IsSingleFinger) return;
            if (!_canAdd) return;

            _nowBuild?.SetDragEndPos(_nowGt);
            _nowBuild.ShowPutBuildBtns(true, _nowBuild.BuildJd.Type == BuildType.Decorate ? 3 : 1);
            CloseGetBuildView(null);
            _isMoveBuild = false;
        }

        #endregion

        #region 建筑操作
        private void BuildClick(BaseEventData arg0)
        {
            if (_isMoveBuild) return;
            if (!m_IsSingleFinger) return;
            if (_isMoveCamera) return;
            if (_nowCreateBuild != null) return;

            switch (_nowMode)
            {
                case ModeType.Normal:
                {
                    _nowBuild = _builds[int.Parse(arg0.selectedObject.transform.parent.name)];
                    if (_nowBuild.BuildJd.Type != BuildType.Decorate)
                    {
                        ShowBuildMoreView(_nowBuild);
                    }
                    MoveTo(_nowBuild, () =>
                    {
                        //Camera.main.DOOrthoSize(4, 0.7f).OnComplete(() =>
                        //{
                        //    ShowBuildMoreView(_nowBuild);
                        //    Camera.main.orthographicSize = 8;
                        //});
                    });
                    break;
                }
                case ModeType.Build:
                    if (_nowBuild == _builds[int.Parse(arg0.selectedObject.transform.parent.name)])
                    {
                        return;
                    }
                    if (_nowBuild != null)
                    {
                        PutBuild();
                    }
                    _nowBuild = _builds[int.Parse(arg0.selectedObject.transform.parent.name)];
                    MoveTo(_nowBuild);
                    _nowBuild?.ShowBtns(true);
                    _nowGt = _nowBuild.myGroundTiles[0];
                    _nowBuild.GetBuild();
                    SetGroundTitleColor(_nowBuild);
                    break;
                default:
                    return;
            }
        }
        private void BuildDragBegin(BaseEventData arg0)
        {
            if (!m_IsSingleFinger) return;

            _isBuildConIn = true;
            _isMoveBuild = true;
            if (_nowCreateBuild != null)
            {
                _nowBuild = _nowCreateBuild;
                _nowBuild?.ShowBtns(false);
                _nowBuild.GetBuild();
                SetBuildCollider(false);
                return;
            }

            if (_nowMode != ModeType.Build) return;
            _nowBuild?.ShowBtns(false);
            _nowBuild = _builds[int.Parse(arg0.selectedObject.transform.parent.name)];
            _nowBuild.GetBuild();

            SetBuildCollider(false);
        }
        private void BuildDraging(BaseEventData arg0)
        {
            if (!m_IsSingleFinger) return;

            if (_nowCreateBuild != null)
            {
                BuildDrag(arg0);
                return;
            }

            if (_nowMode == ModeType.Build)
            {
                BuildDrag(arg0);
            }
            else
            {
                CameraGO(arg0);
            }
            
        }
        private void BuildDragEnd(BaseEventData arg0)
        {
            if (!m_IsSingleFinger) return;

            _isBuildConIn = false;
            _isMoveBuild = false;
            if (_nowCreateBuild != null)
            {
                _nowBuild?.SetDragEndPos(_nowGt);
                _nowBuild.ShowPutBuildBtns(true, _nowBuild.BuildJd.Type == BuildType.Decorate ? 3 : 1);
                CloseGetBuildView(null);
                return;
            }

            if (_nowMode == ModeType.Build)
            {
                _nowBuild?.SetDragEndPos(_nowGt);
            }
        }
        #endregion

        #region 地板操作
        private void GroundClick(BaseEventData arg0)
        {
            if (_isCreateBuild) return;
            if (_nowBuild != null) return;
            ReSetPutInfo();
        }
        private void GroundDrag(BaseEventData arg0)
        {
            if (!m_IsSingleFinger) return;
            _isMoveCamera = true;
            CameraGO(arg0);
        }

        private void GroundEndGrag(BaseEventData arg0)
        {
            _isMoveCamera = false;
        }
        #endregion

        #region 单个建筑详细操作
        private void CreateBuild(int DicID)
        {
            var go = Instantiate(_goBuild);
            go.SetActive(true);
            go.transform.SetParent(_trBuild);
            go.transform.position = new Vector3(0, 0, -10);
            go.transform.localScale = new Vector3(0.2f, 0.2f, 0);
            go.transform.localRotation = Quaternion.identity;
            go.name = _buildIndex.ToString();

            Build build;
            if (NowTagIndex == 0)
            {
                build = new FuncBuild();
                _builds.Add(_buildIndex,build.New(go));
            }
            else
            {
                build = new DecBuild();
                _builds.Add(_buildIndex,build.New(go));
            }

            build.JB.AddEvent(EventTriggerType.PointerClick, BuildClick);
            build.JB.AddEvent(EventTriggerType.BeginDrag, BuildDragBegin);
            build.JB.AddEvent(EventTriggerType.Drag, BuildDraging);
            build.JB.AddEvent(EventTriggerType.EndDrag, BuildDragEnd);

            build.RecBtn.AddEvent(EventTriggerType.PointerClick, RecoverBuild);
            build.SkinBtn.AddEvent(EventTriggerType.PointerClick, SkinBuild);
            build.ConfirmBtn.AddEvent(EventTriggerType.PointerClick, ConfirmPutBuild);
            build.CancelBtn.AddEvent(EventTriggerType.PointerClick, CancelPutBuild); 
            build.FlipBtn.AddEvent(EventTriggerType.PointerClick, RoteBuild); 

            _buildIndex++;

            _nowBuild = build;
            _nowBuild.CreateBuid(DicID);

            SetBuildCollider(false);
            _nowCreateBuild = _nowBuild;
        }

        private void ConSetBuild()
        {
            _nowGt = _tiles[_nowBuild.BuildData.PosX][_nowBuild.BuildData.PosY];
            _lightGroundTiles.Clear();
            for (int i = 0; i < _nowBuild.Area.Length; i++)
            {
                for (int j = 0; j < _nowBuild.Area[i].Length; j++)
                {
                    if (_nowGt.X + i <= _tiles.Length - 1)
                    {
                        if (_nowGt.Z + j <= _tiles[_nowGt.X + i].Length - 1)
                        {
                            if (_nowBuild.Area[i][j])
                            {
                                _lightGroundTiles.Add(_tiles[_nowGt.X + i][_nowGt.Z + j]);
                            }
                        }
                        else
                        {
                            _dotEnough = true;
                        }
                    }
                    else
                    {
                        _dotEnough = true;
                    }
                }
            }

            _nowBuild.PutBuild(_nowGt, _lightGroundTiles);
        }
        private void SkinBuild(BaseEventData arg0)
        {
            Debug.LogError($"更换皮肤{arg0.selectedObject.name}");
        }
        private void BuildDrag(BaseEventData arg0)
        {
            var worldPos = _tdCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            var curPos = new Vector3(worldPos.x, worldPos.y, _nowBuild.Tr.position.z);
            _nowBuild.SetCurPos(curPos);

            var ray = _tdCamera.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit)) return;
            if (!hit.collider.gameObject.CompareTag("Plane")) return;
            var arr = hit.collider.name.Split('_');

            var gt = GetTile(int.Parse(arr[0]), int.Parse(arr[1]));
            if (gt == null) return;

            _nowGt = gt;

            SetGroundTitleColor(_builds[int.Parse(_nowBuild.Obj.name)]);
        }

        public GroundTile GetTile(int x,int y)
        {
            if (_nowBuild.BuildJd.Type == BuildType.Base) { 
                if (x - 2 < 0 || y - 2 < 0) return null;
                return _tiles[x - 2][y - 2] == null ? null :
                    _tiles[x - 2][y - 2] == _nowGt ? null : _tiles[x - 2][y - 2];
            }
            return _tiles[x][y] == null ? null :
                _tiles[x][y] == _nowGt ? null : _tiles[x][y];
        }

        /// <summary>
        /// 放置建筑
        /// </summary>
        private void PutBuild()
        {
            if (!_nowBuild.CheckCanPut(_nowGt, _lightGroundTiles))
            {
                MoveTo(_nowBuild);
                PopInfoCtrl.Insatnce.Show(true, "当前建筑摆放在错误位置");
                return;
            }

            _nowBuild.PutBuild(_nowGt, _lightGroundTiles);

            ReSetPutInfo();

            CheckErrorBuild();
        }

        private void ConfirmPutBuild(BaseEventData bed)
        {
            if (!_isCreateBuild)
            {
                PutBuild();
                return;
            }

            if (!_nowBuild.CheckCanPut(_nowGt, _lightGroundTiles))
            {
                MoveTo(_nowBuild);
                PopInfoCtrl.Insatnce.Show(true, "当前建筑摆放在错误位置");
                return;
            }

            _ = KingdomDataMgr.Instance.GetBuildReq(_nowGetBuildItem.BuildJd.ID, _nowBuild.IsFilp, _nowGt, (pLandBuild) =>
            {
                _nowBuild.SetBuild(pLandBuild);
                _nowBuild.ShowPutBuildBtns(false);

                RefeshBuidData();
                RefeshTitle();
                if (NowTagIndex == 0)
                {
                    ContinueRefeshGet();
                }
                else
                {
                    ContinueRefeshGetDec();
                }

                PutBuild();

                ShowGetBuildView(null);
            });

            _isCreateBuild = false;
        }

        private void CancelPutBuild(BaseEventData bed)
        {
            _nowBuild.GetBuild();
            _builds.Remove(int.Parse(_nowBuild.Obj.name));
            _nowBuild.SetNull();
            _nowBuild = null;
            _nowGt = null;
            ShowGetBuildView(null);
            ShowTitle(false);
            _isCreateBuild = false;
        }

        private void RecoverBuild(BaseEventData arg0)
        {
            KingdomDataMgr.Instance.RecoverBuildReq(_nowBuild.BuildData.InsID, () =>
            {
                RefeshBuidData();
                if (NowTagIndex == 0)
                {
                    ContinueRefeshGet();
                }
                else
                {
                    ContinueRefeshGetDec();
                }
            });
            _builds[int.Parse(_nowBuild.Obj.name)].SetNull();
            _builds.Remove(int.Parse(_nowBuild.Obj.name));
            ReSetPutInfo();
            CheckErrorBuild();
        }

        /// <summary>
        /// 检查建筑是否摆放正确
        /// </summary>
        private void CheckErrorBuild()
        {
            _canSaveBuild = false;
            TitleCtrl.Insatnce.LockBack = false;
            foreach (var groundTile in _tiles)
            {
                foreach (var tile in groundTile)
                {
                    if (tile.IsErrorTile)
                    {
                        _canSaveBuild = true;
                        TitleCtrl.Insatnce.LockBack = true;
                    }
                }
            }
        }

        /// <summary>
        /// 建筑旋转
        /// </summary>
        private void RoteBuild(BaseEventData arg0)
        {
            _nowBuild.TurnBuild();
        }
        #endregion

        #region 移动相机
        private void MoveTo(Build ChoseBuild, Action MoveAction = null)
        {
            _canDrag = false;
            var pos = new Vector3(-ChoseBuild.Tr.localPosition.x, -ChoseBuild.Tr.localPosition.y, 0);
            _trGround.DOMove(pos, 0.3f).OnComplete(() => { _canDrag = true; });
            
            MoveAction?.Invoke();
        }

        private void CameraGO(BaseEventData ed)
        {
            if (!_canDrag) return;
            var ped = (PointerEventData) ed;

            var curpos = _trGround.position + new Vector3(ped.delta.x * 0.015f * (_tdCamera.orthographicSize / _maxDistance),
                ped.delta.y * 0.015f * (_tdCamera.orthographicSize / _maxDistance), _trGround.position.z);

            _trGround.position = new Vector3(Mathf.Clamp(curpos.x, xMin, xMax), Mathf.Clamp(curpos.y, yMin, yMax),
                curpos.z);
        }                                                           
        #endregion

        /// <summary>
        /// 关闭格子等
        /// </summary>
        private void ReSetPutInfo()
        {
            ShowTitle(false);
            SetBuildCollider(true);

            _nowBuild?.ShowBtns(false);
            if (!(_nowGt == null))
            {
                _nowGt = null;
            }

            if (_lightGroundTiles.Count != 0)
            {
                _lightGroundTiles.Clear();
            }

            _nowBuild = null;
            _nowCreateBuild = null;
        }

        /// <summary>
        /// 设置当前格子颜色
        /// </summary>
        private void SetGroundTitleColor(Build build)
        {
            if (_lightGroundTiles.Count != 0)
            {
                foreach (var GT in _lightGroundTiles)
                {
                    GT.CloseTile();
                }
                _lightGroundTiles.Clear();
            }

            _dotEnough = false;
            for (int i = 0; i < build.Area.Length; i++)
            {
                for (int j = 0; j < build.Area[i].Length; j++)
                {
                    if (build.Area[i][j])
                    {
                        if (_nowGt.X + i <= _tiles.Length - 1)
                        {
                            if (_nowGt.Z + j <= _tiles[_nowGt.X + i].Length - 1)
                            {
                                _lightGroundTiles.Add(_tiles[_nowGt.X + i][_nowGt.Z + j]);
                            }
                            else
                            {
                                _dotEnough = true;
                            }
                        }
                        else
                        {
                            _dotEnough = true;
                        }
                    }
                }
            }

            if (_dotEnough)
            {
                _canPut = false;
                foreach (var GT in _lightGroundTiles)
                {
                    GT.SetColorByUse(_canPut);
                }
                return;
            }

            if (_lightGroundTiles.Count == 0) return;
            {
                var tileHight = _lightGroundTiles[0].Y;
                foreach (var GT in _lightGroundTiles)
                {
                    if (!GT.UseAble)
                    {
                        _canPut = false;
                        GT.SetColorByUse(_canPut);
                        continue;
                    }

                    if (tileHight != GT.Y)
                    {
                        _canPut = false;
                        foreach (var GT2 in _lightGroundTiles)
                        {
                            GT2.SetColorByUse(_canPut);
                        }
                        return;
                    }

                    if (GT.UsedBuildList.Count >= 1)
                    {
                        _canPut = false;
                        GT.SetColorByUse(_canPut);
                        continue;
                    }

                    _canPut = true;
                    GT.SetColorByUse(_canPut);
                }
            }


        }

        /// <summary>
        /// 初始化格子
        /// </summary>
        private void ShowTitle(bool _bool)
        {
            foreach (var GTs in _tiles)
            {
                foreach (var GT in GTs)
                {
                    if (_bool)
                    {
                        GT.CheckCanUse();
                    }
                    else
                    {
                        GT.CloseTile();
                    }
                }
            }
        }

        /// <summary>
        /// 开关建筑碰撞
        /// </summary>
        private void SetBuildCollider(bool _bool)
        {
            if (_bool)
            {
                foreach (var build in _builds)
                {
                    if (!build.Value.IsNull)
                    {
                        build.Value.Collider.enabled = true;
                    }
                }
            }
            else
            {
                foreach (var build in _builds)
                {
                    if (!build.Value.IsNull)
                    {
                        build.Value.Collider.enabled = false;
                    }
                }
            }
        }
    }
}


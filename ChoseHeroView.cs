using System;
using System.Collections.Generic;
using Google.Protobuf;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utage;

namespace Luna
{
    public partial class BuildView
    {
        private Transform tr_choseHeroView;

        private JButton _confirmHeroBtn;
        private JButton _closeHeroViewBtn;
        private JButton _clearHeroViewBtn;

        private Image _heroimg;

        private Dictionary<int ,CharItem> _workHeroItems;

        private SuperScrollViewController _scView;

        private int _showHeroCount;
        private int _nowIndex;
        private List<CharItem> _selectHeros;

        private List<PAvatar> _heroDatas;

        private string _nowBuildInsID;

        private int _showindex;


        public void InitChoseHeroView()
        {
            tr_choseHeroView = RootCanvasTransform.Find("ChoseHeroView");

            _heroimg = tr_choseHeroView.Find("Mask/img_Hero").GetComponent<Image>();

            _workHeroItems = new Dictionary<int, CharItem>();
            _scView = tr_choseHeroView.Find("Heros").GetComponent<SuperScrollViewController>();

            _selectHeros = new List<CharItem>();

            _confirmHeroBtn = tr_choseHeroView.Find("btn_Confirm").GetComponent<JButton>();
            _confirmHeroBtn.AddEvent(EventTriggerType.PointerClick, ConfirmBtn);
            _closeHeroViewBtn = tr_choseHeroView.Find("btn_Close").GetComponent<JButton>();
            _closeHeroViewBtn.AddEvent(EventTriggerType.PointerClick, CloseChangeHeroView);
            _clearHeroViewBtn = tr_choseHeroView.Find("btn_Clear").GetComponent<JButton>();
            _clearHeroViewBtn.AddEvent(EventTriggerType.PointerClick, ClearChoseHero);

            _heroDatas = new List<PAvatar>();
        }

        private void GetData()
        {
            _heroDatas.Clear();
            foreach (var characterData in CharDataMgr.Instance.CharMap)
            {
                var isin = false;
                foreach (var pLandBuildProduce in _BuildDatas)
                {
                    if (pLandBuildProduce.Value.InsID != _nowBuildInsID)
                    {
                        foreach (var i in pLandBuildProduce.Value.AvatarQ)
                        {
                            if (i == characterData.Value.Id)
                            {
                                isin = true;
                            }
                        }
                    }
                }

                if (!isin)
                {
                    _heroDatas.Add(characterData.Value);
                }
            }
        }

        public void ShowChoseHeroView(bool _bool,int showIndex = 0, int Index = 3)
        {
            tr_choseHeroView.gameObject.SetActive(_bool);
            if (_bool)
            {
                _showindex = showIndex;
                _nowIndex = 1;
                _showHeroCount = Index;
                if (_showindex == 0)
                {
                    _nowBuildInsID = _nowBuild.BuildData.InsID;
                }
                else
                {
                    _nowBuildInsID = _nowWorkLineItem.WorkBuild.InsID;
                }

                GetData();
                RefeshHeros();

                var nowBuildHeros = _BuildDatas[_nowBuildInsID].AvatarQ;
                for (int i = 0; i < nowBuildHeros.Count; i++)
                {
                    if (nowBuildHeros[i] != 0)
                    {
                        _workHeroItems[nowBuildHeros[i]].SetSelect(true);
                        _selectHeros.Add(_workHeroItems[nowBuildHeros[i]]);
                        _nowIndex++;
                    }
                }
            }
        }

        private void RefeshHeroInfo(CharItem ci)
        {
            LoadSpine(ci.CharId);
        }
        private async UniTask LoadSpine(int id)
        {
            var cdt = CharDataMgr.Instance.CharTalentMap[id];
            var pname = $"{ id }_{ cdt.JinjieIdx}_l";
            //var sp = await AssetMgr.Instance.LoadSprite(AssetMgr.LIHUI_PATH, pname);
            //_heroimg.sprite = sp;
            //_heroimg.SetNativeSize();
            //_heroimg.gameObject.SetActive(true);
        }
        private void ChoseHero(BaseEventData arg0)
        {
            var ci = _workHeroItems[int.Parse(arg0.selectedObject.name)];

            if (_showHeroCount == 1)
            {
                if (_selectHeros.Count == 0)
                {
                    ci.SetSelect(true);
                    _selectHeros.Add(ci);
                }
                else
                {
                    _selectHeros[0].SetSelect(false);
                    _selectHeros[0] = ci;
                    _selectHeros[0].SetSelect(true);
                }
            }
            else
            {
                if (_selectHeros.Contains(_workHeroItems[int.Parse(arg0.selectedObject.name)]))
                {
                    ci.SetSelect(false);
                    _selectHeros.Remove(ci);
                    _nowIndex--;
                }
                else if (_nowIndex <= _showHeroCount)
                { 
                    ci.SetSelect(true);
                    _selectHeros.Add(ci);
                    _nowIndex++;
                }
            }
            RefeshHeroInfo(ci);
        }

        /// <summary>
        /// 清空选择角色
        /// </summary>
        private void ClearChoseHero(BaseEventData arg0)
        {
            foreach (var SelectHero in _selectHeros)
            {
                SelectHero.SetSelect(false);
            }
            _selectHeros.Clear();
            _nowIndex = 1;
        }

        /// <summary>
        /// 确认更换驻扎角色
        /// </summary>
        private void ConfirmBtn(BaseEventData arg0)
        {
            KingdomDataMgr.Instance.SetWorkerReq(_nowBuildInsID, _selectHeros, () =>
            {
                RefeshBuidData();
                _selectHeros.Clear();
                ShowChoseHeroView(false);
                RefeshOtherViewHeros();
            });
        }

        private void RefeshOtherViewHeros()
        {
            if (_showindex == 0)
            {
                _nowBuild.RefeshBuid();
                RefeshBuildInInfo(_nowBuild);
            }
            else
            {
                _nowWorkLineItem.RefeshWorkLineData();
                RefeshWorkHero();

                _selectHeros.Clear();
            }
        }

        private void CloseChangeHeroView(BaseEventData arg0)
        {
            tr_choseHeroView.gameObject.SetActive(false);
        }

        private void RefeshHeros()
        {
            _workHeroItems.Clear();
            _scView.NewScroll(_heroDatas.Count, OnRefeshOne);
        }
        private void OnRefeshOne(Transform Tr, int Index)
        {
            CharItem whi = new CharItem();
            whi = whi.New(Tr);
            whi.SetItem(_heroDatas[Index]);
            whi.Obj.name = _heroDatas[Index].Id.ToString();
            whi.Transform.localRotation = Quaternion.identity;
            whi.JB.ClearEvent();
            whi.JB.AddEvent(EventTriggerType.PointerClick, ChoseHero);
            _workHeroItems.Add(_heroDatas[Index].Id, whi);
        }
    }

}
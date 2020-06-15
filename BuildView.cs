using Google.Protobuf;
using JsonData;
using Spine.Unity;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UtageExtensions;

namespace Luna
{
    public partial class BuildView : ViewCtrl
    {
        public const string PATH = "build";

        private bool showNow;

        private Transform _trGround;
        private Transform _trWorkHeroSps;

        private Camera _tdCamera;

        #region 按钮
        private enum ModeType
        {
            Normal,
            Build
        }
        private ModeType _nowMode = ModeType.Normal;

        private Transform _btns;
        private JButton _produceBtn;
        private JButton _getBuildBtn;
        private JButton _buildTypeBtn;
        private JButton _normalTypeBtn;

        private bool _isCloseUI;
        private JButton _closeUIBtn;
        private CanvasGroup _canvasGroup;
        private GameObject _imgCloseUI;

        #endregion

        #region 建筑
        private GameObject _goBuild;
        private Transform _trBuild;
        private int _buildIndex = 0;
        private Dictionary<int, Build> _builds;

        private Build _nowBuild;

        private bool _canPut = false;
        private bool _dotEnough = false;
        #endregion

        #region 顶部标题栏

        private Transform _trTitle;
        private Text _baseBuildname;
        private int _baseBuildLv;

        private Transform _trResView;
        private JButton _showResViewBtn;
        private JButton _closeResViewBtn;
        private ObjectMap _resGetMap;

        private float _baseExpWidth;
        private Image _baseExp;

        private ObjectMap _resMap;

        private JButton _backBtn;

        private Transform _trPiggyBank;
        private JButton _getMoneyBtn;
        private Image _moneyImage;
        private Image _moneyRedPoint;
        private Text _moneyproduce;

        private Text _texMonny;
        private Text _texDiamond;
        #endregion

        #region 地板
        private JButton _moveCon;
        private Transform _terrain;
        private GroundTile[][] _tiles;

        public GroundTile _nowGt;
        private List<GroundTile> _lightGroundTiles = new List<GroundTile>();
        #endregion

        #region 获取建筑页面
        private Transform _trGetBuildView;
        private CanvasGroup _GetBuildViewCVS;

        private JButton _closeGetBuildBtn;

        private PLandBuild _mainData;
        private Dictionary<string, PLandBuild> _BuildDatas;

        private SuperScrollViewController _scrollViewBuild;
        private SuperScrollViewController _scrollViewDec;
        private Dictionary<int, GetBuildItem> _items;
        private Dictionary<int, GetBuildItem> _itemsDec;
        private GetBuildItem _nowGetBuildItem;

        private List<BuildJd> _buildJDs;
        private List<BuildJd> _buildDecJDs;

        //Tag页
        private int NowTagIndex;
        private Transform Tr_MenuTags;
        private Dictionary<int, MenuTagItem> MenuTags = new Dictionary<int, MenuTagItem>();
        #endregion

        #region 皮肤商店页面
        private Transform _trShopView;

        private Transform _trShopMenuTags;
        private List<MenuTagItem> _shopMenuTags;
        private Transform _trShopInMenuTags;
        private List<MenuTagItem> _shopInMenuTags;

        private JButton _closeShopBtn;
        private JButton _allEquipBtn;
        private JButton _allBuyBtn;

        private int _nowShopTagIndex;

        private GameObject _goShopitem;
        private SuperScrollViewController _scrollViewShop;
        private Dictionary<int, GetBuildItem> _shopItems;
        private GetBuildItem _nowShopItem;

        private List<BuildingSkinJd> _shopList;
        private List<BuildingSkinJd> _skinList;
        private List<BuildingSkinJd> _decList;
        private List<BuildingSkinJd> _showList;
        #endregion

        #region 进入单个建筑页面
        private Transform _trBuildInView;

        private JButton _infoViewBackBtn;
        private JButton _infoViewBackBtn2;
        private JButton _LvUpBuildBtn;

        private Image _buildIcon;
        private Image _buildIcon2;
        private List<Text> _buildEffects;
        private List<Text> _buildEffects2;
        private Text _buildLv;
        private Text _buildGDP;
        private Text _buildCost;
        private ObjectMap _buildCostMap;

        private ObjectMap _buildWorkerMap;
        #endregion

        #region 生产管理页面
        private Transform _trProduceView;
        private Transform _trProductView;
        private Transform _trWorkInfoView;

        private Transform _trWorkLineView;

        private JButton _colseProduceBtn;
        private JButton _colseProduceBtn2;

        private Transform _heroSps;
        private JButton _stopWorkBtn;
        private JButton _collectBtn;
        private RecipeItem _nowWorkItem;
        private Text _workTime;

        private int _nowProduceIndex;
        private Transform _trProduceTags;
        private List<MenuTagItem> _produceTags;

        private int showRecipeLayerIndex;
        private Transform _trTecipeLayer;
        private List<RecipeLayerItem> _recipeLayerItems;

        private RecipeItem _productItem;
        private ObjectMap _materialItemMap;

        private ObjectMap _workHeroMap;
        private List<GameObject> _workHeroSpines;

        private WorkLineItem _nowWorkLineItem;
        private ObjectMap _workLineMap;
        private JButton _colseWorkLineBtn;
        private JButton _colseWorkLineBtn2;
        private List<RecipeItem> _productiItems;
        private SuperScrollViewController _productScrollView;
        private Dictionary<int, List<RecipeJd>> _recipeDataDic;

        
        

        private JButton _addBtn;
        private JButton _delBtn;
        private JButton _productBtn;
        private int _productNum;
        private Text _producttime;
        private Text _texProductnum;

        private bool _costEnough;
        #endregion

        private bool _canSaveBuild;


        public override void Init()
        {
            base.Init();

            _trGround = transform.Find("Ground");
            _trGround.gameObject.SetActive(false);

            _trWorkHeroSps = _trGround.Find("HeroSps");

            _tdCamera = RootTransform.parent.Find("cam").GetComponent<Camera>();

            var index = 0;

            #region 按钮
            _buildTypeBtn = RootCanvasTransform.Find("btn_BuildType").GetComponent<JButton>();
            _buildTypeBtn.AddEvent(EventTriggerType.PointerClick, SetBuildMode);
            _normalTypeBtn = RootCanvasTransform.Find("btn_NormalType").GetComponent<JButton>();
            _normalTypeBtn.AddEvent(EventTriggerType.PointerClick, SetNormalMode);

            _btns = RootCanvasTransform.Find("Btns");
            _produceBtn = _btns.Find("btn_Produce").GetComponent<JButton>();
            _produceBtn.AddEvent(EventTriggerType.PointerClick, ShowWorkLineView);

            _closeUIBtn = RootCanvasTransform.Find("btn_CloseUI").GetComponent<JButton>();
            _closeUIBtn.AddEvent(EventTriggerType.PointerClick, CloseUI);
            _canvasGroup = RootCanvasTransform.GetComponent<CanvasGroup>();
            _imgCloseUI = RootCanvasTransform.Find("img_CloseUI").gameObject;
            _imgCloseUI.SetActive(false);
            _imgCloseUI.GetComponent<JButton>().AddEvent(EventTriggerType.PointerClick, CloseUI);

            _getBuildBtn = RootCanvasTransform.Find("btn_GetBuild").GetComponent<JButton>();
            _getBuildBtn.AddEvent(EventTriggerType.PointerClick, ShowGetBuildView);
            #endregion

            #region 建筑
            _trBuild = _trGround.Find("Builds");
            _goBuild = _trBuild.Find("Build").gameObject;
            _builds = new Dictionary<int, Build>();
            _goBuild.SetActive(false);
            #endregion

            #region 顶部标题栏
            _trTitle = RootCanvasTransform.Find("Title");
            _baseBuildname = _trTitle.Find("tex_baseLv").GetComponent<Text>();

            _trResView = RootCanvasTransform.Find("ResView");
            _showResViewBtn = _trTitle.Find("bg").GetComponent<JButton>();
            _showResViewBtn.AddEvent(EventTriggerType.PointerClick, ShowResView);
            _closeResViewBtn = _trResView.Find("closebg").GetComponent<JButton>();
            _closeResViewBtn.AddEvent(EventTriggerType.PointerClick, CloseResView);
            _resGetMap = _trResView.Find("ResItems").GetComponent<ObjectMap>();
            index = 0;
            foreach (var itemJd in JsonDataMgr.Instance.ItemMap)
            {
                if (itemJd.Value.Type == ItemType.BasicRes)
                {
                    _resGetMap.Get<ResGetItem>(index);
                    index++;
                }
            }

            _resMap = _trTitle.Find("ResItems").GetComponent<ObjectMap>();
            index = 0;
            foreach (var itemJd in JsonDataMgr.Instance.ItemMap)
            {
                if (itemJd.Value.Type == ItemType.BasicRes)
                {
                    _resMap.Get<GenericItem>(index);
                    index++;
                }
            }

            _backBtn = _trTitle.Find("backbtn").GetComponent<JButton>();
            _backBtn.AddEvent(EventTriggerType.PointerClick, Back);

            _trPiggyBank = RootCanvasTransform.Find("PiggyBank");
            _getMoneyBtn = _trPiggyBank.Find("btn_GetMoney").GetComponent<JButton>();
            _getMoneyBtn.AddEvent(EventTriggerType.PointerClick, GetMoney);
            _moneyImage = _getMoneyBtn.transform.Find("img_icon").GetComponent<Image>();
            _moneyRedPoint = _getMoneyBtn.transform.Find("img_redpoint").GetComponent<Image>();
            _moneyproduce = _trPiggyBank.Find("tex_Produce").GetComponent<Text>();

            _baseExp = RootCanvasTransform.Find("Title/img_exp").GetComponent<Image>();
            _baseExpWidth = _baseExp.rectTransform.GetWith();

            _texMonny = _trTitle.Find("tex_jinbi").GetComponent<Text>();
            _texDiamond = _trTitle.Find("tex_zuanshi").GetComponent<Text>();
            #endregion

            #region 创建地板
            _terrain = _trGround.Find("Terrain");
            _tiles = new GroundTile[36][];
            for (int i = 0; i < 36; i++)
            {
                _tiles[i] = new GroundTile[36];
            }

            for (int i = 0; i < _terrain.childCount; i++)
            {
                var gti = _terrain.GetChild(i).GetComponent<GroundTile>();
                _tiles[gti.X][gti.Z] = gti;
                gti.Init();
                gti.JB.AddEvent(EventTriggerType.PointerClick, GroundClick);
                gti.JB.AddEvent(EventTriggerType.Drag, GroundDrag);
                gti.JB.AddEvent(EventTriggerType.EndDrag, GroundEndGrag);
                var tile = JsonDataMgr.Instance.TerrainInfoMap[gti.Z + 1].XList[gti.X];
                var tileInfo = JsonDataMgr.Instance.TerrainTileInfoMap[tile];
                if (tileInfo.ID == 132 || tileInfo.ID == 133)
                {
                    _tiles[gti.X][gti.Z].Height = 0.3f;
                }
            }
            RefeshTileInfo();

            ShowTitle(false);
            SetBuildCollider(true);
            #endregion

            #region 生产线页面
            _trWorkLineView = RootCanvasTransform.Find("WorkLineView");
            _workLineMap = _trWorkLineView.Find("WorkLines/Mask/List").GetComponent<ObjectMap>();
            for (int i = 0; i < 4; i++)
            {
                var ti = _workLineMap.Get<WorkLineItem>(i);
                ti.GetAction = WorkLineItemGetBtn;
                ti.QuickenAction = WorkLineItemQuickenBtn;
                ti.ShowProduceAction = ShowProduceView;
                ti.SetActions();
            }
            _colseWorkLineBtn = _trWorkLineView.Find("closebg").GetComponent<JButton>();
            _colseWorkLineBtn.AddEvent(EventTriggerType.PointerClick, CloseWorkLineView);
            _colseWorkLineBtn2 = _trWorkLineView.Find("btn_Close").GetComponent<JButton>();
            _colseWorkLineBtn2.AddEvent(EventTriggerType.PointerClick, CloseWorkLineView);
            #endregion
            #region 生产管理页面
            _trProduceView = RootCanvasTransform.Find("ProduceView");
            _trProductView = _trProduceView.Find("ProductView");
            _trProductView.gameObject.SetActive(false);
            _trWorkInfoView = _trProduceView.Find("WorkInfoView");
            _trWorkInfoView.gameObject.SetActive(false);

            _colseProduceBtn = _trProduceView.Find("btn_close").GetComponent<JButton>();
            _colseProduceBtn.AddEvent(EventTriggerType.PointerClick, CloseProduceView);
            _colseProduceBtn2 = _trProduceView.Find("closebg").GetComponent<JButton>();
            _colseProduceBtn2.AddEvent(EventTriggerType.PointerClick, CloseProduceView);
            _productScrollView = _trProductView.Find("Products").GetComponent<SuperScrollViewController>();
            _productiItems = new List<RecipeItem>();

            _workHeroMap = _trProduceView.Find("Heros").GetComponent<ObjectMap>();
            for (int i = 0; i < 3; i++)
            {
                var whi = _workHeroMap.Get<WorkHeroItem>(i);
                whi.Object.SetActive(true);
                whi.SetNull(true);
                whi.JB.AddEvent(EventTriggerType.PointerClick, ShowChoseHero);
            }

            _productItem = new RecipeItem();
            _productItem.Object = _trProductView.Find("ProductItem").gameObject;
            _productItem.Init();
            _materialItemMap = _trProductView.Find("MaterialItems").GetComponent<ObjectMap>();
            for (int i = 0; i < 4; i++)
            {
                var mi = _materialItemMap.Get<RecipeItem>(i);
                mi.Init();
                mi.JB.AddEvent(EventTriggerType.PointerClick, AddQueue);
            }

            _trTecipeLayer = _trProductView.Find("RecipeLayer");
            _recipeLayerItems = new List<RecipeLayerItem>();
            for (int i = 0; i < _trTecipeLayer.childCount; i++)
            {
                var rli = new RecipeLayerItem();
                rli.Object = _trTecipeLayer.GetChild(i).gameObject;
                rli.Init();
                rli.ID = i;
                rli.Object.name = $"{i}";
                rli.JB.AddEvent(EventTriggerType.PointerClick, ShowQueue);
                rli.SetItem(null);
                _recipeLayerItems.Add(rli);
            }

            _heroSps = _trWorkInfoView.Find("Heros");
            _workHeroSpines = new List<GameObject>();
            _nowWorkItem = new RecipeItem();
            _nowWorkItem.Object = _trWorkInfoView.Find("ProductItem").gameObject;
            _nowWorkItem.Init();
            _workTime = _trWorkInfoView.Find("tex_time").GetComponent<Text>();
            _stopWorkBtn = _trWorkInfoView.Find("btn_stopWork").GetComponent<JButton>();
            _stopWorkBtn.AddEvent(EventTriggerType.PointerClick, StopWorkLine);
            _collectBtn = _trWorkInfoView.Find("btn_collect").GetComponent<JButton>();
            _collectBtn.AddEvent(EventTriggerType.PointerClick, CollectWorkLine);

            _trProduceTags = _trProductView.Find("MenuTags");
            _produceTags = new List<MenuTagItem>();
            for (int i = 0; i < _trProduceTags.childCount; i++)
            {
                MenuTagItem MT = new MenuTagItem();
                MT = MT.New(_trProduceTags.GetChild(i));
                MT.Obj.name = $"{i}";
                MT.JB.AddEvent(EventTriggerType.PointerClick, ChangeProduceTag);
                _produceTags.Add(MT);
            }

            _addBtn = _trProductView.Find("btn_add").GetComponent<JButton>();
            _addBtn.AddEvent(EventTriggerType.PointerClick, AddProduct);
            _delBtn = _trProductView.Find("btn_del").GetComponent<JButton>();
            _delBtn.AddEvent(EventTriggerType.PointerClick, DelProduct);
            _productBtn = _trProductView.Find("btn_product").GetComponent<JButton>();
            _productBtn.AddEvent(EventTriggerType.PointerClick, StartWorkLine);
            _texProductnum = _trProductView.Find("tex_num").GetComponent<Text>();
            _producttime = _trProductView.Find("tex_time").GetComponent<Text>();

            _recipeDataDic = new Dictionary<int, List<RecipeJd>>();
            #endregion

            #region 进入单个建筑页面
            _trBuildInView = RootCanvasTransform.Find("BuildInView");

            _LvUpBuildBtn = _trBuildInView.Find("btn_Con").GetComponent<JButton>();
            _LvUpBuildBtn.AddEvent(EventTriggerType.PointerClick, ConLvUpBuild);
            _infoViewBackBtn = _trBuildInView.Find("btn_Close").GetComponent<JButton>();
            _infoViewBackBtn.AddEvent(EventTriggerType.PointerClick, CloseBuildMoreView);
            _infoViewBackBtn2 = _trBuildInView.Find("closebg").GetComponent<JButton>();
            _infoViewBackBtn2.AddEvent(EventTriggerType.PointerClick, CloseBuildMoreView);

            _buildIcon = _trBuildInView.Find("img_build").GetComponent<Image>();
            _buildIcon2 = _trBuildInView.Find("img_build2").GetComponent<Image>();
            _buildLv = _trBuildInView.Find("tex_lv").GetComponent<Text>();
            _buildGDP = _trBuildInView.Find("tex_gdp").GetComponent<Text>();
            _buildCost = _trBuildInView.Find("tex_cost").GetComponent<Text>();

            _buildEffects = new List<Text>();
            _buildEffects2 = new List<Text>();
            var buildEffect1 = _trBuildInView.Find("effect1");
            for (int i = 0; i < buildEffect1.childCount; i++)
            {
                _buildEffects.Add(buildEffect1.GetChild(i).GetComponent<Text>());
            }
            var buildEffect2 = _trBuildInView.Find("effect2");
            for (int i = 0; i < buildEffect2.childCount; i++)
            {
                _buildEffects2.Add(buildEffect2.GetChild(i).GetComponent<Text>());
            }

            _buildCostMap = _trBuildInView.Find("CostlItems").GetComponent<ObjectMap>();
            for (int i = 0; i < 4; i++)
            {
                _buildCostMap.Get<GenericItem>(i);
            }

            _buildWorkerMap = _trBuildInView.Find("Heros").GetComponent<ObjectMap>();
            for (int i = 0; i < 3; i++)
            {
                var whi = _buildWorkerMap.Get<WorkHeroItem>(i);
                whi.Object.SetActive(true);
                whi.SetNull(true);
                whi.JB.AddEvent(EventTriggerType.PointerClick, ShowChoseHero);
            }
            #endregion

            #region 获取建筑页面
            _trGetBuildView = RootCanvasTransform.Find("GetBuildView");
            _trGetBuildView.gameObject.SetActive(true);
            _GetBuildViewCVS = _trGetBuildView.GetComponent<CanvasGroup>();

            _closeGetBuildBtn = _trGetBuildView.Find("CloseBtn").GetComponent<JButton>();
            _closeGetBuildBtn.AddEvent(EventTriggerType.PointerClick, CloseGetBuildView);

            _items = new Dictionary<int, GetBuildItem>();
            _itemsDec = new Dictionary<int, GetBuildItem>();
            _scrollViewBuild = _trGetBuildView.Find("BuildItems").GetComponent<SuperScrollViewController>();
            _scrollViewDec = _trGetBuildView.Find("DecItems").GetComponent<SuperScrollViewController>();

            _buildJDs = new List<BuildJd>();
            _buildDecJDs = new List<BuildJd>();

            //Tag页
            Tr_MenuTags = _trGetBuildView.Find("MenuTags");
            for (int i = 0; i < 2; i++)
            {
                MenuTagItem MT = new MenuTagItem();
                MT = MT.New(Tr_MenuTags.GetChild(i));
                MT.Obj.name = $"{i}";
                MT.JB.AddEvent(EventTriggerType.PointerClick, ChangeTag);
                MenuTags[int.Parse(MT.Obj.name)] = MT;
            }
            #endregion

            #region 商店页面
            _trShopView = RootCanvasTransform.Find("ShopView");

            //Tag页
            _trShopMenuTags = _trShopView.Find("MenuTags");
            _shopMenuTags = new List<MenuTagItem>();
            for (int i = 0; i < _trShopMenuTags.childCount; i++)
            {
                MenuTagItem MT = new MenuTagItem();
                MT = MT.New(_trShopMenuTags.GetChild(i));
                MT.Obj.name = $"{i}";
                MT.JB.AddEvent(EventTriggerType.PointerClick, ChangeShopTag);
                _shopMenuTags.Add(MT);
            }

            //商店内分类页
            _trShopInMenuTags = _trShopView.Find("ShopTypes/Mask/List");
            _shopInMenuTags = new List<MenuTagItem>();
            for (int i = 0; i < _trShopInMenuTags.childCount; i++)
            {
                MenuTagItem MT = new MenuTagItem();
                MT = MT.New(_trShopInMenuTags.GetChild(i));
                MT.Obj.name = $"{i}";
                MT.JB.AddEvent(EventTriggerType.PointerClick, ChangeShopInTag);
                _shopInMenuTags.Add(MT);
            }

            _closeShopBtn = _trShopView.Find("CloseBtn").GetComponent<JButton>();
            _closeShopBtn.AddEvent(EventTriggerType.PointerClick, CloseShopView);
            _allEquipBtn = _trShopView.Find("AllEquipBtn").GetComponent<JButton>();
            _allEquipBtn.AddEvent(EventTriggerType.PointerClick, AllEquip);
            _allBuyBtn = _trShopView.Find("AllBuyBtn").GetComponent<JButton>();
            _allBuyBtn.AddEvent(EventTriggerType.PointerClick, AllBuy);

            _goShopitem = _trShopView.Find("GetBuildItem").gameObject;
            _goShopitem.SetActive(false);
            _shopItems = new Dictionary<int, GetBuildItem>();
            _scrollViewShop = _trShopView.Find("MyBuildItems").GetComponent<SuperScrollViewController>();

            _shopList = new List<BuildingSkinJd>();
            _skinList = new List<BuildingSkinJd>();
            _decList = new List<BuildingSkinJd>();
            _showList = new List<BuildingSkinJd>();
            #endregion

            InitChoseHeroView();
        }

        #region 统一继承生命周期

        public override string Path()
        {
            return PATH;
        }

        public override void BeforeEnter()
        {
            base.BeforeEnter();

            //CreateTdCam();

            Show(true);
            showNow = true;
        }

        private void CreateTdCam()
        {
            var go = new GameObject("td_cam");
            go.AddComponent<PhysicsRaycaster>();
            var cam = go.GetComponent<Camera>();
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.depth = -2;
            go.transform.position = new Vector3(0, 0, -40);

            cam.cullingMask = LayerMask.GetMask("ThreeD");
            cam.orthographic = true;
            cam.orthographicSize = 2.5f;

            _tdCamera = cam;
            go.AddComponent<CameraCon>();
        }

        public override void AfterEnter()
        {
            base.AfterEnter();
        }

        public override void BeforeExit()
        {
            showNow = false;
            base.BeforeExit();
        }

        public override void AfterExit()
        {
            base.AfterExit();
            Show(false);
        }

        public override void Dispose()
        {
            _nowMode = ModeType.Normal;
            Destroy(_tdCamera.gameObject);
            _tdCamera = null;

            _buildIndex = 0;
            foreach (var build in _builds)
            {
                Destroy(build.Value.Obj);
            }
            _builds.Clear();

            foreach (var groundTile in _tiles)
            {
                foreach (var tile in groundTile)
                {
                    tile.ResetTile();
                }
            }
        }

        #endregion

        public void Show(bool _bool)
        {
            _trGround.gameObject.SetActive(_bool);

            if (_bool)
            {
                GetDatas();

                _trBuildInView.gameObject.SetActive(false);
                _trShopView.gameObject.SetActive(false);
                _trProduceView.gameObject.SetActive(false);
                _trWorkLineView.gameObject.SetActive(false);
                _trResView.gameObject.SetActive(false);

                ShowChoseHeroView(false);

                ChoseBuildType(_nowMode);

                SetMyBuildsAsync();

                //NPC移动
                //InitASMap(); 
                //UniTaskHeroRun();
            }
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        private void GetDatas()
        {
            _BuildDatas = new Dictionary<string, PLandBuild>();
            _recipeDataDic.Clear();
            _shopList.Clear();

            RefeshBuidData();

            foreach (var BSJd in JsonDataMgr.Instance.BuildingSkinMap)
            {
                _shopList.Add(BSJd.Value);

                if (BSJd.Value.GroupID == 1)
                {
                    _skinList.Add(BSJd.Value);
                }
                else if (BSJd.Value.GroupID == 2)
                {
                    _decList.Add(BSJd.Value);
                }
            }

            var recipe1 = new List<RecipeJd>();
            var recipe2 = new List<RecipeJd>();
            foreach (var recipeJd in JsonDataMgr.Instance.RecipeMap)
            {
                if (recipeJd.Value.Type == RecipeType.Manuf)
                {
                    recipe1.Add(recipeJd.Value);
                }
                else
                {
                    recipe2.Add(recipeJd.Value);
                }
            }
            _recipeDataDic.Add(0, recipe1);
            _recipeDataDic.Add(1, recipe2);

            _buildJDs.Clear();
            _buildDecJDs.Clear();

            foreach (var buildJd in JsonDataMgr.Instance.BuildMap)
            {
                if (buildJd.Value.Type != BuildType.Decorate && buildJd.Value.Type != BuildType.Base)
                {
                    _buildJDs.Add(buildJd.Value);
                }
                else if (buildJd.Value.Type == BuildType.Decorate)
                {
                    _buildDecJDs.Add(buildJd.Value);
                }
            }
        }
        private void RefeshBuidData()
        {
            _BuildDatas.Clear();
            foreach (var pLandBuild in KingdomDataMgr.Instance.BuildDataMap)
            {
                _BuildDatas.Add(pLandBuild.Value.InsID, pLandBuild.Value);
                if (pLandBuild.Value.DictID == 1001)
                {
                    _mainData = pLandBuild.Value;
                }
            }

            if (_builds.Count != 0)
            {
                foreach (var build in _builds)
                {
                    build.Value.RefeshBuid();
                }
            }
        }

        /// <summary>
        /// 刷新地板开启状态
        /// </summary>
        private void RefeshTileInfo()
        {
            var fjd = JsonDataMgr.Instance.FieldUnlocklMap[1];
            foreach (var groundTile in _tiles)
            {
                foreach (var tile in groundTile)
                {
                    if (tile.X >= fjd.Unlock[0].X - 1 && tile.X <= fjd.Unlock[1].X - 1)
                    {
                        if (tile.Z >= fjd.Unlock[0].Y - 1 && tile.Z <= fjd.Unlock[1].Y - 1)
                        {
                            tile.Unlock = true;
                        }
                    }
                    tile.SetLock();
                }
            }
        }

        private async UniTask SetMyBuildsAsync()
        {
            foreach (var myBuildData in _BuildDatas)
            {
                CreateBuild(myBuildData.Value.DictID);
                _nowBuild.SetBuild(myBuildData.Value);
                await _nowBuild.SetIcon();
                ConSetBuild();
            }

            CheckErrorBuild();
            ReSetPutInfo();
            _nowMode = ModeType.Normal;
        }

        private void RefeshTitle()
        {
            _baseBuildname.text = $"Lv{KingdomDataMgr.Instance.GetBaseLv()}";
            _baseExp.rectTransform.sizeDelta = new Vector2(KingdomDataMgr.Instance.GetBaseExp() > 15 ? KingdomDataMgr.Instance.GetBaseExp() / _baseExpWidth : 15, _baseExp.rectTransform.sizeDelta.y);
            _baseBuildLv = KingdomDataMgr.Instance.GetBaseLv();

            var index = 0;
            foreach (var itemJd in JsonDataMgr.Instance.ItemMap)
            {
                if (itemJd.Value.Type == ItemType.BasicRes)
                {
                    var gi = _resMap.Get<GenericItem>(index);
                    UserDataMgr.Instance.NormalItemMap.TryGetValue(itemJd.Value.ID, out var itemData);
                    gi.SetItem(itemJd.Value, itemData?.Num ?? 0);
                    index++;
                }
            }

            var moneyproduce = 0;
            foreach (var buildData in _BuildDatas)
            {
                moneyproduce += JsonDataMgr.Instance.BuildLvMap[buildData.Value.DictID][buildData.Value.Lv].GDP;
            }
            _moneyproduce.text = $"+{moneyproduce}/m";

            _moneyImage.fillAmount = (float)KingdomDataMgr.Instance.Tax / (float)JsonDataMgr.Instance.BaseLvMap[_baseBuildLv].TaxMax;
            _moneyRedPoint.gameObject.SetActive(KingdomDataMgr.Instance.Tax >= JsonDataMgr.Instance.BaseLvMap[_baseBuildLv].TaxMax);

            UserDataMgr.Instance.NormalItemMap.TryGetValue(900001, out var m1);
            UserDataMgr.Instance.NormalItemMap.TryGetValue(900003, out var m2);
            _texMonny.text = $"{m1.Num}";
            _texDiamond.text = $"{m2.Num}";
        }

        private void ShowResView(BaseEventData bed)
        {
            _trResView.gameObject.SetActive(true);
            var index = 0;
            foreach (var itemJd in JsonDataMgr.Instance.ItemMap)
            {
                if (itemJd.Value.Type != ItemType.BasicRes) continue;
                var gi = _resGetMap.Get<ResGetItem>(index);
                gi.SetItem(itemJd.Value);
                index++;
            }
        }
        private void CloseResView(BaseEventData bed)
        {
            _trResView.gameObject.SetActive(false);
        }

        private void Back(BaseEventData arg0)
        {
            if (_isCreateBuild)
            {
                PopInfoCtrl.Insatnce.Show(true, "当前建筑未确认放置");
                return;
            }

            ViewGroup.SwitchGroup(HomeView.PATH);
        }

        private void GetMoney(BaseEventData baseEventData)
        {
            if (_moneyImage.fillAmount != 0)
            {
                KingdomDataMgr.Instance.GetTaxReq(() =>
                {
                    RefeshTitle();
                });
            }
        }

        #region 最外部各种按钮开关
        private void SetBuildMode(BaseEventData arg0)
        {
            _nowMode = ModeType.Build;

            ChoseBuildType(_nowMode);
        }
        private void SetNormalMode(BaseEventData arg0)
        {
            //PopInfoCtrl.Insatnce.ShowAnim(true, "确认保存当前建筑位置信息吗");
            if (_canSaveBuild)
            {
                MoveTo(_nowBuild);
                PopInfoCtrl.Insatnce.Show(true, "当前建筑摆放在错误位置");
                return;
            }

            foreach (var build in _builds)
            {
                if (build.Value.CheckIsSetBuid())
                {
                    MoveTo(_nowBuild);
                    PopInfoCtrl.Insatnce.Show(true, "当前建筑未确认放置");
                    return;
                }
            }

            if (_isCreateBuild)
            {
                MoveTo(_nowBuild);
                PopInfoCtrl.Insatnce.Show(true, "当前建筑未确认放置");
                return;
            }

            _nowMode = ModeType.Normal;

            ChoseBuildType(_nowMode);

            SaveBuildInfo();
        }

        /// <summary>
        /// 0.普通模式；1.建筑模式；
        /// </summary>
        private void ChoseBuildType(ModeType modeType)
        {
            if (_nowBuild != null)
            {
                _nowBuild.ShowPutBuildBtns(false);
            }

            _normalTypeBtn.gameObject.SetActive(modeType == ModeType.Build);
            _getBuildBtn.gameObject.SetActive(modeType == ModeType.Build);
            _btns.gameObject.SetActive(modeType == ModeType.Normal);
            _trPiggyBank.gameObject.SetActive(modeType == ModeType.Normal);
            _buildTypeBtn.gameObject.SetActive(modeType == ModeType.Normal);

            if (modeType == ModeType.Normal)
            {
                _nowMode = ModeType.Normal;

                CloseGetBuildView(null);
                ReSetPutInfo();//结束当前放置
            }
            else if (modeType == ModeType.Build)
            {
                _nowMode = ModeType.Build;

                ShowGetBuildView(null);
            }

            RefeshTitle();
        }


        private void CloseUI(BaseEventData bd)
        {
            _isCloseUI = !_isCloseUI;
            _canvasGroup.alpha = _isCloseUI ? 0 : 1;
            _imgCloseUI.SetActive(_isCloseUI);
        }
        #endregion


        #region 升级建筑页面
        private void ShowBuildMoreView(Build Build)
        {
            _trBuildInView.gameObject.SetActive(true);

            RefeshBuildInInfo(Build);
        }

        private void CloseBuildMoreView(BaseEventData arg0)
        {
            ChoseBuildType(_nowMode);
            _trBuildInView.gameObject.SetActive(false);
            _nowBuild = null;
        }

        private void RefeshBuildInInfo(Build _Build)
        {
            _buildLv.text = $"{_Build.BuildData.Lv}";
            var blvjd = JsonDataMgr.Instance.BuildLvMap[_Build.BuildJd.ID][_Build.BuildData.Lv];
            _buildCost.text = $"{blvjd.Cost}";
            _buildGDP.text = $"{blvjd.GDP}";
            for (int i = 0; i < _buildCostMap.Map.Count; i++)
            {
                _buildCostMap.Get<GenericItem>(i).Object.SetActive(false);
            }

            var index = 0;
            foreach (var genericItemJd in blvjd.Recipe)
            {
                var ijd = JsonDataMgr.Instance.ItemMap[genericItemJd.Id];
                _buildCostMap.Get<GenericItem>(index).SetItem(genericItemJd, true);
                index++;
            }

            foreach (var buildEffect in _buildEffects)
            {
                buildEffect.gameObject.SetActive(false);
            }
            foreach (var buildEffect2 in _buildEffects2)
            {
                buildEffect2.gameObject.SetActive(false);
            }


            for (var i = 0; i < blvjd.Skills.Count; i++)
            {
                _buildEffects[i].gameObject.SetActive(true);
                JsonDataMgr.Instance.ExSkillMap.TryGetValue(blvjd.Skills[i],out var sjd);
                if (sjd != null) _buildEffects[i].text = sjd.Desc;
            }
            JsonDataMgr.Instance.BuildLvMap[_nowBuild.BuildData.DictID].TryGetValue(_nowBuild.BuildData.Lv + 1, out var bljd2);
            if (bljd2 != null)
            {
                for (var i = 0; i < bljd2.Skills.Count; i++)
                {
                    _buildEffects2[i].gameObject.SetActive(true);
                    JsonDataMgr.Instance.ExSkillMap.TryGetValue(bljd2.Skills[i], out var sjd);
                    if (sjd != null) _buildEffects2[i].text = sjd.Desc;
                }
            }
                

            SetIcon(blvjd);

            index = 0;
            foreach (var charID in _nowBuild.BuildData.AvatarQ)
            {
                var hjd = JsonDataMgr.Instance.CharacterMap[charID];
                _buildWorkerMap.Get<WorkHeroItem>(index).SetHero(hjd);
                index++;
            }

            for (int i = index; i < _buildWorkerMap.Map.Count; i++)
            {
                _buildWorkerMap.Get<WorkHeroItem>(i).SetNull(true);
            }
        }

        public async UniTask SetIcon(BuildLvJd blvjd)
        {
            _buildIcon.sprite = await AssetMgr.Instance.LoadSprite(AssetMgr.Build_PATH, blvjd.Icon);
            JsonDataMgr.Instance.BuildLvMap[_nowBuild.BuildData.DictID].TryGetValue(_nowBuild.BuildData.Lv + 1, out var bljd2);
            _buildIcon2.sprite = await AssetMgr.Instance.LoadSprite(AssetMgr.Build_PATH, bljd2 == null ? blvjd.Icon : bljd2.Icon);

        }

        private void ConLvUpBuild(BaseEventData arg0)
        {
            if (_nowBuild.BuildData.Lv == _nowBuild.BuildJd.LvMax)
            {
                PopInfoCtrl.Insatnce.Show(true, "达到最大等级");
                return;
            }

            if (_nowWorkLineItem != null)
            {
                if (!KingdomDataMgr.Instance.CheckWorkLineFree(_nowWorkLineItem.WorkBuild.InsID))
                {
                    PopInfoCtrl.Insatnce.Show(true, "生产中建筑无法升级");
                    return;
                }
            }
            

            KingdomDataMgr.Instance.LvUpBuildReq(_nowBuild.BuildData.InsID, () =>
            {
                RefeshBuidData();
                _nowBuild.RefeshBuid();
                RefeshTitle();
                RefeshBuildInInfo(_nowBuild);
            });
        }

        #endregion

        #region 生产线
        private void ShowWorkLineView(BaseEventData arg0)
        {
            _trWorkLineView.gameObject.SetActive(true);
            _trProduceView.gameObject.SetActive(false);

            for (int i = 0; i < _workLineMap.Map.Count; i++)
            {
                var gi = _workLineMap.Get<WorkLineItem>(i);
                gi.Object.SetActive(false);
            }

            RefeshWlineItem();
        }

        private async UniTask RefeshWlineItem()
        {
            var index = 0;
            //按生产建筑创建生产线
            foreach (var pLandBuild in _BuildDatas)
            {
                var bjd = JsonDataMgr.Instance.BuildMap[pLandBuild.Value.DictID];
                if (bjd.Type == BuildType.Factory)
                {
                    var gi = _workLineMap.Get<WorkLineItem>(index);
                    await gi.SetItem(pLandBuild.Value);
                    index++;
                }
            }
        }

        private void CloseWorkLineView(BaseEventData arg0)
        {
            _trWorkLineView.gameObject.SetActive(false);
        }

        private void WorkLineItemGetBtn(BaseEventData arg0)
        {
            _nowWorkLineItem = _workLineMap.Get<WorkLineItem>(int.Parse(arg0.selectedObject.name));
            CollectWorkLine(null);
        }
        private void WorkLineItemQuickenBtn(BaseEventData arg0)
        {
            _nowWorkLineItem = _workLineMap.Get<WorkLineItem>(int.Parse(arg0.selectedObject.name));
        }
        #endregion
        #region 生产管理页面
        private void ShowProduceView(BaseEventData arg0)
        {
            _nowWorkLineItem = _workLineMap.Get<WorkLineItem>(int.Parse(arg0.selectedObject.name));

            _trWorkLineView.gameObject.SetActive(false);
            _trProduceView.gameObject.SetActive(true);

            RefeshWorkHero();

            if (_nowWorkLineItem.IsWork)
            {
                ShowWorkInInfoView();
            }
            else
            {
                _trProductView.gameObject.SetActive(true);
                _trWorkInfoView.gameObject.SetActive(false);

                var pos = _workHeroMap.transform.localPosition;
                _workHeroMap.transform.localPosition = new Vector3(pos.x, 200, 0);

                MakeProduceSelect(_nowProduceIndex);
            }
        }

        private async UniTask RefeshWorkHero()
        {
            var index = 0;
            foreach (var charID in _nowWorkLineItem.WorkBuild.AvatarQ)
            {
                var hjd = JsonDataMgr.Instance.CharacterMap[charID];
                _workHeroMap.Get<WorkHeroItem>(index).SetHero(hjd);
                index++;
            }

            for (int i = index; i < _workHeroMap.Map.Count; i++)
            {
                _workHeroMap.Get<WorkHeroItem>(i).SetNull(true);
            }
        }

        private void ShowWorkInInfoView()
        {
            _trProductView.gameObject.SetActive(false);
            _trWorkInfoView.gameObject.SetActive(true);

            var pos = _workHeroMap.transform.localPosition;
            _workHeroMap.transform.localPosition = new Vector3(pos.x, -260, 0);

            ShowWorkInfoViewAsync();
        }

        private void CloseProduceView(BaseEventData arg0)
        {
            _trWorkLineView.gameObject.SetActive(true);
            _trProduceView.gameObject.SetActive(false);
            _trProductView.gameObject.SetActive(false);
            _trWorkInfoView.gameObject.SetActive(false);
        }
        private void ChangeProduceTag(BaseEventData arg0)
        {
            MakeProduceSelect(int.Parse(arg0.selectedObject.name));
        }
        private async UniTask MakeProduceSelect(int Type)
        {
            _nowProduceIndex = Type;
            foreach (var Tag in _produceTags)
            {
                Tag.MakeSelected(false);
            }
            _produceTags[_nowProduceIndex].MakeSelected(true);

            RefeshProducts();

            ShowProductCompose(_productiItems[0].RecipeJd.ID);
            ResetQueue();
            var rli = _recipeLayerItems[showRecipeLayerIndex];
            rli.SetItem(_productItem);
            AddLoadSpriteAction(rli.ItemJd.ID.ToString(), v => { rli.SetIcon(v); });
            showRecipeLayerIndex++;
        }

        private void StartWorkLine(BaseEventData arg0)
        {
            if (KingdomDataMgr.Instance.CheckWorkLineFree(_nowWorkLineItem.WorkBuild.InsID))
            {
                KingdomDataMgr.Instance.StartWorkLineReq(_nowWorkLineItem.WorkBuild.InsID, _productItem.ItemID, _productNum,
                    () =>
                    {
                        RefeshBuidData();
                        _nowWorkLineItem.RefeshWorkLineData();
                        ShowWorkInInfoView();
                        RefeshTitle();
                    });
            }
        }
        private void StopWorkLine(BaseEventData arg0)
        {
            KingdomDataMgr.Instance.StopWorkLineReq(_nowWorkLineItem.WorkBuild.InsID,
                () =>
                {
                    RefeshBuidData();
                    ShowWorkLineView(null);
                    RefeshTitle();
                });
        }
        private void CollectWorkLine(BaseEventData arg0)
        {
            if (_nowWorkLineItem.NowWorkItem.WorkDoneNum != 0)
            {
                KingdomDataMgr.Instance.CollectWorkLineReq(_nowWorkLineItem.WorkBuild.InsID,
                    () =>
                    {
                        RefeshBuidData();
                        ShowWorkLineView(null);
                    });
            }
        }

        private void RefeshProducts()
        {
            _productiItems.Clear();
            _productScrollView.NewScroll(_recipeDataDic[_nowProduceIndex].Count, OnRefeshOneProduct);
        }
        private void ContinueRefeshProduct()
        {
            _productScrollView.ContinueScroll(_recipeDataDic[_nowProduceIndex].Count);
        }
        private void OnRefeshOneProduct(Transform Tr, int Index)
        {
            RecipeItem ri = new RecipeItem();
            ri.Object = Tr.gameObject;
            ri.Init();
            ri.SetItemAsync(_recipeDataDic[_nowProduceIndex][Index].ProductID, 1);
            ri.Object.name = $"{Index}";
            ri.Transform.localRotation = Quaternion.identity;
            ri.JB.ClearEvent();
            ri.JB.AddEvent(EventTriggerType.PointerClick, ProductItemClick);
            _productiItems.Add(ri);

            AddLoadSpriteAction(ri.ItemJd.ID.ToString(), v => { ri.SetIcon(v); });
        }

        private void ProductItemClick(BaseEventData arg0)
        {
            var id = _productiItems[int.Parse(arg0.selectedObject.name)].RecipeJd.ID;
            RefeshProductInfo(id);
        }

        private void RefeshProductInfo(int id)
        {
            ShowProductCompose(id);
            ResetQueue();
            var rli = _recipeLayerItems[showRecipeLayerIndex];
            rli.SetItem(_productItem);
            AddLoadSpriteAction(rli.ItemJd.ID.ToString(), v => { rli.SetIcon(v); });
            showRecipeLayerIndex++;
        }
        private void ShowProductCompose(int id)
        {
            _productItem.SetItemAsync(id, 1);
            AddLoadSpriteAction(_productItem.ItemJd.ID.ToString(), v => { _productItem.SetIcon(v); });
            foreach (var materialItem in _materialItemMap.Map)
            {
                materialItem.Value.Object.SetActive(false);
            }

            _productNum = 1;
            RefeshProductCost();
        }

        private void ResetQueue()
        {
            showRecipeLayerIndex = 0;
            foreach (var recipeLayerItem in _recipeLayerItems)
            {
                recipeLayerItem.SetItem(null);
            }
        }
        private void AddQueue(BaseEventData bed)
        {
            var mi = _materialItemMap.Get<RecipeItem>(int.Parse(bed.selectedObject.name));
            if (mi.RecipeJd != null)
            {
                var rli = _recipeLayerItems[showRecipeLayerIndex];
                rli.SetItem(mi);
                AddLoadSpriteAction(rli.ItemJd.ID.ToString(), v => { rli.SetIcon(v); });
                showRecipeLayerIndex++;
                ShowProductCompose(mi.RecipeJd.ProductID);
            }
        }
        private void ShowQueue(BaseEventData bed)
        {
            var rli = _recipeLayerItems[int.Parse(bed.selectedObject.name)];
            if (rli.RecipeJd == null) return;

            ShowProductCompose(rli.RecipeJd.ProductID);
            for (int i = rli.ID + 1; i < _recipeLayerItems.Count; i++)
            {
                _recipeLayerItems[i].SetItem(null);
            }

            showRecipeLayerIndex = rli.ID + 1;
        }

        private void RefeshProductCost()
        {
            _texProductnum.text = $"{_productNum}";

            var time = _productItem.RecipeJd.Time * _productNum;
            var h = time / 3600;
            var m = (time - 3600 * h) / 60;
            var s = (time - 3600 * h - 60 * m);
            var showh = h >= 10 ? $"{h}" : "0" + h;
            var showm = m >= 10 ? $"{m}" : "0" + m;
            var shows = s >= 10 ? $"{s}" : "0" + s;
            _producttime.text = $"{showh}:{showm}:{shows}";

            var recipes = _productItem.RecipeJd.Recipe;
            for (int i = 0; i < recipes.Count; i++)
            {
                var mi = _materialItemMap.Get<RecipeItem>(i);
                mi.SetItemAsync(recipes[i].Id, recipes[i].Count * _productNum);
                AddLoadSpriteAction(mi.ItemJd.ID.ToString(), v => { mi.SetIcon(v); });
            }

        }
        private void AddProduct(BaseEventData arg0)
        {
            _productNum++;
            RefeshProductCost();
        }
        private void DelProduct(BaseEventData arg0)
        {
            if (_productNum - 1 == 0)
            {
                return;
            }
            _productNum--;
            RefeshProductCost();
        }

        private async UniTask ShowWorkInfoViewAsync()
        {
            foreach (var workHeroSpine in _workHeroSpines)
            {
                Destroy(workHeroSpine);
            }
            _workHeroSpines.Clear();

            var index = 0;
            foreach (var charID in _nowWorkLineItem.WorkBuild.AvatarQ)
            {
                var obj = await SetSpine(_heroSps, charID);
                obj.transform.localScale = new Vector3(60, 60, 0);
                obj.transform.localPosition = new Vector3((index - 1) * 150, 0, 0);
                _workHeroSpines.Add(obj);
                index++;
            }

            _nowWorkItem.SetWorkLineItemAsync(_nowWorkLineItem.RecipeJd);
        }
        private async UniTask<GameObject> SetSpine(Transform tr, int charId)
        {
            var spine = JsonDataMgr.Instance.CharacterMap[charId].Spine;

            var sp = await AssetMgr.Instance.LoadObject(
                AssetMgr.SPINE_PATH, $"sp_{spine}_0");

            var obj = Instantiate(sp, tr
                , false);
            var child = obj.transform.GetChild(0);
            var animation = child.GetComponent<SkeletonAnimation>();
            animation.Initialize(true);
            animation.AnimationState.SetAnimation(0, "idle", true);

            var mr = animation.GetComponent<MeshRenderer>();
            mr.sortingOrder = 11;

            return obj;
        }

        public void SetWorkInTime(int nowTime)
        {
            var time = _nowWorkLineItem.WorkBuild.ProduceTask.EndTime - nowTime;

            if (time <= 0)
            {
                _workTime.text = $"订单已完成";
                _nowWorkItem.SetTime(nowTime, _nowWorkLineItem.WorkBuild);
                return;
            }

            var h = time / 3600;
            var m = (time - 3600 * h) / 60;
            var s = (time - 3600 * h - 60 * m);

            var showh = h >= 10 ? $"{h}" : "0" + h;
            var showm = m >= 10 ? $"{m}" : "0" + m;
            var shows = s >= 10 ? $"{s}" : "0" + s;

            _workTime.text = $"{showh}:{showm}:{shows}";
            _nowWorkItem.SetTime(nowTime, _nowWorkLineItem.WorkBuild);
        }

        #endregion


        #region 购买建筑页面
        private void ShowGetBuildView(BaseEventData arg0)
        {
            _nowBuild?.ShowBtns(false);
            _GetBuildViewCVS.alpha = 1;
            _GetBuildViewCVS.blocksRaycasts = true;
            _normalTypeBtn.transform.localPosition = new Vector3(_normalTypeBtn.transform.localPosition.x, -95, 0);

            MakeSelect(0);
        }
        private void CloseGetBuildView(BaseEventData arg0)
        {
            _GetBuildViewCVS.alpha = 0;
            _GetBuildViewCVS.blocksRaycasts = false;
            _normalTypeBtn.transform.localPosition = new Vector3(_normalTypeBtn.transform.localPosition.x, -295, 0);
        }

        private void ChangeTag(BaseEventData arg0)
        {
            MakeSelect(int.Parse(arg0.selectedObject.name));
        }

        public void MakeSelect(int type)
        {
            NowTagIndex = type;
            foreach (var menuTagItem in MenuTags)
            {
                menuTagItem.Value.MakeSelected(false);
            }
            MenuTags[type].MakeSelected(true);

            if (NowTagIndex == 0)
            {
                _scrollViewBuild.gameObject.SetActive(true);
                _scrollViewDec.gameObject.SetActive(false);
                RefeshGetBuild();
            }
            else
            {
                _scrollViewBuild.gameObject.SetActive(false);
                _scrollViewDec.gameObject.SetActive(true);
                RefeshGetDec();
            }
        }

        private void RefeshGetBuild()
        {
            _items.Clear();
            _scrollViewBuild.NewScroll(_buildJDs.Count, OnRefeshOneGet);
        }
        private void ContinueRefeshGet()
        {
            _scrollViewBuild.ContinueScroll(_buildJDs.Count);
        }
        private void OnRefeshOneGet(Transform Tr, int Index)
        {
            GetFucBuildItem gbi = new GetFucBuildItem();
            gbi.Object = Tr.gameObject;
            gbi.Init();
            gbi.SetGetItem(_buildJDs[Index]);
            gbi.Object.name = $"{Index}";
            gbi.Transform.localRotation = Quaternion.identity;
            gbi.JB.ClearEvent();
            gbi.JB.AddEvent(EventTriggerType.BeginDrag, GetBtnDragBegin);
            gbi.JB.AddEvent(EventTriggerType.Drag, GetBtnDraging);
            gbi.JB.AddEvent(EventTriggerType.EndDrag, GetBtnDragEnd);
            _items[Index] = gbi;

            var buildcount = 0;
            foreach (var myBuildData in _BuildDatas)
            {
                if (myBuildData.Value.DictID == _buildJDs[Index].ID)
                {
                    buildcount++;
                }
            }
            var buildNum = 0;
            foreach (var buildInfoJd in JsonDataMgr.Instance.BaseLvMap[_mainData.Lv].UnLockbdg)
            {
                if (buildInfoJd.ID == _buildJDs[Index].ID)
                {
                    buildNum = buildInfoJd.Count;
                }
            }
            gbi.SetCount(buildcount, buildNum);
        }
        #endregion
        #region 装饰建筑
        private void RefeshGetDec()
        {
            _itemsDec.Clear();
            _scrollViewDec.NewScroll(_buildDecJDs.Count, OnRefeshOneGetDec);
        }
        private void ContinueRefeshGetDec()
        {
            _scrollViewDec.ContinueScroll(_buildDecJDs.Count);
        }

        private void OnRefeshOneGetDec(Transform Tr, int Index)
        {
            GetDecBuildItem gdi = new GetDecBuildItem();
            gdi.Object = Tr.gameObject;
            gdi.Init();
            gdi.SetGetItem(_buildDecJDs[Index]);
            gdi.CorrectIcon();
            gdi.Object.name = $"{Index}";
            gdi.Transform.localRotation = Quaternion.identity;
            gdi.JB.ClearEvent();
            gdi.JB.AddEvent(EventTriggerType.BeginDrag, GetBtnDragBegin);
            gdi.JB.AddEvent(EventTriggerType.Drag, GetBtnDraging);
            gdi.JB.AddEvent(EventTriggerType.EndDrag, GetBtnDragEnd);
            _itemsDec[Index] = gdi;

            var buildcount = 0;
            //var buildNum = gdi.BuildData?.Count ?? 0;
            var buildNum = 5;
            if (buildNum != 0)
            {
                foreach (var build in _builds)
                {
                    if (!build.Value.IsNull)
                    {
                        if (build.Value.BuildJd.ID == _buildDecJDs[Index].ID)
                        {
                            buildcount++;
                        }
                    }
                }
            }
            gdi.SetCount(buildcount, buildNum);
        }

        #endregion

        #region 商店
        private void ShowShop(BaseEventData arg0)
        {
            _trShopView.gameObject.SetActive(true);
            MakeShopSelect(0);
            MakeShopInSelect(0);
        }

        private void CloseShopView(BaseEventData arg0)
        {
            _trShopView.gameObject.SetActive(false);
        }

        /// <summary>
        /// 商店标签
        /// </summary>
        private void ChangeShopTag(BaseEventData arg0)
        {
            MakeShopSelect(int.Parse(arg0.selectedObject.name));
        }
        private void MakeShopSelect(int Type)
        {
            _nowShopTagIndex = Type;
            foreach (var Tag in _shopMenuTags)
            {
                Tag.MakeSelected(false);
            }
            _shopMenuTags[_nowShopTagIndex].MakeSelected(true);

            ScreenList();
            RefeshShopBuild();
        }

        private void ChangeShopInTag(BaseEventData arg0)
        {
            MakeShopInSelect(int.Parse(arg0.selectedObject.name));
        }
        private void MakeShopInSelect(int Type)
        {
            foreach (var Tag in _shopInMenuTags)
            {
                Tag.MakeSelected(false);
            }
            _shopInMenuTags[Type].MakeSelected(true);

            ShopInScreenList();
        }

        /// <summary>
        /// 数据筛选
        /// </summary>
        private void ScreenList()
        {
            _showList.Clear();
            if (_nowShopTagIndex == 0)
            {
                _showList.AddRange(_shopList);
            }
            else if (_nowShopTagIndex == 1)
            {
                _showList.AddRange(_skinList);
            }
            else if (_nowShopTagIndex == 2)
            {
                _showList.AddRange(_decList);
            }
        }
        private void ShopInScreenList()
        {

        }

        private void RefeshShopBuild()
        {
            _shopItems.Clear();
            _scrollViewShop.NewScroll(_showList.Count, OnRefeshOneShop);
        }
        private void ContinueRefeshShop()
        {
            _scrollViewShop.ContinueScroll(_showList.Count);
        }

        private void OnRefeshOneShop(Transform Tr, int Index)
        {
            GetBuildItem WHI = new GetBuildItem();
            //WHI = WHI.New(Tr);
            //WHI.SetShopItem(_showList[Index]);
            //WHI.Obj.name = Index.ToString();
            //WHI.Transform.localRotation = Quaternion.identity;
            //WHI.GetBtn.ClearEvent();
            //WHI.GetBtn.AddEvent(EventTriggerType.PointerClick, BeforeCreateBuildShop);
            _shopItems[Index] = WHI;
        }

        /// <summary>
        /// 一键装扮
        /// </summary>
        private void AllEquip(BaseEventData arg0)
        {

        }

        /// <summary>
        /// 一键购买
        /// </summary>
        private void AllBuy(BaseEventData arg0)
        {

        }

        #endregion


        #region 角色选择页面
        private void ShowChoseHero(BaseEventData arg0)
        {
            if (_trBuildInView.gameObject.activeSelf)
            {
                if (_nowBuild.BuildData.ProduceTask != null && _nowBuild.BuildData.ProduceTask.EndTime != 0)
                {
                    PopInfoCtrl.Insatnce.Show(true, "建筑工作中无法更换角色");
                    return;
                }
                ShowChoseHeroView(true,0);
            }
            else
            {
                if (_nowWorkLineItem.IsWork)
                {
                    PopInfoCtrl.Insatnce.Show(true, "建筑工作中无法更换角色");
                    return;
                }
                ShowChoseHeroView(true, 1);
            }
            
        }
        #endregion

        /// <summary>
        /// 保存当前配置
        /// </summary>
        private void SaveBuildInfo()
        {
            _ = KingdomDataMgr.Instance.SaveBuildReq(_builds, () =>
            {

            });
        }

     
        private void Update()
        {
            if (showNow)
            {
                if (_workLineMap.gameObject.activeSelf)
                {
                    for (int i = 0; i < _workLineMap.Map.Count; i++)
                    {
                        var wli = _workLineMap.Get<WorkLineItem>(i);
                        if (wli.IsWork)
                        {
                            wli.SetTime((int)GameMgr.Instance.UnixTimestamp.TotalSeconds);
                        }
                    }
                }

                if (_trWorkInfoView.gameObject.activeSelf)
                {
                    SetWorkInTime((int)GameMgr.Instance.UnixTimestamp.TotalSeconds);
                }

                if (_isMoveBuild) return;

                if (Input.touchCount == 1)
                {
                    //if (Input.GetTouch(0).phase == TouchPhase.Began || !m_IsSingleFinger)
                    //{
                    //    //在开始触摸或者从两字手指放开回来的时候记录一下触摸的位置
                    //    lastSingleTouchPosition = Input.GetTouch(0).position;
                    //}
                    //if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    //{
                    //    MoveCamera(Input.GetTouch(0).position);
                    //}
                    m_IsSingleFinger = true;
                    SetBuildCollider(true);
                }
                else if (Input.touchCount > 1)
                {
                    //当从单指触摸进入多指触摸的时候,记录一下触摸的位置
                    //保证计算缩放都是从两指手指触碰开始的
                    if (m_IsSingleFinger)
                    {
                        oldPosition1 = Input.GetTouch(0).position;
                        oldPosition2 = Input.GetTouch(1).position;
                    }

                    if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        ScaleCamera();
                    }

                    m_IsSingleFinger = false;
                    SetBuildCollider(false);
                }


                //用鼠标的
                if (useMouse)
                {
                    //Debug.LogError(Input.GetAxis("Mouse ScrollWheel"));
                    distance -= Input.GetAxis("Mouse ScrollWheel") * scaleFactor;
                    distance = Mathf.Clamp(distance, _minDistance, _maxDistance);
                }
            }

            RefeshIcon();
        }

        private void LateUpdate()
        {
            if (showNow)
            {
                //if (_isMoveBuild) return;

                //var position = m_CameraOffset;
                //_tdCamera.transform.position = position;

                _tdCamera.orthographicSize = distance;
            }
        }
    }
}

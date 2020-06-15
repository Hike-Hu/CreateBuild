using Google.Protobuf;
using JsonData;
using Luna;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.UI;

public class GetBuildItem
{
    public PLandBuild BuildData;
    public BuildJd BuildJd;
    public BuildingSkinJd BSJd;

    public JButton JB;

    public GameObject Object;
    public Transform Transform;

    protected Transform _trCostItems;
    protected List<GenericItem> _costItems;

    protected Text _btnText;

    private int _nowCount;
    private int _buildNum;

    protected Text _name;
    protected Text _count;
    protected Image _icon;
    protected Image _lock;

    public virtual void Init()
    {
        Transform = Object.transform;
        _name = Transform.Find("tex_name").GetComponent<Text>();
        _count = Transform.Find("tex_count").GetComponent<Text>();
        _icon = Transform.Find("img_icon").GetComponent<Image>();
        _lock = Transform.Find("Lock").GetComponent<Image>();

        JB = Transform.Find("bg").GetComponent<JButton>();

        BuildJd = null;
        BSJd = null;
    }

    public virtual void SetGetItem(BuildJd buildJd)
    {
        BuildJd = buildJd;
        _name.text = BuildJd.Name;
        var blvjd = JsonDataMgr.Instance.BuildLvMap[BuildJd.ID][1];
        _ = SetIcon(AssetMgr.Build_PATH, blvjd.Icon);
    }

    public async UniTask SetIcon(string pass, string pname)
    {
        var icon = await AssetMgr.Instance.LoadSprite(pass, pname);
        if (icon != null)
        {
            _icon.sprite = await AssetMgr.Instance.LoadSprite(pass, pname);
        }
    }

    public void SetCount(int nowCount,int buildNum)
    {
        _nowCount = nowCount;
        _buildNum = buildNum;
        _count.text =$"{nowCount}/{buildNum}";

        _lock.gameObject.SetActive(buildNum == 0);
    }

    public bool Canadd()
    {
        return _nowCount < _buildNum;
    }
}

public class GetFucBuildItem:GetBuildItem
{
    public override void Init()
    {
        base.Init();

        _trCostItems = Transform.Find("CostItems");
        _costItems = new List<GenericItem>();
        for (int i = 0; i < _trCostItems.childCount; i++)
        {
            GenericItem ci = new GenericItem();
            ci.Object = _trCostItems.GetChild(i).gameObject;
            ci.Init();
            _costItems.Add(ci);
        }
    }

    public override void SetGetItem(BuildJd buildJd)
    {
        base.SetGetItem(buildJd);
        foreach (var genericItem in _costItems)
        {
            genericItem.Object.SetActive(false);
        }

        JsonDataMgr.Instance.BuildLvMap.TryGetValue(int.Parse($"{BuildJd.ID}"),out var bljds);
        if (bljds == null) return;
        for (var i = 0; i < bljds[1].Recipe.Count; i++)
        {
            var ijd = JsonDataMgr.Instance.ItemMap[bljds[1].Recipe[i].Id];
            _costItems[i].SetItem(ijd, bljds[1].Recipe[i].Count);
        }
    }
}

public class GetDecBuildItem : GetBuildItem
{
    public override void Init()
    {
        base.Init();
    }

    public void CorrectIcon()
    {
        _icon.SetNativeSize();
    }
}
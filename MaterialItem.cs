using JsonData;
using Luna;
using UniRx.Async;
using UnityEngine.UI;

public class MaterialItem : Item
{
    public int ItemID;
    public ItemJd ItemJd;
    public RecipeJd RecipeJd;

    public int NeedCount;
    public override void Init()
    {
        JB = Object.GetComponent<JButton>();
        Icon = Transform.Find("img_icon").GetComponent<Image>();
        if (Transform.Find("tex_count") != null)
        {
            Count = Transform.Find("tex_count").GetComponent<Text>();
        }
        Name = Transform.Find("tex_name").GetComponent<Text>();
        NeedCount = 0;
    }

    /// <summary>
    /// 显示想要显示的个数
    /// </summary>
    /// <param name="_Ijo"></param>
    /// <param name="_Count"></param>
    public async UniTask SetItem(int id, int _Count,bool isHaveCount = true)
    {
        Object.SetActive(true);

        var iJd = JsonDataMgr.Instance.ItemMap[id];

        JsonDataMgr.Instance.RecipeMap.TryGetValue(id,out RecipeJd);

        ItemJd = iJd;
        ItemID = id;

        await SetIcon(AssetMgr.ICON_PATH, ItemJd.ID.ToString());

        UserDataMgr.Instance.NormalItemMap.TryGetValue(id, out var haveItem);
        var haveCount = haveItem?.Num ?? 0;
        NeedCount = haveCount >= _Count ? 1 : _Count - haveCount;
        if (isHaveCount)
        {
            Count.text = $"{haveCount}/{_Count}";
        }

        Name.text = ItemJd.Name;
    }
}

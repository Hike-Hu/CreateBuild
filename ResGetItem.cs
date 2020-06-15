using JsonData;
using Luna;
using UniRx.Async;
using UnityEngine.UI;

public class ResGetItem : Item
{
    private Text _speed;

    public int ItemID;
    public ItemJd ItemJd;

    public override void Init()
    {
        JB = Object.GetComponent<JButton>();
        Icon = Transform.Find("img_icon").GetComponent<Image>();
        Count = Transform.Find("tex_count").GetComponent<Text>();
        _speed = Transform.Find("tex_speed").GetComponent<Text>();
    }

    public async UniTask SetItem(ItemJd iJd)
    {
        Object.SetActive(true);

        ItemJd = iJd;
        ItemID = iJd.ID;

        await SetIcon(AssetMgr.ICON_PATH, ItemJd.ID.ToString());

        UserDataMgr.Instance.NormalItemMap.TryGetValue(ItemID, out var haveItem);
        var haveCount = haveItem?.Num ?? 0;
        Count.text = $"{haveCount}/{999999}";
    }
}

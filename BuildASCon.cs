using Spine.Unity;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;

namespace Luna
{
    public partial class BuildView : ViewCtrl
    {
        private AStarAlgorithm _aStarMap;

        private List<DoPlayer> hruns;

        private void InitASMap()
        {
            _aStarMap = new AStarAlgorithm();
            _aStarMap.SetMap(_tiles);
            hruns = new List<DoPlayer>();

            _ = SetHikeHero();
        }

        private void UniTaskHeroRun()
        {
            foreach (var hrun in hruns)
            {
                hrun.FindPath(27, 27);
            }
        }

        private async UniTask SetHikeHero()
        {
            var x = 20;
            var y = 6;
            foreach (var characterJd in JsonDataMgr.Instance.CharacterMap)
            {
                if (characterJd.Value.ID!=10013)
                {
                    var obj = await SetSpine(_trWorkHeroSps, characterJd.Value.ID);
                    obj.layer = 9;
                    obj.transform.GetChild(0).gameObject.layer = 9;
                    obj.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    obj.transform.position = new Vector3(_tiles[x][y].transform.position.x,
                        0.5f + _tiles[x][y].transform.position.y, _tiles[x][y].transform.position.z);
                    var child = obj.transform.GetChild(0);
                    var animation = child.GetComponent<SkeletonAnimation>();
                    animation.Initialize(true);
                    animation.AnimationState.SetAnimation(0, "run", true);

                    var hrun = new DoPlayer();
                    hrun.SetMap(_aStarMap);
                    hrun.mGameObject = obj;
                    hrun.mStartPos = _aStarMap.mPointGrid[x, y];
                    hruns.Add(hrun);
                    x++;
                    y++;
                }

            }
        }
    }
}

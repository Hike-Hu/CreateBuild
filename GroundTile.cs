using System.Collections.Generic;
using UnityEngine;

namespace Luna
{
    public class GroundTile:MonoBehaviour
    {
        public int X;
        public int Z;
        public int Y;
        public float Height;
        public int LockLv;
        public bool UseAble; 
        public bool MoveAble; 
        public bool Unlock;
        public bool IsErrorTile;
        public bool IsCanPutTile;
        public List<Build> UsedBuildList;
        public List<Build> CheckBuildList;
        public JButton JB;
        public SpriteRenderer SpR;
        public SpriteRenderer SpIcon;

        public void Init()
        {
            gameObject.name = $"{X}_{Z}";
            JB = gameObject.GetComponent<JButton>();
            SpR = transform.Find("Tile").GetComponent<SpriteRenderer>();
            SpIcon = transform.GetComponent<SpriteRenderer>();
            UsedBuildList = new List<Build>();
            CheckBuildList = new List<Build>();
        }

        public void AddBuild(Build build)
        {
            UsedBuildList.Add(build);
        }

        public void RemoveBuild(Build build)
        {
            UsedBuildList.Remove(build);
        }

        public void CheckError()
        {
            IsErrorTile = false;
            if (UsedBuildList.Count == 0)
            {
                return;
            }
            IsErrorTile = false;
            foreach (var thebuild in UsedBuildList)
            {
                IsErrorTile = thebuild.CheckErrorBuild();
                if (IsErrorTile)
                {
                    return;
                }
            }
        }

        public void SetfalseGT(Build build)
        {
            CheckBuildList.Clear();
            CheckBuildList.AddRange(UsedBuildList);
            CheckBuildList.Add(build);
        }

        public void CheckCanPut()
        {
            IsCanPutTile = false;
            if (CheckBuildList.Count == 0)
            {
                return;
            }
            IsCanPutTile = false;
            foreach (var thebuild in CheckBuildList)
            {
                IsCanPutTile = thebuild.CheckErrorBuild();
                if (IsCanPutTile)
                {
                    return;
                }
            }
        }

        public void SetColorByUse(bool _bool)
        {
            if (!Unlock)
            {
                SpR.color = new Color(255, 0, 0, 255);
            }
            else
            {
                if (_bool)
                {
                    SpR.color = new Color(0, 255, 0, 255);
                }
                else
                {
                    SpR.color = new Color(255, 0, 0, 255);
                }
            }
        }

        public void CheckCanUse()
        {
            if (IsErrorTile)
            {
                SetColorByUse(false);
                return;
            }
            if (!UseAble) 
            {
                SetColorByUse(false);
                return;
            }
            if (UsedBuildList.Count != 0)
            {
                SetColorByUse(UsedBuildList.Count == 1);
            }
            else
            {
                SpR.color = new Color(255, 255, 255, 255);
            }
        }

        public void CloseTile()
        {
            SpR.color = new Color(255, 255, 255, 0);
        }

        public void ResetTile()
        {
            IsErrorTile = false;
            UsedBuildList.Clear();
            SpR.color = new Color(255, 255, 255, 0);
        }

        public void SetLock()
        {
            if (!Unlock)
            {
                SpIcon.color = new Color32(183, 183, 183, 255);
            }
        }
    }
}


using UnityEngine;

namespace Luna
{
    public partial class BuildView
    {
        //摄像机距离
        public float distance = 2f;

        //缩放系数
        public float scaleFactor = 0.3f;


        private float _maxDistance = 5f;
        private float _minDistance = 3f;


        //记录上一次手机触摸位置判断用户是在左放大还是缩小手势
        private Vector2 oldPosition1;
        private Vector2 oldPosition2;


        private Vector2 lastSingleTouchPosition;

        private Vector3 m_CameraOffset;

        public bool useMouse = true;
        public bool lookObj = false;

        //定义摄像机可以活动的范围
        public float xMin = -40;
        public float xMax = -3;
        public float yMin = -10;
        public float yMax = 10;

        //这个变量用来记录单指双指的变换
        private bool m_IsSingleFinger = true;
        private void ScaleCamera()
        {
            //计算出当前两点触摸点的位置
            var tempPosition1 = Input.GetTouch(0).position;
            var tempPosition2 = Input.GetTouch(1).position;


            float currentTouchDistance = Vector3.Distance(tempPosition1, tempPosition2);
            float lastTouchDistance = Vector3.Distance(oldPosition1, oldPosition2);

            //计算上次和这次双指触摸之间的距离差距
            //然后去更改摄像机的距离
            distance -= (currentTouchDistance - lastTouchDistance) * scaleFactor * Time.deltaTime;


            //把距离限制住在min和max之间
            distance = Mathf.Clamp(distance, _minDistance, _maxDistance);


            //备份上一次触摸点的位置，用于对比
            oldPosition1 = tempPosition1;
            oldPosition2 = tempPosition2;
        }

        private void MoveCamera(Vector3 scenePos)
        {
            Vector3 lastTouchPostion =
                _tdCamera.ScreenToWorldPoint(new Vector3(lastSingleTouchPosition.x, lastSingleTouchPosition.y, -40));
            Vector3 currentTouchPosition = _tdCamera.ScreenToWorldPoint(new Vector3(scenePos.x, scenePos.y, -40));

            Vector3 v = currentTouchPosition - lastTouchPostion;
            m_CameraOffset += new Vector3(v.x, 0, v.z) * _tdCamera.transform.position.y;

            //把摄像机的位置控制在范围内
            m_CameraOffset = new Vector3(Mathf.Clamp(m_CameraOffset.x, xMin, xMax), m_CameraOffset.y,
                Mathf.Clamp(m_CameraOffset.z, yMin, yMax));
            //Debug.Log(lastTouchPostion + "|" + currentTouchPosition + "|" + v);
            lastSingleTouchPosition = scenePos;
        }

    }
}
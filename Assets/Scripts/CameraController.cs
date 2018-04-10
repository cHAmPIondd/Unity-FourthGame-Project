using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    #region Component
    private Camera m_Camera;
    #endregion
    #region PrivateAttribute
    private Transform m_Player1Transform;
    private Transform m_Player2Transform;
    private float m_AspectRatio;
    private Vector3 m_TargetPosition;
    #endregion
    #region ParameterSetting
    [SerializeField, TooltipAttribute("场景的Z深度")]
    private float m_ScenePositionZ;
    [SerializeField, TooltipAttribute("镜头最小宽度")]
    private float m_MinWidth;
    [SerializeField, Range(1, 10), TooltipAttribute("镜头跟随人物速度")]
    private int m_FollowingSpeed=5;
    [SerializeField, TooltipAttribute("镜头边缘到场景边缘的X距离")]
    private float m_CameraBorderToSceneBorderX;
    [SerializeField, TooltipAttribute("镜头边缘到场景边缘的Y距离")]
    private float m_CameraBorderToSceneBorderY;
    [SerializeField, TooltipAttribute("人物到镜头边缘的X距离")]
    private float m_PlayerToCameraBorderX;
    [SerializeField, TooltipAttribute("人物到镜头边缘的Y距离")]
    private float m_PlayerToCameraBorderY; 
    #endregion
    // Use this for initialization
    void Awake()
    {
        m_Camera = GetComponent<Camera>();
        m_AspectRatio = Screen.height / (float)Screen.width;
        m_Player1Transform = GameManager.Instance.Player1Transform;
        m_Player2Transform = GameManager.Instance.Player2Transform;

    }
    void Start()
    {
	}
	
	// Update is called once per frame
	void Update () {
        float bottom = GameManager.Instance.LevelController.Bottom + m_CameraBorderToSceneBorderY;
        float top=GameManager.Instance.LevelController.Top - m_CameraBorderToSceneBorderY;
        float left = GameManager.Instance.LevelController.Left + m_CameraBorderToSceneBorderX;
        float right = GameManager.Instance.LevelController.Right - m_CameraBorderToSceneBorderX;

        //计算宽度和高度
        float minW = m_MinWidth;
        float w = Mathf.Abs(m_Player1Transform.position.x - m_Player2Transform.position.x) ;
        
        if (m_Player1Transform.position.x - m_PlayerToCameraBorderX > left && m_Player1Transform.position.x + m_PlayerToCameraBorderX < right)
            w += m_PlayerToCameraBorderX;
        else if (m_Player1Transform.position.x - m_PlayerToCameraBorderX < left)
            w+=(m_Player1Transform.position.x-left);
        else
            w += (right-m_Player1Transform.position.x );
        if (m_Player2Transform.position.x - m_PlayerToCameraBorderX > left && m_Player2Transform.position.x + m_PlayerToCameraBorderX < right)
            w += m_PlayerToCameraBorderX;
        else if (m_Player2Transform.position.x - m_PlayerToCameraBorderX < left)
            w += (m_Player2Transform.position.x - left);
        else
            w += (right - m_Player2Transform.position.x);
        
        if (minW < w)
            minW = w;

        float minH = Mathf.Abs(m_Player1Transform.position.y - m_Player2Transform.position.y);
        if (m_Player1Transform.position.y - m_PlayerToCameraBorderY > bottom && m_Player1Transform.position.y + m_PlayerToCameraBorderY < top)
            minH += m_PlayerToCameraBorderY;
        else if (m_Player1Transform.position.y - m_PlayerToCameraBorderY < bottom)
            minH += m_Player1Transform.position.y-bottom;
        else
            minH += top-m_Player1Transform.position.y;
        if (m_Player2Transform.position.y - m_PlayerToCameraBorderY > bottom && m_Player2Transform.position.y + m_PlayerToCameraBorderY < top)
            minH += m_PlayerToCameraBorderY;
        else if (m_Player2Transform.position.y - m_PlayerToCameraBorderY < bottom)
            minH += m_Player2Transform.position.y - bottom;
        else
            minH += top - m_Player2Transform.position.y;

        if (minW * m_AspectRatio < minH)
            minW = minH / m_AspectRatio;
        else
            minH = minW * m_AspectRatio;
        //Z位置
        float z = m_ScenePositionZ - (minH / 2) / (Mathf.Tan(m_Camera.fieldOfView/2 * Mathf.Deg2Rad));

        //X Y位置
        Vector2 center=(m_Player1Transform.position+m_Player2Transform.position)/2;
        if(center.x+minW/2<right||center.x-minW/2>left)
        {
            if (center.x + minW / 2 > right)
                center.x = right - minW / 2;
            else if(center.x-minW/2<left)
                center.x = left + minW / 2;
        }
        if (center.y + minH / 2 < top || center.y - minH / 2 > bottom)
        {
            if (center.y + minH / 2 > top)
                center.y = top - minH / 2;
            else if(center.y-minH/2<bottom)
                center.y = bottom + minH / 2;
        }
        //目标位置
        m_TargetPosition = new Vector3(center.x,center.y,z);

        //
        transform.position = Vector3.Lerp(transform.position, m_TargetPosition, m_FollowingSpeed * Time.deltaTime);
	}
    void OnDrawGizmos()
    {
        LevelController level = GameObject.Find("Level").GetComponent<LevelController>();

        //画边界
        float bottom = level.Bottom;
        float top = level.Top;
        float left = level.Left;
        float right = level.Right;
        Gizmos.DrawWireCube(new Vector3((left + right) / 2, (bottom + top) / 2, m_ScenePositionZ - 2), new Vector2(right - left, top - bottom));
        bottom = level.Bottom + m_CameraBorderToSceneBorderY;
        top = level.Top - m_CameraBorderToSceneBorderY;
        left = level.Left + m_CameraBorderToSceneBorderX;
        right = level.Right - m_CameraBorderToSceneBorderX;
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(new Vector3((left + right) / 2, (bottom + top) / 2, m_ScenePositionZ - 2), new Vector2(right - left, top - bottom));
    }
    void OnDrawGizmosSelected()
    {
        m_Player1Transform = GameManager.Instance.Player1Transform;
        m_Player2Transform = GameManager.Instance.Player2Transform;
        LevelController level = GameObject.Find("Level").GetComponent<LevelController>();
        float bottom = level.Bottom + m_CameraBorderToSceneBorderY;
        float top = level.Top - m_CameraBorderToSceneBorderY;
        float left = level.Left + m_CameraBorderToSceneBorderX;
        float right = level.Right - m_CameraBorderToSceneBorderX;
        //画开始
        m_AspectRatio = Screen.height / (float)Screen.width; 
        float minW = m_MinWidth;
        float w = Mathf.Abs(m_Player1Transform.position.x - m_Player2Transform.position.x);

        if (m_Player1Transform.position.x - m_PlayerToCameraBorderX > left && m_Player1Transform.position.x + m_PlayerToCameraBorderX < right)
            w += m_PlayerToCameraBorderX;
        else if (m_Player1Transform.position.x - m_PlayerToCameraBorderX < left)
            w += (m_Player1Transform.position.x - left);
        else
            w += (right - m_Player1Transform.position.x);
        if (m_Player2Transform.position.x - m_PlayerToCameraBorderX > left && m_Player2Transform.position.x + m_PlayerToCameraBorderX < right)
            w += m_PlayerToCameraBorderX;
        else if (m_Player2Transform.position.x - m_PlayerToCameraBorderX < left)
            w += (m_Player2Transform.position.x - left);
        else
            w += (right - m_Player2Transform.position.x);

        if (minW < w)
            minW = w;

        float minH = Mathf.Abs(m_Player1Transform.position.y - m_Player2Transform.position.y);
        if (m_Player1Transform.position.y - m_PlayerToCameraBorderY > bottom && m_Player1Transform.position.y + m_PlayerToCameraBorderY < top)
            minH += m_PlayerToCameraBorderY;
        else if (m_Player1Transform.position.y - m_PlayerToCameraBorderY < bottom)
            minH += m_Player1Transform.position.y - bottom;
        else
            minH += top - m_Player1Transform.position.y;
        if (m_Player2Transform.position.y - m_PlayerToCameraBorderY > bottom && m_Player2Transform.position.y + m_PlayerToCameraBorderY < top)
            minH += m_PlayerToCameraBorderY;
        else if (m_Player2Transform.position.y - m_PlayerToCameraBorderY < bottom)
            minH += m_Player2Transform.position.y - bottom;
        else
            minH += top - m_Player2Transform.position.y;

        if (minW * m_AspectRatio < minH)
            minW = minH / m_AspectRatio;
        else
            minH = minW * m_AspectRatio;
        Vector2 center = (m_Player1Transform.position + m_Player2Transform.position) / 2;
        if (center.x + minW / 2 < right || center.x - minW / 2 > left)
        {
            if (center.x + minW / 2 > right)
                center.x = right - minW / 2;
            else if (center.x - minW / 2 < left)
                center.x = left + minW / 2;
        }
        if (center.y + minH / 2 < top || center.y - minH / 2 > bottom)
        {
            if (center.y + minH / 2 > top)
                center.y = top - minH / 2;
            else if (center.y - minH / 2 < bottom)
                center.y = bottom + minH / 2;
        }
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, new Vector3(minW, minH, m_ScenePositionZ - 2));
    }
}
